using UnityEngine;
using System.Collections;

public class potraitClicked : MonoBehaviour {

	//Automatically gets assigned from LoadNPCsPortraits
	public AchievementManager manager;

	void OnClick ()
	{
		if (manager != null)
			manager.zoomIn(this.gameObject);//, clicked);
	}
}
