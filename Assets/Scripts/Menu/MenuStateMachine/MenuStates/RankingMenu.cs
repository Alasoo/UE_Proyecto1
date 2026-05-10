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


        private const string ITEMSKEY = "items";

        public override void Init()
        {
            backButton.onClick.AddListener(OnClickBack);
        }


        protected override void OnClickBack()
        {
            //sonido?
            MenuStateMachine.Instance.GoBack();
        }




    }
}
