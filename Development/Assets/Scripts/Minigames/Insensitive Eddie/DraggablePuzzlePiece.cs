using UnityEngine;
using System.Collections;

/// <summary>
/// Draggable plate on the buffet
/// </summary>

public class DraggablePuzzlePiece : MonoBehaviour
{
	UITexture uiTexture;
	//starting position of the object
	Vector3 startPos;
	Vector3 slotStartPos;
	//Whether the object is the correct solution to the puzzle
	//In editor, set to true if this is a part of the correct final tray
	public bool isSolution;
	public bool pressed;
	public GameObject mySlot;
	EddiePuzzleManager manager;
	bool onHint = false;
	Camera sceneCamera;

	
	// Use this for initialization
	void Start ()
	{
		manager = GameObject.Find ("EddieMinigame").GetComponent<EddiePuzzleManager> ();
		sceneCamera = GameObject.Find ("Camera").camera;
		
		//startPos = this.transform.position;
		setStartPos (); 
		uiTexture = GetComponent<UITexture> ();
	}
	   
	public void LateUpdate ()
	{
		if (Input.GetMouseButtonDown (0) && !manager.hintAsked)
		{
			if (onHint)
			{
				uiTexture.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);//change this sprite's color to white
				mySlot.GetComponent<UITexture> ().enabled = false;//hide the correct slot
			}
			
			onHint = false;
		}
	}
	
	public void DestroySelf()
	{
		Destroy(gameObject);	
	}
	
	public void wrongFX()
	{
		AudioManager.Instance.Play(manager.wrongAnswerFX, transform, 1, false);
	}
	
	public void hasBeenDroppedInContainer (string slot)
	{
		if (!manager.ValidDrop ()) return; 
		uiTexture.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);//change this sprite's color to white
		mySlot.GetComponent<UITexture> ().enabled = false;//hide the correct slot
		onHint = false;
		
		
		if (isSolution) 
		{
			if (slot == mySlot.name) 
			{
				
				AudioManager.Instance.StopSoundFX();
				AudioManager.Instance.Play(manager.rightAnswerFX, transform, 1, false);
				if (manager.ShowDialogue (EddiePuzzleManager.DialogueType.CORRECT) == false) {
					Debug.Log ("S: That's right! Let's try another piece.");
				}
				
				//mySlot.GetComponent<DropContainerPuzzle>().myUITexture.mainTexture = uiTexture.mainTexture;
				
				//Changed this because puzzle pieces are using shaders instead of modyfing textures manually
				UITexture slotUITexture = mySlot.GetComponent<UITexture>();
				slotUITexture.material = uiTexture.material;
				slotUITexture.uvRect = uiTexture.uvRect;
				mySlot.GetComponent<UITexture> ().enabled = true;

				manager.IncreaseCorrectCount();
				manager.wrongTries = 0;
				
				uiTexture.enabled = false;
				Invoke("DestroySelf", 2f);
				//gameObject.SetActive(false);
				//Destroy(gameObject);
				//manager.wait("nextLevel");
			} 
			else 
			{
				AudioManager.Instance.Play(manager.wrongAnswerFX, transform, 1, false);
				
				if (manager.wrongTries < 2) {
					this.transform.position = startPos;
					//if (manager.ShowDialogue (EddiePuzzleManager.DialogueType.MISTAKE) == false) {
					//	Debug.Log ("S: That piece doesn't go there. Let's try a different location.");
					//}
					manager.wrongTries++;
				} else {
					this.transform.position = startPos;
					if (manager.ShowDialogue (EddiePuzzleManager.DialogueType.WANTHINT) == false) {
						Debug.Log ("S: Want a hint? Tap the hint button at the top of the screen.");
					}
					manager.wrongTries = 0;
				}
				
			}
		}
	}
	
	public void OnPress (bool state)
	{
		if (!state) 
		{
			AudioManager.Instance.Play(manager.wrongAnswerFX, transform, 1, false);
			this.transform.position = startPos;
			mySlot.transform.position = slotStartPos;
		} 
		else 
		{
			Vector3 tempPos = Vector3.zero;
			tempPos.x = sceneCamera.ScreenToWorldPoint(Input.mousePosition).x;
			tempPos.y = sceneCamera.ScreenToWorldPoint(Input.mousePosition).y;
			uiTexture.transform.position = tempPos;
			tempPos = this.transform.localPosition;
			tempPos.z = -1.75f ; //depth of puzzle piece when dragged
			uiTexture.transform.localPosition = tempPos;
			//bring up the slot to the front to avoid odd collision where slots overlap
			tempPos = mySlot.transform.localPosition;
			tempPos.z = -1.5f;  //depth of slot when piece dragged
			mySlot.transform.localPosition = tempPos;
		}
			
		if (state)
		{
			if (manager.hintAsked || onHint) {
				uiTexture.color = new Color (1.0f, 1.0f, 0, 1.0f);//change this sprite's color to yellow
				mySlot.GetComponent<UITexture> ().enabled = true;
				onHint = !onHint;
			}
		} else if (!state && manager.hintAsked) {
			manager.hintAsked = false;
		}

	}
	
	public void setStartPos ()
	{
		startPos = this.transform.position;
		slotStartPos = mySlot.transform.position;
	}
	
	
	

}
