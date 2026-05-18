using System;
using System.Collections.Generic;
using GameSystem;
using MyUI.Panels;
using SaveSystem;
using SaveSystem.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MyUI.ItemShop
{
    public class RankingMenu : MenuState
    {
        [Header("REFERENCES")]
        [SerializeField] private ScrollRect scrollRect;
        [Space]
        [SerializeField] private Button backButton;
        [Header("PREFAB")]
        [SerializeField] private RankingGame rankingGame;


        public override void Init()
        {
            backButton.onClick.AddListener(OnClickBack);

            var gameWrap = SaveLoadManager<GameWrap>.LoadData(GameWrap.GAME_KEY);
            if (gameWrap.success)
            {
                if (gameWrap.data == null || gameWrap.data.games == null || gameWrap.data.games.Count == 0) return;

                foreach (var game in gameWrap.data.games)
                {
                    RankingGame rankingClone = Instantiate(rankingGame, scrollRect.content);
                    rankingClone.Init(game);
                }

                return;
            }

        }


        protected override void OnClickBack()
        {
            //sonido?
            MenuStateMachine.Instance.GoBack();
        }




    }
}
