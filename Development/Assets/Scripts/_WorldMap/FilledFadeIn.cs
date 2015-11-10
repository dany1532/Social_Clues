using UnityEngine;
using System.Collections;

public class FilledFadeIn : MonoBehaviour {

	public UIFilledSprite sprite;
	public float fillTime = 1f;
	float fillAmount = 0;
	bool filling = false;

	// Use this for initialization
	void Start () {
		sprite.fillAmount = 0;
		fillAmount = 0;
	}
	
	// Update is called once per frame
	public void Appear () {
		sprite.fillAmount = 1;	
	}

	public void PlayFadeIn ()
	{
		filling = true;
	}

	public void Update()
	{
		if (filling) {
			fillAmount += Time.deltaTime;

			if (fillAmount >= fillTime)
			{
				sprite.fillAmount = 1;
				filling = false;
			}
			else
			{
				sprite.fillAmount = Mathf.Lerp(0, 1, fillAmount / fillTime);
			}
		}
	}
}
