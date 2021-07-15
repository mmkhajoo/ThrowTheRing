using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class CameraController : MonoBehaviour
{
    public GenerateLevel generateLevel;

    public Transform cameraPos;

    public Transform bgCollectorTransform;

    private Vector3 _startPos; // the position of camera when game start For When Player Lose, We Back The Camera To The Start Position;

    private Vector3 startPos, endPos ,bgStartPos, bgEndPos;

    public float speed;
    
    public bool isCameraMove;

    public float addYDistance; // how much go higher than normal distance beetween rods;
    private float step;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 temp = cameraPos.transform.position;
        _startPos = temp;
        startPos = temp;
        endPos = temp;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isCameraMove)
        {
            step += speed * Time.fixedDeltaTime;
            cameraPos.transform.position = Vector3.MoveTowards(startPos, endPos, step);
            bgCollectorTransform.position = Vector3.MoveTowards(bgStartPos, bgEndPos, step);
        }

        if (Vector3.Distance(cameraPos.transform.position, endPos) < 0.001f)
        {
            isCameraMove = false;
        }
    }

    public void MoveCameraForNextThrow()
    {
        Vector3 endRodPosition = generateLevel.GetRodPosition(ObscuredPrefs.GetInt("RodCounter"));
        Vector3 startRodPosition = generateLevel.GetRodPosition(ObscuredPrefs.GetInt("RodCounter")-1);
        Debug.Log("StartRodPosition : " +startRodPosition);
        Debug.Log("EndRodPosition : " +endRodPosition);
        Vector3 targetPos = startRodPosition + (endRodPosition - startRodPosition);
        Debug.Log("Target : " +targetPos);
        
        SetPositions(cameraPos.transform.position ,new Vector3(endPos.x,targetPos.y + addYDistance ,endPos.z));
    }

    public void SetPositions(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        var bgPosition = bgCollectorTransform.position;
        bgStartPos = bgPosition;
        bgEndPos = bgPosition +bgCollectorTransform.up * (endPos.y - startPos.y);
        isCameraMove = true;
        step = 0;
    }

    // public void SetPositionsForBGCollector(Vector3 startPos, Vector3 endPos)
    // {
    //     
    // }
    

    public void BackToStartPosition()
    {
        SetPositions(cameraPos.transform.position,_startPos);
    }
}
