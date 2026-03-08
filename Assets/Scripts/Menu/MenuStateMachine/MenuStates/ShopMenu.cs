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
    public class ShopMenu : MenuState
    {
        [Header("REFERENCES")]
        [SerializeField] private ScrollRect scrollRect;
        [Space]
        [SerializeField] private Button backButton;
        [Space]
        [SerializeField] private TMP_Text moneyText;

        [Header("PREFAB")]
        [SerializeField] private ItemUI itemUIPrefab;

        [Header("POPUP")]
        [field: SerializeField] public PopupBuy popupBuy { get; private set; }


        private List<ItemUI> itemButtons = new();
        public ItemWrap itemWrap { get; private set; } = new();

        public event Action<bool, ItemDataScriptable> OnConfirmPurchase;
        public event Action<ItemDataScriptable> OnEquipItem;
        public event Action<int> OnAddMoney;                                    //ya veremos como lo usamos

        private const string ITEMSKEY = "items";

        public override void Init()
        {
            backButton.onClick.AddListener(OnClickBack);
            popupBuy.Init(this);

            var dataLoad = SaveLoadManager<ItemWrap>.LoadData(ITEMSKEY);
            if (dataLoad.success)
                itemWrap = dataLoad.data;

            moneyText.text = itemWrap.money.ToString();
            CreateItems();
        }


        protected override void OnClickBack()
        {
            //sonido?
            GameInfo.playerSelected = null;
            MenuStateMachine.Instance.GoBack();
        }

        private void CreateItems()
        {
            ItemDataScriptable[] itemsScriptable = Resources.LoadAll<ItemDataScriptable>("Items");

            foreach (var itemScriptable in itemsScriptable)
            {
                //playerScriptable.LoadPlayerData();
                ItemUI clone = Instantiate(itemUIPrefab, scrollRect.content);
                clone.Init(this, itemScriptable);
                itemButtons.Add(clone);
                if (itemWrap.purchasedItems.Exists(x => x == itemScriptable.id))
                {
                    clone.Bought();

                    if (itemWrap.equipedItems.Exists(x => x == itemScriptable.id))
                        clone.Equip();
                }

            }
        }

        public void Purchase(bool purchased, ItemDataScriptable itemDataScriptable)
        {
            itemWrap.money -= itemDataScriptable.price;
            moneyText.text = itemWrap.money.ToString();

            OnConfirmPurchase?.Invoke(purchased, itemDataScriptable);
            if (!itemWrap.purchasedItems.Contains(itemDataScriptable.id))
                itemWrap.purchasedItems.Add(itemDataScriptable.id);
            SaveLoadManager<ItemWrap>.SaveData(ITEMSKEY, itemWrap);
        }

        public void Equip(ItemDataScriptable itemDataScriptable)
        {
            OnEquipItem?.Invoke(itemDataScriptable);
            if (!itemWrap.equipedItems.Contains(itemDataScriptable.id))
                itemWrap.equipedItems.Add(itemDataScriptable.id);
            SaveLoadManager<ItemWrap>.SaveData(ITEMSKEY, itemWrap);
        }




    }
}
