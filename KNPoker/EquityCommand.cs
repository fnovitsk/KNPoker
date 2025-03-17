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
        foreach (var group in combos.GroupBy(x => (x.hand1, x.hand2)))
        {
            int hand1winner = 0, hand2winner = 0, ties = 0;
            var numCombos = group.Count();
            var (hand1, hand2) = group.Key;
            // put (hand1, hand2) on channel
            var boards = Board.EnumerateBoards(hand1.GetAllCards().Concat(hand2.GetAllCards()));
            foreach (var board in boards)
            {
                var ranks = EquityCalculator.CalcRanking([hand1, hand2], board).ToArray();
                if (ranks[0] < ranks[1])
                    hand1winner += 1;
                else if (ranks[0] > ranks[1])
                    hand2winner += 1;
                else
                    ties += 1;
            }
            int sum = numCombos * (hand1winner + hand2winner + ties);
            Console.WriteLine(
                $"{hand1}({(double)(hand1winner * numCombos) / sum:0.000})" + " vs " +
                $"{hand2}({(double)(hand2winner * numCombos) / sum:0.000})" +
                $", {(double)(ties * numCombos)/ sum:0.000}" +
                $", {numCombos}"
                );
        }
        //TODO: add range
        return 0;
    }
}
