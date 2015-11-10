using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectedTrain : MonoBehaviour {
	[System.SerializableAttribute]
	public class TrainColor{
		public string trainType;
		public Color color;
	}

	public List<TrainColor> trainWagons;

	//whether the user decided to view the tutorial, passed on to the second half of the train minigame
	public bool showTutorial; 

	public static SelectedTrain instance;

	// Use this for initialization
	void Start () {
		showTutorial = false;
		instance = this;
		DontDestroyOnLoad (gameObject);	
	}

	public void SetColor (string wagonName, Color color)
	{
		foreach (TrainColor trainWagon in trainWagons) {
			if (trainWagon.trainType == wagonName)
			{
				trainWagon.color = color;
				break;
			}
		}
	}
}
