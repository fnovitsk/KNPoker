using HoldemPoker.Cards;
using KNPokerLib;
using Spectre.Console.Cli;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Channels;

namespace KNPoker;

public class EquityCommand : AsyncCommand<EquityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<firstRange>")]
        public required string FirstRange { get; set; }

        [CommandArgument(1, "<secondRange>")]
        public required string SecondRange { get; set; }
    }

    private record ComboResultCount(int Winner1, int Winner2, int Tie)
    {
        public int NumOfCombos { get; set; }
    };

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var range1 = PocketRange.Parse(settings.FirstRange);
        var range2 = PocketRange.Parse(settings.SecondRange);
        var combos = PocketRange.GenCombos(range1, range2);
        combos = PocketRange.UnifyColorsOnCombos(combos);
        long range1Wins = 0, range2Wins = 0, totalTies = 0, totalSum = 0;
        var resultChannel = Channel.CreateUnbounded<(int hand1winner, int hand2winner, int ties, int numCombos, HoldemHand hand1, HoldemHand hand2)>();
        var tasks = combos.GroupBy(x => (x.hand1, x.hand2)).Select(group =>
        {
            return Task.Run(async () =>
            {
                int hand1winner = 0, hand2winner = 0, ties = 0;
                var numCombos = group.Count();
                var (hand1, hand2) = group.Key;

                var boards = Board.EnumerateBoards(hand1.GetAllCards().Concat(hand2.GetAllCards()));
                foreach (var board in boards)
                {
                    var ranks = EquityCalculator.CalcRanking(new[] { hand1, hand2 }, board).ToArray();
                    if (ranks[0] < ranks[1])
                        hand1winner += 1;
                    else if (ranks[0] > ranks[1])
                        hand2winner += 1;
                    else
                        ties += 1;
                }
                await resultChannel.Writer.WriteAsync((hand1winner, hand2winner, ties, numCombos, hand1, hand2));
            });
        }).ToList();
        await Task.WhenAll(tasks);
        resultChannel.Writer.Complete();

        await foreach (var result in resultChannel.Reader.ReadAllAsync())
        {
            var (hand1winner, hand2winner, ties, numCombos, hand1, hand2) = result;
            int sum = numCombos * (hand1winner + hand2winner + ties);

            Console.WriteLine(
                $"{hand1}({(double)(hand1winner * numCombos) / sum:0.000})" + " vs " +
                $"{hand2}({(double)(hand2winner * numCombos) / sum:0.000})" +
                $", {(double)(ties * numCombos) / sum:0.000}" +
                $", {numCombos}"
            );

            range1Wins += hand1winner * numCombos;
            range2Wins += hand2winner * numCombos;
            totalTies += ties * numCombos;
            totalSum += sum;
        }
        Console.WriteLine(
            $"Range 1 Wins:({(double)(range1Wins) / totalSum:0.000})" +
            $"Range 2 Wins:({(double)(range2Wins) / totalSum:0.000})" +
            $"Ranges Tie:({(double)(totalTies) / totalSum:0.000})"
        );

        return 0;
    }
}
