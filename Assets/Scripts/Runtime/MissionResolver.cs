namespace Grodowladcy
{
    public readonly struct MissionReward
    {
        public readonly int Fame;
        public readonly int Coins;

        public MissionReward(int fame, int coins)
        {
            Fame = fame;
            Coins = coins;
        }
    }

    /// <summary>
    /// Rozstrzygnięcie misji: siła bohatera vs siła wroga.
    /// MVP: sukces gdy siła bohatera &gt;= siła wroga.
    /// </summary>
    public static class MissionResolver
    {
        public static int EnemyStrength(FieldJobType jobType)
        {
            switch (jobType)
            {
                case FieldJobType.KillMonster:
                    return GameConstants.MonsterStrength;
                case FieldJobType.SeizeVillage:
                    return GameConstants.VillageStrength;
                case FieldJobType.DefendOwnField:
                    return GameConstants.OwnFieldDefenseStrength;
                default:
                    return 0;
            }
        }

        public static bool IsSuccess(int heroStrength, FieldJobType jobType)
        {
            return heroStrength >= EnemyStrength(jobType);
        }

        public static MissionReward SuccessReward(FieldJobType jobType)
        {
            switch (jobType)
            {
                case FieldJobType.KillMonster:
                    return new MissionReward(GameConstants.MonsterFameReward, 0);
                case FieldJobType.SeizeVillage:
                    return new MissionReward(0, GameConstants.VillageCoinReward);
                case FieldJobType.DefendOwnField:
                    return new MissionReward(GameConstants.DefenseFameReward, 0);
                default:
                    return new MissionReward(0, 0);
            }
        }
    }
}
