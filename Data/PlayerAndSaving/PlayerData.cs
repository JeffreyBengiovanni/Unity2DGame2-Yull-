using Inventory.Model;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public List<Item> inventory;
    public List<Item> equipped;
    public List<bool> inventoryLocked;
    public List<bool> equippedLocked;
    public float playerXP;
    public int coins;
    public bool autoEquip;
    public bool autoSell;

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
                s += inventory[i].itemName;
        }
        s += "\n\n\n\n";
        for (int i = 0; i < equipped.Count; i++)
        {
            if (equipped[i] != null)
                s += equipped[i].itemName;
        }
        return $"{s}";
    }
}
