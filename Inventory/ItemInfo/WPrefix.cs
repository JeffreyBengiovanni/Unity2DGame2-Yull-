using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WPrefix : Prefix
{
    // Modifier Information
    public float modifierDamage;
    public float modifierCastSpeed;
    public float modifierVelocity;
    public float modifierCrit;
    public float modifierKnockback;
    public float modifierPierce;
    public int modifierProjectiles;
    public float valueScore;


    public WPrefix(string prefixName = "", float modifierDamage = 0f, float modifierCastSpeed = 0f,
                        float modifierVelocity = 0f, float modifierCrit = 0f, float modifierKnockback = 0f,
                        int modifierPierce = 0, int modifierProjectiles = 0, float valueScore = 0f)
    {
        this.prefixName = prefixName;
        this.modifierDamage = modifierDamage;
        this.modifierCastSpeed = modifierCastSpeed;
        this.modifierVelocity = modifierVelocity;
        this.modifierCrit = modifierCrit;
        this.modifierKnockback = modifierKnockback;
        this.modifierPierce = modifierPierce;
        this.modifierProjectiles = modifierProjectiles;
        this.valueScore = valueScore;
    }
}
