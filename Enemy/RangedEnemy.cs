using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField]
    private float baseDamage = 1000f;
    [SerializeField]
    private float damage = 1000f;
    [SerializeField]
    private float percentDamageIncrease = 1f;
    [SerializeField]
    private float castSpeed = 1.5f;
    [SerializeField]
    private float velocity = 1.5f;
    [SerializeField]
    private int projectileCount = 1;
    [SerializeField]
    private int weaponIndex = -1;
    [SerializeField]
    public bool randomSpread = true;

    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private Enemy enemyRef;
    [SerializeField]
    private Transform playerLocation;
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
    private Color weaponLightColor;
    [SerializeField]
    public Transform firePosition;
    [SerializeField]
    private Projectile projectilePrefab;
    [SerializeField]
    private bool canFire = true;
    [SerializeField]
    public float rangeUntilTrigger;
    [SerializeField]
    public List<Transform> aimPositions;
    [SerializeField]
    public Animator weaponHolderAnimator;

    private void Awake()
    {
        damage = baseDamage + (baseDamage * GameManager.instance.player.playerInfo.playerLevel * percentDamageIncrease);
        playerLocation = GameObject.Find("Player").transform;
        rangeUntilTrigger = enemyRef.detectionDistance * 2 / 3;
    }


    public void Start()
    {
        weaponIndex = Random.Range(0, GameManager.instance.itemDirectory.projectileImages0.Count);
        weaponLight.color = GameManager.instance.itemDirectory.weaponLights[weaponIndex];
        weaponSpriteRenderer0.sprite = GameManager.instance.itemDirectory.weaponImages0[weaponIndex];
        weaponSpriteRenderer1.sprite = GameManager.instance.itemDirectory.weaponImages1[weaponIndex];
        projectileSprite0 = GameManager.instance.itemDirectory.projectileImages0[weaponIndex];
        projectileSprite1 = GameManager.instance.itemDirectory.projectileImages1[weaponIndex];
    }

    public void Update()
    {
        if (Vector3.Distance(transform.position, playerLocation.position) <= rangeUntilTrigger * 2)
        {
            ChangeSpriteLayer();
            UpdateWeaponPosition();
        }
        if (Vector3.Distance(transform.position, playerLocation.position) <= rangeUntilTrigger) 
        {
            FireWeapon();
        }
    }

    private void UpdateWeaponPosition()
    {
        Vector3 rotation = enemyRef.aiPath.velocity;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        weaponHolder.rotation = Quaternion.Euler(0, 0, zRotation);
        if (rotation.x < 0)
        {
            weaponHolder.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            weaponHolder.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ChangeSpriteLayer()
    {
        if (enemyRef.isFacingBackwards)
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
            for (int i = 0; i < projectileCount; i++)
            {
                Projectile p = Instantiate(projectilePrefab, firePosition.position,
                                Quaternion.identity, GameManager.instance.projectilesContainer);

                p.lightColor = weaponLight.color;
                if (randomSpread)
                {
                    p.lookAt = aimPositions[Random.Range(0, aimPositions.Count)];
                }
                else
                {
                    p.lookAt = aimPositions[i];
                }
                p.spriteRenderer0.sprite = projectileSprite0;
                p.spriteRenderer1.sprite = projectileSprite1;
                p.damage = damage;
                p.castSpeed = castSpeed;
                p.velocityScalar = velocity;
                p.crit = 0;
                p.knockback = 1f;
                p.pierce = 0;
                p.projectiles = projectileCount;
                p.firePosition = firePosition;
                p.UpdateDirection();
            }
            canFire = false;
            StartCoroutine(CastSpeedCooldown(castSpeed));
        }
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
