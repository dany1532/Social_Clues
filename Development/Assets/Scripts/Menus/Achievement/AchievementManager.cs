using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour {

	public GameObject achievementGO;

	public float zoomTime = 5.0f;
	public float zoomInFOV = 8.3f;
	public float fadeOutTime = 2.0f;
	
	public GameObject npcDetail;
	public GameObject prefabParent;
	public GameObject leftArrow;
	public GameObject rightArrow;
	public List<Collider> frameCollider;
	//public List<GameObject> prefabs;
	public PlaySoundFX lessonVoiceOver;
	public AchievementsLessonQuestion lessonQuestion;

	GameObject currentNPC;
	Vector3 npcLocation;
	Vector3 orgPos;
	float orgFOV;
	Dictionary<string,int> prefabIndex = new Dictionary<string,int>();
	
	private float elapsedLERP;
	private float timeOfLERP;

	private enum CameraState {
		IDLE,
		ZOOM_IN,
		ZOOM_OUT
	}
	private CameraState cameraState;

	void Start()
	{
		prefabIndex.Add("Amy",0);
		prefabIndex.Add("Max",1);
		prefabIndex.Add("Jake",2);
		prefabIndex.Add("Eddie",3);
		prefabIndex.Add("Nancy",4);
		prefabIndex.Add("Dad",5);
		prefabIndex.Add("Dora",6);
		prefabIndex.Add("Gaby",7);
		prefabIndex.Add("Sam",8);
		orgFOV = camera.fieldOfView;
		orgPos = camera.gameObject.transform.localPosition;
		elapsedLERP = 0.0f;
		timeOfLERP = zoomTime;
		cameraState = CameraState.IDLE;
	}

	public void zoomIn(GameObject go)
	{
		if(camera)
		{
			TweenAlpha.Begin(npcDetail,fadeOutTime,0);
			currentNPC = go;
			Vector3 npcframe = go.transform.parent.transform.parent.localPosition;
			npcLocation = go.transform.parent.transform.parent.position;
			Vector3 newXYPos = new Vector3(npcframe.x,npcframe.y,camera.gameObject.transform.localPosition.z);
			TweenPosition.Begin(camera.gameObject, zoomTime, newXYPos).onFinished += FinishZoomIn;
			//FinishZoomIn();

			switchToNPCDetails();

			TweenFOV.Begin(camera.gameObject,zoomTime,zoomInFOV);
			//Invoke ("FinishZoomIn", zoomTime);

			//TweenScale.OnFinished();
		}
	}

	void Update() {
		if(cameraState == CameraState.ZOOM_IN) {
			elapsedLERP += Time.deltaTime;
			camera.orthographicSize = Mathf.Lerp(1, 0.17f, elapsedLERP / timeOfLERP);
		} else if(cameraState == CameraState.ZOOM_OUT) {
			elapsedLERP += Time.deltaTime;
			camera.orthographicSize = Mathf.Lerp(0.17f, 1, elapsedLERP / timeOfLERP);
		}
	}

	private void cameraToIdle() {
		cameraState = CameraState.IDLE;
		elapsedLERP = 0;
	}

	private void switchToNPCDetails() {
		//camera.orthographicSize = 0.17f;

		if(camera.orthographicSize == 1) {
			cameraState = CameraState.ZOOM_IN;
			Invoke("cameraToIdle", zoomTime);
		}
		//Mathf.Lerp();
		npcDetail.transform.position = new Vector3(npcLocation.x,npcLocation.y,npcDetail.transform.localPosition.z);
		CreateNPCDetails();
		TweenAlpha.Begin(npcDetail, fadeOutTime, 1);
		togglenArrows();
	}

	private void FinishZoomIn(UITweener tween)
	//private void FinishZoomIn()
	{
		//npcDetail.transform.position = new Vector3(npcLocation.x,npcLocation.y,npcDetail.transform.localPosition.z);
		//CreateNPCDetails();
		//TweenAlpha.Begin(npcDetail,fadeOutTime,1);
		//togglenArrows();
	}

	private void switchToAllNPCs() {
		cameraState = CameraState.ZOOM_OUT;
		Invoke("cameraToIdle", zoomTime);
	}

	public void zoomOut()
	{
		if(camera)
		{
			TweenAlpha.Begin(npcDetail,fadeOutTime,0);
			Vector3 newXYPos = orgPos;
			TweenPosition.Begin(camera.gameObject, zoomTime, newXYPos);
			TweenFOV.Begin(camera.gameObject,zoomTime,orgFOV);

			switchToAllNPCs();

			//TweenScale.OnFinished();
		}
	}

	void togglenArrows()
	{
		int currentNPCno = int.Parse(currentNPC.transform.parent.transform.parent.name);
		Debug.Log(currentNPCno);
		if(currentNPCno == 0 || currentNPCno == 5)
		{
			leftArrow.SetActive(false);
		}
		else if(frameCollider[currentNPCno-1].enabled == true)
		{
			leftArrow.SetActive(true);
		}
		else
		{
			leftArrow.SetActive(false);
		}

		if((currentNPCno == frameCollider.Count -1) || currentNPCno == 4)
		{
			rightArrow.SetActive(false);
		}
		else if(frameCollider[currentNPCno+1].enabled == true)
		{
			rightArrow.SetActive(true);
		}
		else
		{
			rightArrow.SetActive(false);
		}
	}

	void CreateNPCDetails()
	{
		//destroying previous prefabs
		foreach(Transform child in prefabParent.transform)
		{
			Destroy(child.gameObject);
		}
		GameObject details = Instantiate(ResourceManager.LoadNPCAchievements(currentNPC.transform.parent.name)) as GameObject;
		details.name = currentNPC.transform.parent.name;
		details.transform.parent = prefabParent.transform;
		details.transform.localPosition = new Vector3 (0,0,0);
		details.transform.localScale = new Vector3(1,1,1);
		NPCDetails npcDetails = details.GetComponent<NPCDetails> ();
		lessonVoiceOver.audioClip = npcDetails.lesson;
		lessonQuestion.audioClip = npcDetails.lessonQuestion;
		lessonQuestion.delay = npcDetails.lesson.length;
	}
	
}

