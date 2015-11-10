using UnityEngine;
using System.Collections;

public class ButtonClickHandlerNancy : MonoBehaviour {
	
	public GameObject minigame;
	public bool isObj, found;
	public bool isInterest;
	public int correct;
	public GameObject findObj;
		
	void Start(){
		found = false;
	}
	
	
	void OnPress(bool isDown)
	{
		if(isDown)
		{
			Debug.Log (this.gameObject.name + "clicked");
			if(isObj)
			{
				if(found)
				{
					minigame.GetComponent<NewNancyManager>().ShowDialogue(NewNancyManager.DialogueType.FOUND);
				}
				else
				{
					findObj.SetActive(false);
					found = true;
					minigame.GetComponent<NewNancyManager>().HiddenObjectFound(findObj.transform.position);	
				}
			}
			else if(isInterest)
			{
				if(correct == minigame.GetComponent<NewNancyManager>().currentLevel)
					minigame.GetComponent<NewNancyManager>().interestCorrect(Vector3.zero);
				else
				{
					this.gameObject.transform.parent.gameObject.SetActive(false);
					minigame.GetComponent<NewNancyManager>().interestIncorrect(null);
				}
			}
			else
			{
				minigame.GetComponent<NewNancyManager>().ObjNotFound();
			}
		}

	}
	
	
	


}

