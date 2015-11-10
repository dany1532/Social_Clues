using UnityEngine;
using System.Collections;

public class DisplayedObject : MonoBehaviour {
	
	public NewNancyManager manager;
	
	public UIScaledSprite sprite;
	public UISprite tick;
	
	private HiddenObject hiddenObject;
	
	public void SetObject (HiddenObject newObject)
	{
		hiddenObject = newObject;
	}
	
	public void Found()
	{
		tick.enabled = true;
		manager.HiddenObjectFound(hiddenObject.transform.position);
	}
}
