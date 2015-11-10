using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelection : MonoBehaviour
{
    public Vector3 nextPos;
    public PositionAnimationFX anim;
    public UILabel currentLevelLabel;
    public UILabel currentLevelName;
    public List<string> levelNames;
    public GameObject nextButton;
    public GameObject prevButton;
    public int currentLevel = 1;
    float buttonDistance;

    public GameObject levelButton;
	public bool animationPlaying = false;

    // Use this for initialization
    void Start()
    {
		buttonDistance = 1024;//Screen.width;

        prevButton.SetActive(false);
        nextButton.SetActive(true);
	
        List<DBLevel> dbLevels = MainDatabase.Instance.GetLevelsInfo();
		levelNames.Clear();
        Transform currentTransform = transform;
        CustomLevel customLevel = GameObject.FindObjectOfType(typeof(CustomLevel)) as CustomLevel;
		int i = 1;
        foreach (DBLevel level in dbLevels)
        {
            LevelCreationInfo levelInfo = new LevelCreationInfo();
            levelInfo.levelId = level.levelId;
            levelInfo.levelIdx = level.levelIdx;
            levelInfo.levelName = level.levelName;
            levelNames.Add(level.levelName);
            levelInfo.levelScreenshot = ResourceManager.LoadTexture("Levels/" + level.levelTexture);
	
            GameObject newLevelButton = Instantiate(levelButton, currentTransform.position, Quaternion.identity) as GameObject;
            newLevelButton.name = levelInfo.levelName;
            newLevelButton.transform.parent = transform;
            newLevelButton.transform.localPosition = new Vector3(buttonDistance * (i - 1), 0, 0);
            CustomLevelButton button = newLevelButton.gameObject.GetComponent(typeof(CustomLevelButton)) as CustomLevelButton;
            button.levelInfo = levelInfo;
            button.levelTexture.mainTexture = levelInfo.levelScreenshot;
            button.levelStretch.initialSize = new Vector2(levelInfo.levelScreenshot.width, levelInfo.levelScreenshot.height);
            button.customLevel = customLevel;
            button.lvlSelect = this;
            button.GetComponent<UIButtonScale>().SendMessage("OnHover", false);
			i++;
        }
        UpdateLevelLabel();
	}
	
	void ReenableButtonColliders()
	{
		nextButton.collider.enabled = true;
		prevButton.collider.enabled = true;
	}
	
    //Animation to the right
    void PlayNextAnimation()
    {
        if (currentLevel < levelNames.Count)
		{
			nextButton.collider.enabled = false;
			prevButton.collider.enabled = false;

            currentLevel++;
            nextPos = this.transform.localPosition;
            nextPos.x -= buttonDistance;
            anim.InitializePositionLerp(this.transform.localPosition, nextPos, false);
            prevButton.SetActive(true);
            currentLevelName.text = levelNames [currentLevel - 1];
			
            anim.PlayAnimation();
			Invoke("ReenableButtonColliders", anim.duration);
        }

        if (currentLevel == levelNames.Count)
            nextButton.SetActive(false);
    }

    //Animation to the left
    void PlayPrevAnimation()
    {
        if (currentLevel > 1)
		{
			nextButton.collider.enabled = false;
			prevButton.collider.enabled = false;

            currentLevel --;
            nextPos = this.transform.localPosition;
            nextPos.x += buttonDistance;
            anim.InitializePositionLerp(this.transform.localPosition, nextPos, false);
            nextButton.SetActive(true);
            currentLevelName.text = levelNames [currentLevel - 1];
			
			anim.PlayAnimation();
			Invoke("ReenableButtonColliders", anim.duration);
        }
        if (currentLevel == 1)
            prevButton.SetActive(false);
    }
	
    //Displays the next level (anim to the right)
    public void DisplayNextLevel()
    {
        PlayNextAnimation();
        UpdateLevelLabel();
    }
	
    //Displays the prev level (anim to the left)
    public void DisplayPrevLevel()
    {
        PlayPrevAnimation();
        UpdateLevelLabel();
    }
	
    void UpdateLevelLabel()
    {
        if (levelNames.Count > 0 && currentLevel > 0)
            currentLevelName.text = levelNames [currentLevel-1];
        
        currentLevelLabel.text = currentLevel.ToString() + "/" + levelNames.Count.ToString();
    }
	
}
