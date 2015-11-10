using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimations : MonoBehaviour {

	public List<NPCAnimations.AnimationIndex> animationIndexes;
	public NPCAnimations animations;

	void Start()
	{
		if (animations == null) {
			animations = (NPCAnimations)gameObject.AddComponent (typeof(NPCAnimations));
			animations.animations = new List<NPCAnimations.AnimationSequence> ();
		}

		GameObject animationsPrefab = ResourceManager.LoadObject ("Player/" + ApplicationState.Instance.selectedCharacter + "Animations");
		if (animationsPrefab != null) {
			NPCAnimations playerInstanceAnimations = animationsPrefab.GetComponent<NPCAnimations> ();

			foreach (NPCAnimations.AnimationIndex index in animationIndexes) {
					NPCAnimations.AnimationSequence sequence;
					animations.animations.Add (playerInstanceAnimations.RetrieveAnimationSequence (index));
			}

			playerInstanceAnimations = null;
			animationsPrefab = null;
			Resources.UnloadUnusedAssets ();
		}
	}
}
