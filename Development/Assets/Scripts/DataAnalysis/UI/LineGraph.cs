using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineGraph : MonoBehaviour {
	private float sizeOfOneHundred = 0.25f;
	private float panelSpaceTop = 121.45f;
	private float panelSpaceBottom = 34.36f;
	private float panelSpaceDifference;
	private float screenSpaceLeftX = 0f;
	private float screenSpaceRightX = 1.305f;
	private LineRenderer lineRenderer;
	
	public int numberOfVertices;
	public List<Vector3> linePositions = new List<Vector3>();
	public enum LineType {Today, Last, Aggregate};
	public LineType type;
	/*
	private List<float> aggregateTodayPercentages;
	private List<float> aggregateLastPlayPercentages;
	private List<float> aggregateTotalPercentages;
	*/
	public List<UILabel> percentageLabels;
	
	// Use this for initialization
	void Start () {
		panelSpaceDifference = panelSpaceTop - panelSpaceBottom;
		/*
		aggregateTodayPercentages    = new List<float>();
		aggregateLastPlayPercentages = new List<float>();
		aggregateTotalPercentages    = new List<float>();
		
		for(int i = 0; i < numberOfVertices; ++i) {
			aggregateTodayPercentages.Add(0);
			aggregateLastPlayPercentages.Add(0);
			aggregateTotalPercentages.Add(0);
		}
		
		// prepare aggregate data
		for(int i = 0; i < numberOfVertices; ++i) {
			for(int j = 0; j < AnalyticsController.Instance.numberOfNPCs; ++j) {
				// sum all the values
				if(AnalyticsController.Instance.todayPercentages[j][i] >= 0) {
					aggregateTodayPercentages[i]    += AnalyticsController.Instance.todayPercentages[j][i];
					aggregateLastPlayPercentages[i] += AnalyticsController.Instance.lastPlayPercentages[j][i];
					aggregateTotalPercentages[i]    += (AnalyticsController.Instance.todayPercentages[j][i] + AnalyticsController.Instance.lastPlayPercentages[j][i]) / 2;
				}
			}
			// divide to get the average
			aggregateTodayPercentages[i]    /= (AnalyticsController.Instance.numberOfNPCs - AnalyticsController.Instance.communicationMissing[i]);
			aggregateLastPlayPercentages[i] /= (AnalyticsController.Instance.numberOfNPCs - AnalyticsController.Instance.communicationMissing[i]);
			aggregateTotalPercentages[i]    /= (AnalyticsController.Instance.numberOfNPCs - AnalyticsController.Instance.communicationMissing[i]);
		}
		*/
		
		lineRenderer = this.GetComponent<LineRenderer>();

		numberOfVertices = 0;
		for(int i = 0; i < 5; ++i) {
			if(AnalyticsController.Instance.communicationMissing[i] < AnalyticsController.Instance.numberOfNPCs) {
				numberOfVertices++;
			}
		}		

		lineRenderer.SetVertexCount(numberOfVertices);
		float spaceBetweenVertices = (screenSpaceRightX - screenSpaceLeftX) / (numberOfVertices - 1);
		int vertexNumber = 0;

		for(int i = 0; i < 5; ++i) {
			if(AnalyticsController.Instance.communicationMissing[i] < AnalyticsController.Instance.numberOfNPCs) {
				switch(type) {
					case LineType.Today:
						lineRenderer.SetPosition(vertexNumber, new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.aggregateTodayPercentages[i] / 100f), 0));
						break;
					case LineType.Last:
						lineRenderer.SetPosition(vertexNumber, new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.aggregateLastPlayPercentages[i] / 100f), 0));
						break;
					case LineType.Aggregate:
						lineRenderer.SetPosition(vertexNumber, new Vector3(vertexNumber * spaceBetweenVertices, sizeOfOneHundred * (AnalyticsController.Instance.aggregateTotalPercentages[i] / 100f), 0));
						percentageLabels[i].transform.localPosition = new Vector3(percentageLabels[i].transform.localPosition.x, panelSpaceBottom + (panelSpaceDifference * AnalyticsController.Instance.aggregateTodayPercentages[i] / 100f), percentageLabels[i].transform.localPosition.z);	
						percentageLabels[i].text = Mathf.Round(AnalyticsController.Instance.aggregateTodayPercentages[i]).ToString() + "%";	
						break;
				}
				
				++vertexNumber;
			}
		}		
/*		
		if(type == LineType.Today) {
			for(int i = 0; i < numberOfVertices; ++i) {
				//lineRenderer.SetPosition(i, new Vector3(linePositions[i].x, sizeOfOneHundred * (aggregateTodayPercentages[i] / 100f), 2f));
				lineRenderer.SetPosition(i, new Vector3(linePositions[i].x, sizeOfOneHundred * (AnalyticsController.Instance.aggregateTodayPercentages[i] / 100f), 2f));
			}
		} else if(type == LineType.Last) {
			for(int i = 0; i < numberOfVertices; ++i) {
				//lineRenderer.SetPosition(i, new Vector3(linePositions[i].x, sizeOfOneHundred * (aggregateLastPlayPercentages[i] / 100f), 2f));
				lineRenderer.SetPosition(i, new Vector3(linePositions[i].x, sizeOfOneHundred * (AnalyticsController.Instance.aggregateLastPlayPercentages[i] / 100f), 2f));
			}			
		} else if(type == LineType.Aggregate) {
			for(int i = 0; i < numberOfVertices; ++i) {
				//lineRenderer.SetPosition(i, new Vector3(linePositions[i].x, sizeOfOneHundred * (aggregateTotalPercentages[i] / 100f), 2f));
				lineRenderer.SetPosition(i, new Vector3(linePositions[i].x, sizeOfOneHundred * (AnalyticsController.Instance.aggregateTotalPercentages[i] / 100f), 2f));
				// prepare % labels
				//percentageLabels[i].transform.localPosition = new Vector3(percentageLabels[i].transform.localPosition.x, percentageLabels[i].transform.localPosition.y, percentageLabels[i].transform.localPosition.z);
				//percentageLabels[i].transform.localPosition = new Vector3(percentageLabels[i].transform.localPosition.x, panelSpaceTop, percentageLabels[i].transform.localPosition.z);
				//percentageLabels[i].transform.localPosition = new Vector3(percentageLabels[i].transform.localPosition.x, panelSpaceBottom + (panelSpaceDifference * aggregateTodayPercentages[i] / 100f), percentageLabels[i].transform.localPosition.z);
				percentageLabels[i].transform.localPosition = new Vector3(percentageLabels[i].transform.localPosition.x, panelSpaceBottom + (panelSpaceDifference * AnalyticsController.Instance.aggregateTodayPercentages[i] / 100f), percentageLabels[i].transform.localPosition.z);
				//percentageLabels[i].text = Mathf.Round(aggregateTodayPercentages[i]).ToString() + "%";
				percentageLabels[i].text = Mathf.Round(AnalyticsController.Instance.aggregateTodayPercentages[i]).ToString() + "%";
			}
		}
*/		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
