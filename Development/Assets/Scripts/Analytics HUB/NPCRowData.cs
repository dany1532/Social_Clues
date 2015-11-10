using UnityEngine;
using System.Collections;

public class NPCRowData : MonoBehaviour {
	public UILabel e, c, i, m, p;
	public DBTotalPercentageWithDate percentages;

	public UILabel date;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void populateRow() {
		e.text = percentages.e.ToString() + "%";
		c.text = percentages.c.ToString() + "%";
		i.text = percentages.i.ToString() + "%";
		m.text = percentages.m.ToString() + "%";
		p.text = percentages.p.ToString() + "%";
		string dateText = string.Format("{0:MM/dd/yy}", percentages.date);
		date.text = dateText;

		AnalyticsHUBController.setColor(e, percentages.e);
		AnalyticsHUBController.setColor(c, percentages.c);
		AnalyticsHUBController.setColor(i, percentages.i);
		AnalyticsHUBController.setColor(m, percentages.m);
		AnalyticsHUBController.setColor(p, percentages.p);
	}
}
