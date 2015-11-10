using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloseUserSettings : MonoBehaviour {
	public GameObject eventReceiver;
	public string functionName = "saveUserSettings";
	
	public UISlider musicVolumeSlider;
	public UISlider soundFXSlider;
	public UISlider processingTimeSlider;

	public Toggle conversationToggle;
	public Toggle exitMinigameToggle;

	public UILabel minProcessingTime;
	public UILabel maxProcessingTime;

	public List<UISprite> backgroundMusic = new List<UISprite>();

	public bool parentPassword = true;
	public GameObject parentHide;	// reference to everything that must be hidden by parent password
	public GameObject passwordMenu;
	public bool inMainMenu = false;

	// Use this for initialization
	void Awake () {
		eventReceiver = UserSettings.Instance.gameObject;

		transform.parent.localPosition = new Vector3 (0, 0, -1);
		inMainMenu = !GameManager.WasInitialized () && !ApplicationState.Instance.InMinigame ();
		if (inMainMenu == false)
		{
			Destroy (GetComponent<MenuButton>());
			eventReceiver = UserSettings.Instance.gameObject;
		}
        else
            parentPassword = false;
        
		if(parentPassword == true) {
			parentHide.SetActive(false);
		} else {
			passwordMenu.SetActive(false);
		}
	}

	void OnEnable() {
		if(ApplicationState.Instance.userID != -1) {
			DBUserSettings dbSettings = MainDatabase.Instance.getUserSettings(ApplicationState.Instance.userID);
			musicVolumeSlider.sliderValue = dbSettings.VolumeMusic;
			soundFXSlider.sliderValue =  dbSettings.VolumeSFX;
			AudioManager.Instance.backgroundMusicIndex = dbSettings.BackgroundTrack;
			//UserSettings.Instance.updateBackgroundMusicSprites();
			updateBackgroundMusicSprites();
			processingTimeSlider.sliderValue = UserSettings.Instance.getSliderValueForProcessingTime(dbSettings.DialogueProcessingTime);
			conversationToggle.setState(dbSettings.ExitConversationOption);
			exitMinigameToggle.setState(dbSettings.ExitMinigameOption);
			minProcessingTime.text = UserSettings.Instance.minProcessingTime.ToString();
			maxProcessingTime.text = UserSettings.Instance.maxProcessingTime.ToString();
		}
	}

	void OnClick() {
		if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
		{
			eventReceiver.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
		}
		
		if (inMainMenu == false)
		{
			Destroy(transform.parent.gameObject);
		}
	}

	public void updateBackgroundMusicSprites() {
		for(int i = 0; i < backgroundMusic.Count; ++i) {
			backgroundMusic[i].spriteName = "MusicalNote";
		}
		backgroundMusic[AudioManager.Instance.backgroundMusicIndex].spriteName = "MusicalNoteColored";
	}
	
	public void OnToggleExitMinigames(bool value)
	{
		if (GameManager.WasInitialized () && GameManager.Instance.InConversation () && GameManager.Instance.InMinigame()) {
			PauseExitButton exitButton = GameObject.FindObjectOfType(typeof(PauseExitButton)) as PauseExitButton;

			exitButton.SetVisible(value);
		}

		UserSettings.Instance.OnToggleExitMinigames(value);
	}
	
	public void OnToggleExitConversation(bool value)
	{
		if (GameManager.WasInitialized () && GameManager.Instance.InConversation ()) {
			OnPauseUI onPauseUI = GameObject.FindObjectOfType(typeof(OnPauseUI)) as OnPauseUI;
			
			onPauseUI.exitButton.SetVisible(value);
		}

		UserSettings.Instance.OnToggleExitConversation(value);
	}
	
	public void OnToggleInstantAnswer(bool value)
	{
		UserSettings.Instance.OnToggleInstantAnswer(value);
	}
	
	public void OnProcessingTimeChange(float value)
	{
		UserSettings.Instance.OnProcessingTimeChange(value);
	}
	
	// inverse of OnProcessingTimeChange
	public float getSliderValueForProcessingTime(float optionsProcessingTime) {
		return UserSettings.Instance.getSliderValueForProcessingTime(optionsProcessingTime);
	}
	
	public void MusicSliderChange(float value) {
		UserSettings.Instance.MusicSliderChange(value);
	}
	
	public void SoundFXSliderChange(float value) {
		UserSettings.Instance.SoundFXSliderChange(value);
	}

	private void enableParentMenu() {
		parentHide.SetActive(true);
	}
}
