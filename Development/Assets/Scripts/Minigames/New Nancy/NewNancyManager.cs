using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewNancyManager : MinigameManager {
	
	Minigame minigame;
	
	/*Dialogue*/
	public enum DialogueType
	{
		START, CORRECT, INCORRECT, INTEREST, INCORRECTINTEREST, JULIAINTEREST, AMBERINTEREST, START_NEXT_ROUND, FOUND, END, INTERESTS_QUESTION
	}
	
	public Dialogue start;
	public Dialogue end;
	public Dialogue correct;
	public Dialogue incorrect;
	public Dialogue interest;
	public Dialogue juliainterest;
	public Dialogue amberinterest;
	public Dialogue incorrectinterest;
	public Dialogue amber;
	public Dialogue found;
	public Dialogue tutorial1;
	public Dialogue tutorial2;
	public Dialogue tutorial3;
	public Dialogue tutorial4;
	public Dialogue dialogueFoundAllHidden;
	public Dialogue interestsQuestion;
	/*end Dialogue*/
	
	public List<HiddenObjectCharacter> characters;
	
	public GameObject interestOptionsRoot;
	int questionIndex;
	public List<Interest> interests;
	
	TutorialHand tutorialHand;
	public float tutorialHandFinalInterval = 1;
	
	public int currentLevel;
	
	//specify the number of items players will have to find for each student
	public int numItems;
	int itemsLeft;
	int numWrong;
	
	public Difficuly tutorialDifficulty;
	public HiddenObjectCharacter tutorialCharacter;
	public bool showTutorial;
	public GameObject tutorialOptions;
	
	List<HiddenObjectCharacter> currentLevelCharacter = new List<HiddenObjectCharacter>();
	
	[System.Serializable]
	public class Level
	{
		public enum Question_Type
		{
			CHOOSE_TOPIC,
			CHOOCE_CHARACTER
		}
		public int noCharacters;
		public int noEasyObjects;
		public int noHardObjects;
	}
	
	[System.Serializable]
	public class Difficuly
	{
		public MinigameDifficulty.Difficulty difficulty;
		public List<Level> levels;
	}
	public List<Difficuly> difficultySessions;
	Difficuly currentDifficulty;
	
	public List<UIScaledTexture> oddObjectsList;
	public List<UIScaledTexture> evenObjectsList;
	
	public GameObject hiddenObjectsRoot;
	public List<DisplayedObject> hiddenObjectsList;
	
	public GameObject starParticlesPrefab;
	
	public AudioClip rightAnswerFX;
	public AudioClip wrongAnswerFX;
	
	// Use this for initialization
	void Start () {
		Random.seed = (int) (System.DateTime.UtcNow.Ticks % int.MaxValue);
		
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		Sherlock.Instance.SetBubblePosition(Sherlock.side.DOWN);
		

		
		interestOptionsRoot.SetActive(false);
		hiddenObjectsRoot.SetActive(false);
		
		tutorialHand = GameObject.Find ("TutorialHand").GetComponent<TutorialHand>();
		
		currentLevel = -1;
		ShuffleCharacters();
		ResetDisplayObjects();

		Sherlock.Instance.HideDialogue();

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
			if(showTutorial)
			{
				// Show the instructions of the minigame
				if (minigame != null && minigame.StartConversation() == false)
				{
					Debug.Log("To learn other peoples’ interests, use your eyes to find clues about them.");
				}
				StartCoroutine("startTutorial");
			}
			else
			{
				// Show the instructions of the minigame
				if (minigame != null && minigame.StartConversation() == false)
				{
					Debug.Log("To learn other peoples’ interests, use your eyes to find clues about them.");
				}
				Invoke ("InitializeFirstLevel", minigame.GetCurrentDialogueDuration());
			}
		}

	}

	public override void yesTutorial ()
	{
		Destroy(tutorialOptions);
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("To learn other peoples’ interests, use your eyes to find clues about them.");
		}
		StartCoroutine("startTutorial");
	}

	public override void noTutorial ()
	{
		Destroy(tutorialOptions);
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("To learn other peoples’ interests, use your eyes to find clues about them.");
		}
		Invoke ("InitializeFirstLevel", minigame.GetCurrentDialogueDuration());
	}
		
	void InitializeFirstLevel()
	{		
		// Select difficult session based on minigame
		foreach (Difficuly session in difficultySessions)
		{
			if (session.difficulty == minigame.difficulty)
			{
				currentDifficulty = session;
				break;
			}
		}
		if (currentDifficulty == null && difficultySessions != null && difficultySessions.Count > 0)
			currentDifficulty = difficultySessions[0];
		
			
		Invoke ("nextLevel", 1 /*minigame.GetCurrentDialogueDuration()*/);
	}
	
	IEnumerator startTutorial()
	{
		// Hide question
		interestOptionsRoot.SetActive(false);
		currentLevelCharacter.Clear();
		itemsLeft = 1;
		hiddenObjectsRoot.SetActive(true);
		
		List<UIScaledTexture> activeTextures = new List<UIScaledTexture>();
		AssignCharacterSlots(activeTextures, 1);
		currentLevelCharacter.Add(tutorialCharacter);
		tutorialCharacter.texture.SetComponents(activeTextures[0].texture, activeTextures[0].stretch);
		tutorialCharacter.LoadCharacter();
		tutorialCharacter.transform.position = activeTextures[0].texture.transform.position;
		
		List<HiddenObject> currentLevelObjects = new List<HiddenObject>();
		HiddenObject hiddenObject = tutorialCharacter.GetRandomObject(HiddenObject.ObjectDifficulty.EASY);
		hiddenObject.SetCollider(false);
		currentLevelObjects.Add(hiddenObject);
		
		float moveInterval = tutorialHand.moveInterval;
		
		Respond(tutorial1);
		yield return new WaitForSeconds(tutorial1.voiceOver.length + 0.5f);
		
		DisplayObjects(currentLevelObjects);
				
		Respond(tutorial2);
		tutorialHand.nextWayPoint();
		yield return new WaitForSeconds(tutorial2.voiceOver.length + 0.5f);
		
		Respond(tutorial3);
		
		tutorialHand.nextWayPoint();
		yield return new WaitForSeconds(moveInterval + 2.0f);
		tutorialHand.isPointing(true);
		hiddenObjectsList[3].tick.enabled = true;
		PlayParticleAnimation(hiddenObject.transform.position);
		
		yield return new WaitForSeconds(tutorial3.voiceOver.length - moveInterval);
		
		// Display interests
		hiddenObjectsRoot.SetActive(false);
		
		if (ShowDialogue(DialogueType.INTEREST) == false)
		{
			//Debug.Log("Based on the objects you found, what do you think "+studentNames[currLevel]+"'s interests are?");
			Debug.Log("Based on the objects you found, what do you think their interests are?");
		}
		
		interestOptionsRoot.SetActive(true);
		
		questionIndex = Random.Range(0, currentLevelCharacter.Count - 1);
		
		SetCharacterInterestSlot();
		oddObjectsList[0].SetTexture(currentLevelCharacter[0].texture.texture.mainTexture, Color.white);
		
		int startingIndex = 0;
		interests[0].Enable(GetInterest(currentLevelCharacter[questionIndex].topicName, ref startingIndex));
		startingIndex++;
		interests[1].Enable(GetInterest(currentLevelCharacter[questionIndex].topicName, ref startingIndex));
		interests[2].Enable(currentLevelCharacter[questionIndex].topicName);
		
		tutorialHand.isPointing(false);
		
		Respond(tutorial4);
		tutorialHand.nextWayPoint();
		tutorialHand.moveInterval = tutorialHandFinalInterval;
		yield return new WaitForSeconds(moveInterval);
		tutorialHand.nextWayPoint();
		yield return new WaitForSeconds(moveInterval);
		tutorialHand.nextWayPoint();
		yield return new WaitForSeconds(moveInterval);
		
		yield return new WaitForSeconds(3.75f);
		tutorialHand.isPointing(true);
		
		PlayParticleAnimation(interests[2].transform.position);
		yield return new WaitForSeconds(tutorial4.voiceOver.length - moveInterval * 2);
				
		tutorialHand.gameObject.SetActive(false);
		tutorialCharacter.ResetCharacter(true);
		
		InitializeFirstLevel();
	}
	
	void nextLevel()
	{
		// Hide question
		interestOptionsRoot.SetActive(false);
		// Reset the characters
		foreach(HiddenObjectCharacter character in currentLevelCharacter)
			character.UnloadCharacter();
		currentLevelCharacter.Clear();
		// Increase level
		currentLevel ++;
		
		// Show dialogues from Sherlock
		if(currentLevel == currentDifficulty.levels.Count)
		{
			ResetCharacterSlots();
			
			if (ShowDialogue(DialogueType.END) == false)
			{
				Debug.Log("Great job! Now Nancy has many topics of conversation for her new classmates. She can talk about their interests.");
			}

			// Get the time it takes to speak all the dialogues in "end"
			// The Addition to the dialogue time's value can be found in sherlock.PlaySequenceInstructions
			float dialogueTime = 0f;
			Dialogue dialoguePointer = end;
			while(dialoguePointer != null) {
				if(dialoguePointer.voiceOver != null)
					dialogueTime += (dialoguePointer.voiceOver.length + dialoguePointer.text.Length * 0.1f * 0.2f);
				dialoguePointer = dialoguePointer.nextDialogue;
			}
			dialogueTime += 1f;
			Debug.Log (dialogueTime);
			minigame.Invoke("CompleteIfStandalone", dialogueTime);
		
			return;
		}
		
		if (currentLevel == 0)
		{
			if (ShowDialogue(DialogueType.START) == false)
			{
				Debug.Log("This is Julia, Nancy's classmate. Find the three hidden objects for Julia that relate to her interests.");
			}
		}
		else
		{
			if (ShowDialogue(DialogueType.START_NEXT_ROUND) == false)
			{
				Debug.Log("This is Amber, Nancy's classmate. Find the three hidden objects on or around Amber that relate to her interests.");
			}
		}
		
		itemsLeft = currentDifficulty.levels[currentLevel].noEasyObjects + currentDifficulty.levels[currentLevel].noHardObjects;
		hiddenObjectsRoot.SetActive(true);
		
		List<UIScaledTexture> activeTextures = new List<UIScaledTexture>();
		AssignCharacterSlots(activeTextures, currentDifficulty.levels[currentLevel].noCharacters);
		
		for (int i = 0, noCharacters = currentDifficulty.levels[currentLevel].noCharacters ; i < noCharacters ; i++)
		{
			HiddenObjectCharacter newCharacter = GetRandomCharacter(activeTextures[i]);
			currentLevelCharacter.Add(newCharacter);
		}
		
		int characterIndex = 0;
		int noObjects = currentDifficulty.levels[currentLevel].noEasyObjects;
		List<HiddenObject> currentLevelObjects = new List<HiddenObject>();
		for (int j = 0 ; j < noObjects ; j++)
		{
			HiddenObject hiddenObject = null;
			
			do{
				hiddenObject = currentLevelCharacter[characterIndex].GetRandomObject(HiddenObject.ObjectDifficulty.EASY);
				characterIndex = (characterIndex+1) % currentLevelCharacter.Count;
			}while(hiddenObject == null);
			hiddenObject.SetCollider(true);
			currentLevelObjects.Add(hiddenObject);
		}
		
		characterIndex = 0;
		noObjects = currentDifficulty.levels[currentLevel].noHardObjects;
		for (int j = 0 ; j < noObjects ; j++)
		{
			HiddenObject hiddenObject = null;
			
			do{
				hiddenObject = currentLevelCharacter[characterIndex].GetRandomObject(HiddenObject.ObjectDifficulty.HARD);
				characterIndex = (characterIndex+1) % currentLevelCharacter.Count;
			}while(hiddenObject == null);
			currentLevelObjects.Add(hiddenObject);	
		}
		
		DisplayObjects(currentLevelObjects);
	}
	
	void ResetDisplayObjects()
	{
		foreach(DisplayedObject hiddenObject in hiddenObjectsList)
		{
			hiddenObject.manager = this;
			hiddenObject.sprite.Disable();
			hiddenObject.tick.enabled = false;
			hiddenObject.SetObject(null);
		}
	}
		
	void DisplayObjects(List<HiddenObject> currentLevelObjects)
	{
		ResetDisplayObjects();
		
		ShuffleObjects(currentLevelObjects);
		
		if (currentLevelObjects.Count == 1)
		{
			hiddenObjectsList[3].sprite.Enable();
			hiddenObjectsList[3].sprite.SetSprite(currentLevelObjects[0].GetAtlas(), currentLevelObjects[0].filename, Color.white);
			hiddenObjectsList[3].SetObject(currentLevelObjects[0]);
			currentLevelObjects[0].SetSlot(hiddenObjectsList[3]);
		}
		else if (currentLevelObjects.Count == 3)
		{
			for (int i = 0, j = 1 ; i < currentLevelObjects.Count ; i++, j+=2)
			{
				hiddenObjectsList[j].sprite.Enable();
				hiddenObjectsList[j].sprite.SetSprite(currentLevelObjects[i].GetAtlas(), currentLevelObjects[i].filename, Color.white);
				hiddenObjectsList[j].SetObject(currentLevelObjects[i]);
				currentLevelObjects[i].SetSlot(hiddenObjectsList[j]);
			}
		}
		else if(currentLevelObjects.Count == 5)
		{
			for (int i = 0, j = 1 ; i < currentLevelObjects.Count ; i++, j++)
			{
				hiddenObjectsList[j].sprite.Enable();
				hiddenObjectsList[j].sprite.SetSprite(currentLevelObjects[i].GetAtlas(), currentLevelObjects[i].filename, Color.white);
				hiddenObjectsList[j].SetObject(currentLevelObjects[i]);
				currentLevelObjects[i].SetSlot(hiddenObjectsList[j]);
			}
		}
		else
		{
			for (int i = 0, j = 0 ; i < currentLevelObjects.Count ; i++, j++)
			{
				hiddenObjectsList[j].sprite.Enable();
				hiddenObjectsList[j].sprite.SetSprite(currentLevelObjects[i].GetAtlas(), currentLevelObjects[i].filename, Color.white);
				hiddenObjectsList[j].SetObject(currentLevelObjects[i]);
				currentLevelObjects[i].SetSlot(hiddenObjectsList[j]);
			}
		}
	}
	
	void ResetCharacterSlots()
	{
		foreach(UIScaledTexture scaledTexture in oddObjectsList)
			scaledTexture.Disable();
		foreach(UIScaledTexture scaledTexture in evenObjectsList)
			scaledTexture.Disable();
	}
	
	void AssignCharacterSlots(List<UIScaledTexture> activeList, int activeCharacters)
	{
		foreach(UIScaledTexture scaledTexture in oddObjectsList)
			scaledTexture.Disable();
		foreach(UIScaledTexture scaledTexture in evenObjectsList)
			scaledTexture.Disable();
		
		if (activeCharacters == 1)
		{
			oddObjectsList[1].Enable();
			activeList.Add(oddObjectsList[1]);
		}
		else if (activeCharacters == 2)
		{
			evenObjectsList[0].Enable();
			evenObjectsList[1].Enable();
			activeList.Add(evenObjectsList[0]);
			activeList.Add(evenObjectsList[1]);
		}
		else
		{
			oddObjectsList[0].Enable();
			oddObjectsList[1].Enable();
			oddObjectsList[2].Enable();
			activeList.Add(oddObjectsList[0]);
			activeList.Add(oddObjectsList[1]);
			activeList.Add(oddObjectsList[2]);
		}
	}
	
	void SetCharacterInterestSlot()
	{
		foreach(UIScaledTexture scaledTexture in oddObjectsList)
			scaledTexture.Disable();
		foreach(UIScaledTexture scaledTexture in evenObjectsList)
			scaledTexture.Disable();
		
		oddObjectsList[0].Enable();
	}
	
	HiddenObjectCharacter GetRandomCharacter(UIScaledTexture slot)
	{
		for (int i = 0 ; i < characters.Count ; i++)
		{
			if (characters[i].selected == false && characters[i].used == false)
			{
				characters[i].texture.SetComponents(slot.texture, slot.stretch);
				characters[i].LoadCharacter();
				characters[i].transform.position = slot.texture.transform.position;
				return characters[i];
			}
		}
		
		for (int i = 0 ; i < characters.Count ; i++)
		{
			characters[i].ResetCharacter(false);
		}
		
		ShuffleCharacters();
		
		for (int i = 0 ; i < characters.Count ; i++)
		{
			if (characters[i].selected == false && characters[i].used == false)
			{
				characters[i].texture.SetComponents(slot.texture, slot.stretch);
				characters[i].LoadCharacter();
				characters[i].transform.position = slot.texture.transform.position;
				return characters[i];
			}
		}
		
		return null;
	}
	
	void ShuffleCharacters()  
	{  
	    Random rng = new Random();  
	    int n = characters.Count;	
    	for (int i = 1 ; i < n ; i++)
		{
			int k = Random.Range(0, i);
			
			if (i != k)
			{
		        HiddenObjectCharacter value = characters[k];
		        characters[k] = characters[i];  
		        characters[i] = value; 
			}
		}  
	}
	
	void ShuffleObjects(List<HiddenObject> currentLevelObjects)  
	{  
	    Random rng = new Random();  
	    int n = currentLevelObjects.Count;	
    	for (int i = 1 ; i < n ; i++)
		{
			int k = Random.Range(0, i);
			
			if (i != k)
			{
		        HiddenObject value = currentLevelObjects[k];
		        currentLevelObjects[k] = currentLevelObjects[i];  
		        currentLevelObjects[i] = value; 
			}
		}  
	}
	
	public void PlayParticleAnimation(Vector3 position)
	{
		GameObject starParticle = GameObject.Instantiate(starParticlesPrefab) as GameObject;
		position.z -= 1f;
		starParticle.transform.position = position;
	}
	
	public void HiddenObjectFound(Vector3 objectPosition)
	{
		AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
		
		PlayParticleAnimation(objectPosition);
		
		itemsLeft--;
		if (itemsLeft > 0)
		{
			if (ShowDialogue(DialogueType.CORRECT) == false)
			{
				//Debug.Log("Great! You found "+studentNames[currLevel]+"'s "+ objName +"! Let's find another hidden object!");
				Debug.Log ("Great! You found a hidden object! Keep going!");
			}
		}
		else
		{
			Sherlock.Instance.PlaySequenceInstructions(dialogueFoundAllHidden, chooseInterest);
		}
	}
	
	public void ObjNotFound()
	{
		AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
		
		if (ShowDialogue(DialogueType.INCORRECT) == false)
		{
			Debug.Log("I don't see any hidden objects there. Let's try again.");
		}
	}

	public void InterestSelected (Interest interest)
	{
		if (currentLevelCharacter[questionIndex].topicName == interest.topic)
		{
			foreach(Interest i in interests)
				i.gameObject.GetComponent<BoxCollider>().enabled = false;
			interestCorrect(interest.transform.position);
		}
		else
		{
			interestIncorrect(interest);
		}
	}
	
	public void interestCorrect(Vector3 position)
	{
		AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
		
		PlayParticleAnimation(position);
		
		switch (currentLevel)
		{
		case 0:
			if (ShowDialogue(DialogueType.JULIAINTEREST) == false)	
			{
				Debug.Log("Nancy can talk to Julia about Tennis! Great job!");
			}
			break;
		case 1:
			if (ShowDialogue(DialogueType.AMBERINTEREST) == false)	
			{
				Debug.Log("Nancy can talk to Amber about Space! Great job!");
			}
			break;
			
		}
		
		Invoke ("nextLevel", minigame.GetCurrentDialogueDuration());
	}
	
	public void interestIncorrect(Interest interest)
	{
		AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
		
		interest.Disable();
		
		if (ShowDialogue(DialogueType.INCORRECTINTEREST) == false)
		{
			Debug.Log("I don't think they liked that. Let's look again. Remember to use your eyes to figure out what their interests are.");
		}
		
		Invoke ("returnInterests", minigame.GetCurrentDialogueDuration());
	}
	
	void returnInterests()
	{
		if (ShowDialogue(DialogueType.INTEREST) == false)
		{
			//Debug.Log("Based on the objects you found, what do you think "+studentNames[currLevel]+"'s interests are?");
			Debug.Log("Based on the objects you found, what do you think their interests are?");
		}
	}
	
	
	void chooseInterest()
	{		
		hiddenObjectsRoot.SetActive(false);
		
		if (ShowDialogue(DialogueType.INTERESTS_QUESTION) == false)
		{
			//Debug.Log("Based on the objects you found, what do you think "+studentNames[currLevel]+"'s interests are?");
			Debug.Log("Based on the objects you found, what do you think their interests are?");
		}
		
		interestOptionsRoot.SetActive(true);
		
		questionIndex = Random.Range(0, currentLevelCharacter.Count - 1);
		
		int displayIndex = Random.Range(0, 2);
		SetCharacterInterestSlot();
		oddObjectsList[0].SetTexture(currentLevelCharacter[questionIndex].texture.texture.mainTexture, Color.white);
		
		int startingIndex = 0;
		interests[displayIndex].Enable(currentLevelCharacter[questionIndex].topicName);
		interests[(displayIndex+1) % 3].Enable(GetInterest(currentLevelCharacter[questionIndex].topicName, ref startingIndex));
		startingIndex++;
		interests[(displayIndex+2) % 3].Enable(GetInterest(currentLevelCharacter[questionIndex].topicName, ref startingIndex));
	}	
			
	string GetInterest(string usedTopic, ref int startingIndex)
	{
		for (int i = 0 ; i < characters.Count ; i++)
		{
			string newTopic = characters[(i + startingIndex) % characters.Count].topicName;
			
			if (newTopic != usedTopic)
			{
				startingIndex = i;
				return newTopic; 
			}
		}
		return string.Empty;
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
			case DialogueType.START:
				return minigame.ContinueConversation(start);
			case DialogueType.CORRECT:
				return minigame.ContinueConversation(correct);
			case DialogueType.INCORRECT:
				return minigame.ContinueConversation(incorrect);
			case DialogueType.INTEREST:
				return minigame.ContinueConversation(interest);
			case DialogueType.JULIAINTEREST:
				return minigame.ContinueConversation(juliainterest);
			case DialogueType.INCORRECTINTEREST:
				minigame.ContinueConversation(incorrectinterest);
				return true;
			case DialogueType.START_NEXT_ROUND:
				return minigame.ContinueConversation(amber);
			case DialogueType.AMBERINTEREST:
				return minigame.ContinueConversation(amberinterest);
			case DialogueType.FOUND:
				return minigame.ContinueConversation(found);
			case DialogueType.END:
				Sherlock.Instance.PlaySequenceInstructions(end, null);//minigame.ContinueConversation(end);
				return true;
			case DialogueType.INTERESTS_QUESTION:
				return minigame.ContinueConversation(interestsQuestion);
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
	
}
