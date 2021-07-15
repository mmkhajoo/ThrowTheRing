using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class CreateLevelObject : MonoBehaviour
{
    // Start is called before the first frame update

    public string objectName;

    public SaveLevelObject saveLevelObject;

    public LevelObject levelObject;

    public Moveable move;


    void Start()
    {
        
        move = new Moveable(new Vector2(10, 20), new Vector2(15, 30), 30f);

        saveLevelObject = ScriptableObject.CreateInstance<SaveLevelObject>();

        saveLevelObject.levelObject = new LevelObject();

        saveLevelObject.levelObject.move = move;
        
        Save("/Editor","Test", saveLevelObject);


//        levelObject = new LevelObject();

//        var instance = ScriptableObject.CreateInstance<LevelObject>();
//
//        instance.objectName = objectName;
//
//        if (!Directory.Exists(Application.streamingAssetsPath + "/Test"))
//        {
//            Directory.CreateDirectory(Application.streamingAssetsPath + "/Test");
//        }
//
//        AssetDatabase.CreateFolder("Assets", "Editor");
//        AssetDatabase.CreateAsset(saveLevelObject ,"Assets/Editor/MyData1.asset");
//        AssetDatabase.SaveAssets(); 
//        AssetDatabase.Refresh();
//
//         saveLevelObject = AssetDatabase.LoadAssetAtPath<SaveLevelObject>("Assets/Editor/MyData1.asset");
    }

    public void Save(string path, string fileName, SaveLevelObject saveLevelObject) // Save Asset With Path And File Name And Type OF the Class
    {
        if (!Directory.Exists(Application.dataPath + path))
        {
            Directory.CreateDirectory(Application.dataPath + path);
        }

        if (!File.Exists(Application.dataPath + path + "/" + fileName + ".asset"))
        {
            AssetDatabase.CreateAsset(saveLevelObject,"Assets"+ path + "/" + fileName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            // Tell Player Do You Want Replace Or Not
            Debug.Log("File Exist");
        }
        
    }

//    public void Save()
//    {
//        if (!Directory.Exists(Application.persistentDataPath + "/Level")); //Application.streamingAssetsPath for assets Folder But not Work For Build
//        {
//            Directory.CreateDirectory(Application.persistentDataPath + "/Level");
//        }
//        BinaryFormatter bf = new BinaryFormatter();
//
//        FileStream file = File.Create(Application.persistentDataPath + "/Level/Level01.txt");
//        
//        var json = JsonUtility.ToJson(//Scriptble Object);
//            bf.Serialize(file, json);
//        file.Close();
//
//    }
//
//    public void Load()
//    {
//        if (!Directory.Exists(Application.persistentDataPath + "/Level")); //Application.streamingAssetsPath for assets Folder
//        {
//            Directory.CreateDirectory(Application.persistentDataPath + "/Level");
//        }
//        BinaryFormatter bf = new BinaryFormatter();
//        if (Directory.Exists(Application.persistentDataPath + "/Level/Level01.txt"))
//        {
//            FileStream file = File.Open(Application.persistentDataPath + "/Level/Level01.txt", FileMode.Open);
//            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file),//Scriptble Object);
//
//        }
//    }
}