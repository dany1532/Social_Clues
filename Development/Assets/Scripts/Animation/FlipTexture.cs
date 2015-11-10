using UnityEngine;
using System.Collections;

public class FlipTexture : MonoBehaviour {
    
    bool flipHorizontal = false;
    Transform myTransform;
    Vector3 flipVector = new Vector3(0,180,0);
	// Use this for initialization
	void Awake () {
	    myTransform = transform;
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
            this.transform.Rotate(flipVector);
        }
    }
     
     /// <summary>
     /// False-fies the current flip variable
     /// </summary>
     public void ChangeFlipHorizontal(){
         FlipHorizontal(!flipHorizontal);
     }
}
