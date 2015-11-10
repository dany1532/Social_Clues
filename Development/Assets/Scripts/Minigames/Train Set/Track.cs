using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour {
	
	public GameObject track;
	public float time;
	public float distanceToCam;
	public float distCovered;
	public float fracJourney;
	public Vector3 startPos;
	public Vector3 curPos;

	void Awake()
	{
		track = gameObject;
	}
}
