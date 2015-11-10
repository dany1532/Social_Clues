using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Dialogue progress system, that rewards player with stars
/// </summary>
public class DialogueProgress : MonoBehaviour {
	
	// Rewarding star animation
	//public Animation animatedStar;
	
	public StarAnimationFX animatedStar;
	
	// Textures for grey and gold star
	public Texture greyStar;
	public Texture goldStar;
	public GameObject starParticlesPrefab;
	
	public UIStretch starsBackground;
	
	// Sprite for each star for the conversation
	public List<UISprite> parts;
	public List<RotationAnimationFX> partsAnim;
	// Current part of the conversation
	public int currentStar = 0;
	
	float INITIALSCALE = 0.34f;
	float OFFSET = 0.05F;
	int MAXSTARS = 5;
	int starIndex = 0; //for the allStarsReward...
	
	public void Start()
	{
		animatedStar.gameObject.SetActive(false);
	}
	
	/// <summary>
	/// Clears all the stars to their grey texture, and start from the beginning of the progress
	/// </summary>
	public void ClearStars(int visibleStars)
	{
		int counter = 0;
		int differenceStars = 0;
		currentStar = 0;
		
		starsBackground.relativeSize.x = INITIALSCALE;
		
		// Update all 
		for (; counter < Mathf.Min(visibleStars, parts.Count) ; counter++)
		{
			parts[counter].gameObject.SetActive(true);
			parts[counter].spriteName = Utilities.SetTextureName(greyStar.name);
		}
		
		for ( ; counter < parts.Count ; counter++)
		{
			parts[counter].gameObject.SetActive(false);
		}
		
		differenceStars = MAXSTARS - visibleStars;
		starsBackground.relativeSize.x = starsBackground.relativeSize.x - (differenceStars * OFFSET);
		
		
	}
	
	/// <summary>
	/// Add a new star, and progress through dialogue
	/// </summary>
	public void AddStar()
	{
		// If there are more stars to be rewarded
		if (currentStar < parts.Count)
			ShowStar();
			//StartCoroutine(ShowStar());
		else
			ClearStars(parts.Count);
	}
	
	//Gained a star: play animation and go to proper place
	public void ShowStar(){
		animatedStar.transform.localEulerAngles = new Vector3(0,0,180);
		animatedStar.gameObject.SetActive(true);
		animatedStar.starCompleteDelegate = AnimationEnded;
		animatedStar.SetFinalPos(parts[currentStar].gameObject.transform.localPosition);
		animatedStar.PlayAnimation();
		
	}
	
	//Star animation ended display static star on proper place
	public void AnimationEnded(StarAnimationFX anim){
		if(animatedStar == anim){
			// Update next star to gold texture
			parts[currentStar].spriteName = Utilities.SetTextureName(goldStar.name);
			currentStar++;	
	
			// and deactivate its gameobject
			animatedStar.gameObject.SetActive(false);
			
			if(currentStar == parts.Count){
				InvokeRepeating("AllStarsReward", 0.3f, 0.8f);	
			}
		}
	}
	
    public void PlayStarParticle(Vector3 position)
    {
        GameObject starParticle = GameObject.Instantiate(starParticlesPrefab) as GameObject;
        starParticle.transform.position = position;
    }
    
	void AllStarsReward(){
		GameObject starParticle = GameObject.Instantiate(starParticlesPrefab) as GameObject;
		Vector3 newPos = parts[starIndex].transform.position;
		partsAnim[starIndex].PlayAnimation();
		
		newPos.z -= 0.2f;
		starParticle.transform.position = newPos;
		starIndex++;
		
		if(starIndex == parts.Count){
			CancelInvoke("AllStarsReward");
			starIndex = 0;
		}
		
		
	}
	
//	/// <summary>
//	/// Reward the player with a star
//	/// </summary>
//	public IEnumerator ShowStar()
//	{
//		// Activated the animation of the rewarding star
//		animatedStar.gameObject.SetActive(true);
//		// and play animation
//		animatedStar.Play();
//		
//		// wait a bit into the animation
//		yield return new WaitForSeconds(0.5f);
//		
//		// Update next star to gold texture
//		parts[currentStar].spriteName = Utilities.SetTextureName(goldStar.name);
//		currentStar++;
//		
//		// Stop the animation of the rewarding star
//		animatedStar.Stop();
//		// and deactivate its gameobject
//		animatedStar.gameObject.SetActive(false);
//	}
}
