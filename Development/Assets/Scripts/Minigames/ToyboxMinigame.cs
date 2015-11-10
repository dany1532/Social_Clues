using UnityEngine;
using System.Collections;

public class ToyboxMinigame : MonoBehaviour {
	
	// The background audio
	public AudioClip backgroundAudio;
	public bool showTutorial;
	public GameObject myTutorial;
	
	// Use this for initialization
	void Start () {
	
		// If minigame has a background audio, fade it in
		if (backgroundAudio != null)
			AudioManager.Instance.FadeMusic(backgroundAudio, 0.1f);

	}

	void Awake()
	{
		//if there is no tutorial for this level, turn off the tutorial object
		if(!showTutorial && myTutorial)
		{
			//Debug.Log("shutting down tutorial");
			myTutorial.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
