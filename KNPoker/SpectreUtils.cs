using Spectre.Console.Rendering;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KNPokerLib;
using HoldemPoker.Cards;

namespace KNPoker;

internal class SpectreUtils
{
    internal static Table PocketRangeToTable(PocketRange range)
    {
        CardRank[] ranks = Enum.GetValues<CardRank>();
        var table = new Table()
        {
            ShowHeaders = false
        };

        // Add column headers
        for (int i = 0; i < ranks.Length; i++)
            table.AddColumn(new TableColumn("").Centered());

        // Fill the table with poker hands
        for (CardRank row = CardRank.Ace; row >= CardRank.Deuce; row--)
        {
            List<IRenderable> rowCells = [];
            for (CardRank col = CardRank.Ace; col >= CardRank.Deuce; col--)
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
