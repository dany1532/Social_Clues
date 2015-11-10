using UnityEngine;
using System.Collections;

public class NPCDetailObjectClicked : MonoBehaviour {
	public AchievementManager am;
	// Use this for initialization
	void OnClick()
	{
		am.zoomOut();
	}
}
