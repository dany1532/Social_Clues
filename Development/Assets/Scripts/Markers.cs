using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Markers manager
/// </summary>
public class Markers : Singleton<Markers> {
	// The markers the player wins
	public List<string> markers;
	// The markers the player wins
	public List<Color> colors;
	// The sprites that the markers will be shown at
	public List<UISprite> targetMarkers;
	// The sprites that the markers will be shown at
	public List<UIStretch> targetStretch;
	
	public UIAtlas missingAtlas;
	public UIAtlas foundAtlas;
	
	// The current marker
	int currentMarker = 0;
	
	// The animation to add a new marker
	public GameObject markerAnimation;
	
	private PositionAnimationFX markerPosAnim;
	private ScaleAnimationFX    markerScaleAnim;
	private UISprite			markerSpriteAnim;
	
	private Vector3 centerPosition;
	private Vector3 initialScale;
	private Vector3 slotPosition;
	private Vector3 centerScale;
	private Vector3 toyPosition;
	private Vector3 slotScale;
	
	private string toyName;
	private int animationIndex;
	
	public float initialDuration = 0.5f;
	public float finalDuration = 2.0f;
	
	public GameObject startAnimationPrefab;
	
	void Awake()
	{
		instance = this;
	}

	public void ReplaceMarker (int index, string spriteName, Color color)
	{
		markers[index] = spriteName;
		targetMarkers[index].atlas = missingAtlas;
		targetMarkers[index].spriteName = spriteName;
		targetStretch[index].initialSize = new Vector2(targetMarkers[index].mInner.width, targetMarkers[index].mInner.height);
		targetMarkers[index].color = Color.white;
		colors[index] = color;
	}

	public void HideMarker (int count)
	{
		targetMarkers[count].gameObject.SetActive(false);
	}
	
	// Use this for initialization
	void Start () {
		if (markerAnimation != null){
			//Get there required animation FX for animation
			markerPosAnim = markerAnimation.GetComponent<PositionAnimationFX>();
			markerScaleAnim = markerAnimation.GetComponent<ScaleAnimationFX>();
			
			//Get the marker's sprite (will change depending on character)
			markerSpriteAnim = markerAnimation.GetComponent<UISprite>();
			
			//Set initial state of the marker anim
			centerPosition = markerAnimation.transform.position;
			initialScale = 0.1f * markerAnimation.transform.localScale;
			centerScale = markerAnimation.transform.localScale;
			
			// Deactivate the game object with the animated marker (adding marker animation)
			markerAnimation.gameObject.SetActive(false);
		}
	}
	
	void PlayMarkerAnimation(string spriteName){
		//Set to initial state
		markerAnimation.transform.position = toyPosition;
		markerAnimation.transform.localScale = initialScale;
		markerSpriteAnim.pivot = UIWidget.Pivot.Center;
		markerSpriteAnim.spriteName = spriteName;
		markerSpriteAnim.color = colors[animationIndex];
		
		//Reset AnimationFX
		markerPosAnim.Reset();
		markerScaleAnim.Reset();
		
		//Set the correct marker sprite and destination pos and scale
		
		slotPosition = targetMarkers[animationIndex].gameObject.transform.position;
		slotPosition.z -= 1;
		slotScale = targetMarkers[animationIndex].gameObject.transform.localScale;
		
		//initialize animation values and set up end animation event
		markerPosAnim.InitializePositionLerp(toyPosition, centerPosition, false);
		markerPosAnim.duration = initialDuration;
		
		markerScaleAnim.IntializeScaleLerp(initialScale, centerScale);
		markerScaleAnim.duration = initialDuration;
		markerScaleAnim.animationCompleteDelegate = EndCenterAnimation;
		
		markerAnimation.gameObject.SetActive(true);
		
		//Play animations
		markerPosAnim.PlayAnimation();
		markerScaleAnim.PlayAnimation();
	}
		
	private void EndCenterAnimation(ScaleAnimationFX anim, string what){
		if(markerScaleAnim = anim){
			GameObject starParticle = GameObject.Instantiate(startAnimationPrefab) as GameObject;
			starParticle.transform.parent = markerAnimation.transform;
			starParticle.transform.localPosition = new Vector3(0, 0, 1);
			starParticle.transform.parent = null;
				
			Invoke("StartSlotAnimation", 1.0f);	
		}
	}
	
	private void StartSlotAnimation(){		
		markerPosAnim.Reset();
		markerSpriteAnim.pivot = UIWidget.Pivot.Bottom;
		markerPosAnim.InitializePositionLerp(markerSpriteAnim.transform.position, slotPosition, false);
		markerPosAnim.duration = finalDuration;
		markerPosAnim.animationCompleteDelegate = EndSlotAnimation;
		markerPosAnim.PlayAnimation();
		
		markerScaleAnim.Reset();
		markerScaleAnim.animationCompleteDelegate = null;
		markerScaleAnim.IntializeScaleLerp(centerScale, slotScale);
		markerScaleAnim.duration = finalDuration;
		markerScaleAnim.PlayAnimation();
	}
	
	private void EndSlotAnimation(PositionAnimationFX anim, string what){
		if(markerPosAnim == anim){
			//Reset AnimationFX
			markerPosAnim.animationCompleteDelegate = null;
			markerPosAnim.Reset();
			markerScaleAnim.Reset();
			
			targetMarkers[animationIndex].atlas = foundAtlas;
			targetMarkers[animationIndex].spriteName = Utilities.SetTextureName(toyName);
			targetStretch[animationIndex].initialSize = new Vector2(targetMarkers[animationIndex].mInner.width, targetMarkers[animationIndex].mInner.height);
			targetMarkers[animationIndex].color = colors[animationIndex];
			animationIndex = -1;
			toyName = "";
			markerAnimation.SetActive(false);
			
			bool completedAllToys = true;
			
			foreach (UISprite toy in targetMarkers)
			{
				if (toy.atlas == missingAtlas)
				{
					completedAllToys = false;
					break;
				}
			}
			
			if (completedAllToys)
				GameManager.Instance.CompleteLevel();
		}
	}
	
	//when loading a previous gameplay, if npc is completed display toy
	public void DisplayNPCToyCompleted(int npcIndex){
		animationIndex = npcIndex;
		
		if (animationIndex > -1 && animationIndex < targetMarkers.Count)
		{
			toyName = markers[npcIndex];
			targetMarkers[animationIndex].atlas = foundAtlas;
			targetMarkers[animationIndex].spriteName = Utilities.SetTextureName(toyName);
			targetStretch[animationIndex].initialSize = new Vector2(targetMarkers[animationIndex].mInner.width, targetMarkers[animationIndex].mInner.height);
			targetMarkers[animationIndex].color = colors[animationIndex];
			animationIndex = -1;
			toyName = "";
		}
		
	}
	
	/// <summary>
	/// Executed when the on quest completed event is triggered and adds a new marker
	/// </summary>
	public void OnQuestCompleted(Vector3 toyWorldPosition, int npcIndex)
	{
		animationIndex = npcIndex;
		
		if (animationIndex > -1 && animationIndex < targetMarkers.Count)
		{
			toyName = markers[npcIndex];
			markerSpriteAnim.spriteName = toyName;
			
			toyPosition = GameManager.Instance.mainCamera.WorldToViewportPoint(toyWorldPosition);
			toyPosition = GameManager.Instance.uiCamera.ViewportToWorldPoint(toyPosition);
			toyPosition.z = -1;
			markerAnimation.SetActive(true);
			
			//Play animation for the required marker
			PlayMarkerAnimation(toyName);
		}
	}
}