using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONSaving : MonoBehaviour
{
    private PlayerData playerData;
    public bool doneLoading = true;

    private string persistentPath = "";
    private string fileName = Path.AltDirectorySeparatorChar + "SaveData.txt";
   
    private void Awake()
    {
        SetPaths();
    }

    private void Start()
    {
        LoadData();
    }

    public void GetDataToStore()
    {
        Player playerRef = GameManager.instance.player;

        // INVENTORY ITEMS STORING
        List<Item> inventory = new List<Item>();
        for (int i = 0; i < playerRef.inventoryController.GetInventoryData().GetInventoryItems().Count; i++)
        {
            ItemSO reference = playerRef.inventoryController.GetInventoryData().GetItemAt(i).item;
            if (reference == null)
            {
                inventory.Add(null);
                continue;
            }
            EquipableItemSO r = reference as EquipableItemSO;
            if (r != null)
            {
                if (r.BaseCategory < 4)
                {
                    if ((r as EquipmentSO).itemRef != null) 
                    {
                        inventory.Add((r as EquipmentSO).itemRef as Equipment);
                    }
                    //Debug.Log(reference.itemRef);
                    //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(reference.itemRef));
                    continue;
                }
                else
                {
                    if ((r as WeaponSO).itemRef != null)
                    {
                        inventory.Add((r as WeaponSO).itemRef as Weapon);
                    }

                    continue;
                }
            }
            else
            {
                inventory.Add(null);
                continue;
            }
        }

        // EQUIPPED ITEMS STORING
        List<Item> equipped = new List<Item>();
        for (int i = 0; i < playerRef.equipSystem.GetEquippedList().Count; i++)
        {
            EquipableItemSO reference = playerRef.equipSystem.GetEquippedList()[i];
            if (reference == null)
            {
                equipped.Add(null);
                continue;
            }
            if (i < 4)
            {
                equipped.Add((reference as EquipmentSO).itemRef);
                //Debug.Log(reference.itemRef);
                //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(reference.itemRef));
                continue;
            }
            else
            {
                equipped.Add((reference as WeaponSO).itemRef);
                continue;
            }
        }

        playerData = new PlayerData();
        playerData.equipped = equipped;
        playerData.inventory = inventory;
        playerData.playerXP = playerRef.playerInfo.playerXP;
        playerData.equippedLocked = playerRef.inventoryController.equippedLocked;
        playerData.inventoryLocked = playerRef.inventoryController.inventoryLocked;
        playerData.coins = playerRef.playerInfo.GetCoins();
        playerData.autoEquip = playerRef.GetComponent<PickUpSystem>().autoEquipBetter;
        playerData.autoSell = playerRef.GetComponent<PickUpSystem>().autoSellWorse;
    }

    private void SetPaths()
    {
        persistentPath = Application.persistentDataPath;
        if (!Directory.Exists(persistentPath))
        {
            Directory.CreateDirectory(persistentPath);
        }
    }

    public void SetPlayerData()
    {
        GetDataToStore();
    }

    public void SaveData()
    {
        SetPlayerData();

        string savePath = persistentPath + fileName;

        Debug.Log("Saving Data at " + savePath);
        string json = JsonUtility.ToJson(playerData);

        File.WriteAllText(savePath, json);
    }

    public void LoadData()
    {
        string loadPath = persistentPath + fileName;
        if (File.Exists(loadPath))
        {
            string json = File.ReadAllText(loadPath);

            playerData = JsonUtility.FromJson<PlayerData>(json);
            InitializeData();
        }
        else
        {
            Debug.Log("Save file does not exist");
            FirstTimeStartup();
        }
    }

    private void FirstTimeStartup()
    {
        Player playerRef = GameManager.instance.player;
        playerRef.inventoryController.FirstTimeResetLists();
        playerRef.inventoryController.SetPlayerInfo();
        playerRef.pickUpSystem.SetAutoEquip(false);
        playerRef.pickUpSystem.SetAutoSell(false);
    }


    private void InitializeData()
    {
        if (playerData != null)
        {
            Player playerRef = GameManager.instance.player;

            playerRef.playerInfo.playerXP = playerData.playerXP;
            playerRef.playerInfo.SetPlayerLevel();
            playerRef.equipSystem.SetEquippedList(playerData.equipped);
            playerRef.equipSystem.UpdatePlayerInfo();
            playerRef.inventoryController.UpdateInventoryUIEquipped();
            playerRef.inventoryController.SetInventoryData(playerData.inventory);
            playerRef.playerInfo.ResetHealth();
            playerRef.playerHealthBar.UpdateHealthbar();
            playerRef.playerInfo.SetCoins(playerData.coins);
            playerRef.inventoryController.equippedLocked = playerData.equippedLocked;
            playerRef.inventoryController.inventoryLocked = playerData.inventoryLocked;
            playerRef.inventoryController.ApplyLockedVisuals();
            playerRef.inventoryController.SetPlayerInfo();
            playerRef.pickUpSystem.SetAutoEquip(playerData.autoEquip);
            playerRef.pickUpSystem.SetAutoSell(playerData.autoSell);
            playerRef.pickUpSystem.UpdateVisuals();

        }
    }
}