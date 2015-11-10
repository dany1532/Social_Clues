using UnityEngine;
using System.Collections;

public class TutorialHand : MonoBehaviour {
	
	UISprite mySprite;
	public Vector3[] waypoints;
	int currentWayPoint = -1;
	public float moveInterval = 1.0f;
	bool pointing;

	// Use this for initialization
	void Start () {
		mySprite = gameObject.GetComponent<UISprite>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void isPointing(bool state)
	{
		if (state) 
		{
			mySprite.spriteName = "HandDrag";
			pointing = true;
		}
		else
		{
			mySprite.spriteName = "HandMove";
			pointing = false;
		}
	}

	public bool getIsPointing()
	{
		return pointing;
	}
	
	void setHover()
	{
		mySprite.spriteName = "HandMove";
	}
	
	public void nextWayPoint()
	{
		currentWayPoint++;
		TweenPosition.Begin(gameObject,moveInterval,waypoints[currentWayPoint]);
	}

	public Vector2 get2DPos()
	{
		return new Vector2(transform.position.x, transform.position.y);
	}
}
