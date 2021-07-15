using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Swipe : MonoBehaviour
{

    public Camera cam;

    public GameManager gameManager;
    
    public Vector3 startPos,movedPos , endPos, direction;

    public Transform playerPos;

    public float startTime, endTime, intervalTime;

    public float multiple_Vector;

    public TorusPhysic torusPhysic;

    public Trajectory trajectory;

    public InterfaceManager interfaceManager;

    // private bool startPos_Set_For_Mouse;


    private string gameManagerTag = "GameManager";
    
    private string uiManagerTag = "UiManager";


    public float minMagnitude, maxMagnitude;



    void Start()
    {
        cam = Camera.main;

        gameManager = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponent<GameManager>();

        interfaceManager = GameObject.FindGameObjectWithTag(uiManagerTag).GetComponent<InterfaceManager>();
        
        ObscuredPrefs.SetBool("Permision",true);

        // StartCoroutine(PermisionToThrow(1f));

        // startPos_Set_For_Mouse = true;
    }

    // public IEnumerator PermisionToThrow(float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     ObscuredPrefs.SetBool("Permision",true);
    // }

    // Update is called once per frame
    void Update()
    {
        if (!interfaceManager.mouseOverUIElement)
        {
            if (torusPhysic.isReady)
            {
                // WithTouch();
                WithMouse();
            }
        }
        
    }

    public void WithTouch()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTime = Time.time;
                startPos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                movedPos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);

                direction = (movedPos - startPos) * multiple_Vector;
                
                direction.z = 0; // cause of rotation in camera i had to set zero on Z in swipe direction

                if (minMagnitude < direction.magnitude && direction.magnitude < maxMagnitude)
                {
                    trajectory.Active_Trajectory_Dots();
                    trajectory.SetDots(playerPos.position,-direction);
                }
                else
                {
                    if (minMagnitude >= direction.magnitude)
                    {
                        trajectory.DeActive_Trajectory_Dots();
                    }
                    else if (direction.magnitude > maxMagnitude)
                    {
                        Vector3 lastDirectionPermitted = Vector3.zero; //when the direction is out of range we will set last direction was allowed
                        Vector3 temp = direction.normalized;
                        // Debug.Log("Normalized Direction :" + temp );
                        float chord = Mathf.Pow(temp.x, 2) + Mathf.Pow(temp.y, 2);
                        lastDirectionPermitted.x = (temp.x / chord) * maxMagnitude;
                        lastDirectionPermitted.y = (temp.y / chord) * maxMagnitude;
                        direction = lastDirectionPermitted;
                        trajectory.Active_Trajectory_Dots();
                        trajectory.SetDots(playerPos.position,-direction);
                    }
                }
                
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endTime = Time.time;
                endPos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);

                direction = (endPos - startPos)*multiple_Vector;

                intervalTime = endTime - startTime;

                direction.z = 0; // cause of rotation in camera i had to set zero on Z in swipe direction
                if (minMagnitude < direction.magnitude && direction.magnitude < maxMagnitude)
                {
                    torusPhysic.AddForce_With_Vector(-direction);
                    trajectory.DeActive_Trajectory_Dots();
                    torusPhysic.isReady = false;
                    gameManager.game_Start = true;
                    // torusPhysic.ProvideNextTorus();
                }
                else
                {
                    if (direction.magnitude > maxMagnitude)
                    {
                        Vector3 lastDirectionPermitted = Vector3.zero; //when the direction is out of range we will set last direction was allowed
                        Vector3 temp = direction.normalized;
                        // Debug.Log("Normalized Direction :" + temp );
                        float chord = Mathf.Pow(temp.x, 2) + Mathf.Pow(temp.y, 2);
                        lastDirectionPermitted.x = (temp.x / chord) * maxMagnitude;
                        lastDirectionPermitted.y = (temp.y / chord) * maxMagnitude;
                        direction = lastDirectionPermitted;
                        torusPhysic.AddForce_With_Vector(-direction);
                        trajectory.DeActive_Trajectory_Dots();
                        // startPos_Set_For_Mouse = true;
                        torusPhysic.isReady = false;
                        gameManager.game_Start = true;
                        // torusPhysic.ProvideNextTorus();
                    }
                }

                // if (direction.magnitude > Vector3.one.magnitude)
                // {
                //     
                // }
            }
        }
    }

    public void WithMouse()
    {
        // && startPos_Set_For_Mouse
        // && ObscuredPrefs.GetBool("Permision")
        if (Input.GetMouseButton(0) && ObscuredPrefs.GetBool("Permision"))
        {
            startTime = Time.time;
            Debug.Log(cam.ScreenToWorldPoint (Input.mousePosition));
            startPos = cam.ScreenToWorldPoint (Input.mousePosition);
             ObscuredPrefs.SetBool("Permision",false);
            // startPos_Set_For_Mouse = false;
            
        }

        if (Input.GetMouseButton(0))
        {
//            Debug.Log("Dots Called");
            movedPos = cam.ScreenToWorldPoint (Input.mousePosition);
            
            direction = (movedPos - startPos)*multiple_Vector;

            direction.z = 0; // cause of rotation in camera i had to set zero on Z in swipe direction
            
            if (minMagnitude < direction.magnitude && direction.magnitude < maxMagnitude)
            {
                trajectory.Active_Trajectory_Dots();
                trajectory.SetDots(playerPos.position,-direction);
            }
            else
            {
                if (minMagnitude >= direction.magnitude)
                {
                    trajectory.DeActive_Trajectory_Dots();
                    
                }
                else if (direction.magnitude > maxMagnitude)
                {
                    Vector3 lastDirectionPermitted = Vector3.zero; //when the direction is out of range we will set last direction was allowed
                    Vector3 temp = direction.normalized;
                    // Debug.Log("Normalized Direction :" + temp );
                    float chord = Mathf.Pow(temp.x, 2) + Mathf.Pow(temp.y, 2);
                    lastDirectionPermitted.x = (temp.x / chord) * maxMagnitude;
                    lastDirectionPermitted.y = (temp.y / chord) * maxMagnitude;
                    direction = lastDirectionPermitted;
                    trajectory.Active_Trajectory_Dots();
                    trajectory.SetDots(playerPos.position,-direction);
                }
            }
        }
        else
        {
            ObscuredPrefs.SetBool("Permision",true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTime = Time.time;
            endPos = cam.ScreenToWorldPoint (Input.mousePosition);
            
            direction = (endPos - startPos)*multiple_Vector;
            
            direction.z = 0; // cause of rotation in camera i had to set zero on Z in swipe direction

            intervalTime = endTime - startTime;
            if (minMagnitude < direction.magnitude && direction.magnitude < maxMagnitude)
            {
                torusPhysic.AddForce_With_Vector(-direction);
                trajectory.DeActive_Trajectory_Dots();
                // startPos_Set_For_Mouse = true;
                torusPhysic.isReady = false;
                gameManager.game_Start = true;
                // torusPhysic.ProvideNextTorus();
            }
            else
            {
                 if (direction.magnitude > maxMagnitude)
                {
                    Vector3 lastDirectionPermitted = Vector3.zero; //when the direction is out of range we will set last direction was allowed
                    Vector3 temp = direction.normalized;
                    // Debug.Log("Normalized Direction :" + temp );
                    float chord = Mathf.Pow(temp.x, 2) + Mathf.Pow(temp.y, 2);
                    lastDirectionPermitted.x = (temp.x / chord) * maxMagnitude;
                    lastDirectionPermitted.y = (temp.y / chord) * maxMagnitude;
                    direction = lastDirectionPermitted;
                    torusPhysic.AddForce_With_Vector(-direction);
                    trajectory.DeActive_Trajectory_Dots();
                    // startPos_Set_For_Mouse = true;
                    torusPhysic.isReady = false;
                    gameManager.game_Start = true;
                    // torusPhysic.ProvideNextTorus();
                }
            }
            
        }
    }
}