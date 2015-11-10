using UnityEngine;
using System.Collections;

[AddComponentMenu("Social Clues/Play Sound FX")]
public class PlaySoundFX : MonoBehaviour {
	
	public enum Trigger
	{
		OnClick,
		OnPress,
		OnRelease,
	}
	
	public AudioClip audioClip;
	public Trigger trigger = Trigger.OnClick;
	public float volume = 1f;
	public bool loop = false;
	public enum TypeOfAudio
	{
		VOICE_OVER,
		SOUND_FX
	}
	public TypeOfAudio audioType = TypeOfAudio.SOUND_FX;

	void OnPress (bool isPressed)
	{
		if (ApplicationState.Instance.isPaused ())
			return;
		if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			if (audioType == TypeOfAudio.SOUND_FX)
				AudioManager.Instance.Play(audioClip, this.transform, volume, loop);		
			else
				AudioManager.Instance.PlayVoiceOver(audioClip, volume);
		}
	}
	
	void OnClick ()
	{
		if (ApplicationState.Instance.isPaused ())
			return;
		if (enabled && trigger == Trigger.OnClick)
		{
			if (audioType == TypeOfAudio.SOUND_FX)
				AudioManager.Instance.Play(audioClip, this.transform, volume, loop);		
			else
				AudioManager.Instance.PlayVoiceOver(audioClip, volume);
		}
	}
}
