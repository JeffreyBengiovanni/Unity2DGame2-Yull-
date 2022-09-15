using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : BaseItemType
{
    // Item Information
    public float baseHealth;
    public float baseMana;
    public float baseDefense;
    public float baseHealthRegen;
    public float baseManaRegen;
    public float baseMovement;
    public float valueScore;

    public BaseEquipment(string baseName = "", float baseHealth = 0f, float baseMana = 0f,
                        float baseDefense = 0f, float baseHealthRegen = 0f, float baseManaRegen = 0f,
                        float baseMovement = 0f, float valueScore = 0)
    {
        this.baseName = baseName;
        this.baseHealth = baseHealth;
        this.baseMana = baseMana;
        this.baseDefense = baseDefense;
        this.baseHealthRegen = baseHealthRegen;
        this.baseManaRegen = baseManaRegen;
        this.baseMovement = baseMovement;
        this.valueScore = valueScore;
    }
}
