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

    public override int Execute(CommandContext context, Settings settings)
    {
        var range1 = PocketRange.Parse(settings.FirstRange);
        var range2 = PocketRange.Parse(settings.SecondRange);
        var combos = PocketRange.GenCombos(range1, range2);
        foreach (var combo in combos)
        {
            var boards = Board.EnumerateBoards([combo.hand1.Item1, combo.hand1.Item2, combo.hand2.Item1, combo.hand2.Item2]);
            int hand1winner = 0, hand2winner = 0, ties = 0;
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
            int sum = hand1winner + hand2winner + ties;
            Console.WriteLine($"{combo.hand1.ToShortString()}({(double) hand1winner / sum:0.000})" + " vs " +
                $"{combo.hand2.ToShortString()}({(double) hand2winner / sum:0.000})" +
                $", {(double)ties / sum:0.000}");
        }
        return 0;
    }
}
