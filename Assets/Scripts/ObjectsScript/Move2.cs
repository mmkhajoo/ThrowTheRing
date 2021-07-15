using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rod;

    public GameObject rod2;

    public Vector3 startPos, endPos;

    public Vector3 startPos2 ,endPos2;

    private Vector3 targetPos , targetPos2;

    public float speed;

    private float step;

    void Start()
    {
        targetPos = startPos;
        targetPos2 = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        
            Move_With_Vector();


    }

    public void Move_With_Vector()
    {
        step = speed * Time.deltaTime;
        if ((rod.transform.localPosition == startPos) && (rod2.transform.localPosition == startPos2))
        {
            targetPos = endPos;
            targetPos2 = endPos2;
        }
        else if ((rod.transform.localPosition == endPos) && (rod2.transform.localPosition == endPos2))
        {
            targetPos = startPos;
            targetPos2 = startPos2;
        }

        rod.transform.localPosition = Vector3.MoveTowards(rod.transform.localPosition, targetPos, step);
        rod2.transform.localPosition = Vector3.MoveTowards(rod2.transform.localPosition, targetPos2, step);
    }

}
