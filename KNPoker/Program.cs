using Spectre.Console;
using KNPoker;
using KNPokerLib;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    config.AddCommand<EquityCommand>("equity")
        .WithDescription("Equity of two hands in poker.");
    config.AddCommand<GenCombosCommand>("gencombos")
        .WithDescription("Generate hand combos.");
    config.AddCommand<CreateCacheCommand>("createCache")
        .WithDescription("Create Cache of all possible combos");
});
app.Run(args);
//var range = PocketRange.Parse("AA-TT, KTs-K9s, T9o");
//AnsiConsole.Write(SpectreUtils.PocketRangeToTable(range));
