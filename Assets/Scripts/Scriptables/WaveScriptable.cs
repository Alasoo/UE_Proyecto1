using System;
using System.Collections.Generic;
using UnityEngine;


namespace EnemySystem
{
    [CreateAssetMenu(fileName = "NuevaOleada", menuName = "ScriptableObjects/Oleada", order = 5)]
    public class WaveScriptable : ScriptableObject
    {
        public List<WaveData> waveData = new();
    }

    [Serializable]
    public class WaveData
    {
        public EnemyScriptable enemyScriptable;
        public int minEnemies;
        public int maxEnemies;
        public int secondToSpawn;
        [NonSerialized] public bool spawned;
    }


}


