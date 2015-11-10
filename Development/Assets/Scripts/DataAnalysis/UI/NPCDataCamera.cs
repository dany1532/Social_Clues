using UnityEngine;
using System.Collections;
using System;

public class NPCDataCamera : MonoBehaviour {
	/// <summary>
	/// Root object that will be used for drag-limiting bounds.
	/// </summary>

	public Transform rootForBounds;

	/// <summary>
	/// Scale value applied to the drag delta. Set X or Y to 0 to disallow dragging in that direction.
	/// </summary>

	public Vector2 scale = Vector2.one;

	/// <summary>
	/// Effect the scroll wheel will have on the momentum.
	/// </summary>

	public float scrollWheelFactor = 0f;

	/// <summary>
	/// Effect to apply when dragging.
	/// </summary>

	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	/// <summary>
	/// Whether the drag operation will be started smoothly, or if if it will be precise (but will have a noticeable "jump").
	/// </summary>

	public bool smoothDragStart = true;

	/// <summary>
	/// How much momentum gets applied when the press is released after dragging.
	/// </summary>

	public float momentumAmount = 35f;

	Camera mCam;
	Transform mTrans;
	bool mPressed = false;
	Vector2 mMomentum = Vector2.zero;
	Bounds mBounds;
	float mScroll = 0f;
	UIRoot mRoot;
	bool mDragStarted = false;
	
	// for scroll bar
	public UIScrollBar scrollBar;
	public UITable npcContainer;
	public bool hasScrollBar = false;

	/// <summary>
	/// Current momentum, exposed just in case it's needed.
	/// </summary>

	public Vector2 currentMomentum { get { return mMomentum; } set { mMomentum = value; } }
	public bool reset;
	
	/// <summary>
	/// Cache the common components.
	/// </summary>
	
	void Awake ()
	{
		mCam = camera;
		mTrans = transform;

		if (rootForBounds == null)
		{
			Debug.LogError(NGUITools.GetHierarchy(gameObject) + " needs the 'Root For Bounds' parameter to be set", this);
			enabled = false;
		}
	}

	/// <summary>
	/// Cache the root.
	/// </summary>

	void Start () { 
		mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		if(hasScrollBar) {
			Invoke("updateScrollBar", 0.1f);
		}
	}

	/// <summary>
	/// Calculate the offset needed to be constrained within the panel's bounds.
	/// </summary>

	Vector3 CalculateConstrainOffset ()
	{
		if (rootForBounds == null || rootForBounds.childCount == 0) return Vector3.zero;

		Vector3 bottomLeft = new Vector3(mCam.rect.xMin * Screen.width, mCam.rect.yMin * Screen.height, 0f);
		Vector3 topRight   = new Vector3(mCam.rect.xMax * Screen.width, mCam.rect.yMax * Screen.height, 0f);

		bottomLeft = mCam.ScreenToWorldPoint(bottomLeft);
		topRight = mCam.ScreenToWorldPoint(topRight);

		Vector2 minRect = new Vector2(mBounds.min.x, mBounds.min.y);
		Vector2 maxRect = new Vector2(mBounds.max.x, mBounds.max.y);

		return NGUIMath.ConstrainRect(minRect, maxRect, bottomLeft, topRight);
	}

	/// <summary>
	/// Constrain the current camera's position to be within the viewable area's bounds.
	/// </summary>

	public bool ConstrainToBounds (bool immediate)
	{
		if (mTrans != null && rootForBounds != null)
		{
			Vector3 offset = CalculateConstrainOffset();

			if (!float.IsInfinity(offset.x) && offset.magnitude > 0f)
			{
				if (immediate)
				{
					mTrans.position -= offset;
				}
				else
				{
					SpringPosition sp = SpringPosition.Begin(gameObject, mTrans.position - offset, 13f);
					sp.ignoreTimeScale = true;
					sp.worldSpace = true;
				}
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Calculate the bounds of all widgets under this game object.
	/// </summary>

	public void Press (bool isPressed)
	{
		if (isPressed) mDragStarted = false;

		if (rootForBounds != null)
		{
			mPressed = isPressed;

			if (isPressed)
			{
				// Update the bounds
				mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds, 31);
				

				// Remove all momentum on press
				mMomentum = Vector2.zero;
				mScroll = 0f;

				// Disable the spring movement
				SpringPosition sp = GetComponent<SpringPosition>();
				if (sp != null) sp.enabled = false;
			}
			else if (dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
			{
				ConstrainToBounds(false);
			}
		}
	}
	
	public void OnEnable()
	{
		Invoke ("Reset", 1f);
	}
	
	public void Reset()
	{
		Press(true);
		reset = true;
		Drag (Vector2.zero);
	}
	
	/// <summary>
	/// Drag event receiver.
	/// </summary>

	public void Drag (Vector2 delta)
	{
		// Prevents the initial jump when the drag threshold gets passed
		if (smoothDragStart && !mDragStarted)
		{
			mDragStarted = true;
			return;
		}

		if (UICamera.currentTouch != null) UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		if (mRoot != null) delta *= mRoot.pixelSizeAdjustment;

		Vector2 offset = Vector2.Scale(delta, -scale);
		mTrans.localPosition += (Vector3)offset;

		// Adjust the momentum
		mMomentum = Vector2.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);

		// Constrain the UI to the bounds, and if done so, eliminate the momentum
		if (dragEffect != UIDragObject.DragEffect.MomentumAndSpring && ConstrainToBounds(true))
		{
			mMomentum = Vector2.zero;
			mScroll = 0f;
		}
		
		// update scroll bar when camera is dragged
		if(hasScrollBar) {
			updateScrollBar ();
		}
	}
	
	public void updateScrollBar() {
		Bounds panelBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds, 20);
		
		Vector3 cameraMax = this.GetComponent<UIViewport>().getCamera().transform.position + new Vector3(this.GetComponent<UIViewport>().getCamera().rect.xMax, this.GetComponent<UIViewport>().getCamera().rect.yMax, 0);
		Vector3 cameraMin = this.GetComponent<UIViewport>().getCamera().transform.position + new Vector3(this.GetComponent<UIViewport>().getCamera().rect.xMin, this.GetComponent<UIViewport>().getCamera().rect.yMin, 0);
		
		float cameraHeight = Math.Abs(cameraMax.y - cameraMin.y);
		//Debug.Log ("Camera Height: " + cameraHeight);
		
		//Vector4 clip = mPanel.clipRange;
		//float extents = clip.w * 0.5f;
		float extents = cameraHeight;
		
		//float min = clip.y - extents - bmin.y;
		float min = cameraMin.y - extents - panelBounds.min.y;
		//float max = bmax.y - extents - clip.y;
		float max = panelBounds.max.y - extents - cameraMin.y;
		
		//float height = bmax.y - bmin.y;
		float height = panelBounds.max.y - panelBounds.min.y;
		min = Mathf.Clamp01(min / height);
		//min = Mathf.Clamp01(min / panelHeight);
		max = Mathf.Clamp01(max / height);
		//max = Mathf.Clamp01(max / panelHeight);
		float sum = min + max;

		scrollBar.barSize = 1f - sum;
		scrollBar.scrollValue = (sum > 0.001f) ? 1f - min / sum : 0f;		
		
		
		//Debug.Log ("camera max: " + cameraMax);
		//Debug.Log ("camera min: " + cameraMin);
		//Debug.Log ("Panel bounds: " + panelBounds);
		/*
		Debug.Log ("Scroll bar size: " + scrollBar.barSize);
		Debug.Log ("Scroll value: " + scrollBar.scrollValue);	
		
		Debug.Log ("camera position: " + this.GetComponent<UIViewport>().getCamera().transform.position);
		*/
	}
	
	/// <summary>
	/// If the object should support the scroll wheel, do it.
	/// </summary>

	public void Scroll (float delta)
	{
		if (enabled && NGUITools.GetActive(gameObject))
		{
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta)) mScroll = 0f;
			mScroll += delta * scrollWheelFactor;
		}
	}

	/// <summary>
	/// Apply the dragging momentum.
	/// </summary>

	void Update ()
	{
		float delta = Time.deltaTime;
		
		if (reset)
		{
			reset = false;
			Drag (Vector2.one);
			Press (false);
		}
		
		if (mPressed)
		{
			// Disable the spring movement
			SpringPosition sp = GetComponent<SpringPosition>();
			if (sp != null) sp.enabled = false;
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * (mScroll * 20f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, delta);

			if (mMomentum.magnitude > 0.01f)
			{
				// Apply the momentum
				mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
				mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds, 20);

				if (!ConstrainToBounds(dragEffect == UIDragObject.DragEffect.None))
				{
					SpringPosition sp = GetComponent<SpringPosition>();
					if (sp != null) sp.enabled = false;
				}
				return;
			}
			else mScroll = 0f;
		}

		// Dampen the momentum
		NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
	}
}
