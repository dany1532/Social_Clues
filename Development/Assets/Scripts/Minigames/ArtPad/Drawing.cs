using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Drawing : MonoBehaviour
{

#region painter
	public Texture2D sourceBaseTex;
	private Texture2D baseTex;
	public Texture2D undoTex;
	public Texture2D redoTex;
	public bool canUndo, canRedo, canDraw;

	GameObject tutorial;
		
 	public Camera uiCamera;
 
	static Vector2 screenToTextureAspectRatio;
	
	private Vector2 dragStart;
	private Vector2 dragEnd;
	
	private Vector2 preDrag;
	private static Color transparent = new Color(0,0,0,0);
	public enum ToolType
	{
		None=-1,
		Eraser=0,
		Crayon,
		Marker,
		Pencil
	}
	
	[System.Serializable]
	public class Tool
	{
		public ToolType type = ToolType.None;
		public float width = 10;
		public float hardness = 1;
		public Color baseColor = Color.white;
		public float noise = -1;
		
		public enum Mode
		{
			None,
			Overwrite,
			Blend
		}
		public Mode mode = Mode.None;
		
		public void  Brush (Color color, Vector2 p1, Vector2 p2, Texture2D tex)
		{
			if (p2 == Vector2.zero) {
				p2 = p1;
			}
			Drawing.PaintLine (p1, p2, width, (Color) ((type == ToolType.Eraser) ? new Color(0,0,0,0) : color * baseColor), hardness, tex, mode, noise);
			tex.Apply ();
		}
	}
	
	public List<Tool> tools;
	Dictionary<ToolType, Tool> toolsDict;
	
	public Tool currentTool;
	public Transform currentToolInstance;
	
	public Color color = Color.white;
	
	UITexture canvas;
	Rect canvasBounds;
	bool drawing = false;
	Vector2 canvasOffset;
	public Transform topLeft;
	
	void  Start ()
	{
		baseTex = (Texture2D)Instantiate (sourceBaseTex);
		undoTex = new Texture2D(baseTex.width, baseTex.height);
		redoTex = new Texture2D(baseTex.width, baseTex.height);
		setUndo();
		canUndo = false;
		canRedo = false;
		canDraw = true;
		
		canvas = gameObject.GetComponent<UITexture> ();
		canvas.mainTexture = baseTex;
		
		Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(canvas.transform);
		Vector3 min = uiCamera.WorldToScreenPoint(bounds.min);
		Vector3 max = uiCamera.WorldToScreenPoint(bounds.max);
				
		canvasBounds = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
		toolsDict = new Dictionary<ToolType, Tool>();
		foreach(Tool tool in tools)
		{
			toolsDict.Add(tool.type, tool);
		}
		
		screenToTextureAspectRatio = new Vector2 (baseTex.width / canvasBounds.width, baseTex.height / canvasBounds.height);
		changeTool(ToolType.Marker);
		setColor(Color.red);
		canvasOffset = uiCamera.WorldToScreenPoint(topLeft.position);
		canvasOffset.y = Screen.height - canvasOffset.y;

		tutorial = GameObject.Find("Tutorial");
	}
	
	void  Update ()
	{

		if(canDraw)
		{
			Vector2 mouse = Input.mousePosition;
			mouse.y = Screen.height - mouse.y;

			if (Input.GetKeyDown ("mouse 0")) {
				if (canvasBounds.Contains (mouse)) { 
					setUndo();
					canUndo = true;
					canRedo = false;
					drawing = true;
					dragEnd = mouse - canvasOffset;
					dragEnd.x = Mathf.Clamp (dragEnd.x, 0, canvasBounds.width);
					dragEnd.y = canvasBounds.height - Mathf.Clamp (dragEnd.y, 0, canvasBounds.height);
				} else {
					drawing = false;
				}
	               
			}
			if (Input.GetKey ("mouse 0") && drawing) {			
				dragEnd = mouse - canvasOffset;
				dragEnd.x = Mathf.Clamp (dragEnd.x, 0, canvasBounds.width);
				dragEnd.y = canvasBounds.height - Mathf.Clamp (dragEnd.y, 0, canvasBounds.height);
				currentTool.Brush(color, dragEnd, preDrag, baseTex);
			}
			
			if (Input.GetKeyUp ("mouse 0")) {
				drawing = false; dragEnd = Vector2.zero;
			}
			preDrag = dragEnd;
		}


	}
#endregion

#region Control Commands
	
	public void changeTool(ToolType type)
	{		
		currentTool = toolsDict[type];
	}
	
	public void setColor(Color c)
	{
		color = c;
	}

	public void SaveTexture ()
	{
#if !UNITY_WEBPLAYER
		// Encode texture into PNG
		var bytes = baseTex.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
#endif
	}
	
	public void UndoStep()
	{
		//before undoing set current state to redo, so if player wants to redo they can come back to this state
		Color[] tempColors = new Color[baseTex.width * baseTex.height];
		tempColors = baseTex.GetPixels();
		redoTex.SetPixels(tempColors);
		redoTex.Apply();

		tempColors = undoTex.GetPixels();
		baseTex.SetPixels(tempColors);
		baseTex.Apply();
		
		canRedo = true;
		canUndo = false;
		
	}
	
	public void RedoStep()
	{
		//before redoing set current state to undo, so if player wants to undo they can come back to this state
		Color[] tempColors = new Color[baseTex.width * baseTex.height];
		tempColors = baseTex.GetPixels();
		undoTex.SetPixels(tempColors);
		undoTex.Apply();
		
		tempColors = redoTex.GetPixels();
		baseTex.SetPixels(tempColors);
		baseTex.Apply();
		
		canUndo = true;
		canRedo = false;
	}
	
	public void setUndo()
	{
		Color[] tempColors = new Color[baseTex.width * baseTex.height];
		tempColors = baseTex.GetPixels();
		undoTex.SetPixels(tempColors);
		undoTex.Apply();
	}
#endregion
	
#region drawing
 
	public void restart()
	{
		int textureSize = baseTex.width * baseTex.height;
		Color[] colors = new Color[textureSize];
		for (int i = 0 ; i < textureSize ; i++) colors[i] = transparent;
		baseTex.SetPixels(colors);
		baseTex.Apply ();
	}
	
	public static Texture2D  PaintLine (Vector2 from, Vector2 to, float rad, Color col, float hardness, Texture2D tex, Tool.Mode mode, float noise)
	{
		var texStart = new Vector2(from.x * screenToTextureAspectRatio.x, from.y * screenToTextureAspectRatio.y);
		var texEnd = new Vector2(to.x * screenToTextureAspectRatio.x, to.y * screenToTextureAspectRatio.y);
		
		var width = rad * 2;
 
		var extent = rad;
		var stY = Mathf.Clamp (Mathf.Min (texStart.y, texEnd.y) - extent, 0, tex.height);
		var stX = Mathf.Clamp (Mathf.Min (texStart.x, texEnd.x) - extent, 0, tex.width);
		var endY = Mathf.Clamp (Mathf.Max (texStart.y, texEnd.y) + extent, 0, tex.height);
		var endX = Mathf.Clamp (Mathf.Max (texStart.x, texEnd.x) + extent, 0, tex.width);
 
		var lengthX = endX - stX;
		var lengthY = endY - stY;
 
		var sqrRad = rad * rad;
		var sqrRad2 = (rad + 1) * (rad + 1);
		Color[] pixels = tex.GetPixels ((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0);
		var start = new Vector2 (stX, stY);
		
		Color c;
		Color baseColor = col;
		for (int y=0; y<(int)lengthY; y++) {
			for (int x=0; x<(int)lengthX; x++) {
				Vector2 p = new Vector2 (x, y) + start;
				Vector2 center = p + new Vector2 (0.5f, 0.5f);
				float dist = (center - NearestPointStrict (texStart, texEnd, center)).sqrMagnitude;
				if (dist > sqrRad2) {
					continue;
				}
				dist = GaussFalloff (Mathf.Sqrt (dist), rad) * hardness;
				
				baseColor = col;
				
				if (noise > 0)
				{
	                float xCoord = (stX + x) + Random.Range(0, 50);
	                float yCoord = (stY + y) + Random.Range(0, 50);
	                float sample = Mathf.PerlinNoise(xCoord, yCoord);
					baseColor = baseColor * noise * sample;
				}
				
				if (dist > 0) {
					if (mode == Tool.Mode.Overwrite)
					{
						c = Color.Lerp (transparent, baseColor, dist);
					}
					else if (mode == Tool.Mode.Blend)
					{
						c = Color.Lerp (pixels [y * (int)lengthX + x], pixels [y * (int)lengthX + x] + baseColor, dist);
					}
					else
						c = Color.Lerp (pixels [y * (int)lengthX + x], baseColor, dist);
				} else {
					c = pixels [y * (int)lengthX + x];
				}
				
				pixels [y * (int)lengthX + x] = c;
			}
		}
		tex.SetPixels ((int)start.x, (int)start.y, (int)lengthX, (int)lengthY, pixels, 0);
		return tex;
	}
 
#endregion
 
#region mathfx
  
	static float Lerp (float start, float end, float value)
	{
		return ((1.0f - value) * start) + (value * end);
	}
 
	static Vector2 NearestPointStrict (Vector2 lineStart, Vector2 lineEnd, Vector2 point)
	{
		var fullDirection = lineEnd - lineStart;
		var lineDirection = fullDirection.normalized;
		var closestPoint = Vector2.Dot ((point - lineStart), lineDirection) / Vector2.Dot (lineDirection, lineDirection);
		return lineStart + (Mathf.Clamp (closestPoint, 0.0f, fullDirection.magnitude) * lineDirection);
	}

	static float GaussFalloff (float distance, float inRadius)
	{
		return Mathf.Clamp01 (Mathf.Pow (360.0f, -Mathf.Pow (distance / inRadius, 2.5f) - 0.01f));
	}
 
#endregion
}
