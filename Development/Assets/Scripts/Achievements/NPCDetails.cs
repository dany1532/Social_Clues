using UnityEngine;
using System.Collections;

public class NPCDetails : MonoBehaviour {

	public GameObject cutscene;
	public GameObject lessonText;
	public GameObject NPCLabel;
	private GameObject star;
	private GameObject starsCompleted;
	public AudioClip lesson;
	public AudioClip lessonQuestion;
	
	public int totalStars = 20;
	// Use this for initialization
	void Start () {
		cutscene.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
		lessonText.transform.localPosition = new Vector3(0.1375f, -0.44f, lessonText.transform.localPosition.z);
		NPCLabel.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		NPCLabel.transform.localPosition = new Vector3(-0.25f, 0.471f, NPCLabel.transform.localPosition.z);

		star = GameObject.Find ("Star");
		star.GetComponent<UISprite>().type = UISprite.Type.Filled;
		star.GetComponent<UISprite>().fillDirection = UISprite.FillDirection.Vertical;

		int userid = ApplicationState.Instance.userID;
		if(ApplicationState.Instance.userID == -1)
			userid = 1;


		int npcid = MainDatabase.Instance.getIDs("select npcid from npc where npcname = '"+ gameObject.name+"';");

		int noOfStars = MainDatabase.Instance.GetStars(userid,npcid);
		Debug.Log(npcid+" "+noOfStars+" "+gameObject.name);
		GameObject.Find("StarsCompleted").GetComponent<UILabel>().text = noOfStars.ToString();

		if(noOfStars >= 20)
			noOfStars = 20;
		star.GetComponent<UISprite>().fillAmount = noOfStars/(totalStars*1.0f);
	}
}

// P: 0.245, 0.177, -1.55601
// S: 0.295, 0.26, 0.2456747