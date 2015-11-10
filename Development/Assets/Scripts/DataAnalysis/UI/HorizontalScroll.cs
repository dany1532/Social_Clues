using UnityEngine;
using System.Collections;

public class HorizontalScroll : MonoBehaviour {
	public enum Direction { LEFT, RIGHT }

	public int numberOfNPCs;
	private int currentNPCIndex;

	public GameObject leftButton;
	public GameObject rightButton;

	public GameObject npcInfoContainer;

	private const int PANEL_OFFSET = 320;

	private GameObject[] npcs;

	void Awake()
	{
		//instance = this;
	}
	// Use this for initialization
	void Start () {
		numberOfNPCs = AnalyticsController.Instance.numberOfNPCs;
		setInitialState();
		delayedInitialize ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void delayedInitialize() {
		Invoke ("initialize", 0.1f);
	}
	public void initialize() {
		npcs = GameObject.FindGameObjectsWithTag("NPCInfo");
		if(npcs.Length > 3) {
			for(int i = 3; i < npcs.Length; ++i) {
				npcs[i].SetActive (false);
			}
		}
	}

	public void setInitialState() {
		if(numberOfNPCs <= 3) {
			setButtonState (leftButton, false);
			setButtonState (rightButton, false);
		} else {
			currentNPCIndex = 0;
			setButtonState (leftButton, false);
			setButtonState (rightButton, true);
		}
	}

	public void setButtonState(GameObject gameObject, bool state) {
		gameObject.SetActive(state);
	}

	public void scroll(Direction direction) {
		if(direction == Direction.LEFT) {
			--currentNPCIndex;
			// hide NPCs on the right
			npcs[currentNPCIndex + 3].SetActive(false);
			// show NPCs on the left
			npcs[currentNPCIndex].SetActive(true);

			if(currentNPCIndex == 0) {
				setButtonState (leftButton, false);
			}
			setButtonState (rightButton, true);
			npcInfoContainer.transform.localPosition = new Vector3(npcInfoContainer.transform.localPosition.x + PANEL_OFFSET, npcInfoContainer.transform.localPosition.y, npcInfoContainer.transform.localPosition.z);
		} else {
			++currentNPCIndex;
			// hide NPCs on the left
			npcs[currentNPCIndex - 1].SetActive(false);
			// show NPCs on the right
			npcs[currentNPCIndex + 2].SetActive(true);

			// check if there are no more NPCs left on the right
			if(currentNPCIndex + 3 >= numberOfNPCs) {
				setButtonState (rightButton, false);
			}
			setButtonState(leftButton, true);
			npcInfoContainer.transform.localPosition = new Vector3(npcInfoContainer.transform.localPosition.x - PANEL_OFFSET, npcInfoContainer.transform.localPosition.y, npcInfoContainer.transform.localPosition.z);

		}
	}
}
