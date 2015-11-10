using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnalyticsPopulateCategories : MonoBehaviour {
	private UITable table;
	public GameObject category;
	private int numberOfCategories = 5;
	public int indexOfNPC;
	
	// Use this for initialization
	void Start () {
		GameObject newCategory;
		
		for(int i = 0; i < numberOfCategories; ++i) {
			newCategory = (GameObject)Instantiate(category);
			int j = 0;
			while(newCategory.transform.childCount > 0) {
				Transform child = newCategory.transform.GetChild (0);
				child.parent = this.transform;
				//child.name = (i+1).ToString () + child.name;
				child.name = indexOfNPC.ToString() + (i+1).ToString () + child.name;
				if(j % 2 == 1) {
					// prepare expandable sub-menu
					child.gameObject.GetComponent<AnalyticsPopulateTable>().type = i + 1;
					child.gameObject.GetComponent<AnalyticsPopulateTable>().indexOfNPC = indexOfNPC;
					child.gameObject.GetComponent<AnalyticsPopulateTable>().Populate();
					
					// disable expandable section
					Vector3 temp_vector = child.position;
					child.gameObject.SetActive (false);
					child.localPosition = temp_vector;
				}
				++j;
			}
			Destroy(newCategory);
		}
		table = this.GetComponent<UITable>();
		
		// fix Z-position (bring in front) -- don't know cause right now
		table.transform.localPosition = new Vector3(table.transform.localPosition.x, table.transform.localPosition.y, -99);
		table.repositionNow = true;
						
		// Problem
		PopulateCategory(1,"Emotion");
		PopulateCategory(2,"Comprehension");
		PopulateCategory(3,"Initiate Conversation");
		PopulateCategory(4,"Maintain Conversation");
		PopulateCategory(5,"Problem Solving");
		
		this.gameObject.SetActive (false);
	}
		
	public void PopulateCategory(int counter, string text)
	{
		List<DBReplyTimeWithType> timeTaken = MainDatabase.Instance.getTimeTaken(counter, AnalyticsController.Instance.npc_interactions[indexOfNPC].InteractionID);
		CategoryTop top = GameObject.Find (indexOfNPC.ToString() + counter + "A Top").GetComponent<CategoryTop>();
		top.category_name.text = text;
		if(timeTaken.Count != 0) {
			top.last_percentage.text = ((int)AnalyticsController.Instance.todayPercentages[indexOfNPC][counter-1]).ToString();
			top.today_percentage.text  = ((int)AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][counter-1]).ToString();
		} else {
			top.gameObject.SetActive(false);
		}
		
		DragNPCDataCamera dragCamera = top.colliderTranfsorm.gameObject.AddComponent("DragNPCDataCamera") as DragNPCDataCamera;
		dragCamera.draggableCamera = AnalyticsController.Instance.npcCamera;
	}
}
