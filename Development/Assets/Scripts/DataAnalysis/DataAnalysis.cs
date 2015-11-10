using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_WEBPLAYER
using System.Data.SQLite;
#endif

public class DataAnalysis : MonoBehaviour {
#if !UNITY_WEBPLAYER
	UILabel label;
	public class ResultEntry
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
		
		public override string ToString()
		{
			string returnString = "Level: \t" + LevelName + "\nInteraction: \t" + InteractionID + "\nNPC: \t" + NPCName + "\nPoint of communication: \t" + CommText;
			returnString += "\nCategory: \t" + Category + "\n Question: \t" + Question + "\nReply: \t" + Reply + "\n Points: \t" + Points + "\nReply time: \t" + ReplyTimeTaken;
			
			return returnString;
		}
	}
	List<ResultEntry> results;
	
	int currentResult = 0;
	
	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		
		label.text = MainDatabase.Instance.ConnectDB();
		
		results = new List<ResultEntry>();
		
		SQLiteDataReader reader = MainDatabase.Instance.SelectSql("Select LevelName, A.InteractionID, NPCName, CommText, Category, Q.Text, R.Text, ReplyType, Points, ReplyTimeTaken from Answer as A, Reply as R, Question as Q, Category as C, NPC as N, PointCommunication as P, Interaction as I, Level as L where A.ReplyID = R.ReplyID AND R.QuestionID = Q.QuestionID AND Q.CategoryID = C.CategoryID AND Q.NPCID = N.NPCID AND Q.CommID = P.CommID AND A.InteractionID = I.InteractionID AND I.LevelID = L.LevelID;");
		if (reader != null)
		{
			while(reader.Read()) {
				ResultEntry entry = new ResultEntry();
	         	entry.LevelName = reader.GetString(0);
				entry.InteractionID = reader.GetInt32(1);
				entry.NPCName = reader.GetString(2);
				entry.CommText = reader.GetString(3);
				entry.Category = reader.GetString(4);
				entry.Question = reader.GetString(5);
				entry.Reply = reader.GetString(6);
				entry.Points = reader.GetInt32(7);
				entry.ReplyTimeTaken = reader.GetFloat(8);
				results.Add(entry);
	       }
			
	       reader.Close();
	       reader = null;
		
			if (results.Count > 0)
				label.text = results[0].ToString();
			else
				label.text = "No results retrieved";
		}
		//	MainDatabase.InsertSql("INSERT INTO Progress VALUES (1,3,'Cafeteria','minigame1',2,'false',45)");
		MainDatabase.Instance.DisconnectDB();
	}
	
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			currentResult++;
			if (currentResult == results.Count)
				currentResult = 0;
			
			if (results.Count != 0)
				label.text = results[currentResult].ToString();
		}
	}
#endif
}
