using HoldemPoker.Cards;

namespace KNPokerLib;

public static class CardTypeExtensions
{
    public static char ToChar(this CardType rank) =>
        rank switch
        {
            CardType.Ace => 'A',
            CardType.King => 'K',
            CardType.Queen => 'Q',
            CardType.Jack => 'J',
            CardType.Ten => 'T',
            CardType.Nine => '9',
            CardType.Eight => '8',
            CardType.Seven => '7',
            CardType.Six => '6',
            CardType.Five => '5',
            CardType.Four => '4',
            CardType.Three => '3',
            CardType.Deuce => '2',
            _ => throw new FormatException("Invalid card type")
        };
    public static CardType ToCardType(this char c) =>
        c switch
        {
            'A' => CardType.Ace,
            'K' => CardType.King,
            'Q' => CardType.Queen,
            'J' => CardType.Jack,
            'T' => CardType.Ten,
            '9' => CardType.Nine,
            '8' => CardType.Eight,
            '7' => CardType.Seven,
            '6' => CardType.Six,
            '5' => CardType.Five,
            '4' => CardType.Four,
            '3' => CardType.Three,
            '2' => CardType.Deuce,
            _ => throw new FormatException("Invalid card type")
        };
}
