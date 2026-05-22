

using System;
using System.Threading;
using Controller.Player;
using Cysharp.Threading.Tasks;
using MyExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HealthSystem
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("EXPERIENCE")]
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] protected Slider experienceSlider;
        [Header("LVL")]
        [SerializeField] private TMP_Text lvlText;
        [Header("HP")]
        [SerializeField] private TMP_Text hpText;
        [SerializeField] protected Slider hpSlider;

        private PlayerStats playerStats;
        private Material mat;
        private CancellationTokenSource ctsFlash;

        private const string HIT_KEY = "_hit";
        private const float flashDuration = .1f;



        public void Init(PlayerStats playerStats, Material mat)
        {
            this.playerStats = playerStats;
            this.mat = mat;

            playerStats.OnTakeDamage += OnTakeDamage;
            playerStats.OnAddHealth += OnAddHealth;
            playerStats.OnAddExperience += OnAddExperience;
            playerStats.OnAddLvl += OnAddLvl;

            experienceSlider.maxValue = playerStats.maxExperience;
            experienceSlider.value = 0;
            experienceText.text = playerStats.experience + "/" + playerStats.maxExperience;
            lvlText.text = playerStats.lvl.ToString();
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
            hpSlider.maxValue = playerStats.maxHealth;
            hpSlider.value = playerStats.maxHealth;
        }

        void OnDestroy()
        {
            playerStats.OnTakeDamage -= OnTakeDamage;
            playerStats.OnAddHealth -= OnAddHealth;
            playerStats.OnAddExperience -= OnAddExperience;
            playerStats.OnAddLvl -= OnAddLvl;
            Extensions.ClearCts(ref ctsFlash);
        }


        private void OnAddHealth()
        {
            hpSlider.maxValue = playerStats.maxHealth;
            hpSlider.value = playerStats.currentHealth;
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;
        }

        private void OnTakeDamage()
        {
            hpSlider.value = playerStats.currentHealth;
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;

            ctsFlash?.Cancel();
            ctsFlash = new();
            _ = FlashEffect();
        }

        private void OnAddExperience()
        {
            experienceText.text = playerStats.experience + "/" + playerStats.maxExperience;
        }

        private void OnAddLvl()
        {
            lvlText.text = playerStats.lvl.ToString();
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
            finally
            {
                Extensions.ClearCts(ref ctsFlash);
            }
        }


    }

}