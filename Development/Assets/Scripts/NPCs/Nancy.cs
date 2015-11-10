using UnityEngine;
using System.Collections;

public class Nancy : MonoBehaviour {

	public Reply rightReply;

	// Use this for initialization
	void Start () {
		rightReply.text = "Hi my name is " + ApplicationState.Instance.selectedCharacter + ", are you new here?";
	}
}
