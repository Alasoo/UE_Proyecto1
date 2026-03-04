using System;
using UnityEngine;

namespace SaveSystem.Player
{
    [Serializable]
    public class PlayerData
    {
        public int experience = 0;
        public int lvl = 1;


        public void AddExperience(int exp)
        {
            experience += exp;
        }
    }
}
