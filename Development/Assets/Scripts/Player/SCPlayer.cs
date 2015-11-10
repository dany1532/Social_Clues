using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayer : MonoBehaviour {
	public SCTileMap map;
	private Vector3 playerPosition;
	public Vector3 mouseHitPos;
	private Vector2 destinationPos;
	public Camera sceneCamera;
	private PathFinder a_Star;
	private static bool searching = false;
	private SCNode resultNode;
	private Vector3 destination;
	
	
	private int counter = 0;
	private float t = 0f;
	private float duration = .5f;
	private float threshold = 0.1f;
	
	
	void Start(){
		a_Star = new PathFinder();
	}
	
	void Update(){
		
		if(Input.GetKey(KeyCode.UpArrow)){
			Vector3 newPos = transform.position;
			newPos.y += 1f;
			transform.position = newPos;
		}
		
		if(Input.GetKey(KeyCode.DownArrow)){
			Vector3 newPos = transform.position;
			newPos.y -= 1f;
			transform.position = newPos;
		}
		
		if(Input.GetKey(KeyCode.LeftArrow)){
			Vector3 newPos = transform.position;
			newPos.x -= 1f;
			transform.position = newPos;
		}
		
		if(Input.GetKey(KeyCode.RightArrow)){
			Vector3 newPos = transform.position;
			newPos.x += 1f;
			transform.position = newPos;
		}
		
		if(resultNode != null && counter < resultNode.parentHistory.Count && !searching){
			if(this.transform.position != destination){
				MovePlayer();	
			}
			
			else{
				//Vector2 playerTilePos = RecalculatePosition();
				counter++;
				t = 0f;
				GetDestination();
			}
		}
		
		else{
			resultNode = null;
			counter = 0;
			t = 0f;
			
		}
	}
	
	
	void LateUpdate(){
		Vector2 playerTilePos;
		
		// Calculate the cell location on the map based on the player's location
		playerTilePos = RecalculatePosition();
		      
		// if mouse down or mouse drag event occurred
        if (Input.GetMouseButtonUp(0) && !searching)
        {
			CalculateMousePosition();
			// if the mouse is positioned over the layer allow drawing actions to occur
			if(IsMouseOnMap()){
				searching = true;
				setPlayerDestination();	
				t = 0f;
				resultNode = a_Star.PathFind(playerTilePos,destinationPos, Level1.levelArray);
				
		
				if(resultNode != null){
					resultNode.parentHistory.Add(resultNode);
					GetDestination();
					//MyPrint(resultNode);
				}
				searching = false;
				//Debug.Log(resultNode.parentHistory[0].tileLocation.row);
				//Debug.Log(resultNode.parentHistory[0].tileLocation.column);
				
			}
		}
		
	}
	
	private void MovePlayer(){
		t += Time.deltaTime/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		this.transform.position = Vector3.Lerp(transform.position, destination, t);	
		
		if (threshold >= (destination - this.transform.position).magnitude)
				transform.position = destination;
		
	}
	
	private void GetDestination(){
		if(counter < resultNode.parentHistory.Count){
			Vector3 cellPos = GetVectorFromTile(resultNode.parentHistory[counter].tileLocation);
			destination = GetWorldPositionFromTile(cellPos);
		}
	}
	
	private Vector3 GetWorldPositionFromTile(Vector3 cell){
		
		float rowWorld = cell.x * -map.TileHeight + (-map.TileHeight);
		float columnWorld = cell.y * map.TileWidth + (map.TileWidth);
		
		Vector3 newCell = new Vector3(columnWorld,rowWorld,cell.z);
		
		Vector3 cellWorldPos = map.transform.position + newCell;
		
		cellWorldPos.z = this.transform.position.z;
		
		return cellWorldPos;
		
		
	}
	
	private Vector3 GetVectorFromTile(Coordinate cord){
		float row = cord.row;
		float column = cord.column;
		float z = this.transform.position.z;
		
		return new Vector3(row, column, z);
	}
	
	private void MyPrint(SCNode result){
		for(int i = 0; i < result.parentHistory.Count; i++){
			Debug.Log(result.parentHistory[i].tileLocation.row + "," + result.parentHistory[i].tileLocation.column);
		}
	}
	private Vector2 RecalculatePosition(){
		Vector2 tilePos = GetPlayerTilePosition();
		return tilePos;
		//Debug.Log(tilePos);
	}
	
	private void CalculateMousePosition(){
		Vector3 screenPoint = Input.mousePosition;
			
		mouseHitPos = sceneCamera.ScreenToWorldPoint(screenPoint);
			
	}
	
	/// <summary>
    /// Returns true or false depending if the mouse is positioned over the tile map.
    /// </summary>
    /// <returns>Will return true if the mouse is positioned over the tile map.</returns>
    private bool IsMouseOnMap()
    {
		float mapWidth = (map.Columns * map.TileWidth) + map.transform.position.x;
		float mapHeight =  map.transform.position.y - (map.Rows * map.TileHeight);
		
        // return true or false depending if the mouse is positioned over the map
        return mouseHitPos.x > map.transform.position.x && mouseHitPos.x < mapWidth &&
               mouseHitPos.y < map.transform.position.y && mouseHitPos.y > mapHeight;
    }
	
	private void setPlayerDestination(){
		destinationPos = GetMouseTilePosition();
		//Debug.Log(destinationPos);
	}
	
	private Vector2 GetPlayerTilePosition(){
		
		Vector3 playerMapPos = this.transform.position - map.transform.position;
		
		 // calculate column and row location from player's location
		Vector3 pos = new Vector3((playerMapPos.x) / map.TileWidth, (playerMapPos.y) / -map.TileHeight, map.transform.position.z);
		
		return GetTileLocation(pos);
	}
	
	private Vector2 GetMouseTilePosition(){
		
		Vector3 mouseMapPos = mouseHitPos - map.transform.position;
		
		Vector3 pos = new Vector3((mouseMapPos.x) / map.TileWidth, (mouseMapPos.y) / -map.TileHeight, map.transform.position.z);
		
		//Debug.Log("Mouse " + GetTileLocation(pos));
		
		return GetTileLocation(pos);
			
	}
	
	private Vector2 GetTileLocation(Vector3 position){
		Vector3 pos = position;
		
		
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


        // return the column and row values
        return new Vector2(row, col);
	}
	
}

