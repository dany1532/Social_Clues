using UnityEngine;
using System.Collections;

public class rotateTrainWheel : MonoBehaviour {

	float trainWheelZ; 
	public float rotationSpeed = 1.0f; 

	// Use this for initialization
	void Start () {
		//trainWheelZ = this.gameObject.transform.rotation.z;
	}
	
	// Update is called once per frame
	void Update () {
		/*trainWheelZ++;
		this.gameObject.transform.rotation.z = trainWheelZ; 
		if (trainWheelZ >= 360)
		{
			trainWheelZ = 0; 
		}*/

		this.gameObject.transform.Rotate(Time.deltaTime*rotationSpeed, 0, 0, Space.World);

	
	}
}
