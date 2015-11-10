using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundFXLibrary : MonoBehaviour {
 
    [System.Serializable]
    public class SoundFX
    {
        public AudioClip clip;
        public NPCAnimations.AnimationIndex audioIndex;
    }
    
    public List<SoundFX> soundFXs;
    
	public AudioClip RetrieveAudioClip(NPCAnimations.AnimationIndex index)
    {
        foreach(SoundFX soundFX in soundFXs)
        {
            if (soundFX.audioIndex == index)
                return soundFX.clip;
        }
        
        return null;
    }
}
