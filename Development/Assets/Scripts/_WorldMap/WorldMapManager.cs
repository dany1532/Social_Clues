using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMapManager : MonoBehaviour {
	public enum Navigation { FORWARD, BACK };
	public enum Difficulty { EASY, MED, HARD};
	
	public Navigation currentDirection = Navigation.FORWARD;
	
	public List<Transform> levelPositions;
	public float nextLevelDuration = 1.0f;
	public Transform playerTransform;
	public NPCDialogueAnimation playerAnim;
	//public FadeEffect stepsAnim;
	public FilledFadeIn stepsAnim;

	public FadeEffect schoolAnim;
	public FadeEffect schoolDotAnim;
	public GameObject starParticles;
	public TweenScale levelOptions;
	public GameObject selectLevelsObject;
	public GameObject selectDifficultyObject;
	
	
	string levelToLoad = "";
	Difficulty difficulty = Difficulty.EASY;
	float lerpMovement = 0f;
	public bool canMove = false;
	bool reachedDestination = false;
	bool displayLevelsOption = false;
	
	Transform goingFrom;
	Transform goingTo;
	Transform finalDestination;
	int destinationLevel;
	int currentLevel = 0;
	
	static bool worldMapUnlocked = false;
	public AudioClip backgroundMusic;
	// Use this for initialization
	void Start () 
	{
		AudioManager.Instance.FadeMusic (backgroundMusic, 0.5f);
		goingFrom = levelPositions[0];
		playerTransform.position = goingFrom.position;
		
		if(!worldMapUnlocked)
		{
			Invoke("InitTransition", 1f);
		}
		
		else
		{
			stepsAnim.Appear();
			stepsAnim.gameObject.SetActive(true);
			schoolAnim.gameObject.SetActive(false);
			schoolDotAnim.gameObject.SetActive(true);
		}
	}

	public void InitTransition()
	{
		stepsAnim.PlayFadeIn();
		schoolAnim.PlayFadeOut();
		schoolDotAnim.PlayFadeIn();
		CreateParticles(schoolAnim.transform.position);
		GoToLevelPosition(1, true, "");
		worldMapUnlocked = true;
	}
	public void DisplayOptions()
	{
		if(displayLevelsOption){
			selectLevelsObject.SetActive(true);	
		}
		
		else{
			selectDifficultyObject.SetActive(true);	
		}
		
		levelOptions.gameObject.SetActive(true);
		levelOptions.Reset();
		levelOptions.Play(true);
	}
	
	public void HideOptions()
	{
		selectLevelsObject.SetActive(false);
		selectDifficultyObject.SetActive(false);
		levelOptions.gameObject.SetActive(false);
	}
	
	void CreateParticles(Vector3 pos)
	{
		GameObject particles = Instantiate(starParticles) as GameObject;
		particles.transform.position = pos;
	}
	
	public void SelectLevel(string levelName)
	{
		levelToLoad = levelName;	
		
	}
	
	public void DisplayDifficulty()
	{
		selectLevelsObject.SetActive(false);
		selectDifficultyObject.SetActive(true);	
		levelOptions.gameObject.SetActive(true);
	}
	
	public void SetDifficulty(Difficulty diff)
    {
       	difficulty = diff;
		LoadLevelSetUpScene();
    }
	
	public void GoToLevelPosition(int idLevel, bool showLevels, string levelName = "")
	{
		levelToLoad = levelName;
		HideOptions();
		displayLevelsOption = showLevels;
		canMove = false;
		reachedDestination = false;
		finalDestination = levelPositions[idLevel];
		goingTo = levelPositions[idLevel];
		goingFrom = playerTransform;
		
		if(currentLevel < idLevel)
		{
			currentDirection = Navigation.FORWARD;
			canMove = true;
			destinationLevel = idLevel;
			setCharacterAnimation(NPCAnimations.AnimationIndex.WALKING, playerAnim);
		}
		else if(currentLevel > idLevel)
		{
			currentDirection = Navigation.BACK;	
			goingTo = levelPositions[idLevel];
			canMove = true;
			destinationLevel = idLevel;
			setCharacterAnimation(NPCAnimations.AnimationIndex.WALKING, playerAnim);
			playerTransform.Rotate(new Vector3(0,180,0));
		}
		else{
			reachedDestination = true;
			DisplayOptions();
		}
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
	
	 public void LoadLevelSetUpScene()
	{
        ApplicationState.Instance.LoadLevelWithLoading(ApplicationState.Instance.GetLevelFromName(levelToLoad), MenuButton.MenuType.None);	
        Destroy(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () 
	{
		if(canMove)
		{
			if(lerpMovement < 1)
			{
				lerpMovement += Time.deltaTime/nextLevelDuration;
				playerTransform.position = Vector3.Lerp(goingFrom.position, goingTo.position, lerpMovement);
				//Debug.Log(Vector3.Distance(playerTransform.position, goingTo.position));
				//if(playerTransform.position == finalDestination.position)
				if(Vector3.Distance(playerTransform.position, goingTo.position) < 0.005)
				{
					canMove = false;	
					reachedDestination = true;
					currentLevel = destinationLevel;
					lerpMovement = 0;
					playerTransform.position = goingTo.position;
					setCharacterAnimation(NPCAnimations.AnimationIndex.IDLE, playerAnim);
					if(currentDirection == Navigation.BACK){
						playerTransform.Rotate(new Vector3(0,180,0));
					}
					DisplayOptions();
				}
			}
				
			else
			{
				
				lerpMovement = 0;
				playerTransform.position = goingTo.position;
				if(currentDirection == Navigation.FORWARD)
					currentLevel++;
				else
					currentLevel--;
				
			}
		}
	}
}
