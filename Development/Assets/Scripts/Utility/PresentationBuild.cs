using UnityEngine;
using System.Collections;

public class PresentationBuild : MonoBehaviour
{
    public ShowClick showClick;
    
    void Awake()
    {
        showClick = GameObject.FindObjectOfType(typeof(ShowClick)) as ShowClick;
        if (showClick == null)
            enabled = false;
    }
    
    void OnPress(bool pressed)
    {
        if (pressed && this.gameObject.activeSelf)
        {
            ApplicationState.Instance.presentationBuild = !ApplicationState.Instance.presentationBuild;
            showClick.enabled = ApplicationState.Instance.presentationBuild;
        }
    }
}
