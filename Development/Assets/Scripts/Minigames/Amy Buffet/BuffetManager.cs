using UnityEngine;
using System.Collections;

/// <summary>
/// Buffet manager that manages food minigame
/// </summary>
public class BuffetManager : MinigameManager {
	
	// Different states a plate can be set to
	public enum DialogueType
	{
		NOT_ON_TRAY,
		WRONG_SPOT,
		CORRECT,
		END
	}
	
	// Associated minigame
	Minigame minigame;
	
	
	// Which is the current level in the serving process
	public int currLevel;
	// Total number of levels in the serving process
	public int numLevels;
	// The objects associated with each level
	GameObject[] myLevels;
	// Parent game objects of all levels (buffet)
	public GameObject buffet;
	// Gameobject that is the target tray
	public GameObject smallTargetTray;
	public GameObject largeTargetTray;
	public GameObject playerTray;
	// Prefab for buffet item
	public GameObject buffetItem;
	// Atlas of foods
	UIAtlas foodsAtlas;
	// Arrays for sprite strings
	ArrayList appetizerItems = new ArrayList();
	ArrayList mainItems = new ArrayList();
	ArrayList dessertItems = new ArrayList();
	// Arrays for hard sprite strings
	ArrayList hardAppetizerItems = new ArrayList();
	ArrayList hardMainItems = new ArrayList();
	ArrayList hardDessertItems = new ArrayList();

	// Game objects related to the game
	GameObject largeSample;
	GameObject smallSample;
	GameObject smallSampleBubble;
	GameObject myTray;
	GameObject endGame;
	GameObject tutorialHand;
	GameObject tutorialBuffet;
	GameObject tutorialObject;

	public GameObject tutorialOptions;
	
	// Distance needed to be moved in order to get to next level
	public Vector3 moveDistace = new Vector3(1010, 0, 0);
	private Vector3 initialPosition;
	private float movementTime;
	public float transitionTime = 1;
	
	// Dialogue when selected plate is not in the final tray
	public Dialogue notOnTray;
	// Dialogue when selected plate is in the final tray but in different position
	public Dialogue wrongSpot;
	// Dialogue is in the final tray and the correct position
	public Dialogue correct;
	// Dialogue at the end of the minigame, after all the levels
	public Dialogue end;
	// Tutorial dialogue:
	public Dialogue tutorial1;
	public Dialogue tutorial2;
	public Dialogue tutorial3;
	public Dialogue tutorial4;
	
	public bool showTutorial;
	
	public UITexture buffetTexture;
	
	public AudioClip rightAnswerFX;
	public AudioClip wrongAnswerFX;
	
	// Use this for initialization
	void Start () {
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		MinigameDifficulty.Difficulty difficulty = minigame.difficulty;
		
		// Initialize levels array
		myLevels = new GameObject[numLevels];

		// Easy or medium assets
		if(minigame.difficulty != MinigameDifficulty.Difficulty.HARD) {

			foodsAtlas = (UIAtlas) Resources.Load ("NPCs/Amy/AmyLargeFoodBuffetAtlas", typeof(UIAtlas));

			// Gets all large foods
			foreach(string s in foodsAtlas.GetListOfSprites()) {
				if(s.Contains("dess")) {
					dessertItems.Add (s);
				} else if(s.Contains("app")) {
					appetizerItems.Add (s);
				} else if(s.Contains("main")) {
					mainItems.Add (s);
				}
			}

			// Randomizes the possible appetizer foods
			for(int appRandomizer = 0; appRandomizer < appetizerItems.Count; appRandomizer++) {
				int randomindex = Random.Range(appRandomizer, appetizerItems.Count - 1);
				string swap = (string)appetizerItems[randomindex];
				appetizerItems[randomindex] = appetizerItems[appRandomizer];
				appetizerItems[appRandomizer] = swap;
			}

			// Randomizes the possible main foods
			for(int mainRandomizer = 0; mainRandomizer < mainItems.Count; mainRandomizer++) {
				int randomindex = Random.Range(mainRandomizer, mainItems.Count - 1);
				string swap = (string)mainItems[randomindex];
				mainItems[randomindex] = mainItems[mainRandomizer];
				mainItems[mainRandomizer] = swap;
			}

			// Randomizes the possible dessert foods
			for(int dessertRandomizer = 0; dessertRandomizer < dessertItems.Count; dessertRandomizer++) {
				int randomindex = Random.Range(dessertRandomizer, dessertItems.Count - 1);
				string swap = (string)dessertItems[randomindex];
				dessertItems[randomindex] = dessertItems[dessertRandomizer];
				dessertItems[dessertRandomizer] = swap;
			}
		}

		// Hard assets
		if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD) {
			
			foodsAtlas = (UIAtlas) Resources.Load ("NPCs/Amy/AmySmallFoodBuffetAtlas", typeof(UIAtlas));
			
			// Gets all large foods
			foreach(string s in foodsAtlas.GetListOfSprites()) {
				if(s.Contains("dess")) {
					dessertItems.Add (s);
				} else if(s.Contains("app")) {
					appetizerItems.Add (s);
				} else if(s.Contains("main")) {
					mainItems.Add (s);
				}
			}
			
			// Randomizes the possible appetizer foods
			for(int appRandomizer = 0; appRandomizer < appetizerItems.Count; appRandomizer++) {
				int randomindex = Random.Range(appRandomizer, appetizerItems.Count - 1);
				string swap = (string)appetizerItems[randomindex];
				appetizerItems[randomindex] = appetizerItems[appRandomizer];
				appetizerItems[appRandomizer] = swap;
			}
			
			// Randomizes the possible main foods
			for(int mainRandomizer = 0; mainRandomizer < mainItems.Count; mainRandomizer++) {
				int randomindex = Random.Range(mainRandomizer, mainItems.Count - 1);
				string swap = (string)mainItems[randomindex];
				mainItems[randomindex] = mainItems[mainRandomizer];
				mainItems[mainRandomizer] = swap;
			}
			
			// Randomizes the possible dessert foods
			for(int dessertRandomizer = 0; dessertRandomizer < dessertItems.Count; dessertRandomizer++) {
				int randomindex = Random.Range(dessertRandomizer, dessertItems.Count - 1);
				string swap = (string)dessertItems[randomindex];
				dessertItems[randomindex] = dessertItems[dessertRandomizer];
				dessertItems[dessertRandomizer] = swap;
			}
		}

		// Get all the foods in the target trays
		ArrayList targetFoodsSmall = new ArrayList();
		foreach(Transform targetFood in smallTargetTray.transform) {
			if(!targetFood.gameObject.name.Contains("Tray")) {
				targetFoodsSmall.Add (targetFood.gameObject);
			}
		}
		ArrayList targetFoodsLarge = new ArrayList();
		foreach(Transform targetFood in largeTargetTray.transform) {
			if(!targetFood.gameObject.name.Contains("Tray")) {
				targetFoodsLarge.Add (targetFood.gameObject);
			}
		}

		// Remove two plates from the target trays
		for(int randomInt = 0; randomInt < 2; randomInt++) {
			int randomIndex = Random.Range(0, targetFoodsSmall.Count - 1);
			GameObject targetFoodToRemove = ((GameObject)targetFoodsSmall[randomIndex]).gameObject;
			targetFoodToRemove.SetActive(false);
			targetFoodsSmall.Remove(targetFoodToRemove);
			GameObject targetFoodToRemoveLarge = largeTargetTray.transform.FindChild(targetFoodToRemove.name).gameObject;
			targetFoodToRemoveLarge.SetActive(false);
			targetFoodsLarge.Remove(targetFoodToRemoveLarge);
		}

		// Get dropspots from player tray
		GameObject[] dropspots = new GameObject[5];
		for(int dropspotsCounterOne = 0; dropspotsCounterOne < 5; dropspotsCounterOne++)
			dropspots[dropspotsCounterOne] = playerTray.transform.FindChild("Slot" + (dropspotsCounterOne + 1).ToString()).gameObject;
		
		// Get all levels in the scene
		int i = 0;
		moveDistace.x = buffetTexture.transform.localScale.x;
		while(i < myLevels.Length)
		{
			Transform t = buffet.transform.FindChild("BuffetLevel" + i.ToString());
			if (t != null)
			{
				myLevels[i] = t.gameObject;
				
				// Get all food items in the current level
				ArrayList foods = new ArrayList();

				// Easy difficulty, 4 items
				if(difficulty == MinigameDifficulty.Difficulty.EASY) {
					for(int easyCounter = 0; easyCounter < 4; easyCounter++) {
						GameObject newFood = (GameObject) Instantiate(buffetItem);
						newFood.transform.parent = myLevels[i].transform;
						newFood.transform.localPosition = new Vector3(-250 + easyCounter * 90, 290 - easyCounter % 2 * 200, 10);
						newFood.transform.localScale = new Vector3(2, 2, 2);
						newFood.transform.FindChild("Buffet_ItemHard1").gameObject.SetActive(false);
						newFood.transform.FindChild("Buffet_ItemHard2").gameObject.SetActive(false);
						GameObject newFoodItem = newFood.transform.FindChild("Buffet_ItemEasy").gameObject;
						newFoodItem.GetComponent<UISprite>().atlas = foodsAtlas;
						for(int dropspotsCounter = 0; dropspotsCounter < 5; dropspotsCounter++)
							newFood.GetComponent<DraggableObjectBuffet>().dropspots[dropspotsCounter] = dropspots[dropspotsCounter];
						switch(i) {
						case 0:
							newFoodItem.GetComponent<UISprite>().spriteName = (string)appetizerItems[easyCounter];
							break;
						case 1:
							newFoodItem.GetComponent<UISprite>().spriteName = (string)mainItems[easyCounter];
							break;
						case 2:
							newFoodItem.GetComponent<UISprite>().spriteName = (string)dessertItems[easyCounter];
							break;
						default:
							newFood.GetComponent<UISprite>().spriteName = (string)appetizerItems[easyCounter];
							break;
						}
						foods.Add(newFood);
					}
				}

				// Medium difficulty, 8 items
				if(difficulty == MinigameDifficulty.Difficulty.MEDIUM) {
					for(int mediumCounter = 0; mediumCounter < 8; mediumCounter++) {
						GameObject newFood = (GameObject) Instantiate(buffetItem);
						newFood.transform.parent = myLevels[i].transform;
						newFood.transform.localPosition = new Vector3(-370 + mediumCounter * 77 - (mediumCounter % 2)*30, 260 - mediumCounter % 2 * 180, 10);
						newFood.transform.localScale = new Vector3(2, 2, 2);
						newFood.transform.FindChild("Buffet_ItemHard1").gameObject.SetActive(false);
						newFood.transform.FindChild("Buffet_ItemHard2").gameObject.SetActive(false);
						GameObject newFoodItem = newFood.transform.FindChild("Buffet_ItemEasy").gameObject;
						newFoodItem.GetComponent<UISprite>().atlas = foodsAtlas;
						for(int dropspotsCounter = 0; dropspotsCounter < 5; dropspotsCounter++)
							newFood.GetComponent<DraggableObjectBuffet>().dropspots[dropspotsCounter] = dropspots[dropspotsCounter];
						switch(i) {
						case 0:
							newFoodItem.GetComponent<UISprite>().spriteName = (string)appetizerItems[mediumCounter];
							break;
						case 1:
							newFoodItem.GetComponent<UISprite>().spriteName = (string)mainItems[mediumCounter];
							break;
						case 2:
							newFoodItem.GetComponent<UISprite>().spriteName = (string)dessertItems[mediumCounter];
							break;
						default:
							newFood.GetComponent<UISprite>().spriteName = (string)appetizerItems[mediumCounter];
							break;
						}
						foods.Add(newFood);
					}
				}

				// Hard difficulty, 8 items split
				if(difficulty == MinigameDifficulty.Difficulty.HARD) {
					for(int hardCounter = 0; hardCounter < 8; hardCounter++) {
						GameObject newFood = (GameObject) Instantiate(buffetItem);
						newFood.transform.parent = myLevels[i].transform;
						newFood.transform.localPosition = new Vector3(-370 + hardCounter * 77 - (hardCounter % 2)*30, 260 - hardCounter % 2 * 180, 10);
						newFood.transform.localScale = new Vector3(2, 2, 2);
						newFood.transform.localEulerAngles = new Vector3(0, 0, 30);
						newFood.transform.FindChild("Buffet_ItemEasy").gameObject.SetActive(false);
						GameObject newFoodItem1 = newFood.transform.FindChild("Buffet_ItemHard1").gameObject;
						newFoodItem1.GetComponent<UISprite>().atlas = foodsAtlas;
						GameObject newFoodItem2 = newFood.transform.FindChild("Buffet_ItemHard2").gameObject;
						newFoodItem2.GetComponent<UISprite>().atlas = foodsAtlas;
						for(int dropspotsCounter = 0; dropspotsCounter < 5; dropspotsCounter++)
							newFood.GetComponent<DraggableObjectBuffet>().dropspots[dropspotsCounter] = dropspots[dropspotsCounter];
						int nextHardCounter = hardCounter + 1;
						if(nextHardCounter >= 8)
							nextHardCounter = 0;
						switch(i) {
						case 0:
							newFoodItem1.GetComponent<UISprite>().spriteName = (string)appetizerItems[hardCounter];
							newFoodItem2.GetComponent<UISprite>().spriteName = (string)appetizerItems[nextHardCounter];
							break;
						case 1:
							newFoodItem1.GetComponent<UISprite>().spriteName = (string)mainItems[hardCounter];
							newFoodItem2.GetComponent<UISprite>().spriteName = (string)mainItems[nextHardCounter];
							break;
						case 2:
							newFoodItem1.GetComponent<UISprite>().spriteName = (string)dessertItems[hardCounter];
							newFoodItem2.GetComponent<UISprite>().spriteName = (string)dessertItems[nextHardCounter];
							break;
						default:
							newFood.GetComponent<UISprite>().spriteName = (string)appetizerItems[hardCounter];
							newFoodItem2.GetComponent<UISprite>().spriteName = (string)appetizerItems[nextHardCounter];
							break;
						}
						foods.Add(newFood);
					}
				}
				
				// Randomize the order of items in the buffet
				for(int foodsCounter = 0; foodsCounter < foods.Count; foodsCounter++) {
					int randomindex = Random.Range(foodsCounter, foods.Count - 1);
					// Swap positions
					float x = ((GameObject)foods[randomindex]).transform.position.x;
					float y = ((GameObject)foods[randomindex]).transform.position.y;
					((GameObject)foods[randomindex]).transform.position = new Vector3(
						((GameObject)foods[foodsCounter]).transform.position.x,
						((GameObject)foods[foodsCounter]).transform.position.y,
						((GameObject)foods[foodsCounter]).transform.position.z);
					((GameObject)foods[foodsCounter]).transform.position = new Vector3(x, y, ((GameObject)foods[foodsCounter]).transform.position.z);
				}

				// Make one item the target food
				GameObject correctFood = (GameObject)foods[Random.Range(0, foods.Count - 1)];
				GameObject correctFoodEasy = correctFood.transform.FindChild("Buffet_ItemEasy").gameObject;
				GameObject correctFoodHard1 = correctFood.transform.FindChild("Buffet_ItemHard1").gameObject;
				GameObject correctFoodHard2 = correctFood.transform.FindChild("Buffet_ItemHard2").gameObject;
				// Update the small and large food tray's plate for easy and medium
				if(difficulty != MinigameDifficulty.Difficulty.HARD) {
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemHard1").gameObject.SetActive(false);
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemHard2").gameObject.SetActive(false);
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemEasy").gameObject.GetComponent<UISprite>().atlas = foodsAtlas;
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemEasy").gameObject.GetComponent<UISprite>().spriteName =
						correctFoodEasy.GetComponent<UISprite>().spriteName;
					// Find and update the large food tray's plate
					foreach(GameObject g in targetFoodsLarge) {
						if(g.name == ((GameObject)targetFoodsSmall[i]).gameObject.name) {
							g.transform.FindChild("Buffet_ItemHard1").gameObject.SetActive(false);
							g.transform.FindChild("Buffet_ItemHard2").gameObject.SetActive(false);
							g.transform.FindChild("Buffet_ItemEasy").gameObject.GetComponent<UISprite>().atlas = foodsAtlas;
							g.transform.FindChild("Buffet_ItemEasy").GetComponent<UISprite>().spriteName = correctFoodEasy.GetComponent<UISprite>().spriteName;
							break;
						}
					}
				} else if(difficulty == MinigameDifficulty.Difficulty.HARD) {
					((GameObject)targetFoodsSmall[i]).transform.localEulerAngles = new Vector3(0, 0, 30);
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemEasy").gameObject.SetActive(false);
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemHard1").gameObject.GetComponent<UISprite>().atlas = foodsAtlas;
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemHard1").gameObject.GetComponent<UISprite>().spriteName =
						correctFoodHard1.GetComponent<UISprite>().spriteName;
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemHard2").gameObject.GetComponent<UISprite>().atlas = foodsAtlas;
					((GameObject)targetFoodsSmall[i]).transform.FindChild("Buffet_ItemHard2").gameObject.GetComponent<UISprite>().spriteName =
						correctFoodHard2.GetComponent<UISprite>().spriteName;
					// Find and update the large food tray's plate
					foreach(GameObject g in targetFoodsLarge) {
						if(g.name == ((GameObject)targetFoodsSmall[i]).gameObject.name) {
							g.transform.localEulerAngles = new Vector3(0, 0, 30);
							g.transform.FindChild("Buffet_ItemEasy").gameObject.SetActive(false);
							g.transform.FindChild("Buffet_ItemHard1").GetComponent<UISprite>().atlas = foodsAtlas;
							g.transform.FindChild("Buffet_ItemHard1").GetComponent<UISprite>().spriteName = correctFoodHard1.GetComponent<UISprite>().spriteName;
							g.transform.FindChild("Buffet_ItemHard2").GetComponent<UISprite>().atlas = foodsAtlas;
							g.transform.FindChild("Buffet_ItemHard2").GetComponent<UISprite>().spriteName = correctFoodHard2.GetComponent<UISprite>().spriteName;
							break;
						}
					}
				}
				correctFood.GetComponent<DraggableObjectBuffet>().isSolution = true;
				correctFood.GetComponent<DraggableObjectBuffet>().correctSlot = ((GameObject)targetFoodsSmall[i]).name.Replace("Spot", "");

				myLevels[i].transform.localPosition = moveDistace * i;
			}
			else
				break;
			
			i++;			
		}
		
		// Find all game objects related the minigame
		largeSample = GameObject.Find("SampleTrayLarge");
		smallSample = GameObject.Find("SampleTraySmall");
		smallSample.SetActive(false);
		smallSampleBubble = GameObject.Find("SampleTrayBubble");
		smallSampleBubble.SetActive(false);
		myTray = GameObject.Find ("PlayerTray");
		myTray.SetActive(false);
		endGame = GameObject.Find ("EndBackground");
		endGame.SetActive(false);
		buffet.SetActive(false);
		tutorialHand = GameObject.Find("TutorialHand");
		tutorialHand.SetActive(false);
		tutorialBuffet = GameObject.Find ("TutorialBuffet");
		tutorialObject = GameObject.Find ("TutorialObject");
		tutorialBuffet.SetActive(false);
		tutorialObject.SetActive(false);

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
			if(showTutorial)
			{
				largeSample.SetActive(false);
				myTray.SetActive(true);
				tutorialBuffet.SetActive(true);
				StartCoroutine("startTutorial");
			}
			else
			{
				// Show the instructions of the minigame
				if (minigame != null && minigame.StartConversation() == false)
				{
					Debug.Log("S: This is Amy's original order. She had a garden salad, burger and strawberries. We must remake Amy's lunch exactly as it appears, so choose the RIGHT item, and place it in the RIGHT location.");
				}
				
				Invoke ("LayBuffet", minigame.GetCurrentDialogueDuration());
			}
		}
	}

	public override void yesTutorial ()
	{
		Destroy(tutorialOptions);
		largeSample.SetActive(false);
		myTray.SetActive(true);
		tutorialBuffet.SetActive(true);
		StartCoroutine("startTutorial");
	}

	public override void noTutorial ()
	{
		Destroy(tutorialOptions);
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: This is Amy's original order. She had a garden salad, burger and strawberries. We must remake Amy's lunch exactly as it appears, so choose the RIGHT item, and place it in the RIGHT location.");
		}
		
		Invoke ("LayBuffet", minigame.GetCurrentDialogueDuration());
	}

	public void DisableFoodColliders() {
		foreach(Transform t in myLevels[currLevel].transform) {
			if(t.GetComponent<BoxCollider>()) {
				t.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	public void EnableFoodColliders() {
		foreach(Transform t in myLevels[currLevel].transform) {
			if(t.GetComponent<BoxCollider>()) {
				t.GetComponent<BoxCollider>().enabled = true;
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
	
	/// <summary>
	/// Lays the buffet.
	/// </summary>
	public void LayBuffet()
	{
		// Set current level to 0
		currLevel = 0;
		
		// Show the buffet and reset it's position		
		buffet.transform.localPosition = Vector3.zero;
		buffet.SetActive(true);
		
		smallSampleBubble.SetActive(true);
		smallSample.SetActive(true);
		largeSample.SetActive(false);
		myTray.SetActive(true);
		
		// Update the position of the draggable objects in the buffet
		DraggableObjectBuffet currentObject;
		foreach(Transform child in myLevels[currLevel].transform)
		{
			currentObject = child.GetComponent<DraggableObjectBuffet>();
			if(currentObject != null)
			{
				currentObject.collider.enabled = true;
				currentObject.setStartPos();
			}
		}
	}
	
	/// <summary>
	/// Move the tray on the buffet counter (by moving the buffet)
	/// </summary>
	public void MoveBuffet()
	{
		// Change the buffet position
		movementTime += 0.01f;
		buffet.transform.localPosition = Vector3.Lerp(initialPosition, -moveDistace * currLevel, movementTime / transitionTime);
		// buffet.transform.localPosition = new Vector3(buffet.transform.localPosition.x - 0.1f, buffet.transform.localPosition.y, buffet.transform.localPosition.z);

		// If the buffet has reached the target position
		if (buffet.transform.localPosition.x <= (-moveDistace.x * currLevel + float.Epsilon))
		{
			buffet.transform.localPosition = -moveDistace * currLevel;
			// Update the position of the draggable objects in the buffet
			DraggableObjectBuffet currentObject;
			foreach(Transform child in myLevels[currLevel].transform)
			{
				currentObject = child.GetComponent<DraggableObjectBuffet>();
				if(currentObject != null)
				{
					currentObject.collider.enabled = true;
					currentObject.setStartPos();
				}
			}
			
			// Stop moving the buffet
			CancelInvoke("MoveBuffet");
		}
	}
	
	/// <summary>
	/// Increments the level of the buffet
	/// </summary>
	public void IncrementLevel()
	{
		
		
		// Increase current level
		currLevel++;
		
		// If there are more levels
		if(currLevel < numLevels)
		{
			
			// Display / play wrong position dialogue
			if (ShowDialogue(BuffetManager.DialogueType.CORRECT) == false)
			{
				Debug.Log("S: That's right! Now let's move on to the next item.");
			}
			
			// Enable movement of the tray
			movementTime = 0;
			initialPosition = buffet.transform.localPosition;
			InvokeRepeating("MoveBuffet", 0, 0.01f);
		}
		else //if there are no more levels, this was the last one
		{
			// Disable moving the tray
			CancelInvoke("MoveBuffet");
			// Deactivate the buffet game object
			buffet.SetActive(false);
			endGame.SetActive(true);
			
			smallSampleBubble.SetActive(false);
			smallSample.SetActive(false);
			largeSample.SetActive(true);
			myTray.SetActive(false);
			
			// Display end of minigame dialogue
			if (ShowDialogue(DialogueType.END) == false)
			{
				Debug.Log ("S: Yay! You did it. This order is a perfect match. Now let's bring it back to Amy.");
			}
			
			Invoke ("CompleteMinigame", minigame.GetCurrentDialogueDuration());
		}
	}
	
	/// <summary>
	/// Signal the end of the minigame
	/// </summary>
	void CompleteMinigame()
	{
		//minigame.ContinueConversation();
		minigame.CompleteIfStandalone();
	}
	
	IEnumerator startTutorial()
	{
		tutorialObject.SetActive(true);
		//"Let’s help Amy get another lunch by dragging and dropping plates onto her tray."
		Respond(tutorial1);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1.5f);

		tutorialHand.SetActive(true);

		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		//yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);

		//"To move a plate, tap on it like this"
		Respond(tutorial2);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		tutorialHand.GetComponent<TutorialHand>().isPointing(true);

		tutorialObject.transform.parent = tutorialHand.transform;
		tutorialObject.transform.localScale = new Vector3(154f/tutorialHand.transform.localScale.x, 154f/tutorialHand.transform.localScale.y, 0);
		
		yield return new WaitForSeconds(1.5f);
		//"And then move it to the correct tray slot, like this"
		Respond(tutorial3);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval + 0.75f);
		tutorialHand.GetComponent<TutorialHand>().isPointing(false);
		
		//"Now you try!"
		Respond(tutorial4);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 0.5f);
		
		tutorialHand.SetActive(false);
		tutorialObject.SetActive(false);
		largeSample.SetActive(true);
		tutorialBuffet.SetActive(false);
		myTray.SetActive(false);
		// Show the instructions of the minigame
		if (minigame != null && minigame.StartConversation() == false)
		{
			Debug.Log("S: This is Amy's original order. She had a garden salad, burger and strawberries. We must remake Amy's lunch exactly as it appears, so choose the RIGHT item, and place it in the RIGHT location.");
		}
		
		Invoke ("LayBuffet", minigame.GetCurrentDialogueDuration());
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
		case DialogueType.NOT_ON_TRAY:
			return minigame.ContinueConversation(notOnTray);
		case DialogueType.WRONG_SPOT:
			return minigame.ContinueConversation(wrongSpot);
		case DialogueType.CORRECT:
			return minigame.ContinueConversation(correct);
		case DialogueType.END:
			return minigame.ContinueConversation(end);
		}
		
		return false;
	}
	
	public float GetAudioDuration (DialogueType dialogueType)
	{
		// Display dialogue depending on dialogue type
		switch(dialogueType)
		{
		case DialogueType.NOT_ON_TRAY:
			return (float) ((notOnTray.voiceOver != null) ? notOnTray.voiceOver.length : 0.5f);
		case DialogueType.WRONG_SPOT:
			return (float) ((wrongSpot.voiceOver != null) ? wrongSpot.voiceOver.length : 0.5f);
		case DialogueType.CORRECT:
			return (float) ((correct.voiceOver != null) ? correct.voiceOver.length : 0.5f);
		case DialogueType.END:
			return (float) ((end.voiceOver != null) ? end.voiceOver.length : 0.5f);
		}
		
		return 0.5f;
	}
	
	public void Respond (Dialogue response)
	{
		if (minigame != null && minigame.ContinueConversation(response) == false)
		{
			Debug.Log(response.text);
		}
	}
}
