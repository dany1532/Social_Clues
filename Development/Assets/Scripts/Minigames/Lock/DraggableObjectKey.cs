using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Draggable key in key and lock minigame; works similar to buffet items
/// </summary>
public class DraggableObjectKey : MonoBehaviour {
	
	//starting position of the object
	Vector3 startPos;
	//Whether the object is the correct solution to the puzzle
	//In editor, set to true if this is a part of the correct final lock
	public bool isSolution;
	// Which is the lock for this key
	//In editor, set this to the # of the correct lock (locks are numbered left to right, starting with 1)
	public string correctSlot; 
	public GameObject[] dropspots;

	public GameObject[] mySymbols;

	public GameObject myX, myKey;
	public string[] keySymbols;


	bool disabled = false;

	LockManager manager;
	UISprite sprite;
	
	// Use this for initialization
	void Awake () {
		manager = GameObject.Find("Minigame").GetComponent<LockManager>();
		sprite = GetComponent<UISprite>();
		myX.SetActive(false);
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
		
		if (container != null)
		{
			this.transform.position = container.GetComponent<DropContainerLock>().myPadLock.transform.position + new Vector3(0f, 0f, -1f);
			collider.enabled = false;
			container.GetComponent<DropContainerLock>().disableKeys();
		}
		
		if(isSolution)
		{
			// Item dropped in the correct slot
			if(slot.Contains(correctSlot))
			{	
				manager.correctKey(dropspots[System.Convert.ToInt32(correctSlot)]);
				this.gameObject.SetActive(false);

			}
			else // Item dropped in the wrong slot
			{
				container.GetComponent<DropContainerLock>().disableKeys();
				// Display / play wrong position dialogue
				manager.incorrectKey();
				if(gameObject.GetComponent<BalkiShake>())
					gameObject.GetComponent<BalkiShake>().Shake();
				Invoke("snapBack", manager.minigame.GetCurrentDialogueDuration());
			
			}
		}
		else
		{	
			container.GetComponent<DropContainerLock>().enableKeys();
			// Display / play not on tray dialogue
			manager.incorrectKey();
			if(gameObject.GetComponent<BalkiShake>())
				gameObject.GetComponent<BalkiShake>().Shake();
			disabled = true;
			this.collider.enabled = false;
			Invoke("snapBack", manager.minigame.GetCurrentDialogueDuration());

		}

		if (container != null && !isSolution)
		{
			container.GetComponent<DropContainerLock>().StartCoroutine("enableKeys");
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
			//sprite.depth = 10;
			foreach(GameObject g in mySymbols)
				g.GetComponent<UISprite>().depth = 11;
			myKey.GetComponent<UISprite>().depth = 10;
			myX.GetComponent<UISprite>().depth = 12;
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
		foreach(GameObject g in mySymbols)
			g.GetComponent<UISprite>().depth = 3;
		myKey.GetComponent<UISprite>().depth = 2;
		myX.GetComponent<UISprite>().depth = 4;

		manager.enableAllKeys();

		foreach (GameObject ds in dropspots)
		{
			if(ds != null) {
				ds.GetComponent<DropContainerLock>().hover(false);
			}
		}

		if(isSolution || disabled == false)
			this.collider.enabled= true;
		else 
		{
			this.collider.enabled = false;
			myKey.GetComponent<UISprite>().color = new Color(.6f, .6f, .6f, 1.0f);
			if(!myX.activeSelf) {
				foreach(GameObject mySymbol in mySymbols) {
					mySymbol.GetComponent<UISprite>().color = new Color(mySymbol.GetComponent<UISprite>().color.r - .4f, 
					                                                    mySymbol.GetComponent<UISprite>().color.g - .4f, 
					                                                    mySymbol.GetComponent<UISprite>().color.b - .4f, 1.0f);
				}
			}
			//turn on X
			myX.SetActive(true);
		}



	}
	
	void OnDrag()
	{
		foreach (GameObject ds in dropspots)
		{
			if(ds != null) {
				ds.GetComponent<DropContainerLock>().hover(false);
			}
		}
		if(UICamera.currentTouch.current != null)
		{
			if(UICamera.currentTouch.current.GetComponent<DropContainerLock>()) {
				UICamera.currentTouch.current.GetComponent<DropContainerLock>().hover(true);
			}
		}
	}

	public void setSolutionKey(DropContainerLock solutionLock) {
		isSolution = true;
		correctSlot = solutionLock.mySlotNum;
		for(int i = 0; i < mySymbols.Length; i++) {
			GameObject g = mySymbols[i];
			g.GetComponent<UISprite>().spriteName = keySymbols[solutionLock.symbols[i]];
			g.GetComponent<UISprite>().depth = 3;
			g.GetComponent<UISprite>().color = solutionLock.lockForColors[solutionLock.colors[i]];
		}
		if(manager.minigame.difficulty != MinigameDifficulty.Difficulty.EASY) {
			myKey.GetComponent<UISprite>().color = solutionLock.lockBackColors[solutionLock.bgcol];
		}else {
			myKey.GetComponent<UISprite>().color = new Color(.868f, .868f, .868f, 1.0f);
		}
		disabled = false;
	}

	public void setKey(List<int> symbols, DropContainerLock referenceLock) {
		bool keyExists = false;
		do {
			keyExists = false;
			isSolution = false;
			for(int i = 0; i < mySymbols.Length; i++) {
				GameObject g = mySymbols[i];
				g.GetComponent<UISprite>().spriteName = keySymbols[symbols[i]];
				g.GetComponent<UISprite>().depth = 3;
				g.GetComponent<UISprite>().color = referenceLock.lockForColors[Random.Range(0, referenceLock.lockForColors.Length - 1)];
			}
			if(manager.minigame.difficulty != MinigameDifficulty.Difficulty.EASY) {
				myKey.GetComponent<UISprite>().color = referenceLock.lockBackColors[Random.Range(0, referenceLock.lockBackColors.Length - 1)];
			} else {
				myKey.GetComponent<UISprite>().color = new Color(.868f, .868f, .868f, 1.0f);
			}
			disabled = false;
			foreach(GameObject g in manager.getAllKeysOfLevel()) {
				if(g.activeSelf) {
					if(isKeySame(g.GetComponent<DraggableObjectKey>())) {
						keyExists = true;
						break;
					}
				}
			}
		} while(keyExists);
	}

	public bool isKeySame(DraggableObjectKey key) {
		int counter = 0;
		foreach(GameObject thisKeySymbol in mySymbols) {
			if(key.gameObject == this.gameObject) {
				continue;
			}
			foreach(GameObject otherKeySymbol in key.mySymbols) {
				if(thisKeySymbol.GetComponent<UISprite>().spriteName == otherKeySymbol.GetComponent<UISprite>().spriteName &&
				   thisKeySymbol.GetComponent<UISprite>().color == otherKeySymbol.GetComponent<UISprite>().color) {
					counter++;
					break;
				}
			}
		}
		return counter >= mySymbols.Length;
	}
}
