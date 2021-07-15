using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class Win_Collider : MonoBehaviour
{
    public GameManager gameManager;

    // public MeshFilter bottomRodMesh;

    public GameObject rod;

    public GameObject bottomRod;

    public GameObject ground;
    // public Mesh replaceMesh; //The Mesh We Want To Replace With 

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag.Equals("Win"))
        {
            ObscuredPrefs.SetInt("RodCounter", ObscuredPrefs.GetInt("RodCounter") + 1);
            StartCoroutine(ChangeObject());
            // bottomRodMesh.mesh = replaceMesh;
            // bottomRodMesh.mesh.RecalculateNormals();
        }
    }

    private IEnumerator ChangeObject()
    {
        yield return new WaitForSeconds(0.7f);
        if (ObscuredPrefs.GetInt("RodCounter") < ObscuredPrefs.GetInt("RodCount"))
        {
            rod.SetActive(false);
            bottomRod.SetActive(false);
            ground.SetActive(true);
        }
        gameManager.Win();
    }
}