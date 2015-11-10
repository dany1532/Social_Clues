using UnityEngine;
using System.Collections;

public class AnalyticsQuestionRow : MonoBehaviour {
	public UILabel answerName;
	public UILabel timeTaken;
	public UISprite background;
	public UISprite scoreGradient;
	
	public void SetScale ()
	{
		//answerName.transform.localScale = new Vector3(20, 20, 1);
		answerName.transform.localScale = new Vector3(35, 35, 1);
		//timeTaken.transform.localScale = new Vector3(20, 20, 1);
		timeTaken.transform.localScale = new Vector3(35, 35, 1);
		
		// increase background size if replies take two lines
		if(answerName.numberOfLines > 1) {
			background.transform.localScale = new Vector3(background.transform.localScale.x, 65f, background.transform.localScale.z);
		}
	}
}
