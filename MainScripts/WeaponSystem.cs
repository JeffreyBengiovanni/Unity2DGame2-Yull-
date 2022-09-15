using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField]
    private Player playerRef;
    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer0;
    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer1;
    [SerializeField]
    private Sprite projectileSprite0;
    [SerializeField]
    private Sprite projectileSprite1;
    [SerializeField]
    private Light2D weaponLight;
    [SerializeField]
    public Transform firePosition;
    [SerializeField]
    private WeaponSO weapon;
    [SerializeField]
    private Projectile projectilePrefab;
    [SerializeField]
    private bool canFire = true;
    [SerializeField]
    public List<Transform> aimPositions;
    [SerializeField]
    public Animator weaponHolderAnimator;

    public void Start()
    {
        UpdateWeaponValues(null);
    }

    public void UpdateWeaponValues(EquipableItemSO equipableItemSO)
    {
        if (equipableItemSO == null)
        {
            weaponSpriteRenderer0.sprite = GameManager.instance.itemDirectory.baseWeaponImages[0];
            weaponSpriteRenderer1.sprite = GameManager.instance.itemDirectory.baseWeaponImages[1];
            projectileSprite0 = GameManager.instance.itemDirectory.baseWeaponProjectiles[0];
            projectileSprite1 = GameManager.instance.itemDirectory.baseWeaponProjectiles[1];
            weaponLight.color = Color.magenta;
            weaponLight.enabled = true;
            weapon = null;
        }
        else
        {
            weaponSpriteRenderer0.sprite = GameManager.instance.itemDirectory.weaponImages0[equipableItemSO.BaseType];
            weaponSpriteRenderer1.sprite = GameManager.instance.itemDirectory.weaponImages1[equipableItemSO.BaseType];
            projectileSprite0 = GameManager.instance.itemDirectory.projectileImages0[equipableItemSO.BaseType];
            projectileSprite1 = GameManager.instance.itemDirectory.projectileImages1[equipableItemSO.BaseType];
            weapon = equipableItemSO as WeaponSO;
            if ((weaponLight.color = GameManager.instance.itemDirectory.weaponLights[equipableItemSO.BaseType]) == Color.black)
            {
                weaponLight.enabled = false;
            }
            else
            {
                weaponLight.enabled = true;
            }
        }
    }

    public float GetBaseDamage()
    {
        playerRef.playerInfo.baseDamage += (playerRef.playerInfo.baseDamage * .1f * playerRef.playerInfo.GetPlayerLevel());
        return playerRef.playerInfo.baseDamage + (playerRef.playerInfo.baseDamage * .1f * playerRef.playerInfo.GetPlayerLevel());
    }

    public void Update()
    {
        ChangeSpriteLayer();
        UpdateWeaponPosition();
    }

    private void UpdateWeaponPosition()
    {
        Vector3 aimPosition = playerRef.playerLookAt.position;
        Vector3 rotation = aimPosition - playerRef.transform.position;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
        if (aimPosition.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ChangeSpriteLayer()
    {
        if (playerRef.isFacingBackwards)
        {
            weaponSpriteRenderer0.sortingOrder = -1;
            weaponSpriteRenderer1.sortingOrder = -1;
        }
        else
        {
            weaponSpriteRenderer0.sortingOrder = 1;
            weaponSpriteRenderer1.sortingOrder = 1;
        }
    }

    public void FireWeapon()
    {
        if (canFire)
        {
            weaponHolderAnimator.SetTrigger("Shoot");
            PlayerInfo playerInfo = playerRef.playerInfo;
            for (int i = 0; i < playerInfo.projectiles; i++)
            {
                Projectile p = Instantiate(projectilePrefab, firePosition.position,
                                Quaternion.identity, GameManager.instance.projectilesContainer);
                if (i == 0)
                {
                    p.audioSource.enabled = true;
                }
                p.lightColor = weaponLight.color;
                if (playerInfo.castSpeed > .4f)
                {
                    // Normal
                    p.lookAt = aimPositions[i];
                }
                else if (playerInfo.castSpeed > .1f)
                {   
                    // Wild
                    p.lookAt = aimPositions[Random.Range(0, aimPositions.Count/2)];
                }
                else
                {
                    // Unruly
                    p.lookAt = aimPositions[Random.Range(0, aimPositions.Count)];
                }
                p.spriteRenderer0.sprite = projectileSprite0;
                p.spriteRenderer1.sprite = projectileSprite1;
                p.damage = playerInfo.damage;
                p.castSpeed = playerInfo.castSpeed;
                p.velocityScalar = playerInfo.velocity;
                p.crit = playerInfo.crit;
                p.knockback = playerInfo.knockback;
                p.pierce = playerInfo.pierce;
                p.projectiles = playerInfo.projectiles;

                if(RollForCrit(p.crit))
                {
                    p.damage += p.damage;
                }
            }
            canFire = false;
            StartCoroutine(CastSpeedCooldown(Mathf.Clamp((float)Math.Round(playerInfo.castSpeed, 2), .01f, 50f)));
        }
    }

    private bool RollForCrit(float crit)
    {
        float roll = Random.Range(0, 100f);
        return (roll <= crit * 100f);
    }

    private IEnumerator CastSpeedCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        canFire = true;
    }

    private void OnDisable()
    {
        StopCoroutine("CastSpeedCooldown");
        canFire = true;
    }
}
