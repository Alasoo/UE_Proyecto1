using UnityEngine;
using Controller.Player;
using Controller.Enemy;


namespace BulletSystem
{
    public class PlayerArrowBullet : ArrowBullet
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasHit || IsReleased) return;
            if (!collision.TryGetComponent(out EnemyStateMachine enemy)) return;

            enemy.Stuned(PlayerStateMachine.Instance.playerStats.stunTime);
            hasHit = true;
            enemy.health.TakeDamage(physicalDamage: bulletScriptable.physicalDamage + PlayerStateMachine.Instance.playerStats.currentDamage, magicalDamage: bulletScriptable.magicDamage);
            BulletPool.Instance.Return(this);
        }
    }
}
