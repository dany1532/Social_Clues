using UnityEngine;
using System.Collections;

public class MinigameSelection : MonoBehaviour {
	float buttonDistance;
	public PositionAnimationFX anim;

	public GameObject nextButton;
	public GameObject prevButton;

	public int currentScreen = 1;
	private int numberOfScreens = 2;

	public Vector3 nextPos;
	
	// Use this for initialization
	void Start () {
		buttonDistance = 1024; //Screen.width;

		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).transform.localPosition = new Vector3(i * buttonDistance, 0, 0);
		}

		prevButton.SetActive(false);
		nextButton.SetActive(true);	
	}
	
	void ReenableButtonColliders()
	{
		nextButton.collider.enabled = true;
		prevButton.collider.enabled = true;
	}

	//Animation to the right
	void PlayNextAnimation()
	{
		if(currentScreen == numberOfScreens - 1) {
			nextButton.SetActive(false);
		}
		if(currentScreen < numberOfScreens) {
			nextButton.collider.enabled = false;
			prevButton.collider.enabled = false;
			currentScreen++;
			nextPos = this.transform.localPosition;
			nextPos.x -= buttonDistance;
			anim.InitializePositionLerp(this.transform.localPosition, nextPos, false);
			prevButton.SetActive(true);
			//currentLevelName.text = levelNames [currentScreen - 1];
			
			anim.PlayAnimation();
			Invoke("ReenableButtonColliders", anim.duration);
		}
	}
	
	//Animation to the left
	void PlayPrevAnimation()
	{
		if (currentScreen == 2) {
			prevButton.SetActive(false);
		}
		if (currentScreen > 1)
		{
			nextButton.collider.enabled = false;
			prevButton.collider.enabled = false;
			currentScreen--;
			nextPos = this.transform.localPosition;
			nextPos.x += buttonDistance;
			anim.InitializePositionLerp(this.transform.localPosition, nextPos, false);
			nextButton.SetActive(true);
			//currentLevelName.text = levelNames [currentScreen - 1];
			
			anim.PlayAnimation();
			Invoke("ReenableButtonColliders", anim.duration);
		}
	}

	public void DisplayNextScreen() {
		PlayNextAnimation();
	}

	public void DisplayPrevScreen() {
		PlayPrevAnimation();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
