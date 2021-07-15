using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public GridBase gridBase;
    public InterfaceManager ui;
    public ResourcesManager resourcesManager;
    public LevelManager levelManager;


    //place obj variables
    private bool hasObject;
    private GameObject objectToPlace = null;
    private GameObject cloneObject = null;
    public GameObject actualObject;
    public Level_GameObject objProperties;
    private Vector3 touchPosition;
    private Vector3 worldPosition;
    private bool deleteObject;

    public LayerMask objects_layer;

    public LayerMask field_layer;

    private string levelObjectTag = "LevelObject";

    private string playerTag = "Player";


    // Use this for initialization
    void Start()
    {
        cloneObject = null;
        objectToPlace = null;
    }

    // Update is called once per frame
    void Update()
    {
        PlaceObject();
        if(Input.GetMouseButtonDown(1))
        {
            DeleteGameObjectButton();
            DeleteObjects();
        }
    }

    private GameObject CheckRayCollideToLevelObject(LayerMask layer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,layer))
        {
            // Debug.Log(hit.collider.tag);
            if (hit.collider.tag.Equals(levelObjectTag) || hit.collider.tag.Equals(playerTag))
            {
                if (hit.collider.transform.parent != null)
                {
                    return hit.collider.transform.parent.gameObject;
                }
                
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    private void UpdateTouchPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity,field_layer))
        {
            touchPosition = hit.point;
        }
    }

    private void PlaceObject()
    {
        if (hasObject)
        {
            UpdateTouchPosition();

            worldPosition = gridBase.ObjectPosFromWorldPosition(touchPosition);

            worldPosition.x = Mathf.Round(worldPosition.x*2f)/2f;
            worldPosition.y = Mathf.Round(worldPosition.y*2f)/2f;
            worldPosition.z = Mathf.Round(worldPosition.z*2f)/2f;
            // worldPosition.x = Mathf.Round(worldPosition.x);
            // worldPosition.y = Mathf.Round(worldPosition.y);
            // worldPosition.z = Mathf.Round(worldPosition.z);
            
            if (!ui.mouseOverUIElement)
            {
                if (cloneObject == null)
                {
                    cloneObject = Instantiate(objectToPlace, worldPosition, objectToPlace.transform.rotation);
                    objProperties = cloneObject.GetComponent<Level_GameObject>();
                }
                else
                {
                    cloneObject.transform.position = worldPosition;
                    // Save Position Of Last GameObject was in Node in Level Manager List Objects
                    //touch Position
//				Input.GetTouch(0).phase == TouchPhase.Ended)
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject actualObjectPlaced =
                            Instantiate(objectToPlace, worldPosition, cloneObject.transform.rotation);
                        actualObject = actualObjectPlaced;
                        objProperties = actualObjectPlaced.GetComponent<Level_GameObject>();
                        
                        levelManager.objects.Add(actualObjectPlaced);
                        levelManager.objectsProperties.Add(objProperties);
                        
                        CloseAll();
                        hasObject = true;
                    }
                }
            }

            else
            {
                if (cloneObject != null)
                {
                    Destroy(cloneObject);
                    cloneObject = null;
                }
            }
        }
        else
        {
            if (cloneObject != null)
            {
                Destroy(cloneObject);
                cloneObject = null;
            }
        }
    }

    public void ChangeRotationButton()
    {
        objProperties.ChangeRotation();
    }

    private void DeleteObjects()
    {
        //if (deleteObject)
        //{
            UpdateTouchPosition();

            GameObject objectMustDelete = CheckRayCollideToLevelObject(objects_layer);

            //-----position touch-------//
//			Input.GetTouch(0).phase == TouchPhase.Began)
            if (Input.GetMouseButtonDown(1) && !ui.mouseOverUIElement)
            {
                if (objectMustDelete != null)
                {
                    objectMustDelete = FindHeadParent(objectMustDelete);
                    for (int i = 0; i < levelManager.objects.Count; i++)
                    {
                        if (levelManager.objects[i] == objectMustDelete)
                        {
                            levelManager.objects.Remove(levelManager.objects[i]);
                            levelManager.objectsProperties.Remove(levelManager.objectsProperties[i]);
                        }
                    }
                    
                    Destroy(objectMustDelete);
                }
            }
        //}
    }

    public GameObject FindHeadParent(GameObject go)
    {
        while (go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
        }

        return go;
    }

    public void PassGameObjectToPlaceFromButton(string objID , int number)
    {
        if (cloneObject != null)
        {
            Destroy(cloneObject);
        }

        CloseAll();
        hasObject = true;
        cloneObject = null;
        objectToPlace = resourcesManager.GetObjBase(objID).subObjects[number].objPrefab;
    }
    
    public void PassGameObjectToPlaceFromButton(string objID)
    {
        if (cloneObject != null)
        {
            Destroy(cloneObject);
        }

        CloseAll();
        hasObject = true;
        cloneObject = null;
        objectToPlace = resourcesManager.GetObjBase(objID).objPrefab;
    }

    public void DeleteGameObjectButton()
    {
        CloseAll();
        deleteObject = true;
    }


    private void CloseAll()
    {
        hasObject = false;
        deleteObject = false;
    }

    public void MoveObject(GameObject temp)
    {
        switch (temp.name)
        {
            case "Up":
            {
                Debug.Log("Up Called");
                actualObject.transform.position = new Vector3(worldPosition.x
                    , actualObject.transform.position.y, worldPosition.z + gridBase.offset / 2);
                break;
            }

            case "Down":
            {
                actualObject.transform.position = new Vector3(worldPosition.x
                    , actualObject.transform.position.y, worldPosition.z - gridBase.offset / 2);
                break;
            }

            case "Left":
            {
                actualObject.transform.position = new Vector3(worldPosition.x - gridBase.offset / 2
                    , actualObject.transform.position.y, worldPosition.z);
                break;
            }

            case "Right":
            {
                actualObject.transform.position = new Vector3(worldPosition.x + gridBase.offset / 2
                    , actualObject.transform.position.y, worldPosition.z);
                break;
            }

            case "DownRight":
            {
                actualObject.transform.position = new Vector3(worldPosition.x + gridBase.offset / 2
                    , actualObject.transform.position.y, worldPosition.z - gridBase.offset / 2);
                break;
            }

            case "DownLeft":
            {
                actualObject.transform.position = new Vector3(worldPosition.x - gridBase.offset / 2
                    , actualObject.transform.position.y, worldPosition.z - gridBase.offset / 2);
                break;
            }

            case "UpRight":
            {
                actualObject.transform.position = new Vector3(worldPosition.x + gridBase.offset / 2
                    , actualObject.transform.position.y, worldPosition.z + gridBase.offset / 2);
                break;
            }

            case "UpLeft":
            {
                actualObject.transform.position = new Vector3(worldPosition.x - gridBase.offset / 2
                    , actualObject.transform.position.y, worldPosition.z + gridBase.offset / 2);
                break;
            }

            case "Mid":
            {
                actualObject.transform.position = new Vector3(worldPosition.x
                    , actualObject.transform.position.y, worldPosition.z);
                break;
            }
        }
    }
}