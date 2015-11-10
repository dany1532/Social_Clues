using UnityEngine;
using System.Collections;

public class HighlightFollowPuzzle : MonoBehaviour {
	
	public GameObject correctPuzzlePiece; 
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		this.transform.position = correctPuzzlePiece.transform.position; 
	}
	
		/*void OnDrop(GameObject dropped)
	{
		if(dropped.GetComponent<DraggablePuzzlePiece>() != null)
			dropped.GetComponent<DraggablePuzzlePiece>().isTouchingHighlight(this.gameObject.name);
		else{
			//do nothing
		}
			//dropped.GetComponent<DraggablePuzzlePiece>().hasBeenDroppedInContainer();
		//Debug.Log("hasBeenDropped"); 
	}*/
}
