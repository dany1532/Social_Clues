using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewRow : MonoBehaviour {
	public List<GameObject> objectsToUpdate = new List<GameObject>();
	public float amountToUpdate;
	
	public int rowIndex;
	
	// Use this for initialization
	void Start () {
		this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -3);
		for(int i = 0; i < objectsToUpdate.Count; ++i) {
			this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + amountToUpdate, this.transform.localPosition.z);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
