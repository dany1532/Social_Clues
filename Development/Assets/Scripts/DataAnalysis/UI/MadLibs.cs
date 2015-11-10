using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MadLibs : MonoBehaviour
{
	
    public List<string> MadLibsStatement;
    public UILabel label;

	public string highlightNames = "[588AF5]";
	public string highlightCategory = "[F5C658]";

    string ColorCodedScore(int maxCharacterScore)
    {
        if (maxCharacterScore < 50)
            return "[FF0000]" + maxCharacterScore + "%[-]";
        else if (maxCharacterScore < 70)
            return "[FF7700]" + maxCharacterScore + "%[-]";
        else
            return "[61A512]" + maxCharacterScore + "%[-]";
    }

	string ColorCodedNames(string name)
	{
		return highlightNames + name + "[-]";
	}

	string ColorCodedCategory(string category)
	{
		return highlightCategory + category + "[-]";
	}

	string GetProperName(string categoryName)
	{
		if (categoryName == "Emotions")	return "Emotional Awarness";
		if (categoryName == "ConversationStart")	return "Initiaing A Conversation";
		if (categoryName == "MaintainConversation")	return "Maintaining a Conversation";
		return Regex.Replace(categoryName, "([a-z])([A-Z])", "$1 $2");;
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
        int levelPlayID = AnalyticsController.Instance.levelPlayID;
		
        List<DBNPCInteraction> npc_interactions = new List<DBNPCInteraction>();
        List<int> levelIDs = new List<int>();
        levelIDs = MainDatabase.Instance.getLevelPlayIDs(userID, levelPlayID);

        List<DBNPCInteraction> current_npc_interactions = MainDatabase.Instance.getNPCandInteraction(levelPlayID);
        int maxGlobalCharacterScore = 0;
        string maxnameGlobalCharater = "";
        int minGlobalCharacterScore = 100;
        string minnameGlobalCharater = "";
        for (int l=0; l< levelIDs.Count; l++)
        {
            npc_interactions = MainDatabase.Instance.getNPCandInteraction(levelIDs [l]);
			if(npc_interactions.Count == 1)
				singleNPCAnalytics = true;
            for (int i = 0; i < npc_interactions.Count; ++i)
            {
                List<float> percentCharacter = new List<float>();
                percentCharacter = MainDatabase.Instance.calPercentageForInteractionID(npc_interactions [i].InteractionID, userID);
                int totalCharacterPercent = 0;
                for (int j=0; j<percentCharacter.Count; j++)
                {
                    totalCharacterPercent += (int)percentCharacter [j];
                }
				
                if (percentCharacter.Count > 0)
                {
                    int averageCharacter = totalCharacterPercent / percentCharacter.Count;
					
                    if (maxGlobalCharacterScore < averageCharacter)
                    {
                        maxGlobalCharacterScore = averageCharacter;
                        maxnameGlobalCharater = npc_interactions [i].NPCName;
                    }
					
                    if (minGlobalCharacterScore >= averageCharacter)
                    {
                        minGlobalCharacterScore = averageCharacter;
                        minnameGlobalCharater = npc_interactions [i].NPCName;
                    }
                }
            }
        }
		
        int maxCharacterScore = 0;
        string maxnameCharater = "";
		string maxCharaterCategory = "";
        int minCharacterScore = 100;
        string minnameCharater = "";
		string minCharaterCategory = "";
        for (int i = 0; i < current_npc_interactions.Count; ++i)
        {
            List<float> percentCharacter = new List<float>();
            percentCharacter = MainDatabase.Instance.calPercentageForInteractionID(current_npc_interactions [i].InteractionID, userID);
            int totalCharacterPercent = 0;
            int percentagesCount = 0;
            for (int j=0; j<percentCharacter.Count; j++)
            {
                totalCharacterPercent += (int)percentCharacter [j];
                if ((int)percentCharacter [j] > 0)
                    percentagesCount++;
            }
            if (percentagesCount > 0)
            {
				

                int averageCharacter = totalCharacterPercent / percentagesCount;
				
                if (maxCharacterScore < averageCharacter)
                {
                    maxCharacterScore = averageCharacter;
                    maxnameCharater = current_npc_interactions [i].NPCName;
					maxCharaterCategory = current_npc_interactions [i].Category;
                }
				
                if (minCharacterScore >= averageCharacter)
                {
                    minCharacterScore = averageCharacter;
                    minnameCharater = current_npc_interactions [i].NPCName;
					minCharaterCategory = current_npc_interactions[i].Category;
                }
            }
        }

		if(current_npc_interactions.Count > 1)
			MadLibsStatement.Add("For this level " + ColorCodedNames(userName) + "'s best interaction was talking with " + ColorCodedCategory(maxnameCharater) + " and "+ ColorCodedCategory(maxCharaterCategory.ToLower()) + ", scoring a " + ColorCodedScore(maxCharacterScore) + ".");

		
        List<float> percentECIMP = new List<float>();
        percentECIMP = MainDatabase.Instance.calTotalPercentageECIMPForPlay(userID, levelPlayID);
        int maxECIMPScore = 0;
        int maxECIMPScoreIndex = 0;
        string maxnameECIMP = "";
        int minECIMPScore = 100;
        int minECIMPScoreIndex = 0;
        string minnameECIMP = "";
        for (int j=0; j<percentECIMP.Count; j++)
        {
            if (maxECIMPScore < percentECIMP [j])
            {
                maxECIMPScore = (int)percentECIMP [j];
                maxECIMPScoreIndex = j;
            }
            if (minECIMPScore >= percentECIMP [j])
            {
                minECIMPScore = (int)percentECIMP [j];
                minECIMPScoreIndex = j;
            }			
        }
        maxnameECIMP = MainDatabase.Instance.getName("select commtext from Pointcommunication where commid = " + (maxECIMPScoreIndex + 1) + ";");
        minnameECIMP = MainDatabase.Instance.getName("select commtext from Pointcommunication where commid = " + (minECIMPScoreIndex + 1) + ";");

		maxnameECIMP = GetProperName (maxnameECIMP);
        minnameECIMP = GetProperName(minnameECIMP);

		MadLibsStatement.Add(ColorCodedNames(userName)+"'s strongest communication skill in this level was "+ ColorCodedCategory(maxnameECIMP.ToLower())+", with a score of " + ColorCodedScore(maxECIMPScore) + ".");

		if(current_npc_interactions.Count > 1)
			MadLibsStatement.Add(ColorCodedNames(userName)+"'s struggled most while interacting with " + ColorCodedCategory(minnameCharater) + ", with a score of " + ColorCodedScore(minCharacterScore) + ".");
        
		MadLibsStatement.Add(ColorCodedNames(userName)+"'s weakest conversation skill in this level was " + ColorCodedCategory(minnameECIMP.ToLower()) + " with a score of " + ColorCodedScore(minECIMPScore) + ".");
		
        percentECIMP = MainDatabase.Instance.calTotalPercentageECIMP(userID, levelPlayID);
        maxECIMPScore = 0;
        maxECIMPScoreIndex = 0;
        maxnameECIMP = "";
        minECIMPScore = 100;
        minECIMPScoreIndex = 0;
        minnameECIMP = "";
        for (int j=0; j<percentECIMP.Count; j++)
        {
            if (maxECIMPScore < percentECIMP [j])
            {
                maxECIMPScore = (int)percentECIMP [j];
                maxECIMPScoreIndex = j;
            }
            if (minECIMPScore >= percentECIMP [j])
            {
                minECIMPScore = (int)percentECIMP [j];
                minECIMPScoreIndex = j;
            }			
        }

        if (percentECIMP.Count > 0)
        {
            maxnameECIMP = MainDatabase.Instance.getName("select commtext from Pointcommunication where commid = " + (maxECIMPScoreIndex + 1) + ";");
            minnameECIMP = MainDatabase.Instance.getName("select commtext from Pointcommunication where commid = " + (minECIMPScoreIndex + 1) + ";");
    		
            maxnameECIMP = GetProperName(maxnameECIMP);
			minnameECIMP = GetProperName(minnameECIMP);
    		
			MadLibsStatement.Add("For post play discussion, you should talk with " + ColorCodedNames(userName) + " about " + ColorCodedCategory(minCharaterCategory.ToLower()) + " and " + ColorCodedCategory(minnameECIMP.ToLower()) + ".");
        }

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
