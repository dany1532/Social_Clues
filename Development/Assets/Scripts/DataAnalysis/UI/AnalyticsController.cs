using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnalyticsController : Singleton<AnalyticsController> {
	public UITable npcContainer;
	public NPCDataCamera npcCamera;
	public UIScrollBar scrollBar;
	
	// static variables
	public int levelPlayID = -1;
	public int numberOfNPCs;
	
	//public static List<List<DBSelectAll>> test = new List<List<DBSelectAll>>();
	public List<int> interactionTime = new List<int>();
	
	//public static List<float> todayPercentages    = new List<float>();
	public List<List<float>> todayPercentages    = new List<List<float>>();
	//public static List<float> lastPlayPercentages = new List<float>();
	public List<List<float>> lastPlayPercentages = new List<List<float>>();
	public List<List<float>> averagePercentages = new List<List<float>>();
	
	public List<String> categories = new List<String>();
	public List<AudioClip> categoriesAudio = new List<AudioClip>();
	
	public List<DBNPCInteraction> npc_interactions = new List<DBNPCInteraction>();
	public List<DBSelectAll> interactionInfo = new List<DBSelectAll>();
	//public static List<System.DateTime> interactionTime = new List<System.DateTime>();
	
	//public List<DBNPCInteractionTime> getNPCTotalInteractionTime(int levelPlayID) {
	public UILabel totalInteractionTimeValue;
	public Slider totalScore;
	
	public List<int> communicationMissing;	// for tracking missing communication for interactions to calculate averages
	public List<List<bool>> isCommunicationAvailable;
	
	// for the graph
	private int numberOfVertices = 5;
	public List<float> aggregateTodayPercentages;
	public List<float> aggregateLastPlayPercentages;
	public List<float> aggregateTotalPercentages;	
	
	public string file;
	public bool sendEmail = false;
	
	private float verticalOffset;
	private bool verticalOffsetReady = false;
	float cameraToPanelRatio = 0;
	bool readyToSetCameraToPanelRatio = true;
	float originalCameraHeight = 0;
	float originalPanelHeight = 0;
	float cameraPanelDistance = -1;
	
	public GameObject childrenContainer;
	public GameObject parentsContainer;
	
	public List<GameObject> parentCameras;
	public List<GameObject> childrenCameras;

	public GameObject horizontalScroll;

	public enum Tab {
		Parent,
		Child,
	}
	
	public Tab currentTab;
	
	public GameObject npcContainerChild;
	public GameObject npcChild;
	
	public GameObject npcInfo;
	public GameObject npcInfoContainer;
	public List<GameObject> npcDetails = new List<GameObject>();
	
	public AudioClip backgroundMusic;
	
	public Tab initialTab = Tab.Child;

	public List<int> expansions = new List<int>();
	public int maxExpansions = 0;
	public GameObject background;

	void Awake () {
		background = GameObject.Find ("Parents Background");
		
		currentTab = Tab.Parent;
		//currentTab = Tab.Child;
		
		instance = this;

		int userID = ApplicationState.Instance.userID;
		
		if (ApplicationState.Instance.userID == -1)
			userID = 1;

		if (levelPlayID == -1)
			levelPlayID = MainDatabase.Instance.getLevelPlayID(userID);
		
		// populate total time label
		int totalInteractionTime = MainDatabase.Instance.getPlayTotalTime(levelPlayID);
		totalInteractionTimeValue.text = TimeSpan.FromSeconds(totalInteractionTime).ToString();
		
		// get the NPC interactions
		npc_interactions = MainDatabase.Instance.getNPCandInteraction(levelPlayID);
		numberOfNPCs = npc_interactions.Count;
		if (numberOfNPCs > 3) 
			npcCamera.scale = Vector2.one;
				
		// Track which categories are missing
		communicationMissing = new List<int>();
		for(int i = 0; i < 5; ++i) {
			communicationMissing.Add(0);
		}
		
		// create a list of a list of booleans to keep track of which point of communication is missing for which NPCs, [indexOfNPC][indexOfCommunication]
		isCommunicationAvailable = new List<List<bool>>();
		for(int i = 0; i < numberOfNPCs; ++i) {
			List<bool> tempList = new List<bool>();
			for(int j = 0; j < 5; ++j) {
				tempList.Add(true);
			}
			isCommunicationAvailable.Add(tempList);
			
		}
		
		List<DBReplyTimeWithType> timeTaken = new List<DBReplyTimeWithType>();
		for(int i = 1; i <= 5; ++i) {
			for(int j = 0; j < numberOfNPCs; ++j) {
				timeTaken = MainDatabase.Instance.getTimeTaken(i, npc_interactions[j].InteractionID);
				if(timeTaken.Count == 0) {
					++communicationMissing[i - 1];
					isCommunicationAvailable[j][i - 1] = false;
				} else {
					isCommunicationAvailable[j][i - 1] = true;
				}
			}
		}
		
		Vector3 lastChildPosition = Vector3.zero; 
		
		float initialXOffset = 0.022f;
		
		// Prepare data
		for(int i = 0; i < numberOfNPCs; ++i) {
			GameObject newNPCInfo = (GameObject)Instantiate(npcInfo);
			newNPCInfo.transform.parent = npcInfoContainer.transform;
			newNPCInfo.transform.localScale = Vector3.one;
			// position the game object
			newNPCInfo.GetComponentInChildren<UIAnchor>().uiCamera = parentCameras[2].GetComponent<Camera>();
			newNPCInfo.GetComponentInChildren<UIAnchor>().relativeOffset.x = initialXOffset + (i * 0.316f);
			newNPCInfo.GetComponent<GeneralNPC>().indexOfNPC = i;
			newNPCInfo.transform.Find ("Anchor/NPC Details").gameObject.GetComponent<NPCDetailsController>().indexOfNPC = i;
			npcDetails.Add (newNPCInfo.transform.Find ("Anchor/NPC Details").gameObject);
			newNPCInfo.GetComponentInChildren<MiniBar>().indexOfNPC = i;
			
			interactionTime.Add(MainDatabase.Instance.SelectInteractionTime(npc_interactions[i].InteractionID));
			todayPercentages.Add(MainDatabase.Instance.calPercentageForInteractionID(npc_interactions[i].InteractionID,userID));
			lastPlayPercentages.Add(MainDatabase.Instance.calPercentageForPreviousInteractionID(npc_interactions[i].InteractionID, npc_interactions[i].NPCID,userID));
			averagePercentages.Add(MainDatabase.Instance.calTotalPercentage(npc_interactions[i].NPCID,userID));
			categories.Add(MainDatabase.Instance.getCategory(npc_interactions[i].NPCID));
			
			// child analytics
			GameObject newNPCChild = (GameObject)Instantiate (npcChild);
			newNPCChild.transform.parent = npcContainerChild.transform;
			newNPCChild.transform.localScale = Vector3.one;
			
			if(i != 0) {
				lastChildPosition = new Vector3(lastChildPosition.x + 200, lastChildPosition.y, lastChildPosition.z);
			}
			newNPCChild.transform.localPosition = lastChildPosition;
			newNPCChild.GetComponent<NPCChild>().indexOfNPC = i;
		}
		
		// prepare graph data
		aggregateTodayPercentages    = new List<float>();
		aggregateLastPlayPercentages = new List<float>();
		aggregateTotalPercentages    = new List<float>();
		
		for(int i = 0; i < numberOfVertices; ++i) {
			aggregateTodayPercentages.Add(0);
			aggregateLastPlayPercentages.Add(0);
			aggregateTotalPercentages.Add(0);
		}
		
		//if (todayPercentages != null && todayPercentages.Count == numberOfVertices)
		if (todayPercentages != null)
		{
			// prepare aggregate data
			for(int i = 0; i < numberOfVertices; ++i) {
				for(int j = 0; j < numberOfNPCs; ++j) {
					// sum all the values
					if(todayPercentages[j] != null && todayPercentages[j][i] >= 0) {
						aggregateTodayPercentages[i]    += todayPercentages[j][i];
						if (lastPlayPercentages[j] != null)
							aggregateLastPlayPercentages[i] += lastPlayPercentages[j][i];
						if (averagePercentages[j] != null)
							aggregateTotalPercentages[i]    += averagePercentages[j][i];
					}
				}
				// divide to get the average
				aggregateTodayPercentages[i]    /= (numberOfNPCs - communicationMissing[i]);
				aggregateLastPlayPercentages[i] /= (numberOfNPCs - communicationMissing[i]);
				aggregateTotalPercentages[i]    /= (numberOfNPCs - communicationMissing[i]);
			}		
		}
		
		// populate total score label
		float score = 0;
		float npcScore;
		int noOfCategories = 0;
		int maxValue;
		for(int i = 0; i < numberOfNPCs; ++i) {
			npcScore = MainDatabase.Instance.getPoint(npc_interactions[i].InteractionID, ref noOfCategories);
			if (noOfCategories > 0)
			{
				maxValue = noOfCategories * 2;
				npcScore = (npcScore + maxValue) / (maxValue * 2);			
				score += npcScore;
			}
			else
				score += 1;
		}
		if(numberOfNPCs != 0) {
			score /= numberOfNPCs;
		}
		totalScore.sliderValue = score;
		parentsContainer.SetActive(false);

		for(int i = 0; i < numberOfNPCs; ++i) {
			expansions.Add(0);
		}

	}
	
	public void switchTabs(Tab targetTab) {
		switch(targetTab) {
			case Tab.Child:
				childrenContainer.SetActive(true);
				parentsContainer.SetActive(false);
				setParentCameras(parentCameras, false);
				setParentCameras (childrenCameras, true);
				currentTab = Tab.Child;
				break;
			case Tab.Parent:
				parentsContainer.SetActive(true);
				childrenContainer.SetActive(false);
				setParentCameras(parentCameras, true);
				setParentCameras (childrenCameras, false);
				currentTab = Tab.Parent;
				break;
		}	
	}
	
	void setParentCameras(List<GameObject> cameras, bool state) {
		for(int i = 0; i < cameras.Count; ++i) {
			cameras[i].SetActive (state);
		}		
	}
	
	void Start () {	
		//Debug.Log ("START");
		Time.timeScale = 1;
		//Invoke ("Reposition", .01f);
		//Invoke ("Reposition", .05f);
		if (sendEmail) {
			Invoke ("TakeScreenshot", 1);
		}
		
		setNPCDetails (false);
		switchTabs (initialTab);
		AudioManager.Instance.PlayMusic(backgroundMusic, 1);
	}
	/*
	public void Reposition()
	{
		//Debug.Log ("REPOSITIONING");
		npcContainer.repositionNow = true;
		Invoke ("Center", 0.01f);
	}
	
	public void Center()
	{
		//Debug.Log ("CENTER");
		npcCamera.Press(true);
		npcCamera.ConstrainToBounds(true);
	}
	*/
	// enable or disable NPC Details
	public void setNPCDetails(bool state) {
		foreach(GameObject npcDetail in npcDetails) {
			npcDetail.SetActive (state);		
		}		
	}
	
	void TakeScreenshot()
	{
		file = "Screenshot_" + Time.realtimeSinceStartup.ToString() + ".png";
		Application.CaptureScreenshot(file);
		Invoke ("SendEmail", 1);
	}
	
	void SendEmail()
	{
		Email.Instance.sendEmail("socialcluesanalytics@gmail.com","SocialClues: You have just completed a level","Here is how you did:", file);
	}
	
	void Update() {
		/*	
		//if(!verticalOffsetReady) {
			Vector3 centerOfPanel  = new Vector3(0, Math.Abs (panelBounds.max.y - panelBounds.min.y) + panelBounds.min.y, 0);
		//float test = (Math.Abs (panelBounds.max.y - panelBounds.min.y) + panelBounds.min.y);
		//Debug.Log ("wtf is going on: " + Math.Abs (panelBounds.max.y - panelBounds.min.y) + panelBounds.min.y);
		//Debug.Log ("Center of Panel: " + centerOfPanel.y);
		//Debug.Log ("test: " + test);
			Vector3 centerOfCamera = new Vector3(0, Math.Abs (cameraMax.y - cameraMin.y) + cameraMin.y, 0);
			verticalOffset = centerOfPanel.y - centerOfCamera.y;		
			verticalOffsetReady = true;
		//}	
		
		Debug.Log("Panel Bounds: " + panelBounds.min.ToString() + " to " + panelBounds.max.ToString());
		//Debug.Log("Camera: " + (npcCamera.GetComponent<UIViewport>().getCamera().transform.position + new Vector3(npcCamera.GetComponent<UIViewport>().getCamera().rect.xMin, npcCamera.GetComponent<UIViewport>().getCamera().rect.yMin, 0)).ToString() + " to " + (npcCamera.GetComponent<UIViewport>().getCamera().transform.position + new Vector3(npcCamera.GetComponent<UIViewport>().getCamera().rect.xMax, npcCamera.GetComponent<UIViewport>().getCamera().rect.yMax, 0)).ToString());		
		Debug.Log("Camera: " + cameraMin.ToString() + " to " + cameraMax.ToString());		
		
		float panelHeight = Math.Abs(panelBounds.max.y - panelBounds.min.y);
		//float cameraHeight = Math.Abs(npcCamera.GetComponent<UIViewport>().getCamera().rect.yMax - npcCamera.GetComponent<UIViewport>().getCamera().rect.yMin);
		float cameraHeight = Math.Abs(cameraMax.y - cameraMin.y);
		
		Debug.Log ("Camera Height: " + cameraHeight);
		Debug.Log ("Panel Height: " + panelHeight);
		
		float offset = 0.6f;
		//scrollBar.barSize = (cameraHeight + verticalOffset) / panelHeight;
		//scrollBar.barSize = (cameraHeight + offset) / panelHeight;
		
		//scrollBar.scrollValue = (Math.Abs(panelBounds.max.y - (cameraMax.y))) / (cameraHeight + (Math.Abs(panelBounds.max.y - (cameraMax.y))));
		//scrollBar.scrollValue = Math.Abs(panelBounds.max.y - (cameraMax.y)) / panelHeight;
		//scrollBar.scrollValue = ((Math.Abs(panelBounds.max.y - (cameraMax.y))) + (cameraHeight / 2)) / panelHeight;
		
		//Debug.Log ("Center of Panel: " + Math.Abs (panelBounds.max.y - panelBounds.min.y) + panelBounds.min.y);
		//Debug.Log ("Center of Panel: " + centerOfPanel.y);
		//Debug.Log ("Center of Camera: " + Math.Abs (cameraMax.y - cameraMin.y) + cameraMin.y);
		//Debug.Log ("Center of Camera: " + centerOfCamera.y);
		
		//Debug.Log ("Vertical offset: " + verticalOffset);
		*/
		
		/*
		Matrix4x4 m = npcCamera.GetComponent<UIViewport>().getCamera().cameraToWorldMatrix;
        Vector3 newMin = m.MultiplyPoint(cameraMin);
		Vector3 newMax = m.MultiplyPoint(cameraMax);
		
		Debug.Log ("newMin: " + newMin);
		Debug.Log ("newMax: " + newMax);
		*/
		
		// this will run once, and set cameraToPanelRatio
		/*
		if(readyToSetCameraToPanelRatio) {
			originalPanelHeight = panelHeight;
			originalCameraHeight = cameraHeight;
			Invoke("setCameraToPanelRatio", 0.1f);
		}
		*/
		//if (cameraPanelDistance < 0)
		//{
			//cameraPanelDistance = panelHeight - cameraHeight;
		//}
		
		//scrollBar.barSize = 0.5f;
		//scrollBar.barSize = (cameraHeight + cameraPanelDistance) / panelHeight;
		
		//Debug.Log ("Scroll bar size: " + scrollBar.barSize);
		
		//float temp = (Math.Abs (0.4f - cameraMax.y) * cameraToPanelRatio) / panelHeight;
		//float temp = (cameraMin.y  - panelBounds.min.y) / panelHeight;
		//Debug.Log ("Temp: " + temp);
		
		//scrollBar.scrollValue = 0.21f;
		//scrollBar.scrollValue = 1 - temp;
		
	}
	
	void setCameraToPanelRatio() {
		cameraToPanelRatio = originalPanelHeight / originalCameraHeight;
		readyToSetCameraToPanelRatio = false;
		cameraPanelDistance = originalPanelHeight - originalCameraHeight;
		//Debug.Log ("camerToPanel ratio: " + cameraToPanelRatio);
	}

	public void expandBackground(int amount) {
		background.transform.localScale = new Vector3(background.transform.localScale.x, background.transform.localScale.y + amount, background.transform.localScale.z);
	}

	public void contractBackground(int amount, bool reset) {
		if(reset == true) {
			background.transform.localScale = new Vector3(background.transform.localScale.x, amount, background.transform.localScale.z);
		} else {
			background.transform.localScale = new Vector3(background.transform.localScale.x, background.transform.localScale.y - amount, background.transform.localScale.z);
		}

	}
}
