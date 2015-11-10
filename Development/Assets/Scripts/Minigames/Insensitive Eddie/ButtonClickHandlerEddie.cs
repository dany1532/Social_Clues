using UnityEngine;
using System.Collections;

public class ButtonClickHandlerEddie : MonoBehaviour {
	
	//set in editor to YellowBackground
	public GameObject manager;
	public EddiePuzzleManager.eType myType;
	private EddiePuzzleManager EddiePuzzleManagerScript;
	Vector3 startPos;
//	Vector3 offset = new Vector3(-100,0,0); 
	//bool moved = false;
	
	
	void Start(){
		EddiePuzzleManagerScript = manager.GetComponent<EddiePuzzleManager>();
		
	}
	
	void OnPress(bool isDown)
	{
		if(isDown)
		{
			EddiePuzzleManagerScript.OnButtonClickDown(myType);
		}
	}
	

}
	
