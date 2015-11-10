using System;
using System.IO;
using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;

public class QuestionTool : EditorWindow {
	
	private string prefabsPath; // Absolute path for prefabs folder
	private Hashtable allNPCs = new Hashtable(); // List of NPC prefabs
	
	// Edit how the dropDownMenu looks
	private GUIStyle dropDownStyle = new GUIStyle();
	
	// NPC scroll view, on the far left
	Vector2 scrollViewNPCMenu = Vector2.zero;
	
	// Edit dialogue scroll view, on the right
	Vector2 scrollViewConversation = Vector2.zero;
	
	// Edit linkages, on the bottom right
	Vector2 scrollViewLinkages = Vector2.zero;
	
	// Foldouts!!!!
	Hashtable foldOutBooleans = new Hashtable();
	
	// Necessary components to be declared for editing dialogues on the right
	Vector2 scrollViewEditComponent = Vector2.zero;
	private DropDownMenu dropDownDialogueDisplayTypeMenu; // Dropdown for DisplayType
	GUIContent[] dropDownDialogueDisplayTypeList;
	private DropDownMenu dropDownDialogueBubbleTypeMenu; // Dropdown for BubbleType
	GUIContent[] dropDownDialogueBubbleTypeList;
	private DropDownMenu dropDownDialogueSpeakerMenu; // Dropdown for Speaker
	GUIContent[] dropDownDialogueSpeakerList;
	private DropDownMenu dropDownDialogueTypeMenu; // Dropdown for Speaker
	GUIContent[] dropDownDialogueTypeList;
	
	// Necessary components to be declared for editing replies on the right
	private DropDownMenu dropDownReplyTypeMenu; // Dropdown for Reply type
	GUIContent[] dropDownReplyTypeList;
	
	// Components or prefabs to edit
	GameObject NPCtoEdit;
	Component componentToEdit;
	enum varToEdit {NONE, WRONGREPLIES, RIGHTREPLY, NEXTDIALOGUE};
	varToEdit linkToEdit = varToEdit.NONE;
	ArrayList dialogueReplyList = new ArrayList();
	ArrayList dialogueList = new ArrayList();
	ArrayList replyList = new ArrayList();
	
	// Template
	GameObject templateNPC;
	GameObject templateConversation;

	#region Initialize Tool Window
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Conversation Tool")]
	public static void Init()
	{
		//Show existing window instance. If one doesn't exist, make one.
		if(EditorWindow.FindObjectOfType(typeof(QuestionTool)) == null) {
			QuestionTool tool = EditorWindow.GetWindow(typeof(QuestionTool)) as QuestionTool; // Get the tool if it exists
			tool.DestroyPrefabs(); // Destroy it so it destroys all the prefabs it creates
			tool = EditorWindow.GetWindow(typeof(QuestionTool)) as QuestionTool; // Make a new tool
			tool.allNPCs.Clear(); // Get rid of anything left in it
			tool.Initialize(); // Init
			tool.Show(); // Show
		}
	}
	public static void EmptyTool()
	{
		//Show existing window instance. If one doesn't exist, make one.
		if(EditorWindow.FindObjectOfType(typeof(QuestionTool)) == null) {
			QuestionTool tool = EditorWindow.GetWindow(typeof(QuestionTool)) as QuestionTool; // Get the tool if it exists
			tool.DestroyPrefabs(); // Destroy it so it destroys all the prefabs it creates
			tool = EditorWindow.GetWindow(typeof(QuestionTool)) as QuestionTool; // Make a new tool
			tool.allNPCs.Clear(); // Get rid of anything left in it
			tool.Show(); // Show
		}
	}
	void Initialize() {
		// Get the prefabs in the folder Prefabs/NPCs folder
		prefabsPath = Application.dataPath + "/Resources/NPCs";
		#if !UNITY_OSX && !UNITY_IPHONE
		prefabsPath = prefabsPath.Replace('/', '\\');
		#endif
		if(Directory.Exists(prefabsPath)) { // Make sure directory exists
			getPrefabs(prefabsPath);
		} else {
			Debug.Log ("Cannot open path " + prefabsPath);
		}
		
		#region dropdown instantiation
		// Set style of dropDown
		dropDownStyle.normal.textColor = Color.white; 
		dropDownStyle.onHover.background = dropDownStyle.hover.background = new Texture2D(2, 2);
		dropDownStyle.padding.left =
		dropDownStyle.padding.right =
		dropDownStyle.padding.top =
		dropDownStyle.padding.bottom = 4;
		// Create DialogueDisplayType dropdown
		dropDownDialogueDisplayTypeList = new GUIContent[Enum.GetNames(typeof(Dialogue.DialogueDisplayType)).Length]; // Create list
		for(int i = 0; i < Enum.GetNames(typeof(Dialogue.DialogueDisplayType)).Length; i++) // Populate the list
			dropDownDialogueDisplayTypeList[i] = new GUIContent(Enum.GetName(typeof(Dialogue.DialogueDisplayType), i));
		dropDownDialogueDisplayTypeMenu = new DropDownMenu(new Rect(0, 0, Screen.width / 2 - 25, 25), // Put the list in the menu
					dropDownDialogueDisplayTypeList[0], dropDownDialogueDisplayTypeList, "button", "box", dropDownStyle);
		// Create DialogueBubbleType dropdown
		dropDownDialogueBubbleTypeList = new GUIContent[Enum.GetNames(typeof(Dialogue.DialogueLocation)).Length]; // Create list
		for(int i = 0; i < Enum.GetNames(typeof(Dialogue.DialogueLocation)).Length; i++) // Populate the list
			dropDownDialogueBubbleTypeList[i] = new GUIContent(Enum.GetName(typeof(Dialogue.DialogueLocation), i));
		dropDownDialogueBubbleTypeMenu = new DropDownMenu(new Rect(0, 30, Screen.width / 2 - 25, 25), // Put the list in the menu
					dropDownDialogueBubbleTypeList[0], dropDownDialogueBubbleTypeList, "button", "box", dropDownStyle);
		// Create DialogueSpeaker dropdown
		dropDownDialogueSpeakerList = new GUIContent[Enum.GetNames(typeof(Dialogue.Speaker)).Length]; // Create list
		for(int i = 0; i < Enum.GetNames(typeof(Dialogue.Speaker)).Length; i++) // Populate the list
			dropDownDialogueSpeakerList[i] = new GUIContent(Enum.GetName(typeof(Dialogue.Speaker), i));
		dropDownDialogueSpeakerMenu = new DropDownMenu(new Rect(0, 360, Screen.width / 2 - 25, 25), // Put the list in the menu
					dropDownDialogueSpeakerList[0], dropDownDialogueSpeakerList, "button", "box", dropDownStyle);
		// Create DialogueSpeaker dropdown
		dropDownDialogueTypeList = new GUIContent[Enum.GetNames(typeof(Dialogue.DialogueType)).Length]; // Create list
		for(int i = 0; i < Enum.GetNames(typeof(Dialogue.DialogueType)).Length; i++) // Populate the list
			dropDownDialogueTypeList[i] = new GUIContent(Enum.GetName(typeof(Dialogue.DialogueType), i));
		dropDownDialogueTypeMenu = new DropDownMenu(new Rect(0, 450, Screen.width / 2 - 25, 25), // Put the list in the menu
					dropDownDialogueTypeList[0], dropDownDialogueTypeList, "button", "box", dropDownStyle);
		// Create ReplyType dropdown
		dropDownReplyTypeList = new GUIContent[Enum.GetNames(typeof(Reply.AnswerType)).Length]; // Create list
		for(int i = 0; i < Enum.GetNames(typeof(Reply.AnswerType)).Length; i++) // Populate the list
			dropDownReplyTypeList[i] = new GUIContent(Enum.GetName(typeof(Reply.AnswerType), i));
		dropDownReplyTypeMenu = new DropDownMenu(new Rect(0, 450, Screen.width / 2 - 25, 25), // Put the list in the menu
					dropDownReplyTypeList[0], dropDownReplyTypeList, "button", "box", dropDownStyle);
		#endregion
	}
	
	// Delete all instantiated gameobjects and closes the window
	void OnDestroy() {
		DestroyPrefabs();
	}

	void DestroyPrefabs()
	{
		foreach(DictionaryEntry go in allNPCs) {
			DestroyImmediate((GameObject)(go.Value));
			DestroyImmediate((GameObject)(go.Key));
		}
	}
	#endregion

	// Gets all files in all children directories
	void getPrefabs(string directory) {
		// Get children directories
		string[] tempDirectories = Directory.GetDirectories (directory); // Get list of directories
		foreach(string s in tempDirectories)
			getPrefabs(s); // Recursive call on child directory
		// Get files in directory
		string[] tempList = Directory.GetFiles (directory);
		// Go through all files
		for(int i = 0; i < tempList.Length; i++) {
			string tempString = tempList[i];
			// Ignore this if it's a meta file
			if(tempString.Contains("meta"))
				continue;
			// Load this asset
			GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/" + tempString.Substring(Application.dataPath.Length+1), typeof(GameObject));
			// If this is a template, assign it to the template variable
			if(tempString.Contains("Template")) {
				templateNPC = go;
				continue;
			}
			if (go == null) continue;

			// If this is an NPC
			if(go.GetComponent<NPC>() != null) {
				for(int j = 0; j < tempList.Length; j++) { // Go through all the files in this folder
					if(tempList[j].Contains("Conversation") && !tempList[j].Contains("meta")) { // If this file is a conversation
						// Load the asset and put it into the hashtable
						GameObject conversation = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/" + tempList[j].Substring(Application.dataPath.Length+1), typeof(GameObject));
						allNPCs.Add(PrefabUtility.InstantiatePrefab(go) as GameObject, PrefabUtility.InstantiatePrefab(conversation) as GameObject);
					}
				}
			}
		}
	}
	
	// Saves all edits made to prefabs in this program to the prefabs in the folder
	void saveEditsToPrefab() {
		foreach(DictionaryEntry entry in allNPCs) {
			// Replace prefab in folder with prefab in editor
			GameObject go = (GameObject)(entry.Key);
			PrefabUtility.ReplacePrefab(go, PrefabUtility.GetPrefabParent(go), ReplacePrefabOptions.ConnectToPrefab);
			PrefabUtility.ReplacePrefab((GameObject)allNPCs[(GameObject)go], PrefabUtility.GetPrefabParent((GameObject)allNPCs[(GameObject)go]), ReplacePrefabOptions.ConnectToPrefab);
		}
	}
	
	// Everything to be rendered to the screen is called here
	void OnGUI() {
		// Start the scroll view for the list of NPC's
		scrollViewNPCMenu = GUI.BeginScrollView (new Rect (0, 0, Screen.width / 6, Screen.height / 2), scrollViewNPCMenu, new Rect (0, 0, Screen.width / 6 - 25, 30 * allNPCs.Count));
		int itemsY = 0;
		foreach(DictionaryEntry entry in allNPCs) { // Go through all prefabs
			GameObject go = (GameObject)(entry.Key);
			if (go != null)
			{
				if(GUI.Button(new Rect(0, itemsY++ * 30, Screen.width / 6 - 25, 25),
						go.GetComponent("NPC").ToString())) { // Button to choose NPC
					NPCtoEdit = go; // Make this the NPC to edit, rendered by displayNPC()
					componentToEdit = null; // Clear out what the component to edit is
					linkToEdit = varToEdit.NONE; // Clear out the link to edit
					foldOutBooleans.Clear(); // Get rid of the foldouts data
					ConversationTree cTree = ((GameObject)allNPCs[NPCtoEdit]).GetComponent<ConversationTree>(); // Get conversation tree
					if(cTree.root == null)
						continue;
					foldOutBooleans.Add(cTree.root, false); // Add the first conversation to foldouts
				}
			}
		}
		GUI.EndScrollView(); // End the scroll view for the list of NPC's
		// Displays the conversation of the NPC that is selected
		displayConversation();
		// Displays the component to be edited
		displayComponentToEdit();
//		if(linkToEdit != varToEdit.NONE)
//			displayEditLink();
//		// New NPC Button
//		if(GUI.Button(new Rect(0, Screen.height / 2, Screen.width / 6, Screen.height / 4), "New NPC")) {
//			allNPCs.Add(PrefabUtility.CreatePrefab(prefabsPath + "/Eddie", templateNPC));
//			
//		}
		// Save button
		if(GUI.Button(new Rect(0.0f, Screen.height * 7 / 8, Screen.width / 6, Screen.height / 8), "Save"))
			saveEditsToPrefab();
		// Refresh button
		if(GUI.Button(new Rect(Screen.width / 3.0f, Screen.height * 7 / 8, Screen.width / 6, Screen.height / 8), "Refresh"))
			Init();
		// Empty button
		//if(GUI.Button(new Rect(2.0f / 3.0f * Screen.width, Screen.height * 7 / 8, Screen.width / 6, Screen.height / 8), "Clear"))
		//	EmptyTool();
		// Clear button
		if(GUI.Button(new Rect(2.0f / 3.0f * Screen.width, Screen.height * 7 / 8, Screen.width / 6, Screen.height / 8), "Clear"))
			ClearScene();
	}

	public static void ClearScene()
	{ 
		object[] obj = GameObject.FindSceneObjectsOfType(typeof (GameObject));
		foreach (object o in obj)
		{
			GameObject g = (GameObject) o;
			DestroyImmediate (g);
		}
		/*
		MonoBehaviour[] gameObjects = GameObject.FindObjectsOfType(typeof(MonoBehaviour))as MonoBehaviour[];
		foreach(MonoBehaviour go in gameObjects)
        {
			DestroyImmediate (go);
		}
		*/
	}
	ArrayList getDialogueReply(Dialogue currentDialogue) {
		ArrayList dialogueReplyList = new ArrayList();
		while(currentDialogue.nextDialogue != null || currentDialogue.rightReply != null) { // Go through the conversation tree and get all dialogues
			if(dialogueReplyList.Contains(currentDialogue)) {
				throw new InsufficientMemoryException();
			}
			dialogueReplyList.Add(currentDialogue);
			// Display all wrong replies
			if(currentDialogue.wrongReplies != null) {
				for(int i = 0; i < currentDialogue.wrongReplies.Count; i++) {
					dialogueReplyList.Add(currentDialogue.wrongReplies[i]);
					if(currentDialogue.wrongReplies[i].dialogueReply != null)
						dialogueReplyList.Add(currentDialogue.wrongReplies[i].dialogueReply);
				}
			}
			// Displays the right reply
			if(currentDialogue.rightReply != null) {
				dialogueReplyList.Add(currentDialogue.rightReply);
				if(currentDialogue.rightReply.dialogueReply != null) {
					dialogueReplyList.Add(currentDialogue.rightReply.dialogueReply);
				}
				// Sets the next dialogue, either the next dialogue or the scevent's parameter
				if(currentDialogue.rightReply.dialogueReply.nextDialogue != null) {
					currentDialogue = currentDialogue.rightReply.dialogueReply.nextDialogue;
				} else if(currentDialogue.rightReply.dialogueReply.gameObject.GetComponent<SCEvent>() != null) {
					currentDialogue = currentDialogue.rightReply.dialogueReply.gameObject.GetComponent<SCEvent>().parameter;
				}
			} else if(currentDialogue.nextDialogue != null) { // Advance to next dialogue
				currentDialogue = currentDialogue.nextDialogue;
			} else {
				break;
			}
			// Check if currentDialogue is null
			if(currentDialogue == null) 
				break;
		}
		return dialogueReplyList;
	}
	
	ArrayList getDialogues(Dialogue currentDialogue) {
		ArrayList dialogueList = getDialogueReply(currentDialogue);
		for(int i = 0; i < dialogueList.Count; i++) {
			if(dialogueList[i].GetType() != typeof(Dialogue)) {
				dialogueList.Remove(dialogueList[i]);
				i--;
			}
		}
		return dialogueList;
	}
	
	ArrayList getReplies(Dialogue currentDialogue) {
		ArrayList replyList = getDialogueReply(currentDialogue);
		for(int i = 0; i < replyList.Count; i++) {
			if(replyList[i].GetType() != typeof(Reply)) {
				replyList.Remove(replyList[i]);
				i--;
			}
		}
		return replyList;
	}
	
	void displayConversationFoldout(Component componentToDisplay, ref int itemsY, int itemsX) {
		if(!foldOutBooleans.Contains(componentToDisplay))
			foldOutBooleans.Add (componentToDisplay, false);
		// Button to make this selectable to edit
		if(GUI.Button(new Rect(itemsX + 10, itemsY, Screen.width / 2, 20), "")) {
			componentToEdit = componentToDisplay;
			initializeComponentToEdit(componentToEdit);
		}
		// Create the foldout itself
		foldOutBooleans[componentToDisplay] =  EditorGUI.Foldout(new Rect(itemsX, itemsY, Screen.width / 2, 25), (bool)foldOutBooleans[componentToDisplay], componentToDisplay.name, false);
		itemsY += 20;
		// If I need to render its children
		if((bool)foldOutBooleans[componentToDisplay]) {
			// If it's a dialogue
			if(componentToDisplay.GetType() == typeof(Dialogue)) {
				Dialogue currentDialogue = (Dialogue)componentToDisplay;
				if(currentDialogue.wrongReplies != null) { // If this dialogue is a question
					for(int i = 0; i < currentDialogue.wrongReplies.Count; i++) { // Display all the replies
						displayConversationFoldout(currentDialogue.wrongReplies[i], ref itemsY, itemsX + 5);
					}
				}
				Dialogue tempDialogue = currentDialogue.nextDialogue;
				if(tempDialogue == null && currentDialogue.rightReply != null) {
					tempDialogue = currentDialogue.rightReply.dialogueReply;
				}
				if(tempDialogue == null && currentDialogue.eventCauser != null)
					tempDialogue = currentDialogue.eventCauser.parameter;
				if(tempDialogue == null)
					return;
				displayConversationFoldout(tempDialogue, ref itemsY, itemsX);
				return;
			}
			if(componentToDisplay.GetType() == typeof(Reply)) {
				Reply currentReply = (Reply)componentToDisplay;
				if(currentReply.dialogueReply != null) {
						displayConversationFoldout(currentReply.dialogueReply, ref itemsY, itemsX);
				}
				return;
			}
		}
	}
	
	// Displays the current NPC's conversation
	void displayConversation() {
		// Check if NPC is active
		if(NPCtoEdit == null)
			return;
		// Get conversation
		ConversationTree cTree = ((GameObject)(allNPCs[NPCtoEdit])).GetComponent<ConversationTree>(); // Get conversation tree
		if(cTree.root == null) // If the conversation doesn't exist, exit
			return;
		// Create a scroll view for the conversation
		scrollViewConversation = GUI.BeginScrollView (new Rect (Screen.width / 6, 0, Screen.width / 3, Screen.height * 7 / 8), scrollViewConversation, new Rect (0, 0, Screen.width / 3 - 25, 1000));
		Dialogue currentDialogue = cTree.root; // Start the foldouts
		int itemsY = 0;
		displayConversationFoldout(currentDialogue, ref itemsY, 0);
		GUI.EndScrollView();
/*
		if(GUI.Button(new Rect(Screen.width / 6, Screen.height * 7 / 8, Screen.width / 9, Screen.height / 8), "Add Dialogue")) {
			if(componentToEdit != null) {
				if(componentToEdit.GetType() == typeof(Dialogue)) {
					Dialogue currentDialogue = (Dialogue)componentToEdit;
					Component tempDialogue = currentDialogue.gameObject.AddComponent(typeof(Dialogue));
					Dialogue newDialogue = (Dialogue)tempDialogue;
					newDialogue.nextDialogue = currentDialogue.nextDialogue;
					currentDialogue.nextDialogue = newDialogue;
				}
				if(componentToEdit.GetType() == typeof(Reply)) {
					Reply currentReply = (Reply)componentToEdit;
					Component tempDialogue = ((Reply)currentReply).dialogueReply.gameObject.AddComponent(typeof(Dialogue));
					Dialogue newDialogue = (Dialogue)tempDialogue;
					newDialogue.nextDialogue = currentReply.dialogueReply;
					currentReply.dialogueReply = newDialogue;
				}
			}
		}
		if(GUI.Button(new Rect(Screen.width * 5 / 18, Screen.height * 7 / 8, Screen.width / 9, Screen.height / 8), "Add Reply")) {
			if(componentToEdit != null) {
				if(componentToEdit.GetType() == typeof(Dialogue)) {
					Dialogue currentDialogue = (Dialogue)componentToEdit;
					Component tempReply = currentDialogue.gameObject.AddComponent(typeof(Reply));
					Reply newReply = (Reply)tempReply;
					currentDialogue.wrongReplies.Add(newReply);
					newReply.dialogueReply = currentDialogue;
					newReply.dialogue = currentDialogue;
				}
			}
		}
		if(GUI.Button(new Rect(Screen.width * 7 / 18, Screen.height * 7 / 8, Screen.width / 9, Screen.height / 8), "Delete Component")) {
			if(componentToEdit != null) {
				if(componentToEdit.GetType() == typeof(Reply)) {
					Reply currentReply = (Reply)componentToEdit;
					if(currentReply.dialogue.wrongReplies.Contains(currentReply)) {
						currentReply.dialogue.wrongReplies.Remove(currentReply);
						DestroyImmediate(currentReply.dialogueReply.gameObject, true);
						DestroyImmediate(currentReply, true);
					}
				}
				//DestroyImmediate(componentToEdit, true);
			}
		}
		*/
	}
	
	void initializeComponentToEdit(Component componentToEdit) {
		linkToEdit = varToEdit.NONE;
		if(componentToEdit.GetType() == typeof(Dialogue)) {
			Dialogue dialogueToEdit = (Dialogue)componentToEdit;
			dropDownDialogueDisplayTypeMenu.setFirstSelected((int)(dialogueToEdit.displayType));
			dropDownDialogueBubbleTypeMenu.setFirstSelected((int)(dialogueToEdit.dialogueBubbleType));
			dropDownDialogueSpeakerMenu.setFirstSelected((int)(dialogueToEdit.speaker));
			dropDownDialogueTypeMenu.setFirstSelected((int)(dialogueToEdit.type));
		}
		if(componentToEdit.GetType() == typeof(Reply)) {
			Reply replyToEdit = (Reply)componentToEdit;
			dropDownReplyTypeMenu.setFirstSelected(Convert.ToInt32(Enum.Format(typeof(Reply.AnswerType), replyToEdit.type, "d")));
		}
	}
	
	void displayComponentToEdit() {
		if(componentToEdit == null)
			return;
		scrollViewEditComponent = GUI.BeginScrollView (new Rect (Screen.width / 2, 0, Screen.width / 2, Screen.height * 2 / 3), scrollViewEditComponent, new Rect (0, 0, Screen.width / 2 - 25, 1000));
		#region Component is a dialogue
		if(componentToEdit.GetType() == typeof(Dialogue)) {
			Dialogue dialogueToEdit = (Dialogue)componentToEdit;
			int itemsY = 10;
			if(dialogueToEdit.type == Dialogue.DialogueType.QUESTION) {
				// Drop down for bubble type
				GUI.Label (new Rect(0, itemsY, 100, 25), "Bubble Type: "); dropDownDialogueBubbleTypeMenu.Show(110, itemsY, Screen.width / 2 - 130, 25); itemsY += 30;
				dialogueToEdit.dialogueBubbleType = (Dialogue.DialogueLocation)(dropDownDialogueBubbleTypeMenu.SelectedItemIndex);
				// Uploader for voiceover
				if(dialogueToEdit.voiceOver != null)
					GUI.Label(new Rect(0, itemsY, 500, 25), "Voice Over: " + dialogueToEdit.voiceOver.name);
				else
					GUI.Label(new Rect(0, itemsY, 500, 25), "Voice Over: null");
				if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
					string imagePath = EditorUtility.OpenFilePanel("Voice Over", "Assets/Contents/Audio", "ogg");
					// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
				}
				if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
					dialogueToEdit.voiceOver = null;
				itemsY += 30;
				// Textarea for text
				if(dialogueToEdit.text == null)
					dialogueToEdit.text = "";
				GUI.Label (new Rect(0, itemsY, 30, 25), "Text: "); dialogueToEdit.text = GUI.TextArea(new Rect(40, itemsY, Screen.width / 2 - 60, 25), dialogueToEdit.text); itemsY += 30;
				// NPC Portrait 1
				if(dialogueToEdit.npc1Portrait != null)
					GUI.Label(new Rect(0, itemsY, 500, 25), "NPC 1 Portrait: " + dialogueToEdit.npc1Portrait.name);
				else
					GUI.Label(new Rect(0, itemsY, 500, 25), "NPC 1 Portrait: null");
				if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
					string imagePath = EditorUtility.OpenFilePanel("NPC 1 Portrait", "Assets/Contents/Audio", "png");
					// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
				}
				if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
					dialogueToEdit.npc1Portrait = null;
				itemsY += 30;
				// NPC 1 Portrait Height
				GUI.Label(new Rect(0, itemsY, 200, 25), "NPC 1 Portrait Height");
				string npc1HeightText = GUI.TextField(new Rect(200, itemsY, 100, 25), dialogueToEdit.npc1Height.ToString()); itemsY += 30;
				npc1HeightText = Regex.Replace(npc1HeightText, @"[^a-zA-Z0-9 ]", "");
				if(!float.TryParse(npc1HeightText, out dialogueToEdit.npc1Height))
					dialogueToEdit.npc1Height = 1f;
				// NPC Portrait 2
				if(dialogueToEdit.npc2Portrait != null)
					GUI.Label(new Rect(0, itemsY, 500, 25), "NPC 2 Portrait: " + dialogueToEdit.npc2Portrait.name);
				else
					GUI.Label(new Rect(0, itemsY, 500, 25), "NPC 2 Portrait: null");
				if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
					string imagePath = EditorUtility.OpenFilePanel("NPC 2 Portrait", "Assets/Contents/Audio", "png");
					// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
				}
				if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
					dialogueToEdit.npc2Portrait = null;
				itemsY += 30;
				// NPC 2 Portrait Height
				GUI.Label(new Rect(0, itemsY, 200, 25), "NPC 2 Portrait Height");
				string npc2HeightText = GUI.TextField(new Rect(200, itemsY, 100, 25), dialogueToEdit.npc2Height.ToString()); itemsY += 30;
				npc2HeightText = Regex.Replace(npc2HeightText, @"[^a-zA-Z0-9 ]", "");
				if(!float.TryParse(npc2HeightText, out dialogueToEdit.npc2Height))
					dialogueToEdit.npc2Height = 1f;
				// Dropdown for speaker
				GUI.Label (new Rect(0, itemsY, 50, 25), "Speaker: "); dropDownDialogueSpeakerMenu.Show(60, itemsY, Screen.width / 2 - 80, 25); itemsY += 30;
				dialogueToEdit.speaker = (Dialogue.Speaker)(dropDownDialogueSpeakerMenu.SelectedItemIndex);
				// NPC Number
				GUI.Label(new Rect(0, itemsY, 200, 25), "NPC Number");
				string npcNumber = GUI.TextField(new Rect(200, itemsY, 100, 25), dialogueToEdit.npcNo.ToString()); itemsY += 30;
				npcNumber = Regex.Replace(npcNumber, @"[^a-zA-Z0-9 ]", "");
				if(!int.TryParse(npcNumber, out dialogueToEdit.npcNo))
					dialogueToEdit.npcNo = 1;
				// Replies
				GUI.Label(new Rect(0, itemsY, 200, 25), "Replies");
				if(GUI.Button (new Rect(210, itemsY, Screen.width / 2 - 210, 25), "Add")) {
					Reply replyTemplate = dialogueToEdit.gameObject.GetComponent<Reply>();
					Reply newReply = dialogueToEdit.gameObject.AddComponent<Reply>();
					newReply.type = replyTemplate.type;
					newReply.text = "INSERT TEXT HERE";
					newReply.atlas = replyTemplate.atlas;
					newReply.dialogue = replyTemplate.dialogue;
					dialogueToEdit.wrongReplies.Add (newReply);
//					Dialogue dialogueTemplate = replyTemplate.dialogueReply
					GameObject newReplyDialogueGameObject = new GameObject("Reply" + (dialogueToEdit.wrongReplies.Count + 2));
					newReplyDialogueGameObject.transform.parent = dialogueToEdit.transform;
				}
				itemsY += 30;
				if(dialogueToEdit.wrongReplies != null) {
					// Wrong replies
					for(int i = 0; i < dialogueToEdit.wrongReplies.Count; i++) {
						GUI.Label(new Rect(10, itemsY, Screen.width / 2 - 200, 25), dialogueToEdit.wrongReplies[i].text);
						/*if(GUI.Toggle(new Rect(Screen.width / 2 - 200, itemsY, 100, 25), dialogueToEdit.wrongReplies[i] == dialogueToEdit.rightReply, "Right Reply")) {
							if(dialogueToEdit.rightReply != null)
								dialogueToEdit.wrongReplies.Add (dialogueToEdit.rightReply);
							dialogueToEdit.rightReply = dialogueToEdit.wrongReplies[i];
							dialogueToEdit.wrongReplies.Remove(dialogueToEdit.wrongReplies[i]);
						}*/
						if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove")) {
							DestroyImmediate(dialogueToEdit.wrongReplies[i], true);
							dialogueToEdit.wrongReplies.RemoveAt(i);
						}
						itemsY += 30;
					}
				}
				// Right reply
				if(dialogueToEdit.rightReply != null) {
					GUI.Label(new Rect(10, itemsY, Screen.width / 2 - 200, 25), dialogueToEdit.rightReply.text);
					/*if(!GUI.Toggle(new Rect(Screen.width / 2 - 200, itemsY, 100, 25), dialogueToEdit.rightReply == dialogueToEdit.rightReply, "Right Reply")) {
						dialogueToEdit.wrongReplies.Add (dialogueToEdit.rightReply);
						dialogueToEdit.rightReply = null;
					}*/
					itemsY += 30;
				}
				GUI.Label (new Rect(0, itemsY, 200, 25), "Number of Displayed Replies: "); dialogueToEdit.numDisplayedReplies = Convert.ToInt32(GUI.TextArea(new Rect(210, itemsY, Screen.width / 2 - 230, 25), dialogueToEdit.numDisplayedReplies.ToString())); itemsY += 30;
				dialogueToEdit.singleClick = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.singleClick, "Single Click"); itemsY += 30;
			}
			/*GUI.Label (new Rect(0, itemsY, Screen.width / 2 - 20, 30), "-DIALOGUE GRAPHICS-"); itemsY += 30;
			// Dropdown for displayType
			GUI.Label (new Rect(0, itemsY, 100, 25), "Display Type: "); dropDownDialogueDisplayTypeMenu.Show(110, itemsY, Screen.width / 2 - 130, 25); itemsY += 30;
			dialogueToEdit.displayType = (Dialogue.DialogueDisplayType)(dropDownDialogueDisplayTypeMenu.SelectedItemIndex);
			// Dropdown for bubbleType
			GUI.Label (new Rect(0, itemsY, 100, 25), "Bubble Type: "); dropDownDialogueBubbleTypeMenu.Show(110, itemsY, Screen.width / 2 - 130, 25); itemsY += 30;
			dialogueToEdit.dialogueBubbleType = (Dialogue.DialogueLocation)(dropDownDialogueBubbleTypeMenu.SelectedItemIndex);
			// Toggles for showBackgroundTexture, showWindow, clearWindow
			dialogueToEdit.showBackgroundTexture = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.showBackgroundTexture, "Show Background Texture"); itemsY += 30;
			dialogueToEdit.showWindow = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.showWindow, "Show Window"); itemsY += 30;
			dialogueToEdit.clearWindow = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.clearWindow, "Clear Window"); itemsY += 30;
			// Uploader for playerPortrait
			if(dialogueToEdit.playerPortrait != null)
				GUI.Label(new Rect(0, itemsY, 500, 25), "Player Portrait: " + dialogueToEdit.playerPortrait.name);
			else
				GUI.Label(new Rect(0, itemsY, 500, 25), "Player Portrait: null");
			if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
				string imagePath = EditorUtility.OpenFilePanel("Player Portrait", "Assets/Images", "png");
				// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
			}
			if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
				dialogueToEdit.playerPortrait = null;
			itemsY += 30;
			// Uploader for audioPortrait
			if(dialogueToEdit.audioPortrait != null)
				GUI.Label(new Rect(0, itemsY, 500, 25), "Audio Portrait: " + dialogueToEdit.audioPortrait.name);
			else
				GUI.Label(new Rect(0, itemsY, 500, 25), "Audio Portrait: null");
			if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
				string imagePath = EditorUtility.OpenFilePanel("Audio Portrait", "Assets/Images", "png");
				// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
			}
			if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
				dialogueToEdit.audioPortrait = null;
			itemsY += 30;
			// Toggle for switchNPCPortrait
			dialogueToEdit.switchNPCPortrait = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.switchNPCPortrait, "Switch NPC Portrait"); itemsY += 30;
			// Title for next section
			GUI.Label (new Rect(0, itemsY, Screen.width / 2 - 20, 30), "-DIALOGUE AUDIO AND TEXT-"); itemsY += 30;
			// Textarea for text
			if(dialogueToEdit.text == null)
				dialogueToEdit.text = "";
			GUI.Label (new Rect(0, itemsY, 30, 25), "Text: "); dialogueToEdit.text = GUI.TextArea(new Rect(40, itemsY, Screen.width / 2 - 60, 25), dialogueToEdit.text); itemsY += 30;
			// Uploader for voiceover
			GUI.Label(new Rect(0, itemsY, 200, 25), "Voice Over");
			if(dialogueToEdit.voiceOver != null)
				GUI.Label(new Rect(0, itemsY, 500, 25), "Voice Over: " + dialogueToEdit.voiceOver.name);
			else
				GUI.Label(new Rect(0, itemsY, 500, 25), "Voice Over: null");
			if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
				string imagePath = EditorUtility.OpenFilePanel("Voice Over", "Assets/Contents/Audio", "ogg");
				// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
			}
			if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
				dialogueToEdit.voiceOver = null;
			itemsY += 30;
			// Toggle for force repetition
			dialogueToEdit.forceRepetition = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.forceRepetition, "Force Repetition"); itemsY += 30;
			// Dropdown for speaker
			GUI.Label (new Rect(0, itemsY, 50, 25), "Speaker: "); dropDownDialogueSpeakerMenu.Show(60, itemsY, Screen.width / 2 - 80, 25); itemsY += 30;
			dialogueToEdit.speaker = (Dialogue.Speaker)(dropDownDialogueSpeakerMenu.SelectedItemIndex);
			GUI.Label(new Rect(0, itemsY, 200, 25), "Owner"); itemsY += 30;
			// New section for other linkages
			GUI.Label (new Rect(0, itemsY, Screen.width / 2 - 20, 30), "-AFTER DIALOGUE-"); itemsY += 30;
			dialogueToEdit.dialogueMilestone = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.dialogueMilestone, "Dialogue Milestone"); itemsY += 30;
			dialogueToEdit.displayTimer = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.displayTimer, "Display Timer"); itemsY += 30;
			GUI.Label (new Rect(0, itemsY, 100, 25), "Timer Duration: "); dialogueToEdit.timerDuration = Convert.ToSingle(GUI.TextArea(new Rect(110, itemsY, Screen.width / 2 - 130, 25), dialogueToEdit.timerDuration.ToString())); itemsY += 30;
			GUI.Label(new Rect(0, itemsY, 200, 25), "Event Causer"); itemsY += 30;
			// Next Dialogue editing
			if(dialogueToEdit.nextDialogue != null)
				GUI.Label(new Rect(0, itemsY, 500, 25), "Next Dialogue: " + dialogueToEdit.nextDialogue.name);
			else
				GUI.Label(new Rect(0, itemsY, 500, 25), "Next Dialogue: null");
			if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Change")) {
				linkToEdit = varToEdit.NEXTDIALOGUE;
			}
			if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove")) {
				Dialogue dialogueToDelete = dialogueToEdit.nextDialogue;
				if(dialogueToEdit.nextDialogue.nextDialogue != null)
					dialogueToEdit.nextDialogue = dialogueToEdit.nextDialogue.nextDialogue;
				DestroyImmediate(dialogueToDelete, true);
			}
			itemsY += 30;
			// New section for type and question
			GUI.Label (new Rect(0, itemsY, Screen.width / 2 - 20, 30), "-DIALOGUE TYPE AND REPLIES (if Question)-"); itemsY += 30;
			GUI.Label (new Rect(0, itemsY, 30, 25), "Type: "); dropDownDialogueTypeMenu.Show(40, itemsY, Screen.width / 2 - 60, 25); itemsY += 30;
			dialogueToEdit.type = (Dialogue.DialogueType)(dropDownDialogueTypeMenu.SelectedItemIndex);
			dialogueToEdit.singleClick = GUI.Toggle(new Rect(0, itemsY, 200, 25), dialogueToEdit.singleClick, "Single Click"); itemsY += 30;
			if(dialogueToEdit.type == Dialogue.DialogueType.QUESTION) {
				GUI.Label(new Rect(0, itemsY, 200, 25), "Wrong Replies");
				if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Change")) {
					linkToEdit = varToEdit.WRONGREPLIES;
				}
				itemsY += 30;
				if(dialogueToEdit.wrongReplies != null) {
					for(int i = 0; i < dialogueToEdit.wrongReplies.Count; i++) {
						GUI.Label(new Rect(20, itemsY, Screen.width / 2 - 170, 25), dialogueToEdit.wrongReplies[i].name + ": " + dialogueToEdit.wrongReplies[i].text);
						if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
							dialogueToEdit.wrongReplies.RemoveAt(i);
						itemsY += 30;
					}
				}
				if(dialogueToEdit.rightReply != null) {
					GUI.Label(new Rect(0, itemsY, 200, 25), "Right Reply: " + dialogueToEdit.rightReply.text);
				}else {
					GUI.Label(new Rect(0, itemsY, 200, 25), "Right Reply: null");
				}
				if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Change")) {
					linkToEdit = varToEdit.RIGHTREPLY;
				}
				if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
					dialogueToEdit.rightReply = null;
				itemsY += 30;
				GUI.Label (new Rect(0, itemsY, 200, 25), "Number of Displayed Replies: "); dialogueToEdit.numDisplayedReplies = Convert.ToInt32(GUI.TextArea(new Rect(210, itemsY, Screen.width / 2 - 230, 25), dialogueToEdit.numDisplayedReplies.ToString())); itemsY += 30;
			}*/
		}
		#endregion
		#region Component is a reply
		if(componentToEdit.GetType() == typeof(Reply)) {
			Reply replyToEdit = (Reply)componentToEdit;
			int itemsY = 10;
			// Type
			GUI.Label (new Rect(0, itemsY, 100, 25), "Reply Type: "); dropDownReplyTypeMenu.Show(110, itemsY, Screen.width / 2 - 130, 25); itemsY += 30;
			replyToEdit.type = (Reply.AnswerType)Enum.ToObject(typeof(Reply.AnswerType), dropDownReplyTypeMenu.SelectedItemIndex);
			// Text
			GUI.Label (new Rect(0, itemsY, 30, 25), "Text: "); replyToEdit.text = GUI.TextArea(new Rect(40, itemsY, Screen.width / 2 - 60, 25), replyToEdit.text); itemsY += 30;
			// Voiceover
			if(replyToEdit.voiceOver != null)
				GUI.Label(new Rect(0, itemsY, 500, 25), "Voice Over: " + replyToEdit.voiceOver.name);
			else
				GUI.Label(new Rect(0, itemsY, 500, 25), "Voice Over: null");
			if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
				string imagePath = EditorUtility.OpenFilePanel("Voice Over", "Assets/Contents/Audio", "ogg");
				// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
			}
			if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
				replyToEdit.voiceOver = null;
			itemsY += 30;
			// Image
			if(replyToEdit.type != Reply.AnswerType.TEXT) {
				if(replyToEdit.image != null)
					GUI.Label(new Rect(0, itemsY, 500, 25), "Image: " + replyToEdit.image.name);
				else
					GUI.Label(new Rect(0, itemsY, 500, 25), "Image: null");
				if(GUI.Button(new Rect(Screen.width / 2 - 170, itemsY, 70, 25), "Add")) {
					string imagePath = EditorUtility.OpenFilePanel("Image", "Assets/Contents/Audio", "png");
					// Can't get texture to load dynamically, tried Resources.Load, AssetDatabase.LoadAssetAtPath, WWW
				}
				if(GUI.Button(new Rect(Screen.width / 2 - 90, itemsY, 70, 25), "Remove"))
					replyToEdit.image = null;
				itemsY += 30;
			}
			// Available
			replyToEdit.available = GUI.Toggle(new Rect(0, itemsY, 200, 25), replyToEdit.available, "Available"); itemsY += 30;
		}
		#endregion
		GUI.EndScrollView();
	}
	/*
	void displayEditLink() {
		scrollViewLinkages = GUI.BeginScrollView (new Rect (Screen.width / 2, Screen.height * 2 / 3, Screen.width / 2, Screen.height / 3), scrollViewLinkages, new Rect (0, 0, Screen.width / 2 - 25, 2000));
		#region NEXTDIALOGUE
		if(linkToEdit == varToEdit.NEXTDIALOGUE) {
			ConversationTree cTree = ((GameObject)conversationList[NPCtoEdit]).GetComponent<ConversationTree>(); // Get conversation tree
			ArrayList dialogueList = getDialogues(cTree.root);
			string[] stringArray = new string[dialogueList.Count + 1];
			for(int i = 0; i < dialogueList.Count; i++) {
				stringArray[i] = ((Dialogue)dialogueList[i]).gameObject.name + ": " + ((Dialogue)dialogueList[i]).text;
			}
			stringArray[dialogueList.Count] = "None";
			int selectionIndex = dialogueList.IndexOf(((Dialogue)componentToEdit).nextDialogue);
			int previousIndex = selectionIndex;
			if(((Dialogue)componentToEdit).nextDialogue == null)
				selectionIndex = dialogueList.Count;
			// Get the new selection from user
			if(((Dialogue)componentToEdit).nextDialogue != null) 
				selectionIndex = GUI.SelectionGrid(new Rect(0, 0, Screen.width / 2, dialogueList.Count * 25), selectionIndex, stringArray, 1, "toggle");
			else
				selectionIndex = GUI.SelectionGrid(new Rect(0, 0, Screen.width / 2, dialogueList.Count * 25), selectionIndex, stringArray, 1, "toggle");
			// Assign new dialogue to user
			if(selectionIndex == dialogueList.Count)
				((Dialogue)componentToEdit).nextDialogue = null;
			else
				((Dialogue)componentToEdit).nextDialogue = (Dialogue)dialogueList[selectionIndex];
			// Check if the edit is valid
			try {
				getDialogueReply(cTree.root);
			} catch(InsufficientMemoryException e) {
				selectionIndex = previousIndex;
				if(selectionIndex == dialogueList.Count)
					((Dialogue)componentToEdit).nextDialogue = null;
				else
					((Dialogue)componentToEdit).nextDialogue = (Dialogue)dialogueList[selectionIndex];
			}
		}
		#endregion
		#region REPLIES
		if(linkToEdit == varToEdit.RIGHTREPLY || linkToEdit == varToEdit.WRONGREPLIES) {
			ConversationTree cTree = ((GameObject)conversationList[NPCtoEdit]).GetComponent<ConversationTree>(); // Get conversation tree
			ArrayList replyList = getReplies(cTree.root);
			string[] stringArray = new string[replyList.Count + 1];
			for(int i = 0; i < replyList.Count; i++) {
				Reply r = (Reply)replyList[i];
				stringArray[i] = r.gameObject.name + ": " + r.text;
			}
			stringArray[replyList.Count] = "None";
			if(linkToEdit == varToEdit.RIGHTREPLY) {
				int selectionIndex = replyList.IndexOf(((Dialogue)componentToEdit).rightReply);
				int previousIndex = selectionIndex;
				if(((Dialogue)componentToEdit).rightReply == null)
					selectionIndex = replyList.Count;
				if(((Dialogue)componentToEdit).rightReply != null) 
					selectionIndex = GUI.SelectionGrid(new Rect(0, 0, Screen.width / 2, replyList.Count * 25), selectionIndex, stringArray, 1, "toggle");
				else
					selectionIndex = GUI.SelectionGrid(new Rect(0, 0, Screen.width / 2, replyList.Count * 25), selectionIndex, stringArray, 1, "toggle");
				// Make the edit
				if(selectionIndex == replyList.Count)
					((Dialogue)componentToEdit).rightReply = null;
				else
					((Dialogue)componentToEdit).rightReply = (Reply)replyList[selectionIndex];
				// Check if edit is valid
				try {
					getDialogueReply(cTree.root);
				} catch(InsufficientMemoryException e) {
					selectionIndex = previousIndex;
					if(selectionIndex == dialogueList.Count)
						((Dialogue)componentToEdit).rightReply = null;
					else
						((Dialogue)componentToEdit).rightReply = (Reply)replyList[selectionIndex];
				}
			} else if(linkToEdit == varToEdit.WRONGREPLIES) {
				int itemsY = 0;
				for(int i = 0 ; i < replyList.Count; i++) {
					if(((Dialogue)componentToEdit).wrongReplies != null) {
						for(int j = 0; j < ((Dialogue)componentToEdit).wrongReplies.Count; j++) {
							if(GUI.Toggle(new Rect(0, itemsY, Screen.width / 2 - 20, 25), ((Dialogue)componentToEdit).wrongReplies.Contains((Reply)replyList[i]), ((Reply)replyList[i]).name + ": " + ((Reply)replyList[i]).text, "toggle")) {
								if(!((Dialogue)componentToEdit).wrongReplies.Contains((Reply)replyList[i]) &&
										!((Reply)replyList[i]).dialogue.rightReply.Equals((Reply)replyList[i])) {
									((Dialogue)componentToEdit).wrongReplies.Add((Reply)replyList[i]);
									((Reply)replyList[i]).dialogue = ((Dialogue)componentToEdit);
								}
							}
						}
					} else {
						GUI.Toggle(new Rect(0, itemsY, Screen.width / 2 - 20, 25), false, ((Reply)replyList[i]).name + ": " + ((Reply)replyList[i]).text);
					}
					itemsY += 30;
				}
			}
		}
		#endregion
		GUI.EndScrollView();
	}
	*/
}