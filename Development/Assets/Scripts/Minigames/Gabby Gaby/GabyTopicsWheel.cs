using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GabyTopicsWheel : MonoBehaviour 
{
	public Camera sceneCamera;
	public List<Texture> listTopics;
	public GameObject topicTemplate;
	//public GameObject topicContainerTemplate;
	public float radius = 1f;
	public float rotationsPerSecond = 3f;
	//Component[] childrenTopics;
	public List<TopicSlotButton> childrenTopics;
	public List<TopicContainer> containers;
	public List<Transform> topicsPositions;
	
	Vector3 prevMouseHitPos;
	Vector3 mouseHitPos;
	//TopicSelector targetContainer;
	TopicSlotButton selectedTopic;
	float correctionSpinSpeed = 0.5f;
	bool canAnimRotate;
	CapsuleCollider capsuleCol;
	
	void Awake()
	{
		sceneCamera = GameObject.FindGameObjectWithTag("MinigameCamera").GetComponent<Camera>();
		capsuleCol = GetComponent<CapsuleCollider>();
		childrenTopics = new List<TopicSlotButton>();
	}
	
	public void InstantiateTopicsInsideBubble()
	{
		Vector3 center = transform.position;
		List<Vector3> listPositions = new List<Vector3>();
		List<int> randomNumbers = new List<int>();
		List<int> randomIndexes = new List<int>();
		int index;
		
		for(int i = 0; i < listTopics.Count; i++)
			randomNumbers.Add(i);
		
		for(int i = 0; i < topicsPositions.Count; i++)
			randomIndexes.Add(i);
		
		DestroyChildren();
		
		for(int i = 0; i < listTopics.Count; ++i)
		{
			GameObject go = AddChild(gameObject, topicTemplate);
			index = Random.Range(0, randomIndexes.Count);
			
			Vector3 newPos = topicsPositions[randomIndexes[index]].position;
			newPos.z = center.z;
			
			go.transform.position = newPos;
			
			randomIndexes.RemoveAt(index);
						
			//listPositions.Add(newPos);
			
			index = Random.Range(0, randomNumbers.Count);
				
			TopicSlotButton topicSlot = go.GetComponent<TopicSlotButton>();
			topicSlot.myIcon.mainTexture = listTopics[randomNumbers[index]];
			
			
			topicSlot.CanUpdate();
				
			go.name = topicSlot.myIcon.mainTexture.name;
			childrenTopics.Add(topicSlot);
			
			randomNumbers.RemoveAt(index);
			
		}
		
//		for(int i = 0; i < listTopics.Count; ++i)
//		{
//			GameObject go = AddChild(this.gameObject, topicTemplate);
//			int index = Random.Range(0, randomNumbers.Count);
//			
//			Vector3 newPos = go.transform.position;
//			
//			do
//			{
//				newPos.x = Random.Range(center.x - ((capsuleCol.height - 0.15f)/2), center.x + ((capsuleCol.height - 0.15f)/2 - 0.1f));
//				newPos.y = Random.Range(center.y - (capsuleCol.radius - 0.1f), center.y + (capsuleCol.radius - 0.1f));
//				newPos.z = center.z;
//				go.transform.position = newPos;
//				
//			}while(CheckOverlapTopics(listPositions, newPos));
//				
//			listPositions.Add(newPos);
//				
//			TopicSlotButton topicSlot = go.GetComponent<TopicSlotButton>();
//			topicSlot.myIcon.mainTexture = listTopics[randomNumbers[index]];
//			
//			
//			topicSlot.CanUpdate();
//				
//			go.name = topicSlot.myIcon.mainTexture.name;
//			childrenTopics.Add(topicSlot);
//			
//			randomNumbers.RemoveAt(index);
//			
//		}
		
		StartCoroutine("displayTopicsAnimation");

		//childrenTopics = GetComponentsInChildren<TopicSlotButton>();
	}
	
	IEnumerator displayTopicsAnimation()
	{
		foreach(TopicSlotButton topic in childrenTopics)
		{
			if (topic == null) break;

			topic.DisplayAnimation();
			yield return new WaitForSeconds(0.8f);

			if (topic.cloudAnim != null)
				topic.cloudAnim.GetComponent<UITexture>().enabled = false;
			if (topic.myIcon != null)
				topic.myIcon.enabled = true;
			//topic.CanUpdate();
		}
		
		
//		foreach(TopicSlotButton topic in childrenTopics)
//		{
//			topic.CanUpdate();
//		}

		
	}
	
	public void Reset()
	{
		//Debug.Log ("Reset animations");
		StopCoroutine ("displayTopicsAnimation");
		listTopics.Clear();	
		childrenTopics.Clear();
		DestroyChildren();
	}
	
	public void DestroyWrongAnswers(List<string> answersList)
	{
		foreach(TopicSlotButton topic in childrenTopics)
		{
			topic.gameObject.SetActive(false);
		}
		
		for (int i = 0; i < answersList.Count; i++){
			foreach(TopicSlotButton topic in childrenTopics)
			{
				if(topic.name == answersList[i])
				{
					topic.gameObject.SetActive(true);
					topic.myContainer.myCheck.enabled = false;
					topic.myContainer.GetComponent<BoxCollider>().enabled = false;
				}
			}
		}
			
	}
	
	public void DestroyChildren()
	{
		var children = new List<GameObject>();
		
		foreach (Transform child in transform) children.Add(child.gameObject);
		
		children.ForEach(child => Destroy(child));
		
		foreach(TopicContainer container in containers)
			container.Reset();
	}
	
//	public void ReturnSelectedTopic()
//	{
//		
//		Vector3 newPos = go.transform.position;
//			
//			do
//			{
//				newPos.x = Random.Range(center.x - ((capsuleCol.height - 0.15f)/2), center.x + ((capsuleCol.height - 0.15f)/2 - 0.1f));
//				newPos.y = Random.Range(center.y - (capsuleCol.radius - 0.1f), center.y + (capsuleCol.radius - 0.1f));
//				newPos.z = center.z;
//				go.transform.position = newPos;
//				
//			}while(CheckOverlapTopics(listPositions, newPos));
//	}
	
	
	public Vector3 FreePos(Vector3 currentPos)
	{
		Vector3 newPos = new Vector3();
		Vector3 center = transform.position;
		newPos = currentPos;
		
		List<Vector3> listPositions = new List<Vector3>();
		
		foreach(TopicSlotButton topic in childrenTopics)
			listPositions.Add(topic.transform.position);
			
			do
			{
				newPos.x = Random.Range(center.x - ((capsuleCol.height - 0.15f)/2), center.x + ((capsuleCol.height - 0.15f)/2 - 0.1f));
				newPos.y = Random.Range(center.y - (capsuleCol.radius - 0.1f), center.y + (capsuleCol.radius - 0.1f));
				newPos.z = center.z;
				
			}while(CheckOverlapTopics(listPositions, newPos));
		
		return newPos;
	}
	
	bool CheckOverlapTopics(List<Vector3> listPos, Vector3 newPos)
	{
		bool isOverlap = false;
		
		if(listPos.Count < 1)
			return false;
		
		foreach(Vector3 pos in listPos)
		{
			float dist = Vector3.Distance(pos, newPos);
			
			if(dist <= 0.2)
			{
				isOverlap = true;
				break;
			}
		}
		
		if(isOverlap)
			return true;
		
		else
			return false;
	}
	
//	public void CreateWheel()
//	{
//		Vector3 center = transform.position;
//		float circleSubdivisions = 360 / listTopics.Count;
//		float angle = 0;
//		
//		var randomNumbers = new List<int>();
//		
//		for(int i = 0; i < listTopics.Count; i++)
//			randomNumbers.Add(i);
//		
//		for(int i = 0; i < listTopics.Count - 3; i++){
//			EliminateRandomNumber(randomNumbers);
//		}
//		
//		randomNumbers.Sort();
//		
//		
//		for(int i = 0; i < listTopics.Count; ++i)
//		{
//			angle += circleSubdivisions;
//			//GameObject go = Instantiate(topicTemplate);
//			GameObject go = AddChild(this.gameObject, topicTemplate);
//			
//			Vector3 newPos = go.transform.position;
//			newPos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
//			newPos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
//			newPos.z = center.z;
//			go.transform.position = newPos;
//			
//			TopicSlotButton topicSlot = go.GetComponent<TopicSlotButton>();
//			topicSlot.myIcon.mainTexture = listTopics[i];
//			
//			go.name = topicSlot.myIcon.mainTexture.name;
//			
//			for(int m = 0; m < randomNumbers.Count; m++){
//				//if(i == randomNumbers[m])
//				if(i == 0)
//				{
//					GameObject container = GameObject.Instantiate(topicContainerTemplate) as GameObject;
//					
//					container.transform.parent = transform.parent;
//					
//					float _radius = radius + 0.2f;
//					
//					Vector3 containerPos = transform.position;
//					
//					/*containerPos.x = center.x + _radius * Mathf.Sin(angle * Mathf.Deg2Rad);
//					containerPos.y = center.y + _radius * Mathf.Cos(angle * Mathf.Deg2Rad);
//					containerPos.z = center.z;*/
//					
//					containerPos.x += _radius;
//					
//					container.transform.position = containerPos;
//					
//					//TopicSlotButton containerSlot = container.GetComponent<TopicSlotButton>();
////					containerSlot.myIcon.enabled = false;
////					containerSlot.myBackground.enabled  = true;
////					containerSlot.myBorder.enabled = true;
////					containerSlot.enabled = false;
////					containerSlot.GetComponent<TopicSelector>().enabled = true;
//					container.GetComponent<TopicSelector>().SetWheel(this);
//			
//					container.name = "container_"+m;
//					//container.name = "Container_"+ randomNumbers[m];
//					break;
//				}
//			}
//		}
//		
//		//childrenTopics = GetComponentsInChildren<TopicSlotButton>();
//		
//	}
	
	void EliminateRandomNumber(List<int> randomNumbers)
	{
    	int index = Random.Range(0, randomNumbers.Count);
    	//int value = randomNumbers[index];
    	randomNumbers.RemoveAt(index);
	}
	
	public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

		if (go != null && parent != null)
		{
			go.layer = parent.layer;
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = prefab.transform.localScale;
			
		}
		
		return go;
	}
	
//	public void RotateToNearestTopic(TopicSelector container)
//	{
//		float[] topicsDistances = new float[listTopics.Count];
//		
//		for(int i = 0; i < listTopics.Count; i++){
//			float topicDistance = Vector3.Distance(childrenTopics[i].transform.position, container.transform.position);
//			topicsDistances[i] = topicDistance;
//		}
//		
//		float minimumDistance = Mathf.Min(topicsDistances);
//		
//		for(int i = 0; i < listTopics.Count; i++)
//		{
//			if(minimumDistance ==  topicsDistances[i])
//			{
//				//targetPos = childrenTopics[i].transform.position;
//				IsWheelParent(false);
//				
//				Vector3 topicDir =  childrenTopics[i].transform.position - transform.position;
//				transform.up = topicDir;
//				
//				IsWheelParent(true);
//				
//				targetContainer = container;
//				selectedTopic = childrenTopics[i].GetComponent<TopicSlotButton>();
//				canAnimRotate = true;
//				Invoke("EndSpin", correctionSpinSpeed);
//				break;
//			}
//		}
//	
//	}
	
//	public void Update(){
//		
//		
//		if(canAnimRotate)
//		{
//			
//			Vector3 forward = Vector3.forward;
//		
//			//Creates plane with normal coming out of the screen
//    		Plane playerPlane = new Plane(-forward, transform.position);
//		
//			Vector3 screenPoint = sceneCamera.WorldToScreenPoint(targetContainer.transform.position);	
//			Ray ray = sceneCamera.ScreenPointToRay(screenPoint);
//		
//
//    		float hitdist = 0.0f;
//
//
//        	// Get the point along the ray that hits the calculated distance.
//        	Vector3 targetPoint = ray.GetPoint(hitdist);
//			targetPoint.z = 0f;
//			
//			//Where the "Wheel" will point to..
//        	Vector3 aimPoint = targetPoint - transform.position;
//			
//			float angle = Vector2.Angle(transform.up, aimPoint.normalized);
//			Vector3 cross = Vector3.Cross(transform.up, aimPoint.normalized);
//			float rotPerSec = 0f;
//			
//			if(cross.z < 0f)
//				rotPerSec = -angle / correctionSpinSpeed;
//			
//			else
//				rotPerSec = angle / correctionSpinSpeed;
//			
//			
//			transform.Rotate(0, 0, rotPerSec * Time.deltaTime);
//			
//			UnrotateChildren();
//			
//			
//		}
//	}
	

	
//	void EndSpin()
//	{
//		canAnimRotate = false;	
//		
//		selectedTopic.DisableTopic();
//		
//		targetContainer.myIcon.mainTexture = selectedTopic.myIcon.mainTexture;
//		targetContainer.myIcon.enabled = true;
//		
//		targetContainer.CheckIfCorrectTopic(selectedTopic.gameObject.name);
//		
//	}
	
	public void RotateWheel()
	{
		//var forward = sceneCamera.transform.TransformDirection(Vector3.forward);
		//Will be used for creation of plane
		Vector3 forward = Vector3.forward;
		
		//Creates plane with normal coming out of the screen
    	Plane playerPlane = new Plane(-forward, transform.position);
		
		//Shoot ray into the screen from the mouse position
    	//Ray ray = sceneCamera.ScreenPointToRay (Input.mousePosition);
		Ray ray = sceneCamera.ScreenPointToRay (Input.mousePosition);

    	float hitdist = 0.0f;

		//Check that it hits plane (will always hit)
    	if (playerPlane.Raycast (ray, out hitdist)) 
		
		{
        	// Get the point along the ray that hits the calculated distance.
        	Vector3 targetPoint = ray.GetPoint(hitdist);
			
			//Where the "Wheel" will point to..
        	Vector3 aimPoint = targetPoint - transform.position;
			
			//"Rotate" around the y-axis and point into the aim vector
        	transform.up = aimPoint;
    	}
		
		//Keep children from rotating
		UnrotateChildren();
	}
	
	void UnrotateChildren(){
		foreach(TopicSlotButton topic in childrenTopics)
			topic.Unrotate(sceneCamera);
	}
	
	public void IsWheelParent(bool isParent)
	{
		if(!isParent){
			foreach(TopicSlotButton topic in childrenTopics)
				topic.transform.parent = transform.parent;	
			
		}
		
		else{
			foreach(TopicSlotButton topic in childrenTopics)
				topic.transform.parent = transform;		
		}
	}
}
