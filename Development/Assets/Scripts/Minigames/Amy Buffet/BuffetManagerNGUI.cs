using UnityEngine;
using System.Collections;

public class BuffetManagerNGUI : MonoBehaviour {

	int phase;
	/* Phase 0 : Instructions
	 * Phase 1 : Levels
	 * Phase 2 : End
	 * * * * * * * * * * * * */
	public int currLevel; 
	public bool incrementLevel;
	
	bool cont;
	
	public int numLevels;

	GameObject[] myLevels;
	GameObject largeSample;
	GameObject smallSample;
	GameObject smallSampleBubble;
	GameObject myTray;
	GameObject endGame;
	

	// Use this for initialization
	void Start () {		
		phase = 0;
		Debug.Log("Phase: " + phase);
		cont = true;
		
		largeSample = GameObject.Find("SampleTrayLarge");
		smallSample = GameObject.Find("SampleTraySmall");
		smallSampleBubble = GameObject.Find("SampleTrayBubble");
		myTray = GameObject.Find ("PlayerTray");
		endGame = GameObject.Find ("EndBackground");
		
		myLevels = new GameObject[numLevels];
		//find first level, or BuffetLevel0
		myLevels[0] = GameObject.Find("BuffetLevel0");
	
		//fill up myLevels with remaining
		int i = 1;
		while(GameObject.Find("BuffetLevel" + i.ToString()) !=null)
		{
			myLevels[i] = GameObject.Find("BuffetLevel" + i.ToString());
			i++;
		}
		
		
		
		Debug.Log("S: This is Amy's original order. She had a garden salad, burger and strawberries. We must remake Amy's lunch exactly as it appears, so choose the RIGHT item, and place it in the RIGHT location.");
		StartCoroutine(waitForInput("startGame"));

	}
	
	void Update(){
		
		if(Input.GetButtonDown("Fire1"))
			cont = true;
		
		

	}
	
	public void wait(string method)
	{
		//Debug.Log(method);
		StartCoroutine(waitForInput(method));
	}
	
	/* Method waitForInput
	 * 
	 * Waits until the user clicks the mouse, then calls the function method (passed in)
	 *
	 * */
	public IEnumerator waitForInput(string method)
	{
		
		cont = false;
		while(!cont)
		{
			yield return 0;
		}
		Invoke (method, 0);
	}
	
	
	/* Method startGame
	 * 
	 * Moves from Phase 0 to Phase 1. Starts the game.
	 * 
	 * */
	void startGame()
	{
		
		phase = 1;
		currLevel = 0;
		
		smallSampleBubble.GetComponent<UISprite>().fillAmount = 1;
		foreach(Transform child in smallSample.transform)
		{
			child.gameObject.GetComponent<UISprite>().fillAmount = 1;
		}
		foreach(Transform child in largeSample.transform)
		{
			child.gameObject.GetComponent<UISprite>().fillAmount = 0;
		}
		foreach(Transform child in myTray.transform)
		{
			child.gameObject.GetComponent<UISprite>().fillAmount = 1;
		}
		
		
		
		myLevels[currLevel].transform.position = new Vector3(0, myLevels[currLevel].transform.position.y, myLevels[currLevel].transform.position.z);
		
		foreach(Transform child in myLevels[currLevel].transform)
		{
			if(child.GetComponent<DraggableObjectBuffet>() != null)
				child.GetComponent<DraggableObjectBuffet>().setStartPos();
		}

			
	}
	
	void nextLevel()
	{
		myLevels[currLevel].transform.position = new Vector3(404.0f, myLevels[currLevel].transform.position.y, myLevels[currLevel].transform.position.z);
		
		currLevel++;
		
		if(currLevel < numLevels)
		{
			myLevels[currLevel].transform.position = new Vector3(0, myLevels[currLevel].transform.position.y, myLevels[currLevel].transform.position.z);
			
			foreach(Transform child in myLevels[currLevel].transform)
			{
				if(child.GetComponent<DraggableObjectBuffet>() != null)
					child.GetComponent<DraggableObjectBuffet>().setStartPos();
			}
			
		}
		else
		{
			phase = 2;
			endGame.transform.position = new Vector3(endGame.transform.position.x, endGame.transform.position.y, endGame.transform.position.z-0.2f);
			
			smallSampleBubble.GetComponent<UISprite>().fillAmount = 0;
			foreach(Transform child in smallSample.transform)
			{
				child.gameObject.GetComponent<UISprite>().fillAmount = 0;
			}
			foreach(Transform child in largeSample.transform)
			{
				child.gameObject.GetComponent<UISprite>().fillAmount = 1;
			}
			foreach(Transform child in myTray.transform)
			{
				child.gameObject.GetComponent<UISprite>().fillAmount = 0;
			}
			
			Debug.Log("S: Yay! You did it. This order is a perfect match. Now let's bring it back to Amy.");
			//Return to Cafeteria
		}
		
	}
	
	

	
}
