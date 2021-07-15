using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLevelEditor : MonoBehaviour
{
    // Use this for initialization
    public float rate;

    public MoveCameraButton up;

    public MoveCameraButton down;

    private Vector3 startPos;

    public Transform bgCollectorTransform;

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (up.isDown)
        {
            Vector3 temp = transform.position;
            temp.y += rate;
            if (startPos.y > temp.y)
            {
                transform.position = startPos;
            }
            else
            {
                transform.position = temp;
                bgCollectorTransform.position += bgCollectorTransform.up * rate;
            }
        }

        if (down.isDown)
        {
            Vector3 temp = transform.position;
            temp.y -= rate;
            if (startPos.y > temp.y)
            {
                transform.position = startPos;
            }
            else
            {
                transform.position = temp;
                bgCollectorTransform.position -= bgCollectorTransform.up * rate;
            }
        }
    }
}