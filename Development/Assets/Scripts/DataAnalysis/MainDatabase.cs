using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_WEBPLAYER 
// For SQLite
using Mono.Data;
using System.Data.SQLite;
using Mono.Data.SqliteClient;

#endif
public class MainDatabase : Singleton<MainDatabase>
{
#if !UNITY_WEBPLAYER
    SQLiteConnection dbcon = null;
    public bool useDB = true;
    public bool overwriteDB = false;
	public static int VERSION_NO = 59;
    
#endif
    void Awake()
    {
        if (instance != null)
        {
			if (instance != this)
           		Destroy(this.gameObject);
            return;
        }
		
        instance = this;
        DontDestroyOnLoad(this.gameObject);
		
        // Check connection or need to overwrite database
        if (ConnectDB() == "OK")
        {
            SQLiteDataReader reader = MainDatabase.Instance.SelectSql("SELECT name FROM sqlite_master WHERE type='table';");
            reader.Read();
            if (!reader.HasRows)
            {
                overwriteDB = true;
            }
            else
            {
                overwriteDB = true;
                do{
                    if (reader.GetString(0) == "VersionControl")
                    {
                        overwriteDB = false;
                    }
                }while (reader.Read());
                
                if (!overwriteDB)
                {
                    int versionNo = MainDatabase.Instance.getIDs("SELECT ifnull(version, -1) FROM VersionControl;");
                    if (versionNo != VERSION_NO)
                        overwriteDB = true;
                }
            }
            				
            reader.Close();
            reader = null;
            DisconnectDB();
        } else
            overwriteDB = true;
        
        #if UNITY_EDITOR			
        #elif UNITY_IPHONE
		if (System.IO.File.Exists(Application.persistentDataPath +"/SocialCluesDB.db"))
		{
			if (overwriteDB)
			{
				System.IO.File.Delete(Application.persistentDataPath +"/SocialCluesDB.db");
				System.IO.File.Copy(Application.dataPath.Replace("/Data","") +"/SocialCluesDB.db", Application.persistentDataPath +"/SocialCluesDB.db");
				Debug.Log("Deleted database to re-write");
			}			
		}
		else
		{
			Debug.Log("Checking DB file in Documents: " + System.IO.File.Exists(Application.persistentDataPath +"/SocialCluesDB.db").ToString());
			Debug.Log("Found DB file at App Path moving to Documents: " + System.IO.File.Exists(Application.dataPath.Replace("/Data","") +"/SocialCluesDB.db").ToString());
			System.IO.File.Copy(Application.dataPath.Replace("/Data","") +"/SocialCluesDB.db", Application.persistentDataPath +"/SocialCluesDB.db");
		}
        #elif UNITY_ANDROID
		if(System.IO.File.Exists(Application.persistentDataPath +"/SocialCluesDB.db"))
		{
			if (overwriteDB)
			{
				System.IO.File.Delete(Application.persistentDataPath +"/SocialCluesDB.db");
			    WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/SocialCluesDB.db");  // this is the path to your StreamingAssets in android
				while(!loadDB.isDone)
				{
				}  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check		
				System.IO.File.WriteAllBytes(Application.persistentDataPath +"/SocialCluesDB.db", loadDB.bytes);
			}
		}
		else
		{
		    WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/SocialCluesDB.db");  // this is the path to your StreamingAssets in android
			while(!loadDB.isDone)
			{
			}  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check		
			System.IO.File.WriteAllBytes(Application.persistentDataPath +"/SocialCluesDB.db", loadDB.bytes);
		}
		
        #endif
    }
	
    // Called on applicaiton quit to disconnect from DB
    void OnApplicationQuit()
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            DisconnectDB();
        }
#endif
    }
	
    public string ConnectDB()
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            if (dbcon == null)
            {
#endif
                #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN
                string connectionString = "URI=file:SocialCluesDB.db";
                #elif UNITY_IPHONE || UNITY_ANDROID
			    string connectionString = "URI=file:" + Application.persistentDataPath +"/SocialCluesDB.db";
                #elif !UNITY_WEBPLAYER
				string connectionString = "URI=file:SocialCluesDB.db";
                #endif
                #if !UNITY_WEBPLAYER
                try
                {
                    dbcon = (SQLiteConnection)new SQLiteConnection(connectionString);
                    dbcon.Open();
									
                    Debug.Log("Database: Database Connected!");
                } catch (SqliteExecutionException e)
                {
                    return e.ToString();
                }
                return "OK";
            } else
            {
                return "Database: Database Already Connected!";
            }
        }
        #endif
        return "Database not in use";
    }
	
    public int GetDatabaseVersion()
    {
        return MainDatabase.Instance.getIDs("SELECT ifnull(version, -1) FROM VersionControl;");
    }
    
    public string InsertSql(string sql)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            ConnectDB();
            SQLiteCommand dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = sql;
            try
            {
                dbcmd.ExecuteNonQuery();
                dbcmd.Dispose();
            } catch
            {
                Debug.LogWarning("Eror during query execution");
                return "Error during query execution";
            }
            //Debug.Log("Database: Inserted Data!");
            return "Database: Inserted Data!";
        }
#endif
        return "Database not in use";
    }
	
    public void UpdateSql(string sql)
    {
#if !UNITY_WEBPLAYER
        InsertSql(sql);
#endif
    }
	
    /**
	 * Status 	0 = not start
	 * 			1 = Completed
	 * 			-1= Not there
	 **/
    public void AddUserNPCList(int userID, int NPC1ID, int NPC1Status, int NPC2ID, int NPC2Status, int NPC3ID, int NPC3Status, int NPC4ID, int NPC4Status)
    {
#if !UNITY_WEBPLAYER
        string sql = "UPDATE User SET NPC1 = " + NPC1ID + ", NPC1Status = " + NPC1Status + ", "
            + "NPC2 = " + NPC2ID + ", NPC2Status = " + NPC2Status + ", "
            + "NPC3 = " + NPC3ID + ", NPC3Status = " + NPC3Status + ", "
            + "NPC4 = " + NPC4ID + ", NPC4Status = " + NPC4Status + " WHERE UserID = " + userID + ";";
        InsertSql(sql);
#endif
	}

	public void UpdateUserCharacter(int userID, string character)
	{
		#if !UNITY_WEBPLAYER
		string sql = "UPDATE User SET Gender = '" + character + "' WHERE UserID = " + userID + ";";
		InsertSql(sql);
		#endif
	}
	
	public string GetUserCharacter(int userID)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{   
			SQLiteDataReader reader = MainDatabase.Instance.SelectSql("Select Gender FROM User WHERE UserID = +" + userID + ";");
			string character = "Pete";
			
			reader.Read();
			List<DBUserNPCStatus> NPCStatusInfo = new List<DBUserNPCStatus>();
			if (reader.HasRows)
			{
				character = reader.GetString(0);

				reader.Close();
				reader = null;
			} else
			{
				Debug.LogWarning("DB: no entry found for userID");
				return "Pete";
			}
			
			return character;
		}
		#endif
		return "Pete";
	}

    /// <summary>
    /// Deletes the user with the given ID from database
    /// </summary>
    /// <param name='userID'>
    /// User I.
    /// </param>
    public void DeleteUser(int userID)
    {
#if !UNITY_WEBPLAYER
        MainDatabase.Instance.UpdateSql("UPDATE User SET active = 0 where UserID = " + userID + ";");
#endif
    }
	
    /**
	 * Status 	0 = not start
	 * 			1 = Completed
	 * 			-1= Not there
	 **/
    public int ChangeUserNPCStatus(int userID, int NPCID, int NPCStatus)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {		
            string NPCnumber = "";
            SQLiteDataReader reader = MainDatabase.Instance.SelectSql("Select NPC1,NPC2,NPC3,NPC4 FROM User WHERE UserID = +" + userID + ";");
            reader.Read();
            int readInt = 0;
            if (reader.HasRows)
            {
                if (NPCID == reader.GetInt32(0))
                    NPCnumber = "NPC1";
                else if (NPCID == reader.GetInt32(1))	
                    NPCnumber = "NPC2";
                else if (NPCID == reader.GetInt32(2))
                    NPCnumber = "NPC3";
                else if (NPCID == reader.GetInt32(3))	
                    NPCnumber = "NPC4";
                else
                {
                    readInt = -1;
                }
				
                reader.Close();
                reader = null;
                string sql = "UPDATE User SET " + NPCnumber + "Status = " + NPCStatus + " WHERE UserID = " + userID + ";";
                InsertSql(sql);
            } else
            {
                readInt = -1;
                Debug.LogWarning("DB: no entry found for NPCID");
            }

            return readInt;
        }
#endif
        return -1;
    }
	/**
	 * Status 	0 = not start
	 * 			1 = Completed
	 * 			-1= Not there
	 **/	
    public List<DBUserNPCStatus> GetUserNPCStatus(int userID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {	
            string NPCnumber = "NPC4";
            SQLiteDataReader reader = MainDatabase.Instance.SelectSql("Select NPC1,NPC1Status,NPC2,NPC2Status,NPC3,NPC3Status,NPC4,NPC4Status FROM User WHERE UserID = +" + userID + ";");
            reader.Read();
            List<DBUserNPCStatus> NPCStatusInfo = new List<DBUserNPCStatus>();
            if (reader.HasRows)
            {
                DBUserNPCStatus record = new DBUserNPCStatus();
                record.NPCID = reader.GetInt32(0);
                record.Status = reader.GetInt32(1);
                NPCStatusInfo.Add(record);
				
                record = new DBUserNPCStatus();
                record.NPCID = reader.GetInt32(2);
                record.Status = reader.GetInt32(3);
                NPCStatusInfo.Add(record);
				
                record = new DBUserNPCStatus();
                record.NPCID = reader.GetInt32(4);
                record.Status = reader.GetInt32(5);
                NPCStatusInfo.Add(record);
				
                record = new DBUserNPCStatus();
                record.NPCID = reader.GetInt32(6);
                record.Status = reader.GetInt32(7);
                NPCStatusInfo.Add(record);
				
                reader.Close();
                reader = null;
            } else
            {
                Debug.LogWarning("DB: no entry found for userID");
                return null;
            }

            return NPCStatusInfo;
        }
#endif
        return null;
    }	
    
    
    public int GetLevelForUser(int userID)
    {
        #if !UNITY_WEBPLAYER
        if (useDB)
        {   
            SQLiteDataReader reader = MainDatabase.Instance.SelectSql("Select MaxLevel FROM User WHERE UserID = +" + userID + ";");
            int maxLevel = 1;

            reader.Read();
            List<DBUserNPCStatus> NPCStatusInfo = new List<DBUserNPCStatus>();
            if (reader.HasRows)
            {
                maxLevel = reader.GetInt32(0);
                
                reader.Close();
                reader = null;
            } else
            {
                Debug.LogWarning("DB: no entry found for userID");
                return 1;
            }
            
            return maxLevel;
        }
        #endif
        return 1;
    }

	public int GetStars(int userID, int npcid)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{   
			SQLiteDataReader reader = MainDatabase.Instance.SelectSql("select count(InteractionID) from Interaction I, LevelPlay L where L.LevelPlayID = I.LevelPlayID and userid = " + userID + " and npcid = " + npcid + ";");

			reader.Read();
			int numberOfInteraction;
			if (reader.HasRows)
			{
				numberOfInteraction = reader.GetInt32(0);
				
				reader.Close();
				reader = null;
			} else
			{
				Debug.LogWarning("DB: no entry found for userID and npcid:");
				return -1;
			}
			
			return numberOfInteraction;
		}
		#endif
		return -1;
	}

#if !UNITY_WEBPLAYER
    public SQLiteDataReader SelectSql(string sql)
    {
#else
	public GameObject SelectSql(string sql) {
#endif
		
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            ConnectDB();
            SQLiteCommand dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = sql;
            SQLiteDataReader reader = dbcmd.ExecuteReader();
            dbcmd.Dispose();
            //Debug.Log("Database: Read Data!");
            return reader;
        }
#endif
        return null;
    }
	
    /* 
	  * Helps to get ID from any table
	  * Example: Select NPCID from NPC where NPCName = 'Amy';
	  * */
    public int getIDs(string sql)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            ConnectDB();
            SQLiteCommand dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = sql;
            SQLiteDataReader reader = dbcmd.ExecuteReader();
            dbcmd.Dispose();
            Int16 readInt;
            reader.Read();
            if (reader.HasRows)
            {
                readInt = reader.GetInt16(0);
            } else
            {
                readInt = -1;
                Debug.LogWarning("DB: no entry found for \n" + sql);
            }
			
            reader.Close();
            reader = null;
				
            return readInt;
        }
#endif
        return -1;
    }
		
    /* 
	  * Helps to get Name from any table
	  * Example: Select NPCID from NPC where NPCName = 'Amy';
	  * */
    public string getName(string sql)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            ConnectDB();
            SQLiteCommand dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = sql;
            SQLiteDataReader reader = dbcmd.ExecuteReader();
            dbcmd.Dispose();
            string readInt;
            reader.Read();
            if (reader.HasRows)
            {
                readInt = reader.GetString(0);
            } else
            {
                readInt = "-1";
                Debug.LogWarning("DB: no entry found for \n" + sql);
            }
			
            reader.Close();
            reader = null;
				
            return readInt;
        }
#endif
        return "-1";
    }
		
		public int getLevelFromName(string levelName)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				//int userID = ApplicationState.Instance.userID;
				SQLiteDataReader reader = SelectSql("SELECT ifnull(MAX(LevelID),-1)from Level L where L.LevelName = '" + levelName + "';");
            reader.Read();
         
            int levelId = -1;
            if (reader.HasRows)
            {
                levelId = reader.GetInt32(0);
            } else
            {
                levelId = -1;
                Debug.LogWarning("DB: Level " + levelName + " not found.");
            }
         
            reader.Close();
            reader = null;
             
            return levelId;
        }
#endif
        return 1;
    }
    
    public DBIncompleteLevel getIncompleteLevel(int userID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT ifnull(MAX(LPI.LevelPlayID),-1), L.LevelName, U.NPC1, U.NPC1Status, U.NPC2, U.NPC2Status, U.NPC3, U.NPC3Status, U.NPC4, U.NPC4Status from LevelPlay P, levelplayInstance LPI, Level L, User U where L.LevelID = P.LevelID AND P.LevelPlayID = LPI.LevelPlayID AND U.UserID = P.UserID AND U.UserID = " + userID + " AND endtime <> '';");// ORDER P.LevelPlayID Desc LIMIT 0, 1");
            reader.Read();
			
            DBIncompleteLevel incompleteLevel = new DBIncompleteLevel();
            if (reader.HasRows)
            {
                incompleteLevel.LevelPlayID = reader.GetInt32(0);
                if (incompleteLevel.LevelPlayID != -1)
                {
                    incompleteLevel.LevelName = reader.GetString(1);
                    bool isIncomplete = false;
                    for (int i = 2; i < 8; i+= 2)
                    {
                        if (reader.GetInt32(i) >= 0 && reader.GetInt32(i + 1) == 0)
                        {
                            isIncomplete = true;
                            break;
                        }
                    }

                    if (!isIncomplete)
                        incompleteLevel.LevelPlayID = -1;
                }
            } else
            {
                incompleteLevel.LevelPlayID = -1;
                Debug.LogWarning("DB: no entry found for max levelplayid");
            }
			
            reader.Close();
            reader = null;
				
            return incompleteLevel;
        }
#endif
        return null;
    }
	
    public int getLevelPlayID(int userID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT MAX(P.LevelPlayID)  from LevelPlay P, levelplayInstance L where P.LevelPlayID = L.LevelPlayID AND userID = " + userID + " AND endtime <> ''");
            reader.Read();
            int readInt;
            if (reader.HasRows)
            {
                readInt = reader.GetInt32(0);
            } else
            {
                readInt = -1;
                Debug.LogWarning("DB: no entry found for max levelplayid");
            }
			
            reader.Close();
            reader = null;
				
            return readInt;
        }
#endif
        return -1;
    }
		
    public List<int> getLevelPlayIDs(int userID, int levelId)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT P.LevelPlayID  from LevelPlay P where userID = " + userID + " and LevelPlayID <= " + levelId + ";");
            List<int> readlist = new List<int>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    readlist.Add(reader.GetInt32(0));
                }
            } else
            {
                readlist = null;
                Debug.LogWarning("DB: no entry found for max levelplayid");
            }
			
            reader.Close();
            reader = null;
				
            return readlist;
        }
#endif
        return null;
    }		
	
    /*
	 * Returns List of percentage of ALL right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */
    public List<float> calTotalPercentage(int NPCID, int userID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {	
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT ifnull(EmotionsScore,-1), ifnull(ComprehensionScore,-1), ifnull(ConversationStartScore,-1), ifnull(MaintainConversationScore,-1), ifnull(ProblemSolvingScore,-1) "
                + "FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND L.UserID = " + userID + " AND I.NPCID = " + NPCID + ";");
			
            List<float> Scores = new List<float>();
            List<float> ScoresCounts = new List<float>();
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
				
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i=0; i<5; i++)
                    {
                        if (reader.GetInt32(i) > -1)
                        {
                            ScoresCounts [i] += 1;
                            Scores [i] += reader.GetInt32(i);
                        }
                    }
                }
                for (int i=0; i<5; i++)
                {
                    if (ScoresCounts [i] > 0)
                        Scores [i] = Scores [i] / ScoresCounts [i];
                    else
                        Scores [i] = 0;
                }
            } else
            {
                Scores = null;
                Debug.LogWarning("DB: no entry found for NPCID \n" + NPCID);
            }
			
            return Scores;
        }
#endif
        return null;
    }	
	
    /*
	 * Returns List of percentage of interactionID right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */	
    public List<float> calPercentageForInteractionID(int interactionID, int userID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT ifnull(EmotionsScore,-1), ifnull(ComprehensionScore,-1), ifnull(ConversationStartScore,-1), ifnull(MaintainConversationScore,-1), ifnull(ProblemSolvingScore,-1) "
                + "FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND L.UserID = " + userID + " AND I.interactionid = " + interactionID + ";");
			
            List<float> Scores = new List<float>();
            if (reader.HasRows)
            {
                reader.Read();
                for (int i=0; i<5; i++)
                {
                    if (reader.GetInt32(i) > -1)
                    {
                        Scores.Add(reader.GetInt32(i));
                    }
                }
            } else
            {
                Scores = null;
                Debug.LogWarning("DB: no entry found for interactionID \n" + interactionID);
            }
			
            return Scores;				
        }
#endif
        return null;
    }		

    /*
	 * Returns List of percentage of interactionID right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */		
    public List<float> calPercentageForPreviousInteractionID(int interactionID, int NPCID, int userID)
    {
        #if !UNITY_WEBPLAYER
        if (useDB)
        {
            do
            {
                //int userID = ApplicationState.Instance.userID;
                SQLiteDataReader previousInteractionSQL = SelectSql("SELECT I.InteractionID FROM Interaction I, LevelPlay L "
                    + " WHERE I.LevelPLayID = L.LevelPlayID AND NPCID = " + NPCID + " AND UserID = " + userID
                    + " AND I.InteractionID <= " + interactionID + " ORDER BY InteractionID Desc LIMIT 1,1;");
                /*SQLiteDataReader previousInteractionSQL = SelectSql("SELECT ifnull(InteractionID,-1) FROM [Interaction]"
    			//+"WHERE NPCID IN ( SELECT NPCID FROM Interaction ORDER BY InteractionID Desc LIMIT 0,1)"
    			+"WHERE NPCID = "+NPCID+";"
    			+"ORDER BY InteractionID Desc LIMIT 1,1");// AND interactionid = "+interactionID+";");
    			*/
                previousInteractionSQL.Read();
                List<float> percentage = new List<float>();
                if (previousInteractionSQL.HasRows)
                {
                    interactionID = previousInteractionSQL.GetInt16(0);
                } else
                    interactionID = -1;

                previousInteractionSQL.Close();
                previousInteractionSQL = null;

                if (interactionID >= 0)
                {
                    percentage = calPercentageForInteractionID(interactionID, userID);
                    
                    if (percentage != null && percentage.Count > 0)
                    {
                        int counter = 0;
                        for (; counter < percentage.Count; counter++)
                        {
                            if (percentage [counter] <= 0)
                                break;
                        }
                        if (counter == percentage.Count)
                            return percentage;
                    }
                }
            } while(interactionID >= 0);
        }
        #endif
        return null;
    }

		/*
	 * Returns List of percentage of interactionID right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */		
		public List<float> calPercentageForLatestInteractionID(int userID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
					SQLiteDataReader previousInteractionSQL = SelectSql("SELECT  ifnull(max(I.InteractionID),-1) FROM Interaction I, LevelPlay L WHERE Completed= 'True' AND I.LevelPLayID = L.LevelPlayID " +
					                                                    "AND UserID = "+userID+";");
					int interactionID;
					previousInteractionSQL.Read();
					List<float> percentage = new List<float>();
					if (previousInteractionSQL.HasRows)
					{
						interactionID = previousInteractionSQL.GetInt16(0);
					} else
						interactionID = -1;
					
					previousInteractionSQL.Close();
					previousInteractionSQL = null;
					
					if (interactionID >= 0)
					{
						percentage = calPercentageForInteractionID(interactionID, userID);
						
						if (percentage != null && percentage.Count > 0)
						{
							int counter = 0;
							for (; counter < percentage.Count; counter++)
							{
								if (percentage [counter] <= 0)
									break;
							}
							if (counter == percentage.Count)
								return percentage;
						}
					}
			}
			#endif
			return null;
		}

		/*
	 * Returns List of percentage of interactionID right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */		
		public List<float> calPercentageForFirstInteractionID(int userID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
					//int userID = ApplicationState.Instance.userID;
				SQLiteDataReader previousInteractionSQL = SelectSql("SELECT ifnull(min(I.InteractionID),-1) FROM Interaction I, LevelPlay L WHERE Completed= 'True' AND I.LevelPLayID = L.LevelPlayID " +
					                                                    "AND UserID = "+userID+";");
					/*SQLiteDataReader previousInteractionSQL = SelectSql("SELECT ifnull(InteractionID,-1) FROM [Interaction]"
    			//+"WHERE NPCID IN ( SELECT NPCID FROM Interaction ORDER BY InteractionID Desc LIMIT 0,1)"
    			+"WHERE NPCID = "+NPCID+";"
    			+"ORDER BY InteractionID Desc LIMIT 1,1");// AND interactionid = "+interactionID+";");
    			*/
				int interactionID;
					previousInteractionSQL.Read();
					List<float> percentage = new List<float>();
					if (previousInteractionSQL.HasRows)
					{
						interactionID = previousInteractionSQL.GetInt16(0);
					} else
						interactionID = -1;
					
					previousInteractionSQL.Close();
					previousInteractionSQL = null;
					
					if (interactionID >= 0)
					{
						percentage = calPercentageForInteractionID(interactionID, userID);
						
						if (percentage != null && percentage.Count > 0)
						{
							int counter = 0;
							for (; counter < percentage.Count; counter++)
							{
								if (percentage [counter] <= 0)
									break;
							}
							if (counter == percentage.Count)
								return percentage;
						}
					}
			}
			#endif
			return null;
		}


    /*
	 * Returns List of percentage of ALL right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */
    public List<float> calTotalPercentageECIMP(int userID, int levelID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {	
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT ifnull(EmotionsScore,-1), ifnull(ComprehensionScore,-1), ifnull(ConversationStartScore,-1), ifnull(MaintainConversationScore,-1), ifnull(ProblemSolvingScore,-1) "
                + "FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND L.UserID = " + userID + " and L.LevelPlayID = " + levelID + ";");
			
            List<float> Scores = new List<float>();
            List<float> ScoresCounts = new List<float>();
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i=0; i<5; i++)
                    {
                        if (reader.GetInt32(i) > 0)
                        {
                            ScoresCounts [i] += 1;
                            Scores [i] += reader.GetInt32(i);
                        }
                    }
                }
                for (int i=0; i<5; i++)
                {
                    if (ScoresCounts [i] > 0)
                        Scores [i] = Mathf.RoundToInt(Scores [i] / ScoresCounts [i]);
                    else
                        Scores [i] = 0;
                }
            } else
            {
                Scores = null;
                Debug.LogWarning("DB: no entry found for userID \n" + userID);
            }
			
            return Scores;
        }
#endif
        return null;
    }

		public List<DBTotalPercentageWithDate> calTotalPercentageECIMPwithDATES(int userID, int NPCID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{	
				//int userID = ApplicationState.Instance.userID;
				SQLiteDataReader reader = SelectSql("SELECT ifnull(EmotionsScore,-1), ifnull(ComprehensionScore,-1), ifnull(ConversationStartScore,-1), ifnull(MaintainConversationScore,-1), ifnull(ProblemSolvingScore,-1) "
				                                    + ", ifnull(Endtime,StartTime) FROM Interaction I, LevelPlay L, InteractionTime T WHERE I.InteractionID = T.InteractionID AND I.LevelPlayID = L.LevelPlayID AND Completed = 'True' AND L.UserID = " + userID + " AND I.NPCID = "+NPCID+" ;");
				
				List<DBTotalPercentageWithDate> results = new List<DBTotalPercentageWithDate>();
				while (reader.Read())
				{
					DBTotalPercentageWithDate entry = new DBTotalPercentageWithDate();
					
					if (reader.HasRows)
					{
						entry.e = reader.GetInt32(0);
						entry.c = reader.GetInt32(1);
						entry.i = reader.GetInt32(2);
						entry.m = reader.GetInt32(3);
						entry.p = reader.GetInt32(4);
						entry.date = reader.GetDateTime(5);
						results.Add(entry);
					}
				}
				
				
				reader.Close();
				reader = null;
				
				return results;
			}
			#endif
			return null;
		}

	/*
	 * Returns List of percentage of ALL right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */
    public List<float> calTotalPercentageECIMPForPlay(int userID, int levelID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {	
            //int userID = ApplicationState.Instance.userID;
            SQLiteDataReader reader = SelectSql("SELECT ifnull(EmotionsScore,-1), ifnull(ComprehensionScore,-1), ifnull(ConversationStartScore,-1), ifnull(MaintainConversationScore,-1), ifnull(ProblemSolvingScore,-1) "
                + "FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND L.UserID = " + userID + " and L.LevelPlayID = " + levelID + ";");
			
            List<float> Scores = new List<float>();
            List<float> ScoresCounts = new List<float>();
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            Scores.Add(0.0f);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            ScoresCounts.Add(0);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i=0; i<5; i++)
                    {
                        if (reader.GetInt32(i) > 0)
                        {
                            ScoresCounts [i] += 1;
                            Scores [i] += reader.GetInt32(i);
                        }
                    }
                }
                for (int i=0; i<5; i++)
                {
                    if (ScoresCounts [i] > 0)
                        Scores [i] = Mathf.RoundToInt(Scores [i] / ScoresCounts [i]);
                    else
                        Scores [i] = 0;
                }
            } else
            {
                Scores = null;
                Debug.LogWarning("DB: no entry found for userID \n" + userID);
            }
			
            return Scores;
        }
#endif
        return null;
    }
		//SELECT NPCID, avg(ifnull(EmotionsScore,0)), avg(ifnull(ComprehensionScore,0)), avg(ifnull(ConversationStartScore,0)), avg(ifnull(MaintainConversationScore,0)), avg(ifnull(ProblemSolvingScore,0)) FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND Completed = 'True' AND L.UserID = 2 Group by NPCID;
		/*
	 * Returns List of percentage of ALL right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */
		public List<float> calTotalAveragePercentageECIMP(int userID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{	
				//int userID = ApplicationState.Instance.userID;
				SQLiteDataReader reader = SelectSql("SELECT ifnull(EmotionsScore,-1), ifnull(ComprehensionScore,-1), ifnull(ConversationStartScore,-1), ifnull(MaintainConversationScore,-1), ifnull(ProblemSolvingScore,-1) "
				                                    + "FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND Completed = 'True' AND L.UserID = " + userID + ";");
				
				List<float> Scores = new List<float>();
				List<float> ScoresCounts = new List<float>();
				Scores.Add(0.0f);
				Scores.Add(0.0f);
				Scores.Add(0.0f);
				Scores.Add(0.0f);
				Scores.Add(0.0f);
				ScoresCounts.Add(0);
				ScoresCounts.Add(0);
				ScoresCounts.Add(0);
				ScoresCounts.Add(0);
				ScoresCounts.Add(0);
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						for (int i=0; i<5; i++)
						{
							if (reader.GetInt32(i) > 0)
							{
								ScoresCounts [i] += 1;
								Scores [i] += reader.GetInt32(i);
							}
						}
					}
					for (int i=0; i<5; i++)
					{
						if (ScoresCounts [i] > 0)
							Scores [i] = Mathf.RoundToInt(Scores [i] / ScoresCounts [i]);
						else
							Scores [i] = 0;
					}
				} else
				{
					Scores = null;
					Debug.LogWarning("DB: no entry found for userID \n" + userID);
				}
				
				return Scores;
			}
			#endif
			return null;
		}

		public List<float> calAvgECIMP(int userID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				//int userID = ApplicationState.Instance.userID;
				SQLiteDataReader reader = SelectSql("SELECT avg(ifnull(EmotionsScore,0)), avg(ifnull(ComprehensionScore,0)), avg(ifnull(ConversationStartScore,0)), avg(ifnull(MaintainConversationScore,0)), avg(ifnull(ProblemSolvingScore,0)) FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND Completed = 'True' AND L.UserID = "+userID+";");
				
				List<float> Scores = new List<float>();
				if (reader.HasRows)
				{
					reader.Read();
					for (int i=0; i<5; i++)
					{
						if (reader.GetFloat(i) > -1)
						{
							Scores.Add(reader.GetFloat(i));
						}
					}
				} else
				{
					Scores = null;
					Debug.LogWarning("DB: no entry found for userID \n" + userID);
				}
				
				return Scores;				
			}
			#endif
			return null;
		}

		public List<DBAvgECIMPwithNPCID> calAvgECIMPperNPC(int userID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				//int userID = ApplicationState.Instance.userID;
				SQLiteDataReader reader = SelectSql("SELECT NPCID, round(avg(ifnull(EmotionsScore,0))), round(avg(ifnull(ComprehensionScore,0))), round(avg(ifnull(ConversationStartScore,0))), round(avg(ifnull(MaintainConversationScore,0))), round(avg(ifnull(ProblemSolvingScore,0))) FROM Interaction I, LevelPlay L WHERE I.LevelPlayID = L.LevelPlayID AND Completed = 'True' AND L.UserID = "+userID+" Group by NPCID;");
				
				List<DBAvgECIMPwithNPCID> results = new List<DBAvgECIMPwithNPCID>();
				while(reader.Read())
				{

					DBAvgECIMPwithNPCID entry = new DBAvgECIMPwithNPCID();
					
					if (reader.HasRows)
					{
						entry.npcID = reader.GetInt32(0);
						entry.e = reader.GetFloat(1);
						entry.c = reader.GetFloat(2);
						entry.i = reader.GetFloat(3);
						entry.m = reader.GetFloat(4);
						entry.p = reader.GetFloat(5);

						results.Add(entry);
					}
				}
				if(results == null)
				{
					results = null;
					Debug.LogWarning("DB: no entry found for userID \n" + userID);
				}
				reader.Close();
				reader = null;
				return results;				
			}
			#endif
			return null;
		}

		/*
	 * Get total points earn for an interaction
	 * */
		public int getPoint(int interactionID, ref int noOfCategories)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				SQLiteDataReader reader = SelectSql("Select MIN(R.Points) as Points from Answer as A, Reply as R where A.ReplyID = R.ReplyID AND ReplyType <> 2 AND A.InteractionID = " + interactionID + " GROUP BY R.QuestionID;");
			
            int readInt = 0;
            noOfCategories = 0;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    readInt += reader.GetInt32(0);
                    noOfCategories++;
                }
            } else
            {
                readInt = -1;
                noOfCategories = -1;
                Debug.LogWarning("DB: no entry found for InteractionID \n" + interactionID);
            }
			
            reader.Close();
            reader = null;
				
            return readInt;
        }
#endif
        return -1;
    }

		/*
	 * Get total points earn for an NPC played
	 * */
		/*
		 * Update reply set points = '-5' where points = '-2'
		*/
		public float getPointsForNPC(int npcID, int userID)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				SQLiteDataReader sum = SelectSql("Select sum(R.Points) as Points from Answer as A, Reply as R, Interaction I, LevelPlay L where ReplyType <> 2 AND A.ReplyID = R.ReplyID AND A.InteractionID = I.InteractionID AND I.Completed = 'True' AND I.LevelPlayID = L.LevelPlayID AND NPCID = "+npcID+" AND UserID = "+userID+"");
				SQLiteDataReader min = SelectSql("Select sum(R.Points) as Points from Reply as R, Question Q where Q.QuestionID = R.QuestionID AND NPCID = "+npcID+";");
				SQLiteDataReader totalinteractionsCompleted = SelectSql("Select count(Completed) from Interaction I, LevelPlay L where I.LevelPlayID = L.LevelPlayID AND I.Completed = 'True' AND NPCID = "+npcID+" AND UserID = "+userID+"");
				
				float avgSum = 0;
				float points = 0;
				if (sum.HasRows && min.HasRows && totalinteractionsCompleted.HasRows)
				{
					sum.Read();
					min.Read();
					totalinteractionsCompleted.Read();

					float pointsSum = sum.GetInt32(0);
					float completedInteractions = totalinteractionsCompleted.GetInt32(0);
					avgSum = pointsSum / completedInteractions * 1.0f;

					float maxPoints = 10;

					//avgSum = avgSum - min.GetInt32(0);
					//float diff = maxPoints - min.GetInt32(0);
					float minPoints = min.GetInt32(0);
					maxPoints = -minPoints + maxPoints;
					avgSum = -minPoints + avgSum;

					points = avgSum / maxPoints * 1.0f;

				} else
				{
					points = -1;
					Debug.LogWarning("DB: no entry found for npcID \n" + npcID +" and userID "+userID );
				}
				
				sum.Close();
				sum = null;
				min.Close();
				min = null;
				totalinteractionsCompleted.Close();
				totalinteractionsCompleted = null;
				
				return points;
			}
			#endif
			return -1;
		}

    /*
	 * Get total points earn for an interaction
	 * */
    public string getCategory(int NPCID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            SQLiteDataReader reader = SelectSql("Select Category from Category as C, NPC as N where C.CategoryID = N.CategoryID AND NPCID = " + NPCID + ";");
            reader.Read();
            string readInt;
            if (reader.HasRows)
            {
                readInt = reader.GetString(0);
            } else
            {
                readInt = null;
                Debug.LogWarning("DB: no entry found for InteractionID \n" + NPCID);
            }
			
            reader.Close();
            reader = null;
				
            return readInt;
        }
#endif
        return null;
    }	
	
    /*
	 * get total time taken to answer question based on CommunicationID and interactionID
	 * 1 - Emotions
	 * 2 - Comprehension
	 * 3 - ConversationStart
	 * 4 - MaintainConversation
	 * 5 - ProblemSolving
	 * */
    public List<DBReplyTimeWithType> getTimeTaken(int CommID, int interactionID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            SQLiteDataReader reader = SelectSql("Select ReplyTimeTaken, ReplyType, R.Text, R.Points FROM Answer as A, Reply as R, Question as Q WHERE A.ReplyID = R.ReplyID AND R.QuestionID = Q.QuestionID AND CommID = " + CommID + " AND interactionid = " + interactionID + ";");
            List<DBReplyTimeWithType> results = new List<DBReplyTimeWithType>();
            while (reader.Read())
            {
                DBReplyTimeWithType entry = new DBReplyTimeWithType();
					
                if (reader.HasRows)
                {
                    entry.Timetaken = reader.GetFloat(0);
                    entry.ReplyType = reader.GetInt32(1);
                    entry.ReplyText = reader.GetString(2);
                    entry.ReplyPoints = reader.GetInt32(3);
                    results.Add(entry);
                }
            }

			
            reader.Close();
            reader = null;
				
            return results;
        }
#endif
        return null;
    }		
	
    /*
	 * get total time spend with each NPC in one level interactions
	 * */
    public List<DBNPCInteraction> getNPCandInteraction(int levelPlayID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
			SQLiteDataReader reader = SelectSql("Select I.NPCID, NPCName, NPCAtlas, NPCTexture, T.filename, ColorR, ColorG, ColorB, I.InteractionID, N.Stars, C.Category from Interaction I, UserToy U, Toy T, NPC N, Category C where C.CategoryID = N.CategoryID and N.NPCID = I.NPCID and U.InteractionID = I.InteractionID and T.ToyID = U.ToyID and LevelPlayID = " + levelPlayID + " group by I.NPCID;");
            List<DBNPCInteraction> results = new List<DBNPCInteraction>();
            while (reader.Read())
            {
                DBNPCInteraction entry = new DBNPCInteraction();
					
                if (reader.HasRows)
                {
                    entry.NPCID = reader.GetInt32(0);
                    entry.NPCName = reader.GetString(1);
                    entry.NPCAtlas = reader.GetString(2);
                    entry.NPCTexture = reader.GetString(3);
                    entry.ToyName = reader.GetString(4);
                    entry.ToyColorR = reader.GetFloat(5);
                    entry.ToyColorG = reader.GetFloat(6);
                    entry.ToyColorB = reader.GetFloat(7);
                    entry.InteractionID = reader.GetInt32(8);
                    entry.Stars = reader.GetInt32(9);
					entry.Category = reader.GetString(10);
                    results.Add(entry);
                }
            }

			
            reader.Close();
            reader = null;
				
            return results;
        }
#endif
        return null;
    }	
	
    /*
	 * get total time spend with each NPC in one level interactions
	 * */
    public int getPlayTotalTime(int levelPlayID)
    {
		
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            //SQLiteDataReader reader = SelectSql("Select Sum( strftime('%s',EndTime) - strftime('%s', StartTime)) from levelplayinstance where LevelPlayId " + levelPlayID + ";");
            SQLiteDataReader reader = SelectSql("Select ifnull(Sum( strftime('%s',EndTime) - strftime('%s', StartTime)),0) from LevelPlayInstance where LevelPlayId = " + levelPlayID + ";");
			
            DBNPCInteractionTime entry = new DBNPCInteractionTime();
			
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    entry.totalInteractionTimeSec = reader.GetInt32(0);
                }
            }

            reader.Close();
            reader = null;
				
            return entry.totalInteractionTimeSec;
        }
#endif
        return 0;
    }		
	
    /* 
	 * selects all answers and their details
	 * */
    public List<DBSelectAll> SelectAll(int interactionID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            SQLiteDataReader reader = SelectSql("Select LevelName, A.InteractionID, NPCName, CommText, Category, Q.Text, R.Text, ReplyType, Points, ReplyTimeTaken from Answer as A, Reply as R, Question as Q, Category as C, NPC as N, PointCommunication as P, Interaction I, LevelPlay as LP, Level as L where A.ReplyID = R.ReplyID AND R.QuestionID = Q.QuestionID AND Q.CategoryID = C.CategoryID AND Q.NPCID = N.NPCID AND Q.CommID = P.CommID AND A.InteractionID = I.InteractionID AND I.LevelPlayID = LP.LevelPlayID AND LP.LevelID = L.LevelID AND A.InteractionID = " + interactionID + ";");
            List<DBSelectAll> results = new List<DBSelectAll>();
				
            while (reader.Read())
            {
                DBSelectAll entry = new DBSelectAll();
                if (reader.HasRows)
                {
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
            }
				
            reader.Close();
            reader = null;
            return results;
        }
#endif
        return null;
    }
	
    /*
	 * gets list of interaction time foa an interaction
	 * 0 - starttime
	 * 1 - endtime
	 * */
    public int SelectInteractionTime(int interactionid)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            SQLiteDataReader reader = SelectSql("Select ifnull(strftime('%s',EndTime) - strftime('%s', StartTime), 0) from interactiontime where interactionid = " + interactionid + ";");
            int interactionTime = 0;
			
            if (reader.Read())
            {
                if (reader.HasRows)
                {
                    interactionTime = reader.GetInt32(0);
                }
            }
            reader.Close();
            reader = null;
			
            return interactionTime;
        }
#endif
        return 0;
    }
	
		/*
	 * get total time spend with each NPC in one level interactions
	 * */
		public int getTotalInteractionTime(int userID)
		{
			
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				SQLiteDataReader reader = SelectSql ("Select ifnull(Sum(strftime('%s',EndTime) - strftime('%s', StartTime)), 0) from interactiontime t, interaction i,levelplay l where t.interactionid =i.interactionid and i.levelplayid = l.levelplayid and endtime <> '' and l.userID = "+userID+";");

				DBNPCInteractionTime entry = new DBNPCInteractionTime();
				
				while (reader.Read())
				{
					if (reader.HasRows)
					{
						entry.totalInteractionTimeSec = reader.GetInt32(0);
					}
				}
				
				reader.Close();
				reader = null;
				
				return entry.totalInteractionTimeSec;
			}
			#endif
			return 0;
		}		


		/*
	 * get total time spend with each NPC in one level interactions
	 * */
		public int getTotalInteractionTimeForNPC(int userID, int NPCID)
		{
			
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				SQLiteDataReader reader = SelectSql ("Select ifnull(Avg(strftime('%s',EndTime) - strftime('%s', StartTime)), 0) from interactiontime t, interaction i,levelplay l where t.interactionid =i.interactionid and i.levelplayid = l.levelplayid and endtime <> '' and NPCID = "+NPCID+" AND l.userID = "+userID+";");
				reader.Read();
				DBNPCInteractionTime entry = new DBNPCInteractionTime();
				if (reader.HasRows)
				{
					entry.totalInteractionTimeSec = (int)(reader.GetFloat(0));
				}
				else
					Debug.LogError("Could not find totall time for NPC :" + NPCID +" or UserID:"+userID);
				reader.Close();
				reader = null;
				
				return entry.totalInteractionTimeSec;
			}
			#endif
			return 0;
		}	

    /*
	 * Returns List of percentage of interactionID right answers in following Point of communication
	 * List: 
	 * 0 - % of Emotions
	 * 1 - % of Comprehension
	 * 2 - % of ConversationStart
	 * 3 - % of MaintainConversation
	 * 4 - % of ProblemSolving
	 * */	
    public DBNPCMiniGameInfo getNPCInfo(int NPCID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {		
            SQLiteDataReader reader = SelectSql("Select NPCName, NPCNameParent, NPCTexture, NPC2Texture, NPCPortrait, MiniGame, MiniGameDescription, Stars, NPCNameChild from NPC where NPCID = " + NPCID + ";");
            reader.Read();
            DBNPCMiniGameInfo entry = new DBNPCMiniGameInfo();
            if (reader.HasRows)
            {
                entry.NPCName = reader.GetString(0);
                entry.NPCNameParent = reader.GetString(1);
                entry.NPCTexture = reader.GetString(2);
                entry.NPC2Texture = reader.GetString(3);
                entry.NPCPortrait = reader.GetString(4);
                entry.MiniGame = reader.GetString(5);
                entry.MiniGameDescription = reader.GetString(6);
                entry.Stars = reader.GetInt32(7);
                entry.NPCNameChild = reader.GetString(8);
            
                reader.Close();
                reader = null;
    								
                return entry;
            }

            reader.Close();
            reader = null;
        }
#endif
        return null;
    }
		
    public List<DBUserInfo> getUserInfo()
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
				SQLiteDataReader reader = SelectSql("SELECT U.UserID , U.Name , T.Filename , T.ColorR, T.ColorG, T.ColorB  FROM Toy T, User U Where U.PicId = T.ToyID AND  U.active = 1;");
            List<DBUserInfo> results = new List<DBUserInfo>();
				
            while (reader.Read())
            {
                DBUserInfo entry = new DBUserInfo();
                if (reader.HasRows)
                {
                    entry.UserID = reader.GetInt32(0);
                    entry.UserName = reader.GetString(1);
                    entry.ToyFilename = reader.GetString(2);
                    entry.ToyColorR = reader.GetInt32(3);
                    entry.ToyColorG = reader.GetInt32(4);
                    entry.ToyColorB = reader.GetInt32(5);
                    results.Add(entry);
                }
            }
				
            reader.Close();
            reader = null;
            /*
			List<DBUserInfo> results = new List<DBUserInfo>();
			DBUserInfo entry = new DBUserInfo();
         	entry.UserID = 1;
			entry.UserName = "Player 1";
			entry.ToyFilename = "CoalCar";
			entry.ToyColorR = 100;
			entry.ToyColorG = 200;
			entry.ToyColorB = 186;
			results.Add(entry);
				
				
			entry = new DBUserInfo();
         	entry.UserID = 2;
			entry.UserName = "Player 2";
			entry.ToyFilename = "Brick_x4_Thick";
			entry.ToyColorR = 255;
			entry.ToyColorG = 20;
			entry.ToyColorB = 86;
			results.Add(entry);
				
				
			entry = new DBUserInfo();
         	entry.UserID = 3;
			entry.UserName = "Player 3";
			entry.ToyFilename = "Engine";
			entry.ToyColorR = 180;
			entry.ToyColorG = 180;
			entry.ToyColorB = 200;
			results.Add(entry);
				
			entry = new DBUserInfo();
         	entry.UserID = 4;
			entry.UserName = "Player 4";
			entry.ToyFilename = "Crayon";
			entry.ToyColorR = 100;
			entry.ToyColorG = 100;
			entry.ToyColorB = 190;
			results.Add(entry);
			*/
            return results;
        }
#endif
        return null;
    }

    public DBUserInfo getSingleUserInfo(int userID)
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            SQLiteDataReader reader = SelectSql("SELECT U.UserID , U.Name , T.Filename , T.ColorR, T.ColorG, T.ColorB "
                + "FROM UserToy UT, Toy T, User U " +
                "Where UT.ToyID = T.ToyID AND UT.UserID = U.UserID AND UT.UserID = " + userID);
            DBUserInfo entry = new DBUserInfo();
            reader.Read();
			
            if (reader.HasRows)
            {
                entry.UserID = reader.GetInt32(0);
                entry.UserName = reader.GetString(1);
                entry.ToyFilename = reader.GetString(2);
                entry.ToyColorR = reader.GetInt32(3);
                entry.ToyColorG = reader.GetInt32(4);
                entry.ToyColorB = reader.GetInt32(5);
            }
            reader.Close();
            reader = null;
            return entry;
            /*
			DBUserInfo entry = new DBUserInfo();
			if (userID == 1)
			{
	         	entry.UserID = 1;
				entry.UserName = "Player 1";
				entry.ToyFilename = "CoalCar";
				entry.ToyColorR = 100;
				entry.ToyColorG = 200;
				entry.ToyColorB = 186;
			}
			else if (userID == 2)
			{
	         	entry.UserID = 2;
				entry.UserName = "Player 2";
				entry.ToyFilename = "Brick_x4_Thick";
				entry.ToyColorR = 255;
				entry.ToyColorG = 20;
				entry.ToyColorB = 86;
			}
			else if (userID == 3)
			{
	         	entry.UserID = 3;
				entry.UserName = "Player 3";
				entry.ToyFilename = "Engine";
				entry.ToyColorR = 180;
				entry.ToyColorG = 180;
				entry.ToyColorB = 200;
			}
			else
			{
	         	entry.UserID = 4;
				entry.UserName = "Player 4";
				entry.ToyFilename = "Crayon";
				entry.ToyColorR = 100;
				entry.ToyColorG = 100;
				entry.ToyColorB = 190;
			}
				
			return entry;
			*/
        }
#endif
        return null;
    }
	
    public List<DBToyInfo> getToyInfo()
    {
#if !UNITY_WEBPLAYER
        if (useDB)
        {
            SQLiteDataReader reader = SelectSql("SELECT ToyID, Name, Filename, ColorR, ColorG, ColorB FROM Toy");
            List<DBToyInfo> results = new List<DBToyInfo>();
				
            while (reader.Read())
            {
                DBToyInfo entry = new DBToyInfo();
                if (reader.HasRows)
                {
                    entry.ToyID = reader.GetInt32(0);
                    entry.ToyName = reader.GetString(1);
                    entry.Filename = reader.GetString(2);
                    entry.ToyColorR = reader.GetInt32(3);
                    entry.ToyColorG = reader.GetInt32(4);
                    entry.ToyColorB = reader.GetInt32(5);
                    results.Add(entry);
                }
            }
				
            reader.Close();
            reader = null;
            return results;
        }
#endif
        return null;
    }
	
	public DBToyInfo getToyInfoByID(int toyID)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{
			SQLiteDataReader reader = SelectSql("SELECT ToyID, Name, Filename, ColorR, ColorG, ColorB FROM Toy WHERE ToyID = "+ toyID+";");
			DBToyInfo entry = new DBToyInfo();
			reader.Read();
			if (reader.HasRows)
			{
				entry.ToyID = reader.GetInt32(0);
				entry.ToyName = reader.GetString(1);
				entry.Filename = reader.GetString(2);
				entry.ToyColorR = reader.GetInt32(3);
				entry.ToyColorG = reader.GetInt32(4);
				entry.ToyColorB = reader.GetInt32(5);
			}
			else
			{
				Debug.LogError("Toy not found with ToyID:" + toyID);
				return null;
			}
			reader.Close();
			reader = null;
			return entry;
		}
		#endif
		return null;
	}

	public DBToyInfo getToyInfoByInteractionID(int interactionID)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{
				SQLiteDataReader reader = SelectSql("Select toyid from ToyAssigned T, Interaction I where I.NPCID = T.NPCID and I.InteractionID = "+ interactionID+";");
				reader.Read();
				DBToyInfo entry = new DBToyInfo();
				if(reader.HasRows)
				{
					entry = getToyInfoByID(reader.GetInt32(0));
				}
				else
				{
					Debug.LogError("Toy not found with interactionID:" + interactionID);
					return null;
				}
			reader.Close();
			reader = null;
			return entry;
		}
		#endif
		return null;
	}

		public List<DBToyInfo> getToysInfoByNPCID(int userID, int npcid)
		{
			#if !UNITY_WEBPLAYER
			if (useDB)
			{
				SQLiteDataReader reader = SelectSql("Select toyid from UserToy U, Interaction I, LevelPlay L where U.InteractionID = I.InteractionID and I.LevelPLayID = L.LevelPlayID and L.UserID = "+userID+" and I.Completed = 'True' and I.NPCID = "+ npcid+";");
				List<DBToyInfo> results = new List<DBToyInfo>();
				
				while (reader.Read())
				{
					DBToyInfo entry = new DBToyInfo();
					if (reader.HasRows)
					{
						entry = getToyInfoByID(reader.GetInt32(0));
						results.Add(entry);
					}
				}
				
				reader.Close();
				reader = null;
				return results;
			}
			#endif
			return null;
		}


	public List<DBToyInfo> getUserToysByCategory(int userID, int toyCategoryID)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{
			SQLiteDataReader reader = SelectSql("SELECT T.ToyID, Name, Filename, ColorR, ColorG, ColorB, ifnull(UserID,-1) FROM Toy T LEFT JOIN UserToy U  ON T.ToyID = U.ToyID AND UserID = "+ userID+" Where ToyCategoryID = "+ toyCategoryID+"");
			List<DBToyInfo> results = new List<DBToyInfo>();
			
			while (reader.Read())
			{
				DBToyInfo entry = new DBToyInfo();
				if (reader.HasRows)
				{
					entry.ToyID = reader.GetInt32(0);
					entry.ToyName = reader.GetString(1);
					entry.Filename = reader.GetString(2);
					entry.ToyColorR = reader.GetInt32(3);
					entry.ToyColorG = reader.GetInt32(4);
					entry.ToyColorB = reader.GetInt32(5);
					entry.UserID = reader.GetInt32(6);
					results.Add(entry);
				}
			}
			
			reader.Close();
			reader = null;
			return results;
		}
		#endif
		return null;
	}

    public DBUserSettings getUserSettings(int userID)
    {
        #if !UNITY_WEBPLAYER
        if (useDB)
        {
            //SQLiteDataReader reader = SelectSql ("SELECT VolumeMusic, VolumeSFX, BackgroundTrack, DialogueProcessingTime, ExitConversationOption, SkipVOOption, ExitMinigameOption, SkipMinigameTutorial FROM User Where UserID = " + userID + ";");
            SQLiteDataReader reader = SelectSql("SELECT VolumeMusic, VolumeSFX, BackgroundTrack, DialogueProcessingTime, ExitConversationOption, InstantAnswer, ExitMinigameOption FROM User Where UserID = " + userID + ";");
            reader.Read();
			
            DBUserSettings userSetting = new DBUserSettings();
            if (reader.HasRows)
            {
                userSetting.VolumeMusic = reader.GetFloat(0);
                userSetting.VolumeSFX = reader.GetFloat(1);
                userSetting.BackgroundTrack = reader.GetInt32(2);
                userSetting.DialogueProcessingTime = reader.GetFloat(3);
                userSetting.ExitConversationOption = reader.GetBoolean(4);
                userSetting.InstantAnswer = reader.GetBoolean(5);
                userSetting.ExitMinigameOption = reader.GetBoolean(6);
            } else
            {
                userSetting = null;
                Debug.LogWarning("DB: no entry found for userID = " + userID);
            }
			
            reader.Close();
            reader = null;
			
            return userSetting;
        }
        #endif
        return null;
    }

	public List<DBCompletedNPCs> getCompletedNPCs(int userID)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{
				SQLiteDataReader reader = SelectSql("SELECT N.NPCID, Stars, NPCName, Category FROM Interaction I, LevelPlay L, NPC N, Category C WHERE Completed = 'True' AND L.LevelPlayID = I.LevelPlayID AND I.NPCID = N.NPCID AND C.CategoryID = N.CategoryID AND UserID = " + userID + " GROUP BY N.NPCID;");
			reader.Read();
			
			List<DBCompletedNPCs> npcs = new List<DBCompletedNPCs>();
			while(reader.Read())
			{
				if (reader.HasRows)
				{
					DBCompletedNPCs npc = new DBCompletedNPCs();
					npc.NPCID = reader.GetInt32 (0);
					npc.stars = reader.GetInt32(1);
					npc.NPCName = reader.GetString(2);
					npc.Category = reader.GetString(3);
					npcs.Add(npc);
				}
			}
			reader.Close();
			reader = null;
			
			return npcs;
		}
		#endif
		return null;
	}

    public void setUserSettings(int userID, float VolumeMusic, float VolumeSFX, int BackgroundTrack, 
	                                       float DialogueProcessingTime, int ExitConversationOption,
	                                       int InstantAnswer, int ExitMinigameOption)
    {
        #if !UNITY_WEBPLAYER
        MainDatabase.Instance.UpdateSql("UPDATE User "
            + " SET VolumeMusic = " + VolumeMusic
            + ", VolumeSFX = " + VolumeSFX
            + ",  BackgroundTrack = " + BackgroundTrack
            + ",  DialogueProcessingTime = " + DialogueProcessingTime
            + ",  ExitConversationOption = " + ExitConversationOption
            + ",  InstantAnswer = " + InstantAnswer
            + ",  ExitMinigameOption = " + ExitMinigameOption
            + " WHERE UserID = " + userID + ";");
        #endif
    }

	public void InsertAssignedToy(int UserID, int ToyID, string ToyName, int NPCID)
	{
			InsertSql("INSERT INTO ToyAssigned VALUES ("+UserID+","+ToyID+","+NPCID+",'"+ToyName+"');");
	}

	public void DeleteAllAssignedToy()
	{
			InsertSql("DELETE FROM ToyAssigned;");
	}

	public DBToyInfo GetRandomToy(int UserID)
	{
		#if !UNITY_WEBPLAYER
		if (useDB)
		{
			
			SQLiteDataReader reader = SelectSql("SELECT T.ToyID FROM TOY T where T.ToyID not in ( Select U.ToyID from UserToy U Where U.UserID = "+UserID+" UNION Select A.ToyID from ToyAssigned A Where A.UserID = "+UserID+") AND T.Name not in (Select A.ToyName from ToyAssigned A Where A.UserID = "+UserID+");");
			DBToyInfo NewToy = new DBToyInfo();
			if (reader.HasRows)
			{
				List<int> toys = new List<int>();
				while (reader.Read())
				{
					toys.Add(reader.GetInt32(0));
				}
					NewToy = getToyInfoByID(toys[UnityEngine.Random.Range(0,toys.Count-1)]);				
			} else
			{
				SQLiteDataReader reader2 = SelectSql("SELECT T.ToyID FROM TOY T where T.ToyID not in ( Select U.ToyID from UserToy U Where U.UserID = "+UserID+" UNION Select A.ToyID from ToyAssigned A Where A.UserID = "+UserID+");");
				NewToy = new DBToyInfo();
				if (reader2.HasRows)
				{
					List<int> toys = new List<int>();
					while (reader2.Read())
					{
						toys.Add(reader2.GetInt32(0));
					}
					NewToy = getToyInfoByID(toys[UnityEngine.Random.Range(0,toys.Count-1)]);				
				}
				else
				{
					Debug.LogWarning("DB: No Random Toy found");
				}
			}
			
			reader.Close();
			reader = null;
			
			return NewToy;
		}
		#endif
		return null;
	}

    public List<DBLevel> GetLevelsInfo()
    {
        #if !UNITY_WEBPLAYER
        if (useDB)
        {
            List<DBLevel> levels = new List<DBLevel>();
				SQLiteDataReader reader = SelectSql("SELECT LevelID, LevelName, Texture, DisplayIndex FROM Level ORDER BY LevelName ASC;");
			
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DBLevel dbLevel = new DBLevel();
                    dbLevel.levelId = reader.GetInt32(0);
                    dbLevel.levelName = reader.GetString(1);
                    dbLevel.levelTexture = reader.GetString(2);
                    dbLevel.levelIdx = reader.GetInt32(3);
                    levels.Add(dbLevel);
                }
            } else
            {
                Debug.LogWarning("DB: levels could not be retrieved");
            }
			
            reader.Close();
            reader = null;
			
            return levels;
        }
        #endif
        return null;
    }

    public LevelCharactersInfo GetLevelsInfoExtended(int levelId)
    {
        #if !UNITY_WEBPLAYER
        if (useDB)
        {
            LevelCharactersInfo characters = new LevelCharactersInfo();               
            SQLiteDataReader reader = SelectSql("SELECT NPC.NPCID, NPC.NPCName, ifnull(LC.LevelID,-1), ifnull(LC.PositionIndex,-1), NPC.NPCTexture FROM NPC LEFT JOIN LevelNPCs as LC ON LC.NPCID = NPC.NPCID AND LC.LevelID = " + levelId);
                
            if (reader.HasRows)
            {
                while (reader.Read())
                {
					if (reader.GetInt32(3) > -2)
					{
						LevelCharactersInfo.Character character = new LevelCharactersInfo.Character();                    
						character.npcId = reader.GetInt32(0);
						character.npcName = reader.GetString(1);
						character.positionId = reader.GetInt32(3);
						character.npcTexture = reader.GetString(4);

						if (reader.GetInt32(2) == -1)
							characters.otherCharacters.Add(character);
						else
							characters.levelSpecificCharacters.Add(character);
					}
                }
            } else
            {
                Debug.LogWarning("DB: levels could not be retrieved");
            }
                
            reader.Close();
            reader = null;
                
            return characters;
        }
        #endif
        return null;
    }
	
    public void DisconnectDB()
    {
#if !UNITY_WEBPLAYER
        if (dbcon != null)
        {
            dbcon.Close();
            dbcon = null;
            Debug.Log("Database: Database Dis-connected!");
        } else
        {
            Debug.Log("Database: Database Already Dis-connected!");
        }
#endif
    }
}

