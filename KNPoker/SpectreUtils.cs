using Spectre.Console.Rendering;
using Spectre.Console;
using KNPokerLib;
using HoldemPoker.Cards;

namespace KNPoker;

internal class SpectreUtils
{
    internal static Table PocketRangeToTable(PocketRange range)
    {
        CardType[] ranks = Enum.GetValues<CardType>();
        var table = new Table()
        {
            ShowHeaders = false
        };

        // Add column headers
        for (int i = 0; i < ranks.Length; i++)
            table.AddColumn(new TableColumn("").Centered());

        // Fill the table with poker hands
        for (CardType row = CardType.Ace; row >= CardType.Deuce; row--)
        {
            List<IRenderable> rowCells = [];
            for (CardType col = CardType.Ace; col >= CardType.Deuce; col--)
            {
                if (row == col)
                {
                    Decoration decoration = (range.Pairs.Contains(row)) ? Decoration.Invert: Decoration.None;
                    rowCells.Add(
                        new Markup($"{row.ToChar()}{row.ToChar()}",
                            new Style(foreground: Color.Green, decoration: decoration)));
                }
                else if (row > col)
                {
                    var decoration = (range.Suited.Contains((row, col))) ? Decoration.Invert : Decoration.None;
                    rowCells.Add(
                        new Markup($"{row.ToChar()}{col.ToChar()}s",
                            new Style(foreground: Color.Blue, decoration: decoration)));
                }
                else
                {
                    var decoration = (range.Offsuited.Contains((col, row))) ? Decoration.Invert : Decoration.None;
                    rowCells.Add(
                        new Markup($"{col.ToChar()}{row.ToChar()}o",
                            new Style(foreground: Color.Red, decoration: decoration)));
                }
            }

            table.AddRow(rowCells.ToArray());
        }

        return table;
    }
}
