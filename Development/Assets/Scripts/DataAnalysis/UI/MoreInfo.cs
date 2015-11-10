using UnityEngine;
using System.Collections;

public class MoreInfo : MonoBehaviour {
	private bool expanded;
	public UISprite sprite;
	public UILabel label;
	public UIStretch spriteStretch;
	public UIAnchor spriteAnchor;
	public GameObject mainCamera;
	public NPCDataCamera dataCamera;
	
	// Use this for initialization
	void Start () {
		expanded = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnClick() {
		//Debug.Log ("test");
		if(expanded) {
			sprite.transform.localEulerAngles = Vector3.zero;
			sprite.spriteName = "More Info Tab";
			label.gameObject.SetActive (true);
			spriteStretch.relativeSize.x = 0.15f;
			spriteAnchor.relativeOffset.x = 0.096f;
			mainCamera.transform.localPosition = Vector3.zero;
			AnalyticsController.Instance.setNPCDetails(false);
			expanded = false;
			AnalyticsController.Instance.contractBackground(690, true);
		} else {
			sprite.transform.localEulerAngles = new Vector3(0,0,180);
			sprite.spriteName = "More Info";
			label.gameObject.SetActive (false);
			spriteStretch.relativeSize.x = 0.05f;
			spriteAnchor.relativeOffset.x = 0.0452f;
			
			AnalyticsController.Instance.setNPCDetails(true);
			// move main camera
			//Debug.Log ("GUICamera local position:              " + mainCamera.transform.localPosition);
			dataCamera.Reset();
			mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, -317f, mainCamera.transform.localPosition.z);
			//Debug.Log ("GUICamera local position after update: " + mainCamera.transform.localPosition);
			expanded = true;
			AnalyticsController.Instance.expandBackground(320);
		}
	}
}
