using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemCategory itemCategory;
    public BaseItemType itemType;
    public string itemName;
    public int prefixIndex;
    public int suffixIndex;
    public int itemLevel;
    public int itemRarity;
    public int itemCategoryIndex;
    public int itemBaseIndex;
    public float itemScore;

    public Item(int itemRarity, int itemCategory = 0, int itemType = 0,
            int prefixIndex = 0, int suffixIndex = 0, int itemLevel = 0)
    {
        itemScore = 50f;
        this.itemRarity = itemRarity;
        itemCategoryIndex = itemCategory;
        itemBaseIndex = itemType;
        this.itemCategory = GameManager.instance.itemDirectory.categories[itemCategory];
        this.itemType = GameManager.instance.itemDirectory.categories[itemCategory].baseTypes[itemType];
        this.prefixIndex = prefixIndex;
        this.suffixIndex = suffixIndex;
        this.itemLevel = itemLevel;
    }

    public int GetItemRarity()
    {
        return itemRarity;
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public int GetItemCategoryIndex()
    {
        return itemCategoryIndex;
    }

    protected void UpdateName()
    {
        if(itemCategory.isEquipment)
        {
            itemName = GameManager.instance.itemDirectory.equipmentPrefixes[prefixIndex].prefixName + " " +
                        itemType.baseName + " " +
                        GameManager.instance.itemDirectory.equipmentSuffixes[suffixIndex].suffixName;
        }
        else
        {
            itemName = GameManager.instance.itemDirectory.weaponPrefixes[prefixIndex].prefixName + " " +
                        itemType.baseName + " " +
                        GameManager.instance.itemDirectory.weaponSuffixes[suffixIndex].suffixName;
        }
    }

    public override string ToString()
    {
        return $"Item Name: {itemName} | Item Level: {itemLevel} | Item Category: {itemCategory.categoryName} " +
            $"| Rarity: {itemRarity}";
    }
}
