using UnityEngine;
using System.Collections;

public class EyeContactManager : MonoBehaviour {
 
    public EyeContact arrowControl;
    public PreDialogueMinigame minigame;
    public GameObject playerEyeContact;
    public Dialogue ending;
    
    void Awake()
    {
			minigame = GetComponent<PreDialogueMinigame> ();
	}
    
	void OnEnable()
	{
		GameObject playerEyeContactPrefab = ResourceManager.LoadObject ("Player/" + ApplicationState.Instance.selectedCharacter + "EyeContact");
        playerEyeContact = Instantiate(playerEyeContactPrefab) as GameObject;
        playerEyeContact.transform.parent = DialogueWindow.instance.playerParentTransform;
		playerEyeContact.transform.localPosition = playerEyeContactPrefab.transform.position;
        playerEyeContact.transform.localScale = playerEyeContactPrefab.transform.localScale;
        Resources.UnloadUnusedAssets();

        arrowControl = playerEyeContact.GetComponentInChildren<EyeContact>();
        arrowControl.minigame = minigame;
        arrowControl.ending = GameObject.Find("Ending").GetComponent<Dialogue>();
    }

	void OnDisable()
	{
		if (playerEyeContact != null)
			Destroy (playerEyeContact);
	}
}
