using UnityEngine;
using System.Collections;

public class TrainCameraToggle : MonoBehaviour {

	public GameObject frontCam;
	public GameObject sideCam; 
	public GameObject trainCarts;
	public GameObject engine;
	public GameObject switchViewButton; 

	bool clickedRef; 

	// Use this for initialization
	void Start () {
		engine.SetActive(true);
		sideCam.SetActive(false); 
		trainCarts.SetActive(false); 
	}

	/*bool clicked()
	{
		clickedRef = switchViewButton.GetComponent<SwitchViewButton>().clicked; 
		return clickedRef;
	}*/
	
	// Update is called once per frame
	void Update () {

		Debug.Log ("ClickedRef: " + clickedRef); 

		//if (clicked() == true)//(clickedRef == true)//(InputManager.Instance.HasReceivedClick())
		{
			if (sideCam.gameObject.activeSelf == false)
			{
				Debug.Log ("sideCam null");
				engine.SetActive(false);
				frontCam.SetActive(false);
				sideCam.SetActive(true); 
				trainCarts.SetActive(true);
				//sideCam.gameObject.tag = "MainCamera"; 
			}
			else
			{
				frontCam.SetActive(true);
				engine.SetActive(true);
				sideCam.SetActive(false);
				trainCarts.SetActive(false);
				//frontCam.gameObject.tag = "MainCamera"; 

			}
		}

	
	}
}
