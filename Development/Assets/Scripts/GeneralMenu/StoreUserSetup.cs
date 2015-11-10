using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class StoreUserSetup : MonoBehaviour
{
    public MenuController menuController;
	
    public UIInput UserName;
    public UILabel Age;
    public UILabel Gender;
    public bool toySelected;
    public UISprite okayButton;
	
    private SelectToy _selectedToy;
    public Color okayOriColor;
    public SelectToy selectedToy
    {
        get { return _selectedToy; }
        set
        { 
            if (_selectedToy != null)
                _selectedToy.Unselect();
			
            _selectedToy = value;

            //if (_selectedToy != null) toySelected = true;
            if (selectedToy != null && UserName.text != "Player" && UserName.text.Trim() != "" && !Regex.IsMatch(UserName.text, @"\A(?=[^0-9]*[0-9])(?=[^A-Za-z]*[A-Za-z])\w+\Z", RegexOptions.IgnorePatternWhitespace))
            {
                okayButton.color = okayOriColor;
                gameObject.collider.enabled = true;
            }
        }
    }
	
    public List<SelectToy> toys;
    public LoginMenu loginMenu;
	
    // Use this for initialization
    void Awake()
    {
        toySelected = false;
        okayOriColor = okayButton.color;
        okayButton.color = Color.gray;
        gameObject.collider.enabled = false;

        LoadToys();
    }
	
    void OnEnable()
    {
        Clear();
        LoadToys();
    }
	
    public void Clear()
    {
        UserName.text = UserName.defaultText;
        selectedToy = null;
        toySelected = false;
        //okayOriColor = okayButton.color;
        okayButton.color = Color.gray;
        gameObject.collider.enabled = false;
    }
	
    /// <summary>
    /// Load toys from database
    /// </summary>
    public void LoadToys()
    {
        MainDatabase.Instance.ConnectDB();
        List<DBToyInfo> allToys = MainDatabase.Instance.getToyInfo();
		
        // Set toys from database
        for (int i = 0; i < allToys.Count; i++)
        {
            if (i < toys.Count)
                toys [i].LoadToy(allToys [i].ToyID, allToys [i].Filename, new Color(allToys [i].ToyColorR / 255.0f, (float)allToys [i].ToyColorG / 255.0f, (float)allToys [i].ToyColorB / 255.0f), this);
        }
		
        // Fill remaining slots
        for (int i = allToys.Count; i < toys.Count; i++)
        {
            toys [i].LoadToy(-1, "", Color.white, this);
        }
		
        selectedToy = null;
    }
	
    /// <summary>
    /// Handle click event: save record and proceed to user menu
    /// </summary>
    void OnClick()
    {
        if ((UserName.text != "Player" && UserName.text.Trim() != "") && selectedToy != null && !Regex.IsMatch(UserName.text, @"\A(?=[^0-9]*[0-9])(?=[^A-Za-z]*[A-Za-z])\w+\Z", RegexOptions.IgnorePatternWhitespace))
        {	
            SaveUserData();
            loginMenu.LoadUsers();
            menuController.enableMenu(MenuButton.MenuType.UserCharacterSelection);
        }
    }

    /// <summary>
    /// Save user data in database
    /// </summary>
    void SaveUserData()
    {				
        if (selectedToy != null)
        {
            //default values when user get created
            string insertUserDataSQL = "INSERT INTO USER (Name,PicID,VolumeMusic,VolumeSFX,Age,Gender, Active, NPC1, NPC1Status, NPC2, NPC2Status, NPC3, NPC3Status, NPC4, NPC4Status," +
                "BackgroundTrack, DialogueProcessingTime, ExitConversationOption, InstantAnswer, ExitMinigameOption, MaxLevel)"
                + " VALUES('" + UserName.text + "','" + selectedToy.toyID + "','1','1','0','Pete','1','0','-1','0','-1','0','-1','0','-1','1', '0', '1', '1', '1', '1');";
            string results = MainDatabase.Instance.InsertSql(insertUserDataSQL);
            Debug.Log(results);
			
            int userID = MainDatabase.Instance.getIDs("Select MAX(UserID) from USER;");
            string insertToySQL = "INSERT INTO USERTOY (UserID , ToyID) VALUES('" + userID + "','" + selectedToy.toyID + "');";			
            results = MainDatabase.Instance.InsertSql(insertToySQL);
            MainDatabase.Instance.AddUserNPCList(userID, 0, -1, 0, -1, 0, -1, 0, -1);
            Debug.Log(results);
            ApplicationState.Instance.userID = userID;
			
            MainDatabase.Instance.DisconnectDB();
        }
    }
}