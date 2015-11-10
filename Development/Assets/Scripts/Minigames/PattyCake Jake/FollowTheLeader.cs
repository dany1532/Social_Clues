using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 1. Right and left hand animations

2. Switching the images of the hands (right, left & clap) for each new kid. Right now it only plays Jake's hand images. 
I know there's a way to change the sprites that are displayed, but I haven't had a chance to look at Mad Max to learn how to do it yet. 

3. Getting the clapping hands image to appear when you press the clap button. 
(and the right and left animations to play at the correct button presses as well). 
This by itself is easy enough, but I assumed we would only want the animations to play 
if the user was pressing the correct buttons in the sequence. (i.e. If they click the wrong hand, the animation won't play.)
I have been having trouble getting this to work. (If you search for "step 3" in the Follow the Leader script, 
you'll see what I was working on.)

4. I believe Jeremy wants the Jake Sequence to play back again as the user presses the correct sequence of hands.  
Once I get step 3 working, I can add in a line to tell it to play back the corresponding hand in the Jake Sequence. 

5. Change the look of the button glow? I'm not sure how Jeremy wants it to look and there wasn't an art asset provided. 
*/
struct PlayerTextures
{
	public PlayerTextures (string playerTextureName)
	{
		playerTexture = GameObject.Find (playerTextureName).GetComponent<UITexture>();
	}
		
	public UITexture playerTexture;		
}

struct PlayerSprite
{
	public PlayerSprite (string playerTextureName)
	{
		playerSprite = GameObject.Find (playerTextureName).GetComponent<UIButtonPattyJake>();
	}
	
	public UIButtonPattyJake playerSprite;
}

public class FollowTheLeader : MinigameManager
{
	
	public enum eType
	{
		INVALID_TYPE = -1,
		RIGHT,
		LEFT,
		CLAP,
		NUM_TYPES,
		IDLE
	}
	
	// Different states a plate can be set to
	public enum DialogueType
	{
		TUTORIAL,
		TOBLAKE,
		TODRAKE,
		END
	}
	
	// Associated minigame
	Minigame minigame;
	public float displaySequenceRepeatInterval = 1.0f;
	public float displaySequencePauseInterval = 0.5f;
	public UITexture npcTexture;
	public UIStretch npcStretch;
	PlayerTextures[] npcTextures = new PlayerTextures[(int)eType.NUM_TYPES];
	PlayerSprite[] userButtons = new PlayerSprite[(int)eType.NUM_TYPES];
	[System.NonSerializedAttribute]
	public FollowTheLeader Instance;
	
	int numClickSeq = 3;
	private Color playerTextureColor;
	public Collider peteRight;
	public Collider peteLeft;
	public Collider peteClap;
	public GameObject jakeSpeech;
	public GameObject jakeFace;
	public GameObject glowClap; 
	public GameObject tutorialHand;
	enum eState
	{
		INVALID_STATE = -1,
		THINKING,
		DISPLAY_SEQUENCE,
		WAITING_FOR_USER,
		NUM_STATES,
		WAIT
	}
	
	enum kidState
	{
		INVALID_STATE = -1,
		JAKE,
		ANDREW,
		PETE,
		WIN_GAME, 
		NUM_STATES,
		TUTORIAL
	}
	
	eState currentState = eState.INVALID_STATE;
	//Can I set it to Jake at the outset? May have to change this. It works, but maybe it's not the cleanest. 
	kidState currentKidState = kidState.TUTORIAL;
	List<int> sequence = new List<int> (100); // holds the sequence
	int sequenceCount = 0; // the counter for which index of the sequence we're currently showing.
	
	List<int> clickedSequence = new List<int> (100);

	public GameObject jakeDialogues;
	public GameObject laurenDialogues;
	public GameObject andrewDialogues;

	public Dialogue startGameDialogue;

	SherlockMultipleDialogues incorrectResponses;
	SherlockMultipleDialogues correctResponses;
	
	public Dialogue nextPattern;
	// Dialogue when player has completed Blake and moves on to play with Drake
	public Dialogue teachFriend;
	// Dialogue when player has completed Jake and moves on to play with Blake
	public Dialogue switchCharacter;
	// Andrew says this after you create the pattern
	public Dialogue greatPattern;
	// Andrew says this after you play with him
	public Dialogue thanksForTeaching;
	// Dialogue at the end of the minigame, after all the levels
	public Dialogue end;

	// Dialogue at the end of the first two characters
	public Dialogue endCharacter;
	
	[System.SerializableAttribute]
	public class Character
	{
		public Texture idle;
		public Texture rightHand;
		public Texture leftHand;
		public Texture clap;
		public Dialogue yourTurn;
		public Dialogue ready;
	}
	
	public Color highlightColor = new Color(255, 168, 48);
	public List<Character> players;
	private int currentPlayer = 0;
	
	public bool showTutorial;
	public GameObject tutorialOptions;
	
	public UILabel instructionCountLabel;
	public Vector3 instructionLabelOffset;
	private Vector3 leftLabelOffset;
	public Vector3 rightLabelOffset;
	
	public AudioClip clapAudio;

	public List<Dialogue> tutorial;
	
	public AudioClip rightAnswerFX;
	public AudioClip wrongAnswerFX;
		
	void Start ()
	{

		Instance = this;
		
		//changing these to the background sprite to attempt to access their color directly, if it doesn't work, 
		//change it back to "MoveRightHandButton" etc. 
		userButtons [(int)eType.RIGHT] = new PlayerSprite ("MoveRightHandButton");
		userButtons [(int)eType.LEFT] = new PlayerSprite ("MoveLeftHandButton");
		userButtons [(int)eType.CLAP] = new PlayerSprite ("MoveClapHandButton");
		UpdatePlayerTextures();
		
		tutorialHand = GameObject.Find("TutorialHand");
		tutorialHand.SetActive(false);



		Sherlock.Instance.SetBubblePosition (Sherlock.side.DOWN);
		
		// Find assocciated minigame
		minigame = GetComponent<Minigame> ();

		Sherlock.Instance.HideDialogue();

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
			showTutorial = true;

			// Show the instructions of the minigame
			if (minigame != null && minigame.StartConversation () == false) {
				Debug.Log (minigame.conversationTree.root);
			}
			
			Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
			leftLabelOffset = instructionLabelOffset;
			rightLabelOffset = instructionLabelOffset;
			rightLabelOffset.x = -rightLabelOffset.x;
			
			SetHandColliders(false);
		}
		//noTutorial();
	}

	public override void yesTutorial ()
	{
		Destroy(tutorialOptions);
		showTutorial = true;
		
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation () == false) {
			Debug.Log (minigame.conversationTree.root);
		}
		
		Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		leftLabelOffset = instructionLabelOffset;
		rightLabelOffset = instructionLabelOffset;
		rightLabelOffset.x = -rightLabelOffset.x;
		
		SetHandColliders(false);
	}

	public override void noTutorial ()
	{
		Destroy(tutorialOptions);
		showTutorial = false;
		
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation () == false) {
			Debug.Log (minigame.conversationTree.root);
		}
		
		Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		leftLabelOffset = instructionLabelOffset;
		rightLabelOffset = instructionLabelOffset;
		rightLabelOffset.x = -rightLabelOffset.x;
		
		SetHandColliders(false);
	}
	
	void SetHandColliders(bool state)
	{
		peteRight.enabled = state;
		peteLeft.enabled = state;
		peteClap.enabled = state;
	}

	void enableHandColliders() {
		peteRight.enabled = true;
		peteLeft.enabled = true;
		peteClap.enabled = true;
	}
	
	void continueDialogue()
	{
		if(!minigame.ContinueConversation())
		{
			if(showTutorial) 
				StartCoroutine("startTutorial");
			else 
				Invoke ("startGame", 1);
		}
		else
		{
			Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		}
		
	}
	
	IEnumerator startTutorial()
	{
		TutorialHand hand = tutorialHand.GetComponent<TutorialHand>();
        Respond(tutorial[0]);
        yield return new WaitForSeconds(tutorial[0].voiceOver.length + 0.75f);
		sequence.Add((int) eType.RIGHT);
		sequence.Add((int) eType.LEFT);
		PrepareDisplaySequence(false);
		yield return new WaitForSeconds(1.5f);
		Respond(tutorial[1]);
		yield return new WaitForSeconds(2*(displaySequencePauseInterval + displaySequenceRepeatInterval)+1.5f);
		//yield return new WaitForSeconds(0.5f);
		
		Respond(tutorial[2]);
		tutorialHand.SetActive(true);
		hand.nextWayPoint();
		yield return new WaitForSeconds(hand.moveInterval);
		
		hand.isPointing(true);
		userButtons[(int)eType.RIGHT].playerSprite.animation.Play();
		Invoke ("UpdatePlayerTextures", 1.0f);
		yield return new WaitForSeconds(1.0f);
		hand.isPointing(false);
		yield return new WaitForSeconds(1.0f);
		
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		
		tutorialHand.GetComponent<TutorialHand>().isPointing(true);
		userButtons[(int)eType.LEFT].playerSprite.animation.Play();
		Invoke ("UpdatePlayerTextures", 1.0f);
		yield return new WaitForSeconds(1.0f);
		tutorialHand.GetComponent<TutorialHand>().isPointing(false);
		yield return new WaitForSeconds(0.75f);
		
		Respond(tutorial[3]);
		//Respond (clap);
		sequence.Clear();
		sequence.Add((int) eType.CLAP);
		PrepareDisplaySequence(false);
		yield return new WaitForSeconds((displaySequencePauseInterval + displaySequenceRepeatInterval));
		
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		
		tutorialHand.GetComponent<TutorialHand>().isPointing(true);
		UserClap(true);
		yield return new WaitForSeconds(2);
		tutorialHand.SetActive(false);
		UserClap (false);
        yield return new WaitForSeconds(1);
		startGame();
	}
	

	void startGame ()
	{
		Sherlock.Instance.SetDialogue(startGameDialogue);
		Respond(startGameDialogue);
		currentKidState = kidState.JAKE; 
		Invoke("ResetGame", minigame.GetCurrentDialogueComulativeDuration() + 1);
	}
	
	Texture GetPlayerTexture (int type)
	{
		if (currentPlayer < players.Count)
		{
			switch(type)
			{
			case (int) eType.IDLE:
				return players[currentPlayer].idle;
			case (int) eType.LEFT:
				return players[currentPlayer].leftHand;
			case (int) eType.RIGHT:
				return players[currentPlayer].rightHand;
			case (int) eType.CLAP:
				return players[currentPlayer].clap;
			}
		}
		
		return npcTexture.mainTexture;
	}
	
	void UpdatePlayerTextures ()
	{
		UserClap(false);
		turnOffHand ();
		if (currentPlayer < players.Count)
		{
			npcTexture.mainTexture = players[currentPlayer].idle;
		}
	}
	
	void PrepareDisplaySequence (bool clear = true)
	{
		currentState = eState.DISPLAY_SEQUENCE;
		
		if (clear)
			clickedSequence.Clear ();
		
		sequenceCount = 0;
		InvokeRepeating ("DisplaySequence", 1, displaySequencePauseInterval + displaySequenceRepeatInterval);
	}

	void PrepareDisplaySequenceAndrew() {
		currentState = eState.DISPLAY_SEQUENCE;

		clickedSequence.Clear ();
		
		sequenceCount = 0;
		InvokeRepeating ("DisplaySequence", 1, displaySequencePauseInterval + displaySequenceRepeatInterval);
	}
	
	void OnButtonClickDown (eType handType)
	{
		if (currentState == eState.WAITING_FOR_USER)
		{
			clickedSequence.Add ((int)handType);
			
			if (currentKidState == kidState.PETE)
			{	
				sequence.Add ((int)handType);
			}
		}
	}
	
	void OnButtonClickUp (eType handType)
	{
		if (currentState == eState.WAITING_FOR_USER) {
			npcTexture.mainTexture = GetPlayerTexture(sequence[clickedSequence.Count-1]);

			if (clickedSequence.Count > 0 && clickedSequence.Count <= sequence.Count)
			{
				if(clickedSequence[clickedSequence.Count - 1] == (int) eType.CLAP) {
					UserClap(true);
					Invoke ("UpdatePlayerTextures", 0.75f);
				} else if(clickedSequence[clickedSequence.Count - 1] == (int) eType.LEFT || 
				          clickedSequence[clickedSequence.Count - 1] == (int) eType.RIGHT) {
					userButtons[(int)clickedSequence[clickedSequence.Count - 1]].playerSprite.animation.Play();
					Invoke ("UpdatePlayerTextures", 1.0f);
				} else {
					Invoke ("UpdatePlayerTextures", 1.0f);
				}

				AudioManager.Instance.Play(clapAudio, this.transform, 1, false);
			}
			
			if (!VerifySequence ()) 
			{
				AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
				SetHandColliders(false);
				//If the student gets a wrong answer, Sherlock prompts them to try again and Jake repeats the pattern
				Respond(incorrectResponses.GetRandomDialogue());	
				showSpeechBubble();
				PrepareDisplaySequence();
				
			} 
			else 
			{
				if (currentKidState == kidState.JAKE || currentKidState == kidState.ANDREW)
				{
					if (clickedSequence.Count == numClickSeq) 
					{
						SetHandColliders(false);
						Respond(correctResponses.GetRandomDialogue());
						showSpeechBubble ();
					
						//do not switch to Thinking
						Invoke("KidChange", minigame.GetCurrentDialogueComulativeDuration());
					} 
					else 
					{
						if (clickedSequence.Count == sequence.Count) 
						{	
							SetHandColliders(false);
							Respond(correctResponses.GetRandomDialogue());
							showSpeechBubble ();

							currentState = eState.THINKING;
							clickedSequence.Clear ();
							
							// Add new sequence.
							Invoke("PrepareNewSequence", minigame.GetCurrentDialogueDuration());
						}
					}
				}
				//some repeated code, could be improved
				//If it's Pete/Kate's turn, wait until clicked sequence is the proper number(3), then display the sequence
				if (currentKidState == kidState.PETE) {
					if (clickedSequence.Count == numClickSeq) {
						//do not switch to Thinking
						Respond(greatPattern);
						showSpeechBubble();
						SetHandColliders(false);
						Invoke("PrepareDisplaySequenceAndrew", greatPattern.GetCommulativeDuration());
					}
				}
					
			}
		}
	}

	void HideUserClap()
	{
		UserClap(false);
	}
	
	void UserClap(bool clap)
	{
		userButtons[(int) eType.CLAP].playerSprite.gameObject.SetActive(!clap);
		userButtons[(int) eType.LEFT].playerSprite.gameObject.SetActive(!clap);
		userButtons[(int) eType.RIGHT].playerSprite.gameObject.SetActive(!clap);
		glowClap.SetActive(clap);
	}
		
	void KidChange ()
	{
		currentPlayer++;
		if (currentPlayer == 1) {
			currentKidState = kidState.ANDREW;
			Respond(endCharacter);
			Invoke ("UpdatePlayerTextures", minigame.GetCurrentDialogueDuration());

			StartCoroutine(setSherlockResponse(minigame.GetCurrentDialogueDuration(), switchCharacter));
			Invoke ("ResetGame", switchCharacter.GetCommulativeDuration() + minigame.GetCurrentDialogueDuration());

		} else if (currentPlayer == 2) {
			currentKidState = kidState.PETE;
			Respond(endCharacter);
			Invoke ("UpdatePlayerTextures", minigame.GetCurrentDialogueDuration());

			StartCoroutine(setSherlockResponse(minigame.GetCurrentDialogueDuration(), teachFriend));
			Invoke ("ResetGame", teachFriend.GetCommulativeDuration() + minigame.GetCurrentDialogueDuration());
			Invoke ("enableHandColliders", teachFriend.GetCommulativeDuration() + minigame.GetCurrentDialogueDuration());
		} else {
			currentKidState = kidState.WIN_GAME;
			Respond (thanksForTeaching);
			Invoke("playEndDialogue", minigame.GetCurrentDialogueComulativeDuration());
			ResetGame (); 
		}
	}

	void playEndDialogue() {
		Sherlock.Instance.PlaySequenceInstructions(end, NotifyEndMinigame);
	}
	
	void NotifyEndMinigame()
	{
		minigame.CompleteMinigame();
	}
	
	bool VerifySequence ()
	{
		if (currentKidState == kidState.JAKE || currentKidState == kidState.ANDREW) 
		{
			// Let's verify the sequence up until that point where the user left off.
			for (int i = 0; i < clickedSequence.Count; ++i) {
			
				if (clickedSequence [i] != sequence [i]) {
					return false;
				}
			}
		
			return true;
		} else {
			
			//If it's Pete's turn, his 3-hand sequence verifies as true after the sequence has played. 
			return true; 
		}
	}
	
	void showSpeechBubble ()
	{
		string bubbleLabel = minigame.conversationTree.currentNode.text;
		bubbleLabel = bubbleLabel.Replace(' ', '\n');
		jakeSpeech.transform.FindChild("BubbleLabel").GetComponent<UILabel>().text = bubbleLabel;
		jakeSpeech.SetActive (true);
		Invoke ("hideSpeechBubble", minigame.GetCurrentDialogueDuration());
	}
	
	void hideSpeechBubble ()
	{
		jakeSpeech.SetActive (false);
	}
	
	void DisplaySequence ()
	{
		// We're at the next hand in the sequence so turn off the previous one.
		if (sequenceCount > 0)
		{
			npcTexture.mainTexture = GetPlayerTexture((int) eType.IDLE);
			//npcTextures [sequence [sequenceCount - 1]].playerTexture.gameObject.SetActive (false); //.GetComponent<UISprite>().fillAmount = 0;
			//currentFace.SetActive (true);
			//Change previous Pete Hand color back to original 
			//rlcButtonsPete[sequence[sequenceCount-1]].playerTexture.GetComponentInChildren<UISprite>().color = Color.white; 
			userButtons [sequence [sequenceCount - 1]].playerSprite.sprite.color = Color.white;
		}
		
		if (currentKidState == kidState.PETE && sequenceCount == numClickSeq)
		{
			CancelInvoke("DisplaySequence");
			//Once the player has gotten all the hands in the pattern correct, start over. 	
			Invoke("KidChange", 1.0f);
		}
		else {
			if (sequenceCount < sequence.Count)
			{
				Vector3 pos = userButtons [sequence [sequenceCount]].playerSprite.transform.position;
				instructionCountLabel.text = (sequenceCount + 1).ToString();
				instructionCountLabel.transform.position = userButtons [sequence [sequenceCount]].playerSprite.transform.position + ((pos.x < 0) ? leftLabelOffset : rightLabelOffset);
				userButtons [sequence [sequenceCount]].playerSprite.sprite.color = highlightColor;				
				npcTexture.mainTexture = GetPlayerTexture(sequence [sequenceCount]);

				// Teaching Andrew how to play
				if(currentKidState == kidState.PETE) {
					if(sequence[sequenceCount] == (int) eType.CLAP) {
						UserClap(true);
						AudioManager.Instance.Play(clapAudio, this.transform, 1, false);
						Invoke ("UpdatePlayerTextures", 0.75f);
					} else if(sequence[sequenceCount] == (int) eType.LEFT || sequence[sequenceCount] == (int) eType.RIGHT) {
						userButtons[(int)sequence[sequenceCount]].playerSprite.animation.Play();
						AudioManager.Instance.Play(clapAudio, this.transform, 1, false);
						Invoke ("UpdatePlayerTextures", 1.0f);
					} else {
						Invoke ("UpdatePlayerTextures", 1.0f);
					}
				}
			}
			else
			{
				instructionCountLabel.text = "";
				CancelInvoke("DisplaySequence");
				// We're at the end, collect user input
				if (sequenceCount == sequence.Count)
				{
					if(currentKidState != kidState.TUTORIAL && currentPlayer < players.Count && players[currentPlayer].yourTurn != null) {
						Respond(players[currentPlayer].yourTurn);
						showSpeechBubble();
					}
					
					currentState = eState.WAITING_FOR_USER;
					SetHandColliders(true);
				}
			}
			
			
			//Blink on Pete's hand color at the same time as Jake plays the sequence. 
			//playerTextureColor = rlcButtonsPete[sequence[sequenceCount]].playerTexture.GetComponent<UIButtonPattyJake>().c;
			//rlcButtonsPete[sequence[sequenceCount]].playerTexture.GetComponentInChildren<UISprite>().color = playerTextureColor; 
			Invoke ("turnOffHand", displaySequencePauseInterval);
			++sequenceCount;
		}
	}
			
	public void Respond (Dialogue response)
	{
		if (minigame != null && minigame.ContinueConversation(response) == false)
		{
			Debug.Log(response.text);
		}

		if(response.nextDialogue != null) {
			if(response.nextDialogue.speaker == Dialogue.Speaker.SHERLOK) {
				StartCoroutine(setSherlockResponse(minigame.GetCurrentDialogueDuration(), response.nextDialogue));
			}
		}
	}
	
	IEnumerator setSherlockResponse(float waitTime, Dialogue response) {
		yield return new WaitForSeconds(waitTime);
		if(response != null) {
			Sherlock.Instance.SetDialogue(response);
			Respond(response);
		}
	}
	
	void turnOffHand ()
	{
		//currentFace.SetActive (true);
		npcTexture.mainTexture = GetPlayerTexture((int) eType.IDLE);
		//npcTextures [sequence [sequenceCount - 1]].playerTexture.gameObject.SetActive (false); //.GetComponent<UISprite>().fillAmount = 0;
		if (sequenceCount > 0 && sequenceCount < sequence.Count)
			userButtons [sequence [sequenceCount - 1]].playerSprite.sprite.color = Color.white;
	}
	
	void PrepareNewSequence()
	{
		if (sequence.Count > 0)
			Respond(nextPattern);
		Invoke("AddNewSequence", nextPattern.voiceOver.length);
	}
	
	void AddNewSequence ()
	{	
		sequence.Add (Random.Range (0, (int)eType.NUM_TYPES));
		PrepareDisplaySequence(false);
	}
	
	void ResetGame ()
	{
        Sherlock.Instance.HideDialogue();
        
		sequence.Clear ();
		clickedSequence.Clear ();
		sequenceCount = 0;
		
		// Only add new random sequence on ResetGame if the kid playing is Jake or Blake
		// Set the corresponding dialogues
		if(currentKidState == kidState.JAKE) {
			incorrectResponses = jakeDialogues.transform.FindChild("IncorrectResponses").GetComponent<SherlockMultipleDialogues>();
			correctResponses = jakeDialogues.transform.FindChild("CorrectResponses").GetComponent<SherlockMultipleDialogues>();
			AddNewSequence ();
		} else if(currentKidState == kidState.ANDREW) {
			incorrectResponses = laurenDialogues.transform.FindChild("IncorrectResponses").GetComponent<SherlockMultipleDialogues>();
			correctResponses = laurenDialogues.transform.FindChild("CorrectResponses").GetComponent<SherlockMultipleDialogues>();
			AddNewSequence ();
		}
		
		npcTexture.mainTexture = GetPlayerTexture((int) eType.IDLE);
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
	public bool ShowDialogue (DialogueType dialogueType)
	{
		// Display dialogue depending on dialogue type
		switch (dialogueType) {
		case DialogueType.TOBLAKE:
			Sherlock.Instance.PlaySequenceInstructions (switchCharacter, null);
			return true;
		case DialogueType.TODRAKE:
			Sherlock.Instance.PlaySequenceInstructions (teachFriend, null);
			return true;
		case DialogueType.END:
			return minigame.ContinueConversation (end);
		case DialogueType.TUTORIAL:
			return minigame.ContinueConversation (tutorial[0]);
		}
		
		return false;
	}
}
