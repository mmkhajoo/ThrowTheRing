using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class DeadlyObject : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager gameManager;

    private bool castOnce;
    void Start()
    {
        castOnce = true;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.LogError(other.collider.tag);
        if(!castOnce)
            return;
        if (other.collider.tag.Equals("Player") || other.collider.tag.Equals("LevelObject"))
        {
            castOnce = false;
            other.gameObject.SetActive(false);
            gameManager.Lose();
            StartCoroutine(DeleteObject(other.gameObject, 0.2f));
        }
    }

    private void DeleteObject(GameObject go)
    {
        while (go.transform.parent != null && go.transform.parent.parent != null)
        {
            go = go.transform.parent.gameObject;
        }
        Destroy(go);
        castOnce = true;
    }
    private IEnumerator DeleteObject(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        while (go.transform.parent != null && go.transform.parent.parent != null)
        {
            go = go.transform.parent.gameObject;
        }

        Destroy(go);
        castOnce = true;
    }
}
