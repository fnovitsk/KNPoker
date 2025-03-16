using HoldemPoker.Cards;
using MoreLinq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace KNPokerLib;

public record PocketRange(
        IReadOnlySet<CardType> Pairs,
        IReadOnlySet<(CardType, CardType)> Suited,
        IReadOnlySet<(CardType, CardType)> Offsuited)
{
    static private string HandPattern = @"[AKQJT98765432]{2}[os]?";
    static private string SingleHandPattern = $@"^({HandPattern})$";
    static private string HandRangePattern = $@"^({HandPattern})-({HandPattern})$";

    public IEnumerable<(Card, Card)> ToRawHands()
    {
        foreach (var rank1 in Pairs)
            foreach (var suits in Enum.GetValues<CardColor>().Subsets(2))
                foreach (var suit in suits.Permutations())
                    yield return (new Card(rank1, suit[0]), new Card(rank1, suit[1]));
        foreach (var (rank1, rank2) in Suited)
            foreach (var suit in Enum.GetValues<CardColor>().Subsets(1))
                yield return (new Card(rank1, suit[0]), new Card(rank2, suit[0]));
        foreach (var (rank1, rank2) in Offsuited)
            foreach (var suits in Enum.GetValues<CardColor>().Subsets(2))
                foreach (var suit in suits.Permutations())
                    yield return (new Card(rank1, suit[0]), new Card(rank2, suit[1]));
    }

    public static IEnumerable<((Card, Card) hand1, (Card, Card) hand2)> GenCombos(PocketRange range1, PocketRange range2)
    {
        foreach (var h1 in range1.ToRawHands())
            foreach (var h2 in range2.ToRawHands())
            {
                if ((new HashSet<Card> { h1.Item1, h1.Item2, h2.Item1, h2.Item2 }).Count != 4)
                    continue;
                yield return (h1, h2);
            }
    }

    private static CardColor[] UnifiedSuits = [CardColor.Club, CardColor.Diamond, CardColor.Heart, CardColor.Spade];

    private static IEnumerable<Card> UnifyCombos(IEnumerable<Card> combos)
    {
        var suitMap = new Dictionary<CardColor, int>();
        int nextColor = 0;
        foreach (var card in combos) {
            if (!suitMap.TryGetValue(card.Color, out var index))
            {
                index = nextColor++;
                suitMap[card.Color] = index;
            }
            yield return new Card(card.Type, UnifiedSuits[index]);
        }
    }

    public static PocketRange Parse(string v)
    {
        HashSet<CardType> pairs = [];
        HashSet<(CardType, CardType)> suited = [];
        HashSet<(CardType, CardType)> offsuited = [];
        void ParseHand(string hand)
        {
            if (TryParsePair(hand, out var rank))
            {
                pairs.Add(rank);
                return;
            }

            if (TryParseNonPair(hand, out var rank1, out var rank2, out var isSuited))
            {
                if (isSuited)
                    suited.Add((rank1, rank2));
                else
                    offsuited.Add((rank1, rank2));
                return;
            }
            throw new InvalidDataException($"Invalid hand: {hand}");
        }

        void ParseRange(string hand1, string hand2)
        {
            if (TryParsePair(hand1, out var rank1) && TryParsePair(hand2, out var rank2))
            {
                var startRank = rank1;
                var endRank = rank2;
                if (endRank < startRank)
                    (startRank, endRank) = (endRank, startRank);
                for (var rank = startRank; rank <= endRank; rank++)
                    pairs.Add(rank);
                return;
            }
            if (TryParseNonPair(hand1, out var rank11, out var rank12, out var isSuited1) && 
                TryParseNonPair(hand2, out var rank21, out var rank22, out var isSuited2))
            {
                if (isSuited1 == isSuited2 && rank11 == rank21)
                {
                    var startRank2 = rank12;
                    var endRank2 = rank22;
                    if (endRank2 < startRank2)
                        (startRank2, endRank2) = (endRank2, startRank2);
                    for (rank2 = startRank2; rank2 <= endRank2; rank2++)
                        if (isSuited1)
                            suited.Add((rank11, rank2));
                        else
                            offsuited.Add((rank11, rank2));
                    return;
                }
            }
            throw new InvalidDataException($"Invalid range: {hand1}-{hand2}");
        }

        if (v.Trim().Length != 0)
            foreach (var s in v.Split(','))
            {
                var trimmed = s.Trim();
                var m = Regex.Match(trimmed, HandRangePattern);
                if (m.Success)
                {
                    ParseRange(m.Groups[1].Value, m.Groups[2].Value);
                    continue;
                }
                m = Regex.Match(trimmed, SingleHandPattern);
                if (m.Success)
                {
                    ParseHand(m.Groups[1].Value);
                    continue;
                }
                throw new InvalidDataException($"Invalid range: {s}");
            }
        return new PocketRange(pairs, suited, offsuited);
    }

    private static bool TryParseNonPair(string hand, out CardType rank1, out CardType rank2, out bool suited)
    {
        rank1 = CardType.Ace;
        rank2 = CardType.Ace;
        suited = false;
        if (hand.Length != 3 || (hand[2] != 'o' && hand[2] != 's'))
            return false;
        rank1 = hand[0].ToCardType();
        rank2 = hand[1].ToCardType();
        if (rank1 < rank2)
            (rank1, rank2) = (rank2, rank1);
        suited = hand[2] == 's';
        return true;
    }

    private static bool TryParsePair(string hand, out CardType rank)
    {
        rank = CardType.Ace;
        if (hand.Length != 2 || hand[0] != hand[1])
            return false;
        rank = hand[0].ToCardType();
        return true;
    }
}
