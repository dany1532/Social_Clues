using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager>
{
    bool receivedClickUp;
	
    bool receivedUIInput;
    bool receivedUIInput_prev;
	
    bool isDragging = false;
	
    // Use this for initialization
    void Awake()
    {
        instance = this;
        receivedUIInput = false;
    }
	
    void Update()
    {
        // Check if the user has clicked somewhere
        if (Input.GetMouseButtonUp(0))
        {
            receivedClickUp = true;
            isDragging = false;
        } else if (Input.GetMouseButtonDown(0))
        {
            receivedClickUp = false;
            isDragging = true;
        } else
            receivedClickUp = false;
    }
	
    void LateUpdate()
    {
        if (!isDragging)
        {
            receivedUIInput_prev = receivedUIInput;
            receivedUIInput = false;
        }
    }
	
    /// <summary>
    /// Determines whether we have received a click event, not on the UI elements
    /// </summary>
    /// <returns>
    /// <c>true</c> if we received a non UI element click event; otherwise, <c>false</c>.
    /// </returns>
    public bool HasReceivedClick()
    {
        return receivedClickUp && !receivedUIInput && !receivedUIInput_prev;
    }
	
    /// <summary>
    /// Determines whether we have received a drag event, not on the UI elements
    /// </summary>
    /// <returns>
    /// <c>true</c> if we received a non UI element drag event; otherwise, <c>false</c>.
    /// </returns>
    public bool HasReceivedDrag()
    {
        return isDragging && !receivedUIInput && !receivedUIInput_prev;
    }
	
    /// <summary>
    /// Note that we have received a UI element input
    /// </summary>
    public void ReceivedUIInput()
    {
        receivedUIInput = true;	
    }
	
    /// <summary>
    /// Determines whether we have received a UI element input
    /// </summary>
    /// <returns>
    /// <c>true</c> if we received UI element input; otherwise, <c>false</c>.
    /// </returns>
    public bool HasReceivedUIInput()
    {
        return receivedUIInput || receivedUIInput_prev;
    }
}
