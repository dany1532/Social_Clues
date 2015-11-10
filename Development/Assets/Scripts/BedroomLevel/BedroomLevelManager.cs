using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BedroomLevelManager : MonoBehaviour 
{
	
	public enum DialogueType
	{
		SHERLOCK_PROMPT, DAD_REWARD, STORY_END
	}
	
	public Dialogue sherlockPrompt;
	public Dialogue dadReward;
	public Dialogue storyEnd;
	
	
	public GameObject hudObject;
	public UITexture sherlockCagedTexture;
	public UITexture sherlockGlowTexture;
	
	public UITexture sherlockCageBack;
	public UITexture sherlockCageFront;
	public UITexture sherlockCageBranch;
	public UITexture sherlockCagePole;
		
	//public UITexture sherlockUICagePortrait;
	public UISprite sherlockUICagePortrait;
	public Transform npcParent;
	public List<BedroomNPC> listBedroomNPC;
	public GameObject sherlockGameObject;
	public List<GameObject> allNPC;
	GameObject npcSingle;
	GameObject stars;
	Vector3 originalPos;
	bool isCustomLevel = false;
	GameObject dadObject;
	float openCageLerp = 0;
	bool openCageEnabled = false;
	bool obtainedToy = false;
	bool reachedToy = false;
	Vector3 toyPos;
	
	
	
	void Awake()
	{
		//GameObject customLevel = GameObject.Find("Custom_Level");
		
		
		if(ApplicationState.Instance.isStoryMode)
		{
			GameObject go;
            GameObject dadPrefab = ResourceManager.LoadObject("NPCs/Bedroom/Dad_temp");   
			go = GameObject.Instantiate(dadPrefab) as GameObject;
			go.transform.parent = npcParent;
			listBedroomNPC.Add(go.GetComponent<BedroomNPC>());
			
            GameObject mumPrefab = ResourceManager.LoadObject("NPCs/Bedroom/Mum_temp");   
			go = GameObject.Instantiate(mumPrefab) as GameObject;
			go.transform.parent = npcParent;
			listBedroomNPC.Add(go.GetComponent<BedroomNPC>());
			
			if(ApplicationState.Instance.selectedCharacter == "Pete")
			{
                GameObject katePrefab = ResourceManager.LoadObject("NPCs/Bedroom/Kate_temp"); 
				go = GameObject.Instantiate(katePrefab) as GameObject;
				go.transform.parent = npcParent;
				listBedroomNPC.Add(go.GetComponent<BedroomNPC>());
			}
			else
			{
                GameObject petePrefab = ResourceManager.LoadObject("NPCs/Bedroom/Pete_temp"); 
				go = GameObject.Instantiate(petePrefab) as GameObject;
				go.transform.parent = npcParent;
				listBedroomNPC.Add(go.GetComponent<BedroomNPC>());
			}
			sherlockCagedTexture.gameObject.collider.enabled = false;
			isCustomLevel = false;
			
		}
		
		else{
			isCustomLevel = true;
		}
		
		
	}
	
	void Start()
	{
		/*if(ApplicationState.Instance.isStoryMode){
			ApplicationState.Instance.SetCurrent(ApplicationState.LevelNames.BEDROOM);
		}*/
		
		if(!isCustomLevel)
		{
			sherlockGameObject.SetActive(true);
			hudObject = GameObject.FindGameObjectWithTag("HUD");
			Sherlock.Instance.portrait.enabled = false;
			Sherlock.Instance.backgroundTexture.enabled = false;
			hudObject.SetActive(false);
		
			allNPC = new List<GameObject>();
			foreach (Transform child in npcParent) 
			{
				allNPC.Add(child.gameObject);
				child.gameObject.SetActive(false);
			}
		
			foreach(BedroomNPC element in listBedroomNPC)
				element.gameObject.SetActive(true);
		}
		
		else{
			sherlockUICagePortrait.enabled = true;
			sherlockGameObject.SetActive(false);
		}
		
		//StoryEndDialogue(new Vector3(6.5f, 0f, -68f));
		//GameManager.Instance.CompleteLevel();	
			
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
	
	public bool ShowDialogue(DialogueType dialogueType)
	{
		// Display dialogue depending on dialogue type
		switch(dialogueType)
		{	
			case DialogueType.SHERLOCK_PROMPT:
				Sherlock.Instance.PlaySequenceInstructions(sherlockPrompt, null);
				return true;
			
			case DialogueType.DAD_REWARD:
				Sherlock.Instance.PlaySequenceInstructions(dadReward, null);
				return true;
			
			case DialogueType.STORY_END:
				Sherlock.Instance.PlaySequenceInstructions(storyEnd, CompleteLevel);
				return true;


			default:
				return false;
		}
	}
	
	void CompleteLevel()
	{
		//GameManager.Instance.CompleteLevel();	
	}
	
	public void EnableSherlock()
	{
		Debug.Log ("Enable sherlock");
		sherlockGlowTexture.enabled = true;
		sherlockCagedTexture.gameObject.collider.enabled = true;
		ShowDialogue(DialogueType.SHERLOCK_PROMPT);
		
	}
	
	public void EnableSherlockCage(bool enable)
	{
		Debug.Log ("Enable sherlock cage");
		
		sherlockCageBack.enabled = enable;
		sherlockCageFront.enabled = enable;
		sherlockCageBranch.enabled = enable;
		sherlockCagePole.enabled = enable;
		
		sherlockCageBack.transform.parent = DialogueWindow.instance.transform;
		sherlockCageFront.transform.parent = DialogueWindow.instance.transform;
		sherlockCageBranch.transform.parent = DialogueWindow.instance.transform;
		sherlockCagePole.transform.parent = DialogueWindow.instance.transform;
		
		if(enable){
			npcSingle = GameObject.Find("NPCSingle");
			stars = GameObject.Find("Anchor Stars");
			originalPos = npcSingle.transform.position;
			npcSingle.transform.Translate(new Vector3(-0.05f, 0.64f, 0));
			stars.SetActive(false);
		}
		else{
			stars.SetActive(true);
			npcSingle.transform.position = originalPos;
		}
	}
	
	public void OpenSherlockCage()
	{
		//sherlockUICagePortrait.enabled = false;	
		openCageEnabled = true;
		GameObject.Find("Dad(Clone)").GetComponent<NPC>().walkToMinigame = false;
	}
	
	public void DadFinalDialogue()
	{
		if(dadObject == null){
			dadObject = GameObject.Find("Dad(Clone)");	
		}
		
		sherlockCagedTexture.gameObject.SetActive(false);
		
		dadObject.transform.FindChild("Sprites").transform.
			FindChild("Sprite").transform.Rotate(new Vector3(0,180,0));
		
		dadObject.GetComponent<Dad>().OnDialogueComplete();
		dadObject.GetComponent<NPC>().OnDialogueComplete();
		
		//if(hudObject != null)
		//	hudObject.transform.FindChild("Markers").gameObject.SetActive(true);
		
		Invoke("SherlockClickToyDialogue", 15f);
		
	}
	
	public void SherlockClickToyDialogue()
	{
		if(!obtainedToy){
			ShowDialogue(DialogueType.DAD_REWARD);
		}
	}
	
	public void StoryEndDialogue(Vector3 pos)
	{
		//Debug.Log("Story ended");
		obtainedToy = true;
		toyPos = pos;
		toyPos.y = Player.instance.transform.position.y;
		Player.instance.MoveToDestination(toyPos);
		Invoke("HackNavigation", 0.1f);
		Player.instance.SetCutscene(true);
		//ShowDialogue(DialogueType.STORY_END);
	}
	
		void HackNavigation()
	{
		Player.instance.MoveToDestination(toyPos);
	}
	
	void Update()
	{
		if(openCageEnabled)
		{
			openCageLerp += Time.deltaTime/3;
			openCageLerp = Mathf.Clamp(openCageLerp, 0, 1);
			
			sherlockUICagePortrait.fillAmount = Mathf.SmoothStep(1,0,openCageLerp);
			
			if(openCageLerp < 0)
			{
				sherlockUICagePortrait.gameObject.SetActive(false);
				openCageEnabled = false;
			}
		}
		
		if(obtainedToy && !reachedToy && Vector3.Distance(toyPos, Player.instance.transform.position) < 3)
		{
			reachedToy = true;
			
			dadObject.GetComponent<NPC>().toyPosition.gameObject.SetActive(false);
			
			NPCAnimations anims = dadObject.transform.FindChild("Conversation(Clone)").GetComponent<NPCAnimations>();
			
			NPCDialogueAnimation anim = dadObject.transform.FindChild("Sprites").transform.
											FindChild("Sprite").GetComponent<NPCDialogueAnimation>();
			
			NPCAnimations.AnimationSequence currAnimSeq = anims.RetrieveAnimationSequence(NPCAnimations.AnimationIndex.RETURN_END);
			List<Texture> currAnimSeqTextures = currAnimSeq.textures;
			if (currAnimSeqTextures.Count > 0)
			{
				anim.StopAnimation();
				anim.SetAnimationList(currAnimSeqTextures);
				anim.PlayAnimation();
				anim.SetSpeed(currAnimSeq.speed);
			}
			ShowDialogue(DialogueType.STORY_END);
			
			Invoke("QuestCompleted", 10f);
			
			//dadObject.GetComponent<NPC>().
		}
	}
	
	public void QuestCompleted()
	{
		dadObject.GetComponent<NPC>().OnQuestCompleted();
	}
	
	void OnDialogueComplete()
	{
		EnableSherlockCage(false);
		Sherlock.Instance.portrait.enabled = true;
		Sherlock.Instance.backgroundTexture.enabled = true;
		sherlockUICagePortrait.enabled = true;
		sherlockUICagePortrait.transform.parent = Sherlock.Instance.transform.parent;
		Vector3 pos = sherlockUICagePortrait.transform.localPosition;
		pos.z = -54;
		sherlockUICagePortrait.transform.localPosition = pos;
		DialogueWindow.instance.dialogueBubble.background.color = DialogueWindow.instance.dialogueBubble.neutral2Color;
		//sherlockUICagePortrait.transform.
	        
        NPC dadNPC = null;
		foreach(GameObject element in allNPC)
		{
			
			if(element.name == "Dad(Clone)")
			{
                dadNPC = element.GetComponent<NPC>();
				element.SetActive (true);
				element.transform.FindChild("Sprites").
					transform.FindChild("Sprite").GetComponent<BoxCollider>().enabled = true;
				dadObject = element;
			}
		}
		  
        GameObject markersGO = hudObject.transform.FindChild("Markers").gameObject;
        Markers markers = markersGO.GetComponent<Markers>();
        int dadNPCId = NPCs.Instance.GetNpcID(dadNPC);
        markers.ReplaceMarker(dadNPCId, dadNPC.additionalToyObject.spriteName, dadNPC.additionalToyObject.color);
     
        //something about the toy
        for (int index = 0 ; index < markers.markers.Count ; index++)
        {
            if (index != dadNPCId)
                markers.HideMarker(index);
        }
        
		foreach(BedroomNPC element in listBedroomNPC)
		{
			if(element.name.Contains("Dad_temp")){
				element.gameObject.SetActive(false);
			}
		}
		
		//hudObject.transform.FindChild("Markers").gameObject.SetActive(false);
			
		
	}
	
}
