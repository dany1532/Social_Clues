using UnityEngine;
using System.Collections;

public class UIButtonArt : MonoBehaviour {
	
	public enum function{restart, save, changeTool, undo, backToToyBox, openToolMenu, redo}
	public function myFunction;
	
	public Color myColor;
	public Drawing.ToolType myTool;
	public GameObject myMenu;
	public GameObject[] otherMenus;
	Vector3 offset = new Vector3(0.0f,0.1f, 0.0f);

	Drawing canvas;
	
	void Awake()
	{
		canvas = GameObject.FindObjectOfType(typeof(Drawing)) as Drawing;
	}
	
	// Use this for initialization
	void Start () {
		if(myFunction == function.openToolMenu)
			myMenu.SetActive(false);
		if(myTool == Drawing.ToolType.Marker && myFunction == function.openToolMenu)
			myMenu.SetActive(true);
		if(myTool == Drawing.ToolType.Marker && gameObject.name == "Red")
		{
			transform.position = transform.position + offset;
			canvas.currentToolInstance = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if((myFunction == function.undo && !canvas.canUndo)||(myFunction == function.redo && !canvas.canRedo))
			gameObject.GetComponentInChildren<UISprite>().color = new Color(255.0f, 255.0f, 255.0f, 1.0f);
		else if(myFunction == function.undo || myFunction == function.redo)
			gameObject.GetComponentInChildren<UISprite>().color = myColor;

	}
	
	void OnClick()
	{
		switch(myFunction)
		{
		case function.restart:
			canvas.restart();
			break;
		case function.openToolMenu:
			myMenu.SetActive(true);
			canvas.changeTool(myTool);
			foreach(GameObject m in otherMenus) m.SetActive(false);
			break;
		case function.backToToyBox:
			Debug.Log ("Return to ToyBox");
			break;
		case function.changeTool:
			canvas.changeTool(myTool);
			canvas.setColor(myColor);
			if(canvas.currentToolInstance != null && canvas.currentToolInstance.name != "Eraser")
					canvas.currentToolInstance.position = canvas.currentToolInstance.position - offset;
			if(myTool != Drawing.ToolType.Eraser)
				transform.position = transform.position + offset;
			canvas.currentToolInstance = transform;
			break;
		case function.save:
			canvas.SaveTexture();
			Debug.Log ("Save");
			break;
		case function.undo:
			if(canvas.canUndo)
			{
				Debug.Log ("Undo");
				canvas.UndoStep();
			}
			else
			{
				Debug.Log ("Can't Undo");
			}
			break;
		case function.redo:
			if(canvas.canRedo)
			{
				Debug.Log ("Redo");
				canvas.RedoStep();
			}
			else
			{
				Debug.Log("Can't Redo");
			}
			break;
		default:
			break;
		}
	}
}
