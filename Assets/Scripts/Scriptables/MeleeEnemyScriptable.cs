using Controller.Enemy; // <-- Necesitamos esto para que lea el MeleeStateMachine
using UnityEngine;

namespace EnemySystem
{
    [CreateAssetMenu(fileName = "NewMeleeEnemy", menuName = "ScriptableObjects/Enemies/Melee", order = 1)]
    public class MeleeEnemyScriptable : EnemyScriptable
    {
        [Header("MELEE COMBAT")]
        public MeleeStateMachine enemyPrefab; // <-- Hueco para arrastrar tu Prefab
        public float energyMax;

        // Sobrescribimos esto para que el spawner de oleadas sepa qué objeto crear
        public override EnemyStateMachine TakeCharacterPrefab()
        {
            return enemyPrefab;
        }
    }
}