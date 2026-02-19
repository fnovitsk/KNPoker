using HoldemPoker.Cards;
using KNPokerLib;
using System.Threading.Channels;

namespace KNPokerWeb.Services;

public class EquityService
{
    public record EquityResult(
        string Hand1,
        double Hand1Equity,
        string Hand2,
        double Hand2Equity,
        double TieEquity,
        int NumCombos
    );

    public record RangeEquitySummary(
        List<EquityResult> ComboResults,
        double Range1Equity,
        double Range2Equity,
        double TieEquity
    );

    public async Task<RangeEquitySummary> CalculateEquityAsync(
        string firstRange, 
        string secondRange, 
        IProgress<(int completed, int total)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var range1 = PocketRange.Parse(firstRange);
        var range2 = PocketRange.Parse(secondRange);
        var combos = PocketRange.GenCombos(range1, range2);
        combos = PocketRange.UnifyColorsOnCombos(combos);

        var groups = combos.GroupBy(x => (x.hand1, x.hand2)).ToList();
        int totalGroups = groups.Count;
        int completedGroups = 0;

        long range1Wins = 0, range2Wins = 0, totalTies = 0, totalSum = 0;
        var results = new List<EquityResult>();

        var resultChannel = Channel.CreateUnbounded<(int hand1winner, int hand2winner, int ties, int numCombos, HoldemHand hand1, HoldemHand hand2)>();

        var tasks = groups.Select(group =>
        {
            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                int hand1winner = 0, hand2winner = 0, ties = 0;
                var numCombos = group.Count();
                var (hand1, hand2) = group.Key;

                var boards = Board.EnumerateBoards(hand1.GetAllCards().Concat(hand2.GetAllCards()));
                foreach (var board in boards)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var ranks = KNPoker.EquityCalculator.CalcRanking(new[] { hand1, hand2 }, board).ToArray();
                    if (ranks[0] < ranks[1])
                        hand1winner += 1;
                    else if (ranks[0] > ranks[1])
                        hand2winner += 1;
                    else
                        ties += 1;
                }
                await resultChannel.Writer.WriteAsync((hand1winner, hand2winner, ties, numCombos, hand1, hand2), cancellationToken);

                Interlocked.Increment(ref completedGroups);
                progress?.Report((completedGroups, totalGroups));
            }, cancellationToken);
        }).ToList();

        await Task.WhenAll(tasks);
        resultChannel.Writer.Complete();

        await foreach (var result in resultChannel.Reader.ReadAllAsync(cancellationToken))
        {
            var (hand1winner, hand2winner, ties, numCombos, hand1, hand2) = result;
            int sum = numCombos * (hand1winner + hand2winner + ties);

            results.Add(new EquityResult(
                hand1.ToString(),
                (double)(hand1winner * numCombos) / sum,
                hand2.ToString(),
                (double)(hand2winner * numCombos) / sum,
                (double)(ties * numCombos) / sum,
                numCombos
            ));

            range1Wins += hand1winner * numCombos;
            range2Wins += hand2winner * numCombos;
            totalTies += ties * numCombos;
            totalSum += sum;
        }

        return new RangeEquitySummary(
            results,
            totalSum > 0 ? (double)range1Wins / totalSum : 0,
            totalSum > 0 ? (double)range2Wins / totalSum : 0,
            totalSum > 0 ? (double)totalTies / totalSum : 0
        );
    }
}
