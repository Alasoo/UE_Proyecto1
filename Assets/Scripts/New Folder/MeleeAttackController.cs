using AudioController;
using Controller.Player;
using EnemySystem;
using GameSystem;
using UnityEngine;

namespace AttackSystem
{
    public class MeleeAttackController : AttackController
    {
        public override void Attack(EnemyScriptable enemyScriptable, Vector3 direction, Vector3 position)
        {
            // 1. Comprobamos la distancia REAL en el frame exacto del impacto
            float currentDistance = Vector3.Distance(transform.position, PlayerStateMachine.Instance.transform.position);

            // 2. Si el jugador ha huido y está más lejos que el rango de ataque, el golpe falla
            if (currentDistance > enemyScriptable.rangeAttack)
            {
                // El jugador lo esquivó, no hacemos daño
                return;
            }

            // 3. Si sigue estando cerca, aplicamos el daño escalado
            float difficulty = GameManager.Instance.GetDifficultyMultiplier();
            int scaledPhysical = Mathf.RoundToInt(enemyScriptable.physicalDamage * difficulty);
            int scaledMagical = Mathf.RoundToInt(enemyScriptable.magicalDamage * difficulty);

            PlayerStateMachine.Instance.playerStats.TakeDamage(physicalDamage: scaledPhysical, magicalDamage: scaledMagical);

            if (soundAttack != null)
            {
                Audios.Instance.PlayEffect(soundAttack);
            }
        }

        public override void StopAttack()
        {
            Debug.Log("Stop ataque melee!");
        }
    }
}