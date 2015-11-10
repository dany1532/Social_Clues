using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MadLibsHUB : MonoBehaviour
{
	
    public List<string> MadLibsStatement;
    public UILabel label;

	public string highlightNames = "[588AF5]";
	public string highlightCategory = "[F5C658]";
	public string highlightCategory2 = "[E38502]";

    string ColorCodedScore(int maxCharacterScore)
    {
        if (maxCharacterScore < 50)
            return "[FF0000]" + maxCharacterScore + "%[-]";
        else if (maxCharacterScore < 70)
            return "[FF7700]" + maxCharacterScore + "%[-]";
        else
            return "[61A512]" + maxCharacterScore + "%[-]";
    }
	
	string GetProperName(string categoryName)
	{
		if (categoryName == "Emotions")	return "Emotional Awarness";
		if (categoryName == "ConversationStart")	return "Initiating A Conversation";
		if (categoryName == "MaintainConversation")	return "Maintaining a Conversation";
		return Regex.Replace(categoryName, "([a-z])([A-Z])", "$1 $2");;
	}

	string ColorCodedNames(string name)
	{
		return highlightNames + name + "[-]";
	}

	string ColorCodedCategory(string category)
	{
		return highlightCategory + category + "[-]";
	}

	string ColorCodedCategory2(string category)
	{
		return highlightCategory2 + category + "[-]";
	}
	
    // Use this for initialization
    void Start()
    {
        int userID = ApplicationState.Instance.userID;
        if (userID == -1)
            userID = 1;
        string userName = MainDatabase.Instance.getSingleUserInfo(userID).UserName;
		bool singleNPCAnalytics = false;
        MadLibsStatement = new List<string>();
        
        if (label != null)
        {
            label.text = "N/A";
        }
        //int levelPlayID = AnalyticsController.Instance.levelPlayID;
		
        //List<DBNPCInteraction> npc_interactions = new List<DBNPCInteraction>();
        //List<int> levelIDs = new List<int>();
        //levelIDs = MainDatabase.Instance.getLevelPlayIDs(userID, levelPlayID);

		List<float> ecimpAvg = MainDatabase.Instance.calAvgECIMP(userID);

		int maxPoint = 0;
		int maxEcimpID = 0;
		int minPoint = 100;
		int minEcimpID = 0;
		for(int i=0; i<5;i++)
		{
			if(ecimpAvg[i]>maxPoint)
			{
				maxPoint = (int)ecimpAvg[i];
				maxEcimpID = i; // ecimpID starts with 1 .. if i=0 then ID = 1
			}
			if(ecimpAvg[i]<minPoint)
			{
				minPoint = (int)ecimpAvg[i];
				minEcimpID = i;
			}
		}

		string maxnameECIMP = MainDatabase.Instance.getName("select commtext from Pointcommunication where commid = " + (maxEcimpID + 1) + ";");
		string minnameECIMP = MainDatabase.Instance.getName("select commtext from Pointcommunication where commid = " + (minEcimpID + 1) + ";");
		maxnameECIMP = GetProperName (maxnameECIMP);
		minnameECIMP = GetProperName (minnameECIMP);

		MadLibsStatement.Add("Based on all recorded data "+ColorCodedNames(userName)+"'s strongest communication skill is "+ ColorCodedCategory(maxnameECIMP.ToLower())+" with a score of " + ColorCodedScore(maxPoint) + " across all character interactions.");
		MadLibsStatement.Add(ColorCodedNames(userName)+"'s overall weakest communication skill is "+ ColorCodedCategory(minnameECIMP.ToLower())+" with an average weighted score of " + ColorCodedScore(minPoint) + ".");

		List<DBAvgECIMPwithNPCID> avgPerNPC = MainDatabase.Instance.calAvgECIMPperNPC(userID);

		int max = 0;
		int maxid = 0;
		int min = 100;
		int minid = 0;
		for(int i=0;i<avgPerNPC.Count;i++)
		{
			float sum = avgPerNPC[i].e + avgPerNPC[i].c + avgPerNPC[i].i + avgPerNPC[i].m + avgPerNPC[i].p;
			int avg = (int)sum/5;
			if(avg<min)
			{
				min = avg;
				minid = i;
			}
			if(avg>max)
			{
				max = avg;
				maxid = i;
			}
		}

		string minNPCname = MainDatabase.Instance.getName("select NPCName from NPC where NPCID = " + (minid + 1) + ";");
		string maxNPCname = MainDatabase.Instance.getName("select NPCName from NPC where NPCID = " + (maxid + 1) + ";");
		string minCategory = MainDatabase.Instance.getCategory((minid + 1));
		string maxCategory = MainDatabase.Instance.getCategory((maxid + 1));

		MadLibsStatement.Add(ColorCodedNames(userName)+" performed best when interacting with "+ ColorCodedCategory2(maxNPCname)+" and learning " + ColorCodedCategory2(maxCategory.ToLower())+" with a score of " + ColorCodedScore(max) + ".");
		MadLibsStatement.Add(ColorCodedNames(userName)+"'s weakest interactions have been with "+ ColorCodedCategory2(minNPCname)+" while learning " + ColorCodedCategory2(minCategory.ToLower())+".");

		MadLibsStatement.Add("It is recommended " + ColorCodedNames(userName) + " replay " + ColorCodedCategory2(minNPCname) + " and work on " + ColorCodedCategory2(minCategory.ToLower()) + " and " + ColorCodedCategory2(minnameECIMP.ToLower()) + ".");

        if (label != null)
        {
            label.text = string.Empty;
			
            for (int i = 0, statements = MadLibsStatement.Count - 1; i < statements; i++)
            {
                label.text += MadLibsStatement [i] + " ";
            }
				
            label.text += MadLibsStatement [MadLibsStatement.Count - 1];
        }
    }
}
