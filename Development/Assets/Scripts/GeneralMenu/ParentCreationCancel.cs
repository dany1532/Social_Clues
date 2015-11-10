using UnityEngine;
using System.Collections;

public class ParentCreationCancel : MonoBehaviour {
	
	public StoreParentSetup parentData;
	
	void OnClick ()
	{
		parentData.ResetData();
	}
}
