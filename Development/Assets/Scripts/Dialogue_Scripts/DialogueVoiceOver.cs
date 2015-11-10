using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueVoiceOver : MonoBehaviour {

	public enum VoiceOverID
	{
		NATIVE = -1,
		NONE = 0,
		INITIATE_CONV = 10,
		INITIATE_CONV_1,
		INITIATE_CONV_2 = 13,
		INITIATE_CONV_3 = 15,
		INITIATE_CONV_4 = 17,
		MAINTAIN_CONV = 20,
		MAINTAIN_CONV_1,
		MAINTAIN_CONV_2 = 23,
		MAINTAIN_CONV_3 = 25,
		MAINTAIN_CONV_4 = 27,
		PROBLEM_SOLVING = 30,
		PROBLEM_SOLVING_1,
		PROBLEM_SOLVING_2 = 33,
		PROBLEM_SOLVING_3 = 35,
		PROBLEM_SOLVING_4 = 37,
		MINIGAME_RETURN = 40,
		MINIGAME_RETURN_1,
		MINIGAME_RETURN_2,
		MINIGAME_RETURN_3,
		MINIGAME_RETURN_4,
		FAVOR = 50,
		FAVOR_1,
		FAVOR_2,
		FAVOR_3,
		FAVOR_4,
		REWARD = 60,
		REWARD_1,
		REWARD_2,
		REWARD_3,
		REWARD_4,
		GENERAL_REPLY_1 = 100,
		GENERAL_REPLY_2,
		GENERAL_REPLY_3,
		GENERAL_REPLY_4,
		GENERAL_REPLY_5,
		GENERAL_REPLY_6,
		GENERAL_REPLY_7,
		GENERAL_REPLY_8
	}

	[System.Serializable]
	public class VoiceOverAudio
	{
		public VoiceOverID voiceOverId = VoiceOverID.NATIVE;
		public AudioClip voiceOverAudio;
 	}
	public List<VoiceOverAudio> voiceOvers;

	public AudioClip RetrieveVoiceOverAudio(VoiceOverID id,  bool removeVoiceOver = false)
	{
		int index = 0;
		foreach (VoiceOverAudio voiceOver in voiceOvers) {
			if (voiceOver.voiceOverId == id)
			{
				if (!removeVoiceOver)
					return voiceOver.voiceOverAudio;
				else
				{
					AudioClip audioClip = voiceOver.voiceOverAudio;
					voiceOvers.RemoveAt(index);
					return audioClip;
				}
			}
			index++;
		}
		return null;
	}
}
