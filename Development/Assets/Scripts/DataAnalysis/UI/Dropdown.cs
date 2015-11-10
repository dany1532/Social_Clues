using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class Dropdown : MonoBehaviour {
	public NPCDetailsController detailsController;
	public int index;
	private bool expanded;
	public GameObject expandableAnswers;
	public int indexOfNPC;
	
	// Use this for initialization
	void Start () {
		contract (false);
		expanded = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnClick() {
		if(!expanded) {
			expand ();
		} else {
			contract ();
		}
	}
	
	void expand() {
		//detailsController.updateAnchors(index, true);
		detailsController.updatePositions(index, true);
		expandableAnswers.SetActive (true);
		expanded = true;

		++AnalyticsController.Instance.expansions[indexOfNPC];
		if(AnalyticsController.Instance.expansions[indexOfNPC] > AnalyticsController.Instance.maxExpansions) {
			++AnalyticsController.Instance.maxExpansions;
			AnalyticsController.Instance.expandBackground(210);
		}
	}
	
	public void contract(bool updateTable = true) {
		//detailsController.updateAnchors(index, false);
		if (updateTable) {
			detailsController.updatePositions(index, false);
		}
		expandableAnswers.SetActive (false);
		expanded = false;

		if(updateTable) {
			--AnalyticsController.Instance.expansions[indexOfNPC];
			// if number of expansions was a maximum
			if(AnalyticsController.Instance.expansions[indexOfNPC] == (AnalyticsController.Instance.maxExpansions - 1)) {
				bool needToContract = true;
				for(int i = 0; i < AnalyticsController.Instance.expansions.Count; ++i) {
					if(i != indexOfNPC) {
						if(AnalyticsController.Instance.expansions[i] == AnalyticsController.Instance.maxExpansions) {
							// there was a maximum other than the current one...
							needToContract = false;
							break;
						}
					}
				}

				if(needToContract) {
					--AnalyticsController.Instance.maxExpansions;
					AnalyticsController.Instance.contractBackground(210, false);
				}
			}
		}
	}
}
