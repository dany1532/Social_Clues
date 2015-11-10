using UnityEngine;
using System.Collections;

public class cowClick : MonoBehaviour {

	//public GameObject cow; 
	//UITextureAnimation cowAnim; 
	MoveTowardTrain zMov; 
	NPCDialogueAnimation cowAnimation;
	BalkiShake shakeIt; 
	DuckWalk walk; 
	UIStretch myStretch; 
	float initialStretchY; 
	float currStretchY; 
	float lerp; 
	public float maxY = 0.5f; 
	public float time = 5.0f; 
	public float lerpSpeed = 10.0f; 
	bool bouncing; 
	Vector3 currPos; 
	//public Texture cow1; 
	//public Texture cow2;

	void Start()
	{
		//cowAnim = cow.GetComponent<UITextureAnimation>(); 
		//cowAnimation = cow.GetComponent<NPCDialogueAnimation>();
		cowAnimation = this.gameObject.GetComponent<NPCDialogueAnimation>();
		shakeIt = this.gameObject.GetComponent<BalkiShake>(); 
		walk = this.gameObject.GetComponent<DuckWalk>();
		zMov = transform.gameObject.GetComponent<MoveTowardTrain>();

		//Squash and stretch
		myStretch = GetComponent<UIStretch>(); 
		initialStretchY = myStretch.relativeSize.y; 
		currStretchY = initialStretchY; 

		bouncing = false; 

		currPos = new Vector3(0, 0, 0); 
		//InvokeRepeating("startBounce", 0f, 5f);
	}

	void OnClick()
	{
        InputManager.Instance.ReceivedUIInput();
		//renderer.material.mainTexture = cow2;
		audio.Play();
		//cowAnim.UIPlay();
		cowAnimation.PlayAnimation(); 
		Debug.Log ("play animation");
		collider.enabled = false;
		if (shakeIt != null)
		{
			shakeIt.Shake(); 
		}
		if (walk != null)
		{
			walk.Shake(); 
		}
		Invoke ("Wait", cowAnimation.GetDuration());
		//setCowAnimation(NPCAnimations.AnimationIndex.IDLE);
	}

	void Wait()
	{
		stopAnim(); 
		collider.enabled = true;
	}

	void stopAnim()
	{
		cowAnimation.StopAnimation(); 
	}

	void startBounce()
	{
		StartCoroutine("bounce"); 
		bouncing = true; 
	}

	public IEnumerator bounce()
	{
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(currPos.x, maxY, currPos.z));
		yield return new WaitForSeconds(time);
		TweenPosition.Begin(gameObject, time, gameObject.transform.localPosition + new Vector3(currPos.x, -maxY, currPos.z));
		yield return new WaitForSeconds(time);
		bouncing = false; 
	}

	void Update()
	{
		currPos = zMov.currPos;  
		if (bouncing == false)
		{
		//startBounce(); 
		}
		transform.rotation = Camera.main.transform.rotation;
		//lerp += Time.deltaTime * lerpSpeed;
		//Vector2 newStretch = new Vector2(0, Mathf.PingPong(lerp, maxY) + 0.5f); 




	/*void setCowAnimation(NPCAnimations.AnimationIndex animationType) {
		NPCAnimations.AnimationSequence currAnimSeq = this.gameObject.GetComponent<NPCAnimations>().RetrieveAnimationSequence(animationType);
		List<Texture> currAnimSeqTextures = currAnimSeq.textures;
		if (currAnimSeqTextures.Count > 0)
		{
			cowAnimation.StopAnimation();
			cowAnimation.SetAnimationList(currAnimSeqTextures);
			cowAnimation.PlayAnimation();
			cowAnimation.SetSpeed(currAnimSeq.speed);
		}
	}*/
	// Use this for initialization
	/*void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
}