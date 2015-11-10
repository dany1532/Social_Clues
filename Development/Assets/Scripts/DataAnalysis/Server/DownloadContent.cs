using UnityEngine;
using System.Collections;
using  System.IO;

public class DownloadContent : MonoBehaviour {

	// Use this for initialization
	public UILabel processper;
	WWW www;

	void OnClick()
	{
		StartCoroutine("DownloadFile");
	}

	private IEnumerator DownloadFile()
	{
		www = new WWW("http://halfwaydown.x10.mx/SocialClues/Amy.prefab");
		//yield return www;
		
		while (!www.isDone) {
			processper.text = "downloaded " + (www.progress*100).ToString() + "%...";
			yield return null;
		}
		string fullPath;
#if !UNITY_EDITOR
		fullPath = Application.persistentDataPath + "/DownloadedAmy.prefab";
#else
		fullPath = Application.dataPath+ "/DownloadedAmy.prefab";
#endif
		Debug.Log(fullPath);
		File.WriteAllBytes (fullPath, www.bytes);
		
		processper.text = "downloaded @ "+ fullPath ;
	}
}
