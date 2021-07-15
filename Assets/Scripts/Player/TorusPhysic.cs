using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class TorusPhysic : MonoBehaviour
{
    // Start is called before the first frame update
//    public HoldDownAndRelease forceButton;

    public Rigidbody rigidBody;

    public Vector3 direction;

    public Vector3 force;

    public Vector3 rotation;

    public Vector3 torque;

    public float multiple_Force;

    public float multiple_Torque_X, multiple_Torque_Y, multiple_Torque_Z, multiple_Torque_Z_Rotate;

    public bool isReady;

    [SerializeField] private bool IsRotate;

    [SerializeField] private GenerateLevel generateLevel;
    
    [SerializeField] private GameManager gameManager;


    public GameObject playerCollider;


    private string generateLevelTag = "GenerateLevel";

    private string gameManagerTag = "GameManager";


    private IEnumerator deleteObject;

    private Vector3 start;

    void Start()
    {
        generateLevel = GameObject.FindGameObjectWithTag(generateLevelTag).GetComponent<GenerateLevel>();
        gameManager = GameObject.FindGameObjectWithTag(gameManagerTag).GetComponent<GameManager>();
        rigidBody.isKinematic = false;
        isReady = true;
        start = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//        if (forceButton.isPressed)
//        {
//            AddForce_With_Vector_Button();
//        }
    }

    public void AddForce(Vector3 force)
    {
//        Invoke("Rotate1",0.1f);
        rigidBody.velocity = force;
    }

    public void Rotate(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }
//    public void Rotate()
//    {
//        rigidBody.AddTorque(rotation);
//    }

    public void AddTorque(Vector3 torque)
    {
        rigidBody.AddRelativeTorque(torque, ForceMode.Force);
    }


    public void AddForce_To_Position()
    {
        rigidBody.AddForceAtPosition(force, transform.position);
    }

    public void AddForce_With_Vector(Vector3 direction)
    {
        Vector3 force = multiple_Force * direction;

        Vector3 torque;

        if (IsRotate)
        {
            if (direction.x > 0)
            {
                torque = new Vector3(0,
                    multiple_Torque_Y * Mathf.Sqrt(Mathf.Pow(direction.x, 2f) + Mathf.Pow(direction.y, 2f)),
                    multiple_Torque_Z_Rotate);
            }
            else
            {
                torque = new Vector3(0,
                    multiple_Torque_Y * Mathf.Sqrt(Mathf.Pow(direction.x, 2f) + Mathf.Pow(direction.y, 2f)),
                    -multiple_Torque_Z_Rotate);
            }
        }
        else
        {
            torque = new Vector3(0,
                multiple_Torque_Y * Mathf.Sqrt(Mathf.Pow(direction.x, 2f) + Mathf.Pow(direction.y, 2f)),
                multiple_Torque_Z / Mathf.Sqrt(Mathf.Pow(direction.x, 2f) + Mathf.Pow(direction.y, 2f)));
        }

        // Vector3 rotation = direction.normalized;

//        Debug.Log(rotation);

        // rotation = Mathf.Atan2(rotation.y, rotation.x) * Vector3.forward * Mathf.Rad2Deg;

        rigidBody.isKinematic = false;
        AddForce(force);
//        Rotate(rotation);
        AddTorque(torque);
        deleteObject = DeleteObject();
        // deleteObject = StartCoroutine(DeleteObject());
        StartCoroutine(deleteObject);
    }

    public void AddForce_With_Vector_Button()
    {
//        AddForce_With_Vector(direction);
        rigidBody.isKinematic = false;
        AddForce(force);
        AddTorque(torque);
        Rotate(rotation);
    }

    public void Reset()
    {
        StartCoroutine(ReadyForShot());
        transform.position = start;
        rigidBody.isKinematic = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void Ready()
    {
        isReady = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void ProvideNextTorus()
    {
        generateLevel.NextTorus();
    }

    public IEnumerator ReadyForShot()
    {
        yield return new WaitForSeconds(0.2f);
        isReady = true;
    }

    public void Stop()
    {
        if (deleteObject != null)
        {
            StopCoroutine(deleteObject);
        }
        // StopCoroutine(nameof(DeleteObject));
    }

    public IEnumerator DeleteObject()
    {
        yield return new WaitForSeconds(6f);
        Debug.Log("DeleteCoroutine Run");
        if (playerCollider.activeInHierarchy)
        {
            ObscuredPrefs.SetInt("RodCounter", 0);
            generateLevel.NextTorus();
            generateLevel.ResetRods();
            gameManager.cameraController.BackToStartPosition();
            Destroy(gameObject);
            if (gameManager.game_Start && gameManager.out_of_torus)
            {
                gameManager.Lose();
            }
        }
    }
}