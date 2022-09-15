using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform enemySpriteTransform;
    [SerializeField]
    private DropItems enemyDropSystem;
    [SerializeField]
    public AIPath aiPath;
    [SerializeField]
    public float detectionDistance = 6f;
    [SerializeField]
    public float leashDistance = 12f;
    //[SerializeField]
    //private float tooCloseDistance = 1f;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform emptyTransform;
    [SerializeField]
    private AIDestinationSetter aiDestinationSetter;

    [Header("Information")]
    [SerializeField]
    private bool isToggledOff = false;
    [SerializeField]
    private bool isToggledOn = true;
    [SerializeField]
    private float playerDistance;
    [SerializeField]
    public EnemyInfo enemyInfo;
    [SerializeField]
    public EnemyHealthbar enemyHealthbar;
    [SerializeField]
    private float knockbackDuration = .1f;
    [SerializeField]
    private float knockbackImmunityTime = .4f;
    [SerializeField]
    private bool knockbackImmune = false;
    [SerializeField]
    private float knockbackTweak = .1f;
    [SerializeField]
    private float enemyDropChance = 15f;
    [SerializeField]
    public bool isFacingBackwards = false;

    public Vector2 movementInput;
    private bool shootWeapon;
    private Vector2 MovementInput { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        enemyDropSystem = GetComponent<DropItems>();
        aiPath.maxSpeed = enemyInfo.GetMovementSpeed();
        aiDestinationSetter.target = null;
        startPosition = Instantiate(emptyTransform, transform.position, Quaternion.identity, GameObject.Find("EnemyStartLocations").transform).transform;
    }

    private void Start()
    {
        enemyHealthbar.UpdateHealthbar();
        playerTransform = GameObject.Find("Player").transform;
        enemySpriteTransform.GetComponent<SpriteRenderer>().color = enemyInfo.enemyColors[Random.Range(0, enemyInfo.enemyColors.Count)];
    }

    public bool DamageTaken(float damage, float knockback, Transform source, bool isCrit)
    {
        if (enemyInfo.GetCurrentHealth() > 0)
        {
            enemyInfo.SubtractDamage(damage);
            enemyInfo.ShowDamage(damage, transform);
            enemyHealthbar.UpdateHealthbar();
            animator.SetTrigger("Hit");
            if (knockback != 0)
            {
                ApplyKnockback(knockback, source);
            }
            return true;
        }
        return false;
    }

    private void AdjustSprite(Vector2 aimPosition)
    {
        animator.SetFloat("Speed", Mathf.Abs(aimPosition.magnitude));
        if (aimPosition.y > 0)
        {
            animator.SetBool("FacingBackwards", true);
            isFacingBackwards = true;
        }
        else
        {
            animator.SetBool("FacingBackwards", false);
            isFacingBackwards = false;
        }
        if (aimPosition.x < 0)
        {
            enemySpriteTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            enemySpriteTransform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ApplyKnockback(float knockback, Transform source)
    {
        if (!knockbackImmune)
        {
            Vector2 pushDirection = (transform.position - source.position).normalized;
            rb.AddForce(pushDirection * knockback * knockbackTweak, ForceMode2D.Impulse);
            StartCoroutine(KnockedBack(knockbackDuration));
            StartCoroutine(RefreshKnockback(knockbackImmunityTime));
        }
    }

    private IEnumerator RefreshKnockback(float duration)
    {
        knockbackImmune = true;
        yield return new WaitForSeconds(duration);
        knockbackImmune = false;
    }

    private IEnumerator KnockedBack(float duration)
    {
        aiPath.enabled = false;
        yield return new WaitForSeconds(duration);
        aiPath.enabled = true;
    }

    private void PeformAttack()
    {
        // Trigger attack event;
    }

    private void FixedUpdate()
    {
        UpdateEnemy();
    }

    private void UpdateEnemy()
    {
        PlayerNearby();
        AdjustSprite(aiPath.velocity);
        if (isToggledOn)
        {
            UpdateMovement();
        }
    }

    private void PlayerNearby()
    {
        playerDistance = Vector3.Distance(playerTransform.position, transform.position);
        if (playerDistance >= leashDistance * 1.5)
        {
            if (isToggledOn)
            {
                ToggleEnemy(false);
            }
        }
        else
        {
            if (isToggledOff)
            {
                ToggleEnemy(true);
            }
        }
        enemyHealthbar.UpdateHealthbar();
    }

    private void ToggleEnemy(bool toggle)
    {

        aiPath.enabled = toggle;
        aiDestinationSetter.enabled = toggle;
        enemyInfo.enabled = toggle;
        enemyDropSystem.enabled = toggle;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(toggle);
        }
      
        if (toggle == false)
        {
            isToggledOff = true;
            isToggledOn = false;
        }
        else
        {
            isToggledOff = false;
            isToggledOn = true;
        }
    }

    private void UpdateMovement()
    {
        if (playerDistance <= detectionDistance && playerDistance <= leashDistance)
        {
            aiDestinationSetter.target = playerTransform;
        }
        else
        {
            if(Vector3.Distance(startPosition.position, transform.position) <= .5)
            {
                aiDestinationSetter.target = null;
            }
            else
            {
                aiDestinationSetter.target = startPosition;
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null && enemyInfo.GetCurrentHealth() <= 0)
        {
            GameManager.instance.player.playerInfo.AddXP(enemyInfo.GetXPValue());
            float roll = Random.Range(0, 100);
            if (roll <= enemyDropChance || enemyInfo.alwaysDropsLoot)
            {
                enemyInfo.enemyDropSystem.GeneralDropLoot(transform, 1, enemyInfo.GetBaseRoll());
            }
        }
        if (startPosition != null)
        {
            Destroy(startPosition.gameObject);
            StopAllCoroutines();
        }
    }
}
