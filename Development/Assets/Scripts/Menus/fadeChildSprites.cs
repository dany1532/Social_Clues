using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class fadeChildSprites : MonoBehaviour {


	//Component[] childColor;
	//public float time = .2f; 
	public float fadeSpeedIn = 2f; 
	public float fadeSpeedOut = 2f; 
	float spriteAlpha; //AnimatedColor.color = Color.white;
	float tempAlpha; 
	int i = 0; 
	int j = 0; 
	int k = 0;
	int m = 0; 

	bool fadeInTime; 
	bool fadeOutTime;
	GameObject[] childsG;
	List<float> alphaList =  new List<float>();

	// Use this for initialization
	void Start () {
		fadeInTime = false; 
		fadeOutTime = false; 
		childsG = new GameObject[transform.childCount];


		foreach(Transform child in transform)
		{
			childsG[i] = child.gameObject;
			i++;
		}
		
		foreach (Transform child in transform)
		{
			spriteAlpha = childsG[j].GetComponent<AnimatedAlpha>().alpha; 
			alphaList.Add(spriteAlpha);
			j++; 
		}


		//StartCoroutine("fadeIn");
		//alphaList.Add(gameObject.GetComponentsInChildren(AnimatedAlpha().alpha)); 
		//spriteAlpha = gameObject.AnimatedAlpha.alpha; 

		//childColor[] = GetComponentsInChildren (AnimatedColor.color);
	}
	
	// Update is called once per frame
	void Update () {
		//fadeIn(); 
		foreach (Transform child in transform)
		{
			childsG[m].GetComponent<AnimatedAlpha>().alpha = alphaList[m];
			m++; 
		}
		m=0;

		if (fadeInTime)
		{
			for(int i = 0; i < alphaList.Count; i++)
			{
			//if (alphaList[k] == 0f)
			//{
				if (alphaList.Count != 0 && alphaList[k] <= 1)
				{
					alphaList[k] = Mathf.Lerp(alphaList[k],1,Time.deltaTime * fadeSpeedIn);
					//tempAlpha = alphaList[k]; 
					//}
					//else
					//{
					//alphaList[k] = Mathf.Lerp(alphaList[k],0,Time.deltaTime * fadeSpeed);
					//}
					k++;
				}
			
			}
			k=0;
		}
		else if (fadeOutTime)
		{
			for(int i = 0; i < alphaList.Count; i++)
			{
				//if (alphaList[k] == 0f)
				//{
				if (alphaList.Count != 0 && alphaList[k] >= 0)
				{
					alphaList[k] = Mathf.Lerp(alphaList[k],0,Time.deltaTime * fadeSpeedOut);
					//tempAlpha = alphaList[k]; 
					//}
					//else
					//{
					//alphaList[k] = Mathf.Lerp(alphaList[k],0,Time.deltaTime * fadeSpeed);
					//}
					k++;
				}
				
			}
			k=0;
		}
	}

	public void fadeIn()
	{
		Debug.Log ("fade In"); 
		//yield return new WaitForSeconds(time);
		fadeInTime = true; 
		fadeOutTime = false; 
		/*if (tempAlpha != 1f)
		{
			fadeIn();
		}*/
	
	}

	public void fadeOut()
	{
		Debug.Log ("fade Out"); 
		//yield return new WaitForSeconds(time);
		fadeOutTime = true; 
		fadeInTime = false; 
		/*if (tempAlpha != 1f)
		{
			fadeIn();
		}*/
		
	}
}
