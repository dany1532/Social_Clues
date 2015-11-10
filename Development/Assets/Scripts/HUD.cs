using UnityEngine;
using System.Collections;

/// <summary>
/// HUD manager
/// </summary>
public class HUD : Singleton<HUD> {

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	/// <summary>
	/// Executed when the on quest completed event is triggered and adds a new marker
	/// </summary>
	public void OnQuestCompleted()
	{
		// Activate the HUD
		this.gameObject.SetActive(true);
	}
}
