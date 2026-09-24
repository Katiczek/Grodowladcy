using Grodowladcy;
using NUnit.Framework;

namespace Grodowladcy.Tests.EditMode
{
    public class GameBalanceTests
    {
        [Test]
        public void Starting_Resources_Match_Prd()
        {
            Assert.AreEqual(10, GameConstants.StartingCoins);
            Assert.AreEqual(0, GameConstants.StartingFame);
            Assert.AreEqual(3, GameConstants.StartingSubjects);
        }

        [Test]
        public void Economy_Costs_Match_Prd()
        {
            Assert.AreEqual(3, GameConstants.FarmCost);
            Assert.AreEqual(2, GameConstants.FarmCoinsPerTurn);
            Assert.AreEqual(4, GameConstants.HouseCost);
            Assert.AreEqual(1, GameConstants.HouseSubjectBonus);
            Assert.AreEqual(3, GameConstants.HeroesPerTurn);
        }

        [Test]
        public void Win_At_Fame_20()
        {
            Assert.IsFalse(WinCondition.HasWon(19));
            Assert.IsTrue(WinCondition.HasWon(20));
            Assert.IsTrue(WinCondition.HasWon(21));
        }
    }
}
