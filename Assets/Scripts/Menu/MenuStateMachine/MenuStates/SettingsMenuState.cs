using System;
using AudioController;
using MyUI.Panels;
using UnityEngine;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class SettingsMenuState : MenuState
    {
        [Header("MENU STATE")]
        [SerializeField] private MenuState videoMenu;
        [SerializeField] private MenuState audioMenu;
        [SerializeField] private MenuState languageMenu;

        [Header("BUTTONS")]
        [SerializeField] private Button videoButton;
        [SerializeField] private Button audioButton;
        [SerializeField] private Button languageButton;
        [Space]
        [SerializeField] private Button backButton;




        public override void Init()
        {
            videoButton.onClick.AddListener(OnClickVideo);
            audioButton.onClick.AddListener(OnClickAudio);
            languageButton.onClick.AddListener(OnClickLanguage);
            backButton.onClick.AddListener(MenuStateMachine.Instance.GoBack);
        }


        private void OnClickVideo()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(videoMenu);
        }

        private void OnClickAudio()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(audioMenu);
        }

        private void OnClickLanguage()
        {
            Audios.Instance.PlayClickButton();
            MenuStateMachine.Instance.ChangeMenu(languageMenu);
        }



    }
}
