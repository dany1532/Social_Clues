using UnityEngine;
using System.Collections;

public class PuzzlePiece : MonoBehaviour {
	
	public UITexture myUITexture;
	public Color myColor;
	public Shader myShader;

	
	PuzzlePieceInfo myPuzzlePiece;
	Texture2D myTexture;
	Texture2D mainImageTexture;
	Texture2D puzzleOutline;
	//Texture2D pictureTexture;
			
/*///////////////////////////
	 IMPORTANT
////////////////////////////
	Individual piece only has main outline of the piece,
    still need to apply the color of the main picture (Texture2D pictureTexture) 

//////////////////////////*/
	
	
	//Cleans up the individual piece's texture and applies it
	public void CreateTexture(PuzzlePieceInfo piece, Texture2D outLine, Texture2D theImage){
		Color mainImageColor;
		
		
		//Set Up
		myPuzzlePiece = new PuzzlePieceInfo();
		puzzleOutline = outLine;
		mainImageTexture = theImage;
		myPuzzlePiece = piece;
		
		
		//Where does the texture start..
		int initX = myPuzzlePiece.uvX;
		int initY = myPuzzlePiece.uvY;
		
		//How much of the texture will be shown (uv rect)
		int textureWidth = myPuzzlePiece.uvWidth;
		int textureHeight = myPuzzlePiece.uvHeight;
		
		myColor = myPuzzlePiece.myColor;
		
		Material mat = new Material(myShader);
		
		myUITexture.material = mat;
		
		float offsetX = (float) initX/(float)puzzleOutline.width;
		float offsetY = (float) initY/(float)puzzleOutline.height;
		
		//New "Canvas" to paint on
		//myTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

#if USE_SHADER
		myUITexture.material.SetColor("_MainColor", myColor);
		myUITexture.material.SetTexture("_OutlineTex", puzzleOutline);
		myUITexture.material.SetTexture("_MainTex", mainImageTexture);
			
		Rect newUVRect = myUITexture.uvRect;
		newUVRect.x = offsetX;
		newUVRect.y = offsetY;
		newUVRect.width = (float) textureWidth / (float) puzzleOutline.width;
		newUVRect.height = (float) textureHeight / (float) puzzleOutline.height;
		
		myUITexture.uvRect = newUVRect;
		
		this.gameObject.SetActive(true);
#else
		
		myTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);

		int numColors = (int) (textureWidth * textureHeight);
		int index = 0;
		int counter = 0;
		var colors = new Color32[numColors];
		var outlineColors = puzzleOutline.GetPixels(initX, initY, textureWidth, textureHeight);
		var mainImageColors = mainImageTexture.GetPixels(initX, initY, textureWidth, textureHeight);
		
		
		//Using SetPixels32 and GetPixels()
		for(int i = 0; i < numColors; ++i){
			if(outlineColors[i] != myColor) {
				colors[i] = Color.clear;
			}
			else{
				colors[i] = mainImageColors[i];
			}
		}
		 
		
		//Using SetPixels32 with no GetPixels()
		
		//Clears any other color that it is not its own
//		for(int y = initY; y < initY + textureHeight; y++){
//			for(int x = initX; x < initX + textureWidth; x++){
//				
//				//if pixel is not the piece's color, clear it
//				if(puzzleOutline.GetPixel(x,y) != myColor) {
//					colors[counter] = Color.clear;
//				}
//				else{
//					colors[counter] = mainImageTexture.GetPixel(x,y);
//				}
//				counter++;
//			}
//		}
		
		//Using individual set pixels (make sure to comment out setpixels32())
				//Clears any other color that it is not its own
//		for(int y = initY; y < initY + textureHeight; y++){
//			for(int x = initX; x < initX + textureWidth; x++){
//				
//				//if pixel is not the piece's color, clear it
//				if(puzzleOutline.GetPixel(x,y) != myColor) {
//					myTexture.SetPixel(x-initX,y-initY,Color.clear);
//				}
//				else{
//					mainImageColor = mainImageTexture.GetPixel(x,y);
//					myTexture.SetPixel(x-initX, y-initY, mainImageColor);
//				}
//			}
//		}
		
		myTexture.SetPixels32(colors);
		
		//Commit changes to texture and apply it
		myTexture.wrapMode = TextureWrapMode.Clamp;
		myTexture.Apply(false, true);
		myUITexture.mainTexture = myTexture;
		gameObject.SetActive(true);
#endif
	}
	
	void OnApplicationQuit(){
		myUITexture.mainTexture = null;
	}
}
