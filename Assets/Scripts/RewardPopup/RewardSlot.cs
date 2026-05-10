using System;
using System.Collections.Generic;
using Controller.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RewardSystem
{
    public class RewardSlot : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Button button;
        [Space]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text lastValueText;
        [SerializeField] private TMP_Text newValueText;

        private RewardScriptable reward;
        private PlayerStats playerStats;

        void Awake()
        {
            newValueText.color = Color.green;
            button.onClick.AddListener(OnClick);
        }

        void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }


        public void Setup(RewardScriptable reward, PlayerStats playerStats)
        {
            this.reward = reward;
            this.playerStats = playerStats;

            titleText.text = reward.localization.GetLocalizedString();
            var data = reward.ShowReward(playerStats);
            lastValueText.text = data.lastValue;
            newValueText.text = data.newValue;
        }

        private void OnClick()
        {
            reward.SelectReward(playerStats);
            RewardPopup.Instance.Close();

            reward = null;
            playerStats = null;
        }
    }
}
