using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Bar : MonoBehaviour {
	/*
	public UISprite bar1_1,
					bar1_2,
					bar1_3,
					bar2_1,
					bar2_2,
					bar2_3,
					bar3_1,
					bar3_2,
					bar3_3,
					bar4_1,
					bar4_2,
					bar4_3,
					bar5_1,
					bar5_2,
					bar5_3;
	*/
	private List<float> float_results  = new List<float>();
	private List<float> float_results2 = new List<float>();
	
	public List<UISprite> todayBars     = new List<UISprite>();
	public List<UISprite> lastPlayBars  = new List<UISprite>();
	public List<UISprite> aggregateBars = new List<UISprite>();
	
	public List<GameObject> todayLabels     = new List<GameObject>();
	public List<GameObject> lastPlayLabels  = new List<GameObject>();
	public List<GameObject> aggregateLabels = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		int userID = ApplicationState.Instance.userID;
		//float_results  = MainDatabase.Instance.calTotalPercentage();
		float_results = MainDatabase.Instance.calTotalPercentage(userID,ApplicationState.Instance.userID);
		float_results2 = MainDatabase.Instance.calPercentageForInteractionID(userID,userID);

		
		// set values for today
		if (AnalyticsController.Instance.aggregateTodayPercentages != null)
			for(int i = 0; i < todayBars.Count; ++i) {
				todayBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.aggregateTodayPercentages[i] / 100;
			}
		else
		{
			Debug.LogWarning("Aggregate today percentage is null");
			for(int i = 0; i < todayBars.Count; ++i) {
				todayBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		
		if (AnalyticsController.Instance.aggregateLastPlayPercentages != null)
			// set values for last play
			for(int i = 0; i < lastPlayBars.Count; ++i) {
				lastPlayBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.aggregateLastPlayPercentages[i] / 100;
			}
		else
		{
			Debug.LogWarning("Aggregate last play percentage is null");
			// set values for last play
			for(int i = 0; i < lastPlayBars.Count; ++i) {
				lastPlayBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		
		if (AnalyticsController.Instance.aggregateTotalPercentages != null)
			// set values for aggregate
			for(int i = 0; i < aggregateBars.Count; ++i) {
				aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.aggregateTotalPercentages[i] / 100;
			}
		else
		{
			// set values for aggregate
			for(int i = 0; i < aggregateBars.Count; ++i) {
				aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
			Debug.LogWarning("Aggregate total percentage is null");
		}

		// aggregate today labels
		if (AnalyticsController.Instance.aggregateTodayPercentages != null)
			for(int i = 0; i < todayLabels.Count; ++i) {
				//percentageLabels. [i].text = Mathf.Round(AnalyticsController.aggregateTodayPercentages[i]) + "%";
				todayLabels[i].GetComponentInChildren<UILabel>().text = Mathf.Round(AnalyticsController.Instance.aggregateTodayPercentages[i]) + "%";
				todayLabels[i].GetComponent<UIAnchor>().relativeOffset.y = (AnalyticsController.Instance.aggregateTodayPercentages[i] / 100) + 0.025f;
			}
		else
			for(int i = 0; i < todayLabels.Count; ++i) {
			//percentageLabels. [i].text = Mathf.Round(AnalyticsController.aggregateTodayPercentages[i]) + "%";
			todayLabels[i].GetComponentInChildren<UILabel>().text = "-";
			todayLabels[i].GetComponent<UIAnchor>().relativeOffset.y = 0;
		}

		// aggregate last play labels
		if (AnalyticsController.Instance.aggregateLastPlayPercentages != null) {
			for(int i = 0; i < lastPlayLabels.Count; ++i) {
				lastPlayLabels[i].GetComponentInChildren<UILabel>().text = Mathf.Round(AnalyticsController.Instance.aggregateLastPlayPercentages[i]) + "%";
				lastPlayLabels[i].GetComponent<UIAnchor>().relativeOffset.y = (AnalyticsController.Instance.aggregateLastPlayPercentages[i] / 100) + 0.025f;
			}
		} else {
			for(int i = 0; i < todayLabels.Count; ++i) {
				lastPlayLabels[i].GetComponentInChildren<UILabel>().text = "-";
				lastPlayLabels[i].GetComponent<UIAnchor>().relativeOffset.y = 0;
			}
		}

		// aggregate total labels
		if (AnalyticsController.Instance.aggregateTotalPercentages != null) {
			for(int i = 0; i < aggregateLabels.Count; ++i) {
				aggregateLabels[i].GetComponentInChildren<UILabel>().text = Mathf.Round(AnalyticsController.Instance.aggregateTotalPercentages[i]) + "%";
				aggregateLabels[i].GetComponent<UIAnchor>().relativeOffset.y = (AnalyticsController.Instance.aggregateTotalPercentages[i] / 100) + 0.025f;
			}
		} else {
			for(int i = 0; i < aggregateLabels.Count; ++i) {
				aggregateLabels[i].GetComponentInChildren<UILabel>().text = "-";
				aggregateLabels[i].GetComponent<UIAnchor>().relativeOffset.y = 0;
			}
		}

		// disable labels except for Today labels
		switchLabelDisplay(BarSwitch.GraphType.TODAY);
	}

	public void switchLabelDisplay(BarSwitch.GraphType graphType) {
		toggleLabels(todayLabels, false);
		toggleLabels(lastPlayLabels, false);
		toggleLabels(aggregateLabels, false);
		switch(graphType) {
			case BarSwitch.GraphType.TODAY:
				toggleLabels(todayLabels, true);
				break;
			case BarSwitch.GraphType.LAST_PLAY:
				toggleLabels(lastPlayLabels, true);
				break;
			case BarSwitch.GraphType.AGGREGATE:
				toggleLabels(aggregateLabels, true);
				break;
		}
	}

	private void toggleLabels(List<GameObject> gameObjects, bool value) {
		for(int i = 0; i < gameObjects.Count; ++i) {
			gameObjects[i].SetActive(value);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}


}
