using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESuffix : Suffix
{
    // Modifier Information
    public float modifierHealth;
    public float modifierMana;
    public float modifierDefense;
    public float modifierHealthRegen;
    public float modifierManaRegen;
    public float modifierMovement;
    public float valueScore;

    public ESuffix(string suffixName = "", float modifierHealth = 0f, float modifierMana = 0f,
                        float modifierDefense = 0f, float modifierHealthRegen = 0f, float modifierManaRegen = 0f,
                        float modifierMovement = 0f, float valueScore = 0f)
    {
        this.suffixName = suffixName;
        this.modifierHealth = modifierHealth;
        this.modifierMana = modifierMana;
        this.modifierDefense = modifierDefense;
        this.modifierHealthRegen = modifierHealthRegen;
        this.modifierManaRegen = modifierManaRegen;
        this.modifierMovement = modifierMovement;
        this.valueScore = valueScore;
    }
}
