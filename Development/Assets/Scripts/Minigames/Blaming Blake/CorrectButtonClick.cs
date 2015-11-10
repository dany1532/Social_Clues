using UnityEngine;
using System.Collections;

public class CorrectButtonClick : MonoBehaviour {
	
	public bool isCorrect; 
	public int correctCount;
	public int totalCount; 
	public bool hitSlot; 
	public bool turnOnBlame; 
	public bool finishedSeqRef; 
	public bool column5Ref; 
	public bool colorsWhiteRef; 
	public bool resetRef; 
	public bool hitHighlight; 
	public GameObject manager;
	public GameObject x; 
	public GameObject thisButton;
	public GameObject options; 
	private float speed; 
	public Transform target; 
	public Transform highlight; 
	//public GameObject options;
	//private BlakeTurnOnOptions BlakeTurnOnOptionsScript; 
	private BlamingBlakeManager BlamingBlakeManagerScript;
	// Use this for initialization
	void Start () {
		BlamingBlakeManagerScript = manager.GetComponent<BlamingBlakeManager>();
		totalCount = 0;
		//BlamingBlakeTurnOnOptionsScript = options.GetComponent<BlakeTurnOnOptions>();
		correctCount = 0; 
		speed = 1.0f; 
		//x.SetActive(false);
		hitSlot = false; 
		turnOnBlame = false; 
		finishedSeqRef = false; 
		column5Ref = false; 
		colorsWhiteRef = false; 
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (correctCount);
		
		totalCount = manager.GetComponent<BlamingBlakeManager>().correctCountRef;
		finishedSeqRef = manager.GetComponent<BlamingBlakeManager>().finishedSequence;
		column5Ref = manager.GetComponent<BlamingBlakeManager>().column5; 
		colorsWhiteRef = options.GetComponent<BlakeTurnOnOptions>().colorsWhite;
		resetRef = manager.GetComponent<BlamingBlakeManager>().reset;
		if (finishedSeqRef == true && isCorrect && colorsWhiteRef == true)//(totalCount >= 4 && isCorrect && column5Ref == true)
		{
			x.SetActive(false); 
			thisButton.SetActive(false); 
			
		}
		
		if (correctCount >= 4)
		{
			correctCount = 0; 
			BlamingBlakeManagerScript.Instance.SendMessage("OnButtonClickDown", correctCount, SendMessageOptions.DontRequireReceiver);
		}
		/*else if (totalCount>= 4 && !isCorrect)
		{
			// The step size is equal to speed times frame time.
			float step = speed * Time.deltaTime;
		
			// Move our position a step closer to the target.
			transform.position = Vector3.MoveTowards(transform.position, target.position, step);
			
			//blame hack to get it to turn on w/o buttons in position
			/*turnOnBlame = true; 
			BlamingBlakeManagerScript.Instance.SendMessage("BlameHack", turnOnBlame, SendMessageOptions.DontRequireReceiver);
			
			if (transform.position.y >= (target.position.y-0.01))
			{
				Debug.Log ("I hit it!");
				hitSlot = true;
			}
		
		}*/
		Debug.Log ("resetRef Buttons"+resetRef); 
		
		if (resetRef == true)
		{
			Debug.Log ("move back!");
			// The step size is equal to speed times frame time.
			//float step2 = speed * Time.deltaTime;
			hitSlot = false; 
			// Move our position a step closer to the target.
			transform.position = highlight.position; //Vector3.MoveTowards(transform.position, highlight.position, step2);
			
			//blame hack to get it to turn on w/o buttons in position
			/*turnOnBlame = true; 
			BlamingBlakeManagerScript.Instance.SendMessage("BlameHack", turnOnBlame, SendMessageOptions.DontRequireReceiver);*/
			if (transform.position.y <= highlight.position.y)
			{
				Debug.Log ("we're back!!!");
				hitHighlight = true;
				BlamingBlakeManagerScript.Instance.SendMessage("StartOver", hitHighlight, SendMessageOptions.DontRequireReceiver);
			}
			
		}
		
		if (finishedSeqRef == true)
		{
			// The step size is equal to speed times frame time.
			float step = speed * Time.deltaTime;
		
			// Move our position a step closer to the target.
			transform.position = Vector3.MoveTowards(transform.position, target.position, step);
			
			//blame hack to get it to turn on w/o buttons in position
			/*turnOnBlame = true; 
			BlamingBlakeManagerScript.Instance.SendMessage("BlameHack", turnOnBlame, SendMessageOptions.DontRequireReceiver);*/
			
			if (transform.position.y >= (target.position.y-0.01))
			{
				Debug.Log ("I hit it!");
				hitSlot = true;
			}
		}
		BlamingBlakeManagerScript.Instance.SendMessage("FinalSequence", hitSlot, SendMessageOptions.DontRequireReceiver);
		SlotHit();
		
		/*Debug.Log ("TransformPos "+ this.name + ": " + transform.position);
		Debug.Log ("TargetPos "+ this.name + ": " + target.position);*/
		
	
	}
	
	void SlotHit()
	{
		hitSlot = false;
	}
	
	void OnClick(){
		if (isCorrect)
		{
			correctCount++;
			//turn on X
			x.SetActive(true); 
			
			/*if (correctCount == 0)
			{
				BlakeTurnOnOptionsScript.Instance.SendMessage("OnButtonClickDown", correctCount, SendMessageOptions.DontRequireReceiver);
			}*/
			BlamingBlakeManagerScript.Instance.SendMessage("OnButtonClickDown", correctCount, SendMessageOptions.DontRequireReceiver);
		}
		
		else
		{
			Debug.Log("Incorrect choice"); 
		}
		
		
	}
}
