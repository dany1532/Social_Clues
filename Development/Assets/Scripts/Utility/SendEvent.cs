using UnityEngine;
using System.Collections;

public class SendEvent : MonoBehaviour
{
    
    public enum Trigger
    {
        OnClick,
        OnPress,
        OnRelease,
    }

    Trigger trigger = Trigger.OnRelease;

    public GameObject eventReceiever;
    public string eventName;
    public SendMessageOptions messagesOptions = SendMessageOptions.DontRequireReceiver;
    public bool broadcast = false;

    void OnPress(bool isPressed)
    {
        if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
        {      
            GenerateEvent();
        }
    }
    
    void OnClick()
    {
        if (enabled && trigger == Trigger.OnClick)
        {
            GenerateEvent();
        }
    }

    void GenerateEvent()
    {
        if (broadcast)
            eventReceiever.BroadcastMessage(eventName, messagesOptions);
        else
            eventReceiever.SendMessage(eventName, messagesOptions);
    }
}
