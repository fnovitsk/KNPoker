using HoldemPoker.Cards;
using KNPokerLib;
using Spectre.Console.Cli;

namespace KNPoker;

public class EquityCommand : Command<EquityCommand.Settings>
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

    public override int Execute(CommandContext context, Settings settings)
    {
        var range1 = PocketRange.Parse(settings.FirstRange);
        var range2 = PocketRange.Parse(settings.SecondRange);
        var combos = PocketRange.GenCombos(range1, range2);
        combos = PocketRange.UnifyColorsOnCombos(combos);

        Dictionary<(HoldemHand, HoldemHand), ComboResultCount> results = [];
        foreach (var combo in combos)
        {
            int hand1winner = 0, hand2winner = 0, ties = 0;
            if (results.TryGetValue((combo.hand1, combo.hand2), out var count))
            {
                count.NumOfCombos += 1;
                continue;
            }
            var boards = Board.EnumerateBoards(combo.hand1.GetAllCards().Concat(combo.hand2.GetAllCards()));
            foreach (var board in boards)
            {
                var ranks = EquityCalculator.CalcRanking([combo.hand1, combo.hand2], board).ToArray();
                if (ranks[0] < ranks[1])
                    hand1winner += 1;
                else if (ranks[0] > ranks[1])
                    hand2winner += 1;
                else
                    ties += 1;
            }
            results.Add((combo.hand1, combo.hand2), new ComboResultCount(hand1winner, hand2winner, ties) { NumOfCombos = 1 });
        }
        
        foreach (var (k, v) in results)
        {
            int sum = v.NumOfCombos * (v.Winner1 + v.Winner2 + v.Tie);
            Console.WriteLine(
                $"{k.Item1}({(double)(v.Winner1 * v.NumOfCombos) / sum:0.000})" + " vs " +
                $"{k.Item2}({(double)(v.Winner2 * v.NumOfCombos) / sum:0.000})" +
                $", {(double)v.Tie / sum:0.000}");
        }
        return 0;
    }
}
