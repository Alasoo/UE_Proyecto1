

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
        [Header("HP")]
        [SerializeField] private TMP_Text totalEnemiesDieText;


        private PlayerStats playerStats;
        private Material mat;
        private CancellationTokenSource ctsFlash;

        private const string HIT_KEY = "_hit";
        private const float flashDuration = .1f;

        private bool inEffect = false;
        private int totalEnemiesDie = 0;



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
            hpSlider.maxValue = playerStats.maxHealth;
            hpSlider.value = playerStats.currentHealth;
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;

            totalEnemiesDieText.text = "0";
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

        private void OnTakeDamage(int damage)
        {
            hpSlider.value = playerStats.currentHealth;
            hpText.text = hpSlider.value + "/" + hpSlider.maxValue;

            if (!inEffect)
            {
                inEffect = true;
                ctsFlash?.Cancel();
                ctsFlash = new();
                _ = FlashEffect();
            }
        }

        private void OnAddExperience()
        {
            experienceText.text = playerStats.experience + "/" + playerStats.maxExperience;
            experienceSlider.value = playerStats.experience;
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
                inEffect = false;
                Extensions.ClearCts(ref ctsFlash);
            }
        }

        public void AddEnemyDie()
        {
            totalEnemiesDie++;
            totalEnemiesDieText.text = totalEnemiesDie.ToString();
        }


    }

}