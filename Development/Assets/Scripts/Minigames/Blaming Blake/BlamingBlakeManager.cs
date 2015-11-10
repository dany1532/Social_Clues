using UnityEngine;
using System.Collections;

public class BlamingBlakeManager : MonoBehaviour {

	public int currLevel; 
	public bool incrementLevel;
	
	bool cont;
	public int sherTapObj; 
	public int correctCountRef;
	public int playSeqRef; 
	public int nextRowRef; 
	public int jRef;
	public int numSlotsCor; 
	public int numHighCor; 
	public bool turnOffPanels; 
	public bool newPanels; 
	public bool hitSlot; 
	public bool colorsWhiteRef; 
	
	//public int numLevels;

	//public GameObject[] myLevels;
	//public GameObject notepad;
	
	// Associated minigame
	Minigame minigame;
	
	public Dialogue TapObject;
	public Dialogue TapObject2;
	public Dialogue TapObject3;
	public Dialogue WrongChoiceFriend;
	public Dialogue WrongChoiceHead;
	public Dialogue RightChoiceBook;
	public Dialogue CorrectSequence;
	public Dialogue CompareSequence;
	public Dialogue CorrectButton;
	public BlamingBlakeManager Instance;//{get; private set;}
	public bool correctSeqRef; 
	public bool compareSeqRef; 
	public bool corSeqDia; 
	public bool turnOnBlameRef;
	public bool finishedSequence; 
	
	public bool column1; 
	public bool column2; 
	public bool column3; 
	public bool column4; 
	public bool column5; 
	public bool reset; 
	public bool hitHighlight; 
	public bool lightsOn; 
	public GameObject options; 
	//public GameObject correctButton; 
	public GameObject redDotted1;
	public GameObject redDotted2;
	public GameObject redDotted3;
	public GameObject redDotted4;
	public GameObject blameSequence; 
	public GameObject face1; 
	public GameObject face2; 
	public GameObject face3; 
	public GameObject thoughtBubble1; 
	public GameObject thoughtBubble2; 
	public GameObject thoughtBubble3;
	public GameObject highlights; 
	//public Texture waiting;
	//public Texture correct;
	//public Texture finish;
	
	//public UITexture max;
	
	public enum eType
	{
		Invalid_Type = -1, 
		Correct,
		Friend,
		Head
	}
	
	eType currentType = eType.Invalid_Type;
	
	// Use this for initialization
	void Start () {		
		//phase = 0;
		//Debug.Log("Phase: " + phase);
		Instance = this;
		//notepad.SetActive(false);
		Sherlock.Instance.SetBubblePosition (Sherlock.side.DOWN);
		//set cont to true if you want to use the WaitforInput coroutine
		cont = true;
		turnOffPanels = false; 
		sherTapObj = 0; 
		newPanels = false; 
		correctCountRef = 0;
		turnOnBlameRef = false; 
		hitSlot = false; 
		finishedSequence = false; 
		column1 = false;
		column2 = false;
		column3 = false;
		column4 = false;
		column5 = false; 
		reset = false; 
		colorsWhiteRef = false; 
		lightsOn = false; 
		jRef = 0;
		numSlotsCor = 0; 
		playSeqRef = 0;
		redDotted1.SetActive(false);
		redDotted2.SetActive(false);
		redDotted3.SetActive(false);
		redDotted4.SetActive(false);
		blameSequence.SetActive(false);  
		highlights.SetActive(false); 
		/*myLevels = new GameObject[numLevels];
		//find first level, or MaxLevel0
		//myLevels[0] = GameObject.Find("MaxLevel0");
		//fill up myLevels with remaining
		//int i = 1;
		for (int i = 0 ; i < numLevels ; i++)//(GameObject.Find("MaxLevel" + i.ToString()) !=null)
		{
			myLevels[i] = GameObject.Find("MaxLevel" + i.ToString());
			myLevels[i].SetActive(false);
		}*/
		
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: Let's help Max make new choices based on what he already likes. Max will give you a description for each category, you choose something similar.");
		}
		//StartCoroutine(WaitForInput("nextLevel"));
		
		//turnOffPanels = true; 
		playSeqRef = options.GetComponent<BlakeTurnOnOptions>().playSeq;
		
		nextLevel(); 
		

	}
	
	void Update()
	{
		Debug.Log("sherTapObj" + sherTapObj);
		//Debug.Log("correct: " + correctCount); 
		Debug.Log("playSeqRef: " + playSeqRef);
		//Debug.Log ("numSlotsCor: " + numSlotsCor);
		
		correctSeqRef = options.GetComponent<BlakeTurnOnOptions>().correctSeq;
		compareSeqRef = options.GetComponent<BlakeTurnOnOptions>().compareSeq;
		colorsWhiteRef = options.GetComponent<BlakeTurnOnOptions>().colorsWhite;
		
		nextRowRef = options.GetComponent<BlakeTurnOnOptions>().nextRow;
		jRef = options.GetComponent<BlakeTurnOnOptions>().j;
		//turnOnBlameRef = correctButton.GetComponent<CorrectButtonClick>().turnOnBlame;
		
		if (lightsOn == true)
		{
			
			StartCoroutine(WaitForInput("resetGame"));
			correctCountRef = 0; 
			playSeqRef = 0; 
			blameSequence.SetActive(false);
			lightsOn = false; 
			finishedSequence = false; 
		}
		
		if (correctCountRef >= 4)
		{
			//remove x'd answers and move correct answers to the top
		}
		
		if (playSeqRef == 4 && correctCountRef == 0 && jRef == 4)
		{
			//redDotted1.SetActive(true);
			column1 = true; 
		}
		else if (correctCountRef == 1)
		{
			//redDotted1.SetActive(false);
			column1 = false; 
			//redDotted2.SetActive(true);
			column2 = true; 
			Respond(CorrectButton);
		}
		else if (correctCountRef == 2)
		{
			//redDotted2.SetActive(false);
			column2 = false; 
			//redDotted3.SetActive(true);
			column3 = true;
			Respond(CorrectButton);
		}
		else if (correctCountRef == 3)
		{
			//redDotted3.SetActive(false);
			column3 = false; 
			//redDotted4.SetActive(true);
			column4 = true;
			Respond(CorrectButton);
		}
		else if (correctCountRef == 4)
		{
			//redDotted4.SetActive(false);
			column4 = false; 
			column5 = true; 
			Respond(CorrectSequence);
			face1.SetActive(false); 
			face2.SetActive(false); 
			face3.SetActive(false); 
			thoughtBubble1.SetActive(false);
			thoughtBubble2.SetActive(false); 
			thoughtBubble3.SetActive(false); 
			finishedSequence = true; 
		}
		
		if (numSlotsCor >= 1)
		{
			column5 = false; 
		}
		
		if (numSlotsCor >= 100)
		{
			blameSequence.SetActive(true);
			
			StartCoroutine(WaitForInput("highlightsOn"));
			
		}
		
		//This is just a hack to turn on the blame sequence and makes it happen at the same time that the buttons start moving
		/*if (turnOnBlameRef)
		{
			blameSequence.SetActive(true);
			Respond(CompareSequence); 
		}*/
		
		if(playSeqRef<4)//currLevel < numLevels)
		{
			/*myLevels[currLevel].SetActive(true);
			
			foreach(Transform child in myLevels[currLevel].transform)
			{
				if(child.GetComponent<DraggableObject>() != null)
					child.GetComponent<DraggableObject>().setStartPos();
			}*/
			playSeqRef = options.GetComponent<BlakeTurnOnOptions>().playSeq;
			switch(playSeqRef)
			{
			case 0:
				//do nothing
				break; 
			case 1:
				// Show the instructions of the minigame
				Respond(TapObject);
				break;
			case 2:
				//Debug.Log("Max: My favorite shirt is red with short sleeves.");
				// Show the instructions of the minigame
				Respond(TapObject2);
				break;
			case 3:
				//Debug.Log("Max: My favorite drink is apple juice. It's a sweet, fruit juice.");
				// Show the instructions of the minigame
				Respond(TapObject3);
				playSeqRef = 4; 
				//notepad.SetActive(false);
				break;
			}
			//max.mainTexture = waiting;
		}
		else
		{
			//Debug.Log("Return to Cafeteria");
		}
		
		/*if (currentType == eType.Correct)
			{
				Respond(RightChoiceBook);
				//change panels to blue
				//newPanels = true; 
				//increase the correctCount
				correctCount++; 
				//currentType = eType.Invalid_Type; 
				
			}
		else if (currentType == eType.Friend)
			{
				Respond(WrongChoiceFriend);
		}
		else if (currentType == eType.Head)
			{
				Respond(WrongChoiceHead);
		}*/
		
		if (correctSeqRef)
		{
			Respond(CorrectSequence);
			corSeqDia = true; 
			//correct seq dialogue has played
		}
		else if (compareSeqRef)
		{
			Respond(CompareSequence); 

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
		//notepad.SetActive(true);
		//phase = 1;
		//currLevel = 0;
		/*
		myLevels[currLevel].SetActive(true);
		
		foreach(Transform child in myLevels[currLevel].transform)
		{
			if(child.GetComponent<DraggableObject>() != null)
				child.GetComponent<DraggableObject>().setStartPos();
		}
		
		Debug.Log("Max: I like Pretzels. They are a salty snack.");
		max.mainTexture = waiting;
		*/
		//cont = true;
		turnOffPanels = true; 
		
		// Show the instructions of the minigame
		
		
		
		
		//StartCoroutine(WaitForInput("nextLevel"));
		
			
		
	}
	
	/*void nextSeq()
	{
		sherTapObj++; 
		if(sherTapObj == 1)
		{
		Respond(TapObject);
		StartCoroutine(WaitForInput("nextSeq"));
		}
		else if (sherTapObj == 2)
		{
		Respond(TapObject2);
		//StartCoroutine(WaitForInput("nextSeq"));
		}
		else
		{
			//do nothing
		}
		
	}*/
	
	
	
	void nextLevel()
	{
		//myLevels[currLevel].SetActive(false);
		
		//currLevel++;
		sherTapObj++; 
		if(playSeqRef<4)//currLevel < numLevels)
		{
			/*myLevels[currLevel].SetActive(true);
			
			foreach(Transform child in myLevels[currLevel].transform)
			{
				if(child.GetComponent<DraggableObject>() != null)
					child.GetComponent<DraggableObject>().setStartPos();
			}*/
			
			switch(playSeqRef)
			{
			case 0:
				//do nothing
				break; 
			case 1:
				// Show the instructions of the minigame
				Respond(TapObject);
				break;
			case 2:
				//Debug.Log("Max: My favorite shirt is red with short sleeves.");
				// Show the instructions of the minigame
				Respond(TapObject2);
				break;
			case 3:
				//Debug.Log("Max: My favorite drink is apple juice. It's a sweet, fruit juice.");
				// Show the instructions of the minigame
				Respond(TapObject3);
				//notepad.SetActive(false);
				break;
			}
			//max.mainTexture = waiting;
		}
		else
		{
			//Debug.Log("Return to Cafeteria");
		}
		
	}
	
	void highlightsOn()
	{	
		
		cont = false; 
		highlights.SetActive(true); 
		lightsOn = true; 
		
	}
	
	void resetGame()
	{
		reset = true; 
		highlights.SetActive(false);
	}
	
	void OnButtonClickDown(int corCountTemp)
	{
		//currentType = buttonType; 
		if (corCountTemp == 0)
		{
		correctCountRef = corCountTemp; 
		}
		else
		{
		correctCountRef++;
		}
	}
	
	void FinalSequence(bool hitSlotTemp)
	{
		hitSlot = hitSlotTemp;
		//Debug.Log ("hitSlotRef: " + hitSlot);
		if (hitSlot == true) //&& numSlotsCor < 100)
		{
			numSlotsCor++;
		}
		else
		{
			//do nothing
		}
	}
	
	void StartOver(bool hitHighlightTemp)
	{
		hitHighlight = hitHighlightTemp;
		Debug.Log ("hitHighlightRef: " + hitHighlight);
		reset = false; 
		if (hitHighlight == true) //&& numSlotsCor < 100)
		{
			
			numHighCor++;
		}
		else
		{
			//do nothing
		}
	}
	
	void BlameHack(bool blameTemp)
	{
		turnOnBlameRef = blameTemp; 
		/*if (hitSlot == true)
		{
		numSlotsCor++; 
		}
		else
		{
			//do nothing
		}*/
	}
	
	public void Respond (Dialogue response)
	{
		if (minigame != null && minigame.ContinueConversation(response) == false)
		{
			Debug.Log(response.text);
		}
	}
	/*
	public void UpdateCorrectSprite ()
	{
		if ((currLevel+1) == numLevels)
		{
			minigame.Invoke("CompleteMinigame", 2f);
			max.mainTexture = finish;
		}
		else{
			Invoke("nextLevel", 2.5f);
			max.mainTexture = correct;
		}
	}*/
}

