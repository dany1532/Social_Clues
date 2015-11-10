using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Audio manager to play all background music, sound effects and voice overs
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    /// <summary>
    /// Clip info to manage playing clips, especially volume normalization
    /// </summary>
    public class ClipInfo
    {
        // Audio srouce of clip
        public AudioSource source { get; set; }
        // Default volume of clip if audio player is at full volume
        public float defaultVolume { get; set; }
    }
	
	#region Clips playing
    // List of all audio clips currently playing
    private List<ClipInfo> activeAudio;
	
    // Current playing music
    private ClipInfo activeMusic;
    // Music clip fading in to replace current music
    private ClipInfo pendingMusic;
	
    // Active voice over audio
    private AudioSource activeVoiceOver;
	#endregion
	
	#region Volume control
    // Mute all audio
    public bool mute = false;
    // Current maximum volume
    public float musicVolume = 1;
    float maxMusicVolume = 1;
    public float soundFXVolume = 1;
	
    // Volume multiplayer and minimum volume
    private float volumeMod = 1.0f, volumeMin;
	
    private float dialogueVolume = 0.5f;	// NOT BEING USED
	
    // Whether voice over is being faded in or out
    private bool vOfade;
	
    // Music volume
    //private float mVolume;
    public float mVolume;

    // Whether music is being faded in or out
    private bool mFade;

    private List<AudioClip> backgroundMusic;
    // stores index of background track for backgroundMusic
    public int backgroundMusicIndex;
    AudioClip backgroundClip;

	#endregion

    void Start()
    {
        //DBUserSettings userSettings = MainDatabase.Instance.getUserSettings(ApplicationState.Instance.userID);
        //musicVolume = userSettings.VolumeMusic;
        //Debug.Log ("music volume: " + userSettings.VolumeMusic);
        //soundFXVolume;
    }

    void Awake()
    {
        // Initialize background music
        backgroundMusicIndex = 0;
        backgroundMusic = new List<AudioClip>();
        backgroundClip = (AudioClip)Resources.Load("Audio/Background Music/Social Clues Idea 4 V1.2 Unlooped & Faded", typeof(AudioClip));
        //backgroundMusic.Add((AudioClip)Resources.Load ("Audio/Background Music/Social Clues Idea 4 V1.2 Unlooped & Faded", typeof(AudioClip)));
        //backgroundMusic.Add((AudioClip)Resources.Load ("Audio/Background Music/Social Clues Idea 5 V1.0 Unlooped & Faded", typeof(AudioClip)));
        //backgroundMusic.Add((AudioClip)Resources.Load ("Audio/Background Music/Social Clues MX Idea 2 V1.0-Stereo Out", typeof(AudioClip)));

        // Initialize list of active audio
        activeAudio = new List<ClipInfo>();
        // Initialize active audio clips
        activeVoiceOver = null;
        activeMusic = null;

        // Move audio manager under main camera
		
        /*
		GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if (mainCamera == null)
		{
			mainCamera = GameObject.Find("Camera");
		}
		if (mainCamera != null)
		{
			transform.parent = mainCamera.transform;
			transform.localPosition = new Vector3(0, 0, 0);
		}
		*/
		
        // Initialize volume information
        volumeMod = 1f;
        volumeMin = 0.1f;
        vOfade = false;
        mVolume = 0.1f;
        mFade = false;
		
        DontDestroyOnLoad(gameObject);
    }
	
    void Update()
    {
        // If voice over is being faded in
        if (vOfade && volumeMod >= volumeMin)
        {
            // reduce volume multiplier for the other audio clips (fade out clips)
            volumeMod -= Time.deltaTime;
            // If voice over is being faded out, restoring full volume
        } else if (!vOfade && volumeMod < 1f)
        {
            // increase volume multiplier for all audio clips (fade in clips)
            volumeMod += Time.deltaTime;
        }
		
        // If new music clip is being faded in
        if (mFade)
        {
            // If current music volume is big enough
            if (mVolume > 0.01f)
            {
                // Decrease music volume
                mVolume -= 0.1f * Time.deltaTime;
                // Fade out active music clip based on music volume
                activeMusic.defaultVolume = mVolume;
                // Fade in new music clips based on music volume
                pendingMusic.defaultVolume = 0.1f - mVolume;
            }
			// is current music is almost zero
			else
            {
                // Stop music fading
                mFade = false;
                // Stop active music
                stopSound(activeMusic);
                // Set volume to new active audio to full;
                pendingMusic.defaultVolume = 0.1f;
                // Destroy previous music clip game object
                Destroy(activeMusic.source.gameObject);
                // Set new playing music to the one being faded in
                activeMusic = pendingMusic;
                // Set fading in music clip to null
                pendingMusic = null;
            }
        }
		
        // Update volume to all playing audio clips
        updateActiveAudio();
    }

	#region Audio
    /// <summary>
    /// Update the audio source based on the given clip
    /// </summary>
    /// <param name='source'>
    /// Audio source to be updated
    /// </param>
    /// <param name='clip'>
    /// Audio clip to be played
    /// </param>
    /// <param name='volume'>
    /// Maximum volume of the audio clip
    /// </param>
    private void setSource(ref AudioSource source, AudioClip clip, float volume)
    {
        // Update source information
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0.2f;
        source.minDistance = 20;
        source.maxDistance = 100;
        source.mute = mute;
        source.clip = clip;
        source.volume = volume;
    }
	
    /// <summary>
    /// Updates the volume of all playing audio clips
    /// </summary>
    private void updateActiveAudio()
    { 
        // Initialize list for clips to be removed
        var toRemove = new List<ClipInfo>();
        try
        {
            // If there is no active voice over
            if (!activeVoiceOver)
            {
                // The disable voice over fade
                vOfade = false;
            }
            // Go through all playing audio clips
            foreach (var audioClip in activeAudio)
            {
				
                // If the audio clip source does't exist any more (got destroyed)
                if (!audioClip.source)
                {
                    // Mark it to be removed
                    toRemove.Add(audioClip);
                    // If audio source clip is not the active voice over
                } else if (audioClip.source != activeVoiceOver)
                {
                    if (audioClip == activeMusic)
						// change volume based on volume multiplier and current volume
                        audioClip.source.volume = audioClip.defaultVolume * volumeMod * musicVolume * maxMusicVolume;
                    else // sound effect
						// change volume based on volume multiplier and current volume
                        audioClip.source.volume = audioClip.defaultVolume * volumeMod * soundFXVolume;
                } else
                {
                    // change volume of active voice over
                    audioClip.source.volume = audioClip.defaultVolume;
                }
            }
        } catch
        {
            Debug.Log("Error updating active audio clips");
            return;
        }
        /*
		if (activeMusic != null) {
			//if (DialogueWindow.instance.isShowing()) {
			if(DialogueWindow.instance.isShowing) {
				activeMusic.source.volume = audioClip.defaultVolume * dialogueVolume * currentVolume;
			} else {
				activeMusic.source.volume = audioClip.defaultVolume * Mathf.Min(volumeMod, 1) * currentVolume;
			}
		}
		*/
		
        // Remvoe all clips that were marked to be removed
        foreach (var audioClip in toRemove)
        {
            activeAudio.Remove(audioClip);
        }
    }
	
    /// <summary>
    /// Play the specified clip, at given origin, with a specific volume and loop.
    /// </summary>
    /// <param name='clip'>
    /// Clip to be played
    /// </param>
    /// <param name='soundOrigin'>
    /// Origin where the audio will be played from
    /// </param>
    /// <param name='volume'>
    /// Max volume of the audio clip
    /// </param>
    /// <param name='loop'>
    /// Whether the audio clip will be looped or not
    /// </param>
    public ClipInfo Play(AudioClip clip, Vector3 soundOrigin, float volume, bool loop)
    {
        // Create an empty game object to hold audio source
        GameObject soundLoc = new GameObject("Audio: " + clip.name);
        // and set it to origin
        soundLoc.transform.position = soundOrigin;
		
        // Create the source clip
        AudioSource source = soundLoc.AddComponent<AudioSource>();
        setSource(ref source, clip, volume);
        // Set whether source will be looped
        source.loop = loop;
        // Start playing audio source
        source.Play();
		
        // If audio will not be looped
        if (!loop)
			// Destroy the game object of the audio after the clip ends (length of the audio)
            Destroy(soundLoc, clip.length);
		
        // Create new clip information
        ClipInfo clipInfo = new ClipInfo{source = source, defaultVolume = source.volume};
        // Add the audio to the playing audio clips
        activeAudio.Add(clipInfo);
		
        // Return clip info
        return clipInfo;
    }
	
    /// <summary>
    /// Play the specified clip, make it a child under the emitter, with a specific volume and loop.
    /// </summary>
    /// <param name='clip'>
    /// Clip to be played
    /// </param>
    /// <param name='emitter'>
    /// Emitter that the audio will be a child of
    /// </param>
    /// <param name='volume'>
    /// Max volume of the audio clip
    /// </param>
    /// <param name='loop'>
    /// Whether the audio clip will be looped or not
    /// </param>
    public ClipInfo Play(AudioClip clip, Transform emitter, float volume, bool loop)
    {
        // Play audio clip at the emitter position
        var clipInfo = Play(clip, emitter.position, volume, loop);
        // Set clip parent to the emitter
        clipInfo.source.transform.parent = emitter;
        // reutrn clip info
        return clipInfo;
    }
	
    /// <summary>
    /// Stop playing audio based on given audio source
    /// </summary>
    /// <param name='toStop'>
    /// To stop.
    /// </param>
    public void stopSound(AudioSource toStop)
    {
        try
        {
            // Destroy game object of given audio source
            Destroy(activeAudio.Find(s => s.source == toStop).source.gameObject);
        } catch
        {
            Debug.Log("Error trying to stop audio source " + toStop);
        }
    }
	
    /// <summary>
    /// Stop playing audio based on given clip info
    /// </summary>
    /// <param name='toStop'>
    /// To stop.
    /// </param>
    public void stopSound(ClipInfo toStop)
    {
        try
        {
            // Destroy game object of given clip info
            Destroy(activeAudio.Find(s => s == toStop).source.gameObject);
        } catch
        {
            Debug.Log("Error trying to stop audio source " + toStop);
        }
    }
	
    /// <summary>
    /// Mute all audio
    /// </summary>
    public void MuteAll()
    {
        mute = !mute;
        foreach (var audioClip in activeAudio)
        {
            audioClip.source.mute = mute;
        }
    }
	
    /// <summary>
    /// Returns if the audio is mute or not
    /// </summary>
    /// <returns>
    /// Audio is mute
    /// </returns>
    public bool isMute()
    {
        return mute;
    }
	
    /// <summary>
    /// Updated the maximum volume for all audio
    /// </summary>
    /// <param name='newVolume'>
    /// New maximum volume
    /// </param>
    public void ChangeAudio(float newVolume)
    {
        musicVolume = newVolume;
    }
	
    /// <summary>
    /// Pauses / Resumes all playing audio
    /// </summary>
    /// <param name='pause'>
    /// Paused or not
    /// </param>
    public void PauseAll(bool pause)
    {
        // Go through all active audio clips on the scene
        foreach (var audioClip in activeAudio)
        {
            try
            {
                if (pause)
                    audioClip.source.Pause();
                else if (!audioClip.source.isPlaying)
                    audioClip.source.Play();
            } catch
            {
                continue;
            }
        }
    }
	#endregion
	
	#region Sound FXs	
    /// <summary>
    /// Pauses all the sound FX
    /// </summary>
    public void PauseFX(bool pause)
    {
        // Go through all active audio clips on the scene
        foreach (var audioClip in activeAudio)
        {
            try
            {
                // the audio clip is not the playing music or pending music then stop it
                if (audioClip != activeMusic && audioClip != pendingMusic)
                {
                    if (pause)
                        audioClip.source.Pause();
                    else
                        audioClip.source.Play();
                }
            } catch
            {
                continue;
            }
        }
    }
	
    /// <summary>
    /// Unpauses all the soud FX
    /// </summary>
    public void unpauseFX()
    {
        // Go through all active audio clips on the scene
        foreach (var audioClip in activeAudio)
        {
            try
            {
                // if audio clip is not playing
                if (!audioClip.source.isPlaying)
                {
                    // then start playing the clip
                    audioClip.source.Play();
                }
            } catch
            {
                continue;
            }
        }
    }
	#endregion
	
	#region Music

	public void LowerMusicSound ()
	{
		maxMusicVolume *= 0.25f;
	}

	public void RestoreMusicSound ()
	{
		maxMusicVolume *= 4;
        if (maxMusicVolume > 1) maxMusicVolume = 1;
	}
    
    /// <summary>
    /// Play background music
    /// </summary>
    /// <returns>
    /// Clip info of the music being played
    /// </returns>
    /// <param name='music'>
    /// Audio clip of the music to be played
    /// </param>
    /// <param name='volume'>
    /// Max volume of the audio clip to be played
    /// </param>
    public ClipInfo PlayMusic(AudioClip music, float volume)
    {
        // If there is an active music clip set (being played)
        if (activeMusic != null)
			// stop playing active music
            stopSound(activeMusic);
		
        // Set new active music
        activeMusic = Play(music, transform, volume, true);
        return activeMusic;
    }

    public ClipInfo PlayBackgroundMusic(float volume)
    {
        /*
        if (activeMusic != null)
        {
            stopSound(activeMusic);
            Resources.UnloadAsset(backgroundClip);
        }
        */
        switch (backgroundMusicIndex)
        {
            case 0: 
                backgroundClip = (AudioClip)Resources.Load("Audio/Background Music/Social Clues Idea 4 V1.2 Unlooped & Faded", typeof(AudioClip));
                break;
            case 1:
                backgroundClip = (AudioClip)Resources.Load("Audio/Background Music/Social Clues Idea 5 V1.0 Unlooped & Faded", typeof(AudioClip));
			//backgroundMusic.Add((AudioClip)Resources.Load ("Audio/Background Music/Social Clues Idea 4 V1.2 Unlooped & Faded", typeof(AudioClip)));
			//backgroundMusic.Add();
			//backgroundMusic.Add((AudioClip)Resources.Load ("Audio/Background Music/Social Clues MX Idea 2 V1.0-Stereo Out", typeof(AudioClip)));
                break;
            case 2:
                backgroundClip = (AudioClip)Resources.Load("Audio/Background Music/Social Clues MX Idea 2 V1.0-Stereo Out", typeof(AudioClip));
                break;
            case 3:
                backgroundClip = (AudioClip)Resources.Load("Audio/Background Music/WorldMap", typeof(AudioClip));
                break;
            default:
                backgroundClip = (AudioClip)Resources.Load("Audio/Background Music/Social Clues Idea 4 V1.2 Unlooped & Faded", typeof(AudioClip));
                break;
        }
        PlayMusic(backgroundClip, volume);
        return activeMusic;
    }

    /// <summary>
    /// Fade in background music
    /// </summary>
    /// <returns>
    /// Clip info of the music being played
    /// </returns>
    /// <param name='music'>
    /// Audio clip of the music to be played
    /// </param>
    /// <param name='volume'>
    /// Max volume of the audio clip to be played
    /// </param>
    public ClipInfo FadeMusic(AudioClip music, float volume)
    {
        // If there is an active music clip playing which is diffent from given clip, or there is no active music clip playing
        if (activeMusic != null && music != activeMusic.source.clip)
        {
            maxMusicVolume = 1;
            
            // Fade music in
            mFade = true;
			
            // Set music volume to current volume of playing audio clip
            mVolume = activeMusic.defaultVolume;
            // Set pending music clip to new audio clip
            pendingMusic = Play(music, transform, 0, true);
        } else if (activeMusic == null)
        {
            PlayMusic(music, volume);
        }
		
        return pendingMusic;
    }
	#endregion
	
	#region Voice Over
    /// <summary>
    /// Play voice over.
    /// </summary>
    /// <returns>
    /// The voice over source being played
    /// </returns>
    /// <param name='voiceOver'>
    /// The voice over clip to be played
    /// </param>
    /// <param name='volume'>
    /// Max volume of the voice over clip
    /// </param>
    public AudioSource PlayVoiceOver(AudioClip voiceOver, float volume)
    {
        // Play the audio clip
        AudioSource source = Play(voiceOver, transform, volume, false).source;
		
        // If there is a voice over set
        if (activeVoiceOver != null)
			// then stop playing voice over
            stopSound(activeVoiceOver);
		
        // Set active voice over to given source
        activeVoiceOver = source;
		
        // Enable voice over fading
        vOfade = true;
		
        // Return the voice over source being played
        return source;
    }

    public void StopSoundFX()
    {
        // Go through all active audio clips on the scene
        foreach (var audioClip in activeAudio)
        {
            try
            {
                // the audio clip is not the playing music or pending music then stop it
                if (audioClip != activeMusic && audioClip != pendingMusic)
                {
                    stopSound(audioClip);
                }
            } catch
            {
                continue;
            }
        }
    }

    public void StopVoiceOver()
    {
        // If there is a voice over set
        if (activeVoiceOver != null)
			// then stop playing voice over
            stopSound(activeVoiceOver);
		
        // Set active voice over to given source
        activeVoiceOver = null;
		
        // Enable voice over fading
        vOfade = false;
    }
	#endregion
}
