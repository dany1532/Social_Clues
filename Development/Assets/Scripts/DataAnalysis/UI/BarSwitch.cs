using UnityEngine;
using System.Collections;

public class BarSwitch : MonoBehaviour {
	public enum GraphType {
		TODAY = 0,
		LAST_PLAY = 1,
		AGGREGATE = 2
	}

	public GraphType graphType;

	public GameObject barGraphController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		barGraphController.SendMessage("switchLabelDisplay", graphType, SendMessageOptions.DontRequireReceiver);
	}
}
