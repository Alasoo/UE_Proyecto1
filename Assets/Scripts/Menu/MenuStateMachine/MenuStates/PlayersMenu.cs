using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem;
using MyUI.Panels;
using MyUI.PlayerSelection;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class PlayersMenu : MenuState
    {
        [Header("REFERENCES")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private LocalizeStringEvent title;
        [SerializeField] private LocalizeStringEvent description;
        [Space]
        [SerializeField] private Button playButton;
        [SerializeField] private Button backButton;
        [Space]
        [SerializeField] private GameObject infoPanel;
        [Space]
        [SerializeField] private Slider sliderExp;
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] private TMP_Text lvlText;

        [Header("PREFAB")]
        [SerializeField] private PlayerButton playerButton;


        [Header("SCENE INDEX")]
        [SerializeField] private int sceneIndex = 1;

        private PlayerButton currentPlayerSelected = null;
        private List<PlayerButton> playerButtons = new();


        public override void Init()
        {
            playButton.onClick.AddListener(OnClickPlay);
            backButton.onClick.AddListener(OnClickBack);

            playButton.interactable = false;
            CreateButtons();
            UpdatePlayerInfo();
        }

        private void OnClickPlay()
        {
            GameInfo.playerSelected = currentPlayerSelected.playerScriptable;
            Debug.Log($"Cambio de escena, a jugar");
            _ = SceneLoader.Instance.LoadGame();
        }

        protected override void OnClickBack()
        {
            //sonido?
            GameInfo.playerSelected = null;
            OnClickPlayerButton(null);
            MenuStateMachine.Instance.GoBack();
        }

        private void CreateButtons()
        {
            PlayerDataScriptable[] playersScriptable = Resources.LoadAll<PlayerDataScriptable>("Players");

            foreach (var playerScriptable in playersScriptable)
            {
                playerScriptable.LoadPlayerData();
                PlayerButton clone = Instantiate(playerButton, scrollRect.content);
                clone.Init(playerScriptable, this);
                playerButtons.Add(clone);
            }
        }

        public void OnClickPlayerButton(PlayerButton playerButton)
        {
            currentPlayerSelected?.Deselect();
            currentPlayerSelected = playerButton;
            currentPlayerSelected?.Select();

            playButton.interactable = currentPlayerSelected != null;
            UpdatePlayerInfo();
        }

        private void UpdatePlayerInfo()
        {
            if (currentPlayerSelected == null)
            {
                title.StringReference.Clear();
                description.StringReference.Clear();
                infoPanel.gameObject.SetActive(false);
                return;
            }

            title.StringReference = currentPlayerSelected.playerScriptable.playerName;
            description.StringReference = currentPlayerSelected.playerScriptable.description;
            sliderExp.value = currentPlayerSelected.playerScriptable.playerData.experience;
            sliderExp.maxValue = currentPlayerSelected.playerScriptable.maxExperience;
            experienceText.text = currentPlayerSelected.playerScriptable.playerData.experience.ToString();
            lvlText.text = currentPlayerSelected.playerScriptable.playerData.lvl.ToString();
            infoPanel.gameObject.SetActive(true);
        }

    }
}
