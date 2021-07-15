using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    public GameObject nodePrefab;

    public int sizeX;
    public int sizeY;
    public float offset;

    public float distance;

    public Node[,] grid;

    public SubNode[,] subGrid;


//static instance------------------


    // Use this for initialization
    void Start()
    {
        CreateGrid();
        CreateMouseOrTouchCollision();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateGrid()
    {
        grid = new Node[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                float posX = x * offset;
                float posy = y * offset;

                GameObject temp = Instantiate(nodePrefab, new Vector3(posX, posy, distance), Quaternion.identity);
                temp.transform.parent = transform.GetChild(0).transform;
                //NodeObject------------
                //---------------Sign Node----------------------//
                Node node = new Node {posX = x, posY = y, quad = temp};
                //----------------End Sign Node-----------------//
                grid[x, y] = node;
            }
        }
    }

//    public void CreateSubGrid()
//    {
//        subGrid = new SubNode[sizeX*2, sizeY*2];
//
//        for (int x = 0; x < sizeX*2; x++)
//        {
//            for (int z = 0; z < sizeY*2; z++)
//            {
//                float posX = x * offset;
//                float posZ = z * offset;
//
//                GameObject temp = Instantiate(nodePrefab, new Vector3(posX, 0f, posZ), Quaternion.identity);
//                temp.transform.parent = transform.GetChild(0).transform;
//                //NodeObject------------
//                //---------------Sign Node----------------------//
//                Node node = new Node {posX = x, posZ = z, quad = temp};
//                //----------------End Sign Node-----------------//
//                grid[x, z] = node;
//            }
//        }
//    }

    public void CreateMouseOrTouchCollision()
    {
        GameObject temp = new GameObject {name = "MouseORTouchCollider"};
        temp.AddComponent<BoxCollider>();
        temp.GetComponent<BoxCollider>().size = new Vector3(sizeX * offset, sizeY * offset, 0.1f);
        temp.transform.position = new Vector3(sizeX * offset / 2f - offset / 2.0f, sizeY * offset / 2f - offset / 2.0f, 0
        );
    }

    public Vector3 ObjectPosFromWorldPosition(Vector3 worldPosition)
    {
        return new Vector3(worldPosition.x,worldPosition.y,0f);
    }


    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        float worldX = worldPosition.x;
        float worldY = worldPosition.y;

        worldX /= offset;
        worldY /= offset;

        int x = Mathf.RoundToInt(worldX);
        int y = Mathf.RoundToInt(worldY);

        if (x > sizeX)
            x = sizeX;
        else if (x < 0)
            x = 0;

        if (y > sizeY)
            y = sizeY;
        else if (y < 0)
            y = 0;


        return grid[x, y];
    }
}