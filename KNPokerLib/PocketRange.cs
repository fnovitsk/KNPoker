using HoldemPoker.Cards;
using MoreLinq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace KNPokerLib;

public record PocketRange
{
    public IReadOnlySet<CardType> Pairs { get; }
    public IReadOnlySet<(CardType, CardType)> Suited { get; }
    public IReadOnlySet<(CardType, CardType)> Offsuited { get; }

    public PocketRange(
        IReadOnlySet<CardType> pairs,
        IReadOnlySet<(CardType, CardType)> suited,
        IReadOnlySet<(CardType, CardType)> offsuited)
    {
        if (suited.Any(x => x.Item1 < x.Item2))
            throw new ArgumentException("Suited must be ordered",
                nameof(suited) + ": " + suited.Where(x => x.Item1 < x.Item2).First());
        if (offsuited.Any(x => x.Item1 < x.Item2))
            throw new ArgumentException("Offsuited must be ordered",
                nameof(offsuited) + ": " + offsuited.Where(x => x.Item1 < x.Item2).First());
        Pairs = pairs;
        Suited = suited;
        Offsuited = offsuited;
    }

    static private string HandPattern = @"[AKQJT98765432]{2}[os]?";
    static private string SingleHandPattern = $@"^({HandPattern})$";
    static private string HandRangePattern = $@"^({HandPattern})-({HandPattern})$";

    public static Lazy<PocketRange> All = new Lazy<PocketRange>(
        () => new PocketRange(
            Enum.GetValues<CardType>().ToHashSet(),
            Enum.GetValues<CardType>().Subsets(2).Select(x => x[0] > x[1] ? (x[0], x[1]) : (x[1], x[0])).ToHashSet(),
            Enum.GetValues<CardType>().Subsets(2).Select(x => x[0] > x[1] ? (x[0], x[1]) : (x[1], x[0])).ToHashSet()));

    public IEnumerable<HoldemHand> ToRawHands()
    {
        foreach (var rank1 in Pairs)
            foreach (var suits in Enum.GetValues<CardColor>().Subsets(2))
                yield return new HoldemHand(new Card(rank1, suits[0]), new Card(rank1, suits[1]));
        foreach (var (rank1, rank2) in Suited)
            foreach (var suit in Enum.GetValues<CardColor>().Subsets(1))
                yield return new HoldemHand(new Card(rank1, suit[0]), new Card(rank2, suit[0]));
        foreach (var (rank1, rank2) in Offsuited)
            foreach (var suits in Enum.GetValues<CardColor>().Subsets(2))
                foreach (var suit in suits.Permutations())
                    yield return new HoldemHand(new Card(rank1, suit[0]), new Card(rank2, suit[1]));
    }

    public static IEnumerable<(HoldemHand hand1, HoldemHand hand2)> GenCombos(PocketRange range1, PocketRange range2)
    {
        foreach (var h1 in range1.ToRawHands())
            foreach (var h2 in range2.ToRawHands())
            {
                if ((new HashSet<Card>(h1.GetAllCards().Concat(h2.GetAllCards())).Count != 4))
                    continue;
                yield return (h1, h2);
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

    private static bool TryParseNonPair(string str, out CardType rank1, out CardType rank2, out bool suited)
    {
        rank1 = CardType.Ace;
        rank2 = CardType.Ace;
        suited = false;
        if (str.Length != 3 || (str[2] != 'o' && str[2] != 's'))
            return false;
        rank1 = str[0].ToCardType();
        rank2 = str[1].ToCardType();
        if (rank1 < rank2)
            (rank1, rank2) = (rank2, rank1);
        suited = str[2] == 's';
        return true;
    }

    private static bool TryParsePair(string str, out CardType rank)
    {
        rank = CardType.Ace;
        if (str.Length != 2 || str[0] != str[1])
            return false;
        rank = str[0].ToCardType();
        return true;
    }

    private static CardColor[] UnifiedSuits = [CardColor.Club, CardColor.Diamond, CardColor.Heart, CardColor.Spade];
    

    private static IEnumerable<Card> UnifyColorsOnCards(IEnumerable<Card> combos)
    {
        var suitMap = new Dictionary<CardColor, int>();
        int nextColor = 0;
        foreach (var card in combos)
        {
            if (!suitMap.TryGetValue(card.Color, out var index))
            {
                index = nextColor++;
                suitMap[card.Color] = index;
            }
            yield return new Card(card.Type, UnifiedSuits[index]);
        }
    }

    public static IEnumerable<(HoldemHand hand1, HoldemHand hand2)> UnifyColorsOnCombos(IEnumerable<(HoldemHand hand1, HoldemHand hand2)> combos)
    {
        foreach (var (hand1, hand2) in combos)
        {
            var unified = UnifyColorsOnCards(hand1.GetAllCards().Concat(hand2.GetAllCards())).ToArray();
            yield return (new HoldemHand(unified[0], unified[1]), new HoldemHand(unified[2], unified[3]));
        }
    }
}
