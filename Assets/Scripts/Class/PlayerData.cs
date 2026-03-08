using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.Player
{
    [Serializable]
    public class PlayerData
    {
        public int experience = 0;
        public int lvl = 1;

        public List<int> itemsEquiped = new();


        public void AddExperience(int exp)
        {
            experience += exp;
        }
    }
}
