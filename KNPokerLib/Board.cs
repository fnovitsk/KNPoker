using HoldemPoker.Cards;
using MoreLinq;
using System.Text;

namespace KNPokerLib;

public record Board
{
    public Board(Card[]? flop = null, Card? turn = null, Card? river = null)
    {
        if (river is not null && (turn is null || flop is null))
            throw new ArgumentException("Invalid board definition");
        if (turn is not null && flop is null)
            throw new ArgumentException("Invalid board definition");
        Flop = flop;
        Turn = turn;
        River = river;
    }
    public Card[]? Flop { get; }
    public Card? Turn{ get; }
    public Card? River { get; }

    public Card[] GetAllCards()
    {
        if (Flop is Card[] flop && Turn is Card turn && River is Card river)
            return new Card[] { flop[0], flop[1], flop[2], turn, river };
        throw new InvalidOperationException("Board is not fully defined");
    }

    static private Card[] FullDeck = new Card[52];
    static Board()
    {
        int index = 0;
        foreach (CardColor color in Enum.GetValues<CardColor>())
            foreach (CardType type in Enum.GetValues<CardType>())
                FullDeck[index++] = new Card(type, color);
    }

    public static IEnumerable<Board> EnumerateBoards(IEnumerable<Card> taken, Board? board = null)
    {
        var takenSet = new HashSet<Card>(taken);
        int numCardsToTake = 5;
        if (board?.Flop is Card[] flop)
        {
            numCardsToTake -= 3;
            takenSet.UnionWith(flop);
            if (board?.Turn is Card turn)
            {
                numCardsToTake -= 1;
                takenSet.Add(turn);
            }
            if (board?.River is Card river)
                throw new InvalidOperationException("Board is fully defined");
        }

        return FullDeck.Where(card => !takenSet.Contains(card))
            .Subsets(numCardsToTake)
            .Select(cards =>
            {
                var stack = new Stack<Card>(cards);
                var river = stack.Pop();
                var turn = board?.Turn ?? stack.Pop();
                var flop = board?.Flop ?? [.. stack];
                return new Board(flop, turn, river);
            });
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        if (Flop is not null)
            builder.Append($"{Flop[0].ToShortString()}{Flop[1].ToShortString()}{Flop[2].ToShortString()}");
        else
            builder.Append("---");
        if (Turn is Card turn)
            builder.Append($" {turn.ToShortString()}");
        else
            builder.Append(" -");
        if (River is Card river)
            builder.Append($" {river.ToShortString()}");
        else
            builder.Append(" -");
        return builder.ToString();
    }
}
