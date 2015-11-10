using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockManager : MinigameManager{
	
	public int currLevel; 
	
	public int numLevels;

	private int locksSet = 0; //the number of locks that have been set. This is used to determine when remaining keys should be set to random when
						 // levels  include more than one lock.

	public int getLocksSet() {return locksSet;}
	public void incrementLocksSet() { locksSet++;}

	int keysCorrect = 0;
	
	public GameObject[] myLevels;

	GameObject tutorialHand;
	GameObject tutorialObject;
	public GameObject tutorialOptions;
	
	// Associated minigame
	public Minigame minigame;

	public GameObject cage;
	public int moveInterval;

	GameObject currentLock;

	public Dialogue tutorial;
	public Dialogue end;
	public Dialogue ClearDialogue;
	public Dialogue Level1;
	public Dialogue Level2;
	public Dialogue Level3;

	public Dialogue[] wrong;
	public Dialogue[] turnKeyReplies; //these play to indicate the user needs to turn the key
	public Dialogue[] correctKeyReplies; //these play when user selects correct key
	public Dialogue[] successReplies; //these play when user turns the key successfully
	public Dialogue[] tutorialLine; //these play during the tutorial
	int correctKeyIterator = 0;
	int turnKeyIterator = 0;
    int _successIterator = 0;
	int successIterator {
        get { return _successIterator;}
        set { 
            _successIterator = value;
            if (_successIterator == successReplies.Length)
            {
                _successIterator = 0;
            }
        }
    }

	public bool showTutorial;

	public GameObject prefabLock;
	public GameObject prefabKey;
	public GameObject prefabTurningAsset;

	public int keyLeftPosition;
	public int keySpacing;
	public int padlockCenterPosition;
	public int padlockSpacing;
	public int turnAssetsCenterPosition;
	public int turnAssetsSpacing;

	GameObject[] keys = new GameObject[7];
	public GameObject key;
	public GameObject hardKey;
	GameObject[] padlocks = new GameObject[3]; 
	public GameObject padlock;
	public GameObject hardPadlock;
	GameObject[] turnAssets = new GameObject[3];
	public GameObject turnAsset;

	[System.Serializable]
	public class Level
	{
		public bool differentColors;
		public int numberOfKeys;
		public int numberOfLocks;
		public int numberOfShapesPerKey;
	}
	
	[System.Serializable]
	public class DifficultySession
	{
		public MinigameDifficulty.Difficulty difficulty;
		public List<Level> levels;
	}

	public List<DifficultySession> difficultySessions;

	DifficultySession currentDifficultySession;

	[System.NonSerialized]
	public List<List<int>> symbolsCombinationList;
	
	// Use this for initialization
	void Start () {		


		// Find assocciated minigame
		minigame = GetComponent<Minigame>();

		tutorialHand = GameObject.Find ("TutorialHand");
		tutorialHand.SetActive(false);

		currLevel = 0;
		foreach(DifficultySession ds in difficultySessions) {
			if(ds.difficulty == minigame.difficulty) {
				currentDifficultySession = ds;
				break;
			}
		}



		setupGame();

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
			if(!showTutorial)
			{
				setupLevel();
				disableAllKeys();
			}
			
			// Show the instructions of the minigame
			if (minigame != null && minigame.StartConversation() == false)
			{
				Debug.Log("S: Let's get this lock open!");
			}
			
			Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		}

		if (minigame.minigameEndEvent != null)
			minigame.minigameEndEvent.location = null;

	}

	public override void yesTutorial ()
	{
		Destroy(tutorialOptions);
		showTutorial = true;

		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: Let's get this lock open!");
		}
		
		Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
	}

	public override void noTutorial()
	{
		Destroy(tutorialOptions);
		showTutorial = false;

		setupLevel();
		disableAllKeys();

		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: Let's get this lock open!");
		}
		
		Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
	}

	void setCombinationsOfSymbols(int numOfCombinations, int numSymbolsPerCombinations) {
		List<int> symbolList = new List<int>();
		for(int i = 0; i < 5; i++) {
			symbolList.Add(i);
		}
		
		symbolsCombinationList = getCombinationsFor(symbolList, numSymbolsPerCombinations);
		
		for(int i = symbolsCombinationList.Count; i < numOfCombinations; i++) {
			symbolsCombinationList.Add (symbolsCombinationList[0]);
		}
		
		shuffleList(symbolsCombinationList);
		
		symbolsCombinationList.RemoveRange(numOfCombinations, symbolsCombinationList.Count - numOfCombinations);
	}

	List<List<int>> getCombinationsFor(List<int> group, int subsetSize) {
		List<List<int>> resultingCombinations = new List<List<int>>();
		int totalSize = group.Count;
		if (subsetSize == 0) {
			resultingCombinations.Add(new List<int>());
		} else if (subsetSize <= totalSize) {
			List<int> remainingElements = new List<int>(group.Count);
			remainingElements.InsertRange(0, group);
			int X = remainingElements[remainingElements.Count - 1];
			remainingElements.RemoveAt(remainingElements.Count - 1);
			
			List<List<int>> combinationsExclusiveX = getCombinationsFor(remainingElements, subsetSize);
			List<List<int>> combinationsInclusiveX = getCombinationsFor(remainingElements, subsetSize-1);

			foreach (List<int> combination in combinationsInclusiveX) {
				combination.Add(X);
			}
			resultingCombinations.AddRange(combinationsExclusiveX);
			resultingCombinations.AddRange(combinationsInclusiveX);
		}
		return resultingCombinations;
	}

	List<List<int>> shuffleList(List<List<int>> list) {
		for(int i = 0; i < list.Count; i++) {
			int randomIndex = Random.Range(0, list.Count - 1);
			List<int> randomIndexObject = list[randomIndex];
			list[randomIndex] = list[i];
			list[i] = randomIndexObject;
		}
		return list;
	}

	void shuffleKeyPlacement(int startingIndex, int endingIndex) {
		for(int i = startingIndex; i <= Mathf.Min(endingIndex, keys.Length - 1); i++) {
			int randomIndex = Random.Range(0, endingIndex);
			GameObject randomIndexObject = (GameObject)keys[randomIndex];
			float randomIndexObjectX = randomIndexObject.transform.localPosition.x;
			float randomIndexObjectY = randomIndexObject.transform.localPosition.y;
			float randomIndexObjectZ = randomIndexObject.transform.localPosition.z;
			randomIndexObject.transform.localPosition = new Vector3(
				((GameObject)keys[i]).transform.localPosition.x,
				((GameObject)keys[i]).transform.localPosition.y,
				((GameObject)keys[i]).transform.localPosition.z);
			randomIndexObject.GetComponent<DraggableObjectKey>().setStartPos();
			((GameObject)keys[i]).transform.localPosition = new Vector3(randomIndexObjectX, randomIndexObjectY, randomIndexObjectZ);
			((GameObject)keys[i]).GetComponent<DraggableObjectKey>().setStartPos();
		}
	}

	void setupGame() {
		// Keys Setup
		for(int i = 0; i < keys.Length; i++) {
			if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD) {
				keys[i] = Instantiate(hardKey) as GameObject;
			} else {
				keys[i] = Instantiate(key) as GameObject;
			}
			keys[i].name = "Key" + i;
			keys[i].SetActive(false);
			keys[i].transform.parent = myLevels[0].transform;
			keys[i].transform.localScale = new Vector3(1.75f, 1.75f, 1);
		}
		// Turn Assets Setup
		for(int i = 0; i < turnAssets.Length; i++) {
			turnAssets[i] = Instantiate(turnAsset) as GameObject;
			turnAssets[i].name = "TurnAssets" + i;
			turnAssets[i].SetActive(false);
			turnAssets[i].transform.parent = myLevels[0].transform;
			turnAssets[i].transform.localScale = new Vector3(1, 1, 1);
		}
		// Padlocks Setup
		for(int i = 0; i < padlocks.Length; i++) {
			if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD) {
				padlocks[i] = Instantiate(hardPadlock) as GameObject;
			} else {
				padlocks[i] = Instantiate(padlock) as GameObject;
			}
			padlocks[i].name = "Padlock" + i;
			padlocks[i].SetActive(false);
			padlocks[i].transform.parent = myLevels[0].transform;
			padlocks[i].transform.localScale = new Vector3(1, 1, 1);
			// DropContainerLock script setup
			if(padlocks[i].GetComponent<DropContainerLock>()) {
				DropContainerLock dropContainerLockScript = padlocks[i].GetComponent<DropContainerLock>();
				// Clear the keys, and repopulate with the current keys
				for(int j = 0; j < 7; j++) {
					dropContainerLockScript.myKeys[j] = keys[j];
				}
				// Setup the variables, some from their corresponding turnAssets
				dropContainerLockScript.mySlotNum = i.ToString();
				dropContainerLockScript.myTurnAssets = turnAssets[i];
				dropContainerLockScript.myTurnKeyHead = turnAssets[i].transform.FindChild("TurningHead").gameObject;
				dropContainerLockScript.myTurnIcon = turnAssets[i].transform.FindChild("TurnIcon").gameObject;
				dropContainerLockScript.myTurnCircle = turnAssets[i].transform.FindChild("TurnCircle").gameObject;
				dropContainerLockScript.manager = this;
			}
		}
	}

	void setupLevel() {
		setCombinationsOfSymbols(currentDifficultySession.levels[currLevel].numberOfKeys,
		                         currentDifficultySession.levels[currLevel].numberOfShapesPerKey);
		// Setup the padlocks
		setupPadlocksForLevel(currentDifficultySession.levels[currLevel].numberOfKeys, currentDifficultySession.levels[currLevel].numberOfLocks);
		// Setup the turnAssets
		setupTurnAssetsForLevel(currentDifficultySession.levels[currLevel].numberOfLocks);
		// Setup the keys
		setupKeysForLevel(currentDifficultySession.levels[currLevel].numberOfKeys, currentDifficultySession.levels[currLevel].numberOfLocks);
	}

	void setupKeysForLevel(int numOfKeys, int numOfPadlocks) {
		for(int i = 0; i < numOfKeys; i++) {
			keys[i].SetActive(true);
			keys[i].GetComponent<DraggableObjectKey>().myX.SetActive(false);
			// Set position and scale
			keys[i].transform.localPosition = new Vector3(keyLeftPosition + keySpacing * (i+ 3 - numOfKeys / 2), -340, -5);
			keys[i].GetComponent<DraggableObjectKey>().setStartPos();
			// Clear the dropspots, and repopulate with the current padlocks
			System.Array.Clear(keys[i].GetComponent<DraggableObjectKey>().dropspots, 0, keys[i].GetComponent<DraggableObjectKey>().dropspots.Length);
			for(int j = 0; j < numOfPadlocks; j++) {
				keys[i].GetComponent<DraggableObjectKey>().dropspots[j] = padlocks[j];
			}
			if(i >= numOfPadlocks)
				keys[i].GetComponent<DraggableObjectKey>().setKey(symbolsCombinationList[i], padlock.GetComponent<DropContainerLock>());
		}
		shuffleKeyPlacement(0, numOfKeys - 1);
	}

	void setupPadlocksForLevel(int numOfKeys, int numOfPadlocks) {
		// Setup X of the first padlock, other locks' position based off of this first padlock
		float startingX = padlockCenterPosition - 0.5f * ((float)numOfPadlocks - 1f) * (float)padlockSpacing;
		for(int i = 0; i < numOfPadlocks; i++) {
			padlocks[i].SetActive(true);
			if(currLevel == 0) {
				padlocks[i].transform.localPosition = new Vector3(startingX + padlockSpacing * (float)i, -320, 115);
			} else {
				padlocks[i].transform.localPosition = new Vector3(startingX + padlockSpacing * (float)i, -920, 115);
			}
			padlocks[i].transform.FindChild("Keyhole").gameObject.SetActive(true);
			if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD) {
				foreach(Transform t in padlocks[i].transform) {
					if(t.gameObject.name.Contains("Symbol")) {
						t.gameObject.GetComponent<UITexture>().enabled = true;
					}
				}
			} else {
				padlocks[i].transform.FindChild("Symbol").gameObject.GetComponent<UITexture>().enabled = true;
			}
			padlocks[i].transform.FindChild("Padlock").localPosition = new Vector3(66, 382, -112);
			// Setup relationship between keys and locks
			padlocks[i].GetComponent<DropContainerLock>().setLock(symbolsCombinationList[i]);
			//padlocks[i].GetComponent<DropContainerLock>().setLockAndKey(i, currentDifficultySession.levels[currLevel].numberOfShapesPerKey);
			keys[i].GetComponent<DraggableObjectKey>().setSolutionKey(padlocks[i].GetComponent<DropContainerLock>());
		}
	}
	
	void setupTurnAssetsForLevel(int numOfTurnAssets) {
		float startingX = turnAssetsCenterPosition - 0.5f * ((float)numOfTurnAssets - 1f) * (float)turnAssetsSpacing;
		for(int i = 0; i < numOfTurnAssets; i++) {
			turnAssets[i].transform.localPosition = new Vector3(startingX + turnAssetsSpacing * i, 35, 0);
			turnAssets[i].SetActive(false);
			turnAssets[i].transform.FindChild("TurnIcon").gameObject.GetComponent<UISprite>().fillAmount = 0.75f;
			GameObject turnCircle = turnAssets[i].transform.FindChild("TurnCircle").FindChild("FingerPos").gameObject;
			if(turnCircle.GetComponent<TurnKey>()) {
				turnCircle.transform.localPosition = new Vector3(-59, 164, 0);
				TurnKey turnKeyScript = turnCircle.GetComponent<TurnKey>();
				turnKeyScript.myLock = padlocks[i];
				turnKeyScript.resetTurnCircle();
			}
		}
	}
	
	void continueDialogue()
	{
		
		if(!minigame.ContinueConversation())
		{
			if(showTutorial) StartCoroutine("startTutorial");
			else startGame();
		}
		else
		{
			Invoke("continueDialogue", minigame.GetCurrentDialogueDuration());
		}
		
	}

	
	/* Method startGame
	 * 
	 * Starts the game.
	 * *
	 * */
	void startGame()
	{
		// Reenable the boxcolliders on the keys/padlocks
		foreach(Transform t in myLevels[0].transform) {
			if(t.gameObject.activeSelf) {
				t.GetComponent<BoxCollider>().enabled = true;
			}
		}
		disableAllKeys();
		/*
		Respond(Level1);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		minigame.ContinueConversation();
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		minigame.ContinueConversation();
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		*/
		enableAllKeys();
	}
	
	IEnumerator startTutorial()
	{
		if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
		{	setCombinationsOfSymbols(1,3); }
		else
		{	setCombinationsOfSymbols(1,1); }

		setupPadlocksForLevel(1,1);

		// Setup the turnAssets
		setupTurnAssetsForLevel(1);
		// Setup the keys
		setupKeysForLevel(1,1);
		disableAllKeys();

		tutorialObject = GameObject.Find ("Key0");

		Respond(tutorialLine[0]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		//move key on screen
		tutorialHand.SetActive(true);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);

		tutorialHand.GetComponent<TutorialHand>().isPointing(true);
		tutorialObject.transform.parent = tutorialHand.transform; //hand picks up the key

		Respond(tutorialLine[1]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		//move key to lock
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().isPointing(false);
		tutorialObject.transform.parent = GameObject.Find("LockLevel0").transform;
		tutorialObject.SetActive(false);

		Respond(tutorialLine[2]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		tutorialObject = GameObject.Find("Padlock0");

		tutorialObject.GetComponent<DropContainerLock>().keyInsert();
		//set turn circle's box collider off
		tutorialObject.GetComponent<DropContainerLock>().myTurnAssets.transform.GetComponentInChildren<BoxCollider>().enabled = false;

		tutorialObject.GetComponent<DropContainerLock>().keyStartTurn();

		Respond(tutorialLine[3]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().isPointing(true);
		yield return new WaitForSeconds(.5f);

		tutorialObject.GetComponent<DropContainerLock>().myTurnAssets.GetComponentInChildren<TurnKey>().OnTutorialPress(true);

		tutorialHand.GetComponent<TutorialHand>().moveInterval = tutorialHand.GetComponent<TutorialHand>().moveInterval/3;
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		//set turn circle's box collider on
		tutorialObject.GetComponent<DropContainerLock>().myTurnAssets.transform.GetComponentInChildren<BoxCollider>().enabled = true;
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);

		//tutorialObject.GetComponent<DropContainerLock>().myTurnAssets.GetComponentInChildren<TurnKey>().OnTutorialPress(false);

		Respond(tutorialLine[4]);

		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		//here, disable all the parts you used in the tutorial
		keysCorrect = 0;
		tutorialHand.SetActive(false);



		setupLevel();
		disableAllKeys();
		
		//StartCoroutine("startGame");
		startGame ();
	}


	
	IEnumerator nextLevel()
	{
		if(currLevel+1 >= numLevels)
		{
			GameObject go = GameObject.Find("BedroomManager");
			
			if(go != null){
				BedroomLevelManager bedManager = go.GetComponent<BedroomLevelManager>();
				bedManager.OpenSherlockCage();
			}
			yield return new WaitForSeconds(3f);
			
			Respond(successReplies[successIterator]);
			successIterator++;
		}
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		
		currLevel++;



		if(currLevel < numLevels)
		{
			setupLevel();
			if(currLevel == 1)
				TweenPosition.Begin(cage, moveInterval, cage.transform.localPosition + new Vector3(0, 600.0f, 0));
			else if(currLevel == 2)
				TweenPosition.Begin(cage, moveInterval, cage.transform.localPosition + new Vector3(0, 630.0f, 0));
			foreach(Transform t in myLevels[0].transform) {
				if(t.gameObject.GetComponent<DropContainerLock>()) {
					TweenPosition.Begin(t.gameObject, moveInterval, t.localPosition + new Vector3(0, 600.0f, 0));
				}
			}
			/*foreach(Transform t in myLevels[currLevel - 1].transform) {
				if(t.gameObject.GetComponent<DropContainerLock>()) {
					TweenPosition.Begin(t.gameObject, moveInterval, t.localPosition + new Vector3(0, 565.0f, 0));
				}
			}*/
			yield return new WaitForSeconds(moveInterval);
		
			disableAllKeys();/*
			foreach(Transform obj in myLevels[0].transform)
			{
				if(obj.GetComponent<DraggableObjectKey>())
				{
					obj.gameObject.SetActive(true);
					obj.GetComponent<DraggableObjectKey>().setStartPos();
					obj.GetComponent<UIDragObject>().enabled = false;
					obj.GetComponent<BoxCollider>().enabled = false;
				}
			}*/

			switch(currLevel)
			{
			case 1:
				Dialogue tempDialogue = Level2;
				float dialogueLength = 0f;
				while(tempDialogue != null) {
					if(tempDialogue.voiceOver != null) {
						dialogueLength += tempDialogue.voiceOver.length;
					} else {
						dialogueLength += 2f;
					}
					tempDialogue = tempDialogue.nextDialogue;
				}
				Invoke("enableAllKeys", dialogueLength);
				Respond(Level2);
				break;
			case 2:
				Dialogue tempDialogue2 = Level2;
				float dialogueLength2 = 0f;
				while(tempDialogue2 != null) {
					if(tempDialogue2.voiceOver != null) {
						dialogueLength2 += tempDialogue2.voiceOver.length;
					} else {
						dialogueLength2 += 2f;
					}
					tempDialogue2 = tempDialogue2.nextDialogue;
				}
				Invoke("enableAllKeys", dialogueLength2);
				Respond(Level3);
				break;
			}

			do
			{
				yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
			}while(minigame.ContinueConversation());


			enableAllKeys();
		}
		else
		{
			EndMinigame1();
		}
		
	}

	public void enableAllKeys() {
		foreach(Transform t in myLevels[0].transform) {
			if(t.gameObject.GetComponent<DraggableObjectKey>()) {
				t.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}

	public void disableAllKeys() {
		foreach(Transform t in myLevels[0].transform) {
			if(t.gameObject.GetComponent<DraggableObjectKey>()) {
				t.gameObject.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	public GameObject[] getAllKeysOfLevel() {
		return keys;
	}

	public void correctKey(GameObject loc)
	{
		currentLock = loc;
		StartCoroutine("correctKeyRespond");

	}

	IEnumerator correctKeyRespond()
	{
		if(correctKeyReplies.Length > correctKeyIterator)
		{
			Respond (correctKeyReplies[correctKeyIterator]);
			correctKeyIterator++;
		}
		currentLock.GetComponent<DropContainerLock>().keyInsert();
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		currentLock.GetComponent<DropContainerLock>().keyStartTurn();
		if(turnKeyReplies.Length >turnKeyIterator)
		{
			Respond (turnKeyReplies[turnKeyIterator]);
			disableAllKeys();
			turnKeyIterator++;
		}
	}

	public void keyTurned(GameObject mLock)
	{
		mLock.SetActive(false);
		
		switch(currLevel)
		{
		case 0:
			Respond(successReplies[successIterator]);
			successIterator++;
			StartCoroutine("nextLevel");
			break;
		case 1:
			Respond(successReplies[successIterator]);
			successIterator++;
			if(keysCorrect == 0)
			{
				keysCorrect++;
				enableAllKeys();
			}
			else
			{
				keysCorrect = 0;
				StartCoroutine("nextLevel");
			}
			break;
		case 2:
			if(keysCorrect == 0)
			{
				Respond(successReplies[successIterator]);
				successIterator++;
				keysCorrect++;
				enableAllKeys();
			}
			else if (keysCorrect == 1)
			{
				Respond(successReplies[successIterator]);
				successIterator++;
				keysCorrect++;
				enableAllKeys();
			}
			else
			{
				keysCorrect = 0;
				StartCoroutine("nextLevel");
			}
			break;

		}
	}

	public void incorrectKey()
	{
		System.Random rng = new System.Random();  
		Respond (wrong[rng.Next(wrong.Length)]);
	}
	
	void EndMinigame1()
	{
		GameObject go = GameObject.Find("BedroomManager");
		
		if(go != null)
		{
			BedroomLevelManager bedManager = go.GetComponent<BedroomLevelManager>();
			bedManager.DadFinalDialogue();
		}
		minigame.Invoke("CompleteMinigame", minigame.GetCurrentDialogueDuration());
		
	}
	
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
	}
}