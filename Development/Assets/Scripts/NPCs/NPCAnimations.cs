using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCAnimations : MonoBehaviour
{
    public enum AnimationIndex
    {
        NONE = -1,
        PRE_DIALOGUE = 1,
        EMOTION_BEGIN = 4,
        EMOTION_MID,
        EMOTION_END,
        COMPREHEND_BEGIN = 7,
        COMPREHEND_MID,
        COMPREHEND_END,
        INITIATE_BEGIN = 10,
        INITIATE_MID,
        INITIATE_END,
        MAINTAIN_BEGIN = 13,
        MAINTAIN_MID,
        MAINTAIN_END,
        PROBLEM_SOLVE_BEGIN = 16,
        PROBLEM_SOLVE_MID,
        PROBLEM_SOLVE_END,
        MINIGAME_RETURN_BEGIN,
        MINIGAME_RETURN_MID,
        MINIGAME_RETURN_END,
        FAVOR_BEGIN,
        FAVOR_MID,
        FAVOR_END,
        REWARD_BEGIN,
        REWARD_MID,
        REWARD_END,
        RETURN_BEGIN,
        RETURN_MID,
		RETURN_END,
		LISTENING = 1024,
		THINKING,
		THINKING_FLIPPED,
		THINKING_TALKING,
		MAKE_CHOICE = 1032,
		RIGHT_CHOICE,
		WRONG_CHOICE,
		WAITING,
		REACH_FOR_TOY = 1064,
		RECEIVE_TOY,
		LEVEL_COMPLETE,
        IDLE = 1240,
        WALKING = 1242,
        WALKING_LEFT_RIGHT,
        WALKING_UP,
        WALKING_DOWN,
		MAGNIFYING_GLASS = 2047,
		WITH_TRAY = 2048,
        PLAYER_NO_EYES,
        SHERLOCK = 4096,
        NEUTRAL,
        INSTRUCTING,
        TALKING,
        CURIOUS,
        DOUBTFUL,
        HAPPY,
        CELEBRATING,
        WINK
    }

    [System.Serializable]
    public class AnimationSequence
    {
        public AnimationIndex animationIndex;
        public List<Texture> textures;
        public float speed = 1;
    }
    public List<AnimationSequence> animations;
   
	public bool PlayAnimationSequence(AnimationIndex index, DialogueWindow.CharacterTexture characterTexture)
	{
		NPCAnimations.AnimationSequence playerAnim = RetrieveAnimationSequence(index);
		List<Texture> playerTex = playerAnim.textures;
		if (playerTex.Count > 0)
		{
			characterTexture.animation.StopAnimation();
			characterTexture.animation.SetAnimationList(playerTex);
			characterTexture.animation.PlayAnimation();
			characterTexture.animation.SetSpeed(playerAnim.speed);
            characterTexture.stretch.initialSize = new Vector2 (playerTex[0].width, playerTex[0].height);

			return true;
		} else
		{
			Debug.LogError("No texures were found for index " + index.ToString());
			return false;
		}
	}

    public AnimationSequence RetrieveAnimationSequence(AnimationIndex index)
    {
        if (animations == null || animations.Count == 0)
            return null;
        
        AnimationSequence prevSequence = null;
        foreach (AnimationSequence sequence in animations)
        {
            if (sequence.animationIndex == index)
                return sequence;
            else if (prevSequence == null && (int)sequence.animationIndex > (int)index)
                prevSequence = sequence;
        }
        if (prevSequence == null)
            prevSequence = animations [0];
        return prevSequence;
    }

    public List<Texture> RetrieveAnimationList(AnimationIndex index)
    {
        if (animations == null || animations.Count == 0)
            return null;

        AnimationSequence prevSequence = null;
        foreach (AnimationSequence sequence in animations)
        {
            if (sequence.animationIndex == index)
                return sequence.textures;
            else if (prevSequence == null && (int)sequence.animationIndex > (int)index)
                prevSequence = sequence;
        }
        if (prevSequence == null)
            prevSequence = animations [0];
        return prevSequence.textures;
    }
}
