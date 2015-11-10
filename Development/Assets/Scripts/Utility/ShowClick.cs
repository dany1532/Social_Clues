using UnityEngine;
using System.Collections;

public class ShowClick : MonoBehaviour
{
    public UITexture clickTexture;
    public float displayTime = 1f;
    Vector3 pos = Vector3.zero;
    Vector2 dim;
    Vector2 center;
    float aspectRatio = (4.0f / 3);
    
    void Start()
    {        
        dim = new Vector2(Screen.width, Screen.height);
        center = new Vector2(0.5f * aspectRatio, 0.5f);
        clickTexture.enabled = false;
        pos.z = -1;
    }
    
    void Update()
    {
        if (ApplicationState.Instance.presentationBuild)
        {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches [0];
            
            if (touch.phase == TouchPhase.Began)
                clickTexture.enabled = true;
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                clickTexture.enabled = false;
            else
            {
                pos.x = 2 * (touch.position.x / dim.x * aspectRatio - center.x);
                pos.y = 2 * (touch.position.y / dim.y - center.y);
                transform.position = pos;
            }
        }
#else
            if (Input.GetMouseButtonDown(0))
            {
                clickTexture.enabled = true;
            } else if (Input.GetMouseButtonUp(0))
            {
                clickTexture.enabled = false;
            } else if (Input.GetMouseButton(0))
            {
                pos.x = 2 * (Input.mousePosition.x / dim.x * aspectRatio - center.x);
                pos.y = 2 * (Input.mousePosition.y / dim.y - center.y);
                transform.position = pos;
            }
#endif
        }
    }
}
