using UnityEngine;
using System.Collections;
 
public class HUDFPS : MonoBehaviour 
{	
	public  float frequency = 0.5F; // The update frequency of the fps
	private float accum   = 0f; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private UILabel fpsText;
	
	void Start()
	{
		fpsText = GetComponent<UILabel>();
	    if( !fpsText )
	    {
	       	Destroy (gameObject);
	        return;
	    }
		InvokeRepeating( "FPS",0, frequency);
	}
 
	void Update()
	{
	    accum += Time.timeScale/ Time.deltaTime;
	    ++frames;
	}
	
	void FPS()
	{
		// Update the FPS
	    float fps = accum/frames;
	    fpsText.text = fps.ToString( "f" + Mathf.Clamp( 0, 0, 10 ) );

		//Update the color
		fpsText.color = (fps >= 40) ? Color.green : ((fps > 30) ? Color.yellow : ((fps > 15) ? new Color(200, 75, 0) : Color.yellow));

        accum = 0.0F;
        frames = 0;
	}
}