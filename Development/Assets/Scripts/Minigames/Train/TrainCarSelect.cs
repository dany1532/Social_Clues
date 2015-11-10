using UnityEngine;
using System.Collections;

public class TrainCarSelect : MonoBehaviour {

	public GameObject myUI;
	public GameObject arrow;
	public GameObject[] colorChoices;

	// Use this for initialization
	void Start () {
		if(myUI !=null)
			myUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		if(!myUI.activeInHierarchy)
			myUI.SetActive(true);

		arrow.transform.position = new Vector3(transform.position.x,arrow.transform.position.y,arrow.transform.position.z);
		foreach(GameObject color in colorChoices)
		{
			color.GetComponent<ChangeColors>().changeCar(gameObject);
		}
	}
}
