using UnityEngine;
using System.Collections;

public class OnPauseUI : MonoBehaviour
{
    public PauseExitButton exitButton;
    public GameObject analyticsButton;
    public string dataAnalyticsScene = "PlayAnalytics";
    public string childsMenuScene = "ChildMenu";
    public string mainMenuScene = "Menu";

    // Use this for initialization
    void Start()
    {
        if (ApplicationState.Instance.InLevel() && NPCs.Instance.hasCompletedNPCs())
        {
            analyticsButton.SetActive(true);
        } else
            analyticsButton.SetActive(false);
    }

    public void OpenDataAnalytics()
    {
        ApplicationState.Instance.Pause();
        LoadScene(ApplicationState.LevelNames.PLAY_ANALYTICS);
    }

    public void GoHome()
    {
        ApplicationState.Instance.Pause();

        if (Player.instance != null)
            LoadScene(ApplicationState.LevelNames.CHILDS_MENU);
        else if (FindObjectOfType(typeof(AnalyticsController)) != null)
            ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.CHILDS_MENU);
        else if (FindObjectOfType(typeof(Minigame)) != null)
            ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.CHILDS_MENU, MenuButton.MenuType.Minigames);
        else
            ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.CHILDS_MENU, MenuButton.MenuType.ToyBox);
    }

    public void LoadScene(ApplicationState.LevelNames sceneName)
    {
        if (Player.instance != null)
        {
            #if !UNITY_WEBPLAYER
            int incompleteInteraction = MainDatabase.Instance.getIDs("select ifnull(max(interactionId),-1) from interaction where completed <> 'True';");
            if (incompleteInteraction != -1)
            {
                MainDatabase.Instance.UpdateSql("Delete from Answer where interactionID = " + incompleteInteraction + ";");
                MainDatabase.Instance.UpdateSql("Delete from InteractionTime where interactionID = " + incompleteInteraction + ";");
                MainDatabase.Instance.UpdateSql("Delete from Interaction where interactionID = " + incompleteInteraction + ";");
            }
            #endif
            AudioManager.Instance.StopSoundFX();
            AudioManager.Instance.StopVoiceOver();
            GameManager.DestroyGameLevel();
            GameManager.DestroyInstance();
        }

        ApplicationState.Instance.LoadLevelWithLoading(sceneName);
    }
}
