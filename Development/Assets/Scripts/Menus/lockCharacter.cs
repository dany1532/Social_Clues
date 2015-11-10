using UnityEngine;
using System.Collections;

public class lockCharacter : MonoBehaviour {

	public GameObject target; 
	float x; 
	float y; 
	float z; 

	// Use this for initialization
	void Start () {
		/*x = target.transform.position.x;
		y = target.transform.position.y;
		z = this.transform.position.z; */
	}
	
	// Update is called once per frame
	void Update () {
		//this.transform.position = new Vector3(x, y, z); 
		this.transform.position = target.transform.position; 
	
}
}