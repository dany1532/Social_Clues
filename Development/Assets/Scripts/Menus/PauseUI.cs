using UnityEngine;
using System.Collections;

public class PauseUI : MonoBehaviour
{
	
    public Texture pause;
    public Texture play;
    public GameObject pauseMenu;

    UISprite pauseUI;
	
    // Use this for initialization
    void Start()
    {
        pauseUI = GetComponentInChildren<UISprite>();
		
        if (pauseUI == null)
            gameObject.SetActive(false);
        else
            UpdateUIIcon();
    }
	
    /// <summary>
    /// Raises the press event.
    /// </summary>
    /// <param name='pressed'>
    /// If the ui was pressed or released
    /// </param>
    void OnPress(bool pressed)
    {
        if (enabled && NGUITools.GetActive(gameObject) && gameObject != null)
        {
            if (pressed)
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        ApplicationState.Instance.Pause();

        UpdateUIIcon();

        if (InputManager.WasInitialized())
            InputManager.Instance.ReceivedUIInput();
    }

    /// <summary>
    /// Updates the UI icon for audio (mute or not)
    /// </summary>
    void UpdateUIIcon()
    {
        if (ApplicationState.Instance.isPaused())
        {
            pauseUI.spriteName = Utilities.SetTextureName(play.name);
            Transform t = this.transform.parent;			
            GameObject pauseMenuO = Instantiate(pauseMenu) as GameObject;
            pauseMenuO.name = "OnPauseUI";
            pauseMenuO.transform.parent = t;
            pauseMenuO.transform.localPosition = new Vector3(0, 0, 1);
            pauseMenuO.transform.localRotation = Quaternion.identity;
            pauseMenuO.transform.localScale = Vector3.one;
        } else
        {	
            pauseUI.spriteName = Utilities.SetTextureName(pause.name);
            Destroy(GameObject.Find("OnPauseUI"));
        }
    }
}
