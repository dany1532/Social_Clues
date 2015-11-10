using UnityEngine;
using System.Collections;

public class SCTileMap : MonoBehaviour {
	/// <summary>
    /// Gets or sets the number of rows of tiles.
    /// </summary>
  	public int Rows = Level1.rows;
	
    /// <summary>
    /// Gets or sets the number of columns of tiles.
    /// </summary>
    public int Columns = Level1.columns;

    /// <summary>
    /// Gets or sets the value of the tile width.
   /// </summary>
    public float TileWidth = Level1.tileWidth;

    /// <summary>
    /// Gets or sets the value of the tile height.
    /// </summary>
    public float TileHeight = Level1.tileHeight;
	
	private Vector3 markerPos = new Vector3();
	
	//private Level1 cafeteriaLevel;
	
	/// <summary>
    /// When the game object is selected this will draw the grid
    /// </summary>
    /// <remarks>Only called when in the Unity editor.</remarks>
    private void OnDrawGizmosSelected()
    {
        // store map width, height and position
        var mapWidth = this.Columns * this.TileWidth;
        var mapHeight = this.Rows * this.TileHeight;
        var position = this.transform.position;
		Vector3 tilePos;
		//Vector3 markerPos;

        // draw layer border
        Gizmos.color = Color.white;
		
		//upper line
        Gizmos.DrawLine(position, position + new Vector3(mapWidth, 0, 0));
		
		//left line
        Gizmos.DrawLine(position, position - new Vector3(0, mapHeight, 0));
		
		//right
        Gizmos.DrawLine(position + new Vector3(mapWidth, 0, 0), position + new Vector3(mapWidth, -mapHeight, 0));
		
		//bottom line
        Gizmos.DrawLine(position - new Vector3(0, mapHeight, 0), position + new Vector3(mapWidth, -mapHeight, 0));

        // draw tile cells
        Gizmos.color = Color.grey;
		
        for (float i = 1; i < this.Columns; i++)
        {
            Gizmos.DrawLine(position + new Vector3(i * this.TileWidth, 0, 0), position + new Vector3(i * this.TileWidth, -mapHeight, 0));
        }
        
        for (float i = 1; i < this.Rows; i++)
        {
            Gizmos.DrawLine(position + new Vector3(0, i * -this.TileHeight, 0), position + new Vector3(mapWidth, i * -this.TileHeight, 0));
        } 
		
		// Draw marker position
        Gizmos.color = Color.red; 
		
		for(int row = 0; row < Level1.rows; row++){
			for(int column = 0; column < Level1.columns; column++){
				if(Level1.levelArray[row, column] == 1){
					
					tilePos = new Vector3((column * TileWidth) + (TileWidth / 2), (row* -TileHeight) + (-TileHeight / 2));
					this.markerPos = position + new Vector3(tilePos.x, tilePos.y, 0);
					Gizmos.DrawWireCube(this.markerPos, new Vector3(this.TileWidth, -this.TileHeight, 1) * 1.1f);
//					tilePos = new Vector3(column * this.TileWidth, row * -this.TileHeight,0);
//					this.markerPos = position + new Vector3(tilePos.x + (this.TileWidth / 2), tilePos.y + (this.TileHeight / 2), 0);
//					Gizmos.DrawWireCube(this.markerPos, new Vector3(this.TileWidth, -this.TileHeight, 1) * 1.1f);
				}
			}
		}
	}


}
