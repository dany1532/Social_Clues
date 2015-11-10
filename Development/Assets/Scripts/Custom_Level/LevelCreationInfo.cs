using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelCharactersInfo
{
    
    public class Character
    {
        public int npcId;
        public string npcName;
        public string npcTexture;
        public int positionId;
		public DBToyInfo toyInfo;
    }

    public class CharacterComparer : IComparer<Character>
    {
        public int Compare(Character node1, Character node2)
        {
			if (node1.positionId == node2.positionId)
				return node1.npcName.CompareTo (node2.npcName);

			bool check1 = (node1.positionId < node2.positionId && node1.positionId > -1);
			bool check2 = (node2.positionId < 0);

            if (check1 || check2)
                return -1;
            else
                return 1;
        }
    }

    public List<Character> levelSpecificCharacters = new List<Character>();
    public List<Character> otherCharacters = new List<Character>();
}

[System.Serializable]
public class LevelCreationInfo
{
	
    public int 			levelId;
    public int 			levelIdx;
    public string 		levelName;
    public Texture		levelScreenshot;

    public LevelCharactersInfo levelCharacters;

    public void LoadCharacters()
    {
        if (levelCharacters == null)
            levelCharacters = MainDatabase.Instance.GetLevelsInfoExtended(levelId);
    }
}
