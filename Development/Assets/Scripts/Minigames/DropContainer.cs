using UnityEngine;
using System.Collections;

public class DropContainer : MonoBehaviour {	
	public GameObject highlight;
	
	void Start()
	{
		if(highlight != null)
		{
			highlight.SetActive(false);
		}
	}
	
	void OnDrop(GameObject dropped)
	{
		DraggableObjectBuffet draggableObject = dropped.GetComponent<DraggableObjectBuffet>();
		if(draggableObject != null)
			draggableObject.hasBeenDroppedInContainer(this.gameObject.name);
		else
			dropped.GetComponent<DraggableObject>().hasBeenDroppedInContainer();
		
		if(highlight != null)
		{
			highlight.SetActive(false);
		}
	}
	
	public void hover(bool isOver)
	{
		if (isOver && highlight != null)
		{
			highlight.SetActive(true);
			gameObject.GetComponent<UISprite>().color = new Color(.8f,.8f,.8f);
		}
		else if (!isOver)
		{
			highlight.SetActive(false);
			gameObject.GetComponent<UISprite>().color = new Color(1,1,1);
		}
	}
	
	/*void OnHover(bool isOver)
	{
		if (isOver && highlight != null)
		{
			highlight.SetActive(true);
		}
		else if (!isOver)
		{
			highlight.SetActive(false);
		}
	}
	
	void OnDrag(Vector2 delta)
	{
		if (highlight != null)
		{
			highlight.SetActive(true);
		}
	}
	
	void OnCollisionEnter(Collider collider)
	{
		if (highlight != null)
		{
			highlight.SetActive(true);
		}
	}
	
	void OnCollisionExit(Collider collider)
	{
		if (highlight != null)
		{
			highlight.SetActive(false);
		}
	}*/
}
