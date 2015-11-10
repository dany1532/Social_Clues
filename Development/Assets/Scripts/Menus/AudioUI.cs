using UnityEngine;
using System.Collections;

public class AudioUI : MonoBehaviour {
	
	public Texture mute;
	public Texture unmute;
	
	UISprite audioUI;
	
	// Use this for initialization
	void Start () {
		audioUI = GetComponentInChildren<UISprite>();
		
		if (audioUI == null)
			gameObject.SetActive(false);
		else
			UpdateUIIcon();
	}
	
	/// <summary>
	/// Raises the press event.
	/// </summary>
	/// <param name='pressed'>
	/// If the ui was pressed or released
	/// </param>
	void OnPress (bool pressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && gameObject != null)
		{
			if (pressed)
			{
				AudioManager.Instance.MuteAll();
				UpdateUIIcon();
				InputManager.Instance.ReceivedUIInput();
			}
		}
	}
	
	/// <summary>
	/// Updates the UI icon for audio (mute or not)
	/// </summary>
	void UpdateUIIcon()
	{
		if (AudioManager.Instance.isMute())
			audioUI.spriteName = Utilities.SetTextureName(mute.name);
		else
			audioUI.spriteName = Utilities.SetTextureName(unmute.name);
	}
}
