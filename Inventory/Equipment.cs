using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    // Item Information
    public BaseEquipment eType;

    public float bHealth;
    public float bMana;
    public float bDefense;
    public float bHealthRegen;
    public float bManaRegen;

    public float eHealth = 0;
    public float eMana = 0;
    public float eDefense = 0;
    public float eHealthRegen = 0;
    public float eManaRegen = 0;
    public float eMovement = 0;

    public int baseIndex;

    public Equipment(int rarity, int itemCategory = 0, int itemType = 0, int prefixIndex = 0, int suffixIndex = 0, int itemLevel = 0,
                        string itemName = "") : base(rarity, itemCategory, itemType, prefixIndex, suffixIndex, itemLevel)
    {
        eType = (BaseEquipment)GameManager.instance.itemDirectory.categories[itemCategory].baseTypes[itemType];
        this.itemName = itemName;
        baseIndex = itemType;
        itemRarity = rarity;
        UpdateBaseStats();
        if (itemCategory != 3)
        {
            eHealth = bHealth;
            eMana = bMana;
            eDefense = bDefense;
            eHealthRegen = bHealthRegen;
            eManaRegen = bManaRegen;
            eMovement = eType.baseMovement;
        }
        UpdateName();
        ApplyModifiers(itemCategory);
    }

    protected void ApplyModifiers(int itemCategory)
    {
        ApplyModifiersRing(itemCategory);
    }

    protected void ApplyModifiersRing(int itemCategory)
    {
        EPrefix p = GameManager.instance.itemDirectory.equipmentPrefixes[prefixIndex];
        ESuffix s = GameManager.instance.itemDirectory.equipmentSuffixes[suffixIndex];

        itemScore += (GameManager.instance.itemDirectory.categories[itemCategory].baseTypes[baseIndex] as BaseEquipment).valueScore;
        itemScore += p.valueScore;
        itemScore += s.valueScore;

        // Prefixes
        if (p.modifierHealth != 0)
        {
            eHealth += bHealth * p.modifierHealth;
        }
        if (p.modifierMana != 0)
        {
            eMana += bMana + p.modifierMana;
        }
        if (p.modifierDefense != 0)
        {
            eDefense += bDefense * p.modifierDefense;
        }
        if (p.modifierHealthRegen != 0)
        {
            eHealthRegen += bHealthRegen * p.modifierHealthRegen;
        }
        if (p.modifierManaRegen != 0)
        {
            eManaRegen += bManaRegen * p.modifierManaRegen;
        }
        if (p.modifierMovement != 0)
        {
            eMovement += p.modifierMovement;
        }

        // Suffixes
        if (s.modifierHealth != 0)
        {
            eHealth += bHealth * s.modifierHealth;
        }
        if (s.modifierMana != 0)
        {
            eMana += bMana + s.modifierMana;
        }
        if (s.modifierDefense != 0)
        {
            eDefense += bDefense * s.modifierDefense;
        }
        if (s.modifierHealthRegen != 0)
        {
            eHealthRegen += bHealthRegen * s.modifierHealthRegen;
        }
        if (s.modifierManaRegen != 0)
        {
            eManaRegen += bManaRegen * s.modifierManaRegen;
        }
        if (s.modifierMovement != 0)
        {
            eMovement += s.modifierMovement;
        }


        // Defensive stuff for rings
        if (itemCategory == 3)
        {
            if (eHealth != 0)
            {
                eHealth += bHealth;
            }
            if (eMana != 0)
            {
                eMana += bMana;
            }
            if (eDefense != 0)
            {
                eDefense += bDefense;
            }
            if (eHealthRegen != 0)
            {
                eHealthRegen += bHealthRegen;
            }
            if (eManaRegen != 0)
            {
                eManaRegen += bManaRegen;
            }
        }
    }

    protected void ApplyModifiersEquipment()
    {
        EPrefix p = GameManager.instance.itemDirectory.equipmentPrefixes[prefixIndex];
        ESuffix s = GameManager.instance.itemDirectory.equipmentSuffixes[suffixIndex];

        // Prefixes
        if (p.modifierHealth != 0)
        {
            eHealth *= p.modifierHealth;
        }
        if (p.modifierMana != 0)
        {
            eMana *= p.modifierMana;
        }
        if (p.modifierDefense != 0)
        {
            eDefense *= p.modifierDefense;
        }
        if (p.modifierHealthRegen != 0)
        {
            eHealthRegen *= p.modifierHealthRegen;
        }
        if (p.modifierManaRegen != 0)
        {
            eManaRegen *= p.modifierManaRegen;
        }
        if (p.modifierMovement != 0)
        {
            eMovement *= p.modifierMovement;
        }

        // Suffixes
        if (s.modifierHealth != 0)
        {
            eHealth *= s.modifierHealth;
        }
        if (s.modifierMana != 0)
        {
            eMana *= s.modifierMana;
        }
        if (s.modifierDefense != 0)
        {
            eDefense *= s.modifierDefense;
        }
        if (s.modifierHealthRegen != 0)
        {
            eHealthRegen *= s.modifierHealthRegen;
        }
        if (s.modifierManaRegen != 0)
        {
            eManaRegen *= s.modifierManaRegen;
        }
        if (s.modifierMovement != 0)
        {
            eMovement *= s.modifierMovement;
        }
    }

    protected void UpdateBaseStats()
    {
        bHealth = eType.baseHealth + (eType.baseHealth * .5f * (Mathf.Pow(2, itemRarity) + itemLevel));
        bMana = eType.baseMana + (eType.baseMana * .5f * (Mathf.Pow(2, itemRarity) + itemLevel));
        bDefense = eType.baseDefense + (eType.baseDefense * .5f * (Mathf.Pow(2, itemRarity) + itemLevel));
        bHealthRegen = eType.baseHealthRegen + (eType.baseHealthRegen * .5f * (Mathf.Pow(2, itemRarity) + itemLevel));
        bManaRegen = eType.baseManaRegen + (eType.baseManaRegen * .5f * (Mathf.Pow(2, itemRarity) + itemLevel));
    }

    protected string ReturnModifiers()
    {
        return $"Health: {eHealth}\n" +
            $"Mana: {eMana}\n" +
            $"Defense: {eDefense}\n" +
            $"Health Regen: {eHealthRegen}\n" +
            $"Mana Regen: {eManaRegen}\n" +
            $"Movement: {eMovement}";
    }
    public override string ToString()
    {
        return $"Item Name: {itemName} | Item Level: {itemLevel} | Item Category: {itemCategory.categoryName} " +
            $"| Rarity: {itemRarity}";
    }
}
