using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyExtensions;
using UnityEngine;


namespace BulletSystem
{
    public abstract class Bullet : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected TrailRenderer trail;

        protected BulletScriptable bulletScriptable;
        protected Vector3 moveDirection;

        private CancellationTokenSource ctsMove;



        void OnDestroy()
        {
            ctsMove?.ClearCts();
            ctsMove = null;
        }

        public virtual void OnRelease()
        {
            ctsMove?.ClearCts();
            ctsMove = null;

            if (trail != null) trail.Clear();

            gameObject.SetActive(false);
        }



        public virtual void OnGet(Vector3 direction, Vector3 position)
        {
            if (bulletScriptable == null)
            {
                BulletPool.Instance.Return(this);
                return;
            }

            transform.position = position;
            gameObject.SetActive(true);
            moveDirection = direction.normalized;

            ctsMove = new();
            _ = MoveLoop();
            _ = LifeTimeAsync();
        }


        public virtual void Init(BulletScriptable bulletScriptable)
        {
            this.bulletScriptable = bulletScriptable;
            var colors = bulletScriptable.TakeColor();
            spriteRenderer.color = colors.startColor;
            trail.startColor = colors.startColor;
            trail.endColor = colors.endColor;
        }




        private async UniTaskVoid MoveLoop()
        {
            while (!ctsMove.Token.IsCancellationRequested)
            {
                transform.Translate(moveDirection * bulletScriptable.speed * Time.deltaTime, Space.World);

                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                await UniTask.Yield(PlayerLoopTiming.Update, ctsMove.Token);
            }
        }

        private async UniTaskVoid LifeTimeAsync()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(bulletScriptable.lifeTime), cancellationToken: ctsMove.Token);
            BulletPool.Instance.Return(this);
        }

    }
}
