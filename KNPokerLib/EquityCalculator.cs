using HoldemPoker.Cards;
using HoldemPoker.Evaluator;
using KNPokerLib;

namespace KNPoker
{
    public class EquityCalculator
    {
        public static IEnumerable<int> CalcRanking(IReadOnlyCollection<HoldemHand> hands, Board board)
        {
            foreach (var hand in hands)
            {
                var boardCards = board.GetAllCards();
                Card[] fullHand = new Card[7];
                fullHand[0] = hand.Card1;
                fullHand[1] = hand.Card2;
                Array.Copy(boardCards, 0, fullHand, 2, boardCards.Length);
                yield return HoldemHandEvaluator.GetHandRanking(fullHand);
            }
        }
    }
}
