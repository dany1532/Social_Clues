using UnityEngine;
using System.Collections;

public class AdventureMode_Button : MonoBehaviour
{
    public GameObject startGameSelect;
    public GameObject modeSelection;
    LoadGameManager loadGameManager;
	
	
    void OnPress(bool pressed)
    {
        if (!pressed)
        {
            if (name == "Story_Button")
            {
                startGameSelect.SetActive(true);
				
                if (loadGameManager.IsFirstTimeUser() || loadGameManager.IsLevelComplete())
                    startGameSelect.transform.FindChild("LoadGame_Button").GetComponent<AdventureMode_Button>().DisableButton();
				
                //SetBoxCollider(modeSelection, false);
            } else if (name == "BuildLevel_Button")
            {
                loadGameManager.LoadLevelSetUpScene();	
            } else if (name == "LoadGame_Button")
            {
                loadGameManager.LoadGame();
            } else if (name == "NewGame_Button")
            {
                loadGameManager.NewGame();	
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        loadGameManager = GameObject.Find("LoadGame_Manager").GetComponent<LoadGameManager>();

        if (name == "LoadGame_Button" && !loadGameManager.HasLeveToLoad())
        {
            DisableButton();
        }
    }
	
    public void DisableButton()
    {
        GetComponent<UIButtonScale>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<UISprite>().color = Color.gray;
    }
	
    public void EnableButton()
    {
        GetComponent<UIButtonScale>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        GetComponentInChildren<UISprite>().color = Color.white;
    }
	
    /*void SetBoxCollider(GameObject theParent, bool status){
		Component[] boxColliders;

        boxColliders = GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider boxCollider in boxColliders) {
            boxCollider.enabled = status;

    	}
	}*/
	
    // Update is called once per frame
    void Update()
    {
	
    }
}
