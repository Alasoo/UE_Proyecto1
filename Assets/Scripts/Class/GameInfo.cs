

using System;
using System.Collections.Generic;


namespace GameSystem
{
    [Serializable]
    public class GameWrap
    {
        [NonSerialized] public static readonly string GAME_KEY = "GameWrap";
        public List<GameInfo> games = new();
    }

    [Serializable]
    public class GameInfo
    {
        public DateTime gameDate;
        public int totalSeconds;


        public GameInfo()
        {
            gameDate = DateTime.Now;
        }

        public GameInfo(int totalSeconds)
        {
            gameDate = DateTime.Now;
            this.totalSeconds = totalSeconds;
        }
    }
}