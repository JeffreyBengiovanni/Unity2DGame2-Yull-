using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    public List<ItemSO> toDrop = new List<ItemSO>();
    public Player playerRef;

    public void Start()
    {
        playerRef = GameManager.instance.player;
    }

    public void TestDropLoot()
    {
        GenerateLoot(1);
        DropLoot(GameManager.instance.player.transform);
    }

    public void GenerateLoot(int count, float baseRoll = 0)
    {
        for (int i = 0; i < count; i++)
        {
            toDrop.Add(CreateItem(baseRoll));
        }
    }

    public void GeneralDropLoot(Transform location = null, int count = 1, float baseRoll = 0)
    {
        GenerateLoot(count, baseRoll);
        DropLoot(location);
    }

    public void DropLoot(Transform location = null)
    {
        for (int i = 0; i < toDrop.Count; i++)
        {
            ItemPickUp item = Instantiate(GameManager.instance.itemDirectory.itemPickUp, location.position, 
                                            Quaternion.identity, GameManager.instance.sceneComponentsHolder);
            item.InventoryItem = toDrop[i];
            item.Quantity = 1;
            if(location != null)
            {
                item.transform.position = location.position;
            }
            else
            {
                Debug.Log("Could not find position for item");
            }
        }
        toDrop.Clear();
    }

    public EquipableItemSO CreateItem(float baseRoll = 0, Item generatedItem = null)
    {
        generatedItem = GameManager.instance.itemGenerator.GenerateItem(baseRoll);
        if (generatedItem.GetItemCategoryIndex() == 0)
        {
            EquipmentSO i = Instantiate(GameManager.instance.itemDirectory.robePrefab);
            ApplyArmorAttributes(i, generatedItem as Equipment);
            return i;
        }
        if (generatedItem.GetItemCategoryIndex() == 1)
        {
            EquipmentSO i = Instantiate(GameManager.instance.itemDirectory.bootsPrefab);
            ApplyArmorAttributes(i, generatedItem as Equipment);
            return i;
        }
        if (generatedItem.GetItemCategoryIndex() == 2)
        {
            EquipmentSO i = Instantiate(GameManager.instance.itemDirectory.glovesPrefab);
            ApplyArmorAttributes(i, generatedItem as Equipment);
            return i;
        }
        if (generatedItem.GetItemCategoryIndex() == 3)
        {
            EquipmentSO i = Instantiate(GameManager.instance.itemDirectory.defenseRingPrefab);
            ApplyArmorAttributes(i, generatedItem as Equipment);
            return i;
        }
        if (generatedItem.GetItemCategoryIndex() == 4)
        {
            WeaponSO i = Instantiate(GameManager.instance.itemDirectory.offenseRingPrefab);
            ApplyWeaponAttributes(i, generatedItem as Weapon);
            return i;
        }
        if (generatedItem.GetItemCategoryIndex() == 5)
        {
            WeaponSO i = Instantiate(GameManager.instance.itemDirectory.weaponPrefab);
            ApplyWeaponAttributes(i, generatedItem as Weapon);
            return i;
        }

        return null;
    }

    public EquipableItemSO CreateItemLoad(EquipableItemSO equipableItemSO = null, Item generatedItem = null)
    {
        if (generatedItem.itemName == "")
        {
            generatedItem = GameManager.instance.itemGenerator.GenerateItem();
        }
        if (generatedItem.GetItemCategoryIndex() == 0)
        {
            equipableItemSO = Instantiate(GameManager.instance.itemDirectory.robePrefab);
            ApplyArmorAttributes(equipableItemSO as EquipmentSO, generatedItem as Equipment);
            return equipableItemSO as EquipmentSO;
        }
        if (generatedItem.GetItemCategoryIndex() == 1)
        {
            equipableItemSO = Instantiate(GameManager.instance.itemDirectory.bootsPrefab);
            ApplyArmorAttributes(equipableItemSO as EquipmentSO, generatedItem as Equipment);
            return equipableItemSO as EquipmentSO;
        }
        if (generatedItem.GetItemCategoryIndex() == 2)
        {
            equipableItemSO = Instantiate(GameManager.instance.itemDirectory.glovesPrefab);
            ApplyArmorAttributes(equipableItemSO as EquipmentSO, generatedItem as Equipment);
            return equipableItemSO as EquipmentSO;
        }
        if (generatedItem.GetItemCategoryIndex() == 3)
        {
            equipableItemSO = Instantiate(GameManager.instance.itemDirectory.defenseRingPrefab);
            ApplyArmorAttributes(equipableItemSO as EquipmentSO, generatedItem as Equipment);
            return equipableItemSO as EquipmentSO;
        }
        if (generatedItem.GetItemCategoryIndex() == 4)
        {
            equipableItemSO = Instantiate(GameManager.instance.itemDirectory.offenseRingPrefab);
            ApplyWeaponAttributes(equipableItemSO as WeaponSO, generatedItem as Weapon);
            return equipableItemSO as WeaponSO;
        }
        if (generatedItem.GetItemCategoryIndex() == 5)
        {
            equipableItemSO = Instantiate(GameManager.instance.itemDirectory.weaponPrefab);
            ApplyWeaponAttributes(equipableItemSO as WeaponSO, generatedItem as Weapon);
            return equipableItemSO as WeaponSO;
        }
        return null;
    }


    public void ApplyArmorAttributes(EquipmentSO i, Equipment generated)
    {
        if (generated != null && generated.itemName != "")
        {
            i.itemRef = generated;
            i.itemScoreUnweighted = generated.itemScore -
                (GameManager.instance.itemDirectory.categories[generated.itemCategoryIndex]
                .baseTypes[generated.itemBaseIndex] as BaseEquipment).valueScore; i.itemScore = generated.itemScore + (generated.itemScore * (generated.itemLevel + generated.itemRarity * .1f));
            i.SellPrice = GetPrice(generated.GetItemRarity());
            i.name = generated.itemName;
            i.BaseCategory = generated.itemCategoryIndex;
            i.BaseType = generated.itemBaseIndex;
            i.Name = generated.itemName;
            i.itemLevel = generated.itemLevel;
            // Maybe a default description for the item base type here
            //i.Description = GameManager.instance.itemDirectory. (baseTypes) (Descriptions) .. etc
            i.BaseType = generated.baseIndex;
            i.Health = generated.eHealth;
            i.Mana = generated.eMana;
            i.Defense = generated.eDefense;
            i.HealthRegen = generated.eHealthRegen;
            i.ManaRegen = generated.eManaRegen;
            i.Movement = generated.eMovement;
            i.Rarity = generated.GetItemRarity();
            i.UpdateImage();
        }
    }

    public void ApplyWeaponAttributes(WeaponSO i, Weapon generated)
    {
        if (generated != null && generated.itemName != "")
        {
            i.itemRef = generated;
            i.itemScoreUnweighted = generated.itemScore - 
                (GameManager.instance.itemDirectory.categories[generated.itemCategoryIndex]
                .baseTypes[generated.itemBaseIndex] as BaseWeapon).valueScore;
            i.itemScore = generated.itemScore + (generated.itemScore * (generated.itemLevel + generated.itemRarity * .1f));
            i.SellPrice = GetPrice(generated.GetItemRarity());
            i.name = generated.itemName;
            i.BaseCategory = generated.itemCategoryIndex;
            i.BaseType = generated.itemBaseIndex;
            i.Name = generated.itemName;
            i.itemLevel = generated.itemLevel;
            // Maybe a default description for the item base type here
            //i.Description = GameManager.instance.itemDirectory. (baseTypes) (Descriptions) .. etc
            i.BaseType = generated.baseIndex;
            i.Damage = generated.wDamage;
            i.CastSpeed = generated.wCastSpeed;
            i.Velocity = generated.wVelocity;
            i.Crit = generated.wSize;
            i.Knockback = generated.wKnockback;
            i.Pierce = generated.wPierce;
            i.Projectiles = generated.wProjectiles;
            i.Rarity = generated.GetItemRarity();
            i.UpdateImage();
        }
    }

    private float GetPrice(int rarity)
    {
        return Mathf.Pow(2, rarity);
    }

}