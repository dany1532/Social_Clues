using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class LoadArtSet : MonoBehaviour {


	
	public List<GameObject> PortaitFramesGO;
	/**
		ArtSet = 1,
		TrainSet = 2,
		Bricks = 3
	}
	*/
	public int currentSetCategory = 1;

	public UIAtlas achievementsAtlas;
	UISprite portrait;
	
	AchievementManager am;
	List<DBToyInfo> artToys;
	
	// Use this for initialization
	void Awake () {
		artToys = MainDatabase.Instance.getUserToysByCategory(1,currentSetCategory);
		Debug.Log(currentSetCategory);
		int i = 0;
		foreach (GameObject go in PortaitFramesGO) //looping all cells
		{
			if(i<artToys.Count)
			{
				string toyAchieved = "";
				Color c = new Color();
				foreach (Transform child in go.transform) // looping though all children
				{


					if(child.name == "label")
						child.GetComponent<UILabel>().text = artToys[i].Filename;

					else if(child.name == "colors")
					{
						foreach (Transform colors in child.transform) // looping though all children
						{

							if(artToys[i].UserID != -1)
							{
								c = new Color(artToys [i].ToyColorR / 255.0f, (float)artToys [i].ToyColorG / 255.0f, (float)artToys [i].ToyColorB / 255.0f);
								colors.GetComponent<UISprite>().color = c;
								colors.GetComponent<UIButton>().hover = c;
								colors.GetComponent<UIButton>().pressed = c;
								toyAchieved = artToys[i].Filename;
								colors.collider.enabled = true;
							}
							i++;
						}
					}
					/*
					if(toyAchieved != "" && child.name == "FrameCollider") {
						child.GetComponent<UISprite>().color = new Color(214f/255, 214f/255, 214f/255);
					}
					*/

					//if(toyAchieved != "" && child.name == "portait")
					if(child.name == "portait")
					{
						//Debug.Log("here " + toyAchieved);
						if(toyAchieved != "") {
							child.GetComponent<UISprite>().spriteName = toyAchieved;
							child.GetComponent<UISprite>().color = c;
						}
					}

					// Turn on collider if its a npc
					if(child.name == "FrameCollider")
					{
						child.collider.enabled = true;
						child.GetComponent<UISprite>().color = new Color(238f/255, 238f/255, 238f/255);
						go.name = toyAchieved;
						child.GetComponent<UISprite>().spriteName = "Blank White Photo";
						child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y + 14, child.transform.localPosition.z);
					}
				}
//				else
//				{
//					
//				}
				//go.name = artToys[i].Filename;
			}
			else
			{
				foreach (Transform child in go.transform) // looping though all children
				{
					if(child.name == "portait" && go.name == "ArtCell")
					{
						child.GetComponent<UISprite>().atlas = achievementsAtlas;
						child.GetComponent<UISprite>().gameObject.gameObject.SetActive (false);
					}
					if(child.name == "Pin" && go.name == "ArtCell") {
						child.gameObject.SetActive (false);
					}
				}
			}


		}
	}

}
