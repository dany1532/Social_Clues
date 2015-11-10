using UnityEngine;
using System.Collections;

public class EyeContact : MonoBehaviour
{
	/// <summary>
	/// Target object that will be dragged.
	/// </summary>
	public PreDialogueMinigame minigame;
	public Transform arrow;
	public Transform eyes;
	public Transform eyesCenter;
	private float length;
	Vector2 arrowInitialPos;
	Random randomizer;
	public float eyeMinAngle;
	public float eyeMaxAngle;
	float counterFrequency = 0.1f;
	float counter = 0;
	public float holdDuration = 3f;
	public UIFilledSprite target;
	public string emptyTarget;
	public string filledTarget;
	public Color offZoneColor = Color.gray;
	public Color targetZoneColor = Color.green;
	bool reachedGoal = false;
	public Dialogue ending;

	public AudioClip successSFX;
		
	void Start()
	{
		Vector3 arrowScreenPosition = GameManager.Instance.uiCamera.WorldToScreenPoint(transform.position);
		arrowInitialPos = new Vector2(arrowScreenPosition.x, arrowScreenPosition.y);

        MinigameStart();	
	}
	
	/// <summary>
	/// Initialize the eye contact demo, by position the eyes
	/// </summary>
	void MinigameStart()
	{
        float angleOffset = Player.instance.interactingNPC.eyeContactAngleOffset;
		float randomAngle = Random.Range(45, 315) + angleOffset;
		arrow.localEulerAngles = new Vector3(0, 0, randomAngle);
		randomAngle = randomAngle * Mathf.Deg2Rad;
		length = Mathf.Abs(eyesCenter.localPosition.y - eyes.localPosition.y);
		eyes.localPosition = eyesCenter.localPosition + new Vector3(length * Mathf.Cos(randomAngle), length * Mathf.Sin(randomAngle));
		collider.enabled = true;
		reachedGoal = false;
        
        target.fillAmount = Player.instance.interactingNPC.eyeContactArrowLength;
        target.transform.localEulerAngles = new Vector3(0,0, target.transform.localEulerAngles.z + angleOffset);
        eyeMaxAngle += angleOffset;
        eyeMinAngle += angleOffset;
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>
	void OnDrag (Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(arrow.gameObject) && arrow != null && !reachedGoal)
		{
			Vector2 direction = UICamera.currentTouch.pos - arrowInitialPos;
			Vector3 rotationVector;
			if(direction.x <= 0) {
				rotationVector = new Vector3(0, 0, Vector2.Angle(Vector2.up, direction) + 90);
			} else {
				rotationVector = new Vector3(0, 0, -Vector2.Angle(Vector2.up, direction) + 90);
			}
			arrow.localEulerAngles = rotationVector;
			
			eyes.localPosition = eyesCenter.localPosition + new Vector3(length * Mathf.Cos(Mathf.Deg2Rad * rotationVector.z), 0.75f * length * Mathf.Sin(Mathf.Deg2Rad * rotationVector.z));
			if(rotationVector.z <= eyeMaxAngle && rotationVector.z >= eyeMinAngle)
			{
				if (counter < 0)
				{
					counter = 0;
					InvokeRepeating("CountWithinArea", 0, counterFrequency);
					target.spriteName = filledTarget;
					target.color = targetZoneColor;
				}
			}
			else
			{
				counter = -1;
				CancelInvoke("CountWithinArea");
				target.spriteName = emptyTarget;
				target.color = offZoneColor;
			}
		}
	}
	
	void CountWithinArea()
	{
		if (counter >= holdDuration)
		{
			reachedGoal = true;
			counter = -1;
			collider.enabled = false;
			CancelInvoke("CountWithinArea");
			AudioManager.Instance.Play(successSFX, this.transform, 1.0f, false);
			Sherlock.Instance.PlaySequenceInstructions(ending, EndMinigame);
		}
		else
		{
			counter += counterFrequency;
		}
	}
	
	// End minigame and proceed to dialogue
	void EndMinigame()
	{
		// Trigger event		
		minigame.returnEvent.TriggerEvent(true);
		target.spriteName = emptyTarget;
		target.color = offZoneColor;
        Destroy(transform.parent.gameObject);
	}
}
