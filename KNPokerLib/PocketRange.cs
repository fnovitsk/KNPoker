using System.Text.RegularExpressions;

namespace KNPokerLib;

public record PocketRange(
        IReadOnlySet<CardRank> Pairs,
        IReadOnlySet<(CardRank, CardRank)> Suited,
        IReadOnlySet<(CardRank, CardRank)> Offsuited)
{
    static private string HandPattern = @"[AKQJT98765432]{2}[os]?";
    static private string SingleHandPattern = $@"^({HandPattern})$";
    static private string HandRangePattern = $@"^({HandPattern})-({HandPattern})$";

    public static PocketRange Parse(string v)
    {
        HashSet<CardRank> pairs = [];
        HashSet<(CardRank, CardRank)> suited = [];
        HashSet<(CardRank, CardRank)> offsuited = [];
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

    private static bool TryParseNonPair(string hand, out CardRank rank1, out CardRank rank2, out bool suited)
    {
        rank1 = CardRank.Ace;
        rank2 = CardRank.Ace;
        suited = false;
        if (hand.Length != 3 || (hand[2] != 'o' && hand[2] != 's'))
            return false;
        rank1 = hand[0].ToCardRank();
        rank2 = hand[1].ToCardRank();
        suited = hand[2] == 's';
        return true;
    }

    private static bool TryParsePair(string hand, out CardRank rank)
    {
        rank = CardRank.Ace;
        if (hand.Length != 2 || hand[0] != hand[1])
            return false;
        rank = hand[0].ToCardRank();
        return true;
    }
}
