using UnityEngine;
using System.Collections;


public class SCSpriteAnimation : MonoBehaviour {
	
	public UISpriteAnimation spriteAnimation;
	
	public enum AnimationState {
		STOP,
		PLAY,
		CURRENTLYPLAYING,
		CURRENTLYSTOPPED
	}
	
	public AnimationState _animationState = AnimationState.CURRENTLYSTOPPED;
	public string namePrefix = "";
	public bool playOnStart = false;
	public bool ping_pong = false;
	bool flipHorizontal = false;
	
	
	
	
	
	// Use this for initialization
	void Start () {
		//spriteAnimation = GetComponent<UISpriteAnimation>();
		//namePrefix = AnimationPrefix.PLAYER_STAND;
		spriteAnimation.namePrefix = namePrefix;
		
		
		if(playOnStart){
			Play();
		}
		
		
		else if(spriteAnimation.SpriteListSize() < 1)
			Debug.Log("Name Prefix: "+namePrefix+" does not exist in current atlas");
			
	}
	
	// Update is called once per frame
	void Update () {
		
		if(ping_pong)
			SetPingPongEffect(true);
		else
			SetPingPongEffect(false);
		
		if (_animationState == AnimationState.PLAY){
			spriteAnimation.UIPlay();
			_animationState = AnimationState.CURRENTLYPLAYING;
		}
		else if(_animationState == AnimationState.STOP){
			spriteAnimation.UIStop();
			_animationState = AnimationState.CURRENTLYSTOPPED;
		}
	}
	
	public void SetPingPongEffect(bool wantPong){
		ping_pong = wantPong;
		spriteAnimation.UIUSetPingPong(wantPong);	
	}
	
	
	/// <summary>
	/// Flips the sprites horizontally
	/// </summary>
	/// <param name='flipBool'>
	/// True if you want to flip the sprites
	/// </param>
	public void FlipHorizontal(bool flipBool){
		if(flipHorizontal != flipBool){
			flipHorizontal = flipBool;
			
			this.transform.Rotate(new Vector3(0,180,0));
//			Vector3 newScale = transform.localScale;
//			newScale.x = newScale.x * -1f;
//			transform.localScale = newScale;
		}
	}
	
	/// <summary>
	/// False-fies the current flip variable
	/// </summary>
	public void ChangeFlipHorizontal(){
		FlipHorizontal(!flipHorizontal);
	}
	
	/// <summary>
	/// Plays the animation once
	/// </summary>
	public void Play(){
		_animationState = AnimationState.PLAY;
		spriteAnimation.loop = false;
	}
	
	/// <summary>
	/// Plays the animation in a loop
	/// </summary>
	public void PlayLoop(){
		_animationState = AnimationState.PLAY;
		spriteAnimation.loop = true;
	}
	
	/// <summary>
	/// Stops the current animation (Does not reset)
	/// </summary>
	public void Stop(){
		_animationState = AnimationState.STOP;
		spriteAnimation.loop = false;
		ping_pong = false;
	}
	
	/// <summary>
	/// Not yet implemented. 
	/// </summary>
	public void Reset(){
		spriteAnimation.Reset();	
	}
	
	/// <summary>
	/// Changes the animation to be played according to name prefix
	/// </summary>
	/// <param name='nameAnimation'>
	/// The prefix of the animation to be played
	/// </param>
	public void ChangeAnimation(string nameAnimation){
		namePrefix = nameAnimation;
		_animationState = AnimationState.CURRENTLYSTOPPED;
		spriteAnimation.namePrefix = nameAnimation;	
		
		if(spriteAnimation.SpriteListSize() < 1)
			Debug.Log("Name Prefix: "+ namePrefix+ " does not exist in current atlas");
	}

	public void ChangeFrameRate (int frameRate)
	{
		spriteAnimation.framesPerSecond = frameRate;
	}
}
