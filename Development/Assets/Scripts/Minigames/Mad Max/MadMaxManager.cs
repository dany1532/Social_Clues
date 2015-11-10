using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MadMaxManager : MinigameManager {

	int phase;
	/* Phase 0 : Instructions
	 * Phase 1 : Levels
	 * Phase 2 : End
	 * * * * * * * * * * * * */
	public int currLevel; 
	public bool incrementLevel;
	
	bool cont;
	
	public int numLevels;

	public GameObject[] myLevels;
	public GameObject[] myAnswers;
	public GameObject[] crosses;
	public GameObject notepad;
	public GameObject checkmark;
	GameObject tutorialHand;
	GameObject tutorialObject;

	public NPCDialogueAnimation animationController;
	public NPCAnimations animationSet;

	public GameObject tutorialOptions;
	
	// Associated minigame
	public Minigame minigame;
	
	public Dialogue snack;
	public Dialogue sport;
	public Dialogue shirt;
	public Dialogue juice;
	public Dialogue tutorial;
	public Dialogue begin;
	public Dialogue end;
	public Dialogue MaxSnack;
	public Dialogue MaxSport;
	public Dialogue MaxShirt;
	public Dialogue MaxJuice;
	public Dialogue SnackThanks;
	public Dialogue SportThanks;
	public Dialogue ShirtThanks;
	public Dialogue JuiceThanks;
	public Dialogue ClearDialogue;
	
	public AudioClip greatJob;
	
	public Texture waiting;
	public Texture correct;
	public Texture finish;
	public Texture talking;
	
	public UITexture max;
	
	public bool showTutorial;
	
	public AudioClip rightAnswerFX;
	public AudioClip wrongAnswerFX;
	
	// Use this for initialization
	void Start () {		
		phase = 0;
		//Debug.Log("Phase: " + phase);
		
		notepad.SetActive(false);
		cont = true;
		
		myLevels = new GameObject[numLevels];
		myAnswers = new GameObject[numLevels];
		//find first level, or MaxLevel0
		//myLevels[0] = GameObject.Find("MaxLevel0");
		//fill up myLevels with remaining
		//int i = 1;

		for (int i = 0 ; i < numLevels ; i++)//(GameObject.Find("MaxLevel" + i.ToString()) !=null)
		{
			myLevels[i] = GameObject.Find("MaxLevel" + i.ToString());
			foreach(Transform child in myLevels[i].transform)
			{
				child.gameObject.SetActive(false);
			}
			myAnswers[i] = GameObject.Find("Answer" + i.ToString());

			myAnswers[i].SetActive(false);
			
		}
		
		for(int i = 0; i<3; i++)
		{
			crosses[i].SetActive(false);
		}

		
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		
		tutorialHand = GameObject.Find ("TutorialHand");
		tutorialHand.SetActive(false);
		
		tutorialObject = GameObject.Find("TutorialObject");
		tutorialObject.SetActive(false);
		
		notepad.SetActive(true);
		checkmark.SetActive(false);
		phase = 1;
		currLevel = 0;

		PlayAnimation (NPCAnimations.AnimationIndex.NEUTRAL);

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
			// Show the instructions of the minigame
			if (minigame != null && minigame.StartConversation() == false)
			{
				Debug.Log("S: Let's help Max make new choices based on what he already likes. Max will give you a description for each category, you choose something similar.");
			}
			
			Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
			
			//StartCoroutine(WaitForInput("startGame"));
			//Invoke ("startGame", minigame.conversationTree.root.GetCommulativeDuration() + 1.5f);
		}

	}

	public override void yesTutorial()
	{
		showTutorial = true;
		Destroy(tutorialOptions);
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: Let's help Max make new choices based on what he already likes. Max will give you a description for each category, you choose something similar.");
		}
		
		Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		
		//StartCoroutine(WaitForInput("startGame"));
		//Invoke ("startGame", minigame.conversationTree.root.GetCommulativeDuration() + 1.5f);
	}

	public override void noTutorial()
	{
		showTutorial = false;
		Destroy(tutorialOptions);
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: Let's help Max make new choices based on what he already likes. Max will give you a description for each category, you choose something similar.");
		}
		
		//Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		
		//StartCoroutine(WaitForInput("startGame"));
		Invoke ("startGame", minigame.GetCurrentDialogueDuration());
	}
	
	void continueDialogue()
	{
		if(!minigame.ContinueConversation())
		{
			if(showTutorial) 
				StartCoroutine("startTutorial");
			else
				Invoke ("startGame", minigame.GetCurrentDialogueComulativeDuration()); 
				//Invoke ("startGame", 1);
		}
		else
		{
			Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
			//StartCoroutine(WaitForInput("continueDialogue"));
		}
		
	}
	
	/// <summary>
	/// Start coroutine that waits for input
	/// </summary>
	/// <param name='method'>
	/// The method to be invoked once the player has clicked somewhere
	/// </param>
	public void Wait(string method)
	{
		StartCoroutine(WaitForInput(method));
	}
	/// <summary>
	/// Waits until the user clicks the mouse, then calls the function method (passed in)
	/// </summary>
	/// <param name='method'>
	/// The method to be invoked once the player has clicked somewhere
	/// </param>
	IEnumerator WaitForInput(string method)
	{
		while(!InputManager.Instance.HasReceivedClick())
		{
			yield return 0;
		}
		Invoke (method, 0);
	}
		
	/* Method startGame
	 * 
	 * Moves from Phase 0 to Phase 1. Starts the game.
	 * 
	 * */
	void startGame()
	{
		Sherlock.Instance.PlaySequenceInstructions (begin, StartFirstRound);

	}

	void StartFirstRound()
	{
		PlayAnimation (NPCAnimations.AnimationIndex.TALKING);
		Respond(MaxSnack);
		Invoke("sherlockInstruction", minigame.GetCurrentDialogueComulativeDuration());
		myAnswers[currLevel].SetActive(true);
	}

	IEnumerator startTutorial()
	{
		Sherlock.Instance.PlaySequenceInstructions (tutorial, null);

		yield return new WaitForSeconds(tutorial.voiceOver.length + 0.5f);

		tutorialObject.SetActive(true);
		
		yield return new WaitForSeconds(4.0f);
		
		tutorialHand.SetActive(true);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();

		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		
		tutorialHand.GetComponent<TutorialHand>().isPointing(true);
		tutorialObject.transform.parent = tutorialHand.transform;
		
		yield return new WaitForSeconds(2.5f);
		
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);

		yield return new WaitForSeconds(2.5f);

		tutorialHand.SetActive(false);

		Invoke ("startGame", 0.5f);
	}
	
	void sherlockInstruction()
	{
		PlayAnimation (NPCAnimations.AnimationIndex.WAITING);
		
		switch(currLevel)
		{
		case 0:
				Respond(snack);
				StartCoroutine("showLevel");
				break;
		case 1:
				Respond(sport);
				StartCoroutine("showLevel");
				break;
		case 2:
				Respond(shirt);
				StartCoroutine("showLevel");
				break;
		case 3:
				Respond(juice);
				StartCoroutine("showLevel");
				break;
		}
		
	}
	
	IEnumerator showLevel()
	{
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration()-2f);
		PlayAnimation (NPCAnimations.AnimationIndex.WAITING);
		
		SortedList<string, Transform> children = new SortedList<string, Transform>();
		foreach(Transform child in myLevels[currLevel].transform)
		{
			children.Add(child.name, child);
			if(child.GetComponent<DraggableObject>() != null)
				child.GetComponent<DraggableObject>().setStartPos();
		}
		
		foreach(Transform child in children.Values)
		{
			child.gameObject.SetActive(true);
			yield return new WaitForSeconds(1);
		}
		
	}

	void nextLevel()
	{
		PlayAnimation(NPCAnimations.AnimationIndex.NEUTRAL);
		Invoke ("startNextLevel", 1);
	}

	void startNextLevel()
	{
		myLevels[currLevel].SetActive(false);
		myAnswers[currLevel].SetActive(false);
		foreach (GameObject cross in crosses)
			cross.SetActive(false);
		checkmark.SetActive(false);
		
		currLevel++;
		
		if(currLevel < numLevels)
		{
			PlayAnimation(NPCAnimations.AnimationIndex.TALKING);
		
			switch(currLevel)
			{
			case 1:
					Respond(MaxSport);
					break;
			case 2:
					Respond(MaxShirt);
					break;
			case 3:
					Respond(MaxJuice);
					break;
			}
			myAnswers[currLevel].SetActive(true);
		
			Invoke("sherlockInstruction", minigame.GetCurrentDialogueDuration());
		}
		else
		{
			Debug.Log("Return to Cafeteria");
		}
		
	}
	
	void maxThanks()
	{
		PlayAnimation (NPCAnimations.AnimationIndex.TALKING);

		switch(currLevel)
		{
		case 0:
				Respond(SnackThanks);
				Invoke("nextLevel", minigame.GetCurrentDialogueDuration());
				break;
		case 1:
				Respond(SportThanks);
				Invoke("nextLevel", minigame.GetCurrentDialogueDuration());
				break;
		case 2:
				Respond(ShirtThanks);
				Invoke("nextLevel", minigame.GetCurrentDialogueDuration());
				break;
		case 3:
				Respond(JuiceThanks);
				Invoke("EndMinigame1", minigame.GetCurrentDialogueDuration());
				break;
		}
	}
	
	void EndMinigame1()
	{
		PlayAnimation (NPCAnimations.AnimationIndex.CELEBRATING);
		Sherlock.Instance.PlaySequenceInstructions (end, CompleteMinigame);
	}

	void CompleteMinigame()
	{
		minigame.CompleteMinigame ();
	}

	void PlayAnimation(NPCAnimations.AnimationIndex index)
	{
		NPCAnimations.AnimationSequence anim = animationSet.RetrieveAnimationSequence(index);
		List<Texture> textures = anim.textures;
		if (textures.Count > 0)
		{
			animationController.StopAnimation();
			animationController.SetAnimationList(textures);
			animationController.PlayAnimation();
			animationController.SetSpeed(anim.speed);
		}
	}
	
	/*
	void EndMinigame2()
	{
		AudioManager.Instance.PlayVoiceOver(greatJob,1);
		minigame.Invoke("CompleteMinigame", greatJob.length);
	}
	*/
	
	public void Respond (Dialogue response)
	{
		if (minigame != null && minigame.ContinueConversation(response) == false)
		{
			Debug.Log(response.text);
		}
	}

	public void UpdateCorrectSprite ()
	{
		Respond(ClearDialogue);
		PlayAnimation (NPCAnimations.AnimationIndex.RIGHT_CHOICE);
		maxThanks();
	}
}
