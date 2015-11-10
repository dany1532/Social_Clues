using UnityEngine;
using System.Collections;

public class squashStretchTrain : MonoBehaviour {

	Vector3 startScale; 
	Vector3 currScale;
	public Vector3 endScale; 
	public float lerpSpeed, maxY, offset;
	float lerp;
	float offsetLerp; 
	bool goingUp; 

	// Use this for initialization
	void Start () {
		startScale = this.gameObject.transform.localScale; 
		currScale = startScale; 
		goingUp = false;
	}
	
	// Update is called once per frame
	void Update () {
		lerp += Time.deltaTime * lerpSpeed;
		offsetLerp = (Time.time*lerpSpeed) + offset; 
		//Debug.Log ("goingUp: " + goingUp); 
		currScale = new Vector3(startScale.x, (Mathf.PingPong(offsetLerp, maxY) + 3.0f), startScale.z); 
		/*if (!goingUp)
		{
			currScale = Vector3.Lerp (startScale, endScale, lerp);
			this.gameObject.transform.localScale = currScale; 
			if (currScale.y <= endScale.y)
			{
			goingUp = true; 
			}
		}
		else
		{
			currScale = Vector3.Lerp (endScale, startScale, lerp);
			this.gameObject.transform.localScale = currScale; 
			if (currScale.y >= startScale.y)
			{
			goingUp = false; 
			}
		}*/
		this.gameObject.transform.localScale = currScale; 
		/*if (lerp <= 0 || lerp >= 1) {
			lerp = Mathf.Clamp01(lerp);
			enabled = false;
		}*/
	}

	void goUp()
	{
	}
}
