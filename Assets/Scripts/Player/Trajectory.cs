using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    
    
    public float dotShift;						//How far the first dot is from the "ball"
    public float dotSeparation;					//The space between the points representing the trajectory
    public int numberOfDots;                  // how many dots show for path
    public float initialDotSize;                // Size Of Dots
    public GameObject trajectoryDots;
    public Vector3 shotForce;                   //how much velocity added to Torus
    public Transform PlayerPos;                  // Torus Position
    public GameObject[] dots;                   // Dots Shows The Path
    public SpriteRenderer[] dots_Sprite ;
    [SerializeField]
    private Color baseColor = Color.white;


    // Start is called before the first frame update
    void Start()
    {
        // trajectoryDots = GameObject.FindGameObjectWithTag("Trajectory");
        
        SetDotsAlphaColor();
        trajectoryDots.transform.localScale = new Vector3(initialDotSize , initialDotSize , trajectoryDots.transform.localScale.z);

        for (int i = numberOfDots; i < dots.Length; i++)
        {
            dots[i].SetActive(false);
        }
        trajectoryDots.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDotsAlphaColor()
    {
        int temp = numberOfDots;
        for (int i = 0; i < numberOfDots; i++)
        {
            baseColor.a = (float) temp / numberOfDots;
            dots_Sprite[i].color = baseColor;
            temp--;
        }
    }

    public void SetDots(Vector3 startPos , Vector3 shotForce)
    {
        if (numberOfDots > dots.Length)
        {
            numberOfDots = dots.Length;
        }
        for (var k = 0; k < numberOfDots; k++) {							//Each point of the trajectory will be given its position
            var x1 = startPos.x + shotForce.x * Time.fixedDeltaTime * (dotSeparation * k + dotShift);
            var y1 = startPos.y + shotForce.y * Time.fixedDeltaTime * (dotSeparation * k + dotShift) - (-Physics2D.gravity.y/2f * Time.fixedDeltaTime * Time.fixedDeltaTime * (dotSeparation * k + dotShift) * (dotSeparation * k + dotShift));
            dots [k].transform.position = new Vector3 (x1, y1, dots [k].transform.position.z);	//Position is applied to each point
        }
    }

    public void DeActive_Trajectory_Dots()
    {
        trajectoryDots.SetActive(false);
    }

    public void Active_Trajectory_Dots()
    {
        trajectoryDots.SetActive(true);
    }
}
