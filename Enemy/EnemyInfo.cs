using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class EnemyInfo : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public DropItems enemyDropSystem;
    [SerializeField]
    public EnemyHitboxCollider enemyHitbox;
    [SerializeField]
    public List<Color> enemyColors;
    private Transform floatingTextPrefab;

    [Header("Information")]
    [SerializeField]
    private float currentHealth = 1000f;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float baseMaxHealth = 1000f;
    [SerializeField]
    private float amountOfXP = 20f;
    [SerializeField]
    private float maxSpeed = 2f;
    [SerializeField]
    private float baseDamage = 1500;
    [SerializeField]
    private float levelScalar = 1f;
    [SerializeField]
    private float baseRoll = 0;
    [SerializeField]
    public bool alwaysDropsLoot = false;

    public void Start()
    {
        floatingTextPrefab = GameManager.instance.floatingTextPrefab;
        maxHealth = baseMaxHealth + (baseMaxHealth * levelScalar * GameManager.instance.player.playerInfo.playerLevel);
        currentHealth = maxHealth;
        enemyHitbox.SetHitboxBaseDamage(baseDamage, levelScalar);

    }

    public void SetBaseDamage()
    {
        enemyHitbox.SetHitboxBaseDamage(baseDamage, levelScalar);
    }

    public float GetBaseRoll()
    {
        return baseRoll;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void UpdateMaxHealth()
    {
        maxHealth = baseMaxHealth + (baseMaxHealth * levelScalar * GameManager.instance.player.playerInfo.playerLevel);
        if (currentHealth == baseMaxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void UpdateDamage()
    {
        enemyHitbox.UpdateHitboxDamage();
    }

    public float GetXPValue()
    {
        return amountOfXP + (amountOfXP * GameManager.instance.player.playerInfo.playerLevel * .1f); ;
    }

    public void SubtractDamage(float damage)
    {
        if (damage > 0)
        {
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            Destroy(transform.gameObject);
        }
    }

    public float GetMovementSpeed()
    {
        return maxSpeed;
    }

    internal void ResetHealth()
    {
        //maxHealth = baseMaxHealth + (baseMaxHealth * .5f * GameManager.instance.player.playerInfo.playerLevel);
        currentHealth = maxHealth;
    }

    public void ShowDamage(float damage, Transform transformLocation)
    {
        Vector3 adjustment = new Vector3(Random.Range(-.3f, .3f), Random.Range(-.3f, .3f), 0);
        Transform p = Instantiate(floatingTextPrefab, transformLocation.position + adjustment, Quaternion.identity, GameManager.instance.sceneComponentsHolder);
        if (p != null)
        {
            p.GetComponentInChildren<TMP_Text>().text = damage.ToString();
            p.transform.localPosition = transformLocation.position + adjustment;
        }
    }
}
