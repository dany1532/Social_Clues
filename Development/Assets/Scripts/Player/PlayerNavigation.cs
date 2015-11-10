using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerNavigation : MonoBehaviour {
	
	public SCTileMap map;
	public float remainingDistance;
	public Camera sceneCamera;
	public bool walking = false;
	public bool cutScene = false;
	public bool goToCharacter = false;
	public float currentAngle = 0f;
	public float walkingSpeed = 10;
	public float runningSpeed = 25;
	float speed;
	public float runningDistance = 45;
	
	public int counter = 1;
	//private float duration = .5f;
	private float threshold = 0.2f;
	
	private PathFinder a_Star;
	private SCNode resultNode;
	
	private static bool searching = false;
	
	private Vector3 destination;
	private Vector3 scEventPosition;
	private Vector3 playerPosition;
	private Vector3 previousPosition;
	private Vector3 mouseHitPos;
	private Vector2 destinationCell;
	
	
	void Start(){
		//Create a_star script 
		a_Star = new PathFinder();
		
		// Find the main camera
		sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		
		map = GameObject.FindGameObjectWithTag("CafeteriaTileMap").GetComponent<SCTileMap>();
		
		Vector2 player2dPos = RecalculatePosition();
		Coordinate newCord = new Coordinate((int) player2dPos.x, (int) player2dPos.y);
		Vector3 cellPos = GetVectorFromTile(newCord);
		cellPos = GetWorldPositionFromTile(cellPos);
		
		transform.position = cellPos;
		
		previousPosition = transform.position;
		speed = walkingSpeed;
	}
	
	void Update(){
		
		if(Input.GetKey(KeyCode.UpArrow)){
			Vector2 playerTilePos;
		
		// Calculate the cell location on the map based on the player's location
		playerTilePos = RecalculatePosition();
			Coordinate wa = new Coordinate((int)playerTilePos.x, (int)playerTilePos.y);
			Vector3 bla = this.GetVectorFromTile(wa);
			bla = this.GetWorldPositionFromTile(bla);
			Debug.Log("Player: "+playerTilePos);
			Debug.Log("Loc: "+bla);
//			Vector3 newPos = transform.position;
//			newPos.y += 1f;
//			transform.position = newPos;
		}

		

		
		if(resultNode != null && counter < resultNode.parentHistory.Count && !searching){
			
			if(this.transform.position != destination){
				//Debug.Log("here?");
				MovePlayer();	
			}
			
			else{
				//Vector2 playerTilePos = RecalculatePosition();
				counter++;
				GetDestination();
			}
		}
		
		else{
			Stop();
		}
	}
	
	
	void LateUpdate(){
		Vector2 playerTilePos;
		
		// Calculate the cell location on the map based on the player's location
		playerTilePos = RecalculatePosition();
		
		//weird bug
//		playerTilePos.x -= 1f;
//		playerTilePos.y -= 1f;
		 
			
		// if mouse down event occurred
        if (LevelCollider.isPressed && !searching && !cutScene)
        {
			CalculateMousePosition();
			// if the mouse is positioned over the layer allow drawing actions to occur
			if(IsMouseOnMap() && !cutScene){
				Stop();
				SearchNewDestinationPosition(playerTilePos);
				//Debug.Log("Current: " + playerTilePos.ToString() + ", Destination: " + destinationCell.ToString());
				Player.instance.interactionBubble.IgnoreNPC();		
			}
			LevelCollider.isPressed = false;
		}

		
		//Hack to so that mouseButtonUp event doesnt override the go to Character event
		
//		else if(!searching && cutScene){
//			SearchNewDestinationPosition(playerTilePos);	
//		}
		
	}
	
	public void Stop(){
		resultNode = null;
		counter = 0;
		remainingDistance = 0f;
		walking = false;
	}
	
	
	public void SetDestination(Vector3 point, bool isCutscene){
		//goToCharacter = isCharacter;
		cutScene = isCutscene;
		scEventPosition = point;
		
		
		
			// Calculate the cell location on the map based on the player's location
		Vector2 playerTilePos = RecalculatePosition();
		
		//weird bug
		playerTilePos.x -= 1f;
		playerTilePos.y -= 1f;
		
		SearchNewDestinationPosition(playerTilePos);
		//Debug.Log("Current: " + playerTilePos.ToString() + ", Destination: " + GetPointTilePosition(point).ToString());
	}
	
	private void SearchNewDestinationPosition(Vector2 playerPos){
		searching = true;
		setPlayerDestination();	
		resultNode = a_Star.PathFind(playerPos,destinationCell, Level1.levelArray);
//		Debug.Log(playerPos);
//		Debug.Log(destinationCell);

		if(resultNode != null){
			resultNode.parentHistory.Add(resultNode);
			GetDestination();
			walking = true;
			
			Vector3 cellPos = GetVectorFromTile(resultNode.tileLocation);
			scEventPosition = GetWorldPositionFromTile(cellPos);
			//MyPrint(resultNode);
		}
		else
			walking = false;
		
		searching = false;
		if (CalculateRemainingDistance() > runningDistance)
			speed = runningSpeed;
		else
			speed = walkingSpeed;
		
		//Debug.Log(resultNode.parentHistory[0].tileLocation.row);
		//Debug.Log(resultNode.parentHistory[0].tileLocation.column);
	}
	
	private void MovePlayer(){
		//t += Time.deltaTime/duration;
		//t = Mathf.Clamp(t, 0, 1);
		
		 float step = speed * Time.deltaTime;
		
        transform.position = Vector3.MoveTowards(transform.position, destination, step);
		
		//this.transform.position = Vector3.Lerp(transform.position, destination, t);	
		
		if (threshold >= (destination - this.transform.position).magnitude)
				transform.position = destination;
		
		CalculateRemainingDistance();
		//remainingDistance = Vector3.Distance(transform.position, scEventPosition); 
		
		
		
	}
	
	private float CalculateRemainingDistance(){
		Vector2 thePlayer;
		Vector2 theDestination;
		
		thePlayer = new Vector2(transform.position.x, transform.position.y);
		theDestination = new Vector2(scEventPosition.x, scEventPosition.y);
		
		remainingDistance = Vector2.Distance(thePlayer, theDestination);
		return remainingDistance;
	}
	
	private void GetDestination(){
		if(counter < resultNode.parentHistory.Count){
			Vector3 cellPos = GetVectorFromTile(resultNode.parentHistory[counter].tileLocation);
			destination = GetWorldPositionFromTile(cellPos);
			
			Vector3 nextPosition = destination - previousPosition;
			float whichDirection = Vector3.Cross(transform.right, nextPosition).y;
			
			if(whichDirection > 0){
				nextPosition = previousPosition - destination;
				currentAngle = Vector3.Angle(transform.right, nextPosition) + 180f;
			}
			
			else 
				currentAngle = Vector3.Angle(transform.right, nextPosition);
			
			previousPosition = destination;
		}
		
		//				currentPosition = transform.position - previousPosition;
//				whichDirection = Vector3.Cross(transform.right, currentPosition).y;
//				
//				if(whichDirection > 0){
//					currentPosition = previousPosition - transform.position;
//					currentAngle = Vector3.Angle(transform.right, currentPosition) + 180f;	
//				}
//				else 
//					currentAngle = Vector3.Angle(transform.right, currentPosition);
	}
	
	public float GetCurrentAngle(){
		return currentAngle;	
	}
	
	private Vector3 GetWorldPositionFromTile(Vector3 cell){
		
		float rowWorld = cell.x * -map.TileHeight + (-map.TileHeight/2);
		float columnWorld = cell.y * map.TileWidth + (map.TileWidth/2);
		
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
			Vector3 cellPos = GetVectorFromTile(resultNode.parentHistory[i].tileLocation);
			
			Debug.Log(result.parentHistory[i].tileLocation.row + "," + result.parentHistory[i].tileLocation.column);
			Debug.Log("World: "+GetWorldPositionFromTile(cellPos));
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
		if(!cutScene && !goToCharacter)
			destinationCell = GetMouseTilePosition();
		
		else{
			destinationCell = GetEventTilePosition();
			destinationCell.x -=1;
			destinationCell.y +=1;
		}
		//Debug.Log(destinationPos);
	}
	
	private Vector2 GetPointTilePosition(Vector3 point){
		
		Vector3 pointMapPos = point - map.transform.position;
		
		 // calculate column and row location from player's location
		Vector3 pos = new Vector3((pointMapPos.x) / map.TileWidth, (pointMapPos.y) / -map.TileHeight, map.transform.position.z);
		
		return GetTileLocation(pos);
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
	
	private Vector2 GetEventTilePosition(){
		Vector3 eventMapPos = this.scEventPosition - map.transform.position;
		
		Vector3 pos = new Vector3((eventMapPos.x) / map.TileWidth, (eventMapPos.y) / -map.TileHeight, map.transform.position.z);
		
		//Debug.Log(GetTileLocation(pos));
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
	
//	void OnPress(bool onPress)
//	{
//		if (onPress)
//		{
//			Player.instance.Navigate(UICamera.lastHit);
//		}
//	}

	public float Distance (Vector3 position)
	{
		return Vector2.Distance(GetPlayerTilePosition(), GetPointTilePosition(position));
	}
}
