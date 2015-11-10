using UnityEngine;
using System.Collections;

//The Initial interaction between Player and NPC: Talk or ignore
public class PlayerInteractionBubble : MonoBehaviour {
	public Camera myCamera;
	public GameObject talkButton;
	public GameObject ignoreButton;
	public NPC interactingNPC;
	public Vector3 playerLoc;
	Vector3 npcLoc;
	public float distance;
	public Dialogue accept;
	public Dialogue decline;
	bool wasNearNPC = false;
	
	// Use this for initialization
	void Start () {
		DisplayInteractionBubble(false);
		myCamera = GameObject.Find("GUICamera").GetComponent<Camera>();
	}
	
	//Who is the interacting npc
	public void SetNPC(NPC theNPC){
		interactingNPC = theNPC;	
		npcLoc = interactingNPC.sprite.transform.position;
	}
	
	void CalculateInteractionBubblePosition(){
		playerLoc = Player.instance.GetPlayerScreenPosition();
		Vector3 newWorldPos =  myCamera.ScreenToWorldPoint(playerLoc);
		newWorldPos.z = transform.position.z;
		newWorldPos.y += 0.05f;
		transform.position = newWorldPos;
	}
	
	//Displays the interaction options
	void DisplayInteractionBubble(bool display){
		if(display){
			CalculateInteractionBubblePosition();
			setButtonActive(talkButton, true);
			setButtonActive(ignoreButton, true);
		}
		
		else{
			setButtonActive(talkButton, false);
			setButtonActive(ignoreButton, false);
		}
	}

	void setButtonActive(GameObject button, bool active) {
		foreach(Transform t in button.transform) {
			t.gameObject.SetActive(active);
		}
		button.collider.enabled = active;
	}
	
	//Start interaction with NPC
	public void InteractWithNPC(){
        Player.instance.talkToNPC = true;
        DisplayInteractionBubble(false);
        Sherlock.Instance.PlaySequenceInstructions(accept, null);
		interactingNPC.StartInteraction();
        interactingNPC = null;
	}
	
	//Ignore NPC
	public void IgnoreNPC(){
		Player.instance.talkToNPC = false;
		interactingNPC = null;
		DisplayInteractionBubble(false);
		Invoke("PlayerCanMove", 0.25f);
		Sherlock.Instance.PlaySequenceInstructions(decline, null);
		InputManager.Instance.ReceivedUIInput();
		Player.instance.SetFinishedInteraction();
		Player.instance.SetCutscene(true);
	}
	
	public void PlayerCanMove(){
		Player.instance.SetCutscene(false);
		Player.instance.ResetState();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(interactingNPC != null){
			//if(Player.instance.PlayerDistance(interactingNPC.gameObject.transform.position) < 3f){
			if(Player.instance.GetDistance(npcLoc) < Player.instance.npcInteractionDistance){
				if (wasNearNPC == false)
				{
					Player.instance.StopPlayerNavigation();
					DisplayInteractionBubble(true);
					interactingNPC.PlayIntro();
					wasNearNPC = true;
				}
			}
			else
				wasNearNPC = false;
		}
		else
			wasNearNPC = false;
	}
}
