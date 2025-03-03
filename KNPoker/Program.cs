using Spectre.Console;
using KNPoker;
using KNPokerLib;

var range = PocketRange.Parse("AA-TT, KTs-K9s, T9o");
AnsiConsole.Write(SpectreUtils.PocketRangeToTable(range));
