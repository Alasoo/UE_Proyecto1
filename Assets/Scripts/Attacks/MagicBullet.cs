using Controller.Player;
using UnityEngine;


namespace BulletSystem
{
    public class MagicBullet : Bullet
    {

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
            //PlayerStateMachine.Instance.Health.TakeDamage();
            BulletPool.Instance.Return(this);
        }

    }
}
