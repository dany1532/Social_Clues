	using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCDialogueAnimation : MonoBehaviour
{
    public List<Texture> animList;
    public int animListCount;
    UITexture portrait;
    bool canPlay;
    public float mFPS = 30;
    public float speed = 1f;
    float mDelta = 0f;
    int mIndex = 0;
	public bool startOnAwake = false;
    Transform myTransform;
	public bool clearListOnStop = true;
	public bool pingPong = true;

    //Get/Set the list from the NPC to be animated
    public void SetAnimationList(List<Texture> npcList)
    {
        animList.Clear();
        animList.AddRange(npcList);
        mFPS = animList.Count;
        animListCount = animList.Count;
    }
	
    //Start animation
    public void PlayAnimation()
    {
        canPlay = true;
		ChangeSprite (0);
    }
	
    //Set the speed of the animation
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetFPS (int fps)
    {
        mFPS = fps;
    }
	
    // Use this for initialization
    void Awake()
    {
		if (animList == null)
        	animList = new List<Texture>();

        portrait = GetComponent<UITexture>();
  
        if (animList != null && animList.Count > 0)
        {
            animListCount = animList.Count;
        }
        
		if (startOnAwake)
			PlayAnimation ();
        myTransform = transform;
    }
 
    public Vector3 GetPostion()
    {
        return myTransform.position;
    }
    
    //Traverses through the frames with Pin-Pong effect
    void ApplyAnimation(float delta)
    {
        mDelta += delta;
        float rate = 1f / (mFPS * speed);
        int mPingPongIndex = 0;
        
        if (rate < mDelta)
        {
            mDelta = (rate > 0f) ? mDelta - rate : 0f;
			if (pingPong)
			{
				mPingPongIndex = (int)Mathf.PingPong(mIndex++, animListCount - 1);
				mIndex = mIndex % (animListCount * 2);
			}
			else
			{
				mPingPongIndex = (mIndex + 1) % animListCount;
				mIndex = mPingPongIndex;
			}
			ChangeSprite(mPingPongIndex);
		}
    }
	
    //Clear list and reset everything
    public void StopAnimation()
    {
		canPlay = false;
		mDelta = 0f;
		mIndex = 0;

		if (clearListOnStop)
		{
	        speed = 1f;
	        animList.Clear();
		}
    }
	
    //Change the npc portrait
    void ChangeSprite(int index)
    {
		if (index >= 0 && index < animList.Count)
        	portrait.mainTexture = animList [index];
    }
	
    // Update is called once per frame
    void Update()
    {
        if (canPlay)
            ApplyAnimation(Time.deltaTime);
    }

	public float GetDuration ()
	{
		return (animListCount+1) / (mFPS * speed);
	}
}
