namespace CBX.Unity.Editors.Editor
{
    using System;
	
	using System.Collections.Generic;
	
	using System.IO;

    using CBX.TileMapping.Unity;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// Provides a editor for the <see cref="TileMap"/> component
    /// </summary>
    [CustomEditor(typeof(TileMap))]
    public class TileMapEditor : Editor
    {
       /// <summary>
        /// Holds the location of the mouse hit location
        /// </summary>
        private Vector3 mouseHitPos;
		private List<Coordinate> blockList = new List<Coordinate>();


        /// <summary>
        /// Lets the Editor handle an event in the scene view.
        /// </summary>
        private void OnSceneGUI()
        {
			// get reference to the TileMap component
            var map = (TileMap)this.target;
			
            // if UpdateHitPosition return true we should update the scene views so that the marker will update in real time
            if (this.UpdateHitPosition())
            {
                SceneView.RepaintAll();
            }

            // Calculate the location of the marker based on the location of the mouse
            this.RecalculateMarkerPosition();

            // get a reference to the current event
            Event current = Event.current;

            // if the mouse is positioned over the layer allow drawing actions to occur
            if (this.IsMouseOnLayer())
            {
				
                // if mouse down or mouse drag event occurred
                if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
                {
                    if (current.button == 1)
                    {
                        // if right mouse button is pressed then we erase blocks
                        this.Erase();
                        current.Use();
                    }
                    else if (current.button == 0)
                    {
                        // if left mouse button is pressed then we draw blocks
                        this.Draw();
                        current.Use();
                    }
                }
            }
			
			
			if(map.clearMap){
				clearMap();	
			}
			
			if(map.writeToFile){
				create_WriteArray();
			}
			
			if(map.loadFile){
				clearMap();
				LoadFromFile();
			}
				

            // draw a UI tip in scene view informing user how to draw & erase tiles
            Handles.BeginGUI();
            GUI.Label(new Rect(10, Screen.height - 90, 100, 100), "LMB: Draw");
            GUI.Label(new Rect(10, Screen.height - 105, 100, 100), "RMB: Erase");
            Handles.EndGUI();
        }
		
		private void create_WriteArray(){
			
			// get reference to the TileMap component
            var map = (TileMap)this.target;
			
			
			map.levelCounter++;
			StreamWriter writer = new StreamWriter("level"+map.levelCounter+".txt");
			
			string myArray;
			
			int[,] levelArray = new int[map.Rows,map.Columns];
			
			Debug.Log("Writing to file: level"+map.levelCounter+".txt");
			
			for(int row = 0; row < map.Rows; row++){
				for(int column = 0; column < map.Columns; column++){
					levelArray[row,column] = 0;
				}
			}
			
			for(int i = 0; i < blockList.Count; ++i){
				levelArray[blockList[i].row, blockList[i].column] = 1;
			}
			
			//writer.WriteLine("Number of Rows:");
			myArray = map.Rows.ToString();
			writer.WriteLine(myArray);
			
			//writer.WriteLine("Number of Columns:");
			myArray = map.Columns.ToString();
			writer.WriteLine(myArray);
			
			//writer.WriteLine("Tile Width:");
			myArray = map.TileWidth.ToString();
			writer.WriteLine(myArray);
			
			//writer.WriteLine("Tile Height:");
			myArray = map.TileHeight.ToString();
			writer.WriteLine(myArray);
			
		
			for(int row = 0; row < map.Rows; row++){
				myArray = "{";
				
				for(int column = 0; column < map.Columns; column++){
					myArray += levelArray[row,column].ToString() + ", ";
				}
				if(row != map.Rows -1){
					myArray += "}, ";
					writer.WriteLine(myArray);
				}
				
				else{
					myArray += "}};";
					writer.WriteLine(myArray);
				}
			}
			
			writer.Close();
			map.writeToFile = false;
			
			Debug.Log("Writing complete");
			
		}
		
		private void LoadFromFile(){
			// get reference to the TileMap component
            var map = (TileMap)this.target;
			
			StreamReader reader = new StreamReader("level"+map.levelCounter+".txt");
			Debug.Log("loading: level"+map.levelCounter+".txt");
			string line;
			string element = "-1";
			int row;
			int column;
			int widthSize;
			int heightSize;
			int counter = 0;
			int numElement = -1;
			
			//get row
			line = reader.ReadLine();
			row = int.Parse(line);
			
			//get column
			line = reader.ReadLine();
			column = int.Parse(line);
			
			//get width of cell
			line = reader.ReadLine();
			widthSize = int.Parse(line);
			
			//get height of cell
			line = reader.ReadLine();
			heightSize = int.Parse(line);
			
			map.Rows = row;
			map.Columns = column;
			map.TileWidth = widthSize;
			map.TileHeight = heightSize;
			
			
			for(int r = 0; r < row; r++){
				line = reader.ReadLine();
				
				for(int l = 0; l < line.Length; l+=3){
					if(l+3 < line.Length){
						element = line.Substring(l, 3);
					}
					
					element = element.Replace(",", "");
					element = element.Replace("{", "");
					element = element.Replace("}", "");
					element = element.Replace(";", "");
					bool res = int.TryParse(element, out numElement);
					if(res){
						Check_CreateBlock(numElement, r, counter);
						//Debug.Log(counter +" "+ numElement);
						counter++;
					}

				}
				counter = 0;
			}
			
			reader.Close();
			
			map.loadFile = false;
			Debug.Log("Loading Completed");
		}
		
		void Check_CreateBlock(int element, int row, int column){
			if(element == 1){
				// get reference to the TileMap component
            	var map = (TileMap)this.target;
			
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);	
				
				var tilePositionInLocalSpace = new Vector3((column * map.TileWidth) + (map.TileWidth / 2), (row* -map.TileHeight) + (-map.TileHeight / 2));
				
            	cube.transform.position = map.transform.position + tilePositionInLocalSpace;

            	// we scale the cube to the tile size defined by the TileMap.TileWidth and TileMap.TileHeight fields 
            	cube.transform.localScale = new Vector3(map.TileWidth, map.TileHeight, 1);

	            // set the cubes parent to the game object for organizational purposes
	            cube.transform.parent = map.transform;
	
	            // give the cube a name that represents it's location within the tile map
	            cube.name = string.Format("Tile_{0}_{1}", column, row);
				
				Coordinate blockLoc = new Coordinate((int) row, (int) column);
				
				this.blockList.Add(blockLoc);
			}
		}
		

        /// <summary>
        /// When the <see cref="GameObject"/> is selected set the current tool to the view tool.
        /// </summary>
        private void OnEnable()
        {
            Tools.current = Tool.View;
            Tools.viewTool = ViewTool.FPS;
        }
		
		/// <summary>
		/// Clears the map and the array of blocks
		/// </summary>
		private void clearMap(){
			// get reference to the TileMap component
            var map = (TileMap)this.target;	
			
			int childs = map.transform.childCount;

            for (int i = childs - 1; i >= 0; i--)
                GameObject.DestroyImmediate(map.transform.GetChild(i).gameObject);
			
			this.blockList.Clear();
			
			map.clearMap = false;
		}

        /// <summary>
        /// Draws a block at the pre-calculated mouse hit position
        /// </summary>
        private void Draw()
        {
            // get reference to the TileMap component
            var map = (TileMap)this.target;

            // Calculate the position of the mouse over the tile layer
            var tilePos = this.GetTilePositionFromMouseLocation();

            // Given the tile position check to see if a tile has already been created at that location
            var cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // if there is already a tile present and it is not a child of the game object we can just exit.
            if (cube != null && cube.transform.parent != map.transform)
            {
                return;
            }

            // if no game object was found we will create a cube
            if (cube == null)
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            // set the cubes position on the tile map
            var tilePositionInLocalSpace = new Vector3((tilePos.x * map.TileWidth) + (map.TileWidth / 2), (tilePos.y * -map.TileHeight) + (-map.TileHeight / 2));
            cube.transform.position = map.transform.position + tilePositionInLocalSpace;

            // we scale the cube to the tile size defined by the TileMap.TileWidth and TileMap.TileHeight fields 
            cube.transform.localScale = new Vector3(map.TileWidth, map.TileHeight, 1);

            // set the cubes parent to the game object for organizational purposes
            cube.transform.parent = map.transform;

            // give the cube a name that represents it's location within the tile map
            cube.name = string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y);
			
			Coordinate blockLoc = new Coordinate((int) tilePos.y, (int) tilePos.x);
			
			this.blockList.Add(blockLoc);
        }

        /// <summary>
        /// Erases a block at the pre-calculated mouse hit position
        /// </summary>
        private void Erase()
        {
            // get reference to the TileMap component
            var map = (TileMap)this.target;

            // Calculate the position of the mouse over the tile layer
            var tilePos = this.GetTilePositionFromMouseLocation();

            // Given the tile position check to see if a tile has already been created at that location
            var cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // if a game object was found with the same name and it is a child we just destroy it immediately
            if (cube != null && cube.transform.parent == map.transform)
            {
                UnityEngine.Object.DestroyImmediate(cube);
            }
			
			Coordinate blockLoc = new Coordinate((int) tilePos.y, (int) tilePos.x);
			
			for(int i = 0; i < blockList.Count; ++i){
				if(blockList[i].row == blockLoc.row &&
					blockList[i].column == blockLoc.column)
					blockList.RemoveAt(i);
			}

        }

        /// <summary>
        /// Calculates the location in tile coordinates (Column/Row) of the mouse position
        /// </summary>
        /// <returns>Returns a <see cref="Vector2"/> type representing the Column and Row where the mouse of positioned over.</returns>
        private Vector2 GetTilePositionFromMouseLocation()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;
			
			
			//Vector3 mouseHitPosTemp = mouseHitPos + map.transform.position;
			//Debug.Log(map.transform.position);
			
            // calculate column and row location from mouse hit location
            var pos = new Vector3(mouseHitPos.x / map.TileWidth, mouseHitPos.y / -map.TileHeight, map.transform.position.z);

            // round the numbers to the nearest whole number using 5 decimal place precision
            pos = new Vector3((int)Math.Round(pos.x, 5, MidpointRounding.ToEven), (int)Math.Round(pos.y, 5, MidpointRounding.ToEven), 0);

            // do a check to ensure that the row and column are with the bounds of the tile map
            var col = (int)pos.x;
            var row = (int)pos.y;
            if (row < 0)
            {
                row = 0;
            }

            if (row > map.Rows - 1)
            {
                row = map.Rows - 1;
            }

            if (col < 0)
            {
                col = 0;
            }

            if (col > map.Columns - 1)
            {
                col = map.Columns - 1;
            }
			
			//Debug.Log(new Vector2(col, row));
            // return the column and row values
            return new Vector2(col, row);
        }

        /// <summary>
        /// Returns true or false depending if the mouse is positioned over the tile map.
        /// </summary>
        /// <returns>Will return true if the mouse is positioned over the tile map.</returns>
        private bool IsMouseOnLayer()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;
			
			//Vector3 mouseHitPosTemp = mouseHitPos + map.transform.position;
			//Debug.Log(mouseHitPos);
			
			return this.mouseHitPos.x > 0 && this.mouseHitPos.x < (map.Columns * map.TileWidth) &&
                   this.mouseHitPos.y < 0 && this.mouseHitPos.y > (map.Rows * -map.TileHeight);
			
			
            // return true or false depending if the mouse is positioned over the map
//            return mouseHitPos.x > map.transform.position.x && mouseHitPos.x < mapWidth &&
//                   mouseHitPos.y < map.transform.position.y  && mouseHitPos.y > mapHeight;
			
        }

        /// <summary>
        /// Recalculates the position of the marker based on the location of the mouse pointer.
        /// </summary>
        private void RecalculateMarkerPosition()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;

            // store the tile location (Column/Row) based on the current location of the mouse pointer
            var tilepos = this.GetTilePositionFromMouseLocation();

            // store the tile position in world space
            var pos = new Vector3(tilepos.x * map.TileWidth, tilepos.y * -map.TileHeight, 0);

            // set the TileMap.MarkerPosition value
            map.MarkerPosition = map.transform.position + new Vector3(pos.x + (map.TileWidth / 2), pos.y + (-map.TileHeight / 2), 0);
        }

        /// <summary>
        /// Calculates the position of the mouse over the tile map in local space coordinates.
        /// </summary>
        /// <returns>Returns true if the mouse is over the tile map.</returns>
        private bool UpdateHitPosition()
        {
            // get reference to the tile map component
            var map = (TileMap)this.target;

            // build a plane object that 
            var p = new Plane(map.transform.TransformDirection(Vector3.forward), map.transform.position);

            // build a ray type from the current mouse position
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // stores the hit location
            var hit = new Vector3();

            // stores the distance to the hit location
            float dist;

            // cast a ray to determine what location it intersects with the plane
            if (p.Raycast(ray, out dist))
            {
                // the ray hits the plane so we calculate the hit location in world space
                hit = ray.origin + (ray.direction.normalized * dist);
            }

            // convert the hit location from world space to local space
            var value = map.transform.InverseTransformPoint(hit);

            // if the value is different then the current mouse hit location set the 
            // new mouse hit location and return true indicating a successful hit test
            if (value != this.mouseHitPos)
            {
                this.mouseHitPos = value;
                return true;
            }

            // return false if the hit test failed
            return false;
        }
    } 
} 