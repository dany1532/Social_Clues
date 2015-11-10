using UnityEngine;
using System.Collections;

public class BalkiShake : MonoBehaviour {

	public float time = .2f; //time between shakes. .2 default. Increase to slow down the animation
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
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(30.0f, 0, 0));
		yield return new WaitForSeconds(time);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(-60.0f, 0, 0));
		yield return new WaitForSeconds(time);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(60.0f, 0, 0));
		yield return new WaitForSeconds(time);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(-60.0f, 0, 0));
		yield return new WaitForSeconds(time);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(60.0f, 0, 0));
		yield return new WaitForSeconds(time);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(-30.0f, 0, 0));

	}

}
