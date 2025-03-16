using HoldemPoker.Cards;

namespace KNPokerLib;

public static class CardExtensions
{
    public static string ToShortString(this CardType cardType)
    {
        if (CardType.Deuce <= cardType && cardType <= CardType.Nine)
            return (2 + cardType - CardType.Deuce).ToString();
        return cardType.ToString()[0].ToString();
    }
    public static string ToShortString(this CardColor card) => Char.ToLower(card.ToString()[0]).ToString();
    public static string ToShortString(this Card card) => card.Type.ToShortString() + card.Color.ToShortString();
}
