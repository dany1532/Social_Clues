using UnityEngine;
using System.Collections;

public class Lego : MonoBehaviour {
	public NPC npc;

	void OnClick() {
		if (Vector3.Distance(Player.instance.transform.position, transform.position) < 7)
		{
			npc.OnQuestCompleted();
			this.gameObject.SetActive(false);
		}
	}
}
