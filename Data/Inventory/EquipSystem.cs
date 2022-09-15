using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipSystem : MonoBehaviour
{
    [SerializeField]
    private List<EquipableItemSO> equipped = new List<EquipableItemSO>();

    [SerializeField]
    private int totalSlots = 6;

   [SerializeField]
    private EquipableItemSO weapon;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private Color robeDefaultColor;

    [SerializeField]
    private Color robeColor;

    [SerializeField]
    private SpriteRenderer robeSprite;

    [SerializeField]
    private List<ItemParameter> parametersToModify, itemCurrentState;

    [SerializeField]
    private Player playerRef;

    [SerializeField]
    private DropItems itemCreation;

    public void Awake()
    {
        for (int i = 0; i < totalSlots; i++)
        {
            equipped.Add(null);
        }
        playerRef = transform.GetComponent<Player>();
    }

    public void Start()
    {
        UpdatePlayerInfo();
    }

    public List<EquipableItemSO> GetEquippedList()
    {
        if (equipped.Count == 0)
        {
            Debug.Log("Equipped list is empty");
            return null;
        }
        else
        {
            return equipped;
        }
    }

    public void SetEquippedList(List<Item> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemName != "")
            {
                if (list[i].itemCategoryIndex < 4)
                {
                    Equipment itemRef = new Equipment(list[i].itemRarity, list[i].itemCategoryIndex, list[i].itemBaseIndex,
                                                    list[i].prefixIndex, list[i].suffixIndex, list[i].itemLevel);                    
                    equipped[i] = itemCreation.CreateItemLoad(equipped[i], itemRef);
                }
                else
                {
                    Weapon itemRef = new Weapon(list[i].itemRarity, list[i].itemCategoryIndex, list[i].itemBaseIndex,
                                                    list[i].prefixIndex, list[i].suffixIndex, list[i].itemLevel);
                    equipped[i] = itemCreation.CreateItemLoad(equipped[i], itemRef);
                }
            }
            else
            {
                equipped[i] = null;
            }
        }
        UpdatePlayerInfo();
    }

    public void SetSlot(EquipableItemSO equipableItemSO, List<ItemParameter> itemState, int slot, bool isWeapon)
    {
        if (equipped[slot] != null)
        {
            inventoryData.AddItem(equipped[slot], 1, itemCurrentState);
        }

        this.equipped[slot] = equipableItemSO;

        UpdatePlayerInfo();
    }

    public void UpdatePlayerInfo()
    {
        playerRef.playerInfo.UpdateBases();
        UpdateDefensivePlayerInfo();
        UpdateOffensivePlayerInfo();
        UpdateRobe(equipped[0]);
        UpdateWeapon(equipped[5]);
    }

    private void UpdateOffensivePlayerInfo()
    {
        PlayerInfo playerInfo = playerRef.playerInfo;

        for (int i = 4; i < 6; i++)
        {
            WeaponSO item = equipped[i] as WeaponSO;
            if (item != null)
            {
                playerInfo.damage += item.Damage;
                playerInfo.castSpeed -= (item.CastSpeed * playerInfo.baseCastSpeed);
                playerInfo.velocity += (item.Velocity * playerInfo.baseVelocity);
                playerInfo.crit += item.Crit;
                playerInfo.knockback += (item.Knockback * playerInfo.baseKnockback);
                playerInfo.pierce += item.Pierce;
                playerInfo.projectiles += item.Projectiles;
            }
        }
    }

    public void UpdateDefensivePlayerInfo()
    {
        PlayerInfo playerInfo = playerRef.playerInfo;

        for (int i = 0; i < 4; i++)
        {
            EquipmentSO item = equipped[i] as EquipmentSO;
            if (item != null)
            {
                playerInfo.effectiveHealth += item.Health;
                playerInfo.effectiveMana += item.Mana;
                playerInfo.defense += item.Defense;
                playerInfo.healthRegen += item.HealthRegen;
                playerInfo.manaRegen += item.ManaRegen;
                playerInfo.movement += (item.Movement * playerInfo.baseCastSpeed);
            }
        }
    }

    // Updates the weapon the player is holding
    public void UpdateWeapon(EquipableItemSO equipableItemSO)
    {
        playerRef.weaponSystem.UpdateWeaponValues(equipableItemSO);
    }

    // Updates the weapon the player is holding
    public void UpdateRobe(EquipableItemSO equipableItemSO)
    {
        if (equipableItemSO == null)
        {
            robeColor = robeDefaultColor;
            robeSprite.color = robeColor;
        }
        else
        {
            robeColor = GameManager.instance.itemDirectory.robeColors[equipableItemSO.BaseType];
            robeSprite.color = robeColor;
        }
    }

    private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }



}
