using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadNPCsPortraits : MonoBehaviour {

	public List<GameObject> PortaitFramesGO;
	
	List<UILabel> labels;
	List<UISprite> portraits;

	AchievementManager am;
	List<DBCompletedNPCs> npcs;

	// Use this for initialization
	void Awake () {
		labels = new List<UILabel>();
		portraits = new List<UISprite>();

		npcs = MainDatabase.Instance.getCompletedNPCs(1);
		am = GameObject.Find("Camera").GetComponent<AchievementManager>();

		int i = 0;
		foreach (GameObject go in PortaitFramesGO) //looping all cells
		{
			if(i<npcs.Count)
			{
				foreach (Transform child in go.transform) // looping though all children
				{
					if(child.name == "label")
						labels.Add(child.GetComponent<UILabel>());
					else if(child.name == "portait")
						portraits.Add(child.GetComponent<UISprite>());

					// Turn on collider if its a npc
					if(child.name == "FrameCollider")
					{
						child.GetComponent<potraitClicked>().manager = am;
						child.collider.enabled = true;
						child.GetComponent<UISprite>().spriteName = "Blank White Photo";
						child.localPosition = new Vector3(0,0,0);
					}
				}
			// if its less then number of npcs completed then replace with their name and portrait
				labels[i].text = npcs[i].NPCName;
				portraits[i].spriteName = npcs[i].NPCName;
				portraits[i].enabled = true;
				go.name = npcs[i].NPCName;
			}
			else
				break;

			i++;
		}
	}

}
