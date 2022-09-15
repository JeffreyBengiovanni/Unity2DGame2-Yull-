using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseItemType
{
    // Item Information
    public float baseDamage;
    public float baseCastSpeed;
    public float baseVelocity;
    public float baseCrit;
    public float baseKnockback;
    public float basePierce;
    public int baseProjectiles;
    public float valueScore;

    public BaseWeapon(string baseName = "", float baseDamage = 0f, float baseCastSpeed = 0f,
                        float baseVelocity = 0f, float baseCrit = 0f, float baseKnockback = 0f,
                        int basePierce = 0, int baseProjectiles = 0, float valueScore = 0f)
    {
        this.baseName = baseName;
        this.baseDamage = baseDamage;
        this.baseCastSpeed = baseCastSpeed;
        this.baseVelocity = baseVelocity;
        this.baseCrit = baseCrit;
        this.baseKnockback = baseKnockback;
        this.basePierce = basePierce;
        this.baseProjectiles = baseProjectiles;
        this.valueScore = valueScore;

    }
}
