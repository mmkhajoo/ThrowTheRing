using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update

    // public GameObject rod;

    public Rigidbody rb;

    public Vector3 startPos, endPos;

    private Vector3 currentPos, targetPos;

    public float speed;

    private float step;

    // public bool isVector;

    public float offsetX; // how much the Object Move From The Start Position
    
    public float offsetY; // how much the Object Move From The Start Position

    public bool isHorizontal;

    public float time;

    private bool isDirectionChange;

    public bool isStartAtStart; // Set Start Position From Bottom Or Top Of The Path;


    private float t; //Calculate The Time For Moving Object


    public GameObject dot; // dot prefab For Show Path

    public float dotSeparation = 0.8f; // The Space Between Dots 

    // private List<GameObject> dots;

    public Vector3 firstPos; // the position game object instantiate

    private GameObject dotsParent;

    void Start()
    {
         firstPos = transform.position;
        // dots = new List<GameObject>();
        dot = Resources.Load<GameObject>("Dot");
         SetStartPosition();
         ShowPath();
    }
    

    public void SetStartPosition()
    {
        if (!isHorizontal)
        {
            startPos = new Vector3(firstPos.x-offsetX, firstPos.y - offsetY, firstPos.z);
            endPos = new Vector3(firstPos.x+offsetX, firstPos.y + offsetY, firstPos.z);
        }
        else
        {
            startPos = new Vector3(firstPos.x - offsetX, firstPos.y+offsetY, firstPos.z);
            endPos = new Vector3(firstPos.x + offsetX, firstPos.y-offsetY, firstPos.z);
        }

        if (isStartAtStart)
        {
            transform.position = startPos;
            targetPos = endPos;
            currentPos = startPos;
            isDirectionChange = true;
        }
        else
        {
            transform.position = endPos;
            targetPos = startPos;
            currentPos = endPos;
            isDirectionChange = false;
        }

        t = 0;
    }

    // public void StartState(int state,bool isHorizontal)
    // {
    //     switch (state)
    //     {
    //         case 0:
    //         {
    //             if (!isHorizontal)
    //             {
    //                 startPos = new Vector3(firstPos.x, firstPos.y - offsetY, firstPos.z);
    //                 endPos = new Vector3(firstPos.x, firstPos.y + offsetY, firstPos.z);
    //             }
    //             else
    //             {
    //                 startPos = new Vector3(firstPos.x - offsetX, firstPos.y, firstPos.z);
    //                 endPos = new Vector3(firstPos.x + offsetX, firstPos.y, firstPos.z);
    //             }
    //             break;
    //         }
    //         case 1:
    //         {
    //             if (!isHorizontal)
    //             {
    //                 startPos = new Vector3(firstPos.x-offsetX, firstPos.y - offsetY, firstPos.z);
    //                 endPos = new Vector3(firstPos.x+offsetX, firstPos.y + offsetY, firstPos.z);
    //             }
    //             else
    //             {
    //                 startPos = new Vector3(firstPos.x - offsetX, firstPos.y+offsetY, firstPos.z);
    //                 endPos = new Vector3(firstPos.x + offsetX, firstPos.y-offsetY, firstPos.z);
    //             }
    //             break;
    //         }
            // case 2:
            // {
            //     if (!isHorizontal)
            //     {
            //         startPos = new Vector3(firstPos.x-offsetX, firstPos.y - offsetY, firstPos.z);
            //         endPos = new Vector3(firstPos.x+offsetX, firstPos.y + offsetY, firstPos.z);
            //     }
            //     else
            //     {
            //         startPos = new Vector3(firstPos.x + offsetX, firstPos.y-offsetY, firstPos.z);
            //         endPos = new Vector3(firstPos.x - offsetX, firstPos.y+offsetY, firstPos.z);
            //     }
            //     break;
            // }

        // }
    // }

    // Update is called once per frame
    void Update()
    {
        // if (isVector)
        // {
        //     Move_With_Vector_Time();
        // }
        // else
        // {
        //     Move_With_RigidBody();
        // }
        Move_With_Vector_Time();
    }

    public void Move_With_Vector_Speed()
    {
        step = speed * Time.deltaTime;
        if (transform.position == startPos)
        {
            targetPos = endPos;
        }
        else if (transform.position == endPos)
        {
            targetPos = startPos;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    public void Move_With_Vector_Time()
    {
        if (t > 1 && isDirectionChange)
        {
            targetPos = startPos;
            currentPos = endPos;
            t = 0;
            isDirectionChange = !isDirectionChange;
        }
        else if (t > 1 && !isDirectionChange)
        {
            targetPos = endPos;
            currentPos = startPos;
            t = 0;
            isDirectionChange = !isDirectionChange;
        }

        t += Time.deltaTime / time;


        transform.position = Vector3.Lerp(currentPos, targetPos, t);
    }

    // public void Move_With_RigidBody()
    // {
    //     step = speed * Time.deltaTime;
    //
    //     rb.velocity = Vector3.up * step;
    //
    //     if (transform.position.y <= startPos.y && speed < 0)
    //     {
    //         speed = -speed;
    //     }
    //     else if (transform.position.y >= endPos.y && speed > 0)
    //     {
    //         speed = -speed;
    //     }
    // }
    public void ShowPath()
    {
        float x = endPos.x - startPos.x;

        float y = endPos.y - startPos.y;

        int number = (int) (Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) / dotSeparation);

        float xSeparation = x / number;

        float ySeparation = y / number;

        GameObject parent = Instantiate(dot, startPos, Quaternion.identity);

        dotsParent = parent;

        parent.name = gameObject.name + " " + "Dots";
        for (int i = 1; i < number; i++)
        {
            var temp = Instantiate(dot, startPos + new Vector3(xSeparation * i, ySeparation * i), Quaternion.identity);

            temp.transform.SetParent(parent.transform);
        }
    }

    public void OnDestroy()
    {
        Destroy(dotsParent);
    }
}