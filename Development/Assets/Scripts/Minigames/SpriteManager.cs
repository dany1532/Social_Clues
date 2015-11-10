using UnityEngine;
using System.Collections;

public class SpriteManager : MonoBehaviour {
	
	public string[] mySprites;
	
	public void toSprite(int level)
	{
		this.GetComponent<UISprite>().spriteName = mySprites[level];
	}
}
