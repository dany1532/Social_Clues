using UnityEngine;
using System.Collections;

public class BlakeTurnOnOptions : MonoBehaviour {
	
	public GameObject manager;
	public GameObject corPanel; 
	public GameObject row1; 
	public GameObject row2;
	public GameObject row3;
	public int sherTapObjRef; 
	//public int correctCountRef; 
	public int numLevels; 
	public int nextRow; 
	public int playSeq; 
	public int j; 
	public int numHighCorRef; 
	public bool clickedRef; 
	public bool correctSeq; 
	public bool corSeqDiaRef;
	public bool compareSeq;
	public bool column1Ref; 
	public bool column2Ref;
	public bool column3Ref;
	public bool column4Ref;
	public bool column5Ref; 
	public bool colorsWhite;
	public bool resetRef;
	bool cont; 
	public float displaySequenceRepeatInterval = 1.0f;
	Color[] buttons; 
	GameObject[] myLevels;
	GameObject[] firstRow; 
	GameObject[] secondRow; 
	GameObject[] thirdRow; 
	GameObject[] crosses; 
	GameObject[] seqHighlights; 
	
	// Use this for initialization
	void Start () {
		
		correctSeq = false; 
		compareSeq = false; 
		colorsWhite = false; 
		resetRef = false; 
		//set cont to true if you want to use the WaitforInput coroutine
		cont = false;
		j = 0;
		nextRow = -1;
		playSeq = 0; 
		numLevels = 4; 
		myLevels = new GameObject[numLevels];
		//firstRow = new GameObject[numLevels];
		secondRow = new GameObject[numLevels];
		thirdRow = new GameObject[numLevels];	
		crosses = new GameObject[numLevels*3];
		seqHighlights = new GameObject[numLevels*3];
		buttons = new Color [numLevels];
	
		//fill myLevels with first row sequence
		for(int i = 0; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
		{
			myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
			myLevels[i].SetActive(false); 
		}
		//fill up secondRow with sequence
		for(int i = 0; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
		{
			secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
			secondRow[i].SetActive(false); 
		}
		//fill up thirdRow with sequence
		for(int i = 0; GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
		{
			thirdRow[i] = GameObject.Find("FriendButton" + i.ToString());
			thirdRow[i].SetActive(false); 
		}
		//turn off the crosses
		for(int i = 0; GameObject.Find("CrossOut" + i.ToString()) !=null; i++)
		{
			crosses[i] = GameObject.Find("CrossOut" + i.ToString());
			crosses[i].SetActive(false); 
		}
		//turn off the highlights in the sequence
		for(int i = 0; GameObject.Find("seqHighlight" + i.ToString()) !=null; i++)
		{
			seqHighlights[i] = GameObject.Find("seqHighlight" + i.ToString());
			seqHighlights[i].SetActive(false); 
		}
		StartCoroutine(WaitForInput("startGame"));
		
	}
	
	
	void Update () {
		Debug.Log("sherTapObjRef: " + sherTapObjRef);
		Debug.Log("playSeq: " + playSeq);
		Debug.Log ("numHighCorRef: " + numHighCorRef);
		sherTapObjRef = manager.GetComponent<BlamingBlakeManager>().sherTapObj;
		//correctCountRef = manager.GetComponent<BlamingBlakeManager>().correctCount;
		corSeqDiaRef = manager.GetComponent<BlamingBlakeManager>().corSeqDia;
		column1Ref = manager.GetComponent<BlamingBlakeManager>().column1; 
		column2Ref = manager.GetComponent<BlamingBlakeManager>().column2; 
		column3Ref = manager.GetComponent<BlamingBlakeManager>().column3; 
		column4Ref = manager.GetComponent<BlamingBlakeManager>().column4; 
		column5Ref = manager.GetComponent<BlamingBlakeManager>().column5;
		resetRef = manager.GetComponent<BlamingBlakeManager>().reset;
		numHighCorRef = manager.GetComponent<BlamingBlakeManager>().numHighCor;
		//clickedRef = corPanel.GetComponent<BlakeObjectTap>().clicked;
		
		if (numHighCorRef >= 1)
		{
			StartCoroutine(WaitForInput("startAgain"));
		}
		
		
		/*if (resetRef == true)
		{
			//fill myLevels with first row sequence
			for(int i = 0; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
			{
			myLevels[i].SetActive(false); 
			}
			//fill up secondRow with sequence
			for(int i = 0; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
			{
			secondRow[i].SetActive(false); 
			}
			//fill up thirdRow with sequence
			for(int i = 0; GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
			{
			thirdRow[i].SetActive(false); 
			}
			//turn off the crosses
			for(int i = 0; GameObject.Find("CrossOut" + i.ToString()) !=null; i++)
			{
			crosses[i].SetActive(false); 
			}
			//turn off the highlights in the sequence
			for(int i = 0; GameObject.Find("seqHighlight" + i.ToString()) !=null; i++)
			{
			seqHighlights[i].SetActive(false); 
			}
		}*/
		
		/*if (playSeq == 1)
		{
			//InvokeRepeating("DisplaySequence", 1, displaySequenceRepeatInterval);
			//Invoke ("DisplaySequence", 1);
			//Set which array will be read in here. 
			//DisplaySequence();
			//row2.SetActive(true); 
			//myLevels[j] = firstRow[j];
		}*/
		
		if (nextRow == 0)
		{
			playSeq = 1; 
		}
			else if (nextRow == 1)
			{
				playSeq = 2; 
				//myLevels[j] = secondRow[j]; 
				//row2.SetActive(true); 
				//row3.SetActive(true); 
			}
				else if (nextRow == 2)
				{
					playSeq = 3; 
					//myLevels[j] = thirdRow[j];
					//row2.SetActive(true); 
					//row3.SetActive(true); 
			
				}
					//else if 
		
		Debug.Log ("column1Ref: " + column1Ref);
		if (column1Ref == true)
			{
				myLevels[0].GetComponentInChildren<UISprite>().color = Color.white; 
				secondRow[0].GetComponentInChildren<UISprite>().color = Color.white; 
				thirdRow[0].GetComponentInChildren<UISprite>().color = Color.white; 
				Debug.Log ("column1");
				for(int i = 1; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
				{
				
					myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					myLevels[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				for(int i = 1; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
				{
				
					secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					secondRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				for(int i = 1; GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
				{
				
					thirdRow[i] = GameObject.Find("FriendButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					thirdRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
			}
		else if (column2Ref == true)
			{
				Debug.Log ("column2");
				for(int i = 0; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
				{
				
					myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					
				}
				for(int i = 0; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
				{
				
					secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//secondRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				for(int i = 0; GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
				{
				
					thirdRow[i] = GameObject.Find("FriendButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//thirdRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				
					myLevels[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					myLevels[2].GetComponentInChildren<UISprite>().color = Color.gray; 
					myLevels[3].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[2].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[3].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[2].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[3].GetComponentInChildren<UISprite>().color = Color.gray; 
			
					myLevels[1].GetComponentInChildren<UISprite>().color = Color.white; 
					secondRow[1].GetComponentInChildren<UISprite>().color = Color.white; 
					thirdRow[1].GetComponentInChildren<UISprite>().color = Color.white; 
			}
			else if (column3Ref == true)
			{
				Debug.Log ("column3");
				for(int i = 0; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
				{
				
					myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					
				}
				for(int i = 0; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
				{
				
					secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//secondRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				for(int i = 0; GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
				{
				
					thirdRow[i] = GameObject.Find("FriendButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//thirdRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				
					myLevels[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					myLevels[1].GetComponentInChildren<UISprite>().color = Color.gray; 
					myLevels[3].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[1].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[3].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[1].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[3].GetComponentInChildren<UISprite>().color = Color.gray;
			
					myLevels[2].GetComponentInChildren<UISprite>().color = Color.white; 
					secondRow[2].GetComponentInChildren<UISprite>().color = Color.white; 
					thirdRow[2].GetComponentInChildren<UISprite>().color = Color.white;
			}
			else if (column4Ref == true)
			{
				Debug.Log ("column4");
				for(int i = 0; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
				{
				
					myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					
				}
				for(int i = 0; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
				{
				
					secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//secondRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				for(int i = 0; GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
				{
				
					thirdRow[i] = GameObject.Find("FriendButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//thirdRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
				
					myLevels[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					myLevels[1].GetComponentInChildren<UISprite>().color = Color.gray; 
					myLevels[2].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[1].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[2].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[0].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[1].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[2].GetComponentInChildren<UISprite>().color = Color.gray; 
			
					myLevels[3].GetComponentInChildren<UISprite>().color = Color.white; 
					secondRow[3].GetComponentInChildren<UISprite>().color = Color.white; 
					thirdRow[3].GetComponentInChildren<UISprite>().color = Color.white;
			}
			else if (column5Ref == true)
			{
				Debug.Log ("column5");
				for(int i = 0; i<3; i++)//GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
				{
					if (GameObject.Find("HeadButton" + i.ToString()) !=null)
					{
					//myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					myLevels[i].GetComponentInChildren<UISprite>().color = Color.white;
					}
				}
				for(int i = 0; i<3; i++)//GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
				{
					if (GameObject.Find("CorrectButton" + i.ToString()) !=null)
					{
					//secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//secondRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
					secondRow[i].GetComponentInChildren<UISprite>().color = Color.white;
					}
				}
				for(int i = 0; i<3; i++)//GameObject.Find("FriendButton" + i.ToString()) !=null; i++)
				{
					
					if (GameObject.Find("FriendButton" + i.ToString()) !=null){				
					//thirdRow[i] = GameObject.Find("FriendButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					//thirdRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
					thirdRow[i].GetComponentInChildren<UISprite>().color = Color.white;
					}
				}
				colorsWhite = true; 
			}
		/*if (correctCountRef >= 4)
		{
			row1.SetActive(false); 
			row3.SetActive(false); 
			correctSeq = true; 
		}
		if (corSeqDiaRef)
		{
			correctSeq = false;
			row1.SetActive(true);  
			compareSeq = true; 
		}*/
	
	}
	
	void DisplaySequence()
	{
		if (j < numLevels) //&& sherTapObjRef != 0)
		{
			if (nextRow == 0)
			{
				myLevels[j].SetActive(true);  
				seqHighlights[j].SetActive(true);
				if (j > 0)
				{
					seqHighlights[j-1].SetActive(false);
				}
			}
			else if (nextRow == 1)
			{
				secondRow[j].SetActive(true);  
				seqHighlights[j+4].SetActive(true);
				seqHighlights[j+3].SetActive(false);
				//for(int i = 0; GameObject.Find("HeadButton" + i.ToString()) !=null; i++)
				for (int i = 0; i<4; i++)
				{
				
					myLevels[i] = GameObject.Find("HeadButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					myLevels[i].GetComponentInChildren<UISprite>().color = Color.gray;
					
				}
			}
			else if (nextRow == 2)
			{
				thirdRow[j].SetActive(true); 
				seqHighlights[j+8].SetActive(true);
				seqHighlights[j+7].SetActive(false);
				//for(int i = 0; GameObject.Find("CorrectButton" + i.ToString()) !=null; i++)
				for (int i = 0; i<4; i++)
				{
				
					secondRow[i] = GameObject.Find("CorrectButton" + i.ToString());
					/*buttons[i] = myLevels[i].GetComponentInChildren<UISprite>().color;
					buttons[i] = Color.black; */
					secondRow[i].GetComponentInChildren<UISprite>().color = Color.gray; 
				}
			}
			j++;
		}
		else //if (j >= numLevels)
		{
			nextRow++; 
			j = 0; 
			
			if (nextRow == 3)
			{
			seqHighlights[11].SetActive(false);
			}
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
		//Make it so that this does nothing until we get to Update. Read in empty game objects into myLevels[] in start. 
		InvokeRepeating("DisplaySequence", 1, displaySequenceRepeatInterval);
		nextRow = 0; 
		playSeq = 0; 
		j = 0; 
		colorsWhite = false; 
	}
	
	void startAgain()
	{
		nextRow = 0; 
		playSeq = 0; 
		j = 0; 
		colorsWhite = false;
	}
}
