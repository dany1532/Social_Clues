using UnityEngine;
using System.Collections;

public class TrainSetTutorial : MinigameManager {
	
	AudioManager manager;
	TrainManager gameManager;
	public AudioClip[] tutorial;
	/* 0: Now we're riding the train!
	 * 1: Tap on things to interact with them.
	 * 2: Click this button to watch the train from a different view.
	 * 3: Find more toys in story mode to unlock new things to see while riding the train.
	 * 4: Have fun!
	 * */
	
	TutorialHand hand;
	GameObject tutorialCow;

	
	// Use this for initialization
	void Start () 
	{
		manager = gameObject.GetComponent<AudioManager>();
		gameManager = GameObject.Find("TrainManager").GetComponent<TrainManager>();
		hand = gameObject.GetComponentInChildren<TutorialHand>();
		tutorialCow = GameObject.Find("TutorialCow");

		tutorialCow.SetActive(false);

		if(GameObject.Find("SelectedTrain") != null)
		{
			SelectedTrain selectedTrain = GameObject.Find("SelectedTrain").GetComponent<SelectedTrain>();

			if (selectedTrain.showTutorial)
			{
				//Debug.Log("SHOWING TUTORIAL");
				StartCoroutine("runTutorial");	
			}
			else
			{
				//Debug.Log("NOT SHOWING TUTORIAL");
				gameManager.startTrain();
				Destroy(this.gameObject);
			}
		}
		else
		{
			//Debug.Log("NOT SHOWING TUTORIAL - selected train not found");
			gameManager.startTrain();
			Destroy(this.gameObject);
		}

	}
	/* NOTE: Right now I don't start the train creating new clickable objects until the tutorial is over. But the objects still come 
	 * towards the screen, regardless. Need some sort of fix. The game should STOP and wait for the tutorial to finish.*/
	
	IEnumerator runTutorial()
	{
		GameObject cameraSwitch = GameObject.Find ("OtherViewButton");
		cameraSwitch.GetComponent<BoxCollider>().enabled = false;
		//Dialogue #0
		manager.Play(tutorial[0],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[0].length);

		//Cow shows up to the side of the train
		tutorialCow.SetActive(true);

		//Dialogue #1
		manager.Play(tutorial[1],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[1].length);

		//Tutorial Hand moves to cow, taps, and cow moo's
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);
		tutorialCow.GetComponentInChildren<AudioSource>().Play();
		yield return new WaitForSeconds(2.0f);

		//Dialogue #2
		manager.Play(tutorial[2],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[2].length);

		//Tutorial hand moves to camera switch button
		hand.isPointing(false);
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);
		yield return new WaitForSeconds(1.5f);

		//Tutorial hand disappears
		hand.gameObject.SetActive(false);
		tutorialCow.SetActive(false);

		//Dialogue #3
		manager.Play(tutorial[3],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[3].length);

		//Dialogue #4
		manager.Play(tutorial[4],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[4].length);

		//Allow player to continue playing

		cameraSwitch.GetComponent<BoxCollider>().enabled = true;
		gameManager.startTrain();
		Destroy(this.gameObject);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
