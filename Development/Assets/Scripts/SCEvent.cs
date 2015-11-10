using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary> 
/// Sociale Clues Event class:
/// Information for each event that can be triggered in the game
/// Holds the event type, location, related NPC and dialogue, etc
/// </summary>
public class SCEvent : MonoBehaviour {
	
	/// <summary>
	/// Description / Type of the event
	/// (Add unique values for each event triggered in the game. Once a value has been set should not be changed)
	/// </summary>
	public enum SCEventType
	{
		None = 0,
		OnQuestGiven = 1,
		OnQuestCompleted = 2,
		LoadMinigame = 3,
		CompleteMinigame = 4,
		PlayCutScene = 5,
		CutSceneReturn = 6,
		CutSceneReplay = 7,
		ComprehensionEvent = 8,
		EmotionEvent = 16,
		ConversationStartEvent = 32,
		ProblemSolvingFailure = 128,
		WaitingEvent = 129,
		PreDialogueMiniGameStart = 256,
		PreDialogueMiniGameReturn = 257,
		PreLoadMinigame = 292,
		OnDialogueComplete = 293
	}
	public SCEventType type;
	
	// The NPC that caused the event
	public NPC eventCauser;
	
	// The position the event takes place
	public Transform location;
	
	// Time delay before event starts
	public float delay = 0;
	
	// Game objects to be notifed about the event when it starts
	public List<GameObject> gameObjectsToNotify;
	
	// Message to be sent to objects needed to be notified
	string messageName;
	
	// Dialogue to be passed as parameter when notifying objects if required
	public Dialogue parameter;
	
	// If the event is a atomic operation. Used to skip dialogue that triggers the event
	public bool atomicOperation = true;
	
	#region Set Event Information	
	/// <summary>
	/// Sets the event causer.
	/// </summary>
	/// <param name='_eventCauser'>
	/// The causer of the event
	/// </param>
	/// <param name='returnToCauser'>
	/// If true player return to the causer (NPC) before the event is triggered
	/// </param>
	public void SetEventCauser(NPC _eventCauser, bool returnToCauser = true)
	{
		eventCauser = _eventCauser;
		
		// If event causes player to return to the NPC that caused the event
		if (returnToCauser)
			// Set the location of the causer
			location = _eventCauser.transform;
	}
	
	/// <summary>
	/// Clears the list with game objects to notify
	/// </summary>
	public void ClearObjectsToNotify ()
	{
		gameObjectsToNotify.Clear();
	}
	
	/// <summary>
	/// Adds game object to be notified that the event has been triggered
	/// </summary>
	/// <param name='go'>
	/// Game object to be notified
	/// </param>
	public void AddObjectToNotify (GameObject go)
	{
		gameObjectsToNotify.Add(go);
	}
	#endregion
	
	#region Trigger Event
	/// <summary>
	/// Triggers the event with no delay
	/// </summary>
	/// <param name='forceStart'>
	/// Force start directly without moving to a target location first
	/// </param>
	public void TriggerEvent (bool forceStart)
	{
		// Get message name that needs to be sent to objects to be notified
		messageName = type.ToString();
		// If event doesn't need to start directly and there is a target lcation set
		/*if (location != null)
		{
			// Tell player to move to target location for the event
			Player.instance.MoveToEvent(location);
			// and update message that needs to be sent to objects
			//messageName = "Pre" + messageName;
		}
		*/
		// Trigger either main event or "pre" event with set delay time
		Invoke("TriggerEvent", delay);
	}
	
	/// <summary>
	/// Triggers the event with delay
	/// </summary>
	private void TriggerEvent()
	{
		// Notify all game objects about the event
		foreach(GameObject go in gameObjectsToNotify)
		{
			if (go != null)
			{
				// activate target game object
				go.SetActive(true);
				// and notify it
				try{
					go.BroadcastMessage(messageName, parameter, SendMessageOptions.DontRequireReceiver);
				}catch{
					Debug.LogWarning(messageName + " was unabled to be sent to " + go.name);
				}
			}
			else
			{
				Debug.LogWarning("Game object not set when triggering event " + type.ToString());
			}
		}
	}
	#endregion
}
