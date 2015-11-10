using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserSettings : Singleton<UserSettings> {
	public bool exitMinigameOption = true;
	public bool exitConversationOption = true;
	public bool instantAnswer = true;

	public float minProcessingTime = 0.0f;
	public float maxProcessingTime = 3.0f;
	public float optionsProcessingTime = 0.0f;

	public void Awake()
	{
		instance = this;
		//loadUserSettings ();
	}

	public void OnToggleExitMinigames(bool value)
	{
		exitMinigameOption = value;
	}

	public void OnToggleExitConversation(bool value)
	{
		exitConversationOption = value;
	}
	
	public void OnToggleInstantAnswer(bool value)
	{
		instantAnswer = value;
	}

	public void OnProcessingTimeChange(float value)
	{
		optionsProcessingTime = value * (maxProcessingTime - minProcessingTime) + minProcessingTime;
	}

	// inverse of OnProcessingTimeChange
	public float getSliderValueForProcessingTime(float optionsProcessingTime) {
		return (optionsProcessingTime - minProcessingTime) / (maxProcessingTime - minProcessingTime);
	}

	public void MusicSliderChange(float value) {
		AudioManager.Instance.musicVolume = value;
	}

	public void SoundFXSliderChange(float value) {
		AudioManager.Instance.soundFXVolume = value;
	}



	public void loadUserSettings() {
		DBUserSettings dbSettings = MainDatabase.Instance.getUserSettings(ApplicationState.Instance.userID);
		if(dbSettings != null) {
			AudioManager.Instance.musicVolume = dbSettings.VolumeMusic;
			AudioManager.Instance.soundFXVolume = dbSettings.VolumeSFX;
			AudioManager.Instance.backgroundMusicIndex = dbSettings.BackgroundTrack;
			if(Player.instance != null) {
				AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.musicVolume);
			}
			OnProcessingTimeChange(dbSettings.DialogueProcessingTime);
			exitMinigameOption = dbSettings.ExitMinigameOption;
			exitConversationOption = dbSettings.ExitConversationOption;
			instantAnswer = dbSettings.InstantAnswer;
		}
	}

	public void saveUserSettings() {
		MainDatabase.Instance.setUserSettings(ApplicationState.Instance.userID, AudioManager.Instance.musicVolume, AudioManager.Instance.soundFXVolume, AudioManager.Instance.backgroundMusicIndex, optionsProcessingTime, (exitConversationOption ? 1 : 0), (instantAnswer ? 1 : 0), (exitMinigameOption ? 1 : 0));
	}
}
