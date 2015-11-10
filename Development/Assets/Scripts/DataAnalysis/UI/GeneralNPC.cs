using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GeneralNPC : MonoBehaviour {
	
	public UISprite npc_sprite;
	
	public UILabel npc_name,
				   lesson,
				   time_amount,
				   score_amount,
					stars;
	
	public Slider slider;
	
	public int indexOfNPC;
	// Use this for initialization
	void Start () {
		// Top Section
		//npc_name.text         = AnalyticsController.Instance.interactionInfo[0].NPCName;
		//lesson.text           = AnalyticsController.Instance.interactionInfo[0].Category;
		//time_amount.text      = AnalyticsController.Instance.interactionTime[1].Subtract(AnalyticsController.Instance.interactionTime[0]).ToString();
		//npc_sprite.spriteName = AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCTexture;
		if(AnalyticsController.Instance.npc_interactions != null & indexOfNPC < AnalyticsController.Instance.npc_interactions.Count)
		{
			npc_sprite.spriteName = AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCName;
			npc_name.text         = AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCName;
			stars.text 			  = "x" + AnalyticsController.Instance.npc_interactions[indexOfNPC].Stars.ToString();
		}
		if (AnalyticsController.Instance.categories != null)
			lesson.text           = AnalyticsController.Instance.categories[indexOfNPC];
		else
			lesson.text = string.Empty;
		
		time_amount.text      = TimeSpan.FromSeconds(AnalyticsController.Instance.interactionTime[indexOfNPC]).ToString ();
		//score_amount.text     = MainDatabase.Instance.getPoint(AnalyticsController.Instance.npc_interactions[indexOfNPC].InteractionID).ToString();
		
		int noOfCategories = 0;
		float sliderValue = MainDatabase.Instance.getPoint(AnalyticsController.Instance.npc_interactions[indexOfNPC].InteractionID, ref noOfCategories);
		if (noOfCategories > 0)
		{
			int maxValue = noOfCategories * 2;
			sliderValue = (sliderValue + maxValue) / (maxValue * 2);
			slider.sliderValue = sliderValue;
		}
		else
			slider.sliderValue = 1;
	}
}
