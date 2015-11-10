using UnityEngine;
using System.Collections;

public class ResolveOverlaping : MonoBehaviour 
{
	Camera sceneCamera;
	
	
	// Use this for initialization
	void Start () {
		sceneCamera = GameObject.FindGameObjectWithTag("MinigameCamera").GetComponent<Camera>();
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(IsOutsideOfView() && col.name != "Boundary_Beach")
		{
			transform.Translate(Vector3.up * Random.Range(0.1f, 0.5f));
		}
	}
	
	bool IsOutsideOfView(){
		Vector3 viewPoint = sceneCamera.WorldToViewportPoint(transform.position);
		
		if(viewPoint.y > 1){
			return true;
		}
		return false;
	}
}
