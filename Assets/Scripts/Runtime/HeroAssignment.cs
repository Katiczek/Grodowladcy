namespace Grodowladcy
{
    /// <summary>
    /// Ukryty krok przydziału: oferta monet vs ukryty próg monet bohatera.
    /// MVP: wyłącznie te dwa wejścia; przyjęcie gdy oferta &gt;= próg.
    /// </summary>
    public static class HeroAssignment
    {
        public static bool IsAccepted(int offeredCoins, int hiddenCoinThreshold)
        {
            return offeredCoins >= hiddenCoinThreshold;
        }
    }
}
