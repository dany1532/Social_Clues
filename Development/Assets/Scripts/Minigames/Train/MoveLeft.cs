using UnityEngine;
using System.Collections;

public class MoveLeft : MonoBehaviour {
	
	private bool go = false;
	private bool starburstShow = false;
	public GameObject train = null;
	public GameObject myStarburst;
	public float speed = 1.0f;
	public GameObject selectArrow;
	public GameObject[] myCars;
	
	public Transform startMarker;
    public Transform endMarker;
    private float startTime;
    private float journeyLength;

	Color grey1;
	Color grey2;

	// Use this for initialization
	void Start () {
		myStarburst.SetActive(false);
		grey1 = GameObject.Find ("Engine").GetComponent<UISprite>().color;
		grey2 = GameObject.Find ("CoalCar").GetComponent<UISprite>().color;
	}
	
	void OnClick ()
    {
		if(starburstShow == true)
		{
			go = true;
			if(selectArrow!=null)
			{
				selectArrow.SetActive(false);
			}

			startTime = Time.time;
        	journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
			StartCoroutine("ToNextPart");
		}
    }
	
	// Update is called once per frame
	void Update () 
	{
		if(go & train !=null){
	        float distCovered = (Time.time - startTime) * speed;
	        float fracJourney = distCovered/journeyLength;
	        train.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
	    }

	}

	IEnumerator ToNextPart()
	{
		yield return new WaitForSeconds(3.0f);
		//Debug.Log("To Next Level");
		Application.LoadLevel("trainSet");
	}

	public void carUpdated()
	{
		starburstShow = true;
		foreach(GameObject car in myCars)
		{
			if((car.GetComponent<UISprite>().color == grey1) || (car.GetComponent<UISprite>().color == grey2))
			{
				starburstShow=false;
			}
		}

		if(starburstShow)
		{
			gameObject.GetComponent<UISprite>().spriteName = "Play Button";
			myStarburst.SetActive(true);
		}

	}
}
