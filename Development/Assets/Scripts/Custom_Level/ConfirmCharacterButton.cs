using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfirmCharacterButton : MonoBehaviour
{
	public CharacterSelectionTable char_Table;
	
	public List<CharacterSelected_Button> availableSlots;
	public List<UILabel> scoreLabelsList;
	
	public UISprite currentCharacterSprite;
	public UISprite characterBubbleSprite;
	public UISprite descriptionBubbleSprite;
	public UISprite descriptionToySprite;
	public UIStretch descriptionToyStretch;

	public UILabel characterNameLabel;
	public UILabel characterLessonLabel;
	public UILabel characterStarLabel;
	public UILabel characterMinigameNameLabel;
	public UILabel characterMinigameDescriptionLabel;
	
	public GameObject createButton;
	public int occupiedSlots = 0;
	//string currentCharacterName;
	
	string characterBubbleSpriteName = "Character Bubble Bottom";
	string characterBubbleBlankSpriteName = "Character Card_Blank";
	string descriptionBubbleBlank = "Info Bubble";
	string descriptionCategoriesSprite = "Info Card_Blank with Categories";
	string descriptionCategoriesSpriteChild = "Toy Card Blank";
	
	LevelCharactersInfo.Character charInfo;
	
	//When the confirm button is selected, display it at the top as selected character
	void OnPress(bool pressed)
	{
		if (!pressed)
		{
			
			//give character info to one of the chosenCharacterSlots
			for (int index = 0; index < availableSlots.Count; index++)
			{
				if (!availableSlots [index].IsDisplayingCharacter())
				{
					availableSlots [index].SetSprite(charInfo);
					occupiedSlots++;
					break;
				}
			}		
			
			//Play button is set active, reset confirmed character slots, disable slot in table
			// and disable confirm button
			createButton.SetActive(true);
			currentCharacterSprite.enabled = false;
			
			characterBubbleSprite.spriteName = characterBubbleSpriteName;
			descriptionBubbleSprite.spriteName = descriptionBubbleBlank;
			descriptionToySprite.enabled = false;
			
			characterNameLabel.text = "";
			characterLessonLabel.text = "";
			characterLessonLabel.text = "";
			characterStarLabel.text = "";
			characterMinigameNameLabel.text = "";
			characterMinigameDescriptionLabel.text = "";
			for (int i = 0; i < scoreLabelsList.Count; i++)
				scoreLabelsList [i].text = "";
			
			char_Table.Disable_Slot(charInfo.npcName);
			
			//this.gameObject.SetActive(false);
			setInactiveWithButtonSound();
		}
	}
	
	//Slot has been selected, display character and corresponding info (from server)
	public void Set_CurrentCharacter(LevelCharactersInfo.Character info)
	{
		charInfo = info;
		currentCharacterSprite.enabled = true;
		currentCharacterSprite.spriteName = charInfo.npcTexture;
		characterNameLabel.text = charInfo.npcName;
		UIStretch stretch = currentCharacterSprite.gameObject.GetComponent<UIStretch>();
		stretch.initialSize.Set(currentCharacterSprite.GetAtlasSprite().inner.width, 
		                        currentCharacterSprite.GetAtlasSprite().inner.height);
		
		characterBubbleSprite.spriteName = characterBubbleBlankSpriteName;
		descriptionBubbleSprite.spriteName = descriptionCategoriesSprite;
		
		
		#if !UNITY_WEBPLAYER
		if (charInfo.npcId != -1)
		{
			if (ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.MAIN_MENU)
			{
				DBNPCMiniGameInfo npcInfo = MainDatabase.Instance.getNPCInfo(charInfo.npcId);
				characterLessonLabel.text = MainDatabase.Instance.getCategory(charInfo.npcId);
				characterStarLabel.text = npcInfo.Stars.ToString();
				characterMinigameNameLabel.text = npcInfo.MiniGame;
				characterMinigameDescriptionLabel.text = npcInfo.MiniGameDescription;
				List<float> scorePercentages = MainDatabase.Instance.calTotalPercentage(charInfo.npcId, ApplicationState.Instance.userID);
				
				if (scorePercentages != null)
				{
					for (int i = 0; i < scoreLabelsList.Count; i++)
					{
						
						if (float.IsNaN(scorePercentages [i]))
						{
							scoreLabelsList [i].text = "--";
							scoreLabelsList [i].color = Color.black;
						} else	
							scoreLabelsList [i].text = Mathf.Round(scorePercentages [i]).ToString() + "%";
						
						if (scorePercentages [i] < 50)
							scoreLabelsList [i].color = Color.red;
						else if (scorePercentages [i] < 70)
							scoreLabelsList [i].color = new Color(255, 193, 0);
						else if (scorePercentages [i] <= 100f)
							scoreLabelsList [i].color = Color.green;
						else
						{
							scoreLabelsList [i].color = Color.black;
						}
					}
				} else
				{
					Debug.LogWarning("No aggregated data found");
					for (int i = 0; i < scoreLabelsList.Count; i++)
					{
						scoreLabelsList [i].text = "--";
						scoreLabelsList [i].color = Color.black;
					}
				}
			}
			else
			{
				descriptionBubbleSprite.spriteName = descriptionCategoriesSpriteChild;
				descriptionToySprite.spriteName = charInfo.toyInfo.Filename;
				descriptionToyStretch.initialSize = new Vector2 (descriptionToySprite.mInner.width, descriptionToySprite.mInner.height);

				float r = (float) charInfo.toyInfo.ToyColorR/255;
				float g = (float) charInfo.toyInfo.ToyColorG/255;
				float b = (float) charInfo.toyInfo.ToyColorB/255;
			
				Color col = new Color(r, g, b, 1);
				descriptionToySprite.color = col;
				descriptionToySprite.enabled = true;
				
			}
		}
		#endif
		
		//red: 0 - 49
		//yellow 50 - 74
		//green 75 - 100
		
		if (occupiedSlots < availableSlots.Count)
		{
			gameObject.SetActive(true);
			setActiveWithButtonSound();
		}
	}
	
	
	//Used to reset everything when returning to level selection
	public void Default()
	{
		currentCharacterSprite.enabled = false;
		
		characterBubbleSprite.spriteName = characterBubbleSpriteName;
		descriptionBubbleSprite.spriteName = descriptionBubbleBlank;
		descriptionToySprite.enabled = false;
		
		characterNameLabel.text = "";
		characterStarLabel.text = "";
		characterMinigameNameLabel.text = "";
		characterMinigameDescriptionLabel.text = "";
		characterLessonLabel.text = "";
		for (int i = 0; i < scoreLabelsList.Count; i++)
			scoreLabelsList [i].text = "";
		
		occupiedSlots = 0;
		
		for (int index = 0; index < availableSlots.Count; index++)
		{
			availableSlots [index].Default();
		}
		
		createButton.SetActive(false);
		//gameObject.SetActive(false);
		setInactiveWithButtonSound();
	}
	
	
	// Use this for initialization
	void Start()
	{
		characterBubbleSprite.spriteName = characterBubbleSpriteName;
		descriptionBubbleSprite.spriteName = descriptionBubbleBlank;
		descriptionToySprite.enabled = false;
		descriptionToyStretch = descriptionToySprite.gameObject.GetComponent<UIStretch> ();
		characterNameLabel.text = "";
		characterStarLabel.text = "";
		characterMinigameNameLabel.text = "";
		characterMinigameDescriptionLabel.text = "";
		characterLessonLabel.text = "";
		for (int i = 0; i < scoreLabelsList.Count; i++)
			scoreLabelsList [i].text = "";
		createButton.SetActive(false);
		//this.gameObject.SetActive(false);
		setInactiveWithButtonSound();
	}
	
	public bool HasAvailableSlots()
	{
		return occupiedSlots < availableSlots.Count;
	}
	
	public void setInactiveWithButtonSound() {
		foreach(Transform t in transform) {
			t.gameObject.SetActive(false);
		}
		gameObject.collider.enabled = false;
	}

	public void setActiveWithButtonSound() {
		foreach(Transform t in transform) {
			t.gameObject.SetActive(true);
		}
		gameObject.collider.enabled = true;
	}
}
