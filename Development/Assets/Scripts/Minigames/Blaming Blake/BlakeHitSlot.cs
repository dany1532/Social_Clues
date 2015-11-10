using UnityEngine;
using System.Collections;

public class BlakeHitSlot : MonoBehaviour {
	public bool hitSlot; 
	public GameObject manager;
	private BlamingBlakeManager BlamingBlakeManagerScript;
	// Use this for initialization
	void Start () {
		BlamingBlakeManagerScript = manager.GetComponent<BlamingBlakeManager>();
		hitSlot = false; 
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Hit Slot: " + hitSlot);
	}
	
		void OnTriggerEnter(Collider other)
	{
		/*if (other.GameObject.tag == button)
		{
			if (totalCount>= 4 && !isCorrect)// && transform.position == target.position)
		{*/
			hitSlot = true; 
			BlamingBlakeManagerScript.Instance.SendMessage("FinalSequence", hitSlot, SendMessageOptions.DontRequireReceiver);
			
		//}
		//}
	}
}
