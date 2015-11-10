    // DatabaseExample.cs (outputs a line of text to the console)
using UnityEngine;
#if !UNITY_WEBPLAYER
using System.Data.SQLite;
#endif
/*
-- Table Script Creation
CREATE TABLE Level(
   LevelID INTEGER PRIMARY KEY
   ,   LevelName varchar(30)
);

CREATE TABLE NPC(
   NPCID INTEGER PRIMARY KEY
   ,   NPCName varchar(15)
   ,   NPCAtlas varchar(30)
   ,   NPCTexture varchar(30)
);

CREATE TABLE Interaction(
   InteractionID INTEGER PRIMARY KEY
   ,   NPCID Int
   ,   LevelID Int
   ,FOREIGN KEY(LevelID) REFERENCES Level(LevelID)
   ,FOREIGN KEY(NPCID) REFERENCES NPC(NPCID)
);

CREATE TABLE InteractionTime(
   InteractionID Int
   ,   StartTime DateTime
   ,   EndTime DateTime
   ,FOREIGN KEY(InteractionID) REFERENCES Interaction(InteractionID)
);

CREATE TABLE PointCommunication(
   CommID INTEGER PRIMARY KEY
   ,   CommText varchar(30)
);

CREATE TABLE Category(
   CategoryID INTEGER PRIMARY KEY
   ,   Category varchar(50)
);

CREATE TABLE Question(
   QuestionID INTEGER PRIMARY KEY
   ,   NPCID Int
   ,   CategoryID Int
   ,   CommID Int
   ,   Text varchar(255)
,FOREIGN KEY(NPCID) REFERENCES NPC(NPCID)
,FOREIGN KEY(CategoryID) REFERENCES Category(CategoryID)
,FOREIGN KEY(CommID) REFERENCES PointCommunication(CommID)
);

CREATE TABLE Reply(
   ReplyID INTEGER PRIMARY KEY
   ,   QuestionID Int
   ,   Points Int
   ,   Text varchar(255)
,FOREIGN KEY(QuestionID) REFERENCES Question(QuestionID)
);

CREATE TABLE Answer(
   ReplyID Int
   ,   ReplyType Int
   ,   ReplyTimeTaken FLOAT
   ,   InteractionID Int
,FOREIGN KEY(ReplyID) REFERENCES Reply(ReplyID)
);


INSERT INTO Level (LevelName)VALUES();
INSERT INTO NPC (NPCName,NPCAtlas,NPCTexture)VALUES();
INSERT INTO Interaction (NPCID,LevelID)VALUES();
INSERT INTO InteractionTime (InteractionID,StartTime,EndTime)VALUES();
INSERT INTO PointCommunication (CommText)VALUES();
INSERT INTO Category (Category)VALUES();
INSERT INTO Question (NPCID,CategoryID,CommID,Text)VALUES();
INSERT INTO Reply (QuestionID,Points,Text)VALUES();
INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES();

Select LevelName, A.InteractionID, NPCName, CommText, Category, Q.Text, R.Text, ReplyType, Points, ReplyTimeTaken from Answer as A, Reply as R, Question as Q, Category as C, NPC as N, PointCommunication as P, Interaction as I, Level as L where A.ReplyID = R.ReplyID AND R.QuestionID = Q.QuestionID AND Q.CategoryID = C.CategoryID AND Q.NPCID = N.NPCID AND Q.CommID = P.CommID AND A.InteractionID = I.InteractionID AND I.LevelID = L.LevelID;
*/
    public class DatabaseExample : MonoBehaviour {
     
#if !UNITY_WEBPLAYER
    void Start() {
    Debug.Log("Database: Example!");
		MainDatabase.Instance.ConnectDB();
		SQLiteDataReader reader = MainDatabase.Instance.SelectSql("Select * from Progress");
		 while(reader.Read()) {
         string LevelId = reader.GetString (0);
         string DialogId = reader.GetString (1);
         string LevelName = reader.GetString (2);
         string DialogName = reader.GetString (3);
         string DialogType = reader.GetString (4);
         string Ans = reader.GetString (5);
		 string AnsTime = reader.GetString (6);
		 // Print to Console
  		 Debug.Log ( LevelId + " " + DialogId + " " + LevelName + " " + DialogName + " " + DialogType + " " + Ans + " " + AnsTime );
       }
		
       reader.Close();
       reader = null;
	
	//	MainDatabase.InsertSql("INSERT INTO Progress VALUES (1,3,'Cafeteria','minigame1',2,'false',45)");
	MainDatabase.Instance.DisconnectDB();
		
    }
#endif
}