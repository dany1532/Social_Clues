using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour {
	public List<Transform> movingObjects;
	List<Vector3> targetPositions;
	public UISprite fillingObject;
	
	private float distance;
	private float progress = 0;
	private float previousProgress = 0;
	AsyncOperation async;
	public bool loadingFinished;
	public float fillTime = 1;
	
	void Start()
	{
		Resources.UnloadUnusedAssets();
		
		loadingFinished = false;
		//distance = fillingObject.transform.localScale.x;
		progress = 0;
		previousProgress = 0;
		
		targetPositions = new List<Vector3>();
		foreach(Transform movingObject in movingObjects)
			targetPositions.Add(movingObject.localPosition);
	}
	
	void LoadNewScene()
	{
		async.allowSceneActivation = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!loadingFinished)
		{
			if (async == null)
			{
				async = ApplicationState.Instance.LoadLevelMain();
				if (async != null)
					async.allowSceneActivation = false;
				else
				{
					ApplicationState.Instance.LoadNextLevel();
					loadingFinished = true;
				}
			}
			else
			{	
				progress = async.progress;//Application.GetStreamProgressForLevel(ApplicationState.Instance.loadingLevel);
				
				if (progress >= (0.9f-float.Epsilon))
				{					
					if (async.allowSceneActivation == false)
					{
						loadingFinished = true;
						Invoke ("LoadNewScene", 0f);
					}
				}
				
#if BASED_ON_LOAD
				fillingObject.fillAmount = Mathf.Lerp(fillingObject.fillAmount, 1.0f - progress, .5f);
				
				Vector3 movingDistnace = new Vector3 (distance * (progress - previousProgress), 0, 0);
				int index = 0;
				foreach(Transform movingObject in movingObjects)
				{
					targetPositions[index] += movingDistnace;					
					movingObject.transform.localPosition = Vector3.Lerp(movingObject.transform.localPosition, targetPositions[index++], .5f);
				}
				previousProgress = progress;
#else			
				//fillingObject.fillAmount = Mathf.Lerp(0, 1, fillingObject.fillAmount + fillTime * Time.deltaTime);
				//if (fillingObject.fillAmount > (1 - float.Epsilon))					
				//	fillingObject.fillAmount = 0;
#endif
			}
		}
		else
		{
#if BASED_ON_LOAD
			int index = 0;
			//fillingObject.fillAmount = Mathf.Lerp(fillingObject.fillAmount, 0, 1.0f);
			foreach(Transform movingObject in movingObjects)
			{
				movingObject.transform.localPosition = Vector3.Lerp(movingObject.transform.localPosition, targetPositions[index++], 1);
			}
#endif
		}
	}
}
