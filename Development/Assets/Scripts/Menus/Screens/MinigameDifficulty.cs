using UnityEngine;
using System.Collections;

public class MinigameDifficulty : MonoBehaviour {

	public enum Difficulty
	{
		EASY = 0,
		MEDIUM = 1,
		HARD = 2
	}
	
	public MinigameSelect minigameSelect;
	public Difficulty difficulty;
	
	public void OnClick()
	{
		transform.parent = null;
		DontDestroyOnLoad(this);
		ApplicationState.Instance.LoadLevelWithLoading(minigameSelect.levelName);
		
		for (int i = 0 ; i < transform.childCount ; i++)
			Destroy(transform.GetChild(i).gameObject);
	}
}
