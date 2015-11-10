using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropContainerLock : MonoBehaviour {	
	public Texture[] lockSymbols;

	public Color[] lockForColors; //the colors of the symbols on the lock
	public Color[] lockBackColors; //the colors of the lock itself; only implemented in med. and hard mode

	public GameObject[] myKeys = new GameObject[7];

	public GameObject otherLock1, otherLock2; 	//these are used to ensure that when the last lock called sets the 
												//remaining locks, it doesn't make a duplicate key of a correct key
	
	public string mySlotNum; //the number of this lock. Part of the gameobject's name.

	public GameObject myTurnAssets, myTurnKeyHead, myTurnIcon, myTurnCircle, myPadLock, myKeyhole;

	public GameObject[] mySymbols;

	public LockManager manager;

	public int bgcol;

	[System.NonSerialized]
	public int[] symbols;
	public int[] colors;

	bool isUnique = false;
	
	void OnDrop(GameObject dropped)
	{
		DraggableObjectKey draggableObject = dropped.GetComponent<DraggableObjectKey>();
		if(draggableObject != null) {
			draggableObject.hasBeenDroppedInContainer(this.gameObject.name);
		} else {
			if(dropped.GetComponent<DraggableObjectKey>())
				dropped.GetComponent<DraggableObjectKey>().hasBeenDroppedInContainer(mySlotNum);
		}

		myPadLock.GetComponent<UITexture>().color = lockBackColors[bgcol];
	}

	public void disableKeys()
	{
		foreach (GameObject key in myKeys)
		{
			if(key)
				key.GetComponent<BoxCollider>().enabled = false;
		}
	}

	public IEnumerator enableKeys()
	{
		foreach (GameObject key in myKeys)
		{
			if(key) {
				key.GetComponent<BoxCollider>().enabled = true;
			}
		}
		yield return new WaitForSeconds(manager.minigame.GetCurrentDialogueDuration());
	}

	public void keyInsert()
	{
		disableKeys();
		myTurnAssets.SetActive(true);
		foreach(Transform t in myTurnAssets.transform) {
			if(t.gameObject.name.Contains("Key") || t.gameObject.name.Contains("Head"))
				t.gameObject.SetActive(true);
		}
		myTurnKeyHead.GetComponent<UISprite>().color = lockBackColors[bgcol];
		myKeyhole.SetActive(false);
	}

	public void keyStartTurn() {
		foreach(Transform t in myTurnAssets.transform) {
			t.gameObject.SetActive(true);
		}
	}

	public IEnumerator keyTurned()
	{
		myTurnAssets.SetActive(false);
		myTurnCircle.SetActive(false);
		myTurnKeyHead.SetActive(false);
		foreach(GameObject g in mySymbols) {
			g.GetComponent<UITexture>().enabled = false;
		}
		//move padlock down
		TweenPosition.Begin(myPadLock, .5f, myPadLock.transform.localPosition - new Vector3(0, 35.0f, 0));
		yield return new WaitForSeconds(2f);
		manager.keyTurned(gameObject);
	}

	public IEnumerator keyTurnedT()
		//this is called to unlock the padlock in the tutorial without advancing the game
	{
		myTurnAssets.SetActive(false);
		myTurnCircle.SetActive(false);
		myTurnKeyHead.SetActive(false);
		foreach(GameObject g in mySymbols) {
			g.GetComponent<UITexture>().enabled = false;
		}
		//move padlock down
		TweenPosition.Begin(myPadLock, .5f, myPadLock.transform.localPosition - new Vector3(0, 35.0f, 0));

		yield return new WaitForSeconds(0);
		//yield return new WaitForSeconds(2f);
		//manager.keyTurned(gameObject);
	}
	
	public void hover(bool isOver)
	{
		if (isOver)
		{
			//highlight.SetActive(true);
			myPadLock.GetComponent<UITexture>().color = lockBackColors[bgcol] + new Color(0.3f, 0.3f, 0.3f, 1.0f);
		}
		else if (!isOver)
		{
			//highlight.SetActive(false);
			//Debug.Log("Stopped Hovering " + gameObject);
			myPadLock.GetComponent<UITexture>().color = lockBackColors[bgcol];
		}
	}

	public void setLock(List<int> symbolsList) {
		symbols = new int[symbolsList.Count];
		colors = new int[symbolsList.Count];
		for(int i = 0; i < symbolsList.Count; i++) {
			GameObject g = mySymbols[i];
			symbols[i] = symbolsList[i];
			g.GetComponent<UITexture>().mainTexture = lockSymbols[symbols[i]];
			colors[i] = Random.Range(0, lockForColors.Length - 1);
			g.GetComponent<UITexture>().color=lockForColors[colors[i]];
		}
		
		if(manager.minigame.difficulty != MinigameDifficulty.Difficulty.EASY) {
			bgcol = Random.Range(0, lockBackColors.Length - 1);
			myPadLock.GetComponent<UITexture>().color = lockBackColors[bgcol];
		} else {
			myPadLock.GetComponent<UITexture>().color = new Color(0.79f, 0.79f, 0.79f, 1.0f);
		}
	}
}
