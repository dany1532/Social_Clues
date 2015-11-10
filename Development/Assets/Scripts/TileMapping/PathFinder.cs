using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class PathFinder{
	//Level Array
	int[,] table;
	
	//Nodes used for
	SCNode currentNode;
	SCNode child;
	SCNode otherChild;
	List<SCNode> open_Q; 
	List<SCNode> closed_Q; 
	List<SCNode> children_Q; 
	Coordinate goalState;
	const int basicDirCost = 10;
	const int diagonalCost = 14;
	
	class Comparer:IComparer<SCNode>
  	{
   #region Implementation of IComparer<in Foo>

   public int Compare(SCNode node1, SCNode node2)
   {
		//node1 should have greater priority
    	if(node1.totalCost < node2.totalCost) return -1;
			
		else return 1;
   }

   #endregion
  }
	
	public SCNode PathFind(Vector2 startPt, Vector2 endPt, int[,] level){
		currentNode = null;
		child = null;
		otherChild = null;
		open_Q = new List<SCNode>();
		closed_Q = new List<SCNode>();
		children_Q = new List<SCNode>();
		
		table = level;
		Coordinate start = new Coordinate((int) startPt.x, (int) startPt.y);
		goalState = new Coordinate((int) endPt.x, (int) endPt.y);
		
		currentNode = CreateRootNode(start);
		
		open_Q.Add(currentNode);
		
		while(true){
			if(open_Q.Count == 0) return null;
			
			currentNode = RemoveFrontOpen();
			
			if(GoalTest(currentNode.tileLocation, goalState)){
				return currentNode;
			}
			
			expandChildren();
			
			while(children_Q.Count != 0){
				child = RemoveFrontChildren();
				
				if(!IsNodeOnOpenQueue(child) && !IsNodeOnClosedQueue(child)){
					open_Q.Add(child);	
				}
				
				else if(IsNodeOnOpenQueue(child)){
					if(IsChildLessCost_Open(child)){
						open_Q.Remove(otherChild);
						open_Q.Add(child);
					}
				}
				
				else if(IsNodeOnClosedQueue(child)){
					if(IsChildLessCost_Closed(child)){
						closed_Q.Remove(otherChild);
						open_Q.Add(child);
					}
				}
			}//end of children
			
			closed_Q.Add(currentNode);
			SortOpenQueue();
		}//end while	
	}
	
	private void SortOpenQueue(){
		open_Q.Sort(new Comparer());
	}
	
	private void expandChildren(){
		//Basic Direction children
		if(CanGoLeft(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(basicDirCost, GetLeft(currentNode.tileLocation));
			}
		}
		
		if(CanGoUp(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(basicDirCost, GetUp(currentNode.tileLocation));
			}
		}
		
		
		if(CanGoRight(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(basicDirCost, GetRight(currentNode.tileLocation));
			}
		}
		
		if(CanGoDown(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(basicDirCost, GetDown(currentNode.tileLocation));
			}
		}
		
		//Diagonal Direction children
		if(CanGoNorthRight(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(diagonalCost, GetNorthRight(currentNode.tileLocation));
			}
		}
		
		if(CanGoNorthLeft(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(diagonalCost, GetNorthLeft(currentNode.tileLocation));
			}
		}
		
		if(CanGoSouthRight(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(diagonalCost, GetSouthRight(currentNode.tileLocation));
			}
		}
		
		if(CanGoSouthLeft(currentNode.tileLocation)){
			if(!inParentHistory(currentNode)){
				makeChild(diagonalCost, GetSouthLeft(currentNode.tileLocation));
			}
		}
	}
	
	private bool IsNodeOnOpenQueue(SCNode child){
		for(int i = 0; i < open_Q.Count; i++){
			if(AreLocationEqual(open_Q[i].tileLocation, child.tileLocation))
				return true;
		}
		
		return false;
	}
	
	private bool IsNodeOnClosedQueue(SCNode child){
		for(int i = 0; i < closed_Q.Count; i++){
			if(AreLocationEqual(closed_Q[i].tileLocation, child.tileLocation))
				return true;
		}
		
		return false;
	}
	
	private bool IsChildLessCost_Open(SCNode child){
		for(int i = 0; i < open_Q.Count; i++){
			if(AreLocationEqual(open_Q[i].tileLocation, child.tileLocation)){
				otherChild = open_Q[i];	
				break;
			}
		}
		
		if(child.totalCost < otherChild.totalCost)
			return true;	
		
		else 
			return false;
		
	}
					
					
	private bool IsChildLessCost_Closed(SCNode child){
		for(int i = 0; i < closed_Q.Count; i++){
			if(AreLocationEqual(closed_Q[i].tileLocation, child.tileLocation)){
				otherChild = closed_Q[i];	
				break;
			}
		}
		
		if(child.totalCost < otherChild.totalCost)
			return true;	
		
		else 
			return false;
		
	}
	
	private void makeChild(int cost, Coordinate childLoc){
		SCNode child = new SCNode();
		child.children = new List<SCNode>();
		child.parent = null;
		child.parentHistory = new List<SCNode>();
		child.totalCost = 0;
		child.tileLocation = new Coordinate(-1,-1);
		
		List<SCNode> childHistory = new List<SCNode>(currentNode.parentHistory);
		int heuristicCost = 0;
		child.parent = currentNode;
		child.tileLocation = childLoc;
		
		child.parentHistory = childHistory;
		child.parentHistory.Add(currentNode);
		
		//heuristicCost = HeuristicManhattanMethod(childLoc);
		heuristicCost = HeuristicDiagonalShortcutMethod(childLoc);
		child.totalCost += heuristicCost + cost + currentNode.totalCost;
		
		currentNode.children.Add(child);
		children_Q.Add(child);
		
	}
	
	private bool GoalTest(Coordinate current, Coordinate goal){
		
		return AreLocationEqual(current, goal);
	}
	
	private bool inParentHistory(SCNode node){
		if(node.parentHistory.Count == 0) return false;
		
		else{
			for(int	i = 0; i < node.parentHistory.Count; i++){
				if(AreLocationEqual(node.parentHistory[i].tileLocation, node.tileLocation))
					return true;
			}
			
			return false;
		}
	}
	
	private bool AreLocationEqual(Coordinate c1, Coordinate c2){
		if(c1.column == c2.column && c1.row == c2.row) return true;
		else return false;
	}
	
	private SCNode RemoveFrontOpen(){
		SCNode node = new SCNode();
		node = open_Q[0];
		open_Q.RemoveAt(0);
		return node;
	}
	
	private SCNode RemoveFrontChildren(){
		SCNode node = new SCNode();
		node = children_Q[0];
		children_Q.RemoveAt(0);
		return node;
	}
	
	private SCNode CreateRootNode(Coordinate loc){
		SCNode rootNode = new SCNode();
		rootNode.tileLocation = loc;
		rootNode.totalCost = 0;
		rootNode.parent = null;
		rootNode.parentHistory = new List<SCNode>();
		rootNode.children = new List<SCNode>();
		return rootNode;
	}
	
	
	
	//Basic Direction
	public Coordinate GetLeft(Coordinate loc){
		Coordinate leftLoc = new Coordinate(loc.row,loc.column);
		leftLoc.column -= 1;
		
		return leftLoc;
	}
	
	private bool CanGoLeft(Coordinate loc){
		if(loc.column > 0 && table[loc.row,loc.column-1] != 1) return true;
		else return false;
	}
	
	public Coordinate GetUp(Coordinate loc){
		Coordinate upLoc = new Coordinate(loc.row,loc.column);
		upLoc.row -= 1;
		
		return upLoc;
	}
	
	private bool CanGoUp(Coordinate loc){
		if(loc.row > 0 && table[loc.row-1,loc.column] != 1) return true;
		else return false;
	}
	
	public Coordinate GetDown(Coordinate loc){
		Coordinate downLoc = new Coordinate(loc.row,loc.column);
		downLoc.row += 1;
		
		return downLoc;
	}
	
	private bool CanGoDown(Coordinate loc){
		if(loc.row < table.GetUpperBound(0)  && table[loc.row+1,loc.column] != 1) return true;
		else return false;
	}
	
	public Coordinate GetRight(Coordinate loc){
		Coordinate rightLoc = new Coordinate(loc.row,loc.column);
		rightLoc.column += 1;
		
		return rightLoc;
	}
	
	private bool CanGoRight(Coordinate loc){
		if(loc.column < table.GetUpperBound(1) && table[loc.row,loc.column+1] != 1) return true;
		else return false;
	}
	
	//Diaonal
	private Coordinate GetNorthRight(Coordinate loc){
		Coordinate northRightLoc = new Coordinate(loc.row, loc.column);
		northRightLoc.row -= 1;
		northRightLoc.column += 1;
		
		return northRightLoc;
	}
	
	private bool CanGoNorthRight(Coordinate loc){
		
		if(CanGoUp(loc) && CanGoRight(loc)){
			if(table[loc.row-1,loc.column+1] != 1) return true;
		}
		
		return false;
	}
	
	private int HeuristicManhattanMethod(Coordinate loc){
		int H;

		H =  10*(Math.Abs(loc.column - goalState.column) 
			     + Math.Abs(loc.row - goalState.row));
		return H;
	}
	
	private int HeuristicDiagonalShortcutMethod(Coordinate loc){
		int H;
		int xDistance = Math.Abs(loc.column - goalState.column);
		int yDistance = Math.Abs(loc.row - goalState.row);
		
		if(xDistance > yDistance)
			H = (14 * yDistance) + (10 * (xDistance -yDistance));
		
		else
			H = (14 * xDistance) + (10 * (yDistance - xDistance));
		
		return H;
	
	}
	

	
	private Coordinate GetNorthLeft(Coordinate loc){
		Coordinate northLeftLoc = new Coordinate(loc.row, loc.column);
		northLeftLoc.row -= 1;
		northLeftLoc.column -= 1;
		
		return northLeftLoc;
	}
	
	private bool CanGoNorthLeft(Coordinate loc){
		
		if(CanGoUp(loc) && CanGoLeft(loc)){
			if(table[loc.row-1, loc.column-1] != 1) return true;
		}
		
		return false;
	}
	
	private Coordinate GetSouthRight(Coordinate loc){
		Coordinate southRightLoc = new Coordinate(loc.row, loc.column);
		southRightLoc.row += 1;
		southRightLoc.column += 1;
		
		return southRightLoc;
	}
	
	private bool CanGoSouthRight(Coordinate loc){
		if(CanGoDown(loc) && CanGoRight(loc)){
			if(table[loc.row+1, loc.column+1] != 1) return true;
		}
		
		return false;
	}
	
	private Coordinate GetSouthLeft(Coordinate loc){
		Coordinate southLeftLoc = new Coordinate(loc.row, loc.column);
		southLeftLoc.row += 1;
		southLeftLoc.column -= 1;
		
		return southLeftLoc;
	}
	
	private bool CanGoSouthLeft(Coordinate loc){
		
		if(CanGoDown(loc) && CanGoLeft(loc)){
			if(table[loc.row+1, loc.column-1] != 1) return true;	
		}
		
		return false;
	}
	
}
