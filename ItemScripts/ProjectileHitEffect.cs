using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitEffect : MonoBehaviour
{
    [SerializeField]
    private float duration = 1f;

    private void Start()
    {
        StartCoroutine(Countdown(duration));
    }

    private IEnumerator Countdown(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
