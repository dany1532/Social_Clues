using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NPCData : MonoBehaviour {
	public DBCompletedNPCs generalInfo;
	public UILabel name;
	public UILabel stars;
	public UILabel category;
	public UISprite portrait;

	public UILabel e, c, i, m, p;
	public List <float> averagePercentages;

	public UISprite border;

	public int interactionTimeInSeconds;
	public UILabel interactionTime;

	public GameObject rowContainer;
	public GameObject row;

	public List<DBTotalPercentageWithDate> percentages;

	// Use this for initialization
	void Start () {
		//generalInfo = new DBCompletedNPCs();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void populateData() {
		//Debug.Log("NPC ID: " + generalInfo.NPCID);
		//Debug.Log("NPC Name: " + generalInfo.NPCName);
		//Debug.Log("NPC Stars: " + generalInfo.stars);
		//Debug.Log("NPC Category: " + generalInfo.Category);

		name.text = generalInfo.NPCName;
		stars.text = "x" + generalInfo.stars.ToString();
		category.text = generalInfo.Category;
		portrait.spriteName = generalInfo.NPCName;

		// set average values
		e.text = averagePercentages[0].ToString() + "%";
		c.text = averagePercentages[1].ToString() + "%";
		i.text = averagePercentages[2].ToString() + "%";
		m.text = averagePercentages[3].ToString() + "%";
		p.text = averagePercentages[4].ToString() + "%";

		// set average value colors
		AnalyticsHUBController.setColor(e, averagePercentages[0]);
		AnalyticsHUBController.setColor(c, averagePercentages[0]);
		AnalyticsHUBController.setColor(i, averagePercentages[0]);
		AnalyticsHUBController.setColor(m, averagePercentages[0]);
		AnalyticsHUBController.setColor(p, averagePercentages[0]);

		// set color of border
		float averagePercentageOfNPC = 0;
		for(int j = 0; j < averagePercentages.Count; ++j) {
			averagePercentageOfNPC += averagePercentages[j];
		}
		averagePercentageOfNPC /= averagePercentages.Count;
		AnalyticsHUBController.setColor(border, averagePercentageOfNPC);

		// set average interaction time
		TimeSpan interactionTimeSpan = TimeSpan.FromSeconds(interactionTimeInSeconds);
		string timeText = string.Format("{0:D2}:{1:D2}", interactionTimeSpan.Minutes, interactionTimeSpan.Seconds);
		interactionTime.text = timeText;

		Vector3 lastRowPosition = Vector3.zero; 
		// populate the back with data
		for(int j = 0; j < percentages.Count; ++j) {
			GameObject newRow = (GameObject)Instantiate(row);
			newRow.transform.parent = rowContainer.transform;
			newRow.transform.localScale = new Vector3(1, 1, -1);
			if(j == 0) {
				newRow.transform.localPosition = Vector3.zero;
			} else {
				newRow.transform.localPosition = new Vector3(0, lastRowPosition.y - 24.7f, 0);
			}
			lastRowPosition = newRow.transform.localPosition;

			newRow.GetComponent<NPCRowData>().percentages = percentages[j];
			newRow.GetComponent<NPCRowData>().populateRow();
		}
	}
}
