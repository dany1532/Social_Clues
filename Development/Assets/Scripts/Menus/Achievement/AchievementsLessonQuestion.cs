using UnityEngine;
using System.Collections;

public class AchievementsLessonQuestion : MonoBehaviour {

	public AudioClip audioClip;
	public float delay;

	void OnClick ()
	{
		if (ApplicationState.Instance.isPaused () || audioClip == null)
			return;
		CancelInvoke ();
		Invoke ("PlayVoiceOver", delay);
	}

	void PlayVoiceOver()
	{
		AudioManager.Instance.PlayVoiceOver(audioClip, 1);
	}
}
