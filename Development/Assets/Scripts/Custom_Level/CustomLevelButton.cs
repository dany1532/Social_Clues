using UnityEngine;
using System.Collections;

public class CustomLevelButton : MonoBehaviour
{
    public CustomLevel customLevel;
    public LevelSelection lvlSelect;
    public bool beingDragged = false;
    public LevelCreationInfo levelInfo;
    public UITexture levelTexture;
    public UIStretch levelStretch;
	public MinigameSelection minigameSelect;

	public enum BUTTON_TYPE { LEVEL, MINIGAME }
	public BUTTON_TYPE buttonType = BUTTON_TYPE.LEVEL;

    //Name Set up
    void Start()
    {
        name = this.gameObject.name;    
    }

    //Used to flip through the different levels
    void OnDrag(Vector2 delta)
    {
        //Execute drag event once, then ignore the rest until released
        if (!beingDragged)
        {

            beingDragged = true;

            if (delta.x < 0f)
            {
                switch(buttonType) {
					case BUTTON_TYPE.LEVEL:
						lvlSelect.DisplayNextLevel();
						break;
					case BUTTON_TYPE.MINIGAME:
						minigameSelect.DisplayNextScreen();
						break;
				}

                Invoke("SetNotDrag", 0.5f);
            } else
            {
				switch(buttonType) {
				case BUTTON_TYPE.LEVEL:
					lvlSelect.DisplayPrevLevel();
					break;
				case BUTTON_TYPE.MINIGAME:
					minigameSelect.DisplayPrevScreen();
					break;
				}
                
                Invoke("SetNotDrag", 0.5f);
            }

        }
    }

    public void SetNotDrag()
    {
        beingDragged = false;   
    }

    // Use this for initialization
    void OnPress(bool pressed)
    {
        if (!pressed && !beingDragged)
        {
            //When the return button is selected, return to the Level Select window
            if (name == "ReturnLevel_Button")
				customLevel.ReturnToLevelSelection();
			//When the Main menu is selected, load main menu scene
			else if (name == "ReturnMenu_Button")
				customLevel.ReturnToMainMenu();
            //When the Main menu is selected, load main menu scene
            else if (name == "MainMenu_Button")
                customLevel.ReturnToHomeMenu();
            //When the create button is selected, load the level with the desired characters
            else if (name == "CreateLevel_Button")
                customLevel.CreateLevel();
            else if (name == "Next_Button")
                lvlSelect.DisplayNextLevel();
            else if (name == "Prev_Button")
                lvlSelect.DisplayPrevLevel();
			else if (name == "Next_Screen")
				minigameSelect.DisplayNextScreen();
				//Debug.Log("next");
			else if (name == "Prev_Screen")
				minigameSelect.DisplayPrevScreen();
				//Debug.Log("prev");
            else if (name != "Background")
            {
                customLevel.SetupCharacterSelection(levelInfo);
            }
        }
    }
}
