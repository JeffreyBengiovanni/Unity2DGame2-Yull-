using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NextZoneTrigger : MonoBehaviour
{
    private InputActionReference interact;
    private bool inZone = false;

    private void Awake()
    {
        interact = GameManager.instance.player.interact;
    }

    private void OnEnable()
    {
        interact.action.performed += CreateNextZone;
    }

    private void OnDisable()
    {
        interact.action.performed -= CreateNextZone;
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

    private void CreateNextZone(InputAction.CallbackContext context)
    {
        if (inZone)
        {
            GameManager.instance.dungeonGenerator.GenerateDungeon();
        }
    }
}
