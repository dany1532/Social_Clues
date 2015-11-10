using UnityEngine;
using System.Collections;

public class ResourceManager {
	
	public static GameObject LoadPrefab(string prefabName)
	{
		return Resources.Load("Prefabs\\" + prefabName) as GameObject;
	}

	public static GameObject LoadNPCAchievements(string npcName)
	{
		return Resources.Load("NPCs/" + npcName + "/Achievement") as GameObject;
	}

	public static GameObject LoadNPCVoiceOver(string npcName, string characterName)
	{
		return Resources.Load("NPCs/" + npcName + "/VoiceOvers" + characterName) as GameObject;
	}

	public static GameObject LoadNPCConversation(string npcName)
	{
		return Resources.Load("NPCs/" + npcName + "/Conversation") as GameObject;
	}
	
	public static GameObject LoadNPCCutScene(string npcName)
	{
		return Resources.Load("NPCs/" + npcName + "/CutScene") as GameObject;
	}

	public static void UnloadAsset (GameObject asset)
	{
		Resources.UnloadAsset(asset);
	}
	
	public static GameObject LoadObject(string path)
	{
		return Resources.Load(path) as GameObject;
	}
	
	public static Texture LoadTexture(string path)
	{
		return Resources.Load(path) as Texture;
	}
	
	public static UIAtlas LoadAtlas(string path)
	{
		GameObject atlas = Resources.Load(path) as GameObject;
		if (atlas == null) return null;
		return atlas.GetComponent<UIAtlas>();
	}	
}
