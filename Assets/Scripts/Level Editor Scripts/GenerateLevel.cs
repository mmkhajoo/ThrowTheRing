using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public class GenerateLevel : MonoBehaviour
{
    // Start is called before the first frame update
    

    public Levels levels;

    public ResourcesManager resourcesManager;

    public GameManager gameManager;

    private SaveableLevel_SaveLoad level;

    public int levelNumber;

    private readonly FeatureSystem featureSystem = new FeatureSystem();
    
    [SerializeField] private List<Torus> Toruses;
    
    [SerializeField] private List<RodSystem> rods;
    

    void Awake()
    {
        Toruses = new List<Torus>();
        // ObscuredPrefs.SetInt("LevelNumber",12);
        // level = levels.levels[levelNumber];
        level = levels.levels[ObscuredPrefs.GetInt("LevelNumber")];
        ObscuredPrefs.SetInt("RodCounter", 0);
        ObscuredPrefs.SetInt("RodCount", 0);
        PlayerPrefs.SetString("ThemeId", level.themeId);
        ThemeResource.instance.AssignTheme(PlayerPrefs.GetString("ThemeId"));
        CreateObjects();
        foreach (var torus in Toruses)
        {
            torus.torusPhysic.enabled = true;
        }

        if (Toruses.Count > 0)
        {
            StartCoroutine(SetKinematicObjects(0.2f));
        }
    }

    public IEnumerator SetKinematicObjects(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (var torus in Toruses)
        {
            torus.torusPhysic.rigidBody.isKinematic = true;
        }
        Toruses[0].swipe.enabled = true;
    }

    public void CreateObjects()
    {
        GameObject tempObj;

        foreach (var levelObject in level.saveableLevelObjects_List)
        {
            if (levelObject.isSubObject)
            {
                tempObj = Instantiate(
                    resourcesManager.GetObjBase(levelObject.obj_id).subObjects[levelObject.subNumber].objPrefab,
                    new Vector3(levelObject.posX, levelObject.posY, levelObject.posZ),
                    Quaternion.Euler(levelObject.rotX, levelObject.rotY, levelObject.rotZ));
            }
            else
            {
                tempObj = Instantiate(resourcesManager.GetObjBase(levelObject.obj_id).objPrefab,
                    new Vector3(levelObject.posX, levelObject.posY, levelObject.posZ),
                    Quaternion.Euler(levelObject.rotX, levelObject.rotY, levelObject.rotZ));
            }

            tempObj.transform.localScale = new Vector3(levelObject.scaleX, levelObject.scaleY, levelObject.scaleZ);

            DetectSpecificObject(tempObj);
            featureSystem.AddFeatures(tempObj, levelObject);
//                 ActionsForGameObject(tempObj);
//
//                 CheckSpecialThingsForSomeGameObjects(levelObject.obj_id);
        }
    }

    public void DetectSpecificObject(GameObject go)
    {
        if (go.tag.Equals("Player"))
        {
            Torus torus = new Torus(go.GetComponent<TorusPhysic>(), go.GetComponent<Swipe>());
            Toruses.Add(torus);
        }
        else if (go.tag.Equals("PlayerPack"))
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Torus torus = new Torus(go.transform.GetChild(i).GetComponent<TorusPhysic>(),
                    go.transform.GetChild(i).GetComponent<Swipe>());
                Toruses.Add(torus);
            }
        }
        else if (go.tag.Equals("Rod"))
        {
            ObscuredPrefs.SetInt("RodCount", ObscuredPrefs.GetInt("RodCount") + 1);
            rods.Add(go.GetComponent<RodSystem>());
        }
    }

    public void NextTorus()
    {
        Toruses.RemoveAt(0);
        if (Toruses.Count > 0)
        {
            ObscuredPrefs.SetBool("Permision",true);
            Toruses[0].swipe.enabled = true;
        }
        else
        {
            gameManager.out_of_torus = true;
        }
    }
    public void StopCoroutine_Torus()
    {
        if (Toruses.Count > 0)
        {
            Debug.Log("Stoped");
            // Toruses[0].torusPhysic.StopCoroutine("DeleteObject");
            ObscuredPrefs.SetBool("Permision",true);
            Toruses[0].torusPhysic.Stop();
        }
    }

    public void TorusReadyAgain()
    {
        Debug.Log("Torus Ready Again");
        Toruses[0].torusPhysic.Ready();
        StopCoroutine_Torus();
    }

    public void ResetRods()
    {
        foreach (var rod in rods)
        {
            rod.Reset();
        }
    }

    public Vector3 GetRodPosition(int index)
    {
        return rods[index].transform.position;
    }
}

[Serializable]
public class Torus
{
    public TorusPhysic torusPhysic;
    public Swipe swipe;

    public Torus(TorusPhysic torusPhysic, Swipe swipe)
    {
        this.torusPhysic = torusPhysic;
        this.swipe = swipe;
    }
}