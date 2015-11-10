using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DBSelectAll
{
	
    public string LevelName;
    public int InteractionID;
    public string NPCName;
    public string CommText;
    public string Category;
    public string Question;
    public string Reply;
    public int Points;
    public float ReplyTimeTaken;
}

public class DBReplyTimeWithType
{
    public float Timetaken;
    public int ReplyType;
    public string ReplyText;
    public int ReplyPoints;
}

public class DBNPCInteractionTime
{
    public int totalInteractionTimeSec;
}

public class DBNPCInteraction
{
    public int NPCID;
    public string NPCName;
    public string NPCAtlas;
    public string NPCTexture;
    public string ToyName;
    public float ToyColorR;
    public float ToyColorG;
    public float ToyColorB;
    public int InteractionID;
    public int Stars;
	public string Category;
}

public class DBNPCMiniGameInfo
{
    public string NPCName;
    public string NPCNameParent;
    public string NPCNameChild;
    public string NPCTexture;
    public string NPC2Texture;
    public string NPCPortrait;
    public string MiniGame;
    public string MiniGameDescription;
    public int Stars;
}

public class DBToyInfo
{
    public int ToyID;
    public string ToyName;
    public string Filename;
    public int ToyColorR;
    public int ToyColorG;
    public int ToyColorB;
	public int UserID;
}

public class DBUserInfo
{
    public int UserID;
    public string UserName;
    public string ToyFilename;
    public int ToyColorR;
    public int ToyColorG;
    public int ToyColorB;
}

public class DBUserNPCStatus
{
    public int NPCID;
    public int Status;
}

public class DBIncompleteLevel
{
    public int LevelPlayID;
    public string LevelName;
}

public class DBUserSettings
{
    public float VolumeMusic;
    public float VolumeSFX;
    public int BackgroundTrack;
    public float DialogueProcessingTime;
    public bool ExitConversationOption;
    public bool InstantAnswer;
    public bool ExitMinigameOption;
}

public class DBLevel
{
    public int levelId;
    public string levelName;
    public int levelIdx;
    public string levelTexture;
}

public class DBCompletedNPCs
{
	public int NPCID;
	public int stars;
	public string NPCName;
	public string Category;
}

public class DBTotalPercentageWithDate
{
	public int e;
	public int c;
	public int i;
	public int m;
	public int p;
	public DateTime date;
}

public struct DBAvgECIMPwithNPCID
{
	public float e;
	public float c;
	public float i;
	public float m;
	public float p;
	public int npcID;
}