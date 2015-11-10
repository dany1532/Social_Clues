using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Analytics : MonoBehaviour {
	private List<DBSelectAll> results = new List<DBSelectAll>();
	private int result;
	private List<float> float_results = new List<float>();
	private List<DBReplyTimeWithType> reply_results = new List<DBReplyTimeWithType>();
	public static List<DBNPCInteraction> npc_interactions = new List<DBNPCInteraction>();
	
	public UISprite npc_sprite;
	
	public UILabel npc_name,
				   lesson,
				   time_amount,
				   score_amount;
	
	private int numberOfNPCs;
	
	public static int levelPlayID = 1;
	
	// Use this for initialization
	void Start () {
		int noOfCategories = 0;
		
		npc_interactions = MainDatabase.Instance.getNPCandInteraction(levelPlayID);
		results = MainDatabase.Instance.SelectAll(npc_interactions[0].NPCID);
		// NPC Sprite
		switch(results[0].NPCName) {
			case "Amy":
				npc_sprite.spriteName = "Amy6";
				break;
		}
		
		// Top Section
		result = MainDatabase.Instance.SelectInteractionTime (1);
		npc_name.text     = results[0].NPCName;
		lesson.text       = results[0].LevelName;
		time_amount.text  = TimeSpan.FromSeconds(result).ToString ();
		score_amount.text = MainDatabase.Instance.getPoint(1, ref noOfCategories).ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
