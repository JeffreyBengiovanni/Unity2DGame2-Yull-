using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public Item GenerateItem(float baseRoll = 0)
    {
        int rarity = GetRandomRarity(baseRoll);
        int prefixIndex = 0;
        int suffixIndex = 0;
        int itemCategory = Random.Range(0, 6);
        int itemType = Random.Range(0, GameManager.instance.itemDirectory.categories[itemCategory].numberOfBases);
        //TO BE IMPLEMENTED WITH PLAYER
        int itemLevel = GameManager.instance.player.playerInfo.GetPlayerLevel();
        if (GameManager.instance.itemDirectory.categories[itemCategory].isEquipment)
        {
            prefixIndex = Random.Range(0, GameManager.instance.itemDirectory.equipmentPrefixes.Count);
            suffixIndex = Random.Range(0, GameManager.instance.itemDirectory.equipmentSuffixes.Count);
            Item i = new Equipment(rarity, itemCategory, itemType, prefixIndex, suffixIndex, itemLevel);
            return i;
            //GameManager.instance.playerData.playerInventory.inventory.Add(i);
        }
        else
        {
            prefixIndex = Random.Range(0, GameManager.instance.itemDirectory.weaponPrefixes.Count);
            suffixIndex = Random.Range(0, GameManager.instance.itemDirectory.weaponSuffixes.Count);
            Item i = new Weapon(rarity, itemCategory, itemType, prefixIndex, suffixIndex, itemLevel);
            return i;
            //GameManager.instance.playerData.playerInventory.inventory.Add(i);
        }
    }

    private int GetRandomRarity(float baseRoll)
    {
        float result = Random.Range(baseRoll * 100, 10000);

        if (result < 4500)
        {
            return 0;
        }
        else if (result < 8000)
        {
            return 1;
        }
        else if (result < 9200)
        {
            return 2;
        }
        else if (result < 9900)
        {
            return 3;
        }
        return 4;
    }
}
