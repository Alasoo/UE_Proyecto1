using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.Item
{
    [Obsolete]
    [Serializable]
    public class ItemWrap
    {
        public int money = 100;
        public int healPotions = 0;
        public List<string> purchasedItems = new();
        public List<string> equipedItems = new();
    }
}
