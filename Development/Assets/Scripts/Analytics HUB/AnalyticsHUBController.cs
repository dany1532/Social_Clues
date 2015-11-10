using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnalyticsHUBController : Singleton<AnalyticsHUBController> {

	public UILabel playerName;

	public List<DBCompletedNPCs> completedNPCs;

	public int userID;

	// for the bar graph
	public List<UISprite> averageBars     = new List<UISprite>();
	public List<UISprite> mostRecentBars  = new List<UISprite>();
	public List<UISprite> firstPlayBars   = new List<UISprite>();

	public List<GameObject> averageLabels     = new List<GameObject>();
	public List<GameObject> mostRecentLabels  = new List<GameObject>();
	public List<GameObject> firstPlayLabels   = new List<GameObject>();

	public GameObject barGraphController;

	public GameObject NPC;
	public GameObject NPCContainer;
	public GameObject lockedNPC;

	// for play time
	public UILabel totalPlayTime;

	public static Color RED = new Color(187.0f/255, 0.0f/255, 0.0f/255);
	public static Color YELLOW = new Color(253.0f/255, 190.0f/255, 7.0f/255);
	public static Color GREEN = new Color(123.0f/255, 144.0f/255, 57.0f/255);

	public GameObject sliderContainer;

	void Awake() {
		instance = this;

		userID = ApplicationState.Instance.userID;
		
		if (ApplicationState.Instance.userID == -1) {
			userID = 1;
		}

		DBUserInfo userInfo = MainDatabase.Instance.getSingleUserInfo(userID);

		playerName.text = userInfo.UserName;

		populateBarGraph();
		// show labels for averages (using Bar.cs)
		barGraphController.SendMessage("switchLabelDisplay", BarSwitch.GraphType.TODAY, SendMessageOptions.DontRequireReceiver);

		// play time
		int playTime = MainDatabase.Instance.getTotalInteractionTime(userID);
		totalPlayTime.text = TimeSpan.FromSeconds(playTime).ToString();

		Vector3 lastChildPosition = Vector3.zero; 
		completedNPCs = MainDatabase.Instance.getCompletedNPCs(userID);
		float rowIndex = 0;
		List<DBTotalPercentageWithDate> percentages = new List<DBTotalPercentageWithDate>();
		List <float> averagePercentages = new List<float>();
		int interactionTimeForNPC = 0;
		float averagePointsForNPCs = 0;
		for(int i = 0; i < 9; ++i) {
			// create NPC
			GameObject npc;
			if(i < completedNPCs.Count) {
				npc = (GameObject)Instantiate(NPC);
			} else {
				npc = (GameObject)Instantiate(lockedNPC);
			}

			// position NPC
			npc.transform.parent = NPCContainer.transform;
			npc.transform.localScale = Vector3.one;
			if(i == 0) {
				npc.transform.localPosition = new Vector3(0, 768, 0);
			} else if(i % 3 == 0) {
				// goes down to 516 = 252
				++rowIndex;
				npc.transform.localPosition = new Vector3(0, 768 - (252 * rowIndex), 0);
			} else {
				npc.transform.localPosition = new Vector3(lastChildPosition.x + 322, lastChildPosition.y, 0);
			}
			lastChildPosition = npc.transform.localPosition;

			if(i < completedNPCs.Count) {
				// pass NPC data
				npc.GetComponent<NPCData>().generalInfo = completedNPCs[i];
				averagePercentages = MainDatabase.Instance.calTotalPercentage(completedNPCs[i].NPCID, userID);
				for(int j = 0; j < averagePercentages.Count; ++j) {
					averagePercentages[j] = (float)Mathf.Round(averagePercentages[j]);
				}
				npc.GetComponent<NPCData>().averagePercentages = averagePercentages;

				interactionTimeForNPC = MainDatabase.Instance.getTotalInteractionTimeForNPC(userID, completedNPCs[i].NPCID);
				npc.GetComponent<NPCData>().interactionTimeInSeconds = interactionTimeForNPC;

				percentages = MainDatabase.Instance.calTotalPercentageECIMPwithDATES(userID, completedNPCs[i].NPCID);
				npc.GetComponent<NPCData>().percentages = percentages;

				float points = MainDatabase.Instance.getPointsForNPC(completedNPCs[i].NPCID, userID);
				//Debug.Log ("points: " + points);
				npc.GetComponentInChildren<Slider>().sliderValue = points;
				averagePointsForNPCs += points;
				npc.GetComponent<NPCData>().populateData();
			}
		}

		sliderContainer.GetComponent<Slider>().sliderValue = averagePointsForNPCs / completedNPCs.Count;

/*
		for(int i = completedNPCs.Count; i < 9; ++i) {
			GameObject newLockedNPC = (GameObject)Instantiate(lockedNPC);
			newLockedNPC.transform.parent = NPCContainer.transform;
			newLockedNPC.transform.localScale = Vector3.one;
			if(i % 3 == 0) {
				++rowIndex;
				newLockedNPC.transform.localPosition = new Vector3(0, 768 - (252 * rowIndex), 0);
			} else {
				newLockedNPC.transform.localPosition = new Vector3(lastChildPosition.x + 322, lastChildPosition.y, 0);
			}
		}*/

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void populateBarGraph() {
		float offset = 0.025f;

		// Averages
		List<float> averages = MainDatabase.Instance.calTotalAveragePercentageECIMP(userID);

		if (averages != null) {
				for (int i = 0; i < averages.Count; ++i) {
						averageBars [i].GetComponent<UIStretch> ().relativeSize.y = averages [i] / 100;
						averageLabels [i].GetComponentInChildren<UILabel> ().text = Mathf.Round (averages [i]) + "%";
						averageLabels [i].GetComponent<UIAnchor> ().relativeOffset.y = (averages [i] / 100) + offset;
				}
		}
		// Most recent
		List<float> mostRecent = MainDatabase.Instance.calPercentageForLatestInteractionID(userID);

		if (mostRecent != null)
		{
			for(int i = 0; i < mostRecent.Count; ++i) {
				mostRecentBars[i].GetComponent<UIStretch>().relativeSize.y = mostRecent[i] / 100;
				mostRecentLabels[i].GetComponentInChildren<UILabel>().text = Mathf.Round(mostRecent[i]) + "%";
				mostRecentLabels[i].GetComponent<UIAnchor>().relativeOffset.y = (mostRecent[i] / 100) + offset;
			}
		}
		// Most recent
		List<float> firstPlay = MainDatabase.Instance.calPercentageForFirstInteractionID(userID);

		if (firstPlay != null) {
				for (int i = 0; i < firstPlay.Count; ++i) {
						firstPlayBars [i].GetComponent<UIStretch> ().relativeSize.y = firstPlay [i] / 100;
						firstPlayLabels [i].GetComponentInChildren<UILabel> ().text = Mathf.Round (firstPlay [i]) + "%";
						firstPlayLabels [i].GetComponent<UIAnchor> ().relativeOffset.y = (firstPlay [i] / 100) + offset;
				}
		}
	}

	// sets color of label -- 0%->50% = red, 50%->75% = yellow, 75%->100% = green
	public static void setColor(UILabel label, float percentage) {
		if(percentage <= 50.0) {
			label.color = RED;
		} else if(percentage > 50.0 && percentage <= 75.0) {
			label.color = YELLOW;
		} else {
			label.color = GREEN;
		}
	}

	// overloaded: sets color of sprite
	public static void setColor(UISprite sprite, float percentage) {
		if(percentage <= 50.0) {
			sprite.color = RED;
		} else if(percentage > 50.0 && percentage <= 75.0) {
			sprite.color = YELLOW;
		} else {
			sprite.color = GREEN;
		}
	}

}
