using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExpandableAnswers : MonoBehaviour {
	
	public int indexOfNPC;
	public List<UILabel>  answerText  = new List<UILabel>();
	public List<UILabel>  timeTaken   = new List<UILabel>();
	public List<UISprite> backgrounds = new List<UISprite>();
	public List<UISprite> smileys     = new List<UISprite>();
	
	private List<DBReplyTimeWithType> questions;
	public int type; // i.e. Emotion (1), ...
	
	// Use this for initialization
	void Start () {
		questions = MainDatabase.Instance.getTimeTaken(type, AnalyticsController.Instance.npc_interactions[indexOfNPC].InteractionID);

		//for(int i = 0; i < questions.Count; ++i) {
		for(int i = 0; i < answerText.Count; ++i) {
			//Debug.Log ("type: " + type + ", index: " + i);
			answerText[i].text = questions[i].ReplyText;
			timeTaken[i].text = TimeSpan.FromSeconds((int)questions[i].Timetaken).ToString().Substring(4);
			
			// set the background color, text color, and smiley
			switch(questions[i].ReplyType) {
			case 0:
				//rows[rows.Count - 1].background.color = Color.green;
				backgrounds[i].color = new Color(0.388f, 0.635f, 0.141f, 1);
				smileys[i].spriteName = "Happy Face";
				answerText[i].color = Color.white;
				timeTaken[i].color = new Color(0.388f, 0.635f, 0.141f, 1);
				break;
			case 1:
				//rows[rows.Count - 1].background.color = Color.red;
				backgrounds[i].color = new Color(0.914f, 0.270f, 0.133f, 1);
				smileys[i].spriteName = "Sad Face";
				answerText[i].color = Color.white;
				timeTaken[i].color = new Color(0.914f, 0.270f, 0.133f, 1);
				break;
			case 2:
				//rows[rows.Count - 1].background.color = new Color(0.75f, 0.75f, 0.75f, 1);
				backgrounds[i].color = new Color(0.859f, 0.859f, 0.859f, 1);
				smileys[i].spriteName = "Neutral Face";
				answerText[i].color = Color.black;
				timeTaken[i].color = new Color(0.588f, 0.588f, 0.588f, 1);
				break;
			}
			
			//Debug.Log ("index of NPC: " + indexOfNPC + ", time: " + TimeSpan.FromSeconds((int)questions[i].Timetaken).ToString().Substring(4));
			//Debug.Log ("questions.Count: " + questions.Count);
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
