using Controller.Player;
using UnityEngine;


namespace BulletSystem
{
    public class MagicBullet : Bullet
    {

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
            PlayerStateMachine.Instance.health.TakeDamage(10);
            BulletPool.Instance.Return(this);
        }

    }
}
