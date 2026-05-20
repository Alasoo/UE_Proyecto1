using UnityEngine;
using Controller.Player;
using Controller.Enemy;


namespace BulletSystem
{
    public class PlayerArrowBullet : ArrowBullet
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"collision: {collision.transform.name}");
            if (!collision.TryGetComponent(out EnemyStateMachine enemy)) return;

            enemy.health.TakeDamage(physicalDamage: bulletScriptable.physicalDamage, magicalDamage: bulletScriptable.magicDamage);
            BulletPool.Instance.Return(this);
        }
    }
}
