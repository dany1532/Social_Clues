using UnityEngine;
using System.Collections;

public class onDrop : MonoBehaviour {

	// Use this for initialization
	public bool isPlaceforEngine = false;

	void OnDrop (GameObject go)
	{
		DragDropItem ddo = go.GetComponent<DragDropItem>();
		
			
		if (ddo != null)
		{
			
			IsAEngine isaEngine = go.GetComponent<IsAEngine>();
			if((isaEngine == null && !isPlaceforEngine) || (isPlaceforEngine && isaEngine.isEngine))
			{
			
				//go.transform.parent = gameObject.transform.parent.transform;
			go.transform.localPosition = gameObject.transform.position;
				
			Transform t = go.transform;
			t.parent = gameObject.transform.parent.transform;
			//t.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y, gameObject.transform.position.z - 10.0f);
			t.position = gameObject.transform.position;
			t.position.Set(gameObject.transform.position.x,gameObject.transform.position.y, gameObject.transform.position.z - 10.0f);
			//t.localRotation = Quaternion.identity;
			//t.localScale = gameObject.transform.localScale;
			//go.layer = gameObject.transform.parent.gameObject.layer;
			//	go.transform.localScale = gameObject.transform.localScale;
			//	go.transform.position = gameObject.transform.position;
			//GameObject child = NGUITools.AddChild(gameObject.transform.parent.gameObject, go);
			//Debug.Log(gameObject.transform.parent.transform.parent.name);
			//Transform trans = child.transform;
			//float newz = this.transform.position.z - 10.0f;
			//trans.position = new Vector3(this.transform.position.x,this.transform.position.y,newz);
			//trans.name = go.name;
			///trans.localScale.Set(this.transform.localScale.x,this.transform.localScale.y,this.transform.localScale.x);
			//GameObject.Find(go.name).transform.position.Set(this.transform.position.x,this.transform.position.y,newz);
			//	if (rotatePlacedObject) trans.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
			//Destroy(go);
			//this.GetComponent<BoxCollider>().enabled = false;
			//Destroy(UICamera.lastHit.collider.gameObject);
			}
		}
	}
}
