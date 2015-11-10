using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selfish_Sam_Manager : MinigameManager 
{
	public enum GameState
	{
		STORY,
		MINIGAME
	}
	
	public enum DialogueType
	{
		 WANTHINT, HINT1, HINT2, CORRECT, CHOOSESOL, SOLCHOSEN, MISTAKE, END, START, NEXTROUND,
		START_MINIGAME, BEGIN
	}

	
	
	
	GameState gameState = GameState.STORY;
	Minigame minigame;
	
	//Dialogues
	public Dialogue end;
	public List<Dialogue> sherlockPhase1;
	public List<Dialogue> sherlockPhase2;
	public List<Dialogue> sherlockPhase3;
	public Dialogue begin;
	public Dialogue startMinigame;
	public List<Dialogue> sherlockIntroPhases;
	
	public bool activateTreasureAI;
	public bool activateShootingAI;
	public bool activateMovementAI;
	
	public AIHand shootingAIHand;
	public AIHand treasureAIHand;
	public Wheel_Ship wheel;
	
	//Game Goals
	int goalEnemies = 5;
	int goalTreasure = 4;
	float timeBarDuration = 20f;
	int round = 1;
	bool endRound = false;
	
	//Number of objects that will appear a certain amount of time
	int numRocks = 1;
	int numEnemies = 1;
	int numTreasures = 1;
	
	//Reference to the respective bars
	public UISprite treasureBar;
	public UISprite enemyBar;
	public UISprite timeBar;
	public UISprite healthBar;
	
	//Used for the lerp function
	float lerpTreasure = 0f;
	float lerpEnemyBar = 0f;
	float lerpTime = 0f;
	float lerpHealth = 0f;
	
	//How far one is toward reaching the goal
	public int currentEnemies = 0;
	public int currentTreasures = 0;
	public int currentTime = 0;
	public float maxHealth = 3.0f;
	int timeScore = 0;
	
	//Time lost for colliding (in seconds)
	public float timeLoss = 5f;
	
	//Prefabs for instantiation
	public GameObject enemyPrefab;
	public GameObject treasurePrefab;
	public GameObject rockPrefab;
	public GameObject rockMediumPrefab;
	public GameObject rockHardPrefab;
	
	//Parents for the prefabs
	public GameObject rocksParent;
	public GameObject enemiesParent;
	public GameObject treasuresParent;
	
	//Where the ship is
	public Transform ship;
	
	public List<Vector3> listGameObjects;
	
	//Area in which the objects will randomly appear
    public BoxCollider spawnArea;
	public BoxCollider rockSpawnArea;
	public BoxCollider treasureSpawnArea;
	
	//How fast the objects will move down
	public float verticalSpeedIncrease = 0.2f;
	public bool changeVerticalSpeed = false;
	
	float rockHardPosOffset = 0.30f;
	float rockMedPosOffset = 0.20f;
	
	int score = 0;
	int points_time = 10;
	int points_enemy = 20;
	int points_treasure = 100;
	//int points_collideDeduction = -100;
	
	public UILabel scoreLabel;
	public UILabel scoreTitleLabel;
	
	public ScoreBoard scoreBoard;
	
	public List<AudioClip> samVoice;
	int currentSamVoice = 0;
	public List<AudioClip> luisVoice;
	int currentLuisVoice = 0;
	public NPCDialogueAnimation samAnims;
	public NPCDialogueAnimation luisAnims;
	public AudioClip enemyDefeatFX;
	public AudioClip gainTreasureFX;
	

	//The following will determine whether or not the player sees the tutorial.
	public GameObject tutorialOptions;
	bool showTutorial;
	public Transform barGlow;
	public float barDistance = .144f;
	public List<Transform> bars;

	void Awake()
	{
		VerticalMovement.verticalSpeed = VerticalMovement.startSpeed;
		TiledTextureMovement.movSpeed = TiledTextureMovement.startMovSpeed;
	}
	
	// Use this for initialization
	void Start () 
	{
		// Find assocciated minigame
		minigame = GetComponent<Minigame>();
		
		Sherlock.Instance.SetBubblePosition (Sherlock.side.DOWN);
		
		IncreaseMinigameSpeed(verticalSpeedIncrease);
		
		
		//gameState = GameState.MINIGAME;
		//gameState = GameState.STORY;
		
		//minigame.difficulty = MinigameDifficulty.Difficulty.MEDIUM;
		
		
		
		//EndMinigame();
		//if (true)
		
		if(GameObject.FindGameObjectWithTag("Player") != null){
			gameState = GameState.STORY;
		}
		else{
			gameState = GameState.MINIGAME;
		}
		//gameState = GameState.STORY;
		
		barGlow.gameObject.SetActive(false);
		if(gameState == GameState.STORY)
		{
			Destroy(tutorialOptions);
			scoreLabel.gameObject.SetActive(false);
			scoreTitleLabel.gameObject.SetActive(false);
			healthBar.transform.parent.gameObject.SetActive(false);
			

			if (gameState == GameState.STORY && ShowDialogue(DialogueType.BEGIN) == true)
			{
				Debug.Log("Starting!!");
			}	
//			if (gameState == GameState.STORY && ShowDialogue(DialogueType.NEXTROUND) == true)
//			{
//				Debug.Log("Starting!!");
//			}
//			
		}
		
		else
		{
			treasureBar.transform.parent.gameObject.SetActive(false);
			enemyBar.transform.parent.gameObject.SetActive(false);
			timeBar.transform.parent.gameObject.SetActive(false);
			updateScore(0);
			
			if(minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
				round = 1;
			
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM){
				round = 2;
			}
			
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD){
				IncreaseMinigameSpeed(0.2f);
				round = 3;
			}
			

		}
	}
	
	public override void yesTutorial()
	{
		Destroy(tutorialOptions);
		showTutorial = true;
		if (gameState == GameState.MINIGAME && ShowDialogue(DialogueType.START_MINIGAME) == true)
		{
			Debug.Log("Starting MINIGAME!!");
		}
	}

	public override void noTutorial()
	{
		Destroy(tutorialOptions);
		showTutorial = false;
		if (gameState == GameState.MINIGAME && ShowDialogue(DialogueType.START_MINIGAME) == true)
		{
			Debug.Log("Starting MINIGAME!!");
		}
	}
	
	void IncreaseMinigameSpeed(float multiplier)
	{
		verticalSpeedIncrease = multiplier;
		VerticalMovement.verticalSpeed += VerticalMovement.verticalSpeed * verticalSpeedIncrease;
		TiledTextureMovement.movSpeed -=  TiledTextureMovement.movSpeed * verticalSpeedIncrease;
	}
	
	
	//Instantiate enemies inside the enemy spawn area
	void InstantiateEnemies()
	{
		//Enemies instantiation
		for(int i = 0; i < numEnemies; i++)
		{
			GameObject go = AddChild(spawnArea.gameObject, enemyPrefab);
			go.name = "Enemy";
			go.transform.localPosition = CalculateGameObjectNewPosition(go.transform.position, spawnArea, false);
			
		}
	}
	
	//Instantiate treasures inside the tresasure spawn area
	void InstantiateTreasures()
	{
		for(int i = 0; i < numTreasures; i++)
		{
			
			GameObject go = AddChild(spawnArea.gameObject, treasurePrefab);
			go.name = "Treasure";
			Vector3 pos;
			pos = CalculateGameObjectNewPosition(go.transform.position, treasureSpawnArea, false);
			pos.z -= 2f;
			go.transform.localPosition = pos;
		}
	}
	
	//Instantiate rocks inside enemy spawn area but in reference to the ship
	void InstantiateRocks()
	{
		//Rocks instantiation
		for(int i = 0; i < numRocks; i++)
		{
			
			GameObject go = rockSpawnArea.gameObject;
			
			if(minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
				go = AddChild(rockSpawnArea.gameObject, rockPrefab);
			
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
				go = AddChild(rockSpawnArea.gameObject, rockMediumPrefab);
			
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
				go = AddChild(rockSpawnArea.gameObject, rockHardPrefab);
			
			go.name = "Rock";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, rockSpawnArea, true);
			
		}
	}
	
	//Random position inside a spawn area (Inside rectangle area)
	Vector3 CalculateGameObjectNewPosition(Vector3 newPos, BoxCollider spawnZone, bool isRock)
	{
		Vector3 center = spawnZone.transform.position;
		int counter = 0;
		
		do
		{
			if(isRock)
			{
				
				Vector3 centerGlobalRight = center;
				Vector3 centerGlobalLeft = center;
				
				centerGlobalRight.x = center.x + (spawnZone.size.x/2);
				centerGlobalLeft.x =  center.x - (spawnZone.size.x/2);
				
				centerGlobalRight = spawnZone.transform.TransformPoint(centerGlobalRight);
				centerGlobalLeft = spawnZone.transform.TransformPoint(centerGlobalLeft);
				
				newPos.x = Random.Range(ship.transform.position.x - 0.2f, ship.transform.position.x + 0.2f);
				
//				float rightBound = center.x + (spawnZone.size.x/2);
//				float leftBound =  center.x - (spawnZone.size.x/2);
				
				
				if(minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
				{
					//Debug.Log("newPos: " + newPos.ToString() + "   rightbound: "+ rightBound.toString());
					if(newPos.x > centerGlobalRight.x){
						newPos.x =  centerGlobalRight.x;
					}
					
					else if(newPos.x <  centerGlobalLeft.x){
						newPos.x =  centerGlobalLeft.x;
					}
				}
				
				else if(minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
				{
					if(newPos.x >  centerGlobalRight.x - rockMedPosOffset){
						newPos.x =  centerGlobalRight.x - rockMedPosOffset;
					}
					
					else if(newPos.x <  centerGlobalLeft.x + rockMedPosOffset){
						newPos.x =  centerGlobalLeft.x + rockMedPosOffset;
					}
				}
				
				else if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
				{
					if(newPos.x >  centerGlobalRight.x - rockHardPosOffset){
						newPos.x =  centerGlobalRight.x - rockHardPosOffset;
					}
					
					else if(newPos.x <  centerGlobalLeft.x + rockHardPosOffset){
						newPos.x =  centerGlobalLeft.x + rockHardPosOffset;
					}
				}
				
				newPos.y = center.y;
			}
			else
			{
				newPos.x = Random.Range(center.x - ((spawnZone.size.x)/2), center.x + ((spawnZone.size.x)/2));
				newPos.y = Random.Range(center.y - ((spawnZone.size.y)/2), center.y + ((spawnZone.size.y)/2));
			}
		
			
			newPos.z = center.z ;
			counter++;
			if(counter > 10) {
				break;
			}
			
		} while(Physics.CheckSphere(newPos, 0.3f));	
		
		return newPos;
	}
	
	

	
	//Update treasure bar
	public void updateTreasureBar()
	{
		currentTreasures++;
		AudioManager.Instance.Play(gainTreasureFX, transform, 0.1f, false);
		if(gameState == GameState.STORY)
		{
			if(lerpTreasure < 1f)
			{
				lerpTreasure += 1/(float) goalTreasure;
				treasureBar.fillAmount = Mathf.Lerp(0, 1, lerpTreasure);
			}
			
			else
				lerpTreasure = 1;
			
			if(!endRound && currentEnemies >= goalEnemies && currentTreasures >= goalTreasure && 
				currentTime >= timeBarDuration)
			{
				endRound = true;
				Reset_NextRound();
			}
		}
		
		else
		{
			gainHealthBar();
			if(minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
				updateScore(points_treasure);
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
				updateScore(points_treasure * 2);
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
				updateScore(points_treasure * 3);
		}
	}
	
	void updateScore(int pointsGained)
	{
		score += pointsGained;
		if(score < 0)
			score = 0;
		
		scoreLabel.text = score.ToString();
	}
	
	//Updates enemy bar if enemy got shot down
	public void updateEnemyBar()
	{
		currentEnemies++;
		AudioManager.Instance.Play(enemyDefeatFX, transform, 0.1f, false);
		
		if(gameState == GameState.STORY)
		{
			if(lerpEnemyBar < 1f)
			{
				lerpEnemyBar += (float)	1/(float) goalEnemies;
				enemyBar.fillAmount = Mathf.Lerp(0, 1, lerpEnemyBar);
			}
			
			else
				lerpEnemyBar = 1;
			
			if(!endRound && currentEnemies >= goalEnemies && currentTreasures >= goalTreasure && 
				currentTime >= timeBarDuration)
			{
				endRound = true;
				Reset_NextRound();
			}
		}
		
		else{
			if(minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
				updateScore(points_enemy);
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
				updateScore(points_enemy * 2);
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
				updateScore(points_enemy * 3);
		}
	}
	
	void UpdateTimeBar()
	{
		currentTime++;
		timeScore++;
		
		if(gameState == GameState.STORY)
		{
			if(lerpTime < 1)
			{
				lerpTime += 1f/(float) timeBarDuration;
				timeBar.fillAmount = Mathf.Lerp(0, 1, lerpTime);
			}
		
			else
				lerpTime = 1;
				
			if(!endRound && currentEnemies >= goalEnemies && currentTreasures >= goalTreasure && 
				currentTime >= timeBarDuration)
			{
				endRound = true;
				Reset_NextRound();
			}
		}
		
		else
		{
			if(minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
				updateScore(points_time);
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
				updateScore(points_time * 2);
			else if(minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
				updateScore(points_time * 3);
			
			if(round < 3 && currentTime >= timeBarDuration){
				Reset_MinigameNextRound();
			}
		}
	}
	
	
	//Ship collided, lower timer bar
	public void ShipCollide()
	{
		if(gameState == GameState.STORY)
		{
			currentTime -= (int) timeLoss;
			
			if(lerpTime > 0)
			{
				lerpTime -= timeLoss/(float) timeBarDuration;
				timeBar.fillAmount = Mathf.Lerp(0, 1, lerpTime);
			}
			
			else if(lerpTime <= 0)
				lerpTime = 0;
		}
		
		else{
			updateHealthBar();
		}
	}
	
	void updateHealthBar()
	{
		if(lerpHealth < 1f)
			{
				lerpHealth += 1f/(float) maxHealth;
				healthBar.fillAmount = Mathf.Lerp(1, 0, lerpHealth);
			}
		
			else
				lerpHealth = 1;
		
		if(lerpHealth >= 1f)
		{
			scoreBoard.SetScoreBoard(currentTreasures, currentEnemies, timeScore, score);
			minigame.CompleteMinigame();
		}
	}
	
	void gainHealthBar()
	{
		if(lerpHealth < 1f)
			{
				lerpHealth -= (1f/(float) maxHealth) * 0.1f;
				healthBar.fillAmount = Mathf.Lerp(1, 0, lerpHealth);
			}
		
			else
				lerpHealth = 1;
		
	}
	
	float random_Instantiation_Timer(int min, int max){
		return Random.Range(min,max);
	}
	
	
	//For minigame mode
	void Reset_MinigameNextRound()
	{
		CancelInvoke("InstantiateRocks");
		CancelInvoke("InstantiateEnemies");
		CancelInvoke("InstantiateTreasures");
		CancelInvoke("UpdateTimeBar");
		
		currentTime = 0;
		
		round++;
		
		GameMinigameRoundBegin();
	}
	
	//For Story mode
	void Reset_NextRound()
	{
		CancelInvoke("InstantiateRocks");
		CancelInvoke("InstantiateEnemies");
		CancelInvoke("InstantiateTreasures");
		CancelInvoke("UpdateTimeBar");
		
		shootingAIHand.Reset();
		treasureAIHand.Reset();
		wheel.StopWheel_AI();
		
		currentTime = 0;
		currentTreasures = 0;
		currentEnemies = 0;
		
		timeBar.fillAmount = 0;
		treasureBar.fillAmount = 0;
		enemyBar.fillAmount = 0;
		
		lerpTime = 0;
		lerpTreasure = 0;
		lerpEnemyBar = 0;
		
		DestroyChildren(spawnArea.transform);
		DestroyChildren(spawnArea.transform);
		DestroyChildren(treasureSpawnArea.transform);
		
		
		
		round++;
		
		//GameRoundBegin();
	
		if (gameState == GameState.STORY && ShowDialogue(DialogueType.NEXTROUND) == true)
		{
			Debug.Log("NEXT ROUND!!");
		}	
		
	}
	
	void GameRoundBegin()
	{
		if(gameState == GameState.STORY)
		{
			DestroyChildren(rocksParent.transform);
			DestroyChildren(enemiesParent.transform);
			DestroyChildren(treasuresParent.transform);
			endRound = false;
			
			if(round == 1)
			{
				activateTreasureAI = true;
				activateShootingAI = true;
				activateMovementAI = false;
				
				treasureAIHand.SetLuisHand();
				treasureAIHand.EnableAI();
				
				shootingAIHand.SetSamHand();
				shootingAIHand.EnableAI();
				
				wheel.DisableMovementAI();
				
			}
			
			if(round == 2)
			{
				goalEnemies += 2;
				goalTreasure += 2;
				timeBarDuration += 5f;
				
				activateTreasureAI = false;
				activateShootingAI = true;
				activateMovementAI = true;
				
				treasureAIHand.Reset();
				
				shootingAIHand.SetSamHand();
				shootingAIHand.EnableAI();
				
				wheel.movementAIHand.SetLuisHand();
				wheel.EnableMovementAI();
				
			}
			
			if(round == 3)
			{
				goalEnemies += 2;
				goalTreasure += 1;
				timeBarDuration += 5f;
				
				activateTreasureAI = true;
				activateShootingAI = false;
				activateMovementAI = true;
				
				treasureAIHand.SetSamHand();
				treasureAIHand.EnableAI();
				
				shootingAIHand.Reset();
				
				wheel.movementAIHand.SetLuisHand();
				wheel.EnableMovementAI();
			}
			
			if(round == 4)
			{
				CancelInvoke("InstantiateRocks");
				CancelInvoke("InstantiateEnemies");
				CancelInvoke("InstantiateTreasures");
				CancelInvoke("UpdateTimeBar");
				
				DestroyChildren(spawnArea.transform);
				DestroyChildren(rockSpawnArea.transform);
				
				ShowDialogue(DialogueType.END);
			}
		}
		
		//alarms for the object instantiation
		InvokeRepeating("InstantiateEnemies", 0f, Random.Range(4f, 7f));
		InvokeRepeating("InstantiateTreasures", 0f, Random.Range(4f, 7f));
		InvokeRepeating("InstantiateRocks", 0f, Random.Range(4f, 7f));
		
		InvokeRepeating("UpdateTimeBar", 1f, 1f);
	}
	
//	IEnumerator startRound()
//	{
//		
//		//alarms for the object instantiation
//		InvokeRepeating("InstantiateEnemies", 0f, Random.Range(4f, 7f));
//		InvokeRepeating("InstantiateTreasures", 0f, Random.Range(4f, 7f));
//		InvokeRepeating("InstantiateRocks", 0f, Random.Range(4f, 7f));
//		
//		InvokeRepeating("UpdateTimeBar", 1f, 1f);
//	}
	
	void GameMinigameRoundBegin()
	{
		if(gameState == GameState.MINIGAME)
		{
			endRound = false;
			
			if(round == 1)
			{
				timeBarDuration = 60f;
				Debug.Log("Starting Easy mode");
				InvokeRepeating("InstantiateEnemies", 0f, Random.Range(4f, 7f));
				InvokeRepeating("InstantiateTreasures", 0f, Random.Range(4f, 7f));
				InvokeRepeating("InstantiateRocks", 0f, Random.Range(4f, 7f));
			}
			
			if(round == 2)
			{
				minigame.difficulty = MinigameDifficulty.Difficulty.MEDIUM;
				timeBarDuration = 60f;
				numEnemies = 2;
				IncreaseMinigameSpeed(0.2f);
				Debug.Log("Starting Medium mode");
				
				InvokeRepeating("InstantiateEnemies", 0f, Random.Range(4f, 7f));
				InvokeRepeating("InstantiateTreasures", 0f, Random.Range(4f, 7f));
				InvokeRepeating("InstantiateRocks", 0f, Random.Range(6f, 8f));
			}
			
			if(round == 3)
			{
				minigame.difficulty = MinigameDifficulty.Difficulty.HARD;
				timeBarDuration = 30f;
				round = 2;
				IncreaseMinigameSpeed(0.1f);
				numEnemies = 3;
				Debug.Log("Starting Hard mode");
				InvokeRepeating("InstantiateEnemies", 0f, Random.Range(4f, 7f));
				InvokeRepeating("InstantiateTreasures", 0f, Random.Range(4f, 7f));
				InvokeRepeating("InstantiateRocks", 0f, Random.Range(6f, 8f));
			}
			
			if(round == 4)
			{
				DestroyChildren(spawnArea.transform);
				DestroyChildren(rockSpawnArea.transform);
				minigame.CompleteMinigame();
			}
		}
		
		
		InvokeRepeating("UpdateTimeBar", 1f, 1f);
	}
	
	public bool ShowDialogue(DialogueType dialogueType)
	{
		// Display dialogue depending on dialogue type
		switch(dialogueType)
		{	
			case DialogueType.NEXTROUND:
				if(round == 2) StartCoroutine("Phase2Tutorial");
				if(round == 3) StartCoroutine("Phase3Tutorial");
				if(round == 4) GameRoundBegin();
				return true;
			
			case DialogueType.START:
				StartCoroutine("Phase1Tutorial");
				return true;
			
			case DialogueType.END:
				Sherlock.Instance.PlaySequenceInstructions(end, null);
				Invoke("StoryEnd", minigame.GetCurrentDialogueDuration());
				return true;
			
			case DialogueType.BEGIN:
				Sherlock.Instance.PlaySequenceInstructions(begin, playStart);
				return true;
			
			case DialogueType.START_MINIGAME:
				if(showTutorial)
					Sherlock.Instance.PlaySequenceInstructions(startMinigame, GameMinigameRoundBegin);
				else
				{
					Sherlock.Instance.HideDialogue();
					GameMinigameRoundBegin();
				}
				return true;

			default:
				return false;
		}
	}
	
	void StoryEnd()
	{
		DestroyChildren(spawnArea.transform);
		DestroyChildren(rockSpawnArea.transform);
		
		minigame.CompleteMinigame();	
	}
	
	IEnumerator Phase3Tutorial()
	{
		barGlow.gameObject.SetActive(false);
		DestroyChildren(spawnArea.transform);
		DestroyChildren(rockSpawnArea.transform);
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		shootingAIHand.EnableAI();
		shootingAIHand.ChangeUnselectHandTexture();
		shootingAIHand.SetSamHand();
		int count = 0;

		Sherlock.Instance.PlaySequenceInstructions(sherlockPhase3[count], null);
		
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f );
	
		activateShootingAI = true;

		InstantiateEnemies();
		count++;
		Sherlock.Instance.PlaySequenceInstructions(sherlockPhase3[count], null);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f);
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		Sherlock.Instance.HideDialogue ();
		activateShootingAI = false;
		shootingAIHand.Reset();
		enemyBar.fillAmount = 0;
		lerpEnemyBar = 0;
		barGlow.gameObject.SetActive(true);
		barGlow.localPosition = new Vector3 (barGlow.localPosition.x + barDistance, barGlow.localPosition.y, barGlow.localPosition.z);

		GameRoundBegin();
	}

	IEnumerator Phase2Tutorial()
	{
		barGlow.gameObject.SetActive(false);

		DestroyChildren(spawnArea.transform);
		DestroyChildren(rockSpawnArea.transform);
		
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		AudioManager.Instance.PlayVoiceOver(luisVoice[currentLuisVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, luisAnims);
		yield return new WaitForSeconds(luisVoice[currentLuisVoice].length);
		currentLuisVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, luisAnims);
		
		AudioManager.Instance.PlayVoiceOver(luisVoice[currentLuisVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, luisAnims);
		yield return new WaitForSeconds(luisVoice[currentLuisVoice].length);
		currentLuisVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, luisAnims);
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		int count = 0;
		
		minigame.ContinueConversation (sherlockPhase2 [count]);
		activateTreasureAI = true;
		treasureAIHand.EnableAI();
		treasureAIHand.ChangeUnselectHandTexture();
		treasureAIHand.SetSamHand();
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f );
		InstantiateTreasures();
		count++;
		minigame.ContinueConversation (sherlockPhase2 [count]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f);
		
		count++;
		minigame.ContinueConversation (sherlockPhase2 [count]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f);
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		activateTreasureAI = false;
		treasureAIHand.Reset();
		treasureBar.fillAmount = 0;
		lerpTreasure = 0;
		Sherlock.Instance.HideDialogue ();
		barGlow.gameObject.SetActive(true);
		barGlow.localPosition = new Vector3 (barGlow.localPosition.x + barDistance, barGlow.localPosition.y, barGlow.localPosition.z);
		GameRoundBegin();
	}
	
	IEnumerator Phase1Tutorial()
	{
		DestroyChildren(spawnArea.transform);
		DestroyChildren(rockSpawnArea.transform);
		
		int count = 0;
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		//Sherlock.Instance.PlaySequenceInstructions(sherlockPhase1[count], null);
		activateMovementAI = true;
		wheel.movementAIHand.SetSamHand();
		wheel.EnableMovementAI();
		minigame.ContinueConversation (sherlockPhase1 [count]);
		yield return new WaitForSeconds (minigame.GetCurrentDialogueDuration () + 1f);
		count++;
		minigame.ContinueConversation (sherlockPhase1 [count]);
		InstantiateRocks();
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f);
		
		count++;
		minigame.ContinueConversation (sherlockPhase1 [count]);
		yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration() + 1f);
		
		AudioManager.Instance.PlayVoiceOver(samVoice[currentSamVoice],1);
		setCharacterAnimation(NPCAnimations.AnimationIndex.TALKING, samAnims);
		yield return new WaitForSeconds(samVoice[currentSamVoice].length + 1f);
		currentSamVoice++;
		setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, samAnims);

		Sherlock.Instance.HideDialogue ();
		activateMovementAI = false;
		wheel.DisableMovementAI();
		
		barGlow.gameObject.SetActive(true);
		GameRoundBegin();
	}
	
	void setCharacterAnimation(NPCAnimations.AnimationIndex animationType, NPCDialogueAnimation anim) 
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

	
	void playStart()
	{
		if (gameState == GameState.STORY && ShowDialogue(DialogueType.START) == true)
			{
				Debug.Log("Starting!!");
			}	
	}

	
	public void DestroyChildren(Transform parentTrans)
	{
		var children = new List<GameObject>();
		
		foreach (Transform child in parentTrans) children.Add(child.gameObject);
		
		children.ForEach(child => Destroy(child));
		
	}
	
	//child newly created object to an object
	public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

		if (go != null && parent != null)
		{
			go.layer = parent.layer;
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = prefab.transform.localScale;
			
		}
		
		return go;
	}
}
