using UnityEngine;
using System.Collections;

public class OnPauseButton : MonoBehaviour
{

    public string newLevelName;
	
    void OnPress(bool pressed)
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
    		
            GameManager.DestroyGameLevel();
			GameManager.DestroyInstance();
        }
        Application.LoadLevel(newLevelName);
        Time.timeScale = 1f;
    }
}
