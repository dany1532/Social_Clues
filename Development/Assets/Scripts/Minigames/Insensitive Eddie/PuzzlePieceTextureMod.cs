using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzlePieceTextureMod : MonoBehaviour {
	
	public GameObject puzzlePiecePrefab;
	public Texture2D puzzleOutline;
	public Texture2D mainPuzzleImage;
	public Texture2D hintImage;
	
	public int numRows = 0;
	public int numCol = 0;
	
	public float scalar;

	PuzzlePieceInfo[,] indivPiecesArray;
	Texture2D myTexture;
	bool wantClear = false;
	Color thresholdColor;
	int COLORTHRESHOLD = 30;
	char diff;
	
	GameObject[] locations;
	int locationCounter = 0;

	
	// Use this for initialization
	void Start () {

		//Find and store the default starting puzzle pieces' locations (gameobjects) 
		locations = new GameObject[20];
		int i = 0;
		for (int x = 0; x<5; x++)
		{
			for(int y = 0; y<4; y++)
			{
				locations[i] = GameObject.Find("Location"+x+y);
				i++;
			}
		}
		
		//shuffle the starting locations
		System.Random rng = new System.Random();  
    	int n = locations.Length;  
    	while (n > 1) {  
	        n--;  
	        int k = rng.Next(n + 1);  
	        GameObject value = locations[k];  
	        locations[k] = locations[n];  
	        locations[n] = value;  
	    }  

		//Get the game's difficulty
		diff = GameObject.Find("EddieMinigame").GetComponent<EddiePuzzleManager>().getDifficulty();

		// Set up variables to be used to find # of columns and rows
		int counter = 0;
		var colors = new Color[puzzleOutline.width * puzzleOutline.height];
		colors = puzzleOutline.GetPixels(0,0,puzzleOutline.width, puzzleOutline.height);
		Color prevColor = colors[0];

		//Find # of column pieces from the jigsaw puzzle outline
			//if the previous 30 pixels(color threshold) are from the same color then increment column pieces
		for(int x = 1; x < puzzleOutline.width; x++)
		{
			if(prevColor == colors[x])
				counter++;
			
			else{
				counter = 0;
				prevColor = colors[x];
			}
			
			if(counter == COLORTHRESHOLD)
				numCol++;	
		}
		
		//reset variables for the # of row pieces
		int index = 0;
		counter = 0;
		prevColor = colors[0];
		
		//Get number of row pieces
			//if the previous 30 pixels(colorthreshold) are from the same color then increment row pieces
		for(int y = 1; y < puzzleOutline.height; y++){
			//go up one row
			index = y * puzzleOutline.width;

			if(prevColor == colors[index])
				counter++;
			
			else{
				counter = 0;
				prevColor = colors[index];
			}
			
			if(counter == COLORTHRESHOLD){
				numRows++;	
			
			}
		}
		
		//Depending on number of rows/columns, how far are from each other
		    //Used to get the average middle point of each piece
		int rowOffset = puzzleOutline.height/numRows;
		int columnOffset = puzzleOutline.width/numCol;
		
		//For Storing the puzzle piece's info (color, location, UVs)
			//Create jigsaw pieces depending on now of rows and columns
		int indexX = 0;
		int indexY = 0;
		indivPiecesArray = new PuzzlePieceInfo[numRows,numCol];

		//Using the calculated average row distance from each jigsaw puzzle,
		 //Go over the columns and create/set jigsaw piece info (color, location)
		for(int y = rowOffset/2; y < puzzleOutline.height-10; y += rowOffset)
		{
			index = y * puzzleOutline.width;
			counter = 0;
			prevColor = colors[index];
			
			for(int x = 1; x < puzzleOutline.width; x++){
				//Go up one jigsaw piece
				index = y * puzzleOutline.width + x;
				
				if(prevColor == colors[index])
					counter++;
				
				else{
					counter = 0;
					prevColor = colors[index];
				}

				//Create jigsaw piece with location in relation to the jigsaw puzzle and color)
				if(counter == COLORTHRESHOLD){
					CreateIndividualPiece(indexY, indexX, prevColor);
					indexX++;
				}
			}
			
			indexX = 0;
			indexY++;
		}
			
		//Calculate each piece's UV's and create prefab	
		for(int row = 0; row < numRows; row++)
		{
			for(int col = 0; col < numCol; col++)
			{
				CalculateUVRect(indivPiecesArray[row, col], colors);
				InstantiatePrefab(indivPiecesArray[row,col]);
			}
		}
	}
	
	
	//Create Prefab, position in world and make it pixel perfect
	void InstantiatePrefab(PuzzlePieceInfo thePiece){
		
		//Create Prefab and change name
		GameObject piece = GameObject.Instantiate(puzzlePiecePrefab) as GameObject;
		piece.name = "IndividualPiece "+ thePiece.myRow.ToString() + thePiece.myColumn.ToString();
		
		//Calls the function to further clean up the individual piece
		piece.GetComponent<PuzzlePiece>().CreateTexture(thePiece, puzzleOutline, mainPuzzleImage);
		
		//Sets up the position of the piece
		piece.transform.parent = GameObject.FindGameObjectWithTag("PuzzlePanel").transform;
		
		Vector3 loc = piece.transform.localPosition;
		
		loc.x = locations[locationCounter].transform.position.x;
		loc.y = locations[locationCounter].transform.position.y;
		locationCounter++;
		//loc.x += 160 * thePiece.myColumn;
		//loc.y += 160 * thePiece.myRow;
		
		piece.transform.position = loc;
		
		Vector3 scale = piece.transform.localScale;
		scale.x = thePiece.uvWidth;
		scale.y = thePiece.uvHeight;
		piece.transform.localScale = scale;
		

#if !USE_SHADER
		//Scale it to make it pixel perfect
		piece.GetComponent<UITexture>().MakePixelPerfect();
#endif
		//Some further scaling since the pieces are too big
		scale = piece.transform.localScale;
		
		scale.Scale(new Vector3(scalar, scalar, scalar));
		
		piece.transform.localScale = scale;
		
		//add on extra scripts so it behaves like a puzzle piece
		piece.AddComponent<DraggablePuzzlePiece>();
		piece.GetComponent<DraggablePuzzlePiece>().isSolution = true;
		piece.GetComponent<DraggablePuzzlePiece>().mySlot = GameObject.Find ("Slot"+thePiece.myColumn.ToString()+thePiece.myRow.ToString()+diff);
		piece.GetComponent<DraggablePuzzlePiece>().mySlot.GetComponent<DropContainerPuzzle>().initiate(thePiece, puzzleOutline, hintImage);
		
		piece.AddComponent<BoxCollider>();
		piece.GetComponent<BoxCollider>().size = new Vector3(.8f,.8f,1f);
		
		piece.AddComponent<UIDragObject>();
		piece.GetComponent<UIDragObject>().target= piece.transform;
		piece.GetComponent<UIDragObject>().restrictWithinPanel = true;
		piece.GetComponent<UIDragObject>().dragEffect = UIDragObject.DragEffect.None;
		
	}
	
	//Creates individual piece (without UV's set up)
	void CreateIndividualPiece(int row, int col, Color pieceColor){
		
		PuzzlePieceInfo puzzlePiece = new PuzzlePieceInfo();
		puzzlePiece.myColor = pieceColor;
		puzzlePiece.myRow = row;
		puzzlePiece.myColumn = col;

			
		indivPiecesArray[row,col] = puzzlePiece;
		
	}
	
	//Calculate the UV's depending on piece's grid location
	void CalculateUVRect(PuzzlePieceInfo piece, Color[] colors){
		int prevUVX;
		int prevUVY;
		
	//if it is the very first piece, set UV's location to (0,0)
		if(piece.myRow == 0 && piece.myColumn == 0){
			piece.uvX = 0;
			piece.uvY = 0;
			
			CalculateMinMax(piece, 0, 0, false, colors);
		}
		
		//if it is a piece on the first row, set UV's location to (UVX,0)
		else if(piece.myRow == 0){
			piece.uvY = 0;
			
			prevUVX = indivPiecesArray[piece.myRow, piece.myColumn-1].uvX;
			prevUVY = 0;
			
			CalculateMinMax(piece, prevUVX, prevUVY, true, colors);
		}
		
	//if it is a piece on the first column, set UV's location to (0, UVY)
		else if(piece.myColumn == 0){
			piece.uvX = 0;
			
			prevUVY = indivPiecesArray[piece.myRow-1, piece.myColumn].uvY;
			prevUVX = 0;
			
			CalculateMinMax(piece, prevUVX,  prevUVY, true, colors);
			
		}
		
	
	//For the middle pieces
		else{
			prevUVX = indivPiecesArray[piece.myRow, piece.myColumn-1].uvX;
			prevUVY = indivPiecesArray[piece.myRow-1, piece.myColumn].uvY;
			
			CalculateMinMax(piece, prevUVX, prevUVY, true, colors);
			
		}
			
	}
	
	//Main Calculation to get each individual piece's UVs
	void CalculateMinMax(PuzzlePieceInfo piece, int startX, int startY, bool findMinX, Color[] colors){
		//Set up
		int minX = int.MaxValue;
		int maxX = int.MinValue;
		int minY = 0;
		int maxY = 0;
		
		int counterX = 0;
		int index = 0;
		bool notFound = false;
		bool findMinY = true;
		
		//if the piece is the last on column, then maxX will be texture's width
		if(piece.myColumn == numCol-1)
			maxX = puzzleOutline.width;
		
		//if the piece is the last on row, then maxY will be texture's height
		if(piece.myRow == numRows-1)
			maxY = puzzleOutline.height;
		
		//Don't need to find the minY for pieces on row 0 since it is 0
		if(piece.myRow == 0)
			findMinY = false;
		
		//Main For loop
		for(int y = startY; y < puzzleOutline.height; y++){
			
			//New row, then try to find a new MinX
			findMinX = true;
			
			//if it didn't find the piece's color after one traversal, assume end of piece
			if(notFound)
			{
				if(piece.myRow != numRows - 1){
					maxY = y+2; //Just some padding
				}
				break;			
			}
			
			for(int x = startX; x < puzzleOutline.width; x++)
			{
				index = y * puzzleOutline.width + x;
				//Find the uvX of currentPiece using the piece on the left as reference
				if(findMinX)
				{
					
				   //if the pixel has the same color as the piece...
					if( piece.myColor ==  colors[index]){
					
						//check if pixel positiion is lower than the current min and swap accordingly
						if(x < minX) 
							minX = x;
						
						//find the very first occurence of the piece's color to find minY
						if(findMinY){
							minY = y;
							findMinY = false;
						}
						
						//found first piece's color occurrence find the row's maxX (if not the last column piece)
						if(piece.myColumn != numCol - 1){
							findMinX = false;	
						}
						
						else
							break;
					}
					
					 //if the current pixel is not the piece's color, is greater than current max
					 //Or reached end of texture assume end of piece
					else if((maxX != int.MinValue && x > maxX)  ||
						   (maxX != int.MinValue && !findMinY && x == puzzleOutline.width -1))
					{
						notFound = true;
						break;
					}
				}
				//find the MaxX that the piece covers
				if(!findMinX)
				{
					
					//at the first occurrence that the color is no longer the same as the piece's
					if(piece.myColor != colors[index]){
						
						//Check if currentX is greater than current Max and break regardless
						if(x > maxX) {
							maxX = x;
						}
						break;
					}
				}
			}//end X for loop
		}//end y for loop
		
		piece.uvX = minX;
		piece.uvY = minY;
		piece.uvHeight = maxY - minY;
		piece.uvWidth = maxX - minX;
	}
}
