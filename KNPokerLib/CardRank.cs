using HoldemPoker.Cards;

namespace KNPokerLib;

public enum CardRank
{
    Deuce = 0,
    Three = 1,
    Four = 2,
    Five = 3,
    Six = 4,
    Seven = 5,
    Eight = 6,
    Nine = 7,
    Ten = 8,
    Jack = 9,
    Queen = 10,
    King = 11,
    Ace = 12
}

public static class CardRankExtensions
{
    public static char ToChar(this CardRank rank) =>
        rank switch
        {
            CardRank.Ace => 'A',
            CardRank.King => 'K',
            CardRank.Queen => 'Q',
            CardRank.Jack => 'J',
            CardRank.Ten => 'T',
            CardRank.Nine => '9',
            CardRank.Eight => '8',
            CardRank.Seven => '7',
            CardRank.Six => '6',
            CardRank.Five => '5',
            CardRank.Four => '4',
            CardRank.Three => '3',
            CardRank.Deuce => '2',
            _ => throw new FormatException("Invalid card type")
        };
    public static CardRank ToCardRank(this char c) =>
        c switch
        {
            'A' => CardRank.Ace,
            'K' => CardRank.King,
            'Q' => CardRank.Queen,
            'J' => CardRank.Jack,
            'T' => CardRank.Ten,
            '9' => CardRank.Nine,
            '8' => CardRank.Eight,
            '7' => CardRank.Seven,
            '6' => CardRank.Six,
            '5' => CardRank.Five,
            '4' => CardRank.Four,
            '3' => CardRank.Three,
            '2' => CardRank.Deuce,
            _ => throw new FormatException("Invalid card type")
        };
}
