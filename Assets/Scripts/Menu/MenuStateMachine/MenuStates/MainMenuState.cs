using System;
using AudioController;
using Cysharp.Threading.Tasks;
using GameSystem;
using MyUI.Panels;
using UnityEngine;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class MainMenuState : MenuState
    {
        [Header("MENU STATE")]
        [SerializeField] private MenuState shopMenu;
        [SerializeField] private MenuState settingsMenu;

        [Header("BUTTONS")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button yesQuitButton;
        [SerializeField] private Button noQuitButton;

        [Header("CONFIRMATION PANEL")]
        [SerializeField] private GameObject confirmationPanel;


        public override void Init()
        {
            playButton.onClick.AddListener(OnClickPlayPanel);
            shopButton.onClick.AddListener(OnClickShopPanel);
            settingsButton.onClick.AddListener(OnClickSettingsPanel);
            quitButton.onClick.AddListener(OnClickQuit);
            yesQuitButton.onClick.AddListener(() => ConfirmExit(true));
            noQuitButton.onClick.AddListener(() => ConfirmExit(false));
            OpenConfirmationPanel(false);
        }

        private void OnClickPlayPanel()
        {
            Audios.Instance.PlayClickButton();
             SceneLoader.Instance.LoadGame().Forget();
        }

        private void OnClickShopPanel()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(shopMenu);
        }
        private void OnClickSettingsPanel()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(settingsMenu);
        }


        private void OnClickQuit()
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
                Application.Quit();
            else
                OpenConfirmationPanel(false);
        }


    }
}
