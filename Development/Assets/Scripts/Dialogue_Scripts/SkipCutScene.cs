using UnityEngine;
using System.Collections;

public class SkipCutScene : MonoBehaviour {
	CutScene scene;
	// Use this for initialization
	void Start () {
		scene = GameObject.FindObjectOfType(typeof(CutScene)) as CutScene;
	}

	public void SetActive (bool value)
	{
		this.gameObject.SetActive(value);
	}
	
	void OnClick () {
		scene.Skip();
		SetActive(false);
	}
}
