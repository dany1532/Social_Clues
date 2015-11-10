using UnityEngine;

public class HiddenObject : MonoBehaviour
{
	[HideInInspector]
	public HiddenObjectCharacter character;
	public string filename;
	public bool selected = false;
	public bool found = false;
	private Vector3 localRotation;
	private Vector3 localPostion;
	
	DisplayedObject displaySlot;
	
	public enum ObjectDifficulty
	{
		EASY = 0,
		HARD = 16
	}
	public ObjectDifficulty difficulty = ObjectDifficulty.EASY;
	
	public void Start()
	{
		localRotation = this.transform.localEulerAngles;
		localPostion = this.transform.localPosition;
	}
	
	public void SetCharacter(HiddenObjectCharacter _character)
	{
		character = _character;
	}

	public void SetCollider (bool state)
	{
		collider.enabled = state;
	}

	public UIAtlas GetAtlas ()
	{
		return character.GetAtlas();
	}
	
	public void OnClick()
	{
		if (!found)
		{
			displaySlot.Found();
			found = true;
		}
	}

	public void SetSlot (DisplayedObject slot)
	{
		displaySlot = slot;
	}
}