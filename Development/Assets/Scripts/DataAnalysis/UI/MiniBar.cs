using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniBar : MonoBehaviour {

	public List<UISprite> todayBars     = new List<UISprite>();
	public List<UISprite> lastPlayBars  = new List<UISprite>();
	public List<UISprite> aggregateBars = new List<UISprite>();
	
	public List<GameObject> percentageLabels = new List<GameObject>();	
	
	public int indexOfNPC;
	
	// Use this for initialization
	void Start () {
		if (AnalyticsController.Instance.todayPercentages != null && AnalyticsController.Instance.todayPercentages[indexOfNPC] != null)
		{
			for(int i = 0; i < todayBars.Count; ++i) {
				if (i < AnalyticsController.Instance.todayPercentages[indexOfNPC].Count)
					todayBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.todayPercentages[indexOfNPC][i] / 100;
				else
					todayBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		else
		{
			for(int i = 0; i < todayBars.Count; ++i) {
				todayBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		
		if (AnalyticsController.Instance.lastPlayPercentages != null && AnalyticsController.Instance.lastPlayPercentages[indexOfNPC] != null)
		{
			// set values for last play
			for(int i = 0; i < lastPlayBars.Count; ++i) {
				if (i < AnalyticsController.Instance.lastPlayPercentages[indexOfNPC].Count)
					lastPlayBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.lastPlayPercentages[indexOfNPC][i] / 100;
				else
					lastPlayBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		else
		{
			// set values for last play
			for(int i = 0; i < lastPlayBars.Count; ++i) {
				lastPlayBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		
		if (AnalyticsController.Instance.averagePercentages != null && AnalyticsController.Instance.averagePercentages[indexOfNPC] != null)
		{
			// set values for aggregate
			for(int i = 0; i < aggregateBars.Count; ++i) {
				if (i < AnalyticsController.Instance.averagePercentages[indexOfNPC].Count)
					//aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.totalPercentages[indexOfNPC][i] / 100;
					aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.averagePercentages[indexOfNPC][i] / 100;
				else
					aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		else
		{
			// set values for aggregate
			for(int i = 0; i < aggregateBars.Count; ++i) {
				//aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = AnalyticsController.Instance.totalPercentages[indexOfNPC][i] / 100;
				aggregateBars[i].GetComponent<UIStretch>().relativeSize.y = 0;
			}
		}
		
		if (AnalyticsController.Instance.todayPercentages != null && AnalyticsController.Instance.todayPercentages[indexOfNPC] != null)
		{
			for(int i = 0; i < percentageLabels.Count; ++i) {
				if (i < AnalyticsController.Instance.todayPercentages[indexOfNPC].Count)
				{
					percentageLabels[i].GetComponentInChildren<UILabel>().text = Mathf.Round(AnalyticsController.Instance.todayPercentages[indexOfNPC][i]) + "%";
					percentageLabels[i].GetComponent<UIAnchor>().relativeOffset.y = (AnalyticsController.Instance.todayPercentages[indexOfNPC][i] / 100) + 0.025f;
				}
				else
				{
					percentageLabels[i].GetComponentInChildren<UILabel>().text = "-";
					percentageLabels[i].GetComponent<UIAnchor>().relativeOffset.y = 0.025f;
				}
			}
		}
		else
		{
			for(int i = 0; i < percentageLabels.Count; ++i) {
				percentageLabels[i].GetComponentInChildren<UILabel>().text = "-";
				percentageLabels[i].GetComponent<UIAnchor>().relativeOffset.y = 0.025f;
			}
		}
	}
}
