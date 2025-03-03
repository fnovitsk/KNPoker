using CsCheck;
using KNPokerLib;
using Microsoft.VisualBasic;
using Shouldly;

namespace KNPokerTests;

[TestClass]
public sealed class PocketRangeTest
{
    [TestMethod]
    public void TestParsingGood()
    {
        PocketRange.Parse("  AA-TT\t , K8s-K9s, T2o-T2o").ShouldSatisfyAllConditions(
            r => r.Pairs.Count.ShouldBe(5),
            r => r.Suited.Count.ShouldBe(2),
            r => r.Offsuited.Count.ShouldBe(1),
            r => r.Pairs.ShouldContain(CardRank.Ace)
            );
    }

    [TestMethod]    
    public void TestParsingBad()
    {
        Should.Throw<InvalidDataException>(() => PocketRange.Parse("AA-K8s,")).Message.ShouldBe("Invalid range: AA-K8s");
        Should.Throw<InvalidDataException>(() => PocketRange.Parse("AKs-K2s,")).Message.ShouldBe("Invalid range: AKs-K2s");
        Should.Throw<InvalidDataException>(() => PocketRange.Parse("AKs-A2s,kk")).Message.ShouldBe("Invalid range: kk");
        Should.Throw<InvalidDataException>(() => PocketRange.Parse("AKs-A2o,kk")).Message.ShouldBe("Invalid range: AKs-A2o");
    }

    private static Gen<string> MakeGen()
    {
        var cardRankGet = Gen.Enum<CardRank>();
        var suitgen = Gen.OneOfConst(new[] { 's', 'o' });
        var goodhandgen = cardRankGet
            .Select(cardRankGet)
            .Select((CardRank rank1, CardRank rank2) =>
            {
                if (rank1 < rank2)
                    return (rank2, rank1);
                return (rank1, rank2);
            })
            .SelectMany(((CardRank rank1, CardRank rank2) ranks) =>
            {
                if (ranks.rank1 == ranks.rank2)
                    return Gen.Const($"{ranks.rank1.ToChar()}{ranks.rank2.ToChar()}");
                return suitgen.Select(suit => $"{ranks.rank1.ToChar()}{ranks.rank2.ToChar()}{suit}");
            });
        var badhandgen = Gen.String[0, 5];
        badhandgen = Gen.Frequency((95, goodhandgen), (5, badhandgen));
        var rangegen = badhandgen.SelectMany((string hand1) => goodhandgen.Select((string hand2) => $"{hand1}-{hand2}"));
        var singlehandgen = badhandgen.SelectMany((string hand) => Gen.Const(hand));
        var itemgen = Gen.Frequency((33, rangegen), (67, singlehandgen));
        var separatorgen = (int len) => Gen.OneOfConst(new[] { ' ', '\t' })
            .List[0, 3]
            .SelectMany(l => Gen.Shuffle(l.Append(',').ToList()).Select(l => string.Concat(l)))
            .Array[len];
        var gen = itemgen
            .Array[0, 10]
            .SelectMany(items =>
            {
                if (items.Length == 0)
                    return Gen.Const("");
                return separatorgen(items.Length - 1).Select(seps =>
                    string.Concat(
                        items
                        .SkipLast(1)
                        .Zip(seps, (string item, string sep) => $"{item}{sep}")) + items.Last()
                    );
            });
        return gen;
    }
}
