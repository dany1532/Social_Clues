using UnityEngine;
using System.Collections;

public class TopicSlotButton : MonoBehaviour {
	public GabyTopicsWheel wheel;
	
	
	public UITexture myIcon;
	public UITexture myBackground;
	public UITexture myBorder;
	public SCSpriteAnimation cloudAnim;
	
	UITexture myCheck;
	bool isPressed = false;
	bool isDragged = false;
	
	public float force = 1000f;
	Vector3 myDir;
	
	float speed = 0.1f;
	public bool droppedInContainer = false;
	Vector3 prevPos;
	
	public TopicContainer myContainer;
	
	
	
	// Use this for initialization
	void Start () 
	{
		wheel = transform.parent.GetComponent<GabyTopicsWheel>();
		
	}
	
	
	public void DisplayAnimation(){
		if(cloudAnim == null)
		{
			cloudAnim = gameObject.AddComponent(typeof(SCSpriteAnimation)) as SCSpriteAnimation;
		}
		cloudAnim.GetComponent<UITexture>().enabled = true;
		cloudAnim.Play();	


	}
	
	public void CanUpdate()
	{
		rigidbody.isKinematic = false;
		
		do
		{
			myDir =(Vector3)Random.insideUnitCircle;
		
			rigidbody.velocity = myDir *speed;
			
		} while (rigidbody.velocity == Vector3.zero);
		//rigidbody.AddForce(myDir * force);
	}
	
	
	public void DisableTopic(){
		myIcon.enabled = false;
	}
	
	public void EnableTopic(){
		myIcon.enabled = true;
	}
	
	
	public void Unrotate(Camera sceneCamera)
	{
		transform.up = Vector3.up;
	}
	
	
	
	void OnCollisionEnter(Collision other)
	{
		if(!rigidbody.isKinematic){
			Vector3 newDir = Vector3.Reflect(myDir, other.contacts[0].normal);
			newDir.Normalize();
			
			rigidbody.velocity = newDir * speed;
			
			myDir = newDir;
		}
	}
	
	
	void OnPress(bool state)
	{
		if(state)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.isKinematic = true;	
			prevPos = transform.position;
			
			if(droppedInContainer && myContainer.manager.ContainersFull())
			{
				myContainer.myCheck.enabled = false;
				myContainer.manager.PlayerSelectedTopic(myIcon.mainTexture);
				myIcon.enabled  = false;
				
			}
			
		}
		
		else if (!state && !droppedInContainer)
		{
			DropObject();
		}
		
		
//		if(state)
//		{
//			//Unparent all children of the wheel (hack)
//			wheel.IsWheelParent(false);
//			
//			//Rotate wheel
//			wheel.RotateWheel();
//			
//			//Parent Children back again
//			wheel.IsWheelParent(true);
//		}
		
	}
	
	void DropObject ()
	{
		// Is there a droppable container?
		Collider col = UICamera.lastHit.collider;
		TopicContainer container = (col != null) ? col.gameObject.GetComponent<TopicContainer>() : null;

		if (container != null && container.IsEmpty())
		{
			Vector3 pos = container.transform.position;
			pos.z = transform.position.z;
			transform.position = pos;
			droppedInContainer = true;
			myContainer = container;
			container.CheckIfCorrectTopic(this);
		}
		else
		{
			ReturnToBubble();
		}
	}
	
	public void ReturnToBubble(){
		//transform.position = prevPos;
		transform.position = wheel.FreePos(transform.position);
		CanUpdate();
	}
	
	/*void OnDrag(Vector2 drag)
	{
		//wheel.RotateWheel();
	}*/

}
