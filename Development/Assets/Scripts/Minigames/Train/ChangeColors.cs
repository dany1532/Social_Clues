using UnityEngine;
using System.Collections;

public class ChangeColors : MonoBehaviour {

	GameObject targetCar;
	public GameObject goButton;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeCar(GameObject target)
	{
		targetCar = target;
		//gameObject.GetComponent<UISprite>().spriteName = target.name;
	}

	void OnClick()
	{
		Color color = gameObject.GetComponent<UISprite> ().color;
		targetCar.GetComponent<UISprite>().color = color;
		goButton.GetComponent<MoveLeft>().carUpdated();
		SelectedTrain.instance.SetColor (targetCar.name, color);
	}
}
