using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour
{
    public List<LevelGameObjectBase> levelGameObjects;

    public static ResourcesManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        
    }

    public LevelGameObjectBase GetObjBase(string objId)
    {
        foreach (var levelObj in levelGameObjects)
            if (objId.Equals(levelObj.obj_id))
            { 
                return levelObj;
            }

        return null;
    }
}

[Serializable]
public class LevelGameObjectBase
{
    public string obj_id;
    public string nickName;
    public Sprite image;
    public GameObject objPrefab;
    public LevelGameObject[] subObjects;
}
[Serializable]
public class LevelGameObject
{
    public string name;

    public GameObject objPrefab;

    public Sprite image;
}