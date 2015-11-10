using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour {

	void OnPress(bool press)
	{
		if (!press) {
			ApplicationState.Instance.selectedCharacter = gameObject.name;
			MainDatabase.Instance.UpdateUserCharacter(ApplicationState.Instance.userID, gameObject.name);
		}
	}
}
