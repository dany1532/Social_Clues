using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GabyMinigameManager : MinigameManager {
	
	
	public enum Topic 
	{ 
		FOOD = 0, FEET = 1, READ = 2, SPORTS = 3, VEHICLES = 4, WEATHER = 5
	};
	
	public enum DialogueType
	{
		 WANTHINT, HINT1, HINT2, CORRECT, CHOOSESOL, SOLCHOSEN, MISTAKE, END, START, NEXTROUND
	}
	
	public int maxNumberIncorrectTopics = 8;
	
	// Associated minigame
	Minigame minigame;
	
	//public UITexture friend1Topic;
	//public UITexture friend2Topic;
	
	public List<UITexture> friends;
	public List<Texture> foodTextureList;
	public List<Texture> feetTextureList;
	public List<Texture> readTextureList;
	public List<Texture> sportsTextureList;
	public List<Texture> vehiclesTextureList;
	public List<Texture> weatherTextureList;
	public List<Texture> trainTextureList;
	public List<NPCDialogueAnimation> friendsAnims;
	public NPCDialogueAnimation gabyAnim;
	
	List<List<Texture>> completeList;
	
	//Bunch of Dialogues
	public List<Dialogue> sherlockIntroPhases;
	public Dialogue hint1;
	public Dialogue hint2;
	public Dialogue correct;
	public Dialogue mistake;
	public Dialogue end;
	public Dialogue start;

	public SherlockMultipleDialogues incorrectResponses;
	public SherlockMultipleDialogues correctResponses;
	public SherlockMultipleDialogues chooseSolutions;
	public SherlockMultipleDialogues solutionsChosen;
	
	//public GabySelectionTable table;
	public GabyTopicsWheel wheel;
	public UITexture gabySpeechBubble;
	public UITexture gabySpeechTopic;
	public GameObject starParticles;
	public GameObject hintButton;
	public List<TopicContainer> containers;
	List<int> topicSelectorList;

	public bool showTutorial;
	public Dialogue[] tutorialResponse;
	public List<Texture> tutorialTextures;
	public GameObject tutorialObject;
	public GameObject tutorialObject2;
	public GameObject tutorialHand;
	public GameObject tutorialOptions;
	
	List<string> answersList;
	int correctAnswersFound = 0;
	int currentRound = 1;
	int correctTopics = 1;
	Topic currentTopic;
	
	public AudioClip rightAnswerFX;
	public AudioClip wrongAnswerFX;
	public AudioClip cheeringFX;
	
	public AudioClip foodVO;
	public AudioClip feetVO;
	public AudioClip vehiclesVO;
	public AudioClip weatherVO;
	public AudioClip sportsVO;
	public AudioClip readVO;
	
	// Use this for initialization
	void Start ()
	{
		answersList = new List<string>();
		topicSelectorList = new List <int>();
		Sherlock.Instance.SetBubblePosition (Sherlock.side.DOWN);

		tutorialHand.SetActive(false);
		
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		
		//minigame.difficulty = MinigameDifficulty.Difficulty.EASY;
		
		switch (minigame.difficulty)
		{
			//total = 4
		case MinigameDifficulty.Difficulty.EASY:
			correctTopics = 1;
			maxNumberIncorrectTopics = 3;
			break;
			
			//total = 6
		case MinigameDifficulty.Difficulty.MEDIUM:
			correctTopics = 2;
			maxNumberIncorrectTopics = 4;
			break;
			
			//total = 8
		case MinigameDifficulty.Difficulty.HARD:
			correctTopics = 3;
			maxNumberIncorrectTopics = 5;
			break;	
		}
		
		completeList = new List<List<Texture>>();
		completeList.Add(foodTextureList);
		completeList.Add(feetTextureList);
		completeList.Add(readTextureList);
		completeList.Add(sportsTextureList);
		completeList.Add(vehiclesTextureList);
		completeList.Add(weatherTextureList);
		
		for(int i = 0; i < 6; i++)
			topicSelectorList.Add(i);
		
		
		
		//SelectTopicsOfConversation();
		int counter = 0;
		foreach(UITexture friend in friends)
		{
			setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[counter]);
			friend.transform.parent.gameObject.SetActive(false);
			counter++;
		}
		
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE, gabyAnim);
		
		wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(false);

		Sherlock.Instance.HideDialogue();

		if(!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
		{
			Destroy(tutorialOptions);
            
			showTutorial = !ApplicationState.Instance.presentationBuild;
			if (ShowDialogue(DialogueType.START) == false)
			{
				Debug.Log("Starting!!");
			}
		}

	}

	public override void yesTutorial ()
	{
		Destroy(tutorialOptions);
		showTutorial = true;
		if (ShowDialogue(DialogueType.START) == false)
		{
			Debug.Log("Starting!!");
		}
	}

	public override void noTutorial ()
	{
		Destroy(tutorialOptions);
		showTutorial = false;
		if (ShowDialogue(DialogueType.START) == false)
		{
			Debug.Log("Starting!!");
		}
	}

	public void initiateTutorial()
	{
		StartCoroutine("startTutorial");
	}

	IEnumerator startTutorial()
	{
		string correctObject;
		Texture correctTexture, correctTexture2;
		GameObject originalParent;
		//yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
		Respond(tutorialResponse[0]); //Gaby's friends are talking about two different things.
		yield return new WaitForSeconds(tutorialResponse[0].voiceOver.length);

		correctTopics = 1;

		//show friends talking about 2 different shapes
		int index;
		for(int i = 0; i < 2; i++)
		{
			index =  UnityEngine.Random.Range(0, tutorialTextures.Count);	
			friends[i].mainTexture = tutorialTextures[index];	
			tutorialTextures.RemoveAt(index);
			setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[i]);
		}

		//Show Gaby's thought bubble
		//Choose Gaby's correct topics
		index =  UnityEngine.Random.Range(0, tutorialTextures.Count);
		wheel.listTopics.Add(tutorialTextures[index]);
		correctObject = tutorialTextures[index].name;
		correctTexture = tutorialTextures[index];
		tutorialTextures.RemoveAt(index);

		List<Texture> randomTopics = new List<Texture>();
		for(int i = 0; i < 2; i++)
		{
			for(int m = 0; m < 5; m++){
				randomTopics.Add(completeList[i][m]);
			}
		}

		///Fill with incorrect topics
		for(int i = 0; i < 2; i++)
		{
			index =  UnityEngine.Random.Range(0, randomTopics.Count);
			
			wheel.listTopics.Add(randomTopics[index]);
			
			randomTopics.RemoveAt(index);
		}

		StartCoroutine("startFriendsConversation"); //start showing thought bubbles.

		//wait until both friend bubbles are showing
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 4.0f);
		Respond(tutorialResponse[1]); //But Gaby has a lot of ideas in her head.

		//wait until all gabby objects are showing
		yield return new WaitForSeconds(2.4f);

		/*
		foreach(Transform o in wheel.transform)
		{
			o.gameObject.GetComponent<BoxCollider>().enabled = false;
		}
		*/
		float waitTime = minigame.GetCurrentDialogueDuration() - 2.4f;
		if (waitTime < 0) waitTime = 0;
		yield return new WaitForSeconds(waitTime);
		Respond(tutorialResponse[2]); //Help Gaby by tapping on the right subject...
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());


		//items stop moving
		foreach(Transform o in wheel.transform)
		{
			if(o.gameObject.GetComponent<Rigidbody>())
				o.gameObject.GetComponent<Rigidbody>().isKinematic = true;
		}
		tutorialObject = GameObject.Find(correctObject); //find the correct answer
		//tutorial hand moves to correct choice and taps on it
		tutorialHand.SetActive(true);
		tutorialHand.GetComponent<TutorialHand>().waypoints[0] = new Vector3(tutorialObject.transform.localPosition.x +.05f, 
		                                                                     tutorialObject.transform.localPosition.y -.05f, 
		                                                                     -11.0f);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().isPointing(true);

		Respond(tutorialResponse[3]); //then drag it to the speech bubble...
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		//drags the item to the speech bubble and drops
		tutorialObject.transform.parent = tutorialHand.transform;
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval +.3f);
		tutorialHand.GetComponent<TutorialHand>().isPointing(false);
		tutorialObject.transform.parent = tutorialHand.transform.parent;

		//starburst similar to end of level

		gabySpeechTopic.mainTexture = correctTexture;
		tutorialObject.SetActive(false);
		gabySpeechTopic.enabled = true;
		gabySpeechBubble.enabled = true;
		EnableContainers(false);
		
		wheel.DestroyChildren();
		
		CreateParticles(gabySpeechTopic.transform.position);
		
		for(int i = 0; i < 2; i++)
		{
			CreateParticles(friends[i].transform.position);	
			setFriendAnimation(NPCAnimations.AnimationIndex.RIGHT_CHOICE,friendsAnims[i]);
		}
		
		setFriendAnimation(NPCAnimations.AnimationIndex.RIGHT_CHOICE, gabyAnim);
		wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(false);


		yield return new WaitForSeconds(2.0f);

		//START MODE FOR MED/HARD

		if(minigame.difficulty != MinigameDifficulty.Difficulty.EASY)
		{
			//now, go back to game screen, but with 2 bubbles instead of one.
			tutorialObject.SetActive(true);
			gabySpeechTopic.enabled = false;
			gabySpeechBubble.enabled = false;
			//EnableContainers(false);

			wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(true);
			wheel.Reset();
			correctTopics = 2;
			EnableContainers(true);
			setFriendAnimation(NPCAnimations.AnimationIndex.THINKING, gabyAnim);
			for(int i = 0; i < 2; i++)
			{
				setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[i]);
			}


			//find a new correct object
			index =  UnityEngine.Random.Range(0, tutorialTextures.Count);
			wheel.listTopics.Add(tutorialTextures[index]);
			correctObject = tutorialTextures[index].name;
			correctTexture2 = tutorialTextures[index];

			//display that correct object
			wheel.InstantiateTopicsInsideBubble();
			foreach(Transform o in wheel.transform)
			{
				o.gameObject.GetComponent<BoxCollider>().enabled = false;
			}

			Respond(tutorialResponse[4]); //if there is more than one...
			yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

			//items stop moving
			foreach(Transform o in wheel.transform)
			{
				if(o.gameObject.GetComponent<Rigidbody>())
					o.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			}
			tutorialObject2 = GameObject.Find(correctObject); //find the correct answer
			//tutorial hand moves to correct choice and taps on it
			//tutorialHand.SetActive(true);
			tutorialHand.GetComponent<TutorialHand>().waypoints[2] = new Vector3(tutorialObject2.transform.localPosition.x +.05f, 
			                                                                     tutorialObject2.transform.localPosition.y -.05f, 
			                                                                     -11.0f);
			tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
			yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
			tutorialHand.GetComponent<TutorialHand>().isPointing(true);
			yield return new WaitForSeconds(.5f);

			//drags the item to the speech bubble and drops
			tutorialObject2.transform.parent = tutorialHand.transform;
			tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
			yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval +.3f);
			tutorialHand.GetComponent<TutorialHand>().isPointing(false);
			tutorialObject2.transform.parent = tutorialHand.transform.parent;

			tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
			yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);

			Respond(tutorialResponse[5]); //then tap on your fav.
			yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

			tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
			yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
			tutorialHand.GetComponent<TutorialHand>().isPointing(true);
			yield return new WaitForSeconds(.5f);

			//starburst similar to end of level
			tutorialObject.SetActive(false);
			tutorialObject2.SetActive(false);
			gabySpeechTopic.enabled = true;
			gabySpeechBubble.enabled = true;
			EnableContainers(false);
			
			wheel.DestroyChildren();
			
			CreateParticles(gabySpeechTopic.transform.position);
			
			for(int i = 0; i < 2; i++)
			{
				CreateParticles(friends[i].transform.position);	
				setFriendAnimation(NPCAnimations.AnimationIndex.RIGHT_CHOICE,friendsAnims[i]);
			}
			
			setFriendAnimation(NPCAnimations.AnimationIndex.RIGHT_CHOICE, gabyAnim);
			wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(false);

			yield return new WaitForSeconds(2.0f);
		}

		//END MODE FOR MED/HARD
		else //move the position of the tutorial hand over the hint button forward in the waypoint list
		{
			tutorialHand.GetComponent<TutorialHand>().waypoints[2] = tutorialHand.GetComponent<TutorialHand>().waypoints[6];
		}

		Respond(tutorialResponse[6]); //If you need help, press hint
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		tutorialHand.GetComponent<TutorialHand>().isPointing(false);
		tutorialHand.GetComponent<TutorialHand>().nextWayPoint();
		yield return new WaitForSeconds(tutorialHand.GetComponent<TutorialHand>().moveInterval);
		tutorialHand.GetComponent<TutorialHand>().isPointing(true);
		yield return new WaitForSeconds(.3f);

		Respond(tutorialResponse[7]); //Let's begin
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

		//reset the state of the game
		tutorialObject.SetActive(false);
		gabySpeechTopic.enabled = false;
		gabySpeechBubble.enabled = false;
		tutorialHand.SetActive(false);
		EnableContainers(false);

		wheel.Reset();
		switch (minigame.difficulty)
		{
		case MinigameDifficulty.Difficulty.EASY:
			correctTopics = 1;
			break;
		case MinigameDifficulty.Difficulty.MEDIUM:
			correctTopics = 2;
			break;
		case MinigameDifficulty.Difficulty.HARD:
			correctTopics = 3;
			break;	
		}

		//EnableContainers(true);
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE, gabyAnim);
		for(int i = 0; i < 2; i++)
		{
			setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[i]);
			friends[i].transform.parent.gameObject.SetActive(false);
		}

		Sherlock.Instance.PlaySequenceInstructions(start, SelectTopicsOfConversation);

	}
	
	public void SelectTopicsOfConversation()
	{
		
		List<int> randomNumbers = new List<int>();
		List<Texture> randomTopics = new List<Texture>();
		int count = 0;
		int index = 0;
		
		answersList.Clear();
		wheel.Reset();
		
		var values = System.Enum.GetValues(typeof(Topic));
		System.Random random = new System.Random();
		
		index = random.Next(topicSelectorList.Count);
		
		currentTopic = (Topic)values.GetValue(topicSelectorList[index]);
		
		topicSelectorList.Remove(index);
		
		
		for(int i = 0; i < 5; i++)
			randomNumbers.Add(i);
		
		//choose friends individual topoc
		for(int i = 0; i < 2; i++)
		{
			index =  UnityEngine.Random.Range(0, randomNumbers.Count);	
			friends[i].mainTexture = completeList[(int) currentTopic][randomNumbers[index]];	
			randomNumbers.RemoveAt(index);
			setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[i]);
		}
		
		//Choose Gaby's correct topics
		for(int i = 0; i < correctTopics; i++)
		{
			index =  UnityEngine.Random.Range(0, randomNumbers.Count);
			
			wheel.listTopics.Add(completeList[(int) currentTopic][randomNumbers[index]]);
			answersList.Add(completeList[(int) currentTopic][randomNumbers[index]].name);
			
			randomNumbers.RemoveAt(index);
		}
		
		for(int i = 0; i < completeList.Count; i++)
		{
			if(	i != (int) currentTopic)
			{
				for(int m = 0; m < 5; m++){
					randomTopics.Add(completeList[i][m]);
				}
			}
		}
		
		
		//Fill with incorrect topics
		for(int i = 0; i < maxNumberIncorrectTopics; i++)
		{
			index =  UnityEngine.Random.Range(0, randomTopics.Count);
			
			wheel.listTopics.Add(randomTopics[index]);
			
			randomTopics.RemoveAt(index);
		}
		
		randomTopics.Clear();
		
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE, gabyAnim);
		
		StartCoroutine("startFriendsConversation");
	}
	
	void setFriendAnimation(NPCAnimations.AnimationIndex animationType, NPCDialogueAnimation anim) 
	{
		NPCAnimations.AnimationSequence currAnimSeq = anim.GetComponent<NPCAnimations>().RetrieveAnimationSequence(animationType);
		List<Texture> currAnimSeqTextures = currAnimSeq.textures;
		if (currAnimSeqTextures.Count > 0)
		{
			anim.StopAnimation();
			anim.SetAnimationList(currAnimSeqTextures);
			anim.PlayAnimation();
			anim.SetSpeed(currAnimSeq.speed);
		}

	}
	
	IEnumerator startFriendsConversation()
	{
		foreach(UITexture friend in friends)
			friend.transform.parent.gameObject.SetActive(false);
		
		wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(false);
		EnableContainers(false);
		wheel.gameObject.SetActive(false);
		hintButton.SetActive(false);
		
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() );
		
		int counter = 0;
		
		friends[counter].transform.parent.gameObject.SetActive(true);
		
		setFriendAnimation(NPCAnimations.AnimationIndex.TALKING,friendsAnims[counter]);
		
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE, gabyAnim);
		
		yield return new WaitForSeconds(2f);
		
		friends[counter].transform.parent.gameObject.SetActive(false);
		
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[counter]);
		
		counter++;
			
		friends[counter].transform.parent.gameObject.SetActive(true);
		
		setFriendAnimation(NPCAnimations.AnimationIndex.TALKING,friendsAnims[counter]);
		
		setFriendAnimation(NPCAnimations.AnimationIndex.LISTENING, gabyAnim);
		
		yield return new WaitForSeconds(2f);	
	
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[counter]);
		
		friends[counter].transform.parent.gameObject.SetActive(false);

		wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(true);
		EnableContainers(true);
		wheel.gameObject.SetActive(true);
		hintButton.SetActive(true);
		
		counter = 0;
		foreach(UITexture friend in friends)
		{
			setFriendAnimation(NPCAnimations.AnimationIndex.WAITING,friendsAnims[counter]);
			friend.transform.parent.gameObject.SetActive(true);
		}
		
		setFriendAnimation(NPCAnimations.AnimationIndex.THINKING, gabyAnim);
		
		wheel.InstantiateTopicsInsideBubble();
		
	}
	
	void EnableContainers(bool isEnabled)
	{
		containers[0].transform.parent.gameObject.SetActive(isEnabled);
		
		for(int i = 0; i < correctTopics; i++){
			containers[i].gameObject.SetActive(isEnabled);
			containers[i].GetComponent<BoxCollider>().enabled = isEnabled;
			
		}
	}
	
	public void GotCorrectAnswer()
	{
		correctAnswersFound++;
		
		if(correctAnswersFound == correctTopics)
		{
			
			wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(false);
			wheel.DestroyWrongAnswers(answersList);
			hintButton.SetActive(false);
			
			if(minigame.difficulty != MinigameDifficulty.Difficulty.EASY)
			{
				AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
				
				if (ShowDialogue(DialogueType.CHOOSESOL) == false)
				{
					Debug.Log("Choosing the topic....");
				}
			}
			
			//Don't have to choose solution since there is only one
			else
			{
				PlayerSelectedTopic(containers[0].GetTopic());
			}
		}
		
		else
		{
			AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
			
			if (ShowDialogue(DialogueType.CORRECT) == false)
			{
				Debug.Log("Got it right!");
			}
		}
	}
	
	public void PlayerSelectedTopic(Texture topic)
	{
		gabySpeechTopic.mainTexture = topic;
		
		if(currentRound < 3)
		{
			if (ShowDialogue(DialogueType.SOLCHOSEN) == false)
			{
				Debug.Log("Chose " + topic.name);
			}
		}
		
		StartCoroutine("gabyFollowConversation");
		
		
	}
	
	void CreateParticles(Vector3 pos)
	{
		AudioManager.Instance.Play(cheeringFX, transform, 1, false);
		GameObject particles = Instantiate(starParticles) as GameObject;
		particles.transform.position = pos;
	}
	
	IEnumerator gabyFollowConversation()
	{
		int counter = 0;
		
		gabySpeechTopic.enabled = true;
		gabySpeechBubble.enabled = true;
		EnableContainers(false);
		
		wheel.DestroyChildren();
		
		CreateParticles(gabySpeechTopic.transform.position);
		
		for(int i = 0; i < 2; i++)
		{
			CreateParticles(friends[i].transform.position);	
			setFriendAnimation(NPCAnimations.AnimationIndex.RIGHT_CHOICE,friendsAnims[i]);
		}
		
		setFriendAnimation(NPCAnimations.AnimationIndex.RIGHT_CHOICE, gabyAnim);
		
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() );
		
		gabySpeechTopic.enabled = false;
		gabySpeechBubble.enabled = false;
		wheel.transform.parent.FindChild("ThoughtBubble").gameObject.SetActive(false);
		
		AudioManager.Instance.StopSoundFX();
		correctAnswersFound = 0;
		currentRound++;
		
		if(currentRound != 4)
		{
			if (ShowDialogue(DialogueType.NEXTROUND) == false)
			{
				Debug.Log("Starting!!");
			}
			
			maxNumberIncorrectTopics += 2;
			
			EnableContainers(true);
			
			SelectTopicsOfConversation();
		}
		
		else
		{
			
			foreach(UITexture friend in friends)
			{
				friend.transform.parent.gameObject.SetActive(false);
				setFriendAnimation(NPCAnimations.AnimationIndex.IDLE,friendsAnims[counter]);
				counter++;
			}
			
			setFriendAnimation(NPCAnimations.AnimationIndex.IDLE, gabyAnim);

			Sherlock.Instance.PlaySequenceInstructions(end, completeMinigame);

		}
	}

	void completeMinigame()
	{
		minigame.CompleteMinigame();
	}
	
	public void GotWrongAnswer()
	{
		AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
		if (ShowDialogue(DialogueType.MISTAKE) == false)
		{
			Debug.Log("OH NO, MISTAKE FOUND!");
		}
		StartCoroutine("WrongAnswerAnimation");
	}
	
	IEnumerator WrongAnswerAnimation()
	{
		for(int i = 0; i < 2; i++)
		{
			setFriendAnimation(NPCAnimations.AnimationIndex.WRONG_CHOICE,friendsAnims[i]);
		}
		
		setFriendAnimation(NPCAnimations.AnimationIndex.WRONG_CHOICE, gabyAnim);
		
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() );
		
		setFriendAnimation(NPCAnimations.AnimationIndex.THINKING, gabyAnim);
		
		for(int i = 0; i < 2; i++)
		{
			setFriendAnimation(NPCAnimations.AnimationIndex.WAITING,friendsAnims[i]);
		}
	}
	
	
	public List<string> GetAnswerList()
	{
		return answersList;
	}
	
	public bool ContainersFull()
	{
		if(correctAnswersFound == correctTopics)
			return true;
		else
			return false;
	}
	
	public void HintButtonPressed()
	{
		string hintText = hint1.text;
		//string hintText = "";
		//hintText += System.Enum.GetName(typeof(Topic), currentTopic);
		switch(System.Enum.GetName(typeof(Topic), currentTopic))
		{
			case "FOOD": 	hintText += "things you eat";
							hint2.voiceOver = foodVO;
					   		break;
			case "FEET": 	hintText += "things you wear on your feet";
							hint2.voiceOver = feetVO;
					   		break;
			case "VEHICLES": hintText += "things that go";
							hint2.voiceOver = vehiclesVO;
					   		break;
			case "WEATHER": hintText += "the weather";
							hint2.voiceOver = weatherVO;
					   		break;
			case "SPORTS":  hintText += "sports";
							hint2.voiceOver = sportsVO;
					   		break;
			case "READ": 	hintText += "things you read";
							hint2.voiceOver = readVO;
					   	 	break;
		}
		
		hint1.text = hintText;
		hint2.text = hintText;
		hint1.nextDialogue = hint2;
		ShowDialogue(DialogueType.HINT1);

		
	}
	
	
	/*public bool CheckCorrectTopic(string topic)
	{
		//List<string> answerList = manager.GetAnswerList();
		bool found = false;
		
		for(int i = 0; i < answersList.Count; i++){
			if(topic == answersList[i]){
				found = true;
				break;
			}
		}
		
		return found;
	}*/
	
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
			case DialogueType.HINT1:
				Sherlock.Instance.PlaySequenceInstructions(hint1, null);
				return true;
			case DialogueType.HINT2:
				Sherlock.Instance.PlaySequenceInstructions(hint2, null);
				return true;
			
			case DialogueType.NEXTROUND:
				Sherlock.Instance.PlaySequenceInstructions(sherlockIntroPhases[currentRound - 2], null);
				return true;
			
			case DialogueType.SOLCHOSEN:
				Sherlock.Instance.PlaySequenceInstructions(solutionsChosen.GetRandomDialogue(), null);
				return true;
			
			case DialogueType.CORRECT:
				Sherlock.Instance.PlaySequenceInstructions(correctResponses.GetRandomDialogue(), null);
				return true;
			
			case DialogueType.CHOOSESOL:
				Sherlock.Instance.PlaySequenceInstructions(chooseSolutions.GetRandomDialogue(), null);
				return true;
			
			case DialogueType.MISTAKE:
				Sherlock.Instance.PlaySequenceInstructions(incorrectResponses.GetRandomDialogue(), null);
				return true;
		
			case DialogueType.END:
				Sherlock.Instance.PlaySequenceInstructions(end, null);
				return true;
			
			case DialogueType.START:
				if(showTutorial)
					Sherlock.Instance.PlaySequenceInstructions(start, initiateTutorial);
				else
					Sherlock.Instance.PlaySequenceInstructions(start, SelectTopicsOfConversation);
				return true;

			default:
				return false;
		}
	}

	public void Respond (Dialogue response)
	{
		if (minigame != null && minigame.ContinueConversation(response) == false)
		{
			Debug.Log(response.text);
		}
	}
}
