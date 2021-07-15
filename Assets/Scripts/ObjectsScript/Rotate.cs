using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update

    public float time;

    private float t;
    
    [SerializeField] private Vector3 targetAngle;

    private Quaternion currentRotation;

    private Quaternion targetRotation;

    void Start()
    {
        t = 0;
        // targetAngle = new Vector3(0, 0, 360);
        targetRotation = Quaternion.Euler(targetAngle);
        currentRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        QuaternionRotate();
    }

    public void QuaternionRotate()
    {
        if (t > 1)
        {
            // transform.rotation = Quaternion.Euler(Vector3.zero);
            currentRotation = targetRotation;
            
            targetAngle.z += 90f;
            
            targetRotation = Quaternion.Euler(targetAngle);
            
            t = 0;
        }

        t += Time.deltaTime / time;

        transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, t);
    }
}