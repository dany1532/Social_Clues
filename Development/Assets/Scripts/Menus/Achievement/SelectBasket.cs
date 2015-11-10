using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectBasket : MonoBehaviour {

	public List<GameObject> deactivateBasket;
	public GameObject nextBasket;

	// Use this for initialization
	void OnClick()
	{
		foreach( GameObject go in deactivateBasket)
			go.SetActive(false);
		nextBasket.SetActive(true);
	}
}
