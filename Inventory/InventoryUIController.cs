using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.UI;
using Inventory.Model;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

namespace Inventory
{
    [Serializable]
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryPage inventoryUI;
        [SerializeField]
        private InputActionReference inventory;
        [SerializeField]
        private InventorySO inventoryData;
        [SerializeField]
        private EquipSystem equipSystem;
        [SerializeField]
        private DropItems itemCreation;
        [SerializeField]
        private AudioClip dropClip;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private PlayerInfo playerInfo;
        [SerializeField]
        public bool inventoryActive = false;

        public List<InventoryItem> initalItems = new List<InventoryItem>();

        [SerializeField]
        public List<bool> inventoryLocked;
        [SerializeField]
        public List<bool> equippedLocked;

        private void Awake()
        {
            PrepareUI();
        }

        private void Start()
        {
            playerInfo = GameManager.instance.player.playerInfo;
        }

        public void SetPlayerInfo()
        {
            inventoryUI.GeneratePlayerInfo();   
        }

        public void FirstTimeResetLists()
        {
            inventoryLocked = new List<bool>();
            equippedLocked = new List<bool>();
            for (int i = 0; i < inventoryData.Size; i++)
            {
                inventoryLocked.Add(false);
            }
            for (int i = 0; i < 6; i++)
            {
                equippedLocked.Add(false);
            }
        }

        public UIInventoryPage GetInventoryPage()
        {
            return inventoryUI;
        }

        public InventorySO GetInventoryData()
        {
            return inventoryData;
        }

        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initalItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        public void SetInventoryData(List<Item> list)
        {
            List<ItemSO> toSend = new List<ItemSO>();
            for (int i = 0; i < list.Count; i++)
            {
                toSend.Add(null);
                if (list[i].itemName != "")
                {
                    if (list[i].itemCategoryIndex < 4)
                    {
                        Equipment itemRef = new Equipment(list[i].itemRarity, list[i].itemCategoryIndex, list[i].itemBaseIndex,
                                                        list[i].prefixIndex, list[i].suffixIndex, list[i].itemLevel);
                        toSend[i] = itemCreation.CreateItemLoad(toSend[i] as EquipableItemSO, itemRef);
                    }
                    else
                    {
                        Weapon itemRef = new Weapon(list[i].itemRarity, list[i].itemCategoryIndex, list[i].itemBaseIndex,
                                                        list[i].prefixIndex, list[i].suffixIndex, list[i].itemLevel);
                        toSend[i] = itemCreation.CreateItemLoad(toSend[i] as EquipableItemSO, itemRef);
                    }
                }
            }
            inventoryData.SetInventory(toSend);
        }

        public void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity, item.Value.item.Rarity);
            }
            UpdateInventoryUIEquipped();
        }

        public void UpdateInventoryUIEquipped()
        {
            List<EquipableItemSO> list = equipSystem.GetEquippedList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null) 
                {
                    inventoryUI.GetEquippedItem(i).ResetData();
                    continue;
                }
                inventoryUI.UpdateDataEquipped(i, list[i].ItemImage, 1, list[i].Rarity);
            }
        }

        public void CallToPlayerInfo()
        {
            inventoryUI.GeneratePlayerInfo();
        }

        public void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            this.inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            this.inventoryUI.OnDescriptionRequestedEquipped += HandleDescriptionRequestEquipped;
            this.inventoryUI.OnSwapItems += HandleSwapItems;
            this.inventoryUI.OnStartDragging += HandleDragging;
            this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
            this.inventoryUI.OnItemActionRequestedEquipped += HandleItemActionRequestEquipped;
            this.inventoryUI.OnDoubleClick += HandleDoubleClick;
            this.inventoryUI.OnDoubleClickEquipped += HandleDoubleClickEquipped;

            PrepareInventoryData();
        }

        internal void ApplyLockedVisuals()
        {
            for (int i = 0; i < inventoryLocked.Count; i++)
            {
                if (inventoryLocked[i])
                {
                    inventoryUI.LockIndex(i);
                }
                else
                {
                    inventoryUI.UnlockIndex(i);
                }
            }
            for (int i = 0; i < equippedLocked.Count; i++)
            {
                if (equippedLocked[i])
                {
                    inventoryUI.LockIndexEquipped(i);
                }
                else
                {
                    inventoryUI.UnlockIndexEquipped(i);
                }
            }
        }

        private void HandleDoubleClick(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            //UIInventoryItem i = inventoryUI.GetItem(itemIndex);
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null && !inventoryLocked[itemIndex])
            {
                PerformAction(itemIndex);
            }
            inventoryUI.GeneratePlayerInfo();
        }

        private void HandleDoubleClickEquipped(int itemIndex)
        {
            ItemSO inventoryItem = equipSystem.GetEquippedList()[itemIndex];
            //UIInventoryItem i = inventoryUI.GetEquippedItem(itemIndex);
            IItemAction itemEquip = inventoryItem as IItemAction;
            if (itemEquip != null && !equippedLocked[itemIndex])
            {
                UnequipItem(itemIndex, inventoryItem);
                inventoryData.InformAboutChange();
                inventoryUI.ToggleItemAction(false);
                inventoryUI.ResetSelection();
            }
            inventoryUI.GeneratePlayerInfo();
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.GetActionPanel().RemoveOldButtons();
                return;
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                if (!inventoryLocked[itemIndex])
                {
                    inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
                }
            }
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                if (!inventoryLocked[itemIndex])
                {
                    inventoryUI.AddAction("Lock", () => LockItem(itemIndex));
                    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
                    inventoryUI.AddAction("Sell", () => SellItem(itemIndex, inventoryItem.quantity));
                }
                else
                {
                    inventoryUI.AddAction("Unlock", () => UnlockItem(itemIndex));
                }
            }
            if (!inventoryData.SlotEmpty(itemIndex))
            {
                inventoryUI.SelectIndex(itemIndex);
            }
            inventoryData.InformAboutChange();
            HandleDescriptionRequest(itemIndex);
            UpdateInventoryUIEquipped();
        }

        private void LockItem(int itemIndex)
        {
            inventoryLocked[itemIndex] = true;
            inventoryUI.LockIndex(itemIndex);
            HandleItemActionRequest(itemIndex);
            GameManager.instance.PlayClickSound();
        }

        private void UnlockItem(int itemIndex)
        {
            inventoryLocked[itemIndex] = false;
            inventoryUI.UnlockIndex(itemIndex);
            HandleItemActionRequest(itemIndex);
            GameManager.instance.PlayClickSound();
        }

        private void HandleItemActionRequestEquipped(int itemIndex)
        {
            ItemSO inventoryItem = equipSystem.GetEquippedList()[itemIndex];
            if (inventoryItem == null)
            {
                inventoryUI.GetActionPanel().RemoveOldButtons();
                return;
            }
            IItemAction itemEquip = inventoryItem as IItemAction;
            if (itemEquip != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                if (!equippedLocked[itemIndex])
                {
                    inventoryUI.AddAction("Unequip", () => UnequipItem(itemIndex, inventoryItem));
                }
            }
            /*
            IItemAction itemAction = inventoryItem as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }
            */
            IDestroyableItem destroyableItem = inventoryItem as IDestroyableItem;
            if (destroyableItem != null)
            {
                if (!equippedLocked[itemIndex])
                {
                    inventoryUI.AddAction("Lock", () => LockItemEquipped(itemIndex));
                    inventoryUI.AddAction("Drop", () => DropItemEquipped(itemIndex, 1));
                    inventoryUI.AddAction("Sell", () => SellItemEquipped(itemIndex));
                }
                else
                {
                    inventoryUI.AddAction("Unlock", () => UnlockItemEquipped(itemIndex));
                }
            }
            if (inventoryItem != null)
            {
                inventoryUI.SelectIndexEquipped(itemIndex);
            }
            inventoryData.InformAboutChange();
            HandleDescriptionRequestEquipped(itemIndex);
            UpdateInventoryUIEquipped();
        }
        private void LockItemEquipped(int itemIndex)
        {
            equippedLocked[itemIndex] = true;
            inventoryUI.LockIndexEquipped(itemIndex);
            HandleItemActionRequestEquipped(itemIndex);
            GameManager.instance.PlayClickSound();
        }

        private void UnlockItemEquipped(int itemIndex)
        {
            equippedLocked[itemIndex] = false;
            inventoryUI.UnlockIndexEquipped(itemIndex);
            HandleItemActionRequestEquipped(itemIndex);
            GameManager.instance.PlayClickSound();
        }

        private void UnequipItem(int itemIndex, ItemSO inventoryItem)
        {
            if (!equippedLocked[itemIndex])
            {
                inventoryUI.ToggleItemAction(false);
                if (inventoryItem as WeaponSO != null)
                {
                    transform.GetComponent<EquipSystem>().UpdateWeapon(null);
                }
                int leftover = inventoryData.AddItem(inventoryItem, 1);
                if (leftover > 0)
                {
                    Debug.Log("LEFTOVER " + leftover);
                    DropItemEquipped(itemIndex, 1);
                    equipSystem.UpdatePlayerInfo();
                }
                else
                {
                    equipSystem.GetEquippedList()[itemIndex] = null;
                    inventoryUI.ResetSelection();
                    //audioSource.PlayOneShot(dropClip);
                    UpdateInventoryUIEquipped();
                    equipSystem.UpdatePlayerInfo();
                }
            }
            SetPlayerInfo();
            GameManager.instance.PlayClickSound();

        }

        public void EquipRecommended()
        {
            List<InventoryItem> list = inventoryData.GetInventoryItems();
            List<EquipableItemSO> equippedList = equipSystem.GetEquippedList();
            for (int i = 0; i < list.Count; i++)
            {
                EquipableItemSO item = list[i].item as EquipableItemSO;

                if (!list[i].IsEmpty)
                {
                    if (!inventoryLocked[i] && !equippedLocked[item.BaseCategory])
                    {
                        if ((equippedList[item.BaseCategory]) == null)
                        {
                            PerformAction(i);
                        }
                        if (item.BaseCategory == 4 || item.BaseCategory == 5)
                        {
                            if((item as WeaponSO).Damage == 0 ||(equippedList[item.BaseCategory] as WeaponSO).Damage == 0)
                            {
                                if (item.itemScoreUnweighted > equippedList[item.BaseCategory].itemScoreUnweighted)
                                {
                                    PerformAction(i);
                                }
                                else if (item.itemScoreUnweighted == equippedList[item.BaseCategory].itemScoreUnweighted)
                                {
                                    if (item.itemScore > equippedList[item.BaseCategory].itemScore)
                                    {
                                        PerformAction(i);
                                    }
                                }
                            }
                            else
                            {
                                if (item.itemScore > equippedList[item.BaseCategory].itemScore)
                                {
                                    PerformAction(i);
                                }
                            }
                        }
                        else
                        {
                            if (item.itemScore > equippedList[item.BaseCategory].itemScore)
                            {
                                PerformAction(i);
                            }
                        }

                    }
                }
            }
            inventoryUI.ResetSelection();
            transform.GetComponent<Player>().playerHealthBar.UpdateHealthbar();
            UpdateInventoryUIEquipped();
            GameManager.instance.PlayClickSound();
        }

        private void SellItem(int itemIndex, int quantity)
        {
            if (!inventoryLocked[itemIndex])
            {
                inventoryUI.ToggleItemAction(false);
                for (int i = 0; i < quantity; i++)
                {
                    playerInfo.AddCoins(inventoryData.GetItemAt(itemIndex).item.SellPrice);
                }
                inventoryData.RemoveItem(itemIndex, quantity);
                inventoryUI.ResetSelection();
                //audioSource.PlayOneShot(dropClip);
                UpdateInventoryUIEquipped();
            }
            SetPlayerInfo();
            GameManager.instance.PlayClickSound();

        }
        private void SellItemEquipped(int itemIndex)
        {
            if (!equippedLocked[itemIndex])
            {
                inventoryUI.ToggleItemAction(false);
                playerInfo.AddCoins(equipSystem.GetEquippedList()[itemIndex].SellPrice);
                equipSystem.GetEquippedList()[itemIndex] = null;
                inventoryUI.ResetSelection();
                //audioSource.PlayOneShot(dropClip);
                UpdateInventoryUIEquipped();
                ResetWeapon();
                equipSystem.UpdatePlayerInfo();
            }
            SetPlayerInfo();
            GameManager.instance.PlayClickSound();

        }

        private void DropItemEquipped(int itemIndex, int quantity)
        {
            if (!equippedLocked[itemIndex])
            {
                inventoryUI.ToggleItemAction(false);
                ItemPickUp item = Instantiate(GameManager.instance.itemDirectory.itemPickUp);
                item.transform.position = transform.position;
                item.canPickUp = false;
                ItemSO itemRef = equipSystem.GetEquippedList()[itemIndex];
                item.InventoryItem = itemRef;
                item.Quantity = quantity;
                equipSystem.GetEquippedList()[itemIndex] = null;
                inventoryUI.ResetSelection();
                //audioSource.PlayOneShot(dropClip);
                UpdateInventoryUIEquipped();
                ResetWeapon();
                equipSystem.UpdatePlayerInfo();
            }
            SetPlayerInfo();
            GameManager.instance.PlayClickSound();

        }

        private void DropItem(int itemIndex, int quantity)
        {
            if (!inventoryLocked[itemIndex])
            {
                inventoryUI.ToggleItemAction(false);
                ItemPickUp item = Instantiate(GameManager.instance.itemDirectory.itemPickUp);
                item.transform.position = transform.position;
                item.canPickUp = false;
                InventoryItem itemRef = inventoryData.ItemAtIndex(itemIndex);
                item.InventoryItem = itemRef.item;
                item.Quantity = itemRef.quantity;
                inventoryData.RemoveItem(itemIndex, quantity);
                inventoryUI.ResetSelection();
                //audioSource.PlayOneShot(dropClip);
                UpdateInventoryUIEquipped();
            }
            SetPlayerInfo();
            GameManager.instance.PlayClickSound();

        }

        private void ResetWeapon()
        {
            transform.GetComponent<EquipSystem>().UpdateWeapon(null);
        }

        public void SellAllItems()
        {
            for (int i = 0; i < inventoryLocked.Count; i++)
            {
                if (!inventoryData.ItemAtIndex(i).IsEmpty)
                {
                    SellItem(i, 1);
                }
            }
            SetPlayerInfo();
            GameManager.instance.PlayClickSound();

        }

        // EQUIPPING
        public void PerformAction(int itemIndex)
        {
            if (!inventoryLocked[itemIndex] && !equippedLocked[(inventoryData.GetItemAt(itemIndex).item as EquipableItemSO).BaseCategory])
            {
                InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
                if (inventoryItem.IsEmpty)
                    return;
                IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
                if (destroyableItem != null)
                {
                    inventoryData.RemoveItem(itemIndex, 1);
                    if (!inventoryData.SlotEmpty(itemIndex))
                    {
                        inventoryUI.SelectIndex(itemIndex);
                    }
                }
                IItemAction itemAction = inventoryItem.item as IItemAction;
                if (itemAction != null)
                {
                    itemAction.PerformAction(gameObject, inventoryItem.itemState);
                    //audioSource.PlayOneShot(itemAction.ActionSFX);
                }
                if (!inventoryData.SlotEmpty(itemIndex))
                {
                    inventoryUI.SelectIndex(itemIndex);
                }
                else
                {
                    inventoryUI.ToggleItemAction(false);
                }
                HandleDescriptionRequest(itemIndex);
                UpdateInventoryUIEquipped();
                GameManager.instance.PlayClickSound();
            }
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            inventoryUI.ToggleItemAction(false);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity, inventoryItem.item.Rarity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
            SwapLockedState(itemIndex_1, itemIndex_2);
            ApplyLockedVisuals();
        }

        private void SwapLockedState(int itemIndex_1, int itemIndex_2)
        {
            if (itemIndex_1 == -1)
            {
                return;
            }
            bool temp = inventoryLocked[itemIndex_1];
            inventoryLocked[itemIndex_1] = inventoryLocked[itemIndex_2];
            inventoryLocked[itemIndex_2] = temp;
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            inventoryUI.GeneratePlayerInfo();
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            Debug.Log("W " + (item as EquipableItemSO).itemScore);
            Debug.Log("U " + (item as EquipableItemSO).itemScoreUnweighted);
            string description = PrepareDescription(item);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, description);
            UpdateInventoryUIEquipped();
        }

        private void HandleDescriptionRequestEquipped(int itemIndex)
        {
            inventoryUI.GeneratePlayerInfo();
            List<EquipableItemSO> list = equipSystem.GetEquippedList();
            if(list == null)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = list[itemIndex];
            if (item == null)
            {
                inventoryUI.ResetSelection();
                return;
            }
            Debug.Log("W " + (item as EquipableItemSO).itemScore);
            Debug.Log("U " + (item as EquipableItemSO).itemScoreUnweighted);
            string description = PrepareDescription(item);
            inventoryUI.UpdateDescriptionEquipped(itemIndex, item.ItemImage, item.Name, description);
            UpdateInventoryUIEquipped();

        }

        public string PrepareDescription(InventoryItem inventoryItem)
        {
            return inventoryItem.item.GetStatDescription(inventoryItem.item.Description);
        }

        public string PrepareDescription(ItemSO inventoryItem)
        {
            return inventoryItem.GetStatDescription(inventoryItem.Description);
        }

        private void OnEnable()
        {
            inventory.action.performed += ToggleInventory;
        }

        private void OnDisable()
        {
            inventory.action.performed -= ToggleInventory;
        }

        public void CloseInventory()
        {
            if (inventoryUI.isActiveAndEnabled == true && GameManager.instance.inMenu == true)
            {
                inventoryUI.Hide();
                inventoryActive = false;
                Time.timeScale = 1;
                GameManager.instance.inMenu = false;
                GameManager.instance.PlayClickSound();
            }
        }

        private void ToggleInventory(InputAction.CallbackContext context)
        {
            if (inventoryUI.isActiveAndEnabled == false && GameManager.instance.inMenu == false)
            {
                Time.timeScale = 0;
                inventoryActive = true;
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(
                        item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity,
                        item.Value.item.Rarity);
                }
                GameManager.instance.inMenu = true;
                GameManager.instance.PlayClickSound();
            }
            else if (inventoryUI.isActiveAndEnabled == true && GameManager.instance.inMenu == true)
            {
                inventoryUI.Hide();
                inventoryActive = false;
                Time.timeScale = 1;
                GameManager.instance.inMenu = false;
                GameManager.instance.PlayClickSound();
            }
        }
    }
}