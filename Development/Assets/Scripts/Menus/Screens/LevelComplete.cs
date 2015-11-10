using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelComplete : MonoBehaviour
{
    // Sprites for Odd and Even toys
    public List<UIScaledSprite> toysOdd;
    public List<UIScaledSprite> toysEven;
	
    private List<UIScaledSprite> activeList;
    public UIAtlas finalToyAtlas;
    public UIAtlas initialToyAtlas;
	public UISprite player;
    
    // Maximum number of toys in a level
    private int maxNoToys = 4;
    private int startingIndex = 0;
    private int finalIndex = 0;
    private List<Toy> toys;
    private int currentToy = 0;
	
    // Animation game object along with the associated animatino
    public GameObject animationObject;
    private UIScaledSprite animationSprite;
    private PositionAnimationFX positionAnimation;
    private ScaleAnimationFX scaleAnimation;
    private Vector3 initialPosition;
    private Vector2 initialScale;
    public float timeBetweenRewards = 0.5f;
    public float timeForRewards = 1f;
	
    public AudioClip backgroundMusic;
	
    public Dialogue message;
    public GameObject uiRoot;
	
    // Use this for initialization
    void Start()
    {
        if (animationObject != null)
        {
            positionAnimation = animationObject.GetComponent<PositionAnimationFX>();
            scaleAnimation = animationObject.GetComponent<ScaleAnimationFX>();
            animationObject.SetActive(false);
			
            animationSprite = new UIScaledSprite();
            animationSprite.SetComponents(animationObject.GetComponent<UISprite>(), animationObject.GetComponent<UIStretch>());
			
            initialPosition = animationSprite.GetPosition();
            initialScale = animationSprite.GetRelativeScale();
        }
		
        toys = new List<Toy>(Mathf.Max(toysOdd.Count, toysEven.Count));
        for (int i = 0; i < toys.Capacity; i++)
            toys.Add(new Toy());
		
        foreach (UIScaledSprite sprite in toysOdd)
        {
            sprite.Initialize();
            sprite.Disable();
        }
		
        foreach (UIScaledSprite sprite in toysEven)
        {
            sprite.Initialize();
            sprite.Disable();
        }
        activeList = toysOdd;
		
        if (GameManager.WasInitialized())
        {
            GameManager.Instance.InitializeLevelCompleteSequence(this);
        }
		
        uiRoot.SetActive(false);
        Sherlock.Instance.PlaySequenceInstructions(message, null);
		player.spriteName = ApplicationState.Instance.selectedCharacter + "CircleColor";
        Invoke("ShowLevelComplete", message.GetCommulativeDuration() + 0.5f);
    }
		
    public void ShowLevelComplete()
    {
        uiRoot.SetActive(true);		
        AudioManager.Instance.PlayMusic(backgroundMusic, 1);
        StartLevelCompletionSequence();
    }
	
    /// <summary>
    /// Resets all the toys
    /// </summary>
    /// <param name='noToys'>
    /// No toys.
    /// </param>
    public void ResetToys(int noToys)
    {
        maxNoToys = noToys;
		
        foreach (UIScaledSprite sprite in activeList)
        {
            sprite.Disable();
        }
		
        switch (maxNoToys)
        {
            case 0:
                activeList = toysEven;
                startingIndex = 0;
                break;
            case 1:
                activeList = toysOdd;
                startingIndex = 1;
                break;
            case 2:
                activeList = toysEven;
                startingIndex = 1;
                break;
            case 3:
                activeList = toysOdd;
                startingIndex = 0;
                break;
            case 4:
            default:
                activeList = toysEven;
                startingIndex = 0;
                break;
        }
        currentToy = startingIndex;
        finalIndex = startingIndex + maxNoToys;
		
        for (int i = 0; i < activeList.Count; i++)
        {
            if (i >= startingIndex && i < finalIndex)
            {
                activeList [i].Enable();
            } else
            {
                activeList [i].Disable();
            }
        }
    }
	
    /// <summary>
    /// Sets the next toys' information
    /// </summary>
    /// <param name='toyName'>
    /// Toy name.
    /// </param>
    /// <param name='filename'>
    /// Filename.
    /// </param>
    /// <param name='toyColor'>
    /// Toy color.
    /// </param>
    public void SetToy(string toyName, string filename, Color toyColor)
    {
        if (currentToy < finalIndex)
        {
            toys [currentToy].displayName = toyName;
            toys [currentToy].filename = filename;
            toys [currentToy].color = toyColor;
			
            activeList [currentToy].SetSprite(initialToyAtlas, filename, Color.white);
            currentToy++;
        }
    }
	
    /// <summary>
    /// Sets the next toys' information
    /// </summary>
    /// <param name='toy'>
    /// Toy.
    /// </param>
    public void SetToy(Toy toy)
    {
        if (currentToy < finalIndex && toy != null)
        {
            toys [currentToy].displayName = toy.displayName;
            toys [currentToy].filename = toy.filename;
            toys [currentToy].color = toy.color;
			
            activeList [currentToy].SetSprite(initialToyAtlas, toy.filename, Color.white);
            currentToy++;
        }
    }
	
    /// <summary>
    /// Start level completion sequence
    /// </summary>
    public void StartLevelCompletionSequence()
    {
        currentToy = startingIndex;
        Invoke("DisplayToy", timeBetweenRewards);
    }
    /// <summary>
    /// Calls function to start displaying the toys
    /// </summary>
    private void EndDisplayToy(ScaleAnimationFX scAnim, string whatEffect)
    {	
        activeList [currentToy].UpdateColor(finalToyAtlas, toys [currentToy].color);
        animationObject.SetActive(false);
        // Go to next toy
        currentToy++;
		
        Invoke("DisplayToy", timeBetweenRewards);
    }
	
    private void DisplayToy()
    {	
        if (currentToy < startingIndex || currentToy >= finalIndex)
            return;
		
        //Set to initial state
        animationObject.transform.localPosition = initialPosition;
        animationSprite.UpdateSprite(toys [currentToy].filename, toys [currentToy].color);
        animationSprite.UpdateStretching(initialScale);
		
        //Reset AnimationFX
        positionAnimation.Reset();
        scaleAnimation.Reset();
				
        //initialize animation values and set up end animation event
        positionAnimation.InitializePositionLerp(initialPosition, activeList [currentToy].GetPosition() - Vector3.forward, false);
        positionAnimation.duration = timeForRewards;
		
        scaleAnimation.IntializeScaleLerp(animationSprite.GetScale(), activeList [currentToy].GetScale());
        scaleAnimation.duration = timeForRewards;
        scaleAnimation.animationCompleteDelegate = EndDisplayToy;
		
        animationObject.gameObject.SetActive(true);
		
        //Play animations
        positionAnimation.PlayAnimation();
        scaleAnimation.PlayAnimation();
    }
	
    /// <summary>
    /// Display the final images for the toys
    /// </summary>
    public void EndLevelCompletionSequence()
    {
        if (currentToy < finalIndex)
        {
            for (int i = currentToy; i < finalIndex; i++)
            {
                activeList [i].UpdateColor(finalToyAtlas, toys [i].color);
            }
            Invoke("LoadAnalyitcsScreen", 0.5f);
        } else
        {
            LoadAnalyitcsScreen();
        }
    }
	
    private void LoadAnalyitcsScreen()
    {
		GameManager.DestroyGameLevel();
		GameManager.DestroyInstance();
        ApplicationState.Instance.LoadLevelWithLoading(ApplicationState.LevelNames.PLAY_ANALYTICS);
    }
}
