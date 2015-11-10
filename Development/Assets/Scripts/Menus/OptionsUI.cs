using UnityEngine;
using System.Collections;

public class OptionsUI : MonoBehaviour
{
    public GameObject settingsMenu;
    public Transform parent;

    void OnClick()
    {
        GameObject menu = Instantiate(settingsMenu) as GameObject;
		menu.transform.parent = parent;
        menu.transform.localScale = new Vector3(1, 1, 1);
        menu.transform.localPosition = new Vector3(0, 0, -20);
    }
}
