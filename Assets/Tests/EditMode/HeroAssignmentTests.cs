using Grodowladcy;
using NUnit.Framework;

namespace Grodowladcy.Tests.EditMode
{
    public class HeroAssignmentTests
    {
        [Test]
        public void Accepts_When_Offer_Equals_Threshold()
        {
            Assert.IsTrue(HeroAssignment.IsAccepted(offeredCoins: 5, hiddenCoinThreshold: 5));
        }

        [Test]
        public void Accepts_When_Offer_Exceeds_Threshold()
        {
            Assert.IsTrue(HeroAssignment.IsAccepted(offeredCoins: 6, hiddenCoinThreshold: 5));
        }

        [Test]
        public void Ignores_When_Offer_Below_Threshold()
        {
            Assert.IsFalse(HeroAssignment.IsAccepted(offeredCoins: 4, hiddenCoinThreshold: 5));
        }
    }
}
