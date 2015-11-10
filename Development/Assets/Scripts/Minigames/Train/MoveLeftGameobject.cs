using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveLeftGameobject : MonoBehaviour {
	
	private bool go = false;
	public GameObject train = null;
	public float speed = 1.0f;
	
	public Transform startMarker;
    public Transform tracksParent;
    private float startTime;
    private float journeyLength;
	public List<Transform> tracks;
	private int currentTrack;
	// Use this for initialization
	void Start () {
	
	}
	
	void OnMouseDown()
	{
		tracks = new List<Transform>();
		currentTrack = 0;
		foreach(Transform child in tracksParent)
	    {
			tracks.Add(child.gameObject.transform);
	        //Something(child.gameObject);
	    }
     	go = true;
		startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker.position, tracks[currentTrack].position);
    }
	
	// Update is called once per frame
	void Update () {
	if(go & train !=null){
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered/journeyLength;
        //Vector3 pos = Vector3.Lerp(startMarker.position, tracks[currentTrack].position, fracJourney);
		train.transform.position = Vector3.Lerp(startMarker.position, tracks[currentTrack].position, fracJourney);
		//Vector3 dir = train.rigidbody.MoveRotation(;
		//train.transform.forward  = new Vector3(0,dir.y,0);
		//Vector3 dir = (tracks[currentTrack].position - train.transform.position);
		//dir.Normalize();
		//train.transform.rigidbody.AddForce(dir * 10);
			
		if(train.transform.position == tracks[currentTrack].position)
			{
				startMarker = tracks[currentTrack];
				if(currentTrack == tracks.Count - 1)
					currentTrack = 0;
				else
					currentTrack++;
				startTime = Time.time;
				journeyLength = Vector3.Distance(startMarker.position, tracks[currentTrack].position);
				

			}
    }
	}
}
