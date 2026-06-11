using EnemySystem;
using UnityEngine;

namespace Controller.Enemy
{
    public class MeleeStateMachine : EnemyStateMachine
    {
        public override void Init(EnemyScriptable enemyScriptable)
        {
            base.Init(enemyScriptable);

            // Empieza directamente persiguiendo al jugador, igual que el arquero
            SwitchState(new EnemyFollowPlayerState(this));
        }
    }
}