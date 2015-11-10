using UnityEngine;
using System.Collections;

public class UserCreationCancel : MonoBehaviour {
	public UIInput player;
	public StoreUserSetup userData;
	
	void OnClick ()
	{
		userData.Clear();
		player.text = player.defaultText;
	}
}
