using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public Rigidbody2D rb;
    [SerializeField]
    public SpriteRenderer spriteRenderer0;
    [SerializeField]
    public SpriteRenderer spriteRenderer1;
    [SerializeField]
    public Transform spriterRendererHolder;
    [SerializeField]
    public HashSet<Collider2D> collidedWith = new HashSet<Collider2D>();
    [SerializeField]
    public Player playerRef;
    [SerializeField]
    public Light2D lightInstance;
    [SerializeField]
    public Transform hitEffectPrefab;
    [SerializeField]
    public Transform firePosition;
    [SerializeField]
    public AudioSource audioSource;

    [Header("Projectile Data")]
    [SerializeField]
    public float damage = 0;
    [SerializeField]
    public float castSpeed = 0;
    [SerializeField]
    public float velocityScalar = 0;
    [SerializeField]
    public float crit = 0;
    [SerializeField]
    public float knockback = 0;
    [SerializeField]
    public float pierce = 0;
    [SerializeField]
    public int projectiles = 0;
    [SerializeField]
    public float duration = 2f;
    [SerializeField]
    public float currentPierced = 0;
    [SerializeField]
    public Vector3 direction;
    [SerializeField]
    public Transform lookAt;
    [SerializeField]
    public Color lightColor;
    [SerializeField]
    public bool collisionCooldown = false;
    [SerializeField]
    public bool enemyProjectile = false;

    private void Start()
    {
        playerRef = GameManager.instance.player;
        UpdateDirection();
        if (enemyProjectile)
        {
            ChangeSpriteAngleEnemy();
            lightInstance.color = Color.red;
        }
        else
        {
            ChangeSpriteAngle();
            lightInstance.color = lightColor;
        }
        if (pierce < 0)
        {
            pierce = 0;
        }
        StartCoroutine(ProjectileDuration(duration));
    }

    public void UpdateDirection()
    {
        if (enemyProjectile)
        {

            direction = (lookAt.position - firePosition.position).normalized;
        }
        else
        {
            direction = (lookAt.position - playerRef.weaponSystem.firePosition.position).normalized;
        }
        rb.velocity = (Vector2)direction * (velocityScalar * 2f);
    }

    private void ChangeSpriteAngle()
    {
        Vector3 aimPosition = lookAt.position;
        Vector3 rotation = aimPosition - playerRef.weaponSystem.firePosition.position;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        spriteRenderer0.transform.rotation = Quaternion.Euler(0, 0, zRotation);
        spriteRenderer1.transform.rotation = Quaternion.Euler(0, 0, zRotation);

        if (aimPosition.x - transform.position.x < 0)
        {
            spriteRenderer0.transform.localScale = new Vector3(1, -1, 1);
            spriteRenderer1.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            spriteRenderer0.transform.localScale = new Vector3(1, 1, 1);
            spriteRenderer1.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ChangeSpriteAngleEnemy()
    {
        Vector3 rotation = direction;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, zRotation);
        if (rotation.x < 0)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyProjectile)
        {
            AttackingPlayerCollision(collision);
        }
        else
        {
            AttackingEnemyCollision(collision);
        }
        
    }

    private void AttackingEnemyCollision(Collider2D collision)
    {
        if (!collidedWith.Contains(collision))
        {
            collidedWith.Add(collision);
            Enemy enemyRef = collision.transform.GetComponentInParent<Enemy>();
            if (enemyRef != null)
            {
                bool landedHit = enemyRef.DamageTaken(damage, knockback, transform, false);
                if (landedHit)
                {
                    CreateHitEffect(transform.position);
                    currentPierced++;
                    if (currentPierced >= pierce)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (collision.tag == "Breakable")
            {
                BreakableObject breakable = collision.GetComponentInParent<BreakableObject>();
                if (breakable != null)
                {
                    breakable.BreakObject();
                }
                CreateHitEffect(transform.position);
                currentPierced++;
                if (currentPierced >= pierce)
                {
                    Destroy(gameObject);
                }
            }
        }

        if (collision.tag == "Immovable")
        {
            CreateHitEffect(transform.position);
            Destroy(gameObject);
        }
    }

    private void AttackingPlayerCollision(Collider2D collision)
    {
        if (!collidedWith.Contains(collision))
        {
            collidedWith.Add(collision);
            Player playerRef = collision.transform.GetComponentInParent<Player>();
            if (playerRef != null)
            {
                playerRef.DamageTaken(damage, knockback, transform, false);
                CreateHitEffect(transform.position);
                currentPierced++;
                if (currentPierced >= pierce)
                {
                    Destroy(gameObject);
                }
            }
            /*
            else if (collision.tag == "Breakable")
            {
                BreakableObject breakable = collision.GetComponentInParent<BreakableObject>();
                if (breakable != null)
                {
                    breakable.BreakObject();
                }
                CreateHitEffect(transform.position);
                currentPierced++;
                if (currentPierced >= pierce)
                {
                    Destroy(gameObject);
                }
            }
            */
        }

        if (collision.tag == "Immovable")
        {
            CreateHitEffect(transform.position);
            Destroy(gameObject);
        }
    }

    private void CreateHitEffect(Vector3 location)
    {
        Instantiate(hitEffectPrefab, location, Quaternion.identity, transform.parent);
    }

    private IEnumerator ProjectileDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        CreateHitEffect(transform.position);
        Destroy(transform.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
