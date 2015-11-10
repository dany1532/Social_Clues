using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour {

	//starting position of the object
	Vector3 startPos;
	//Whether the object is the correct solution to the puzzle
	//set in editor
	public bool isSolution;
	//public bool pressed = false; 
	public string onDropResponse; 
	public int wrongTries;
	
	public GameObject managerObj;
	MadMaxManager manager;
	public GameObject dropspot;
	GameObject itemName;
	public Dialogue response;
	public AudioClip tryAgain;
	
	public GameObject marker;
	
	// Use this for initialization
	void Start () {
		//manager = GameObject.Find("MadMaxMinigame").GetComponent<MadMaxManager>();
		//dropspot = GameObject.Find("Slot1");

		manager = managerObj.GetComponent<MadMaxManager>();

		wrongTries = 0;
		
		if (response == null)
			response = GetComponent<Dialogue>();
		
	}
	
	
	public void hasBeenDroppedInContainer()
	{
		foreach(Transform t in transform.parent) {
			if(t.position == dropspot.transform.position) {
				snapBack();
				return;
			}
		}

		this.collider.enabled = false;

		if(isSolution)
		{
			AudioManager.Instance.Play(manager.rightAnswerFX, transform, 1, false);
			this.transform.position = dropspot.transform.position;
			marker.SetActive(true);
			marker.transform.position = this.gameObject.transform.position;
			//Debug.Log (onDropResponse);
			manager.UpdateCorrectSprite();
			//manager.Respond(response);
			//manager.wait("nextLevel");
		}
		else
		{
			//Debug.Log (onDropResponse); 
			AudioManager.Instance.Play(manager.wrongAnswerFX, transform, 1, false);
			//manager.Respond(response);
			this.transform.position = dropspot.transform.position;
			if (response != null && response.nextDialogue != null)
			{
				response.nextDialogue.text = response.text;
			}
			Sherlock.Instance.PlaySequenceInstructions(response, snapBack);

			/*
			if(tryAgain != null)
			{
				Sherlock.Instance.PlaySequenceInstructions(response, snapBack);
				//Invoke("sayTryAgain", manager.minigame.GetCurrentDialogueDuration());
			}
			else
			{
				Sherlock.Instance.PlaySequenceInstructions(response, snapBack);
				//Invoke("snapBack", manager.minigame.GetCurrentDialogueDuration());
			}
			*/
			//StartCoroutine(WaitForInput("snapBack"));
			wrongTries++;

		}
	}
	
	void sayTryAgain()
	{
		//AudioManager.Instance.PlayVoiceOver(tryAgain,1);
		//Invoke("snapBack", tryAgain.length);
	}
	
	void OnPress(bool state)
	{
		if(state)
		{
			this.collider.enabled= false;
			this.GetComponent<UISprite>().depth = 18;
			//pressed = true; 
		}
		else
		{
			this.collider.enabled = true;
			snapBack();
			this.GetComponent<UISprite>().depth = 6;
			//pressed = false; 
		}
	}
	
	public void setStartPos()
	{
		startPos = this.transform.position;
		
	}
	
	public void snapBack()
	{

		this.transform.position = startPos;
		
		if(wrongTries>=3)
		{
			marker.SetActive(true);
			marker.transform.position = this.gameObject.transform.position;
			//show an X over the item
		}
		else
		{
			this.collider.enabled = true;
		}
		dropspot.GetComponent<DropContainer>().hover(false);
	}
	
	void OnDrag()
	{
		if(UICamera.currentTouch.current == dropspot)
		{
			dropspot.GetComponent<DropContainer>().hover(true);
		}
		else
		{
			dropspot.GetComponent<DropContainer>().hover(false);
		}
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

}

