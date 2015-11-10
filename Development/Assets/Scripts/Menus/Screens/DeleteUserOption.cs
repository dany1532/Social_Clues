using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteUserOption : MonoBehaviour {
	public List<DeleteUserButton> deleteButtons;
	
	// Use this for initialization
	void Start () {
		HideAll();
	}
	
	void Update()
	{
		if (InputManager.Instance.HasReceivedClick())
		{
			HideAll();
		}
	}
	
	void OnEnable()
	{
		HideAll();
	}
	
	void OnClick()
	{
		InputManager.Instance.ReceivedUIInput();
		ShowAll();
	}
	
	public void ShowAll()
	{
		foreach(DeleteUserButton button in deleteButtons)
		{
			button.Show();
		}
	}
	
	public void HideAll()
	{
		foreach(DeleteUserButton button in deleteButtons)
		{
			button.Hide();
		}
	}
}
