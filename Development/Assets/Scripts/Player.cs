using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player controller
/// </summary>
public class Player : MonoBehaviour {
	
	
	// Instance to the player
	public static Player instance;
	
	
//	//2D navigation
//	#region Navigation
//	// Navigation for the player
//	PlayerNavigation playerNavigation;
//	// The default camera in the scene
//	//public Camera sceneCamera;
//	// Player's last frame position
//	//public Vector3 previousPosition;
//	// Player's current position
//	//public Vector3 currentPosition;
//	// Angle made from previous position to current (0 - 360)
//	public float currentAngle;
//	// The current direction (to get 181 - 360 degrees)
//	private float whichDirection;
//	// Whether the player is walking or not
//	private bool walking = false;
//	private bool startedWalking = false;
//	#endregion
	
	//3d Navigation
		#region Navigation
	// Navigation mesh agent for the player
	NavMeshAgent playerNavigation;
	// The default camera in the scene
	public Camera sceneCamera;
	// Player's last frame position
	public Vector3 previousPosition;
	// Player's current position
	public Vector3 currentPosition;
	// Angle made from previous position to current (0 - 360)
	public float currentAngle;
	// The current direction (to get 181 - 360 degrees)
	private float whichDirection;
	// Whether the player is walking or not
	private bool walking = false;
	private bool startedWalking = false;
	public bool cutscene = false;
	Quaternion initialRotation;
	#endregion
	
	public PlayerInteractionBubble interactionBubble;
	public GameObject interactionBubblePrefab;
    public float npcInteractionDistance = 7;
    
	public NPCAnimations animationsList;
    public NPCDialogueAnimation animationControl;
    public FlipTexture animationFlip;
    public NPCAnimations.AnimationIndex currentAnimation = NPCAnimations.AnimationIndex.IDLE;
    
	public GameObject eyeContactDemo;
	public bool talkToNPC = false;
	
	public float runnignSpeed = 20;
	public float runningAcceleration = 10;
	public float walkingSpeed = 10;
	public float walkingAcceleration = 5;

	private float winningCelebrationTime;
	public GameObject toyInRightHand;
	public GameObject toyInLeftHand;

	public enum InteractionState
	{
		NONE,
		IN_INTROANIM,
		NEAR_NPC,
		TALKING_NPC,		
		APPROACH_EVENT,
		IN_EVENT,
		APPROACH_NPC
	}
	public InteractionState _interactionState = InteractionState.NONE;
	public InteractionState interactionState
	{
		get { return _interactionState; }
		set {
			// if current interaction state is IN_EVENT or TALKING_NPC then remember that an interaction just finished
			if (_interactionState == InteractionState.IN_EVENT || _interactionState == InteractionState.TALKING_NPC)
			{
				finishedInteraction = true;
			}
			
			_interactionState = value; 
		}
	}
	bool finishedInteraction = false;
	
	// NPC interacting with player
	public NPC interactingNPC = null;
	
	private float farDistance;
	private float nearDistance = 50;
	
	public float nearScale = 0.095f;
	public float farScale = 0.011f;
	
	public float scalePlayerRatio;
	public float scaleCameraRatio;
	
	public UIStretch animationStretch;
	public Transform spriteScale;
	
	void Awake()
	{
		// Set static instance to this object
		instance = this;
		initialRotation = transform.rotation;
		
		if (GameManager.WasInitialized () && GameManager.Instance.playerPosition != null) {
			this.transform.position = GameManager.Instance.playerPosition.position;
		}
		
		// Find the navigation mesh agent in the gameobject (3D)
		playerNavigation = GameObject.FindObjectOfType(typeof(NavMeshAgent)) as NavMeshAgent;
		// Find the main camera (3D)
		sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		
		if(!GameObject.Find("FarDistance_Pos")){
			farDistance = Vector3.Distance(sceneCamera.transform.position, transform.position);
		}
		
		else{
			//Debug.Log("Found Far Distance Pos");
			Vector3 farDistancePos = GameObject.Find("FarDistance_Pos").transform.position;
			farDistance = Vector3.Distance(sceneCamera.transform.position, farDistancePos);
		}
		scaleCameraRatio = farDistance - nearDistance;

		GameObject spritePrefab = ResourceManager.LoadObject ("Player/" + ApplicationState.Instance.selectedCharacter + "Sprite");
		GameObject spriteInstance = Instantiate (spritePrefab) as GameObject;
		spriteInstance.transform.parent = transform;
		spriteInstance.transform.localPosition = new Vector3(0,0.76f,0);
		
        animationsList = spriteInstance.GetComponent<NPCAnimations> ();
        animationControl = spriteInstance.GetComponent<NPCDialogueAnimation> ();
        animationFlip = spriteInstance.GetComponent<FlipTexture> ();
        
		animationStretch = GetComponentInChildren<UIStretch>();
		
		spriteScale = spriteInstance.transform;
		scalePlayerRatio = farScale - nearScale;

		spritePrefab = null;
		Resources.UnloadUnusedAssets ();
	}
	
	// Use this for initialization
	void Start () {
		
		// Find the navigation mesh agent in the gameobject (2D)
		//playerNavigation = GetComponent<PlayerNavigation>();
		
		// Set interaction state to none
		if(interactionState != InteractionState.IN_INTROANIM){
			interactionState = InteractionState.NONE;
			
			// Make the player stand
			Stand();
		}
		
		GameObject interactionGO = Instantiate(interactionBubblePrefab) as GameObject;
		interactionGO.transform.parent = GameObject.Find("GUICamera").transform;
		interactionBubble = interactionGO.GetComponent<PlayerInteractionBubble>();
		NormalizeHeight();
		InvokeRepeating("NormalizeHeight", 0, 0.5f);
	}
	
	void NormalizeHeight()
	{
		float distanceFromCamera = Vector3.Distance(GameManager.Instance.mainCamera.transform.position, transform.position);
		animationStretch.relativeSize.y = (distanceFromCamera - nearDistance) / scaleCameraRatio * scalePlayerRatio + nearScale;
	}
	
	void Update()
	{
		//Depth calculations (2D)
//		if(listReferenceObjects.Length == 0){
//			listReferenceObjects = GameObject.FindGameObjectsWithTag("Depth_Reference");	
//		}
//		
//		Vector3 newDepth;
//		foreach(GameObject go in listReferenceObjects){
//			if(playerNavigation.Distance(go.gameObject.transform.position) < 4){
//				if(this.transform.position.y > go.transform.position.y){
//					newDepth = new Vector3();
//					newDepth = go.transform.position;
//					newDepth.z = this.transform.position.z - 0.5f;
//					go.transform.position = newDepth;
//				}
//				else{
//					newDepth = new Vector3();
//					newDepth = go.transform.position;
//					newDepth.z = this.transform.position.z + 0.5f;
//					go.transform.position = newDepth;
//				}
//			}
//		}
		// If the player is approaching an event
		
		if (interactionState == InteractionState.APPROACH_EVENT)
		{
			// and it's close enough to the target event
			if (interactingNPC != null && interactingNPC.conversationTree != null)
			{
				// Get current dialogue from npc and retrieve the actual event and trigger it again
				SCEvent dialogueEvent = interactingNPC.conversationTree.GetCurrentEvent();
				
				// If there is a dialogue event associated with the current dialogue and the player is close enough
				//if (dialogueEvent != null && PlayerDistance(dialogueEvent.location.position) < 3f)
				if (dialogueEvent != null && playerNavigation.remainingDistance < 3f)
				{
					interactionState = InteractionState.IN_EVENT;
					// Trigger the event
					dialogueEvent.TriggerEvent(true);
				}
			}
		}
		else if (interactionState == InteractionState.APPROACH_NPC)
		{
			// If the player is close enough
			if (playerNavigation.remainingDistance < 3)
			{
				TalkNPC(interactingNPC, true);
			}
		}
		
		// If the player is walking
		//if (playerNavigation.walking)
		if(walking)
		{
			// If the player is close to the destination
			//if (playerNavigation.remainingDistance < 1 /*&& !startedWalking*/)
			if (playerNavigation.remainingDistance < 0.1 /*&& !startedWalking*/)
			{
				// Make him stand
				Stand();
			}
			else // Otherwise update animation sprite
			{
				//startedWalking = false;
				
				GetCurrentAngle();
				
				//Get angle (2D)
				//currentAngle = playerNavigation.GetCurrentAngle();
				
				Walk(7);

				previousPosition = transform.position;
			}
		}
		
		else
			Stand();
		
		if(interactionState == InteractionState.IN_INTROANIM && playerNavigation.remainingDistance < 1f){
			Stand();
			cutscene = false;
			interactionState = InteractionState.NONE;
		}
		
		
		// If the player clicks on screen
		if (InputManager.Instance.HasReceivedClick() && !cutscene)
		{
			// if the player is approaching an event
			if (interactionState == InteractionState.APPROACH_EVENT)
			{
			}
			// if the player is in an event
			else if (interactionState == InteractionState.IN_EVENT)
			{
			}
			// if the player is talking to an NPC
			else if (interactionState == InteractionState.TALKING_NPC)
			{
			}
			else // otherwise
			{
				// if the player has not just finished an interaction with an NPC or an event
				if (/*false && */!finishedInteraction && !InputManager.Instance.HasReceivedUIInput())
				{	
					// Get ray from given input position
					RaycastHit hit;
					Vector3 screenPoint = Input.mousePosition;
					Ray sceneRay = sceneCamera.ScreenPointToRay(screenPoint);
					// and raycast it in scene
					Physics.Raycast(sceneRay, out hit);
					// and set destination to given position
					//playerNavigation.SetDestination(UICamera.lastHit.point);
					NavMeshPath possiblePath = new NavMeshPath();
					if (playerNavigation.CalculatePath(hit.point, possiblePath))
					{
						playerNavigation.SetDestination(hit.point);

	                     if (Vector3.Distance (hit.point, transform.position) > 30)
	                     {
	                         animationControl.SetFPS(8);
	                         playerNavigation.speed = runnignSpeed;
	                         playerNavigation.acceleration = runningAcceleration;
	                 
	                         // Start walking animation
	                         Walk(8);
	                     }
	                     else
	                     {
	                        animationControl.SetFPS(6);
	                        playerNavigation.speed = walkingSpeed;
	                        playerNavigation.acceleration = walkingAcceleration;
	                        
	                        // Start walking animation
	                        Walk(6);
	                     }
					}
				}
			}
		}
//			// If player is near NPC
		if (interactionState == InteractionState.NEAR_NPC && interactingNPC != null)
			{
				//if the player is close to the NPC
				if (playerNavigation.remainingDistance < 25 && Vector3.Distance(transform.position, interactingNPC.transform.position) < 18)
				{
					// and he / she has stopped walking
					if (playerNavigation.desiredVelocity.magnitude < 0)
					{
						// then stop player navigation
						playerNavigation.Stop();
					}
				}
			
				// If navigation speed is 0
				if (playerNavigation.speed == 0)
					Stand(); // then change to standing animation
			
			//player nav stop (2D)
			/*
				playerNavigation.Stop();
				// If navigation speed is 0
				if (!playerNavigation.walking)
					// then change to standing animation
			*/
								
			}
		//}
	}
	
	public void LateUpdate()
	{
		// Remember that we didn't have in interaction with an NPC or an event
		finishedInteraction = false;
		// Reset play rotation
		transform.rotation = initialRotation;
	}
	
	public void SetFinishedInteraction(){
		finishedInteraction = true;	
	}
	
	//Navigate to a desire location (3D)
	public void Navigate (Vector3 desiredLoc, bool isCutscene)
	{
		// if the player is approaching an event
		if (interactionState != InteractionState.APPROACH_EVENT && interactionState != InteractionState.IN_EVENT && interactionState != InteractionState.TALKING_NPC)
		{
			// if the player has not just finished an interaction with an NPC or an event
			if (!finishedInteraction && !cutscene)// && !InputManager.Instance.HasReceivedUIInput())
			{	
				playerNavigation.SetDestination(desiredLoc);
				
				// Start walking animation
				Walk(7);
				//startedWalking = true;
				//SetGoToCharacter(false);
			}
		}
	}
	
	//Navigate to a desired Location (2D)
//	public void Navigate (Vector3 point, bool isCutscene)
//	{
//		// if the player is approaching an event
//		if (interactionState != InteractionState.APPROACH_EVENT && interactionState != InteractionState.IN_EVENT && interactionState != InteractionState.TALKING_NPC)
//		{
//			// if the player has not just finished an interaction with an NPC or an event
//			if (!finishedInteraction)// && !InputManager.Instance.HasReceivedUIInput())
//			{	
//				// Get ray from given input position
//				//RaycastHit hit;
//				//Vector3 screenPoint = Input.mousePosition;
//				//Ray sceneRay = sceneCamera.ScreenPointToRay(screenPoint);
//				// and raycast it in scene
//				//Physics.Raycast(sceneRay, out hit);
//				// and set destination to given position
//				playerNavigation.SetDestination(point, isCutscene);
//				
//				// Start walking animation
//				Walk();
//				startedWalking = true;
//				SetGoToCharacter(false);
//			}
//		}
//	}
	
	//Get the current Angle (3D)
	public void GetCurrentAngle(){
		
		currentPosition = transform.position - previousPosition;
		//whichDirection = Vector3.Cross(transform.right, currentPosition).y;
		whichDirection = Vector3.Cross(sceneCamera.transform.right, currentPosition).y;
				
		if(whichDirection > 0){
			//currentPosition = previousPosition - transform.position;
			currentAngle = Vector3.Angle(sceneCamera.transform.right, currentPosition) ;	
		}
		
		else {
			currentPosition = previousPosition - transform.position;
			currentAngle = Vector3.Angle(sceneCamera.transform.right, currentPosition)+ 180f;
		}
	}
	
	public Vector3 GetPlayerScreenPosition(){
		UITexture texture = GetComponentInChildren<UITexture>();
		Bounds bla = NGUIMath.CalculateAbsoluteWidgetBounds(texture.transform);
		Vector3 woot = bla.center;
		woot.y = bla.center.y + bla.extents.y;

		//return bla.max;
		return Camera.main.WorldToScreenPoint(woot);
		//return Camera.main.WorldToScreenPoint(this.transform.position);
	}
	
	
	public float GetDistance(Vector3 desiredLoc){
		//Debug.Log(transform.position);
		return Vector3.Distance(animationControl.GetPostion(), desiredLoc);
	}
	
	
//	//Get Distance according to position given (2D)
//	public float PlayerDistance (Vector3 position)
//	{
//		return playerNavigation.Distance(position);
//	}
//	
	//Set state to cutscene: player can't move (2D-3D)
	public void SetCutscene(bool what){
		cutscene = what;
	}
//	
//	//Set state to go to character, can move (2D)
//	public void SetGoToCharacter(bool what){
//		playerNavigation.goToCharacter = what;	
//		
//	}
	
	
	public void SetInteractiveBubbleNPC(NPC npc){
		interactionBubble.SetNPC(npc);
	}
	
	/// <summary>
	/// Switch to standing animation
	/// </summary>
	public void Stand()
	{
		//change to idle standing
         PlayAnimation(NPCAnimations.AnimationIndex.IDLE, 2);
		
		//Stop the nav agent (3D)
		playerNavigation.Stop();
		
		// Mark player standing
		walking = false;
		//playerNavigation.walking = false;
	}
	
	
	/// <summary>
	/// Switch to walking animation
	/// </summary>
	void Walk(int fps)
	{
		//Walk Right
		if(currentAngle <= 94f || currentAngle >= 276f){
            PlayAnimation(NPCAnimations.AnimationIndex.WALKING, fps);
			animationFlip.FlipHorizontal(false);	
		}
		
		//Walk Left
		else if(currentAngle >= 97f || currentAngle <= 273f){
			PlayAnimation(NPCAnimations.AnimationIndex.WALKING, fps);
			animationFlip.FlipHorizontal(true);
		}
		
		// Mark player wakling
		walking = true;
		//playerNavigation.walking = true;
	}
	
    public void PlayAnimation(NPCAnimations.AnimationIndex animationIndex, int fps = -1)
    {
        if (currentAnimation != animationIndex)
        {
            animationControl.StopAnimation();
			NPCAnimations.AnimationSequence sequence = animationsList.RetrieveAnimationSequence(animationIndex);
            animationControl.SetAnimationList(sequence.textures);
			animationControl.SetSpeed(sequence.speed);
            animationControl.PlayAnimation();
            currentAnimation = animationIndex;
        }
            
        if (fps > 0)
            animationControl.SetFPS(fps);
    }
    
	public void PlayRewardPortrait(Texture winningSprite, Texture missingToy, Color foundToyColor, float winningCelSec){
//		animationSprite.ChangeAnimation(AnimationPrefix.PLAYER_REWARD_PORTRAIT);
//		animationSprite.Play();
		//animationSprite.gameObject.GetComponent<UITextureAnimation>().enabled = false;
		//animationSprite.gameObject.GetComponent<UITexture>().mainTexture = winningSprite;
        
        PlayAnimation(NPCAnimations.AnimationIndex.RECEIVE_TOY, 2);
        
		UISprite toySprite;
		if(currentAngle <= 94f || currentAngle >= 276f){
			toySprite = toyInLeftHand.GetComponent<UISprite>();
		}
		//Walk Left
		else{
			toySprite = toyInRightHand.GetComponent<UISprite>();
			//toyInHand.transform.position = new Vector3(0,0,0);
		}

		toySprite.spriteName = missingToy.name;
		toySprite.color = foundToyColor;
		toySprite.gameObject.SetActive(true);

		Stand();
		interactionState = InteractionState.NONE;
		interactingNPC = null;
		SetCutscene(false);
		winningCelebrationTime = winningCelSec;
		StartCoroutine(ResumeWalkingAnimation());
	}
	IEnumerator ResumeWalkingAnimation()
	{
		yield return new WaitForSeconds(winningCelebrationTime*10*Time.deltaTime);
		//animationSprite.gameObject.GetComponent<UITextureAnimation>().enabled = true;
        PlayAnimation(NPCAnimations.AnimationIndex.IDLE, 2);
		toyInLeftHand.SetActive(false);
		toyInRightHand.SetActive(false);
	}
	
	public void ResetState() {
		Stand();
		interactionState = InteractionState.NONE;
		interactingNPC = null;
		SetCutscene(false);
	}
	
	/// <summary>
	/// Start player - NPC conversation
	/// </summary>
	/// <param name='npc'>
	/// NPC to interact with
	/// </param>
	/// <param name='force'>
	/// Force the interactino even if player is already talking with someone
	/// </param>
	public void TalkNPC (NPC npc, bool force = false)
	{
		// If player is not talking with someone, or it's a force interaction
		if (interactionState != InteractionState.TALKING_NPC || force)
		{
			// Make player stand
			Stand();
			
			// Switch player interaction state to talking with NPC
			interactionState = InteractionState.TALKING_NPC;
			
			if (npc != null)
			{
				interactingNPC = npc;
				//Debug.Log("Clear NPC");
			}
			
			// and start conversation with NPC
			interactingNPC.StartConversation();
		}
	}
	
	public void PreDialogueMinigameStart(NPC npc) {
		interactingNPC = npc;
		interactionState = InteractionState.TALKING_NPC;
		npc.StartEvent ();
	}
	
	//Stop the player navigation (2D and 3D)
	public void StopPlayerNavigation(){
		playerNavigation.Stop();
		playerNavigation.velocity = Vector3.zero;
		Stand();
		SetCutscene(true);
	}
	
	public void StopNavigation()
	{
		playerNavigation.Stop();
		playerNavigation.velocity = Vector3.zero;
		Stand();
	}
	
	// Handle game events
	#region Events / Minigame control
	/// <summary>
	/// Handle Event: MoveTo (2D)
	/// </summary>
	/// <param name='target'>
	/// Target location transform
	/// </param>
//	public void MoveToEvent(Transform target)
//	{
//		// Set navigation mesh agent target
//		playerNavigation.SetDestination(target.position, true);
//		
//		// Make player walk
//		Walk();
//		
//		// Set interaction state to approaching event
//		interactionState = InteractionState.APPROACH_EVENT;
//	}
	
	/// <summary>
	/// Handle Event: MoveTo (3D)
	/// </summary>
	/// <param name='target'>
	/// Target location transform
	/// </param>
	public void MoveToEvent(Transform target)
	{
		// Set navigation mesh agent target
		playerNavigation.SetDestination(target.position);
		
		// Make player walk
		Walk(7);
		
		// Set interaction state to approaching event
		interactionState = InteractionState.APPROACH_EVENT;
	}
	
	public void MoveToDestination(Vector3 pos)
	{
		//Debug.Log(pos);
		// Set navigation mesh agent target
		playerNavigation.SetDestination(pos);
		Walk(7);
	}
	
	public void PlayIntroAnimation(Vector3 target){
		cutscene = true;
		playerNavigation.speed = 8f;
		playerNavigation.SetDestination(target);
		Walk(6);
		interactionState = InteractionState.IN_INTROANIM;
	}
	
	/// <summary>
	/// Handle Event: MoveTo (3D)
	/// </summary>
	/// <param name='target'>
	/// Target location transform
	/// </param>
	public void MoveToNPC(Transform target)
	{
		// Set navigation mesh agent target
		playerNavigation.SetDestination(target.position);
		
		// Make player walk
		Walk(7);
		
		// Set interaction state to approaching event
		interactionState = InteractionState.APPROACH_NPC;
	}
	/// <summary>
	/// Handle Event: BeginMinigame
	/// </summary>
	/// <param name='minigame'>
	/// Minigame
	/// </param>
	public void BeginMinigame(Minigame minigame)
	{
		// Set mnigame owner to interacting NPC
		minigame.owner = interactingNPC;
		
		// Set scene minigame to give minigame
		GameManager.Instance.minigame = minigame;
		
		// Set interaction state to in event
		interactionState = InteractionState.IN_EVENT;
		
	}
	
	/// <summary>
	/// End current minigame.
	/// </summary>
	public void EndMinigame()
	{
		Destroy(GameManager.Instance.minigame.gameObject);
		Resources.UnloadUnusedAssets();
		
		// Clear scene minigame
		GameManager.Instance.minigame = null;
		
		if (interactionState != InteractionState.APPROACH_EVENT && interactionState != InteractionState.APPROACH_NPC)
		{
			// Set interaction state to none
			interactionState = InteractionState.NONE;
		}
		SetCutscene(false);
	}
	#endregion
}
