using UnityEngine;
using System.Collections;

public class Toggle : MonoBehaviour {
	public enum STATE {ON, OFF};
	public STATE state;
	public UISprite background;
	public UISprite thumb;
	public UILabel description;

	private float spriteOffset = 47;
	private float textOffset = 26;
	
	/// <summary>
	/// Event receiver that will be notified of the value changes.
	/// </summary>
	
	public GameObject eventReceiver;
	
	/// <summary>
	/// Function on the event receiver that will receive the value changes.
	/// </summary>
	
	public string functionName = "OnToggleChange";

	public void setState(bool value) {
		if((value && state == STATE.OFF) || (!value && state == STATE.ON)) {
			toggle ();
		}
	}

	public void toggle() {
		if(state == STATE.ON) {
			// toggle to off
			state = STATE.OFF;
			background.color = new Color(0.745f, 0.745f, 0.745f, 1.0f);
			description.text = "OFF";
			description.color = Color.black;
			description.transform.localPosition = new Vector3(description.transform.localPosition.x + textOffset, description.transform.localPosition.y, description.transform.localPosition.z);
			thumb.transform.localPosition = new Vector3(thumb.transform.localPosition.x - spriteOffset, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
		} else {
			// toggle to on
			state = STATE.ON;
			background.color = new Color(0.9921f, 0.745f, 0f, 1.0f);
			description.text = "ON";
			description.color = Color.white;
			description.transform.localPosition = new Vector3(description.transform.localPosition.x - textOffset, description.transform.localPosition.y, description.transform.localPosition.z);
			thumb.transform.localPosition = new Vector3(thumb.transform.localPosition.x + spriteOffset, thumb.transform.localPosition.y, thumb.transform.localPosition.z);
		}
		
		if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
		{
			eventReceiver.SendMessage(functionName, (state == STATE.ON ? true : false), SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnClick() {
		toggle();
	}
}
