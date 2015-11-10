using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {
	public int backgroundMusicIndex;
	public GameObject userSettings;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		AudioManager.Instance.backgroundMusicIndex = backgroundMusicIndex;
		userSettings.GetComponent<CloseUserSettings>().updateBackgroundMusicSprites();
		//UserSettings.Instance.updateBackgroundMusicSprites();
		if(Player.instance != null) {
			//Debug.Log ("Player exists");
			AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.musicVolume);
		} else {
			//Debug.Log ("Player does not exist");
		}
	}

	void OnHover(bool isOver) {
		/*
		if(isOver) {
			this.GetComponent<UISprite>().spriteName = "MusicalNoteColored";
		} else {
			this.GetComponent<UISprite>().spriteName = "MusicalNote";
		}
		*/
	}
}
