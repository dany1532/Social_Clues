using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCDetailsController : MonoBehaviour {
	public int indexOfNPC;
	public List<UIAnchor> anchorsToUpdate1 = new List<UIAnchor>();
	public List<UIAnchor> anchorsToUpdate2 = new List<UIAnchor>();
	public List<UIAnchor> anchorsToUpdate3 = new List<UIAnchor>();
	public List<UIAnchor> anchorsToUpdate4 = new List<UIAnchor>();
	public List<UIAnchor> anchorsToUpdate5 = new List<UIAnchor>();
	
	public List<List<UIAnchor>> anchorsToUpdate = new List<List<UIAnchor>>();
	
	public List<ExpandableAnswers> answers = new List<ExpandableAnswers>();
	
	public UIAnchor graphAnchor;
	
	public GameObject createRows;
	
	public List<GameObject> objectsToUpdate = new List<GameObject>();
	public GameObject graph;
	public GameObject background;
	
	// Use this for initialization
	void Start () {
		anchorsToUpdate.Add (anchorsToUpdate1);
		anchorsToUpdate.Add (anchorsToUpdate2);
		anchorsToUpdate.Add (anchorsToUpdate3);
		anchorsToUpdate.Add (anchorsToUpdate4);
		anchorsToUpdate.Add (anchorsToUpdate5);
				
		createRows.GetComponent<CreateRows>().indexOfNPC = indexOfNPC;
		
		/*
		for(int i = 0; i < answers.Count; ++i) {
			answers[i].indexOfNPC = indexOfNPC;
			answers[i].type = i + 1;
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void updatePositions(int index, bool expand) {
		float offset = 207;
		for(int i = 0; i < objectsToUpdate.Count; ++i) {
			if(i > index) {
				if(expand) {
					objectsToUpdate[i].transform.localPosition = new Vector3(objectsToUpdate[i].transform.localPosition.x, objectsToUpdate[i].transform.localPosition.y - offset, objectsToUpdate[i].transform.localPosition.z);
				} else {
					objectsToUpdate[i].transform.localPosition = new Vector3(objectsToUpdate[i].transform.localPosition.x, objectsToUpdate[i].transform.localPosition.y + offset, objectsToUpdate[i].transform.localPosition.z);
				}
			}
		}
		
		if(expand) {
			graph.transform.localPosition = new Vector3(graph.transform.localPosition.x, graph.transform.localPosition.y - offset, graph.transform.localPosition.z);
			background.transform.localScale = new Vector3(background.transform.localScale.x, background.transform.localScale.y + offset, background.transform.localScale.z);
		} else {
			graph.transform.localPosition = new Vector3(graph.transform.localPosition.x, graph.transform.localPosition.y + offset, graph.transform.localPosition.z);
			background.transform.localScale = new Vector3(background.transform.localScale.x, background.transform.localScale.y - offset, background.transform.localScale.z);
		}		
	}
	
	/*
	public void updateAnchors(int index, bool expand) {
		for(int i = 0; i < anchorsToUpdate.Count; ++i) {
			for(int j = 0; j < anchorsToUpdate[i].Count; ++j) {
				if(i > index) {
					if(expand) {
						anchorsToUpdate[i][j].relativeOffset.y -= 0.59f;
					} else {
						anchorsToUpdate[i][j].relativeOffset.y += 0.59f;
					}
				}
			}
		}
		
		if(expand) {
			graphAnchor.relativeOffset.y -= 0.59f;
		} else {
			graphAnchor.relativeOffset.y += 0.59f;
		}
	}
	*/	
}
