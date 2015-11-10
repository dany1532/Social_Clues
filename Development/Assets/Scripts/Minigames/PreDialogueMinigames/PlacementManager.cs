using UnityEngine;
using System.Collections;

public class PlacementManager : MonoBehaviour {

	public PreDialogueMinigame minigame;
	public Position position;
	public UIScaledTexture target;

	void Awake()
	{
		minigame = GetComponent<PreDialogueMinigame> ();
	}
	
	void OnEnable()
	{
		GameObject playerPlacementPrefab = ResourceManager.LoadObject ("Player/" + ApplicationState.Instance.selectedCharacter + "Placement");
		PlacementPlayerPreafb placementPlayer = playerPlacementPrefab.GetComponent<PlacementPlayerPreafb> ();
		position.TargetOutlineTexture = placementPlayer.outline;
		position.TargetFilledTexture = placementPlayer.filled;
		target.SetTexture(position.TargetOutlineTexture, Color.white);
		Resources.UnloadUnusedAssets();
        
        this.transform.localScale = Vector3.one * Player.instance.interactingNPC.playerHeight;
	}
}
