using UnityEngine;
using System.Collections;

public class ButtonClickHandler : MonoBehaviour
{
	
	//set in editor to YellowBackground
	public GameObject minigame;
	private FollowTheLeader FollowTheLeaderScript;
	
	void Start ()
	{
		FollowTheLeaderScript = minigame.GetComponent<FollowTheLeader> ();
	}
	
	FollowTheLeader.eType GetButtonClicked (string clickButtonStr)
	{
		if (this.name == "MoveRightHandButton") {
			return FollowTheLeader.eType.RIGHT;
		} else if (this.name == "MoveLeftHandButton") {
			return FollowTheLeader.eType.LEFT;
		} else if (this.name == "MoveClapHandButton") {
			return FollowTheLeader.eType.CLAP;
		}
		return FollowTheLeader.eType.INVALID_TYPE; 
	}
	
	void OnPress (bool isDown)
	{
		if (isDown) {
			//Send result to OnButtonClickDown
			FollowTheLeaderScript.Instance.SendMessage ("OnButtonClickDown", GetButtonClicked (this.name), SendMessageOptions.DontRequireReceiver);
			//Debug.Log ("press down");
		} else {
			//Send result to OnButtonClickUp
			FollowTheLeaderScript.Instance.SendMessage ("OnButtonClickUp", GetButtonClicked (this.name), SendMessageOptions.DontRequireReceiver);
		}
	}
}
