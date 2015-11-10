using UnityEngine;
using System.Collections;

public class Dad : MonoBehaviour {

	// NPC controller associated with the Max
	private NPC npc;
	BedroomLevelManager manager;
	//public GameObject toy;
	
	// Use this for initialization
	void Start () {
		// Get the associated NPC controller
		npc = this.gameObject.GetComponent<NPC>();
		GameObject bedroomManagerGo = GameObject.Find ("BedroomManager").gameObject;
		manager = bedroomManagerGo.GetComponent<BedroomLevelManager>();
		//toy.SetActive(false);
	}
	
	public void OnDialogueComplete() 
	{
		//toy.SetActive (true);
		if(manager != null)
		{
			Vector3 pos = transform.localPosition;
			pos.x -= 9f;
			transform.localPosition = pos;
		}
		//Player.instance.ResetState();
	}
}
