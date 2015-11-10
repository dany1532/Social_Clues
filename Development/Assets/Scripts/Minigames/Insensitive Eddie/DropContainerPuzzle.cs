using UnityEngine;
using System.Collections;

public class DropContainerPuzzle : MonoBehaviour {
	
	public UITexture myUITexture;
	public Color myColor;

	
	PuzzlePieceInfo myPuzzlePiece;
	Texture2D myTexture;
	Texture2D mainImageTexture;
	Texture2D puzzleOutline;
	
	bool initiated = false;


	void OnDrop(GameObject dropped)
	{
		DraggablePuzzlePiece piece = dropped.GetComponent<DraggablePuzzlePiece> ();
		if(piece != null)
		{
			//Debug.Log ("Container "+gameObject.name+ " received puzzle piece "+dropped.name);
			piece.hasBeenDroppedInContainer(this.gameObject.name);
		}
	}
	
	public void initiate(PuzzlePieceInfo piece, Texture2D outLine, Texture2D theImage)
	{
		if(!initiated)
		{
			//set object's texture to puzzle-piece shaped highlight
			Color mainImageColor;
			
			//Set Up
			myPuzzlePiece = new PuzzlePieceInfo();
			puzzleOutline = outLine;
			mainImageTexture = theImage;
			myPuzzlePiece = piece;
			
	//		Debug.Log(pictureTexture.width);
	//		Debug.Log(pictureTexture.height);
	//		
	//		Debug.Log("w "+outLine.width);
	//		Debug.Log("h "+outLine.height);
			
			//Where does the texture start..
			int initX = myPuzzlePiece.uvX;
			int initY = myPuzzlePiece.uvY;
			
			//How much of the texture will be shown (uv rect)
			float textureWidth = myPuzzlePiece.uvWidth;
			float textureHeight = myPuzzlePiece.uvHeight;
			
			myColor = myPuzzlePiece.myColor;
			
			//New "Canvas" to paint on
			myTexture = new Texture2D((int)textureWidth, (int)textureHeight);
			
			//Clears any other color that it is not its own
			for(int y = initY; y < initY + textureHeight; y++){
				for(int x = initX; x < initX + textureWidth; x++){
					
					//if pixel is not the piece's color, clear it
					if(puzzleOutline.GetPixel(x,y) != myColor) {
						myTexture.SetPixel(x-initX,y-initY,Color.clear);
					}
					else{
						mainImageColor = mainImageTexture.GetPixel(x,y);
						myTexture.SetPixel(x-initX, y-initY, mainImageColor);
					}
				}
			}
			
			//Commit changes to texture and apply it
			myTexture.wrapMode = TextureWrapMode.Clamp;
			myTexture.Apply();
			myUITexture.mainTexture = myTexture;
			
			myUITexture.enabled = false;
			initiated =true;
		}
		myUITexture.mainTexture = myTexture;
		myUITexture.enabled = false;
	}

}
