using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitboxCollider : MonoBehaviour
{
    [SerializeField]
    private float baseDamage = 200f;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float knockback = 4f;
    [SerializeField]
    private float percentDamageIncrease = 1f;

    private void Start()
    {
        UpdateHitboxDamage();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CauseDamage(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CauseDamage(collision);
        }
    }

    private void CauseDamage(Collider2D collision)
    {
        UpdateHitboxDamage();
        Player playerRef = collision.transform.GetComponentInParent<Player>();

        if (playerRef != null)
        {
            playerRef.DamageTaken(damage, knockback, transform, false);
        }
    }

    internal void SetHitboxBaseDamage(float amount, float levelScalar)
    {
        percentDamageIncrease = levelScalar;
        this.baseDamage = amount;
        UpdateHitboxDamage();
    }

    internal void UpdateHitboxDamage()
    {
        damage = baseDamage + (baseDamage * GameManager.instance.player.playerInfo.playerLevel * percentDamageIncrease);
    }
}
