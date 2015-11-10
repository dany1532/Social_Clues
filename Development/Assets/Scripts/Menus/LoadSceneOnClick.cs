using UnityEngine;
using System.Collections;

public class LoadSceneOnClick : MonoBehaviour {
		
	public int newLevelIdx = -1;
	public string newLevelName;
	
	void OnClick ()
	{
		if (newLevelIdx > -1)
			ApplicationState.Instance.LoadLevelWithLoading(newLevelIdx);
		else
			ApplicationState.Instance.LoadLevelWithLoading(newLevelName);			
	}
}
