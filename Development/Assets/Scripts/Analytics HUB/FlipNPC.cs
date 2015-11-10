using UnityEngine;

using System.Collections;

public class FlipNPC : MonoBehaviour {

	public GameObject front;
	public GameObject back;

	private bool frontSideUp;
	public float updateSpeed = 0.01f;

	// Use this for initialization
	void Start () {
		frontSideUp = true;
		back.SetActive (false);
		//back.transform.localPosition = new Vector3(back.transform.localPosition.x - 640.0095f, back.transform.localPosition.y, back.transform.localPosition.z);
		back.transform.RotateAround(transform.collider.bounds.center, Vector3.up, 180f);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnClick() {
		//transform.RotateAround(new Vector3(0, transform.collider.bounds.center.y, 0), Mathf.PI);
		//transform.RotateAround(transform.collider.bounds.center, Vector3.up, 180f);

		StartCoroutine(test());
	}

	IEnumerator test() {
		for(int i = 0; i < 45; ++i) {
			yield return new WaitForSeconds(updateSpeed);
			transform.RotateAround(transform.collider.bounds.center, Vector3.up, 2f);
		}

		if(frontSideUp) {
			front.SetActive(false);
			back.SetActive(true);
			frontSideUp = false;
		} else {
			front.SetActive(true);
			back.SetActive(false);
			frontSideUp = true;
		}

		for(int i = 0; i < 45; ++i) {
			yield return new WaitForSeconds(updateSpeed);
			transform.RotateAround(transform.collider.bounds.center, Vector3.up, 2f);
		}

		yield return null;
	}
}
