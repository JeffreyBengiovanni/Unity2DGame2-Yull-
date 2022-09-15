using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    private Transform lookAt;
    [SerializeField]
    private float boundX = 0.15f;
    [SerializeField]
    private float boundY = 0.05f;
    [SerializeField]
    private Transform playerRef;
    [SerializeField]
    private static CameraMovement objInstance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (objInstance == null)
        {
            objInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }

        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }
            else
            {
                delta.y = deltaY + boundY;
            }
        }

        transform.Translate(new Vector3((float)(delta.x * Time.deltaTime * 5), 
                                        (float)(delta.y * Time.deltaTime * 5), 
                                         0));

    }
}
