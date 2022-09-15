using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField]
    private Transform healthbarFill;
    [SerializeField]
    private Transform healthbarContainer;
    [SerializeField]
    private EnemyInfo enemyInfo;

    public void UpdateHealthbar()
    {
        float max = enemyInfo.GetMaxHealth();
        float current = enemyInfo.GetCurrentHealth();
        float ratio = current / max;
        if (ratio >= 1)
        {
            healthbarContainer.gameObject.SetActive(false);
        }
        else
        {
            healthbarContainer.gameObject.SetActive(true);
            healthbarFill.localScale = new Vector3(ratio, 1);
        }
    }
}
