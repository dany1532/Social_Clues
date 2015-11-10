using UnityEngine;
using System.Collections;
using System; 
using System.Collections.Generic;


public class EddiePuzzleManager : MinigameManager {
	
	public int correctCount = 0;
	public EddiePuzzleManager Instance{get; private set;}
	
	public enum eType
	{
		Invalid_Type = -1, 
		LaughManager,
		HelpManager, 
	}
	
	public enum DialogueType
	{
		HELPRIGHT, HELPWRONG, LAUGHRIGHT, LAUGHWRONG, WANTHINT, HINT, CORRECT, CHOOSESOL, MISTAKE, END, START
	}
	
	public int wrongTries;
	
	eType currentType = eType.Invalid_Type;
	
		
	int phase;
	/* Phase 0 : Instructions
	 * Phase 1 : Levels
	 * Phase 2 : End
	 * * * * * * * * * * * * */
	public int currLevel; 
	public bool incrementLevel;
	public bool puzzleComplete; 
	public bool hintAsked;
	public bool hintGiven; 
	public GameObject buttons; 
	public GameObject hintButton; 
	GameObject tutorialHand;
	GameObject tutorialSlot;
	GameObject tutorialObject;
	
	
	bool cont;
	
	public int correctSlotRef {get; set;} 
	
	public int numLights;
	public int numPuzzles;
	public int prevPuzNum; 
	int totalPieces;
	
	public Dialogue helpRight;
	public Dialogue helpWrong;
	public Dialogue laughRight;
	public Dialogue laughWrong;
	public Dialogue hint;
	public Dialogue wantHint;
	public Dialogue correct;
	public Dialogue mistake;
	public Dialogue end;
	public Dialogue start;
	public Dialogue chooseSol;
	public Dialogue tutorial1;
	public Dialogue tutorial2;
	public Dialogue tutorial3;
	
	// Associated minigame
	Minigame minigame;

	public GameObject[] myPuzzles;
	public PuzzleImageType[] mySolutions;
	GameObject[] puzzlePieces; 
	GameObject[] highlightControl;

	public bool hintRef; 
	bool[] puzzlePressed; 
	bool[] imageType;
	public bool showTutorial;
	public GameObject tutorialOptions;
	
	public Texture easyOutline;
	public Texture medOutline;
	public Texture hardOutline;
	public Texture2D easyColored;
	public Texture2D medColored;
	public Texture2D hardColored;
	
	public AudioClip rightAnswerFX;
	public AudioClip wrongAnswerFX;

	public float lastCorrectChoice = 0;
	public float minCorrectChoiceThreshold = 100;

	// Use this for initialization
	void Start () {
		wrongTries = 0;
		Instance = this;
		
		Sherlock.Instance.SetBubblePosition (Sherlock.side.DOWN);
		
		correctSlotRef = -1; 

		phase = 0;

		cont = false;
		puzzleComplete = false; 
		hintGiven = false; 
		hintAsked = false;
		
		//shuffle the puzzles
		System.Random rng = new System.Random();  
    	int n = myPuzzles.Length;  
    	while (n > 1) {  
	        n--;  
	        int k = rng.Next(n + 1);  
	        GameObject value = myPuzzles[k];
			PuzzleImageType sol = mySolutions[k];
	        myPuzzles[k] = myPuzzles[n];  
			mySolutions[k] = mySolutions[n];
	        myPuzzles[n] = value;
			mySolutions[n]  = sol;
	    } 
		mySolutions[0].SetActive(false);
		
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		
		switch (minigame.difficulty)
		{
		case MinigameDifficulty.Difficulty.EASY:
			myPuzzles[0].GetComponent<UITexture>().mainTexture = easyOutline;
			myPuzzles[0].GetComponent<PuzzlePieceTextureMod>().puzzleOutline = easyColored;
			totalPieces = 6;
			break;
		case MinigameDifficulty.Difficulty.MEDIUM:
			myPuzzles[0].GetComponent<UITexture>().mainTexture = medOutline;
			myPuzzles[0].GetComponent<PuzzlePieceTextureMod>().puzzleOutline = medColored;
			totalPieces = 12;
			break;
		case MinigameDifficulty.Difficulty.HARD:
			myPuzzles[0].GetComponent<UITexture>().mainTexture = hardOutline;
			myPuzzles[0].GetComponent<PuzzlePieceTextureMod>().puzzleOutline = hardColored;
			totalPieces = 20;
			break;	
		}
	
		for(int j = 1; j<myPuzzles.Length; j++)
		{
			myPuzzles[j].SetActive(false);
			mySolutions[j].SetActive(false);
			
			switch (minigame.difficulty)
			{
			case MinigameDifficulty.Difficulty.EASY:
				myPuzzles[j].GetComponent<UITexture>().mainTexture = easyOutline;
				myPuzzles[j].GetComponent<PuzzlePieceTextureMod>().puzzleOutline = easyColored;
				break;
			case MinigameDifficulty.Difficulty.MEDIUM:
				myPuzzles[j].GetComponent<UITexture>().mainTexture = medOutline;
				myPuzzles[j].GetComponent<PuzzlePieceTextureMod>().puzzleOutline = medColored;
				break;
			case MinigameDifficulty.Difficulty.HARD:
				myPuzzles[j].GetComponent<UITexture>().mainTexture = hardOutline;
				myPuzzles[j].GetComponent<PuzzlePieceTextureMod>().puzzleOutline = hardColored;
				break;	
			}
		}

		buttons.SetActive(false);
		hintButton.SetActive(false);
		
		tutorialHand = GameObject.Find("TutorialHand");
		tutorialHand.SetActive(false);
		tutorialObject = GameObject.Find("PuzzlePiece");
		tutorialObject.SetActive(false);
		tutorialSlot = GameObject.Find("PuzzleSlot");
		tutorialSlot.SetActive(false);
		
		// Show the instructions of the minigame
		/*if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: Knowing when to laugh is puzzling. Complete each puzzle and choose whether you should laugh, or you should help, the people in the image.");
		}
		*/
		//minigame.StartConversation();

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
			Sherlock.Instance.PlaySequenceInstructions(minigame.conversationTree.root, null);
			if(showTutorial) 
				StartCoroutine("startTutorial");
			else 
				Invoke ("startGame", minigame.conversationTree.root.GetCommulativeDuration() + 1.5f);
		}

	}

	public override void yesTutorial ()
	{
		Destroy(tutorialOptions);
		Sherlock.Instance.PlaySequenceInstructions(minigame.conversationTree.root, null);
		StartCoroutine("startTutorial");
	
	}

	public override void noTutorial ()
	{
		Destroy(tutorialOptions);
		Sherlock.Instance.PlaySequenceInstructions(minigame.conversationTree.root, null);
		Invoke ("startGame", minigame.conversationTree.root.GetCommulativeDuration() + 1.5f);
	}
	
	public void ShowSolution()
	{
		buttons.SetActive(true);
		hintButton.SetActive(false);
		
		GameObject.Find("HelpButton").GetComponent<BoxCollider>().enabled = true;
		GameObject.Find("HelpButton").GetComponentInChildren<UISprite>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		GameObject.Find("LaughButton").GetComponent<BoxCollider>().enabled = true;
		GameObject.Find("LaughButton").GetComponentInChildren<UISprite>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				
		if (ShowDialogue(DialogueType.CHOOSESOL) == false)
		{
			Debug.Log("Choose whether you want to laugh or help the people in the picture.");
		}
		
		Destroy(myPuzzles[currLevel]);
		myPuzzles[currLevel] = null;
		correctCount = 0;
	}
	
	public void hintPressed()
	{
		if (ShowDialogue(DialogueType.HINT) == false)
		{
			Debug.Log("Press on the puzzle piece you need help with to get a hint!");
		}
		hintAsked = true;
	}
	
	public void puzzleNumberPressed (int puzzleNumber)
	{
		correctSlotRef = puzzleNumber;
	}
		
	/* Method startGame
	 * 
	 * Moves from Phase 0 to Phase 1. Starts the game.
	 * 
	 * */
	void startGame()
	{
		phase = 1;
		currLevel = 0;
		
		ShowDialogue(DialogueType.START);
		myPuzzles[currLevel].SetActive(true);	
		hintButton.SetActive(true);		
	}
	
	IEnumerator startTutorial()
	{
		TutorialHand tutorialHandHelper = tutorialHand.GetComponent<TutorialHand>();
		yield return new WaitForSeconds(minigame.conversationTree.root.GetCommulativeDuration() + 2f);
		tutorialObject.SetActive(true);
		tutorialSlot.SetActive(true);
		hintButton.SetActive(true);
		hintButton.collider.enabled = false;
		
		Respond(tutorial1);
		yield return new WaitForSeconds(0.5f);
		
		tutorialHand.SetActive(true);
		tutorialHandHelper.moveInterval = 0.5f;
		tutorialHandHelper.nextWayPoint();
		
		yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
		tutorialHandHelper.moveInterval = 1;
		tutorialHandHelper.isPointing(true);
		tutorialObject.transform.parent = tutorialHand.transform;
		tutorialObject.transform.localScale = new Vector3(118f/tutorialHand.transform.localScale.x, 84f/tutorialHand.transform.localScale.y, 0);
		
		yield return new WaitForSeconds(1);
		
		tutorialHandHelper.nextWayPoint();
		yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
		
		yield return new WaitForSeconds(2.5f);
		tutorialHandHelper.isPointing(false);
		tutorialObject.transform.position = tutorialSlot.transform.position;
		tutorialObject.transform.parent = tutorialSlot.transform;
		
		yield return new WaitForSeconds(0.5f);
		Respond (tutorial3);
		tutorialHandHelper.nextWayPoint();
		yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
		tutorialHandHelper.isPointing(true);
		
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		tutorialHandHelper.isPointing(false);
		
		Respond (tutorial2);
		buttons.SetActive(true);		
		hintButton.SetActive(false);
		hintButton.collider.enabled = true;
		yield return new WaitForSeconds(1);
		
		tutorialHandHelper.nextWayPoint();
		yield return new WaitForSeconds(tutorialHandHelper.moveInterval+0.5f);
		tutorialHandHelper.nextWayPoint();
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		
		tutorialObject.SetActive(false);
		buttons.SetActive(false);
		tutorialSlot.SetActive(false);
		tutorialHand.SetActive(false);

		Invoke ("startGame", 0.5f);
	}
	
	void nextLevel()
	{
		buttons.SetActive(false);
		currLevel++;
		wrongTries = 0;
		correctCount = 0;
		
		if (currLevel > 0)
		{
			mySolutions[currLevel-1].SetActive(false);
			
			for(int x = 0; x<4; x++)
			{
				for(int y = 0; y<5; y++)
				{
					GameObject go = GameObject.Find("IndividualPiece "+x+y);
					if (go != null)
						Destroy(go);
				}
			}
			
			GameObject destroyGO = mySolutions[currLevel-1].gameObject;
			mySolutions[currLevel-1] = null;
			Destroy(destroyGO);
			Resources.UnloadUnusedAssets();
		}
		
		if(currLevel < numPuzzles)
		{
			
			myPuzzles[currLevel].SetActive(true); 
			hintButton.SetActive(true);

		}
		else
		{
			GameObject go = GameObject.Find("PuzzleSlots");
			Destroy (go);
			hintButton.SetActive(false);
			if (ShowDialogue(DialogueType.END) == false)
			{
				Debug.Log ("S: Great job! You helped Eddie learn when it is appropriate to laugh. ");
			}
			
			minigame.Invoke("CompleteIfStandalone", end.GetCommulativeDuration() + 0.5f);
		}
	}
	

		/// <summary>
	/// Display dialogue based on dialogue type
	/// </summary>
	/// <returns>
	/// True if the dialogue was showed
	/// </returns>
	/// <param name='dialogueType'>
	/// Type of dialogue
	/// </param>
	public bool ShowDialogue(DialogueType dialogueType)
	{
		// Display dialogue depending on dialogue type
		switch(dialogueType)
		{
			case DialogueType.HINT:
				return minigame.ContinueConversation(hint);
			case DialogueType.WANTHINT:
				return minigame.ContinueConversation(wantHint);
			case DialogueType.LAUGHRIGHT:
				Sherlock.Instance.PlaySequenceInstructions(laughRight, null);
				return true;
			case DialogueType.LAUGHWRONG:
				Sherlock.Instance.PlaySequenceInstructions(laughWrong, null);
				return true;
			case DialogueType.HELPRIGHT:
				Sherlock.Instance.PlaySequenceInstructions(helpRight, null);
				return true;
			case DialogueType.HELPWRONG:
				Sherlock.Instance.PlaySequenceInstructions(helpWrong, null);
				return true;
			case DialogueType.CORRECT:
				return minigame.ContinueConversation(correct);
			case DialogueType.MISTAKE:
				return minigame.ContinueConversation(mistake);
			case DialogueType.CHOOSESOL:
				return minigame.ContinueConversation(chooseSol);
			case DialogueType.END:
				Sherlock.Instance.PlaySequenceInstructions(end, null);
				return true;
			case DialogueType.START:
				return minigame.ContinueConversation(start);
			default:
				return false;
		}
		
		return false;
	}
	
	public void Respond (Dialogue response)
	{
		if (minigame != null && minigame.ContinueConversation(response) == false)
		{
			Debug.Log(response.text);
		}
	}


	
	public void OnButtonClickDown(eType buttonType)
	{
		//set the current type to be the laugh/help button that was pressed
		currentType = buttonType;
		
		if (mySolutions[currLevel].GetComponent<PuzzleImageType>().currentImageType == PuzzleImageType.LaughOrHelp.Laugh) //if the current level is laughable
		{
			if (currentType == eType.LaughManager)
			{
				AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
				
				GameObject.Find("HelpButton").GetComponent<BoxCollider>().enabled = false;
				GameObject.Find("LaughButton").GetComponent<BoxCollider>().enabled = false;
				ShowDialogue(DialogueType.LAUGHRIGHT);
				currentType = eType.Invalid_Type;
				Invoke("nextLevel",laughRight.GetCommulativeDuration());
			}
			else if (currentType == eType.HelpManager)
			{
				AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
				
				wrongTries++;
				ShowDialogue(DialogueType.HELPWRONG);
				if (wrongTries>=3)
				{
					GameObject.Find("HelpButton").GetComponent<BoxCollider>().enabled = false;
					GameObject.Find("HelpButton").GetComponentInChildren<UISprite>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
				}
			}
		}
		else
		{
			if (currentType == eType.LaughManager)
			{
				AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
				
				wrongTries++;
				ShowDialogue (DialogueType.LAUGHWRONG);
				if (wrongTries>=3)
				{
					GameObject.Find("LaughButton").GetComponent<BoxCollider>().enabled = false;
					GameObject.Find("LaughButton").GetComponentInChildren<UISprite>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
				}
			}
			else if (currentType == eType.HelpManager)
			{
				AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
				
				GameObject.Find("HelpButton").GetComponent<BoxCollider>().enabled = false;
				GameObject.Find("LaughButton").GetComponent<BoxCollider>().enabled = false;
				ShowDialogue (DialogueType.HELPRIGHT);
				currentType = eType.Invalid_Type;
				Invoke("nextLevel",helpRight.GetCommulativeDuration());
			}
			
		}
	}
	
	public char getDifficulty()
	{
		switch(minigame.difficulty)
		{
		case MinigameDifficulty.Difficulty.EASY:
			return 'E';
			break;
		case MinigameDifficulty.Difficulty.MEDIUM:
			return 'M';
			break;
		case MinigameDifficulty.Difficulty.HARD:
			return 'H';
			break;
		default:
			return 'E';
			break;
		}
	}
	
	public void IncreaseCorrectCount ()
	{
		correctCount++;
		
		if (correctCount >= totalPieces)
		{
			hintButton.SetActive(false);
			mySolutions[currLevel].SetActive(true);
			Invoke("ShowSolution", mySolutions[currLevel].ShowDialogue());
		}
	}

	public bool ValidDrop ()
	{
		float newTime = Time.time;
		if ((newTime - lastCorrectChoice) > minCorrectChoiceThreshold) {
			lastCorrectChoice = newTime;
			return true;
		}
		return false;
	}

}
