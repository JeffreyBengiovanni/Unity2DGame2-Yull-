using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    // Item Information
    public BaseWeapon wType;

    public float bDamage;

    public float wDamage = 0;
    public float wCastSpeed = 0;
    public float wVelocity = 0;
    public float wSize = 0;
    public float wKnockback = 0;
    public float wPierce = 0;
    public int wProjectiles = 0;

    public int baseIndex;

    public Weapon(int rarity, int itemCategory = 0, int itemType = 0, int prefixIndex = 0, int suffixIndex = 0, int itemLevel = 0,
                    string itemName = "" ): base(rarity, itemCategory, itemType, prefixIndex, suffixIndex, itemLevel)
    {
        wType = (BaseWeapon)GameManager.instance.itemDirectory.categories[itemCategory].baseTypes[itemType];
        this.itemName = itemName;
        baseIndex = itemType;
        itemRarity = rarity;
        UpdateBaseStats();
        if (itemCategory != 4)
        {
            wDamage = bDamage;
            wCastSpeed = wType.baseCastSpeed;
            wVelocity = wType.baseVelocity;
            wSize = wType.baseCrit;
            wKnockback = wType.baseKnockback;
            wPierce = wType.basePierce;
            wProjectiles = wType.baseProjectiles;
        }
        UpdateName();
        ApplyModifiers(itemCategory);
    }


    protected void ApplyModifiers(int itemCategory)
    {
        ApplyModifiersItem(itemCategory);
    }

    protected void ApplyModifiersItem(int itemCategory)
    {
        /* Rings are special in that they only want modifiers that apply to their
         * prefix and suffix. THis means everything else is set to 0 since these
         * end up acting as keys that unlock the special modifier which are empowered
         * versions of base items.
        */

        WPrefix p = GameManager.instance.itemDirectory.weaponPrefixes[prefixIndex];
        WSuffix s = GameManager.instance.itemDirectory.weaponSuffixes[suffixIndex];

        itemScore += (GameManager.instance.itemDirectory.categories[itemCategory].baseTypes[baseIndex] as BaseWeapon).valueScore;
        itemScore += p.valueScore;
        itemScore += s.valueScore;

        // Prefixes
        if (p.modifierDamage != 0)
        {
            wDamage += bDamage * p.modifierDamage;
        }
        if (p.modifierCastSpeed != 0)
        {
            wCastSpeed += p.modifierCastSpeed;
        }
        if (p.modifierKnockback != 0)
        {
            wKnockback += p.modifierKnockback;
        }
        if (p.modifierPierce != 0)
        {
            wPierce += p.modifierPierce;
        }
        if (p.modifierProjectiles != 0)
        {
            wProjectiles += p.modifierProjectiles;
        }
        if (p.modifierCrit != 0)
        {
            wSize += p.modifierCrit;
        }
        if (p.modifierVelocity != 0)
        {
            wVelocity += p.modifierVelocity;
        }

        // Suffixes
        if (s.modifierDamage != 0)
        {
            wDamage += bDamage * s.modifierDamage;
        }
        if (s.modifierCastSpeed != 0)
        {
            wCastSpeed += s.modifierCastSpeed;
        }
        if (s.modifierKnockback != 0)
        {
            wKnockback += s.modifierKnockback;
        }
        if (s.modifierPierce != 0)
        {
            wPierce += s.modifierPierce;
        }
        if (s.modifierProjectiles != 0)
        {
            wProjectiles += s.modifierProjectiles;
        }
        if (s.modifierCrit != 0)
        {
            wSize += s.modifierCrit;
        }
        if (s.modifierVelocity != 0)
        {
            wVelocity += s.modifierVelocity;
        }

        if (Mathf.Sign(p.modifierDamage) == 1 || Mathf.Sign(s.modifierDamage) == 1)
        {
            wDamage += bDamage;
        }
        // If wDamage was affected, means its modified. Adjust by adding base Damage
        if (p.modifierDamage == -99f || s.modifierDamage == -99f || wDamage < 0)
        {
            wDamage = 0;
        }
    }

    protected void ApplyModifiersWeaponAlt()
    {
        WPrefix p = GameManager.instance.itemDirectory.weaponPrefixes[prefixIndex];
        WSuffix s = GameManager.instance.itemDirectory.weaponSuffixes[suffixIndex];

        // Prefixes
        if (p.modifierDamage != 0)
        {
            wDamage *= p.modifierDamage;
        }
        if (p.modifierCastSpeed != 0)
        {
            wCastSpeed += p.modifierCastSpeed;
        }
        if (p.modifierKnockback != 0)
        {
            wKnockback *= p.modifierKnockback;
        }
        if (p.modifierPierce != 0)
        {
            wPierce *= p.modifierPierce;
        }
        if (p.modifierProjectiles != 0)
        {
            wProjectiles *= p.modifierProjectiles;
        }
        if (p.modifierCrit != 0)
        {
            wSize *= p.modifierCrit;
        }
        if (p.modifierVelocity != 0)
        {
            wVelocity *= p.modifierVelocity;
        }

        // Suffixes
        if (s.modifierDamage != 0)
        {
            wDamage *= s.modifierDamage;
        }
        if (s.modifierCastSpeed != 0)
        {
            wCastSpeed += s.modifierCastSpeed;
        }
        if (s.modifierKnockback != 0)
        {
            wKnockback *= s.modifierKnockback;
        }
        if (s.modifierPierce != 0)
        {
            wPierce *= s.modifierPierce;
        }
        if (s.modifierProjectiles != 0)
        {
            wProjectiles *= s.modifierProjectiles;
        }
        if (s.modifierCrit != 0)
        {
            wSize *= s.modifierCrit;
        }
        if (s.modifierVelocity != 0)
        {
            wVelocity *= s.modifierVelocity;
        }
    }

    protected void UpdateBaseStats()
    {
        bDamage = wType.baseDamage + (wType.baseDamage * .5f * ((Mathf.Pow(2, itemRarity)) + itemLevel));
    }

    protected string ReturnModifiers()
    {
        return $"Damage: {wDamage}\n" +
            $"Cast Speed: {wCastSpeed}\n" +
            $"Velocity: {wVelocity}\n" +
            $"Size: {wSize}\n" +
            $"Knockback: {wKnockback}\n" +
            $"Pierce: {wPierce}\n" +
            $"Projectiles: {wProjectiles}";
    }


    public override string ToString()
    {
        return $"Item Name: {itemName} | Item Level: {itemLevel} | Item Category: {itemCategory.categoryName} " +
            $"| Rarity: {itemRarity}";
    }
}
