using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    public WeaponSystem weaponSystem;
    [SerializeField]
    public InventoryUIController inventoryController;
    [SerializeField]
    public EquipSystem equipSystem;
    [SerializeField]
    public PickUpSystem pickUpSystem;
    [SerializeField]
    private InputActionReference movement, pointerPosition, attack, dash, look;
    [SerializeField]
    public InputActionReference interact;
    [SerializeField]
    public PlayerInfo playerInfo;
    [SerializeField]
    public PlayerHealthBar playerHealthBar;
    [SerializeField]
    private Animator spriteAnimator, eyeAnimator;
    [SerializeField]
    private Transform playerSpriteTransform;
    [SerializeField]
    public Transform playerLookAt;
    public List<Transform> aimPositions;
    [SerializeField]
    public Transform playerSpriteAim;
    [SerializeField]
    private OnScreenButton interactButton, dashButton;

    [Header("Information")]
    [SerializeField]
    private float maxSpeed = 2, acceleration = 50, deacceleration = 100;
    [SerializeField]
    private float currentSpeed = 0;
    [SerializeField]
    private float cameraClampMagnitude = 4f;
    [SerializeField]
    private float cameraOffset = 3f;
    [SerializeField]
    private bool dashReady;
    [SerializeField]
    private float dashCooldown = 2f;
    [SerializeField]
    private float currentDashCooldown = 2f;
    [SerializeField]
    private float dashForce = 2f;
    [SerializeField]
    private bool dashDisabled = true;
    [SerializeField]
    private float clampDisableTime = .4f;
    [SerializeField]
    private float dashModifier = .2f;
    [SerializeField]
    public bool isFacingBackwards;
    [SerializeField]
    public bool isFiring = false;
    [SerializeField]
    private float damageImmunityTime = .4f;
    [SerializeField]
    private bool damageImmune = false;
    [SerializeField]
    private float knockbackImmunityTime = .8f;
    [SerializeField]
    private bool knockbackImmune = false;
    [SerializeField]
    private float knockbackTweak = .1f;
    [SerializeField]
    private bool isMobile = false;

    private Vector2 oldMovementInput;
    public Vector2 movementInput;
    public Vector3 lookInput, previousLook, lookValue;
    private bool shootWeapon;
    private Vector2 MovementInput { get; set; }

    Vector3 mouseInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            isMobile = true;
            GameManager.instance.ToggleMobileControls(true);
        }
        else
        {
            isMobile = false;
            GameManager.instance.ToggleMobileControls(false);
        }
        /*
        if (Input.GetJoystickNames().Length > 0)
        {
            isController = true;
        }
        */
        aimPositions = weaponSystem.aimPositions;
        playerHealthBar.UpdateHealthbar();
        ToggleOnScreenButton(false, interactButton);
    }

    private void OnEnable()
    {
        attack.action.performed += AttackEnable;
        attack.action.canceled += AttackDisable;
        dash.action.performed += Dash;
    }

    private void OnDisable()
    {
        attack.action.performed -= AttackEnable;
        dash.action.performed -= Dash;
    }


    private void AttackEnable(InputAction.CallbackContext context)
    {
        isFiring = true;
    }

    private void AttackDisable(InputAction.CallbackContext context)
    {
        isFiring = false;
    }

    private void ToggleAttack(bool toggle)
    {
        isFiring = toggle;
    }

    private void PerformAttack()
    {
        if (isFiring)
        {
            weaponSystem.FireWeapon();
        }
    }

    private void Update()
    {
        PlayerInputHandler();
        CheckDashCooldown();
    }

    private void PlayerInputHandler()
    {
        if (!inventoryController.inventoryActive)
        {
            GetInputs();
            PerformAttack();
        }
        else
        {
            MovementInput = Vector2.zero;
        }
    }

    private void CheckDashCooldown()
    {
        if (!dashReady)
        {
            if (currentDashCooldown >= dashCooldown)
            {
                dashReady = true;
                ToggleOnScreenButton(true, dashButton);
            }
            else
            {
                currentDashCooldown += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void GetInputs()
    {
        if (isMobile)
            AnalogCamera();
        else
            GetPointerInput();

        movementInput = movement.action.ReadValue<Vector2>();
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            MovementInput = movementInput;
        }
        else
        {
            MovementInput = movementInput.normalized;
        }
        if (movementInput != Vector2.zero)
        {
            spriteAnimator.SetFloat("Speed", 1);
            eyeAnimator.SetFloat("Speed", 1);
        }
        else
        {
            spriteAnimator.SetFloat("Speed", 0);
            eyeAnimator.SetFloat("Speed", 0);
        }
    }

    private void GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        mouseInput = Camera.main.ScreenToWorldPoint(mousePos);
        playerSpriteAim.position = mouseInput;
        AdjustSprite(playerSpriteAim.position);
        AdjustCamera(playerSpriteAim.position);
    }

    private void AnalogCamera()
    {
        lookInput = look.action.ReadValue<Vector2>();
        if (lookInput == Vector3.zero)
        {
            lookValue = previousLook.normalized;
            lookValue.z = Camera.main.nearClipPlane;
        }
        else
        {
            lookValue = lookInput;
            previousLook = lookInput;
            lookValue.z = Camera.main.nearClipPlane;
            lookValue = lookValue * 5f;
        }
        if (lookValue.magnitude > 4f && (lookInput != Vector3.zero))
        {
            ToggleAttack(true);
        }
        else
        {
            ToggleAttack(false);
        }
        playerSpriteAim.localPosition = lookValue;
        AdjustSprite(playerSpriteAim.position);
        AdjustCamera(playerSpriteAim.position);
    }

    private void AdjustCamera(Vector3 aimPosition)
    {
        Vector3 additive = aimPosition - transform.position;
        Vector3 temp = new Vector3(additive.x / cameraOffset, additive.y / cameraOffset, 0);
        playerLookAt.position = transform.position + Vector3.ClampMagnitude(temp, cameraClampMagnitude);
        for (int i = 0; i < aimPositions.Count; i++)
        {
            if (playerInfo.projectiles > i)
            {
                aimPositions[i].gameObject.SetActive(true);
            }
            else
            {
                aimPositions[i].gameObject.SetActive(false);
            }
            //Vector3 additive1 = (aimPosition + new Vector3(0, (Mathf.Pow(-1, i) * Mathf.CeilToInt(i/2f)))) - transform.position;
            //Vector3 temp1 = new Vector3(additive1.x / cameraOffset, additive1.y / cameraOffset, 0);
            //aimPositions[i].position = transform.position + Vector3.ClampMagnitude(temp1, cameraClampMagnitude);
            //aimPositions[i].position = new Vector3(0, Mathf.Pow(-1, i) * Mathf.CeilToInt(i / 2f) / 20f);
            float yValue = (Mathf.Pow(-1, i) * Mathf.CeilToInt(i / 2f) / 15f) * Vector3.Distance(weaponSystem.firePosition.position, playerLookAt.position);
            float xValue = (-1 * Mathf.Pow(1.4f, Mathf.CeilToInt(i / 2f)) / 30f) * Vector3.Distance(weaponSystem.firePosition.position, playerLookAt.position);

            aimPositions[i].localPosition = new Vector3(xValue, yValue, 0);
        }
        Vector3 rotation = aimPosition - transform.position;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        playerLookAt.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    private void AdjustSprite(Vector3 aimPosition)
    {
        if (aimPosition.y - transform.position.y > 0)
        {
            spriteAnimator.SetBool("FacingBackwards", true);
            eyeAnimator.SetBool("FacingBackwards", true);
            isFacingBackwards = true;
        }
        else
        {
            spriteAnimator.SetBool("FacingBackwards", false);
            eyeAnimator.SetBool("FacingBackwards", false);
            isFacingBackwards = false;
        }
        if (aimPosition.x - transform.position.x  < 0)
        {
            playerSpriteTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            playerSpriteTransform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void Death()
    {
        GameManager.instance.saveManager.SaveData();
        Revive();
    }

    public void Revive()
    {
        playerInfo.ResetHealth();
        GameManager.instance.dungeonGenerator.GenerateDungeon();
        GameManager.instance.saveManager.LoadData();
    }

    public void DamageTaken(float damage, float knockback, Transform source, bool isCrit)
    {
        if (!damageImmune && dashDisabled)
        {
            damage = damage - playerInfo.defense;
            if (damage < 0 )
            {
                damage = 1;
            }
            playerInfo.SubtractDamage(damage);
            playerHealthBar.UpdateHealthbar();
            spriteAnimator.SetTrigger("Hit");
            eyeAnimator.SetTrigger("Hit");
            StartCoroutine(RefreshDamage(damageImmunityTime));
            if (knockback != 0)
            {
                ApplyKnockback(knockback, source);
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            ToggleOnScreenButton(true, interactButton);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            ToggleOnScreenButton(false, interactButton);
        }
    }

    private void ToggleOnScreenButton(bool toggle, OnScreenButton button)
    {
        if (toggle)
        {
            button.enabled = true;
            button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            button.transform.Find("GameObject").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            button.enabled = false;
            button.GetComponent<Image>().color = new Color(1, 1, 1, .3f);
            button.transform.Find("GameObject").GetComponent<Image>().color = new Color(1, 1, 1, .3f);
        }
    }

    private void ApplyKnockback(float knockback, Transform source)
    {
        if (!knockbackImmune)
        {
            Vector2 pushDirection = (transform.position - source.position).normalized;
            rb.AddForce(pushDirection * knockback * knockbackTweak, ForceMode2D.Impulse);
            StartCoroutine(RefreshKnockback(knockbackImmunityTime));
        }
    }

    private IEnumerator RefreshKnockback(float duration)
    {
        knockbackImmune = true;
        yield return new WaitForSeconds(duration);
        knockbackImmune = false;
    }

    private IEnumerator RefreshDamage(float duration)
    {
        damageImmune = true;
        yield return new WaitForSeconds(duration);
        damageImmune = false;
    }

    private void UpdateMovement()
    {
        maxSpeed = Mathf.Clamp(playerInfo.movement, 1, 8);
        if(MovementInput.magnitude > 0 && currentSpeed >= 0)
        {
            oldMovementInput = MovementInput;
            currentSpeed += acceleration * maxSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deacceleration * maxSpeed * Time.deltaTime;
        }
        if (dashDisabled)
        {
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed + dashForce, dashModifier);
        }
        rb.velocity = oldMovementInput * currentSpeed;
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && dashReady && 
            oldMovementInput != Vector2.zero && 
            !inventoryController.inventoryActive)
        {
            spriteAnimator.SetTrigger("Dashing");
            eyeAnimator.SetTrigger("Dashing");
            oldMovementInput = MovementInput;
            //rb.AddForce(oldMovementInput * dashForce);
            currentDashCooldown = 0;
            dashReady = false;
            dashDisabled = false;
            ToggleOnScreenButton(false, dashButton);
            StartCoroutine(DisableSpeedClamp());
        }
    }

    private IEnumerator DisableSpeedClamp()
    {
        yield return new WaitForSeconds(clampDisableTime);
        dashDisabled = true;
    }
}
