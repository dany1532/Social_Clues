using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnalyticsPopulateTable : MonoBehaviour {
	private UITable table;
	public GameObject row;
	public int numberOfRows = 4;
	List<AnalyticsQuestionRow> rows;
	private List<DBReplyTimeWithType> questions;
	public int type = 1;	// will be overriden before use
	TweenScale tweener;
	public UITable topTable;
	
	public int indexOfNPC;
	
	// Use this for initialization
	void Start () {
		tweener = GetComponent<TweenScale>();
		topTable = transform.parent.GetComponent<UITable>();
		tweener.eventReceiver = topTable.gameObject;
	}
	
	public void Populate() {
		//questions = new List<DBReplyTimeWithType>();
		
		GameObject newRow;
		
		table = this.GetComponentInChildren<UITable>();
		rows = new List<AnalyticsQuestionRow>();
		
		//questions = MainDatabase.Instance.getTimeTaken(type, 1);
		questions = MainDatabase.Instance.getTimeTaken(type, AnalyticsController.Instance.npc_interactions[indexOfNPC].InteractionID);	
		
		//for(int i = 0; i < numberOfRows; ++i) {
		for(int i = 0; i < questions.Count; ++i) {
			newRow = (GameObject)Instantiate(row);
			rows.Add(newRow.GetComponentInChildren<AnalyticsQuestionRow>());
			
			// set the answer
			rows[rows.Count-1].answerName.text = questions[i].ReplyText;
			
			// set the time taken
			rows[rows.Count-1].timeTaken.text = TimeSpan.FromSeconds((int)questions[i].Timetaken).ToString().Substring(4);
			
			// set the background color for the answer
			switch(questions[i].ReplyType) {
			case 0:
				rows[rows.Count - 1].background.color = Color.green;
				break;
			case 1:
				rows[rows.Count - 1].background.color = Color.red;
				break;
			case 2:
				rows[rows.Count - 1].background.color = new Color(0.75f, 0.75f, 0.75f, 1);
				break;
			}
		
			// fill score gradient
			// first convert from -2 to 2 point scale to 0 to 4
			int baseScore = questions[i].ReplyPoints + 2;
			float redPercentage, greenPercentage;
			greenPercentage = (float)baseScore / 4;
			redPercentage   = 1.0f - greenPercentage;
			float red, green, blue;
			red   = 255 * redPercentage;
			green = 255 * greenPercentage;
			blue  = 0;
			rows[rows.Count - 1].scoreGradient.color = new Color(red, green, blue, 1.0f);
			
			while (newRow.transform.childCount > 0)
			{
				Transform child = newRow.transform.GetChild(0);
				child.parent = table.transform;
				//child.name = (i+1).ToString() + child.name;
				child.name = indexOfNPC.ToString() + (i+1).ToString() + child.name;
				child.localScale = Vector3.one;
			}
			rows[rows.Count-1].SetScale();
			Destroy(newRow);
		}
		table.repositionNow = true;		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
