using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {
	
	/// <summary>
	/// Object that acts as a thumb.
	/// </summary>
	public Transform thumb;
	
	BoxCollider mCol;
	Vector2 mSize = Vector2.zero;
	Vector2 mCenter = Vector3.zero;
	
	/// <summary>
	/// Value of the slider.
	/// </summary>
	float rawValue = 1f;
	public float sliderValue
	{
		get
		{
			return rawValue;
		}
		set
		{
			Set(value);
		}
	}
	
	/// <summary>
	/// Ensure that we have a background and a foreground object to work with.
	/// </summary>
	void Awake ()
	{
		mCol = collider as BoxCollider;
	}
	
	// Use this for initialization
	void Start () {
		if (mCol != null)
		{
			mSize = mCol.size;
			mCenter = mCol.center;
			Destroy(mCol);
		}
		else
		{
			Debug.LogWarning("UISlider expected to find a foreground object or a box collider to work with", this);
		}
		Set(rawValue);
	}
	
	/// <summary>
	/// Update the visible slider.
	/// </summary>
	void Set (float input)
	{
		// Clamp the input
		float val = Mathf.Clamp01(input);
		if (val < 0.001f) val = 0f;

		// Save the raw value
		rawValue = val;
		
		if (thumb != null)
		{
			Vector3 scale = thumb.localPosition;
			scale.x = mSize.x * rawValue;
			thumb.localPosition = scale;
		}
	}
}
