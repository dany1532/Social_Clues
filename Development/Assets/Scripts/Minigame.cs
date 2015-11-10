// if IN_LEVEL_MINIGAME is defined the the minigame has been added in the level before starting (no LoadSceneAdditive was used)
#define IN_LEVEL_MINIGAME

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Minigame Controller: controls dialogue and state of minigame
/// </summary>
public class Minigame : MonoBehaviour {
	// The camera for the minigame
	public GameObject minigameCamera;
	
	// The NPC that caused the minigame
	public NPC owner;
	
	// Conversation tree of the minigame
	public ConversationTree conversationTree;
	// The background audio
	public AudioClip backgroundAudio;
	
	// Event triggered at the end of the minigame
	public SCEvent minigameEndEvent;
	
	// Prefabs to load when loading the scene
	public List<GameManager.LoadPrefab> loadPrefabs;

	// If game is running on standalone or in-level
	private bool standalone = false;
	
	public MinigameDifficulty.Difficulty difficulty = MinigameDifficulty.Difficulty.EASY;

	public Dialogue endMinigameDialogue;
	
	public void Awake()
	{		
		// If the minigame is in it's own scene (standalone) (i.e no UI set in GameManager)
		if (!GameManager.WasInitialized())
		{
			// Load prefabs
			foreach (GameManager.LoadPrefab loadPrefab in loadPrefabs)
			{
				loadPrefab.Load();
			}
			
			// Destroy game manager and start minigame
			// Destroy(GameManager.Instance.gameObject);
			
			// If minigame has a background audio, fade it in
			if (backgroundAudio != null)
				AudioManager.Instance.FadeMusic(backgroundAudio, 0.1f);
			
			standalone = true;
			MinigameDifficulty minigameDifficulty = GameObject.FindObjectOfType(typeof(MinigameDifficulty)) as MinigameDifficulty;
			if (minigameDifficulty != null)
			{
				difficulty = minigameDifficulty.difficulty;
				Destroy (minigameDifficulty.gameObject);
			}
			else
			{
				difficulty = MinigameDifficulty.Difficulty.EASY;

			}

		
		}
		else
		{
			gameObject.SetActive(false);
			// Disable the audio listener of the minigame camera
			Player.instance.interactingNPC.miniGame = this;
			// If minigame is within level, the deactivate minigame	

		}
	}
	
	#region Minigame State
	/// <summary>
	/// Starts the mini game
	/// </summary>
	public void StartMiniGame(bool returnToCauser = true)
	{
#if !IN_LEVEL_MINIGAME
		// Deactivate minigame and sprite manager, to use the ones in the scene
		minigameCamera.SetActive(false);
		spriteManager.SetActive(false);
		
		this.transform.parent = GameManager.Instance.UI.transform;
		this.transform.localPosition = new Vector3(0, 0, 500);
		this.transform.localEulerAngles = Vector3.zero;
#endif
		
		//GameManager.Instance.OT.SetActive(false);
		//view.cameraOverride  = GameManager.Instance.uiCamera;
		//this.gameObject.SetActive(false);


		
		// If  the player instance is set (we are running in the scene)
		if (Player.instance != null)
		{
			// Disable the HUD
			HUD.Instance.gameObject.SetActive(false);
			
			// Notify player for the beginning of the minigame
			Player.instance.BeginMinigame(this);
		}
		
		// Set information of event at the end of the minigame
		minigameEndEvent.SetEventCauser(owner, returnToCauser);
		minigameEndEvent.AddObjectToNotify(gameObject);
			
		// If minigame has a background audio, fade it in
		if (backgroundAudio != null)
			AudioManager.Instance.FadeMusic(backgroundAudio, 0.1f);
	}

	public void CompleteIfStandalone ()
	{
		CompleteMinigame();
	}
	
	/// <summary>
	/// Completes the minigame
	/// </summary>
	public void CompleteMinigame ()
	{	
		//view.cameraOverride = null;
		// If the owner of the minigame is set (i.e. we are running the minigame within the level)
		if (owner != null)
		{
			if (endMinigameDialogue != null)
				owner.conversationTree.SetDialogue(endMinigameDialogue);
			else
				// Go to next dialogue
				owner.conversationTree.ContinueToNextDialogue();

			// Switch UI Camera back to default for level
			GameManager.Instance.SwitchUICamera(null);
			
			if (minigameEndEvent.location != null)
			{
				// Re-enable HUD
				HUD.Instance.gameObject.SetActive(true);
				NPCs.Instance.EnableInput();
				gameObject.SetActive(false);
				Player.instance.MoveToNPC(minigameEndEvent.location);
			}
			else
			{
				Player.instance.TalkNPC(owner, true);
				gameObject.SetActive(false);
			}
            
            AudioManager.Instance.PlayBackgroundMusic(.1f);
		}
		
		// If the player instance is set
		if (Player.instance != null)
		{
			// Notify player about the end of the minigame
			Player.instance.EndMinigame();
		}
		
		Sherlock.Instance.SetBubblePosition(Sherlock.side.RIGHT);
		Sherlock.Instance.HideDialogue();
		
		if (standalone)
		{
			Time.timeScale = 0;
			ApplicationState.Instance.LoadLevelAdditive(ApplicationState.LevelNames.MINIGAME_COMPLETE);
		}
	}
	#endregion
	
	#region Conversation
	/// <summary>
	/// Starts the dialogue in the minigame
	/// </summary>
	/// <returns>
	/// Return true if there is a conversation tree and the text was displayed on screen
	/// </returns>
	public bool StartConversation()
	{
		// Check if there is a conversation tree
		if (conversationTree != null)
		{
			// Start the dialogue
			return conversationTree.StartConversation();
		}
		
		return false;
	}
	
	/// <summary>
	/// Continues current dialogue.
	/// </summary>
	/// <returns>
	/// Return true if there is a conversation tree and the text was displayed on screen
	/// </returns>
	public bool ContinueConversation()
	{
		// Check if there is a conversation tree
		if (conversationTree != null && conversationTree.currentNode != null )
		{
			// Update to the next dialogue
			conversationTree.ShowNextDialogue();
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Continue conversation, but set to specific dialogue
	/// </summary>
	/// <returns>
	/// Return true if there is a conversation tree and the text was displayed on screen
	/// </returns>
	/// <param name='dialogue'>
	/// The next dialogue of the conversation
	/// </param>
	public bool ContinueConversation(Dialogue dialogue)
	{
		// Check if there is a conversation tree
		if (conversationTree != null)
		{
			// Update conversation to input dialogue
			return conversationTree.ShowCurrentDialogue(dialogue);
		}
		
		return false;
	}
	#endregion

	public float GetCurrentDialogueDuration ()
	{
		if (conversationTree != null && conversationTree.currentNode != null)
		{
			if (conversationTree.currentNode.voiceOver != null)
				return conversationTree.currentNode.voiceOver.length;
			else
				return 2f;
		}
		return 0;
	}

	public float GetCurrentDialogueComulativeDuration ()
	{
		float duration = 0;
		if (conversationTree != null && conversationTree.currentNode != null)
		{
			Dialogue currentNode = conversationTree.currentNode;
			
			while (currentNode != null)
			{
				duration += currentNode.voiceOver.length;
				currentNode = currentNode.nextDialogue;
			}
		}
		
		return duration;
	}

	public bool isStandalone()
	{
		return standalone;
	}


}
