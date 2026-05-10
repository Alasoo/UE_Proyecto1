using System;
using System.Collections.Generic;
using Controller.Player;
using UnityEngine;

namespace RewardSystem
{
    public class RewardPopup : Singleton<RewardPopup>
    {
        [Header("REFERENCES")]
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private List<RewardSlot> rewardSlots = new();

        [Header("DEBUG")]
        public List<RewardScriptable> rewards = new();




        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        public void Init()
        {
            RewardScriptable[] loadedRewards = Resources.LoadAll<RewardScriptable>("Rewards");
            rewards = new List<RewardScriptable>(loadedRewards);

            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }



        public void Open(PlayerStats playerStats)
        {
            Time.timeScale = 0;
            foreach (var rewardSlot in rewardSlots)
            {
                RewardScriptable randomReward = rewards[UnityEngine.Random.Range(0, rewards.Count)];
                rewardSlot.Setup(randomReward, playerStats);
            }

            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        public void Close()
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            Time.timeScale = 1;
        }


    }
}
