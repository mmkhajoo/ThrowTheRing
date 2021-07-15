using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThemeResource : MonoBehaviour
{

    public ResourcesManager resourcesManager;

    public List<Theme> themes;

    private List<SpriteRenderer> backGroundsSprite;

    public static ThemeResource instance;
    
    

    // Set If The Materials Didnt Change Do Not The Loops

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

    private void Start()
    {
    }
    

    public void AssignTheme(string themeId)
    {
        AssignBackGroundsSprites();
        
        Theme temp = null;
        foreach (var theme in themes)
        {
            if (theme.id.Equals(themeId))
            {
                temp = theme;
                break;
            }
        }
        
        
        if (temp != null)
        {
            AssignImageToBackGround(temp.image);
            
            if (!PlayerPrefs.GetString("ThemeId").Equals(temp.id))
            {
                PlayerPrefs.SetString("ThemeId" , temp.id);
            
                foreach (var objectMaterial in temp.materials)
                {
                    foreach (var objects in resourcesManager.levelGameObjects)
                    {
                        if (objects.obj_id.Equals(objectMaterial.id))
                        {
                            objects.objPrefab.GetComponent<MeshRenderer>().material = objectMaterial.material;
                            foreach (var subObjects in objects.subObjects)
                            {
                                subObjects.objPrefab.GetComponent<MeshRenderer>().material = objectMaterial.material;
                            }
                        }
                    }
                }
            }
           
        }
        
        
    }

    public void AssignBackGroundsSprites()
    {
        if (backGroundsSprite == null)
        {
            backGroundsSprite = new List<SpriteRenderer>();
        }
        if (backGroundsSprite.Count <= 0)
        {
            GameObject[] backGrounds = GameObject.FindGameObjectsWithTag("BackGround");
            foreach (var background in backGrounds)
            {
                backGroundsSprite.Add(background.GetComponent<SpriteRenderer>());
            }
        }
        
    }

    public void AssignImageToBackGround(Sprite image)
    {
        foreach (var sprite in backGroundsSprite)
        {
            sprite.sprite = image;
        }
    }
    
    
}
[Serializable]
public class Theme
{
    public string id;

    public Sprite image;

    public List<ObjectMaterial> materials;

}

[Serializable]
public class ObjectMaterial
{
    public string id;

    public Material material;
}
