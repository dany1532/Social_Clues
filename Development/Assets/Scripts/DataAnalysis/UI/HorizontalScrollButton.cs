using UnityEngine;
using System.Collections;

public class HorizontalScrollButton : MonoBehaviour {
	public HorizontalScroll.Direction type;
	public GameObject scrollController;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		scrollController.SendMessage("scroll", type, SendMessageOptions.DontRequireReceiver);
		//HorizontalScroll.Instance.scroll(type);
	}
}
