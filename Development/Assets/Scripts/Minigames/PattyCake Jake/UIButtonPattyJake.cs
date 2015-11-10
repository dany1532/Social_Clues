//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Similar to UIButtonColor, but adds a 'disabled' state based on whether the collider is enabled or not.
/// </summary>

public class UIButtonPattyJake : MonoBehaviour
{
	/// <summary>
	/// Color that will be applied when the button is disabled.
	/// </summary>
	public UISprite sprite;
	public Animation animation;
	public GameObject glow;
	
	void Start()
	{
		animation = GetComponentInChildren<Animation>();
	}
}
