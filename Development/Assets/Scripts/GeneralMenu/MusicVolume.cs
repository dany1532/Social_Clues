using UnityEngine;
using System.Collections;

public class MusicVolume : MonoBehaviour {
	public enum VOLUME_TYPE { Music, FX };
	public enum CHANGE { Increase, Decrease};
	public VOLUME_TYPE volumeType;
	public CHANGE type;

	public UISlider slider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnSliderChange(float value) {
		if(volumeType == VOLUME_TYPE.Music) {
			AudioManager.Instance.musicVolume = value;
		} else {
			AudioManager.Instance.soundFXVolume = value;
		}
	}

	void OnClick() {
		if(volumeType == VOLUME_TYPE.Music) {
			if(type == CHANGE.Increase) {
				AudioManager.Instance.musicVolume += 0.1f;
				updateSlider();
			} else {
				AudioManager.Instance.musicVolume -= 0.1f;
				updateSlider();
			}
		} else {
			if(type == CHANGE.Increase) {
				AudioManager.Instance.soundFXVolume += 0.1f;
				updateSlider();
			} else {
				AudioManager.Instance.soundFXVolume -= 0.1f;
				updateSlider();
			}
		}
	}

	void updateSlider() {
		if(volumeType == VOLUME_TYPE.Music) {
			slider.sliderValue = AudioManager.Instance.musicVolume;
		} else {
			slider.sliderValue = AudioManager.Instance.soundFXVolume;
		}
	}
}
