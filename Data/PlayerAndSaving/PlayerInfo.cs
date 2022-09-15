using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInfo : MonoBehaviour
{
    [Header("Base")]
    [SerializeField]
    public int playerLevel = 0;
    [SerializeField]
    public float playerXP = 0;
    [SerializeField]
    public float currentXPAtStage = 0;
    [SerializeField]
    public List<float> playerXPChart;
    [SerializeField]
    public int maxLevel = 99;
    [SerializeField]
    public float baseLevelXP = 100f;
    [SerializeField]
    private float currentRegenTime = 0f;
    [SerializeField]
    private float RegenTime = 1f;
    [SerializeField]
    private int coins = 0;

    [Header("References")]
    [SerializeField]
    private Player playerRef;
    [SerializeField]
    private PlayerHealthBar playerHealthBar;

    [Header("Defensive Info")]
    [SerializeField]
    public float baseHealth = 1500f;
    [SerializeField]
    public float currentHealth = 1500f;
    [SerializeField]
    public float scaledHealth;
    [SerializeField]
    public float effectiveHealth;
    
    [SerializeField]
    public float baseMana = 100f;
    [SerializeField]
    public float currentMana = 100f;
    [SerializeField]
    public float scaledMana;
    [SerializeField]
    public float effectiveMana;

    [SerializeField]
    public float baseDefense = 100f;
    [SerializeField]
    public float scaledDefense;
    [SerializeField]
    public float defense;

    [SerializeField]
    public float baseHealthRegen = 25f;
    [SerializeField]
    public float scaledHealthRegen;
    [SerializeField]
    public float healthRegen;
    
    [SerializeField]
    public float baseManaRegen = 5f;
    [SerializeField]
    public float scaledManaRegen;
    [SerializeField]
    public float manaRegen;

    [SerializeField]
    public float baseMovement = 1f;
    [SerializeField]
    public float scaledMovement;
    [SerializeField]
    public float movement;
    
    [Header("Offensive Info")]
    [SerializeField]
    public float baseDamage = 300f;
    [SerializeField]
    public float scaledDamage;
    [SerializeField]
    public float damage;

    [SerializeField]
    public float baseCastSpeed = 2f;
    [SerializeField]
    public float scaledCastSpeed;
    [SerializeField]
    public float castSpeed;

    [SerializeField]
    public float baseVelocity = 2f;
    [SerializeField]
    public float scaledVelocity;
    [SerializeField]
    public float velocity;

    [SerializeField]
    public float baseCrit = 0;
    [SerializeField]
    public float scaledCrit;
    [SerializeField]
    public float crit;

    [SerializeField]
    public float baseKnockback = 1f;
    [SerializeField]
    public float scaledKnockback;
    [SerializeField]
    public float knockback;

    [SerializeField]
    public float basePierce = 0f;
    [SerializeField]
    public float scaledPierce;
    [SerializeField]
    public float pierce;

    [SerializeField]
    public int baseProjectiles = 1;
    [SerializeField]
    public int scaledProjectiles;
    [SerializeField]
    public int projectiles;

    public void Awake()
    {
        UpdateBases();
        InitializeXPChart();
    }

    public void Start()
    {
        SetPlayerLevel();
    }

    public void Update()
    {
        DoRegens();
    }

    private void DoRegens()
    {
        if (currentRegenTime < RegenTime)
        {
            currentRegenTime += Time.deltaTime;
        }
        else
        {
            if (currentHealth != effectiveHealth)
            {
                DoHealthRegen();
            }
            if (currentMana != effectiveMana)
            {
                DoManaRegen();
            }
            currentRegenTime = 0;

        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SetCoins(float amount)
    {
        coins = Mathf.RoundToInt(amount);
    }

    public void AddCoins(float amount)
    {
        coins += Mathf.RoundToInt(amount);
    }

    public void SubtractCoins(float amount)
    {
        coins -= Mathf.RoundToInt(amount);
    }

    private void DoHealthRegen()
    {
        if (currentHealth < effectiveHealth)
        {
            currentHealth += healthRegen;
        }
        if (currentHealth > effectiveHealth)
        {
            currentHealth = effectiveHealth;
        }
        playerHealthBar.UpdateHealthbar();
    }

    private void DoManaRegen()
    {
        if (currentMana < effectiveMana)
        {
            currentMana += manaRegen;
        }
        if (currentMana > effectiveMana)
        {
            currentMana = effectiveMana;
        }
    }

    public void InitializeXPChart()
    {
        playerXPChart = new List<float>();
        for (int i = 0; i < maxLevel; i++)
        {
            if (i == 0)
            {
                playerXPChart.Add(baseLevelXP);
            }
            else
            {
                playerXPChart.Add(baseLevelXP + (baseLevelXP * .5f * i));
            }
        }
    }

    public void AddXP(float amount)
    {
        playerXP += amount;
        currentXPAtStage += amount;
        if(playerLevel != maxLevel)
        {
            if (currentXPAtStage >= playerXPChart[playerLevel])
            {
                SetPlayerLevel();
            }
        }
        playerRef.inventoryController.SetPlayerInfo();
    }

    public void SetPlayerLevel()
    {
        playerLevel = 0;
        float tempTotal = playerXP;
        for (int i = 0; i < maxLevel; i++)
        {
            if (tempTotal >= playerXPChart[i])
            {
                playerLevel++;
                tempTotal -= playerXPChart[i];
            }
            else
            {
                currentXPAtStage = tempTotal;
                return;
            }
        }
        ResetHealth();
    }

    public int GetPlayerLevel()
    {
        return playerLevel;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return effectiveHealth;
    }

    public void SetHealth(float value)
    {
        currentHealth = value;
    }

    public void AffectHealth(float value)
    {
        currentHealth += value;
    }

    public void ResetHealth()
    {
        currentHealth = effectiveHealth;
    }

    internal void UpdateBases()
    {
        // Offensive
        scaledDamage = baseDamage + (baseDamage * .5f * playerLevel);
        scaledCastSpeed = baseCastSpeed;
        scaledVelocity = baseVelocity;
        scaledCrit = baseCrit;
        scaledKnockback = baseKnockback;
        scaledPierce = basePierce;
        scaledProjectiles = baseProjectiles;

        damage = scaledDamage;
        castSpeed = scaledCastSpeed;
        velocity = scaledVelocity;
        crit = scaledCrit;
        knockback = scaledKnockback;
        pierce = scaledPierce;
        projectiles = scaledProjectiles;

        // Defensive
        scaledHealth = baseHealth + (baseHealth * .1f * playerLevel);
        scaledMana = baseDamage + (baseDamage * .1f * playerLevel);
        scaledDefense = baseDefense + (baseDefense * .1f * playerLevel);
        scaledHealthRegen = baseHealthRegen + (baseHealthRegen * .1f * playerLevel);
        scaledManaRegen = baseManaRegen + (baseManaRegen * .1f * playerLevel);
        scaledMovement = baseMovement;

        effectiveHealth = scaledHealth;
        effectiveMana = scaledMana;
        defense = scaledDefense;
        healthRegen = scaledHealthRegen;
        manaRegen = scaledManaRegen;
        movement = scaledMovement;

        if (pierce < 0)
        {
            pierce = 0;
        }
        castSpeed = Mathf.Clamp((float)Math.Round(castSpeed, 2), .01f, 50f);
    }

    internal void SubtractDamage(float damage)
    {
        if (damage > 0)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            transform.GetComponent<Player>().Death();
        }
    }

    public string GetPlayerInfo()
    {
        string s = "";
        s += GetBaseInfo();
        s += "\n";
        s += GetOffensiveInfo();
        s += "\n";
        s += GetDefensiveInfo();

        return s;
    }

    private string GetBaseInfo()
    {
        string s = "";
        s += $"<color=white>Level: {playerLevel}</color>\n";
        s += $"XP: <color=white>{currentXPAtStage}/{playerXPChart[playerLevel]}</color>\n";
        s += $"Yullites: <color=yellow>{coins}</color>\n";

        return s;
    }

    private string GetOffensiveInfo()
    {
        string s = $"<color=orange>Offensive Stats:</color>\n";

        if (damage > scaledDamage)
        {
            s += $"Damage: <color=green>{Math.Round(damage)}</color>\n";
        }
        else if (damage == scaledDamage)
        {
            s += $"Damage: <color=white>{Math.Round(damage)}</color>\n";
        }
        else
        {
            s += $"Damage: <color=red>{Math.Round(damage)}</color>\n";
        }

        if (castSpeed < scaledCastSpeed)
        {
            s += $"Cast Speed: <color=green>{Mathf.Clamp((float)Math.Round(castSpeed, 2), .01f, 50f)} sec</color>";
            if (castSpeed <= .1f)
            {
                s += $"<color=#ffff00ff> (Unruly)</color>\n";
            }
            else if (castSpeed <= .4f)
            {
                s += $"<color=#bbbb00ff> (Wild)</color>\n";
            }
            else
            {
                s += $"\n";
            }
        }
        else if (castSpeed == scaledCastSpeed)
        {
            s += $"Cast Speed: <color=white>{Math.Round(castSpeed, 1)} sec</color>\n";
        }
        else
        {
            s += $"Cast Speed: <color=red>{Math.Round(castSpeed, 1)} sec</color>\n";
        }
       
        if (crit > scaledCrit)
        {
            s += $"Critical Chance: <color=green>{Math.Round(crit * 100, 1)}%</color>\n";
        }
        else if (crit == scaledCrit)
        {
            s += $"Critical Chance: <color=white>{Math.Round(crit * 100, 1)}%</color>\n";
        }
        else
        {
            s += $"Critical Chance: <color=red>{Math.Round(crit * 100, 1)}%</color>\n";
        }

        if (velocity > scaledVelocity)
        {
            s += $"Velocity: <color=green>{Math.Round(velocity, 1)}</color>\n";
        }
        else if (velocity == scaledVelocity)
        {
            s += $"Velocity: <color=white>{Math.Round(velocity, 1)}</color>\n";
        }
        else
        {
            s += $"Velocity: <color=red>{Math.Round(velocity, 1)}</color>\n";
        }

        if (knockback > scaledKnockback)
        {
            s += $"Knockback: <color=green>{Math.Round(knockback, 1)}</color>\n";
        }
        else if (knockback == scaledKnockback)
        {
            s += $"Knockback: <color=white>{Math.Round(knockback, 1)}</color>\n";
        }
        else
        {
            s += $"Knockback: <color=red>{Math.Round(knockback, 1)}</color>\n";
        }

        if (pierce > scaledPierce)
        {
            s += $"Pierce: <color=green>{Math.Round(pierce)}</color>\n";
        }
        else if (pierce == scaledPierce)
        {
            s += $"Pierce: <color=white>{Math.Round(pierce)}</color>\n";
        }
        else
        {
            s += $"Pierce: <color=red>{Math.Round(pierce)}</color>\n";
        }

        if (projectiles > scaledProjectiles)
        {
            s += $"Projectiles: <color=green>{projectiles}</color>\n";
        }
        else if (projectiles == scaledProjectiles)
        {
            s += $"Projectiles: <color=white>{projectiles}</color>\n";
        }
        else
        {
            s += $"Projectiles: <color=red>{projectiles}</color>\n";
        }

        return s;
    }

    private string GetDefensiveInfo()
    {
        string s = $"<color=#00d0d0ff>Defensive Stats:</color>\n";

        if (effectiveHealth > scaledHealth)
        {
            s += $"Health: <color=green>{Math.Round(effectiveHealth)}</color>\n";
        }
        else if (effectiveHealth == scaledHealth)
        {
            s += $"Health: <color=white>{Math.Round(effectiveHealth)}</color>\n";
        }
        else
        {
            s += $"Health: <color=red>{Math.Round(effectiveHealth)}</color>\n";
        }

        if (effectiveMana > scaledMana)
        {
            s += $"Mana: <color=green>{Math.Round(effectiveMana, 1)}</color>\n";
        }
        else if (effectiveMana == scaledMana)
        {
            s += $"Mana: <color=white>{Math.Round(effectiveMana, 1)}</color>\n";
        }
        else
        {
            s += $"Mana: <color=red>{Math.Round(effectiveMana, 1)}</color>\n";
        }

        if (defense > scaledDefense)
        {
            s += $"Defense: <color=green>{Math.Round(defense, 1)}</color>\n";
        }
        else if (defense == scaledDefense)
        {
            s += $"Defense: <color=white>{Math.Round(defense, 1)}</color>\n";
        }
        else
        {
            s += $"Defense: <color=red>{Math.Round(defense, 1)}</color>\n";
        }

        if (healthRegen > scaledHealthRegen)
        {
            s += $"Health Regen: <color=green>{Math.Round(healthRegen, 1)}</color>\n";
        }
        else if (healthRegen == scaledHealthRegen)
        {
            s += $"Health Regen: <color=white>{Math.Round(healthRegen, 1)}</color>\n";
        }
        else
        {
            s += $"Health Regen: <color=red>{Math.Round(healthRegen, 1)}</color>\n";
        }

        if (manaRegen > scaledManaRegen)
        {
            s += $"Mana Regen: <color=green>{Math.Round(manaRegen, 1)}</color>\n";
        }
        else if (manaRegen == scaledManaRegen)
        {
            s += $"Mana Regen: <color=white>{Math.Round(manaRegen, 1)}</color>\n";
        }
        else
        {
            s += $"Mana Regen: <color=red>{Math.Round(manaRegen, 1)}</color>\n";
        }

        if (movement > scaledMovement)
        {
            s += $"Movement: <color=green>{Math.Round(movement, 1)}</color>\n";
        }
        else if (movement == scaledMovement)
        {
            s += $"Movement: <color=white>{Math.Round(movement, 1)}</color>\n";
        }
        else
        {
            s += $"Movement: <color=red>{Math.Round(movement, 1)}</color>\n";
        }

        return s;
    }

}
