using UnityEngine;
using System.Collections;

public class SwitchTextureUI : MonoBehaviour {
	UISprite uiSprite;
	UIStretch uiStretch;
	
	public float targetHeight;
	
	public Texture newTexture;
	
	// Use this for initialization
	void Start () {
		uiStretch = GetComponent<UIStretch>();
		uiSprite = GetComponent<UISprite>();
	}
	
	// Update is called once per frame
	void OnPress (bool press) {
		
		if (ApplicationState.Instance.isPaused ())
			return;
		if (press)
		{
			InputManager.Instance.ReceivedUIInput();
			uiSprite.spriteName = newTexture.name;
			uiStretch.relativeSize.y = targetHeight;
			uiStretch.initialSize = new Vector2 (uiSprite.innerUV.width, uiSprite.innerUV.height);
		}
	}
}
