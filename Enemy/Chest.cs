using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Chest : MonoBehaviour
{
    [SerializeField]
    private DropItems itemDrop;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float baseRoll = 92f;
    [SerializeField]
    private float costToOpen;
    [SerializeField]
    private bool lootHasDropped = false;
    [SerializeField]
    private Light2D chestLight;
    [SerializeField]
    private SpriteRenderer chestSprite;
    [SerializeField]
    private InputActionReference interact;
    [SerializeField]
    private bool inZone = false;
    [SerializeField]
    private bool statusActive = false;
    [SerializeField]
    private StatusPopupAd statusPopupAdPrefab;
    private StatusPopupAd popup;

    private void Start()
    {
        float roll = Random.Range(0, 100f);
        if (roll <= 85f)
        {
            chestLight.color = Color.magenta;
            chestSprite.color = Color.magenta;
            costToOpen = 100f;
            baseRoll = 92f;
        }
        else
        {
            chestLight.color = new Color(1, .65f, 0, 1); // Orange
            chestSprite.color = new Color(1, .65f, 0, 1); // Orange
            costToOpen = 500f;
            baseRoll = 99f;
        }
    }

    private void Awake()
    {
        interact = GameManager.instance.player.interact;
    }

    private void OnEnable()
    {
        interact.action.performed += TriggerPrompt;
    }

    private void OnDisable()
    {
        interact.action.performed -= TriggerPrompt;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inZone = false;
        }
    }

    public void TriggerPrompt(InputAction.CallbackContext context)
    {
        if (inZone && !statusActive && !GameManager.instance.inMenu)
        {
            popup = Instantiate(statusPopupAdPrefab, GameManager.instance.popupsContainer);
            popup.parentChest = this;
            popup.chestCost = costToOpen;
            popup.costText.text = $"Open Chest (-{costToOpen})";
            GameManager.instance.inMenu = true;
            StartCoroutine(PreventStacking(popup));
        }
    }

    private IEnumerator PreventStacking(StatusPopupAd popup)
    {
        statusActive = true;
        while (popup != null)
        {
            yield return null;
        }
        GameManager.instance.inMenu = false;
        statusActive = false;
    }

    public void OpenChest()
    {
        if (GameManager.instance != null)
        {
            animator.SetTrigger("Open");
        }
        Destroy(popup.gameObject);
        StartCoroutine(CountdownToDestroy());
    }

    private IEnumerator CountdownToDestroy()
    {
        yield return new WaitForSeconds(.8f);
        if (!lootHasDropped)
        {
            itemDrop.GeneralDropLoot(transform, 1, baseRoll);
            lootHasDropped = true;
        }
        Destroy(transform.gameObject);
    }
}
