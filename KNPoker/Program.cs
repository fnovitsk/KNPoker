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
//var range = PocketRange.Parse("22-AA,A2s-AKs,K2s-KQs,Q2s-QJs,J2s-JTs,T2s-T9s,92s-98s,82s-87s,72s-76s,62s-65s,52s-54s,42s-43s,32s,A2o-AKo,K2o-KQo,Q2o-QJo,J2o-JTo,T2o-T9o,92o-98o,82o-87o,72o-76o,62o-65o,52o-54o,42o-43o,32o");
//AnsiConsole.Write(SpectreUtils.PocketRangeToTable(range));
