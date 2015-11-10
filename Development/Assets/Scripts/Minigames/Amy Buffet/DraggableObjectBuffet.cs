using UnityEngine;
using System.Collections;

/// <summary>
/// Draggable plate on the buffet
/// </summary>
public class DraggableObjectBuffet : MonoBehaviour {

	//starting position of the object
	Vector3 startPos;
	//Whether the object is the correct solution to the puzzle
	//In editor, set to true if this is a part of the correct final tray
	public bool isSolution;
	// Which is the slot of this plate
	//In editor, set this to the # of the correct tray slot (slots are numbered left to right, top row starting with 1)
	public string correctSlot; 
	public GameObject[] dropspots;
	BuffetManager manager;
	UISprite sprite;
	bool hasBeenDropped = false;
    
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("BuffetMinigame").GetComponent<BuffetManager>();
		sprite = GetComponent<UISprite>();
	}	
	
	/// <summary>
	/// Handles event where object is dropped in a container
	/// </summary>
	/// <param name='slot'>
	/// Slot that the object was dropped in
	/// </param>
	public void hasBeenDroppedInContainer(string slot)
	{			
		GameObject container = GameObject.Find(slot);

		manager.DisableFoodColliders();

		if (container != null)
		{
			this.transform.position = container.transform.position;
			collider.enabled = false;
		}
		
		if(isSolution)
		{
			// Item dropped in the correct slot
			if(slot.Contains(correctSlot))
			{	
				if (container != null)
				{
					this.transform.parent = container.transform.parent;
             
                    
                     foreach(Transform t in this.transform) {
                         t.gameObject.GetComponent<UISprite>().depth -= 5;
                         t.localPosition += new Vector3(0, 0, 2);
                     }
                                
					//foreach(Transform t in this.transform)
					//	t.gameObject.GetComponent<UIStretch>().relativeSize.Set(0.2f,0.2f);
					container.GetComponent<BoxCollider>().enabled = false;
				}
				AudioManager.Instance.Play(manager.rightAnswerFX, transform, 1, false);
				manager.IncrementLevel();
			}
			else // Item dropped in the wrong slot
			{
				AudioManager.Instance.Play(manager.wrongAnswerFX, transform, 1, false);
				// Display / play wrong position dialogue
				if (manager.ShowDialogue(BuffetManager.DialogueType.WRONG_SPOT) == false)
				{
					Debug.Log("S: Amy did want that item. But it should go in a different location.");
				}
				
				Invoke("snapBack", manager.GetAudioDuration(BuffetManager.DialogueType.WRONG_SPOT));
			}
		}
		else
		{			
			AudioManager.Instance.Play(manager.wrongAnswerFX, transform, 1, false);
			// Display / play not on tray dialogue
			if (manager.ShowDialogue(BuffetManager.DialogueType.NOT_ON_TRAY) == false)
			{
				Debug.Log("S: Is that what she had on her plate? Try another.");
			}
				
			Invoke("snapBack", manager.GetAudioDuration(BuffetManager.DialogueType.NOT_ON_TRAY));
		}
	}
	
	/// <summary>
	/// Handle press event
	/// </summary>
	/// <param name='pressed'>
	/// Whether the object was pressed or not
	/// </param>
	void OnPress(bool pressed)
	{
		if (pressed)
		{
			foreach(Transform t in this.transform) {
				t.gameObject.GetComponent<UISprite>().depth += 10;
				t.localPosition -= new Vector3(0, 0, 5);
			}
			hasBeenDropped = false;
		}
		else
		{
			snapBack();
		}
		this.collider.enabled= !pressed;
	}
	
	/// <summary>
	/// Set the start position of the object on the buffet
	/// </summary>
	public void setStartPos()
	{
		startPos = this.transform.position;
	}
	
	public void snapBack()
	{
        this.transform.position = startPos;
		manager.EnableFoodColliders();

        if (!hasBeenDropped)
        {
            hasBeenDropped = true;
    		foreach(Transform t in this.transform) {
    			t.gameObject.GetComponent<UISprite>().depth -= 10;
    			t.localPosition += new Vector3(0, 0, 5);
    		}
        }
		
		foreach (GameObject ds in dropspots)
		{
			ds.GetComponent<DropContainer>().hover(false);
		}
		
		this.collider.enabled= true;
	}
	
	void OnDrag()
	{
		if(UICamera.currentTouch.current != null && UICamera.currentTouch.current.tag == "Dropspot")
		{
			UICamera.currentTouch.current.GetComponent<DropContainer>().hover(true);
		}
		else
		{
			foreach (GameObject ds in dropspots)
			{
				ds.GetComponent<DropContainer>().hover(false);
			}
		}
	}
}
