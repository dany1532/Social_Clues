using UnityEngine;
using System.Collections;

public class Interest : MonoBehaviour {
	
	public NewNancyManager manager;
	public string topic;
	public UIScaledSprite sprite;
	
	public void OnClick()
	{
		manager.InterestSelected(this);
	}
	
	public void Enable(string newTopic)
	{
		topic = newTopic;
		sprite.UpdateSprite(topic, Color.white);
		sprite.UpdateStretching();
		this.collider.enabled = true;
	}
	
	public void Disable ()
	{
		sprite.UpdateColor(Color.grey);
		this.collider.enabled = false;
	}
}
