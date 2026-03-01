using MyUI.OptionButtons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MyUI.PlayerSelection
{
    public class PlayerButton : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private Image img;
        [SerializeField] private Image select;

        public PlayerDataScriptable playerScriptable { get; private set; }
        private PlayersMenu playersMenu;


        public void Init(PlayerDataScriptable playerScriptable, PlayersMenu playersMenu)
        {
            this.playerScriptable = playerScriptable;
            this.playersMenu = playersMenu;
            img.sprite = playerScriptable.playerSprite;

            select.gameObject.SetActive(false);

            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (playersMenu == null) return;
            playersMenu.OnClickPlayerButton(this);
        }

        public void Select()
        {
            select.gameObject.SetActive(true);
        }


        public void Deselect()
        {
            select.gameObject.SetActive(false);
        }

    }
}