using System;
using AudioController;
using GameSystem;
using MyUI.Panels;
using UnityEngine;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class MainGameMenuState : MenuState
    {
        [Header("MENU STATE")]
        [SerializeField] private MenuState rankingMenu;
        [SerializeField] private MenuState settingsMenu;

        [Header("BUTTONS")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button rankingButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button menuButton;


        public override void Init()
        {
            continueButton.onClick.AddListener(OnClickContinuePanel);
            rankingButton.onClick.AddListener(OnClickRankingPanel);
            settingsButton.onClick.AddListener(OnClickSettingsPanel);
            menuButton.onClick.AddListener(OnClickMenu);
        }

        private void OnClickContinuePanel()
        {
            Audios.Instance.PlayClickButton();
            OnClickBack();
        }

        private void OnClickRankingPanel()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(rankingMenu);
        }
        private void OnClickSettingsPanel()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(settingsMenu);
        }


        private void OnClickMenu()
        {
            Audios.Instance.PlayClickButton();
            _ = SceneLoader.Instance.LoadMenu();
        }




    }
}
