using UnityEngine;
using System.Collections;

public class GameIntroManager : MonoBehaviour {
	public CutScene myCutscene;
	public AudioClip backgroundAudio;
	public float backgroundMusicVolume = 0.1f;
	// Use this for initialization
	void Start () 
	{
		Invoke("play", 0.5f);
		AudioManager.Instance.PlayMusic (backgroundAudio, backgroundMusicVolume);
	}
	
	void play()
	{
		myCutscene.PlayIntroCutScene();
	}
	
	public void CutSceneReturn()
    {
      ApplicationState.Instance.LoadLevelWithLoading(ApplicationState.LevelNames.BEDROOM,MenuButton.MenuType.None);
    }
}
