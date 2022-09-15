using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private DropItems itemDrop;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float chanceToDrop = 10f;
    [SerializeField]
    private bool lootHasDropped = false;

    public void BreakObject()
    {
        if (GameManager.instance != null)
        {
            animator.SetTrigger("Hit");
            if (!lootHasDropped)
            {
                float roll = Random.Range(0, 100);
                if (roll <= chanceToDrop)
                {
                    itemDrop.GeneralDropLoot(transform, 1, 0);
                }
                lootHasDropped = true;
            }
        }
        StartCoroutine(CountdownToDestroy());
    }

    private IEnumerator CountdownToDestroy()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(transform.gameObject);
    }

}
