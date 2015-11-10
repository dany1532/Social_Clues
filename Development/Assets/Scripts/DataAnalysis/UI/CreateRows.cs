using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateRows : MonoBehaviour {
	public GameObject rowPrefab;
	private float amountToUpdate = -33;
	private float currentAmountToUpdate;
	public int indexOfNPC;
	private GameObject npcDetailsController;
	
	// Use this for initialization
	void Start () {
		currentAmountToUpdate = 0;
		npcDetailsController = this.gameObject.transform.parent.gameObject;
		
		//int size = 240;
		for(int i = 0; i < 5; ++i) {
			GameObject newRow;
			newRow = (GameObject)Instantiate(rowPrefab);
			newRow.transform.parent = this.transform;
			newRow.transform.localPosition = Vector3.zero;
			newRow.transform.localScale = Vector3.one;
			newRow.GetComponent<NewRow>().rowIndex = i + 1;
			newRow.GetComponentInChildren<ExpandableAnswers>().type = i + 1;
			newRow.GetComponentInChildren<ExpandableAnswers>().indexOfNPC = indexOfNPC;
			npcDetailsController.GetComponent<NPCDetailsController>().objectsToUpdate.Add(newRow);
			newRow.GetComponentInChildren<Dropdown>().detailsController = npcDetailsController.GetComponent<NPCDetailsController>();
			newRow.GetComponentInChildren<Dropdown>().index = i;
			newRow.GetComponentInChildren<Dropdown>().indexOfNPC = indexOfNPC;
			//newRow.GetComponentInChildren<DropDownMenu>()
			PopulateCategory (newRow, i + 1);
			
			newRow.GetComponent<NewRow>().amountToUpdate = currentAmountToUpdate;
			currentAmountToUpdate += amountToUpdate;
		}
	}

	public void PopulateCategory(GameObject row, int counter)
	{
		string text;
		switch(counter) {
			case 1:	text = "Emotion";				break;
			case 2:	text = "Comprehension";			break;
			case 3:	text = "Initiate Conversation";	break;
			case 4:	text = "Maintain Conversation";	break;
			case 5:	text = "Problem Solving";		break;
			default: text = "MISSING";				break;
		}
		
		List<DBReplyTimeWithType> timeTaken = MainDatabase.Instance.getTimeTaken(counter, AnalyticsController.Instance.npc_interactions[indexOfNPC].InteractionID);
		//GameObject row = this.transform.Find ("Panel/" + counter.ToString()).gameObject;
		row.name = text;

		if(timeTaken.Count != 0) {
			row.transform.Find ("Label Anchor/Label").gameObject.GetComponent<UILabel>().text = text;
			if (AnalyticsController.Instance.todayPercentages != null && AnalyticsController.Instance.todayPercentages[indexOfNPC] != null && AnalyticsController.Instance.todayPercentages[indexOfNPC].Count > (counter - 1))
				row.transform.Find ("Today Anchor/Label").gameObject.GetComponent<UILabel>().text = ((int)AnalyticsController.Instance.todayPercentages[indexOfNPC][counter-1]).ToString();
			else
				row.transform.Find ("Today Anchor/Label").gameObject.GetComponent<UILabel>().text = "-";

			if (AnalyticsController.Instance.lastPlayPercentages != null && AnalyticsController.Instance.lastPlayPercentages[indexOfNPC] != null && AnalyticsController.Instance.lastPlayPercentages[indexOfNPC].Count > (counter-1))				
				row.transform.Find ("Last Anchor/Label").gameObject.GetComponent<UILabel>().text = ((int)AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][counter-1]).ToString();
			else
				row.transform.Find ("Last Anchor/Label").gameObject.GetComponent<UILabel>().text = "-";
			
			//top.last_percentage.text = ((int)AnalyticsController.todayPercentages[indexOfNPC][counter-1]).ToString();
			//top.today_percentage.text  = ((int)AnalyticsController.lastPlayPercentages[indexOfNPC][counter-1]).ToString();
		} else {
			//top.gameObject.SetActive(false);
			//row.GetComponentInChildren<Dropdown>().contract ();
			//Debug.Log ("counter: " + counter);
			row.transform.Find ("Label Anchor/Label").gameObject.GetComponent<UILabel>().text = text;
			row.transform.Find ("Today Anchor/Label").gameObject.GetComponent<UILabel>().text = "N/A";
			row.transform.Find ("Last Anchor/Label").gameObject.GetComponent<UILabel>().text = "N/A";
			row.SetActive(false);
		}
	}		
	
	// Update is called once per frame
	void Update () {
	
	}
}
