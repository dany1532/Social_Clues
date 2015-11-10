using UnityEngine;
using System.Collections;
using System;

public class Position : MonoBehaviour {
	
	public GameObject originalPete;
	public UIWidget target;
	public Texture TargetFilledTexture;
	public Texture TargetOutlineTexture;
		//public Texture emptyTarget;
	//public Texture filledTarget;

	public NPCAnimations.AnimationIndex NPCAnimationMinigame;
	public NPCAnimations.AnimationIndex NPCAnimationEmotion;

	public GameObject NPCTexture;
	
	float counterFrequency = 0.1f;
	float counter = 0;
	public float holdDuration = 2f;
	
	public Dialogue ending;
	
	public PreDialogueMinigame minigame;
	public float minDistance = -0.045f;
	public float maxDistance = 0.045f;
	public Transform minValidPosition;
	public Transform maxValidPosition;
	public Transform leftEdge;
	public Transform rightEdge;

    public AudioClip successSFX;
    
	// Use this for initialization
	void OnEnable () {
		originalPete.SetActive(false);
		gameObject.GetComponentInChildren<UIDragObject>().enabled = true;

		if (Player.instance != null && Player.instance.interactingNPC != null && Player.instance.interactingNPC.preDialogueMinigameSetup != null)
			DialogueWindow.instance.UpdateNPCPortrait (Player.instance.interactingNPC.preDialogueMinigameSetup);
		
		if (UnityEngine.Random.value < 0.5f)
			this.transform.position = new Vector3(UnityEngine.Random.Range(maxValidPosition.position.x, rightEdge.position.x), this.transform.position.y, this.transform.position.z);
		else
			this.transform.position = new Vector3(UnityEngine.Random.Range(leftEdge.position.x, minValidPosition.position.x), this.transform.position.y, this.transform.position.z);
			
		target.mainTexture = TargetOutlineTexture;
        Texture playerTexture = originalPete.GetComponent<UITexture>().mainTexture;
        GetComponent<UITexture>().mainTexture = playerTexture;
        GetComponent<UIStretch>().initialSize = new Vector2(playerTexture.width, playerTexture.height);
	}
	
	void OnDrag(Vector2 delta) {
		if(this.transform.position.x < leftEdge.position.x) {
			this.transform.position = new Vector3(leftEdge.position.x, this.transform.position.y, this.transform.position.z);
		} else if(this.transform.position.x > rightEdge.position.x) {
			this.transform.position = new Vector3(rightEdge.position.x, this.transform.position.y, this.transform.position.z);
		} 
		if(this.transform.position.x > minValidPosition.position.x && this.transform.position.x < maxValidPosition.position.x) {
			if (counter < 0)
			{
				counter = 0;
				InvokeRepeating("CountWithinArea", 0, counterFrequency);
				target.mainTexture = TargetFilledTexture;
				target.color = Color.green;
			}
		} else {
			counter = -1;
			CancelInvoke("CountWithinArea");
			target.mainTexture = TargetOutlineTexture;
			target.color = Color.white;
		}
	}

	
	void OnPress (bool pressed)
	{
		if (!pressed)
		{
			if(this.transform.position.x < leftEdge.position.x) {
				this.transform.position = new Vector3(leftEdge.position.x, this.transform.position.y, this.transform.position.z);
			} else if(this.transform.position.x > rightEdge.position.x) {
				this.transform.position = new Vector3(rightEdge.position.x, this.transform.position.y, this.transform.position.z);
			} 
		}
	}

	void CountWithinArea()
	{
		if (counter >= holdDuration)
		{
			gameObject.GetComponentInChildren<UIDragObject>().enabled = false;
			counter = -1;
			CancelInvoke("CountWithinArea");
			AudioManager.Instance.Play(successSFX, this.transform, 1.0f, false);
			Sherlock.Instance.PlaySequenceInstructions(ending, EndMinigame);
			
			if (Player.instance != null && Player.instance.interactingNPC != null)
				DialogueWindow.instance.UpdateNPCPortrait(Player.instance.interactingNPC.conversationTree.currentNode);
		}
		else
		{
			counter += counterFrequency;
		}
	}

	void OnDestroy()
	{
		CancelInvoke("CountWithinArea");
	}
	// End minigame and proceed to dialogue
	void EndMinigame()
	{
		originalPete.SetActive(true);
		// Trigger event		
		minigame.returnEvent.TriggerEvent(true);
	}	
}
