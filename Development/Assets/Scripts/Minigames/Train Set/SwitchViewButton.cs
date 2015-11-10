using UnityEngine;
using System.Collections;

public class SwitchViewButton : MonoBehaviour {

	public GameObject frontCam;
	public GameObject sideCam; 
	public GameObject trainCarts;
	public GameObject engine;
	//public bool clicked; 

	// Use this for initialization
	void Start() {
		engine.SetActive(true);
		sideCam.SetActive(false); 
		trainCarts.SetActive(false);
	}
	
	// Update is called once per frame
	/*void Update () {
		Debug.Log ("clicked: " + clicked);
	}*/

	void OnClick(){
		Debug.Log ("clicked"); 
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

	/*void OnPress () {
		//clicked = true; 
	}

	void OnClick(){
		clicked = true; 
		StartCoroutine("afterClick"); 
		/*if (clicked == true)
		{
			clicked = false; 
		}
		else{
			clicked = true;
		}
	}

	IEnumerator afterClick(){
		yield return new WaitForSeconds (0.001f); 
		clicked = false; 
	}*/
}
