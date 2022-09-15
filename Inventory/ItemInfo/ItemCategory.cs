using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCategory
{
    public string categoryName;
    public bool isEquipment;
    public int numberOfBases;
    public List<BaseItemType> baseTypes;

    public ItemCategory(string categoryName = "", bool isEquipment = true)
    {
        this.categoryName = categoryName;
        this.isEquipment = isEquipment;        
    }
}