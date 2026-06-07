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

        [SerializeField] private Button yesQuitButton;
        [SerializeField] private Button noQuitButton;

        [Header("CONFIRMATION PANEL")]
        [SerializeField] private GameObject confirmationPanel;


        public override void Init()
        {
            continueButton.onClick.AddListener(OnClickContinuePanel);
            rankingButton.onClick.AddListener(OnClickRankingPanel);
            settingsButton.onClick.AddListener(OnClickSettingsPanel);
            menuButton.onClick.AddListener(OnClickMenu);

            yesQuitButton.onClick.AddListener(() => ConfirmExit(true));
            noQuitButton.onClick.AddListener(() => ConfirmExit(false));
            OpenConfirmationPanel(false);
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
            OpenConfirmationPanel(true);
        }



        private void OpenConfirmationPanel(bool open)
        {
            confirmationPanel.SetActive(open);
        }


        private void ConfirmExit(bool confirm)
        {
            Audios.Instance.PlayClickButton();
            if (confirm)
                _ = SceneLoader.Instance.LoadMenu();
            else
                OpenConfirmationPanel(false);
        }



    }
}
