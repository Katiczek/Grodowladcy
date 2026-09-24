namespace Grodowladcy
{
    public static class WinCondition
    {
        public static bool HasWon(int fame)
        {
            return fame >= GameConstants.FameToWin;
        }
    }
}
