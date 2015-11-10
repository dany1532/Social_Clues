using UnityEngine;
using System.Collections;

public class TurnKey : MonoBehaviour {

	Camera sceneCamera;
	public GameObject myLock, myTurn, myKey, myHead, myCircle, myParent;
	Vector3 prevAngle;

	Vector3 defaultUp;

	GameObject tutorialHand;

	enum currentTurnAngle {
		start,
		left45,
		horiz1,
		left135,
		vert,
		left225,
		horiz2,
		end
	}
	currentTurnAngle myCurrentTurnAngle = currentTurnAngle.start;

	bool startedPress = false;
	Vector3 startPositionDistance = Vector3.zero;

	// Use this for initialization
	void Start () {
		sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
		myParent = transform.parent.gameObject;
		transform.parent = myCircle.transform.parent;
		currentTurnAngle myCurrentTurnAngle = currentTurnAngle.start;
		defaultUp = myTurn.transform.up;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPress(bool state)
	{
		if(state) {
			startedPress = true;
			StartCoroutine("RotateKey");
		}
		else {
			startedPress = false;
			StopCoroutine("RotateKey");
		}
	}

	public void resetTurnCircle() {
		myTurn.SetActive(true);
		myTurn.transform.up = defaultUp;
		currentTurnAngle myCurrentTurnAngle = currentTurnAngle.start;
		myKey.GetComponent<UISprite>().spriteName = "turnkey1";
		myHead.GetComponent<UISprite>().spriteName = "turn keyhead1";
	}

	IEnumerator RotateKey()
	{
		while(true)
		{
			transform.parent = myParent.transform;
			transform.localPosition = new Vector3(-59, 164, 0);
			//Will be used for creation of plane
			Vector3 forward = Vector3.forward;
			
			//Creates plane with normal coming out of the screen
			Plane playerPlane = new Plane(-forward, myTurn.transform.position);
			
			//Shoot ray into the screen from the mouse position
			//Ray ray = sceneCamera.ScreenPointToRay (Input.mousePosition);
			Ray ray = sceneCamera.ScreenPointToRay (Input.mousePosition);
			
			float hitdist = 0.0f;

			prevAngle = myTurn.transform.up;

			Vector3 currAngle = Vector3.zero;
			
			//Check that it hits plane (will always hit)
			if (playerPlane.Raycast (ray, out hitdist))
				
			{
				// Get the point along the ray that hits the calculated distance.
				Vector3 targetPoint = ray.GetPoint(hitdist);

				Vector3 aimPoint;
				
				//Where the "Wheel" will point to..
				aimPoint = targetPoint - myTurn.transform.position;
				if(startedPress) {
					startPositionDistance = myTurn.transform.up - aimPoint;
					startPositionDistance = new Vector3(startPositionDistance.x, 0f, startPositionDistance.z);
					startedPress = false;
				}
				aimPoint += startPositionDistance;

				currAngle = aimPoint.normalized;

				//use cross product between currAngle and prevAngle to change:
				//myCircle.GetComponent<UISprite>().fillAmount
				//and to calculate if circle can move
				if(Vector3.Cross(currAngle,prevAngle).z < 0 && Vector3.Angle(currAngle,prevAngle) < 45)
				{
					//"Rotate" around the y-axis and point into the aim vector
					myTurn.transform.up = aimPoint;
					myCircle.GetComponent<UISprite>().fillAmount -= Vector3.Angle(prevAngle, currAngle)*.0027777f;
				}

			

			}

			float angle = myTurn.transform.localRotation.z;


			if(angle >= 0.2f && angle <= 0.4f && myCurrentTurnAngle == currentTurnAngle.start) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey4";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead4";
				myCurrentTurnAngle = currentTurnAngle.left45;
			}

			if(angle >= 0.4f && angle <= 0.9f && myCurrentTurnAngle == currentTurnAngle.left45) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey3";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead3";
				myCurrentTurnAngle = currentTurnAngle.horiz1;
			}

			if(angle >= 0.9f && angle <= 1.0f && myCurrentTurnAngle == currentTurnAngle.horiz1) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey2";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead2";
				myCurrentTurnAngle = currentTurnAngle.left135;
			}

			if(angle >= 0.98f && angle <= 1.0f && myCurrentTurnAngle == currentTurnAngle.left135) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey1";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead1";
				myCurrentTurnAngle = currentTurnAngle.vert;
			}
			if((angle >= 0.9f && angle <= 0.95f) && myCurrentTurnAngle == currentTurnAngle.vert) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey4";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead4";
				myCurrentTurnAngle = currentTurnAngle.left225;
			}
			if(angle <= 0.0f && myCurrentTurnAngle == currentTurnAngle.left225) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey3";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead3";
				myCurrentTurnAngle = currentTurnAngle.horiz2;
			}

			if(angle >= -0.7f && myCurrentTurnAngle == currentTurnAngle.horiz2) {
				myLock.GetComponent<DropContainerLock>().StartCoroutine("keyTurned");
				myCurrentTurnAngle = currentTurnAngle.start;
				break;
			}

			//Keep children from rotating
			//UnrotateChildren(); //ignore this since I used it  so that the topics around the wheel didn't rotate
		
		yield return null;
		
		}

	}

	//the following methods are used to simulate the key turning within the tutorial
	public void OnTutorialPress(bool state)
	{
		tutorialHand = GameObject.Find ("TutorialHand");
		if(state) {
			startedPress = true;
			StartCoroutine("RotateKeyT");
		}
		else {
			startedPress = false;
			StopCoroutine("RotateKeyT");
		}
	}

	IEnumerator RotateKeyT()
	{ //convert finger movement to tutorialHand movement
		while(true)
		{

			transform.parent = myParent.transform;
			transform.localPosition = new Vector3(-59, 164, 0);
			//Will be used for creation of plane
			//Vector3 forward = Vector3.forward;
			
			/*//Creates plane with normal coming out of the screen
			Plane playerPlane = new Plane(-forward, myTurn.transform.position);
			
			//Shoot ray into the screen from the mouse position
			//Ray ray = sceneCamera.ScreenPointToRay (Input.mousePosition);
			Ray ray = sceneCamera.ScreenPointToRay (tutorialHand.transform.position);
			
			float hitdist = 0.0f;*/
			
			prevAngle = myTurn.transform.up;
			
			Vector3 currAngle = Vector3.zero;
			

			// Get the point along the ray that hits the calculated distance.
			Vector3 targetPoint =tutorialHand.transform.position; //position of hand
			Vector3 aimPoint;
			
			//Where the "Wheel" will point to..
			aimPoint = targetPoint - myTurn.transform.position;
			if(startedPress) { 
				startPositionDistance = myTurn.transform.up - aimPoint;
				startPositionDistance = new Vector3(startPositionDistance.x, 0f, startPositionDistance.z);
				startedPress = false;
			}
			aimPoint += startPositionDistance;
			currAngle = aimPoint.normalized;
			
			//use cross product between currAngle and prevAngle to change:
			//myCircle.GetComponent<UISprite>().fillAmount
			//and to calculate if circle can move
			//if(Vector3.Cross(currAngle,prevAngle).z < 0 && Vector3.Angle(currAngle,prevAngle) < 45)
			//{

				//"Rotate" around the y-axis and point into the aim vector
				myTurn.transform.up = aimPoint;
				myCircle.GetComponent<UISprite>().fillAmount -= Vector3.Angle(prevAngle, currAngle)*.0027777f;
			//}
				

			float angle = myTurn.transform.localRotation.z;
			
			
			if(angle >= 0.2f && angle <= 0.4f && myCurrentTurnAngle == currentTurnAngle.start) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey4";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead4";
				myCurrentTurnAngle = currentTurnAngle.left45;
			}
			
			if(angle >= 0.4f && angle <= 0.9f && myCurrentTurnAngle == currentTurnAngle.left45) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey3";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead3";
				myCurrentTurnAngle = currentTurnAngle.horiz1;
			}
			
			if(angle >= 0.9f && angle <= 1.0f && myCurrentTurnAngle == currentTurnAngle.horiz1) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey2";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead2";
				myCurrentTurnAngle = currentTurnAngle.left135;
			}
			
			if(angle >= 0.98f && angle <= 1.0f && myCurrentTurnAngle == currentTurnAngle.left135) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey1";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead1";
				myCurrentTurnAngle = currentTurnAngle.vert;
			}
			if((angle >= 0.9f && angle <= 0.95f) && myCurrentTurnAngle == currentTurnAngle.vert) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey4";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead4";
				myCurrentTurnAngle = currentTurnAngle.left225;
			}
			if(angle <= 0.0f && myCurrentTurnAngle == currentTurnAngle.left225) {
				myKey.GetComponent<UISprite>().spriteName = "turnkey3";
				myHead.GetComponent<UISprite>().spriteName = "turn keyhead3";
				myCurrentTurnAngle = currentTurnAngle.horiz2;
			}
			
			if(angle >= -0.7f && myCurrentTurnAngle == currentTurnAngle.horiz2) {
				myLock.GetComponent<DropContainerLock>().StartCoroutine("keyTurnedT");
				myCurrentTurnAngle = currentTurnAngle.start;
				break;
			}
			//Keep children from rotating
			//UnrotateChildren(); //ignore this since I used it  so that the topics around the wheel didn't rotate

			yield return null;
			
		}
		
	}

}
