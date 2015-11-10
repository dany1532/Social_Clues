using UnityEngine;
using System.Collections;

public class ColorArtSet : MonoBehaviour {

	public UISprite portait;
	// Use this for initialization
	void OnClick ()
	{
		portait.color = gameObject.GetComponent<UISprite>().color;
	}
}
