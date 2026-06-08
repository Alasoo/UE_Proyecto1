using System;
using System.Collections;
using System.Threading;
using Controller.Player;
using Cysharp.Threading.Tasks;
using MyExtensions;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace BulletSystem
{
    public abstract class Bullet : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected TrailRenderer trail;
        [SerializeField] protected Light2D light2D;
        [SerializeField] protected bool scaleBulletByTime = false;

        protected BulletScriptable bulletScriptable;
        protected Vector3 moveDirection;

        private CancellationTokenSource ctsMove;
        protected bool hasHit = false;
        public bool IsReleased { get; private set; } = false;



        void OnDestroy()
        {
            Extensions.ClearCts(ref ctsMove);
        }

        public virtual void OnRelease()
        {
            if (IsReleased) return;

            IsReleased = true;
            Extensions.ClearCts(ref ctsMove);
            hasHit = false;

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
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
            moveDirection = direction.normalized;
            IsReleased = false;
            hasHit = false;

            ctsMove?.Cancel();
            ctsMove = new();
            try
            {
                _ = MoveLoop();
                _ = LifeTimeAsync();
            }
            catch (OperationCanceledException) { }

        }


        public virtual void Init(BulletScriptable bulletScriptable)
        {
            this.bulletScriptable = bulletScriptable;
            var colors = bulletScriptable.TakeColor();
            spriteRenderer.color = colors.startColor;
            spriteRenderer.sprite = bulletScriptable.sprite;
            if (trail != null)
            {
                trail.startColor = colors.startColor;
                trail.endColor = colors.endColor;
            }
            if (light2D != null)
                light2D.color = colors.endColor;
        }




        private async UniTaskVoid MoveLoop()
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            while (!ctsMove.Token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ctsMove.Token);
                transform.Translate(moveDirection * bulletScriptable.speed * Time.deltaTime, Space.World);
            }
        }

        private async UniTaskVoid LifeTimeAsync()
        {
            if (scaleBulletByTime)
                _ = transform.LerpScale(Vector3.zero, bulletScriptable.lifeTime, ctsMove.Token);
            await UniTask.Delay(System.TimeSpan.FromSeconds(bulletScriptable.lifeTime), cancellationToken: ctsMove.Token);
            BulletPool.Instance.Return(this);
        }


        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (transform.name == "Bullet1")
            {
                Debug.Log($"Bala colisiona con: {collision.transform.name}");
            }
            if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
            if (hasHit || IsReleased) return;

            hasHit = true;
            PlayerStateMachine.Instance.playerStats.TakeDamage(physicalDamage: bulletScriptable.physicalDamage, magicalDamage: bulletScriptable.magicDamage);
            BulletPool.Instance.Return(this);
        }
    }
}
