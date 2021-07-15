using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class Lose_Detector : MonoBehaviour
{
    // Start is called before the first frame update

    public GameManager gameManager;

    public GenerateLevel generateLevel;

    public CameraController cameraController;

    // public Rigidbody player_Rb;

    public float leastMagnitude;

    void Start()
    {
        // player_Rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
//     void Update()
//     {
//         if (gameManager.out_of_torus && gameManager.game_Start)
//         {
//             gameManager.Lose();
//         }
// //        Debug.Log(player_Rb.velocity.magnitude);
//     }

    private void OnTriggerExit(Collider other)
    {
        if (!cameraController.isCameraMove)
        {
            Debug.LogError("Player Exit   " + other.tag);
            if (other.tag.Equals("Player"))
            {
                GameObject go = other.gameObject;
                go.SetActive(false);
                StartCoroutine(DeleteObject(go, 1f));
                gameManager.Lose();

            }
        }
       
    }

    private IEnumerator DeleteObject(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        while (go.transform.parent != null && go.transform.parent.parent != null)
        {
            go = go.transform.parent.gameObject;
        }

        Destroy(go);
    }
}