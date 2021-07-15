using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCollector : MonoBehaviour
{
    [SerializeField] private GameObject[] BackGrounds;

    [SerializeField] private float lastY;
    [SerializeField] private Vector3 lastBackGroundPos;

    public bool isPickHigher; // when the camera come down  backgrounds must be change with lowest; 

    void Awake()
    {
        BackGrounds = GameObject.FindGameObjectsWithTag("BackGround");
    }

    // Use this for initialization
    void Start()
    {
        PickCurrentBackGround();
    }

    public void PickCurrentBackGround()
    {
        lastY = BackGrounds[0].transform.position.y;
        lastBackGroundPos = BackGrounds[0].transform.position;
        for (int i = 1; i < BackGrounds.Length; i++)
        {
            if (isPickHigher)
            {
                if (BackGrounds[i].transform.position.y > lastY)
                {
                    lastY = BackGrounds[i].transform.position.y;
                    lastBackGroundPos = BackGrounds[i].transform.position;
                }
            }
            else
            {
                if (BackGrounds[i].transform.position.y <= lastY)
                {
                    lastY = BackGrounds[i].transform.position.y;
                    lastBackGroundPos = BackGrounds[i].transform.position;
                }
            }
            
        }
    }

    // Update is called once per fram
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Called");
        if (other.gameObject.CompareTag("BackGround"))
        {
            other.gameObject.SetActive(false);
            Vector3 temp;
            float width = ((BoxCollider) other).size.y;
            PickCurrentBackGround();
            for (int i = 0; i < BackGrounds.Length; i++)
            {
                if (!BackGrounds[i].activeInHierarchy)
                {
                    if (isPickHigher)
                    {
                        temp = BackGrounds[i].transform.up * (width * 2) + lastBackGroundPos;
                        Debug.Log(temp);
                    }
                    else
                    {
                        temp = -BackGrounds[i].transform.up * (width * 2) + lastBackGroundPos;
                    }
                    BackGrounds[i].transform.position = temp;
                    lastBackGroundPos = BackGrounds[i].transform.position;
                    BackGrounds[i].SetActive(true);
                }
            }
        }
    }
}