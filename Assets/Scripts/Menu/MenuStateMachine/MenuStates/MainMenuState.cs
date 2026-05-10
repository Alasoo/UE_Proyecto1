using System;
using AudioController;
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


        public override void Init()
        {
            playButton.onClick.AddListener(OnClickPlayPanel);
            shopButton.onClick.AddListener(OnClickShopPanel);
            settingsButton.onClick.AddListener(OnClickSettingsPanel);
            quitButton.onClick.AddListener(OnClickQuit);
        }

        private void OnClickPlayPanel()
        {
            Audios.Instance.PlayClickButton();
            _ = SceneLoader.Instance.LoadGame();
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
            Application.Quit();
        }




    }
}
