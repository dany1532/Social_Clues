using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniLineGraph : MonoBehaviour {
	private float sizeOfOneHundred = 120f;
	private float panelSpaceTopY = 60f;
	private float panelSpaceBottomY = -60f;
	private float panelSpaceDifference;
	private float screenSpaceLeftX = 0f;
	private float screenSpaceRightX = 249f;
	private LineRenderer lineRenderer;
	public Material lineMaterial;
	public int numberOfVertices;
	//public List<Vector3> linePositions = new List<Vector3>();
	public enum LineType {Today, Last, Aggregate};
	public LineType type;

	//public List<UILabel> percentageLabels;
	
	public int indexOfNPC;
	
	private float lineWidth = 0.015f;
	
	// Use this for initialization
	void Start () {
	
		panelSpaceDifference = panelSpaceTopY - panelSpaceBottomY;
		
		lineRenderer = this.GetComponent<LineRenderer>();
		
		numberOfVertices = 0;
		for(int i = 0; i < 5; ++i) {
			if(AnalyticsController.Instance.isCommunicationAvailable[indexOfNPC][i]) {
				numberOfVertices++;
			}
		}
		
		lineRenderer.SetVertexCount(numberOfVertices);
		float spaceBetweenVertices = (screenSpaceRightX - screenSpaceLeftX) / (numberOfVertices - 1);
		int vertexNumber = 0;
		
		Vector3 lastPosition = Vector3.zero;
		bool lastPositionInitialized = false; 	// use this to track whether or not lastPosition has been initialized -- won't always be for i = 0
		for(int i = 0; i < 5; ++i) {
			if(AnalyticsController.Instance.isCommunicationAvailable[indexOfNPC][i]) {
				Vector3 vertexPosition;
				switch(type) {
					case LineType.Today:
						vertexPosition = new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.todayPercentages[indexOfNPC][i] / 100f), 0);
						lineRenderer.SetPosition(vertexNumber, vertexPosition); 	
						
						if(!lastPositionInitialized) {
							lastPosition = vertexPosition;
							lastPositionInitialized = true;
						}
						//lineRenderer.SetPosition(vertexNumber, new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.todayPercentages[indexOfNPC][i] / 100f), 0));
						
						createLine (i, lastPosition, vertexPosition, Color.red, "A Mini Line Today ");
						lastPosition = vertexPosition;
						break;
					case LineType.Last:
						vertexPosition = new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][i] / 100f), 0);
						lineRenderer.SetPosition(vertexNumber, vertexPosition); 	
						
						if(!lastPositionInitialized) {
							lastPosition = vertexPosition;
							lastPositionInitialized = true;
						}						
						//lineRenderer.SetPosition(vertexNumber, new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][i] / 100f), 0));
						
						createLine (i, lastPosition, vertexPosition, Color.blue, "B Mini Line Last Play ");
						lastPosition = vertexPosition;
						break;
					case LineType.Aggregate:
						vertexPosition = new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * ((AnalyticsController.Instance.todayPercentages[indexOfNPC][i] + AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][i]) / 2 / 100f), 0);
						lineRenderer.SetPosition(vertexNumber, vertexPosition); 	
						
						if(!lastPositionInitialized) {
							lastPosition = vertexPosition;
							lastPositionInitialized = true;
						}						
						//lineRenderer.SetPosition(vertexNumber, new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * ((AnalyticsController.Instance.todayPercentages[indexOfNPC][i] + AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][i]) / 2 / 100f), 0));
						createLine (i, lastPosition, vertexPosition, Color.black, "C Mini Line Aggregate ");
						lastPosition = vertexPosition;					
						
						break;
				}
				
				++vertexNumber;
			}
		}
	}
	
	void createLine(int i, Vector3 startVector, Vector3 endVector, Color color, string prefix) {
		GameObject newLine = new GameObject();
		newLine.transform.parent = this.transform.parent;
		newLine.transform.localPosition = this.transform.localPosition;
		newLine.transform.localScale = Vector3.one;
		newLine.name = prefix + i;
		LineRenderer newLineRenderer;
		newLineRenderer = newLine.AddComponent("LineRenderer") as LineRenderer;
		newLineRenderer.gameObject.layer = 20;
		newLineRenderer.castShadows = false;
		newLineRenderer.receiveShadows = false;					
		newLineRenderer.SetVertexCount(2);
		newLineRenderer.SetColors(color, color);
		newLineRenderer.SetWidth(lineWidth, lineWidth);
		newLineRenderer.useWorldSpace = false;
		
		newLineRenderer.SetPosition(0, startVector);
		newLineRenderer.SetPosition(1, endVector);
		
		newLineRenderer.material = lineMaterial;	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
