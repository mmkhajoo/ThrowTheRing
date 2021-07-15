using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRing : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CenterPoint;

    public GameObject piece_Of_Torus;

    public float radius;

    public int angle_Per_Piece;
    void Start()
    {
       Set_Position_To_Pieces(radius,CenterPoint,angle_Per_Piece);
    }

    // Update is called once per frame
    void Update()
    {
//        test.transform.LookAt(CenterPoint.transform);
    }


    public void Set_Position_To_Pieces(float radius,GameObject center , int angle_Per_Piece)
    {
        float posZ, posX , angle; 

        Vector3 temp;

        for (int i = 0; i < 360; i+=angle_Per_Piece)
        {
            angle = (float)(i/180.0) * Mathf.PI;
                
            posZ = Mathf.Sin(angle) * radius;

            posX = Mathf.Cos(angle) * radius;

            temp = center.transform.position + center.transform.TransformDirection(new Vector3(posX, 0, posZ));

            GameObject piece = Instantiate(piece_Of_Torus, temp, Quaternion.identity);
            
            piece.transform.LookAt(center.transform);
            
            piece.transform.Rotate(new Vector3(0,90,0));
            
            piece.transform.SetParent(center.transform);
        }
        
        
        
    }
}
