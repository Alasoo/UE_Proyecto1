using System;
using System.Threading;
using UnityEngine;
using MyExtensions;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Controller.Player;


namespace HealthSystem
{
    public class Health : MonoBehaviour
    {
        [SerializeField] protected Slider hpSlider;

        private Material mat;
        public event Action<Health> OnDie;

        private CancellationTokenSource ctsFlash;

        private const string HIT_KEY = "_hit";
        private const float flashDuration = .1f;


        void OnDestroy()
        {
            Extensions.ClearCts(ref ctsFlash);
        }


        public virtual void Init(int maxHp, Material mat)
        {
            this.mat = mat;

            hpSlider.maxValue = maxHp;
            hpSlider.value = maxHp;
        }


        public virtual void TakeDamage(int damage)
        {
            hpSlider.value = Mathf.Max(0, hpSlider.value - damage);

            ctsFlash?.Cancel();
            ctsFlash = new();
            _ = FlashEffect();

            if (hpSlider.value == 0)
                OnDie?.Invoke(this);
        }


        private async UniTaskVoid FlashEffect()
        {
            try
            {
                mat.SetInt(HIT_KEY, 1);
                await UniTask.WaitForSeconds(flashDuration, cancellationToken: ctsFlash.Token);
                mat.SetInt(HIT_KEY, 0);
            }
            catch (OperationCanceledException) { }
        }

    }

}