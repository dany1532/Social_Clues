using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NPCChild : MonoBehaviour {

	public UISprite character,
					toy;
	
	public UILabel characterName, name,
				   lesson;
	
	public List<UISprite> listStars;
	
	public int indexOfNPC;
	//public UIButtonSound lessonAudio;

	public PlaySoundFX lessonAudio;
	
	float starOffset = 0.04f;
	
	// Use this for initialization
	void Start () {
		characterName.text 	 = AnalyticsController.Instance.npc_interactions [indexOfNPC].NPCName;
		toy.spriteName       = AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCTexture + "_Toy";
		character.spriteName = AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCTexture;
		name.text            = AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCName;
		toy.spriteName 		 = AnalyticsController.Instance.npc_interactions[indexOfNPC].ToyName;
		toy.color			 = new Color(AnalyticsController.Instance.npc_interactions[indexOfNPC].ToyColorR/ 255.0f, AnalyticsController.Instance.npc_interactions[indexOfNPC].ToyColorG/ 255.0f, AnalyticsController.Instance.npc_interactions[indexOfNPC].ToyColorB/ 255.0f, 1);
		lesson.text          = AnalyticsController.Instance.categories[indexOfNPC];
		int indexForAudio;
		//lessonAudio.audioClip = AnalyticsController.Instance.Instance.categoriesAudio[AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCID-1];
		lessonAudio.audioClip = AnalyticsController.Instance.categoriesAudio[AnalyticsController.Instance.npc_interactions[indexOfNPC].NPCID-1];
		int numStars 		 = AnalyticsController.Instance.npc_interactions[indexOfNPC].Stars;
		
		for(int i = 0; i < numStars; i++){
			listStars[i].enabled = true;
			Vector3 pos = listStars[i].transform.position;
			pos.x += ((5 - numStars) * starOffset);
			listStars[i].transform.position = pos;
//			listStars[i].transform.Translate(listStars[i].transform.position.x + ((5 - numStars) * starOffset), 
//											 listStars[i].transform.position.y, listStars[i].transform.position.z);
		}
		
		string toyName = Regex.Replace(toy.spriteName, "([a-z])([A-Z])", "$1 $2");
		toyName = sep(toyName);
		
		name.text = toyName;
		
		UIStretch stretch = character.gameObject.GetComponent<UIStretch>();
		stretch.initialSize = new Vector2(character.GetAtlasSprite().inner.width, character.GetAtlasSprite().inner.height);
		
		stretch = toy.gameObject.GetComponent<UIStretch>();
		stretch.initialSize = new Vector2(toy.GetAtlasSprite().inner.width, toy.GetAtlasSprite().inner.height);
	}
	
	 public string sep(string s)
    {
        int l = s.IndexOf("_");
        if (l >0)
        {
            return s.Substring(0, l);
        }
        return s;

    }
}
