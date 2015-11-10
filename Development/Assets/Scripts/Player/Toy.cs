using UnityEngine;
using System.Collections;

[System.Serializable]
public class Toy {

	public string filename;
	public string displayName;
	public Color color;
	
	public Toy()
	{
		filename = "";
		displayName = "";
		color = Color.white;
	}
	
	public Toy(string _filename, string _displayName, Color _color)
	{
		filename = _filename;
		displayName = _displayName;
		color = _color;
	}
}
