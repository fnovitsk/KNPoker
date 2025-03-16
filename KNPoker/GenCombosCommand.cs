using KNPokerLib;
using Spectre.Console;
using Spectre.Console.Cli;

namespace KNPoker;

public class GenCombosCommand : Command<GenCombosCommand.Settings>
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
        foreach (var hand in range1.ToRawHands())
        {
            Console.WriteLine($"{hand}");
        }
        foreach (var hand in range2.ToRawHands())
        {
            Console.WriteLine($"{hand}");
        }
        return 0;
    }
}
