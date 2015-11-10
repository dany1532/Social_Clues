using UnityEngine;
using System.Collections;

public class DuckWalk : MonoBehaviour {
	
	public float time = .2f; //time between shakes. .2 default. Increase to slow down the animation
	public float shakeDistance = 60.0f; 
	public float forwardDistance = 0.0f;
	//public int numSteps = 0; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Shake()
	{
		StartCoroutine("beginShake");
	}
	public IEnumerator beginShake()
	{
		TweenPosition.Begin(gameObject, time/2, gameObject.transform.localPosition + new Vector3((shakeDistance/2), 0, (forwardDistance/3)));
		yield return new WaitForSeconds(time/2);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(-(shakeDistance), 0, (2*forwardDistance/3)));
		yield return new WaitForSeconds(time);
		//TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(shakeDistance, 0, 0));
		//yield return new WaitForSeconds(time);
		//TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(-(shakeDistance), 0, 0));
		//yield return new WaitForSeconds(time);
		//TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(shakeDistance, 0, (forwardDistance/numSteps)));
		//yield return new WaitForSeconds(time);
		//TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(-(shakeDistance/2), 0, (forwardDistance/numSteps)));
		TweenPosition.Begin(gameObject, time/2, gameObject.transform.localPosition + new Vector3((shakeDistance/2), 0, (forwardDistance)));
		
	}
	
}

