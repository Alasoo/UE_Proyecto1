using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;


namespace MyUI.ItemShop
{
    public class ItemUI : MonoBehaviour
    {
        [SerializeField] private Button buyItem;
        [SerializeField] private Button equipItem;
        [SerializeField] private Image img;

        [Space]
        [SerializeField] private LocalizeStringEvent equipLocalizeEvent;
        [Space]
        [SerializeField] private LocalizedString equipedLocalized;
        [SerializeField] private LocalizedString unEquipedLocalized;


        private ShopMenu shopMenu;
        private ItemDataScriptable itemDataScriptable;

        private bool equiped = false;


        public void Init(ShopMenu shopMenu, ItemDataScriptable itemDataScriptable)
        {
            this.shopMenu = shopMenu;
            this.itemDataScriptable = itemDataScriptable;
            shopMenu.OnConfirmPurchase += OnConfirmPurchase;
            shopMenu.OnEquipItem += OnEquipItem;

            buyItem.onClick.AddListener(OnBuyItem);
            equipItem.onClick.AddListener(() => shopMenu.Equip(this.itemDataScriptable));

            equipItem.interactable = false;
            buyItem.interactable = shopMenu.itemWrap.money >= itemDataScriptable.price;
        }

        void OnDestroy()
        {
            shopMenu.OnConfirmPurchase -= OnConfirmPurchase;
            shopMenu.OnEquipItem -= OnEquipItem;
        }


        private void OnBuyItem()
        {
            shopMenu.popupBuy.Open(itemDataScriptable);
        }

        private void OnEquipItem(ItemDataScriptable itemDataScriptable)
        {
            //toggle de  equipar desequipar
            if (itemDataScriptable == this.itemDataScriptable)
            {
                equiped = !equiped;
                if (equiped)
                    Equip();
                else
                    UnEquip();

                return;
            }

            //es un item para el mismo character
            if (itemDataScriptable.character == this.itemDataScriptable.character)
            {
                //es un item para el mismo slot
                if (itemDataScriptable.slot == this.itemDataScriptable.slot)
                {
                    if (equiped)
                        UnEquip();
                }
            }
        }




        private void OnConfirmPurchase(bool buy, ItemDataScriptable itemDataScriptable)
        {
            if (!buy) return;

            //he comprado este objeto
            if (itemDataScriptable == this.itemDataScriptable)
            {
                Bought();
            }
            else    //he comprado otro item, compruebo si tengo suficientes monedas
            {
                buyItem.interactable = this.itemDataScriptable.price <= shopMenu.itemWrap.money;
            }
        }



        public void Bought()
        {
            buyItem.interactable = false;
            equipItem.interactable = true;

            shopMenu.OnConfirmPurchase -= OnConfirmPurchase;
        }
        public void Equip()
        {
            equipLocalizeEvent.StringReference = unEquipedLocalized;
            equiped = true;
        }

        private void UnEquip()
        {
            equipLocalizeEvent.StringReference = equipedLocalized;
            equiped = false;
        }


    }
}