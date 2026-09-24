using Grodowladcy;
using NUnit.Framework;

namespace Grodowladcy.Tests.EditMode
{
    public class MissionResolverTests
    {
        [Test]
        public void Enemy_Strength_Matches_Prd()
        {
            Assert.AreEqual(5, MissionResolver.EnemyStrength(FieldJobType.KillMonster));
            Assert.AreEqual(3, MissionResolver.EnemyStrength(FieldJobType.SeizeVillage));
            Assert.AreEqual(2, MissionResolver.EnemyStrength(FieldJobType.DefendOwnField));
        }

        [Test]
        public void Success_When_Hero_Meets_Enemy_Strength()
        {
            Assert.IsTrue(MissionResolver.IsSuccess(heroStrength: 5, FieldJobType.KillMonster));
            Assert.IsTrue(MissionResolver.IsSuccess(heroStrength: 3, FieldJobType.SeizeVillage));
            Assert.IsTrue(MissionResolver.IsSuccess(heroStrength: 2, FieldJobType.DefendOwnField));
        }

        [Test]
        public void Failure_When_Hero_Below_Enemy_Strength()
        {
            Assert.IsFalse(MissionResolver.IsSuccess(heroStrength: 4, FieldJobType.KillMonster));
            Assert.IsFalse(MissionResolver.IsSuccess(heroStrength: 2, FieldJobType.SeizeVillage));
            Assert.IsFalse(MissionResolver.IsSuccess(heroStrength: 1, FieldJobType.DefendOwnField));
        }

        [Test]
        public void Success_Rewards_Match_Prd()
        {
            var monster = MissionResolver.SuccessReward(FieldJobType.KillMonster);
            Assert.AreEqual(3, monster.Fame);
            Assert.AreEqual(0, monster.Coins);

            var village = MissionResolver.SuccessReward(FieldJobType.SeizeVillage);
            Assert.AreEqual(0, village.Fame);
            Assert.AreEqual(5, village.Coins);

            var defense = MissionResolver.SuccessReward(FieldJobType.DefendOwnField);
            Assert.AreEqual(1, defense.Fame);
            Assert.AreEqual(0, defense.Coins);
        }
    }
}
