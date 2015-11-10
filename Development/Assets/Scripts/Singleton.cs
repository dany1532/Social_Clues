using UnityEngine;
using System.Collections;

/***
 * Singleton Class used for Managers
***/
public class Singleton<T> : MonoBehaviour 
where T : MonoBehaviour
{
    protected static T instance;
	
    //Returns the instance of this singleton
    public static T Instance
    {
        get
        {
            // if the instance has not been initialized yet
            if (instance == null)
            {
                // try finding the object in the scene
                instance = (T)FindObjectOfType(typeof(T));
		
                // if object not found
                if (instance == null)
                {
                    // then make a new instance of the game object
                    GameObject container = new GameObject();
                    container.name = typeof(T) + "Container";
                    instance = (T)container.AddComponent(typeof(T));
                }
            }
		
            return instance;
        }
    }
	
    // Destroy current instance of singleton
    public static void DestroyInstance()
    {
        if (instance != null)
        {
            if (instance.gameObject != null)
                Destroy(instance.gameObject);
            instance = null;
        }
    }

    public static bool WasInitialized()
    {
        return instance != null;
    }
}