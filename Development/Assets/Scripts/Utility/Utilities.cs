using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour {

	public static string SetTextureName (string texture)
	{
		return texture.Replace(' ', '_');
	}
}
