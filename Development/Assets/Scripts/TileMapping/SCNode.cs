using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCNode{
	public SCNode parent;
	public Coordinate tileLocation;
	public List<SCNode> parentHistory;
	public List<SCNode> children;
	public int totalCost;
	
}
