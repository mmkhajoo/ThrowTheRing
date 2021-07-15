using System;
using System.Collections;
using System.Collections.Generic;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class Level_GameObject : MonoBehaviour
{
    // Use this for initialization
    public string Obj_id;

    public bool isSubObject;

    public int subNumber;

    public int gridPosX;

    public int gridPosZ;

    public GameObject modelVisualization;

    public Vector3 worldPositionOffset;

    public Vector3 worldRotation;
    
    public float rotateDegrees;


    public bool isHasMoveFeature;
    public MoveFeature moveFeature;


    public bool isHasRotateFeature;
    public RotateFeature rotateFeature;

    private Vector3 startPos;

    //-----------------Score --------------------//


    // public String scoreType;
    //
    // public float[] rangeNumbers;
    //
    // public int[] scores;
    //
    // public bool isBall;
    
    
    //---------------End Score --------------//
    

    public void UpdateNode(Node[,] grid)
    {
        Node node = grid[gridPosX, gridPosZ];

        Vector3 worldPosition = node.quad.transform.position;

        transform.rotation = Quaternion.Euler(worldRotation);

        transform.position = worldPosition;
    }

    public void ChangeRotation()
    {
        Vector3 eulerAngels = transform.eulerAngles;
        
        eulerAngels += new Vector3(0,rotateDegrees,0);
        
        transform.localRotation = Quaternion.Euler(eulerAngels);
    }

    public SaveableLevelObject GetSaveableObject()
    {
        SaveableLevelObject saveObj = new SaveableLevelObject();

        saveObj.obj_id = Obj_id;
        saveObj.nodePosX = gridPosX;
        saveObj.nodePosZ = gridPosZ;
        
        var transform1 = transform;
        
        saveObj.posX = startPos.x;
        saveObj.posY = startPos.y;
        saveObj.posZ = startPos.z;

        worldRotation = transform1.localEulerAngles;
 
        saveObj.rotX = worldRotation.x;
        saveObj.rotY = worldRotation.y;
        saveObj.rotZ = worldRotation.z;

        var localScale = transform1.localScale;
        saveObj.scaleX = localScale.x;
        saveObj.scaleY = localScale.y;
        saveObj.scaleZ = localScale.z;

        if (isSubObject)
        {
            saveObj.isSubObject = isSubObject;
            saveObj.subNumber = subNumber;
        }

        if (isHasMoveFeature)
        {
            saveObj.moveFeature = new MoveFeature(moveFeature.offsetX,moveFeature.offsetY,
                moveFeature.time,moveFeature.isHorizontal,moveFeature.isStartAtStart);
            saveObj.isHasMoveFeature = true;
        }

        if (isHasRotateFeature)
        {
            saveObj.rotateFeature = new RotateFeature(rotateFeature.time);
            saveObj.isHasRotateFeature = true;
        }
        

        return saveObj;
    }
    

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
}
[Serializable]
public class SaveableLevelObject
{
    public string obj_id;

    public bool isSubObject;

    public int subNumber;

    public int nodePosX;

    public int nodePosZ;

    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;
    
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    
    public bool isHasMoveFeature;
    public MoveFeature moveFeature;
    
    public bool isHasRotateFeature;
    public RotateFeature rotateFeature;

}
        
