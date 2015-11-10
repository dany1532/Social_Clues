using UnityEngine;
using System.Collections;

public class AchievementsManager : MonoBehaviour {

	//public GameObject maxPolaroid;
	public GameObject max;
	//public Vector3 polaroid1; 
	public GameObject polaroids; 
	public GameObject characters; 
	public GameObject backButton; 
	public GameObject trainsDisplay; 
	public GameObject artPadDisplay; 
	public GameObject maxDisplay;
	public GameObject maxInnerDisplay; 
	public GameObject eddieDisplay;
	public GameObject eddieInnerDisplay; 
	public GameObject eddieArrows; 
	public GameObject maxArrows; 

	fadeChildSprites eddieInnerFade; 
	fadeChildSprites eddieOuterFade; 

	//Animator myAnimator; 
	//public Animation fadeInAnimation;
	int polaroidNum; 
	public float moveIntervalIn;
	public float moveIntervalOut;
	public bool picClicked; 

	// Use this for initialization
	void Start () {
		backButton.SetActive(false); 
		trainsDisplay.SetActive(false); 
		artPadDisplay.SetActive(false); 
		maxDisplay.SetActive(true); 
		maxInnerDisplay.SetActive(false);
		maxArrows.SetActive(false); 
		eddieArrows.SetActive(false); 
		//eddieDisplay.SetActive(true); 
		picClicked = false; 
		//eddieInnerDisplay.SetActive(false);
		polaroidNum = 0; 
		eddieInnerFade = eddieInnerDisplay.GetComponent<fadeChildSprites>(); 
		eddieOuterFade = eddieDisplay.GetComponent<fadeChildSprites>(); 
		//myAnimator = this.gameObject.GetComponent<Animator>(); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//need to write a switch statement. based on which polaroid was clicked, that will be the polaroid that scales up and centers.
	//everything else will move with that polaroid
	

	public void pictureClicked(string nameofPolaroid)
	{
		if (nameofPolaroid != "Trains" && nameofPolaroid != "ArtPad")
		{
		picClicked = true; 
		TweenScale.Begin(polaroids, moveIntervalIn, new Vector3(6, 6, 1));
		if (nameofPolaroid == "Polaroid1" || nameofPolaroid == "eddieArrowLeft")
		{
				Debug.Log("Max Clicked"); 
				TweenPosition.Begin(polaroids, moveIntervalIn, new Vector3(1650, -1350, 0));
				polaroidNum = 1; 
				StartCoroutine("waitForZoom");
		}
			else if (nameofPolaroid == "Polaroid2" || nameofPolaroid == "maxArrowRight")
		{
			Debug.Log("Eddie Clicked"); 
			TweenPosition.Begin(polaroids, moveIntervalIn, new Vector3(850, -1350, 0));
			polaroidNum = 2; 
			StartCoroutine("waitForZoom");
		}
			else if (nameofPolaroid == "Polaroid3" || nameofPolaroid == "eddieArrowRight")
		{
			Debug.Log("Henry Clicked"); 
			TweenPosition.Begin(polaroids, moveIntervalIn, new Vector3(50, -1350, 0));
		}
		else if (nameofPolaroid == "Polaroid4")
		{
			Debug.Log("Amy Clicked"); 
			TweenPosition.Begin(polaroids, moveIntervalIn, new Vector3(-750, -1350, 0));
		}
		else if (nameofPolaroid == "Polaroid5")
		{
			Debug.Log("Jake Clicked"); 
			TweenPosition.Begin(polaroids, moveIntervalIn, new Vector3(-1550, -1350, 0));
		}
		else if (nameofPolaroid == "Polaroid6")
		{
			Debug.Log("Nancy Clicked"); 
			TweenPosition.Begin(polaroids, moveIntervalIn, new Vector3(1650, -500, 0));
		}
		}
		else
		{
		if (nameofPolaroid == "Trains")
		{
			Debug.Log("Trains Clicked"); 
			trainsDisplay.SetActive(true);
			//Vector3 trainPos = trainsDisplay.gameObject.transform.position;
			//trainPos.z = -10.0f; 
			//Debug.Log ("trainPos: " + trainPos);
			
			
		}
		else if (nameofPolaroid == "ArtPad")
		{
			Debug.Log("Art Pad Clicked"); 
			artPadDisplay.SetActive(true);
			//Vector3 artPos = artPadDisplay.gameObject.transform.position;
			//artPos.z = -10.0f;
		}
		}
		backButton.SetActive(true); 
		/*TweenScale.Begin(maxPolaroid, moveInterval, new Vector3(500, 500, 1));
		TweenPosition.Begin(maxPolaroid, moveInterval, new Vector3(0, 0, 0));
		TweenScale.Begin(max, moveInterval, new Vector3(500, 500, 1));
		TweenPosition.Begin(max, moveInterval, new Vector3(0, 0, 0));
		*/
	}

	public void callZoom()
	{
		StartCoroutine("waitForZoom"); 
	}

	IEnumerator waitForZoom()
	{
		Debug.Log ("waiting"); 

		if (picClicked)
		{
			yield return new WaitForSeconds(moveIntervalIn);
			maxDisplay.SetActive(false); 
			maxInnerDisplay.SetActive(true);
			eddieInnerFade.fadeIn(); 
			eddieOuterFade.fadeOut(); 
		switch (polaroidNum)
		{
		case 1:
				maxArrows.SetActive(true); 
				eddieArrows.SetActive(false); 
			//maxDisplay.SetActive(false); 
			//maxInnerDisplay.SetActive(true);
			break; 
		case 2:
				eddieArrows.SetActive(true); 
				maxArrows.SetActive(false); 
			//eddieInnerFade.fadeIn(); 
			//eddieOuterFade.fadeOut(); 
			//myAnimator.SetTrigger("fade");
			//fadeInAnimation.Play();
			break; 
		}
			picClicked = false;
		}
		else
		{
			trainsDisplay.SetActive(false);
			artPadDisplay.SetActive(false);
			maxArrows.SetActive(false); 
			eddieArrows.SetActive(false); 
			yield return new WaitForSeconds(moveIntervalOut);
			//backToFullScreen(); 
			backButton.SetActive(false);
			TweenScale.Begin(polaroids, moveIntervalOut, new Vector3(1, 1, 1));
			TweenPosition.Begin(polaroids, moveIntervalOut, new Vector3(0, 0, 0));
			maxDisplay.SetActive(true); 
			maxInnerDisplay.SetActive(false);
			//eddieDisplay.SetActive(true);
			 
		}
	}

	public void backToFullScreen()
	{	
		eddieInnerFade.fadeOut();
		eddieOuterFade.fadeIn();
		StartCoroutine("waitForZoom"); 


		//eddieInnerDisplay.SetActive(false);
		//myAnimator.SetBool("Fire",false);

	}
}
