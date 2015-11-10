using UnityEngine;
using System.Collections;

public class DraggableObjectDora : MonoBehaviour {

	//starting position of the object
	Vector3 startPos;
	//Whether the object is the correct solution to the puzzle
	//set in editor
	public bool isSolution;
	//public bool pressed = false; 
	public string onDropResponse; 
	public int wrongTries;
	
	public GameObject managerObj;
	DoraManager manager;
	public GameObject dropspot;
	GameObject itemName;

	[System.NonSerialized]
	public bool hasBeenSolved = true;

	public float inactiveSize = .17f;
	//public Dialogue response;
	//public AudioClip tryAgain;
	
	//public GameObject marker;
	
	// Use this for initialization
	void Start () {
		startPos = this.transform.position;
		//manager = GameObject.Find("MadMaxMinigame").GetComponent<MadMaxManager>();
		//dropspot = GameObject.Find("Slot1");
		manager = managerObj.GetComponent<DoraManager>();
		wrongTries = 0;
		
/*		if (response == null)
			response = GetComponent<Dialogue>();
*/		
	}
	
	
	public void hasBeenDroppedInContainer(string slot)
	{
		Debug.Log ("hasBeenDroppedInContainer called");
		//this.collider.enabled = false;
		GameObject container = GameObject.Find(slot);
		 
		if(dropspot == null) {
			this.transform.position = startPos; 
			wrongTries++;
			manager.wrongResponseWrongObject(); 
		}
		else if (container != null && isSolution && container == dropspot)
		{
			hasBeenSolved = true;
			this.transform.position = container.transform.position;
			collider.enabled = false;
			Debug.Log ("we're in the dropspot");
			this.transform.position = dropspot.transform.position;
			manager.numSlotsCorrect(); 
			this.GetComponent<UIStretch>().relativeSize.y = 0.17f;
			manager.getChalkboardItemFromChalkboardThatMatches(gameObject.GetComponent<UISprite>().spriteName).SetActive (true);
			this.gameObject.SetActive(false);
			//marker.SetActive(true);
			//marker.transform.position = this.gameObject.transform.position;
			//Debug.Log (onDropResponse);
			//manager.UpdateCorrectSprite();
			//manager.Respond(response);
		}
		else
		{
			Debug.Log ("not isSolution or null container");
			//Debug.Log (onDropResponse); 
			//manager.Respond(response);
			//this.transform.position = dropspot.transform.position;
			this.transform.position = startPos; 
			/*if(tryAgain != null)
			{
				Invoke("sayTryAgain", manager.minigame.GetCurrentDialogueDuration());
			}
			else
			{
				Invoke("snapBack", manager.minigame.GetCurrentDialogueDuration());
			}
			*/
			//StartCoroutine(WaitForInput("snapBack"));
			wrongTries++;
			manager.wrongResponseRightObject(); 
		}
	}
	
	/*void sayTryAgain()
	{
		AudioManager.Instance.PlayVoiceOver(tryAgain,1);
		Invoke("snapBack", tryAgain.length);
	}
	*/
	void OnPress(bool state)
	{
		if(state)
		{
			this.collider.enabled= false;
			this.GetComponent<UISprite>().depth = 18;
			this.GetComponent<UIStretch>().relativeSize.y = 0.17f;
			//pressed = true; 
		}
		else
		{
			this.collider.enabled = true;
			snapBack();
			this.GetComponent<UISprite>().depth = 6;
			this.GetComponent<UIStretch>().relativeSize.y = inactiveSize;
			//pressed = false; 
		}
	}
	
	/*public void setStartPos()
	{
		startPos = this.transform.position;
		
	}
	*/
	public void snapBack()
	{

		this.transform.position = startPos;
		
		if(wrongTries>=3)
		{
			//marker.SetActive(true);
			//marker.transform.position = this.gameObject.transform.position;
			//show an X over the item
		}
		else
		{
			this.collider.enabled = true;
		}
		if(dropspot != null) {
			dropspot.GetComponent<DropContainerDora>().hover(false);
		}
	}
	
	void OnDrag()
	{
		if(dropspot == null) {
			return;
		}
		else if(UICamera.currentTouch.current == dropspot)
		{
			dropspot.GetComponent<DropContainerDora>().hover(true);
		}
		else
		{
			dropspot.GetComponent<DropContainerDora>().hover(false);
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

