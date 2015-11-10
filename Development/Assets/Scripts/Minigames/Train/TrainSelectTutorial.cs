using UnityEngine;
using System.Collections;

public class TrainSelectTutorial : MinigameManager {
	
	AudioManager manager;
	public AudioClip[] tutorial;
	/* 0: This is your train set!
	 * 1: Here, you can build your train using the train cars you find in story mode.
	 * 2: To build your train, tap on a train car on the tracks.
	 * 3: In the color menu, tap on what color you want that train car to be.
	 * 4: Keep playing story mode to unlock more colors for your train cars!
	 * 5: When you're done, click the button here to ride your train!
	 * */
	
	TutorialHand hand;
	public GameObject colorMenu, engine, tutorialOptions, trainCars;
	public Color redColor;
	Color engineColor;

	
	// Use this for initialization
	void Start () 
	{
		manager = gameObject.GetComponent<AudioManager>();
		hand = gameObject.GetComponentInChildren<TutorialHand>();
		//StartCoroutine("runTutorial");
		engineColor = engine.GetComponent<UISprite>().color;

		foreach(BoxCollider car in trainCars.GetComponentsInChildren<BoxCollider>())
		{
			car.enabled = false;
		}
		
	}

	public override void yesTutorial()
	{
		Destroy(tutorialOptions);
		StartCoroutine("runTutorial");
	}
	
	public override void noTutorial()
	{
		Destroy(tutorialOptions);
		foreach(BoxCollider car in trainCars.GetComponentsInChildren<BoxCollider>())
		{
			car.enabled = true;
		}
	}
	
	IEnumerator runTutorial()
	{
		//set runTutorial to true for the 2nd train scene as well
		GameObject.Find ("SelectedTrain").GetComponent<SelectedTrain>().showTutorial = true;

		//Dialogue #0
		manager.Play(tutorial[0],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[0].length);

		//Dialogue #1
		manager.Play(tutorial[1],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[1].length);

		//Dialogue #2
		manager.Play(tutorial[2],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[2].length);

		//Tutorial Hand appears and moves to the train.
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);

		//Hand taps on the engine.
		hand.isPointing(true);

		//Arrow points to the engine car.
		colorMenu.SetActive(true);

		//Dialogue #3
		manager.Play(tutorial[3],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[3].length);
		hand.isPointing(false);

		//Tutorial Hand moves to color menu and taps on red.
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);

		//Engine changes to red.
		engine.GetComponent<UISprite>().color = redColor;

		//Dialogue #4
		manager.Play(tutorial[4],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[4].length);

		//Dialogue #5
		manager.Play(tutorial[5],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[5].length);

		//Tutorial hand moves to start button
		hand.isPointing(false);
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);

		//Tutorial hand disappears

		yield return new WaitForSeconds(hand.moveInterval);
		hand.gameObject.SetActive(false);

		engine.GetComponent<UISprite>().color = engineColor;
		colorMenu.SetActive(false);

		//Allow player to customize their train
		foreach(BoxCollider car in trainCars.GetComponentsInChildren<BoxCollider>())
		{
			car.enabled = true;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
