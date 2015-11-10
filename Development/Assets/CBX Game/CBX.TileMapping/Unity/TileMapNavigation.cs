namespace CBX.TileMapping.Unity{

	using UnityEngine;
	
	using System.Collections;
	using System;
	



	public class TileMapNavigation : MonoBehaviour {
		/// <summary>
		/// Reference to the tileMap
		/// </summary>
		public TileMap tileMap;
		
		public Camera sceneCamera;
		
		 /// <summary>
        /// Holds the location of the mouse hit location
        /// </summary>
        private Vector3 mouseHitPos;
		
		
			
		void LateUpdate (){
			/// Calculate the cell location on the map based on the location of the mouse
			this.RecalculatePosition();
			
			// get a reference to the current event
            //Event current = Event.current;
			
            // if the mouse is positioned over the layer allow drawing actions to occur
            if (this.IsMouseOnMap())
            {
				// if mouse down or mouse drag event occurred
                if (Input.GetMouseButtonUp(0))
                {
					this.Draw();
                }
				
				
               /* // if mouse down or mouse drag event occurred
                if (current != null && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
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
                }*/
            }
		}
		
		 /// <summary>
        /// Draws a block at the pre-calculated mouse hit position
        /// </summary>
        private void Draw()
        {
            // Calculate the position of the mouse over the tile layer
            var tilePos = this.GetTilePositionFromMouseLocation();

            // Given the tile position check to see if a tile has already been created at that location
            var cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // if there is already a tile present and it is not a child of the game object we can just exit.
            if (cube != null && cube.transform.parent != tileMap.transform)
            {
                return;
            }

            // if no game object was found we will create a cube
            if (cube == null)
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            // set the cubes position on the tile map
            var tilePositionInLocalSpace = new Vector3((tilePos.x * tileMap.TileWidth) + (tileMap.TileWidth / 2), (tilePos.y * -tileMap.TileHeight) + (-tileMap.TileHeight / 2));
			
            cube.transform.position = tileMap.transform.position + tilePositionInLocalSpace;

            // we scale the cube to the tile size defined by the TileMap.TileWidth and TileMap.TileHeight fields 
            cube.transform.localScale = new Vector3(tileMap.TileWidth, tileMap.TileHeight, 1);

            // set the cubes parent to the game object for organizational purposes
            cube.transform.parent = tileMap.transform;

            // give the cube a name that represents it's location within the tile map
            cube.name = string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y);
        }
		
		void RecalculatePosition(){
            // store the tile location (Column/Row) based on the current location of the mouse pointer
            var tilepos = this.GetTilePositionFromMouseLocation();

            // store the tile position in world space
            var pos = new Vector3(tilepos.x * tileMap.TileWidth, tilepos.y * tileMap.TileHeight, 0);

            // set the TileMap.MarkerPosition value
            tileMap.MarkerPosition = tileMap.transform.position + new Vector3(pos.x + (tileMap.TileWidth / 2), pos.y + (tileMap.TileHeight / 2), 0);	
		}
		
		 /// <summary>
        /// Returns true or false depending if the mouse is positioned over the tile map.
        /// </summary>
        /// <returns>Will return true if the mouse is positioned over the tile map.</returns>
        private bool IsMouseOnMap()
        {
			float mapWidth = (tileMap.Columns * tileMap.TileWidth) + transform.position.x;
			float mapHeight = (tileMap.Rows * tileMap.TileHeight) - transform.position.y;
			
			
            // return true or false depending if the mouse is positioned over the map
            return mouseHitPos.x > transform.position.x && mouseHitPos.x < mapWidth &&
                   mouseHitPos.y < transform.position.y && mouseHitPos.y > mapHeight;
        }
		
		 /// <summary>
        /// Erases a block at the pre-calculated mouse hit position
        /// </summary>
        private void Erase()
        {

            // Calculate the position of the mouse over the tile layer
            var tilePos = this.GetTilePositionFromMouseLocation();

            // Given the tile position check to see if a tile has already been created at that location
            var cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // if a game object was found with the same name and it is a child we just destroy it immediately
            if (cube != null && cube.transform.parent == tileMap.transform)
            {
                UnityEngine.Object.DestroyImmediate(cube);
            }
        }
		
		 /// <summary>
        /// Calculates the location in tile coordinates (Column/Row) of the mouse position
        /// </summary>
        /// <returns>Returns a <see cref="Vector2"/> type representing the Column and Row where the mouse of positioned over.</returns>
        private Vector2 GetTilePositionFromMouseLocation()
        {
			Vector3 screenPoint = Input.mousePosition;
			
			mouseHitPos = sceneCamera.ScreenToWorldPoint(screenPoint);
			
			Vector3 mouseHitPosTemp = mouseHitPos - transform.position;

			
			//Debug.Log(this.mouseHitPos - transform.position);
			//mouseHitPos = sceneCamera.ScreenToWorldPoint(screenPoint);
			//mouseHitPos.x -= tileMap.TileWidth;
			//mouseHitPos.y -= tileMap.TileHeight;
			
            // calculate column and row location from mouse hit location
            var pos = new Vector3((mouseHitPosTemp.x) / tileMap.TileWidth, (mouseHitPosTemp.y) / -tileMap.TileHeight, tileMap.transform.position.z);

            // round the numbers to the nearest whole number using 5 decimal place precision
            pos = new Vector3((int)Math.Round(pos.x, 5, MidpointRounding.ToEven), (int)Math.Round(pos.y, 5, MidpointRounding.ToEven), 0);

            // do a check to ensure that the row and column are with the bounds of the tile map
            var col = (int)pos.x;
            var row = (int)pos.y;
			
			
            if (row < 0)
            {
                row = 0;
            }

            if (row > tileMap.Rows - 1)
            {
                row = tileMap.Rows - 1;
            }

            if (col < 0)
            {
                col = 0;
            }

            if (col > tileMap.Columns - 1)
            {
                col = tileMap.Columns - 1;
            }
	

            // return the column and row values
            return new Vector2(col, row);
        }
		
		
	}
	
}
