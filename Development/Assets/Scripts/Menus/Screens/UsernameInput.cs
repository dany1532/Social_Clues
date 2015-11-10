using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class UsernameInput : MonoBehaviour
{

    public StoreUserSetup userSetup;
    private UIInput inputUI;
    void Start()
    {
        inputUI = gameObject.GetComponent<UIInput>();
    }
    void OnInput(string input)
    {
        if (inputUI.text == "Player" || inputUI.text.Trim() == "" || Regex.IsMatch(inputUI.text, @"\A(?=[^0-9]*[0-9])(?=[^A-Za-z]*[A-Za-z])\w+\Z", RegexOptions.IgnorePatternWhitespace))
        {
            userSetup.okayButton.color = Color.gray;
            userSetup.gameObject.collider.enabled = false;
        } else
        {
            if (userSetup.selectedToy != null)
            {
                userSetup.okayButton.color = userSetup.okayOriColor;
                userSetup.gameObject.collider.enabled = true;
            }
        }
    }
}
