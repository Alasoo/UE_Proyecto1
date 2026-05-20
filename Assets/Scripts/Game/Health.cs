using System;
using System.Threading;
using UnityEngine;
using MyExtensions;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


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

        private bool inEffect = false;

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


        public virtual void TakeDamage(int physicalDamage = 0, int magicalDamage = 0)
        {
            int totalDamage = physicalDamage + magicalDamage;
            hpSlider.value = Mathf.Max(0, hpSlider.value - totalDamage);

            if (!inEffect)
            {
                ctsFlash?.Cancel();
                ctsFlash = new();
                _ = FlashEffect();
            }

            if (hpSlider.value == 0)
                OnDie?.Invoke(this);
        }


        private async UniTaskVoid FlashEffect()
        {
            try
            {
                inEffect = true;
                mat.SetInt(HIT_KEY, 1);
                await UniTask.WaitForSeconds(flashDuration, cancellationToken: ctsFlash.Token);
                mat.SetInt(HIT_KEY, 0);
                inEffect = false;
            }
            catch (OperationCanceledException) { }
        }



        public void ClearSubscriptions()
        {
            OnDie = null;
        }

    }

}