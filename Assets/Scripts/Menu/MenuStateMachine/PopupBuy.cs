using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;


namespace MyUI.ItemShop
{
    public class PopupBuy : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [Space]
        [SerializeField] private LocalizeStringEvent localizeEvent;
        [SerializeField] private LocalizedString purchaseMessage;


        private ShopMenu shopmenu;
        private ItemDataScriptable itemDataScriptable;


        public void Init(ShopMenu shopmenu)
        {
            this.shopmenu = shopmenu;
            yesButton.onClick.AddListener(OnClickYes);
            noButton.onClick.AddListener(OnClickNo);
        }

        public void Open(ItemDataScriptable itemDataScriptable)
        {
            this.itemDataScriptable = itemDataScriptable;
            string translatedItemName = itemDataScriptable.itemName.GetLocalizedString();

            localizeEvent.StringReference.Arguments = new object[] {
            translatedItemName,
            itemDataScriptable.price
        };

            localizeEvent.RefreshString();
            gameObject.SetActive(true);
        }

        private void OnClickYes()
        {
            shopmenu.Purchase(true, itemDataScriptable);
            itemDataScriptable = null;
            gameObject.SetActive(false);
        }

        private void OnClickNo()
        {
            shopmenu.Purchase(false, itemDataScriptable);
            itemDataScriptable = null;
            gameObject.SetActive(false);
        }


    }
}
