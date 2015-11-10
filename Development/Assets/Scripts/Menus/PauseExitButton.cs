using UnityEngine;
using System.Collections;

public class PauseExitButton : MonoBehaviour
{
	
    // Use this for initialization
    void Start()
    {		
        if (GameManager.WasInitialized())
        {
			if (GameManager.Instance.InConversation() && GameManager.Instance.InMinigame() && UserSettings.Instance.exitMinigameOption)
            {
                this.gameObject.SetActive(true);
            } else if (GameManager.Instance.InConversation() && UserSettings.Instance.exitConversationOption)
            {
                this.gameObject.SetActive(true);
            } else
            {
                this.gameObject.SetActive(false);
            }
        } else
        {
            this.gameObject.SetActive(false);
        }
    }

	public void SetVisible (bool value)
	{
		this.gameObject.SetActive (value);
	}

    void OnClick()
    {
        InputManager.Instance.ReceivedUIInput();

        if (GameManager.WasInitialized())
        {
            if (GameManager.Instance.InMinigame())
            {
                GameManager.Instance.minigame.CompleteMinigame();
            } else if (GameManager.Instance.InConversation())
            {

                DialogueWindow.instance.ShowWindow(null);
                AudioManager.Instance.StopVoiceOver();
                Sherlock.Instance.SetText(string.Empty);
				if (Player.instance.interactingNPC.cutScene.gameObject != null)
					Destroy(Player.instance.interactingNPC.cutScene.gameObject);
                Player.instance.interactingNPC.ResetState();
                Player.instance.ResetState();
            }
        }
        this.gameObject.SetActive(false);

        PauseUI pauseUI = GameObject.FindObjectOfType(typeof(PauseUI)) as PauseUI;
        pauseUI.Pause();
    }
}
