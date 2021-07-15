using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateObjectButtons : MonoBehaviour {

	// Use this for initialization
	public GameObject selectObjectPanel;

	public GameObject selectThemePanel;

	public GameObject subSelectObjectPanel;
	
	public GameObject objectButtonPrefab;
	
	public List<string> nameObjects = new List<string>();
	
	
	public List<string> nickNameObjects = new List<string>();

	public ResourcesManager resourcesManager;

	public LevelCreator levelCreator;

	private List<GameObject> subButtons = new List<GameObject>();

	public InterfaceManager interfaceManager;

	public ThemeResource themeResource;

	
	void Start () {
		CreateButtonPanel();
		
		CreateThemeButtons(selectThemePanel.transform,themeResource.themes);
		
		// resourcesManager.levelGameObjects
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void CreateButtonPanel(List<LevelGameObjectBase> obs)
	{
		for (int i = 0; i < obs.Count; i++)
		{
			nameObjects.Add(obs[i].obj_id);
			nickNameObjects.Add(obs[i].nickName);
			
			GameObject temp = Instantiate(objectButtonPrefab, transform.position, Quaternion.identity);
			
			Button tempButton =   temp.GetComponent<Button>();

			string tempString = obs[i].obj_id;

			UnityAction action1;
			if (obs[i].subObjects.Length <= 1)
			{
				action1  = () => { levelCreator.PassGameObjectToPlaceFromButton(tempString , 0); };
			 
			}
			else
			{
				var i1 = i;
				action1 = () => { SetActionForSubButtons(temp,obs[i1]); };

				UnityAction action2 = OpenSubObjectPanel;
				
				tempButton.onClick.AddListener(action2);
			}

			tempButton.onClick.AddListener(action1);

			tempButton.image.sprite = obs[i].image;

//			temp.GetComponentInChildren<Text>().text = resourcesManager.levelGameObjects[i].nickName;
			 
			temp.transform.SetParent(selectObjectPanel.transform);

			temp.transform.localScale = Vector3.one;
			
			temp.transform.position = Vector3.zero;
			
		}
	}

	public void CreateThemeButtons(Transform parent , List<Theme> themes)
	{
		foreach (var theme in themes)
		{
			GameObject temp = Instantiate(objectButtonPrefab, transform.position, Quaternion.identity);
			
			Button tempButton =   temp.GetComponent<Button>();

			UnityAction action = () => { themeResource.AssignTheme(theme.id); };
			
			tempButton.onClick.AddListener(action);
			
			temp.transform.SetParent(parent);
			
			temp.transform.localScale = Vector3.one;
			
			temp.transform.localPosition = Vector3.zero;

			temp.transform.rotation = parent.transform.rotation;

		}
	}
	public void CreateButtonPanel()
	{
		for (int i = 0; i < resourcesManager.levelGameObjects.Count; i++)
		{
			nameObjects.Add(resourcesManager.levelGameObjects[i].obj_id);
			nickNameObjects.Add(resourcesManager.levelGameObjects[i].nickName);
			
			GameObject temp = Instantiate(objectButtonPrefab, transform.position, Quaternion.identity);
			
			Button tempButton =   temp.GetComponent<Button>();

			string tempString = resourcesManager.levelGameObjects[i].obj_id;

			UnityAction action1;
			UnityAction deActive_SubObjectPanel_Action;
			// deactivePanelAction;
			if (resourcesManager.levelGameObjects[i].subObjects.Length <= 1)
			{
				action1  = () => { levelCreator.PassGameObjectToPlaceFromButton(tempString); };

				deActive_SubObjectPanel_Action = () => { DeactiveSubSelectObjectPanel_MouseEnter(subSelectObjectPanel); };
				
				tempButton.onClick.AddListener(deActive_SubObjectPanel_Action);
			}
			else
			{
				var i1 = i;
				action1 = () => { SetActionForSubButtons(temp,i1); };

				UnityAction action2 = OpenSubObjectPanel;
				
				tempButton.onClick.AddListener(action2);
			}
			
			// deactivePanelAction = () =>
			// {
			// 	DeactiveSubSelectObjectPanel(selectObjectPanel);
			// };

			tempButton.onClick.AddListener(action1);
			
			// tempButton.onClick.AddListener(deactivePanelAction);

			tempButton.image.sprite = resourcesManager.levelGameObjects[i].image;

//			temp.GetComponentInChildren<Text>().text = resourcesManager.levelGameObjects[i].nickName;
			 
			temp.transform.SetParent(selectObjectPanel.transform);

			temp.transform.localScale = Vector3.one;
			
			temp.transform.localPosition = Vector3.zero;

			temp.transform.rotation = selectObjectPanel.transform.rotation;

		}
	}

	public void SetActionForSubButtons(GameObject parentButton, LevelGameObjectBase levelGameObjectBase)
	{
		string tempString = levelGameObjectBase.obj_id;
		
		for (int i = 0; i < levelGameObjectBase.subObjects.Length; i++)
		{
			GameObject temp = Instantiate(objectButtonPrefab, parentButton.transform.position, Quaternion.identity);
			
			Button tempButton =   temp.GetComponent<Button>();

			var i1 = i;

			void Action1()
			{
				levelCreator.PassGameObjectToPlaceFromButton(tempString, i1);
			}

			UnityAction action2 = () =>
			{
				DeactiveSubSelectObjectPanel_MouseExit(subSelectObjectPanel);
				DeactiveSubSelectObjectPanel_MouseExit(selectObjectPanel);
			};
			
			tempButton.onClick.AddListener(Action1);
			tempButton.onClick.AddListener(action2);

			tempButton.image.sprite = levelGameObjectBase.subObjects[i].image;
			
			temp.transform.SetParent(subSelectObjectPanel.transform);
			
			temp.transform.localScale = Vector3.one;
			

		}
		
	}
	
	public void SetActionForSubButtons(GameObject parentButton, int number)
	{
		LevelGameObjectBase levelGameObjectBase = resourcesManager.levelGameObjects[number];
		
		string tempString = levelGameObjectBase.obj_id;

		foreach (var variable in subButtons)
		{
			Destroy(variable);
		}
		
		subButtons.Clear();
		
		for (int i = 0; i < levelGameObjectBase.subObjects.Length; i++)
		{
			GameObject temp = Instantiate(objectButtonPrefab, parentButton.transform.position, Quaternion.identity);
			
			subButtons.Add(temp);
			
			Button tempButton =   temp.GetComponent<Button>();

			var i1 = i;

			void Action1()
			{
				levelCreator.PassGameObjectToPlaceFromButton(tempString, i1);
			}

			UnityAction action2 = () =>
			{
				DeactiveSubSelectObjectPanel_MouseExit(subSelectObjectPanel);
				DeactiveSubSelectObjectPanel_MouseExit(selectObjectPanel);
			};
			
			tempButton.onClick.AddListener(Action1);
			tempButton.onClick.AddListener(action2);

			tempButton.image.sprite = levelGameObjectBase.subObjects[i].image;
			
			temp.transform.SetParent(subSelectObjectPanel.transform);
			
			temp.transform.localScale = Vector3.one;
			

		}
		
	}
	public void DeactiveSubSelectObjectPanel_MouseEnter(GameObject panel)
	{
		panel.SetActive(false);
		interfaceManager.MouseEnter();
	}
	public void DeactiveSubSelectObjectPanel_MouseExit(GameObject panel)
	{
		panel.SetActive(false);
		interfaceManager.MouseExit();
	}

	public void OpenSubObjectPanel()
	{
		subSelectObjectPanel.SetActive(true);
	}


	public void ReverseSetActivePanelSituation(GameObject go)
	{
		go.SetActive(!go.activeInHierarchy);
	}


}
