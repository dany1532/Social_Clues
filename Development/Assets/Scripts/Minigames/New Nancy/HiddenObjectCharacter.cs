using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiddenObjectCharacter : MonoBehaviour {
	
	public string characterTexture;
	private Texture _characterTexture;
	
	public string objectsAtlas;
	private UIAtlas _objectAtlas;
	
	public string topicName;
	public Transform hiddenObjectsRoot;
	private List<HiddenObject> hiddenObjects;
	public bool selected = false;
	public bool used = false;
	public UIScaledTexture texture;
	int objectsAvailable = 0;
	
	// Use this for initialization
	void Awake () {
		hiddenObjects = new List<HiddenObject>();
		foreach(HiddenObject hiddenObject in hiddenObjectsRoot.GetComponentsInChildren<HiddenObject>())
		{
			hiddenObjects.Add(hiddenObject);
			hiddenObject.SetCharacter(this);
			hiddenObject.SetCollider(false);
		}
		objectsAvailable = hiddenObjects.Count;
	}

	public UIAtlas GetAtlas ()
	{
		return _objectAtlas;
	}
	
	public void LoadCharacter()
	{
		string root = "NPCs/Nancy/Minigame/" + topicName + "/";
		_characterTexture = ResourceManager.LoadTexture(root + characterTexture);
		_objectAtlas = ResourceManager.LoadAtlas(root + objectsAtlas);
		Debug.Log("atlas: " + root + objectsAtlas);
		selected = true;
		used = true;
		texture.SetTexture(_characterTexture, Color.white);	
	}
	
	public void UnloadCharacter()
	{
		Debug.Log (this.gameObject);
		selected = false;
		_characterTexture = null;
		_objectAtlas = null;
		
		foreach(HiddenObject hiddenObject in hiddenObjects)
			hiddenObject.SetCollider(false);
	}
	
	public void ResetCharacter(bool force)
	{
		if (!selected || force)
		{
			Debug.Log (this.gameObject);
			used = false;
			UnloadCharacter();
			foreach(HiddenObject hiddenObject in hiddenObjects)
				hiddenObject.SetCollider(false);
			objectsAvailable = hiddenObjects.Count;
		}
	}
	
	public HiddenObject GetRandomObject(HiddenObject.ObjectDifficulty diffculty)
	{
		if (objectsAvailable == 0) return null;
		
		foreach(HiddenObject hiddenObject in hiddenObjects)
			if (hiddenObject.selected == false && hiddenObject.difficulty == diffculty)
			{
				hiddenObject.SetCollider(true);
				hiddenObject.selected = true;
				objectsAvailable--;
			
				return hiddenObject;
			}
		return null;
	}
}
