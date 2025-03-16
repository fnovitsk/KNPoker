using HoldemPoker.Cards;

namespace KNPokerLib;
public struct HoldemHand
{
    public Card Card1 { get; }
    public Card Card2 { get; }

    public HoldemHand(Card card1, Card card2)
    {
        if (card1 == card2)
            throw new ArgumentException("Cards cannot be the same");
        Card1 = card1;
        Card2 = card2;
    }
    public Card[] GetAllCards() => new Card[] { Card1, Card2 };
    public override string ToString()
    {
        return $"{Card1.ToShortString()}{Card2.ToShortString()}";
    }
}