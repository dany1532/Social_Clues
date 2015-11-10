using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tilingTrainTracks : MonoBehaviour {

	public List<Track> tracks;

	public GameObject track1;
	public GameObject track2;
	public GameObject track3;
	public GameObject track4;
	public GameObject track5;
	public GameObject track6;
	//public GameObject cow;
	public GameObject camera3D;
	public float trackSpeed; 
	public float startTime;
	public float time1;
	public float time2;
	public float time3;
	public float time4;
	public float time5;
	public float time6;
	public float track1ToCamDistance;
	public float track2ToCamDistance;
	public float track3ToCamDistance;
	public float track4ToCamDistance;
	public float track5ToCamDistance;
	public float track6ToCamDistance;
	public float distCovered1; 
	public float distCovered2;
	public float distCovered3; 
	public float distCovered4;
	public float distCovered5;
	public float distCovered6;
	public float fracJourney;
	public float fracJourney2;
	public float fracJourney3;
	public float fracJourney4;
	public float fracJourney5;
	public float fracJourney6;

	public float track2z; 
	//public float cowX; 
	//public float cowY; 

	public Vector3 track1StartPos; 
	public Vector3 track2StartPos;
	public Vector3 track3StartPos; 
	public Vector3 track4StartPos;
	public Vector3 track5StartPos;
	public Vector3 track6StartPos;
	public Vector3 track1CurrPos; 
	public Vector3 track2CurrPos;
	public Vector3 track3CurrPos; 
	public Vector3 track4CurrPos;
	public Vector3 track5CurrPos;
	public Vector3 track6CurrPos;
	public Vector3 cameraPos;
	//public Vector3 cowStartPos; 



	// Use this for initialization
	void Start () {
		startTime = Time.time; 
		cameraPos = camera3D.transform.position; 

		//cowStartPos = cow.transform.position; 
		foreach (Track track in tracks) {
			track.startPos = track.track.transform.position;
			track.distanceToCam = Vector3.Distance(track.startPos, cameraPos);
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Track track in tracks) {
			track.time += Time.deltaTime;
			track.distCovered = (track.time - startTime) * trackSpeed;
			track.fracJourney = track.distCovered / track.distanceToCam;

			track.track.transform.position = track.curPos;
			track.curPos =  Vector3.Lerp(track.startPos, cameraPos, track.fracJourney);
		}

		foreach (Track track in tracks) {
			if (track.curPos == cameraPos)
			{
				Vector3 startPos = tracks[tracks.Count - 1].startPos;
				track.curPos = startPos;
				track.startPos = startPos;
				track.distanceToCam = Vector3.Distance(startPos, cameraPos);
				track.time = 0;
				break;
			}
		}
	}
}
