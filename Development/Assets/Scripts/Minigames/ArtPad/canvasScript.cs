using UnityEngine;
using System.Collections;

public class canvasScript : MonoBehaviour
{
	
	public Camera myCamera;
	Color[] colors;
	Color currentColor;
	public enum tool {crayon, marker, pencil, eraser};
	int toolWidth;
	Texture2D myTexture;
	Vector2 previousPixel;
	Vector2 currentPixel;
	bool prevPixExists;
	public GameObject currentTool;
	
	// Use this for initialization
	void Start ()
	{
	
		myTexture = new Texture2D (1600, 1200);
		gameObject.GetComponent<UITexture> ().mainTexture = myTexture;
		
		Color c = Color.white;
		for (int y = 0; y <myTexture.height; y++)
			for (int x = 0; x<myTexture.width; x++) {	
				myTexture.SetPixel (x, y, c);
			}
		
		myTexture.Apply ();
		
		currentColor = Color.red;
		toolWidth = 20;
		
		colors = new Color[toolWidth*toolWidth];
		for (int i = 0 ; i < (toolWidth*toolWidth) ; i++) colors[i] = currentColor;
		
		prevPixExists = false;
		
	}
	
	public void changeTool(tool t)
	{		
		switch(t)
		{
		case tool.crayon:
			toolWidth = 40;
			break;
		case tool.marker:
			toolWidth = 20;
			break;
		case tool.pencil:
			toolWidth = 10;
			break;
		case tool.eraser:
			toolWidth = 40;
			break;
		default:
			break;
		}
		
		colors = new Color[toolWidth*toolWidth];
	}
	
	public void setColor(Color c)
	{
		currentColor = c;
		for (int i = 0 ; i < (toolWidth*toolWidth) ; i++) colors[i] = currentColor;
	}
	
	public void restart()
	{
		Color c = Color.white;
		for (int y = 0; y <myTexture.height; y++)
			for (int x = 0; x<myTexture.width; x++) {	
				myTexture.SetPixel (x, y, c);
			}
		
		myTexture.Apply ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (InputManager.Instance.HasReceivedDrag()) {
			
			// Send a ray to collide with the plane
			Ray ray = myCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			 
			if (collider.Raycast (ray, out hit, Mathf.Infinity)) {
				// Find the u,v coordinate of the Texture
				Vector2 uv;
				 
				uv.x = (hit.point.x - hit.collider.bounds.min.x) / hit.collider.bounds.size.x;
				uv.y = (hit.point.y - hit.collider.bounds.min.y) / hit.collider.bounds.size.y;
				 
				// Paint it red
				//Texture2D tex = (Texture2D)hit.transform.gameObject.renderer.sharedMaterial.mainTexture;
				
				currentPixel = new Vector2 (uv.x * myTexture.width, uv.y * myTexture.height);
					
				myTexture.SetPixels ((int)currentPixel.x, (int)currentPixel.y, toolWidth, toolWidth, colors);
				
				paint(currentPixel);
				
				//backdrop.GetComponent<GUITexture>().Apply ();
				previousPixel = currentPixel;
				prevPixExists = true;
			
			} //end raycast
		}//end if mouse button down
		else { 
			prevPixExists = false;
			
		}
			
	}
	
	void paint(Vector2 currentPixel)
	{
		if (prevPixExists)
		{
			
			if ((currentPixel.x - previousPixel.x) != 0) 
			{
				//draw a line between the current pixel and the previous one
				float slope = ((float)(currentPixel.y - previousPixel.y) / (currentPixel.x - previousPixel.x));
		
				float y = (int)currentPixel.y;		
				
				if (currentPixel.y > previousPixel.y)
				{
					if(currentPixel.x > previousPixel.x)
					{
						for (int x = (int)currentPixel.x; x>=(int)previousPixel.x; x--) 
						{
							myTexture.SetPixels (x, (int)Mathf.Round (y + slope),toolWidth,toolWidth, colors);
							y -= slope;
						}
					}
					else
					{
						for (int x = (int)currentPixel.x; x<(int)previousPixel.x; x++) 
						{
							myTexture.SetPixels (x, (int)Mathf.Round (y + slope),toolWidth,toolWidth, colors);	
							y += slope;
						}
					}
				}
				else
				{
					if(currentPixel.x > previousPixel.x)
					{
						for (int x = (int)currentPixel.x; x>=(int)previousPixel.x; x--) 
						{
							myTexture.SetPixels (x, (int)Mathf.Round (y + slope), toolWidth,toolWidth, colors);
							y -= slope;
						}
					}
					else
					{
						for (int x = (int)currentPixel.x; x<(int)previousPixel.x; x++) 
						{
							myTexture.SetPixels (x, (int)Mathf.Round (y + slope), toolWidth,toolWidth, colors);
							y += slope;
						}
					}
				}
			} 
			else //if currX - prevX ==0
			{
			
				if (currentPixel.y > previousPixel.y)
				{
					for (int y = (int)currentPixel.y; y>=(int)previousPixel.y; y--) 
					{
						myTexture.SetPixels ((int)currentPixel.x, (int)Mathf.Round (y), toolWidth,toolWidth, colors);
					}
				}
				else
				{
					for (int y = (int)currentPixel.y; y<(int)previousPixel.y; y++) 
					{
						myTexture.SetPixels ((int)currentPixel.x, (int)Mathf.Round (y), toolWidth,toolWidth, colors);
					}
				}
			}
			
			myTexture.Apply ();
		}//end of ifPreviousPixelExists
				
		
	}
 
}
