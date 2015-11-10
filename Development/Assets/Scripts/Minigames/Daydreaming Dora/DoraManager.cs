using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoraManager : MinigameManager
{
 
    public int currLevel;
    int numLevels = 3;
    public GameObject teacherTapHolder;
    public GameObject imageSlots;
    public GameObject doraChalkboardStills;
    public GameObject draggableItems;
    public GameObject teacher;
    NPCDialogueAnimation teacherAnimation;
    public GameObject teacherGlow; 
    //public GameObject category1; 
    public GameObject iconBG;
    public HintButtonTeacher hintButton;
    public bool showTutorial;
    public GameObject tutorialHand;
    public GameObject tutorialCow;
    public GameObject tutorialMoon;
    public Dialogue[] tutorialLines;
    public GameObject chalkboardItemStatic;
    public GameObject chalkboardItemInteractive;
    public UIAtlas chalkboardItemAtlas;
    ArrayList allChalkboardItems = new ArrayList(); // Holds the items on the chalkboard
    ArrayList randomCategoryOptions = new ArrayList(); // List to randomize the categories displayed
    ArrayList randomColorOptions = new ArrayList(); // List to randomize the colors displayed
    ArrayList dropSpotLocations = new ArrayList(); // Holds all the containers for the dropspots
 
    public TeacherTapped teacherScript;
    public Minigame minigame;

    //the tutorial options menu
    public GameObject tutorialOptions;
    public Dialogue preIntro1;
    public Dialogue preIntro2;
    public Dialogue introTeach;
    public Dialogue teacherInstruction;
    public Dialogue playerReady;
    public Dialogue roundEnd;
    public Dialogue endMinigame;
    public Dialogue correctAnswer;
    public Dialogue WrongAnswerRightObject;
    public Dialogue WrongAnswerWrongObject;
    private bool allowDialogueCoroutine = true;
    public GameObject teacherSpeechBubble;
    public GameObject teacherSpeechText;
    public int numSlotsCor;
    Queue<Dialogue> hideTeacherDialogueQueue = new Queue<Dialogue>();
    Dialogue currentDialogue = null;
    public AudioClip rightAnswerFX;
    public AudioClip wrongAnswerFX;
 
    [System.SerializableAttribute]
    public class TeacherInstruction
    {
        public string categroy;
        public AudioClip audioClip;
        public string text;
    }
    public List<TeacherInstruction> teacherInstructions;

    // Use this for initialization
    void Start()
    {
        Sherlock.Instance.SetBubblePosition(Sherlock.side.DOWN);

		teacherScript.teacherCollider.enabled = false;

        teacherAnimation = teacher.GetComponent<NPCDialogueAnimation>();

        setTeacherAnimation(NPCAnimations.AnimationIndex.IDLE);
        teacherSpeechBubble.SetActive(false);
        tutorialCow.SetActive(false);
        tutorialMoon.SetActive(false);
  
        
        setTeacherAnimation(NPCAnimations.AnimationIndex.INSTRUCTING);
        Respond(preIntro1);
        tutorialOptions.SetActive(false);
        Invoke("StartIntroduction", preIntro1.voiceOver.length + preIntro2.voiceOver.length);
    }
 
    public void StartIntroduction()
    {
        tutorialOptions.SetActive(true);
        if (!minigame.isStandalone()) //if the minigame is part of the story, not free play mode
        {
            Destroy(tutorialOptions);
            if (showTutorial)
            {
                StartCoroutine("startTutorial");
            } else
            {
                setupGame();
            }
        }
    }
    
    public override void yesTutorial()
    {
        Destroy(tutorialOptions);
        StartCoroutine("startTutorial");
    }

    public override void noTutorial()
    {
        Destroy(tutorialOptions);
        setupGame();
    }

    void setTeacherAnimation(NPCAnimations.AnimationIndex animationType)
    {
        NPCAnimations.AnimationSequence currAnimSeq = teacher.GetComponent<NPCAnimations>().RetrieveAnimationSequence(animationType);
        List<Texture> currAnimSeqTextures = currAnimSeq.textures;
        if (currAnimSeqTextures.Count > 0)
        {
            teacherAnimation.StopAnimation();
            teacherAnimation.SetAnimationList(currAnimSeqTextures);
            teacherAnimation.PlayAnimation();
            teacherAnimation.SetSpeed(currAnimSeq.speed);
        }
    }

    void setTeacherIdleAnimation()
    {
        setTeacherAnimation(NPCAnimations.AnimationIndex.IDLE);
    }
 
    void setupGame()
    {
        dropSpotLocations = getImageSlots();
        setupChalkboard();
        draggableItems.SetActive(false); 
        //teacherGlow.SetActive(false); 
        iconBG.SetActive(false);
        //category1.SetActive(false);
     
        numSlotsCor = 0; 
     
        //teacherScript = teacher.GetComponent<TeacherTapped>(); 
     
        currLevel = 0; 

		Invoke("startGame", 0.1f);
        //startDialogue();
    }

    // Gets all the slots available from the image slot gameobject
    ArrayList getImageSlots()
    {
        ArrayList imageSlotList = new ArrayList();
        foreach (Transform t in imageSlots.transform)
        {
            if (t.gameObject.name.Contains("Square"))
                imageSlotList.Add(t.gameObject);
        }
        // Order them according to their names
        for (int i = 0; i < imageSlotList.Count; i++)
        {
            int gIndex = i;
            do
            {
                GameObject g = (GameObject)imageSlotList [i];
                gIndex = int.Parse(g.name.Remove(0, 6)) - 1;
                GameObject gameObjectAtCorrectIndex = (GameObject)imageSlotList [gIndex];
                imageSlotList [gIndex] = g;
                imageSlotList [i] = gameObjectAtCorrectIndex;
            } while(gIndex != i);
        }
        return imageSlotList;
    }

    // Populates the draggableItemsLevel gameobject with 4 blackboard items that are clickable
    // Also assigns each a dropspot from the dropspotlist passed in
    void setDraggableItems(GameObject draggableItemsLevel, ArrayList dropSpotLocations)
    {
        ArrayList draggableItems = new ArrayList();
        if (minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
        {
            ArrayList currentCategoryItems = getChalkboardItemsFromChalkboardOfType((string)randomCategoryOptions [currLevel]);
            for (int i = 0; i < 3; i++)
            {
                GameObject newItem = (GameObject)Instantiate(chalkboardItemInteractive);
                newItem.transform.parent = draggableItemsLevel.transform;
                newItem.transform.localPosition = new Vector3(-230f + 130f * i, -175, -10);
                newItem.GetComponent<UIStretch>().relativeSize.y = .15f;
                newItem.GetComponent<DraggableObjectDora>().inactiveSize = .15f;
                newItem.GetComponent<DraggableObjectDora>().hasBeenSolved = false;
                newItem.GetComponent<UISprite>().spriteName = ((GameObject)currentCategoryItems [i]).GetComponent<UISprite>().spriteName;
                newItem.GetComponent<DoraTappableObject>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().dropspot = (GameObject)dropSpotLocations [getIndexOfChalkboardItem((GameObject)currentCategoryItems [i])];
                ((GameObject)currentCategoryItems [i]).SetActive(false);
                draggableItems.Add(newItem);
            }
            int categoryIndex = 3;
            for (int i = 3; i < 5; i++)
            {
                GameObject newItem = (GameObject)Instantiate(chalkboardItemInteractive);
                newItem.transform.parent = draggableItemsLevel.transform;
                newItem.transform.localPosition = new Vector3(-230f + 130f * i, -175, -10);
                newItem.GetComponent<UIStretch>().relativeSize.y = .15f;
                newItem.GetComponent<DraggableObjectDora>().inactiveSize = .15f;
                categoryIndex = Random.Range(0, 4);
                if ((string)randomCategoryOptions [categoryIndex] == (string)randomCategoryOptions [currLevel])
                {
                    categoryIndex++;
                    categoryIndex = categoryIndex % 5;
                }
                newItem.GetComponent<UISprite>().spriteName = "props_" + (string)randomCategoryOptions [categoryIndex] + "_" + (string)randomColorOptions [i % 5];
                newItem.GetComponent<DraggableObjectDora>().hasBeenSolved = false;
                newItem.GetComponent<DoraTappableObject>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().dropspot = null;
                draggableItems.Add(newItem);
            }
        }
        if (minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
        {
            ArrayList currentCategoryItems = getChalkboardItemsFromChalkboardOfType((string)randomCategoryOptions [currLevel]);
            for (int i = 0; i < 5; i++)
            {
                GameObject newItem = (GameObject)Instantiate(chalkboardItemInteractive);
                newItem.transform.parent = draggableItemsLevel.transform;
                newItem.transform.localPosition = new Vector3(-290f + 105f * i, -175, -10);
                newItem.GetComponent<UIStretch>().relativeSize.y = .15f;
                newItem.GetComponent<DraggableObjectDora>().inactiveSize = .15f;
                newItem.GetComponent<DraggableObjectDora>().hasBeenSolved = false;
                newItem.GetComponent<UISprite>().spriteName = ((GameObject)currentCategoryItems [i]).GetComponent<UISprite>().spriteName;
                newItem.GetComponent<DoraTappableObject>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().dropspot = (GameObject)dropSpotLocations [getIndexOfChalkboardItem((GameObject)currentCategoryItems [i])];
                ((GameObject)currentCategoryItems [i]).SetActive(false);
                draggableItems.Add(newItem);
            }
            int categoryIndex = 3;
            for (int i = 5; i < 7; i++)
            {
                GameObject newItem = (GameObject)Instantiate(chalkboardItemInteractive);
                newItem.transform.parent = draggableItemsLevel.transform;
                newItem.transform.localPosition = new Vector3(-290f + 105f * i, -175, -10);
                newItem.GetComponent<UIStretch>().relativeSize.y = .15f;
                newItem.GetComponent<DraggableObjectDora>().inactiveSize = .15f;
                categoryIndex = Random.Range(0, 4);
                if ((string)randomCategoryOptions [categoryIndex] == (string)randomCategoryOptions [currLevel])
                {
                    categoryIndex++;
                    categoryIndex = categoryIndex % 5;
                }
                newItem.GetComponent<DraggableObjectDora>().hasBeenSolved = false;
                newItem.GetComponent<UISprite>().spriteName = "props_" + (string)randomCategoryOptions [categoryIndex] + "_" + (string)randomColorOptions [i % 5];
                newItem.GetComponent<DoraTappableObject>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().dropspot = null;
                draggableItems.Add(newItem);
            }
        }
        if (minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
        {
            for (int i = 0; i < 9; i++)
            {
                GameObject newItem = (GameObject)Instantiate(chalkboardItemInteractive);
                newItem.transform.parent = draggableItemsLevel.transform;
                newItem.transform.localPosition = new Vector3(-300f + 85f * i, -175, -10);
                newItem.GetComponent<UIStretch>().relativeSize.y = .13f;
                newItem.GetComponent<DraggableObjectDora>().inactiveSize = .13f;
                newItem.GetComponent<DraggableObjectDora>().hasBeenSolved = false;
                newItem.GetComponent<UISprite>().spriteName = ((GameObject)allChalkboardItems [i]).GetComponent<UISprite>().spriteName;
                newItem.GetComponent<DoraTappableObject>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().managerObj = this.gameObject;
                newItem.GetComponent<DraggableObjectDora>().dropspot = (GameObject)dropSpotLocations [getIndexOfChalkboardItem((GameObject)allChalkboardItems [i])];
                ((GameObject)allChalkboardItems [i]).SetActive(false);
                draggableItems.Add(newItem);
            }
        }
        draggableItems = shuffleArrayListPlacement(draggableItems);
    }

    // Sets up the chalkboard with random images based on the level
    void setupChalkboard()
    {
        // These randomOptions arraylists determine which color/options to select
        if (randomCategoryOptions.Count == 0)
        { // If the list is empty, populate it with the categories
            foreach (string name in System.Enum.GetNames(typeof(DoraImage.DoraCategory)))
                randomCategoryOptions.Add(name);
            randomCategoryOptions = shuffleArrayList(randomCategoryOptions);
        }
        if (randomColorOptions.Count == 0)
        { // If the list is empty, populate it with the categories
            foreach (string name in System.Enum.GetNames(typeof(DoraImage.DoraColor)))
                randomColorOptions.Add(name);
        }
        // Easy difficulty, runs only once
        // Display 3 categories on board, randomize which categories and which icons
        // Each round is a different category that's missing, don't change the stuff on the board
        if (minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
        {
            int counter = 0; // Used for the positioning of the chalkboard items
            for (int i = 0; i < 3; i++)
            {
                string category = (string)randomCategoryOptions [i]; // Get a random category
                ArrayList typeOptions = new ArrayList(); // Populate a list with all pics from the random type, either color or category
                typeOptions = getAllItemsOfType(category);
                typeOptions = shuffleArrayList(typeOptions);
                for (int j = 0; j < 3; j++)
                { // Select three items from the type
                    if (counter == 5)
                        counter++;
                    GameObject newChalkboardItem0 = (GameObject)Instantiate(chalkboardItemStatic);
                    newChalkboardItem0.transform.parent = doraChalkboardStills.transform;
                    newChalkboardItem0.transform.localPosition = new Vector3(-310f + 155f * (counter % 5), 355f + counter / 5 * -185, 0);
                    newChalkboardItem0.GetComponent<UISprite>().spriteName = (string)typeOptions [j];
                    allChalkboardItems.Add(newChalkboardItem0);
                    counter++;
                }
            }
            allChalkboardItems = shuffleArrayListPlacement(allChalkboardItems);
            // Medium difficulty
            // 5 from one category, doesn't matter where the other 4 come from
            // Find the 5, each round choose from different category
        } else if (minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
        {
            allChalkboardItems.Clear();
            int counter = 0; // Used for the positioning of the chalkboard items
            string category = (string)randomCategoryOptions [currLevel]; // Get a random category
            ArrayList typeOptions = new ArrayList(); // Populate a list with all pics from the random type, either color or category
            typeOptions = getAllItemsOfType(category);
            typeOptions = shuffleArrayList(typeOptions);
            for (int i = 0; i < 5; i++)
            { // Get the hidden items
                GameObject newChalkboardItem0 = (GameObject)Instantiate(chalkboardItemStatic);
                newChalkboardItem0.transform.parent = doraChalkboardStills.transform;
                newChalkboardItem0.transform.localPosition = new Vector3(-310f + 155f * (counter % 5), 355f + counter / 5 * -185, 0);
                newChalkboardItem0.GetComponent<UISprite>().spriteName = (string)typeOptions [i];
                allChalkboardItems.Add(newChalkboardItem0);
                counter++;
            }
            counter++;
            randomColorOptions = shuffleArrayList(randomColorOptions);
            for (int i = currLevel + 1; i < currLevel + 5; i++)
            { // Get the random items to fill up the board
                int categoryIndex = i;
                if (categoryIndex == randomCategoryOptions.IndexOf(category))
                    categoryIndex++;
                if (categoryIndex >= randomCategoryOptions.Count)
                    categoryIndex -= randomCategoryOptions.Count;
                GameObject newChalkboardItem0 = (GameObject)Instantiate(chalkboardItemStatic);
                newChalkboardItem0.transform.parent = doraChalkboardStills.transform;
                newChalkboardItem0.transform.localPosition = new Vector3(-310f + 155f * (counter % 5), 355f + counter / 5 * -185, 0);
                newChalkboardItem0.GetComponent<UISprite>().spriteName = "props_" + (string)randomCategoryOptions [categoryIndex] + "_" + (string)randomColorOptions [i - currLevel];
                allChalkboardItems.Add(newChalkboardItem0);
                counter++;
            }
            allChalkboardItems = shuffleArrayListPlacement(allChalkboardItems);
            // Hard difficulty, runs at start of each level
            // 9 from any category
        } else if (minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
        {
            allChalkboardItems.Clear();
            int counter = 0; // Used for the positioning of the chalkboard items
            randomCategoryOptions = shuffleArrayList(randomCategoryOptions);
            string category = (string)randomCategoryOptions [Random.Range(0, randomCategoryOptions.Count - 1)]; // Get a random category
            randomColorOptions = shuffleArrayList(randomColorOptions);
            for (int i = 0; i < 9; i++)
            { // Get the hidden items
                if (i % 2 == 0)
                {
                    category = (string)randomCategoryOptions [i % 5];
                    randomColorOptions = shuffleArrayList(randomColorOptions);
                }
                GameObject newChalkboardItem0 = (GameObject)Instantiate(chalkboardItemStatic);
                newChalkboardItem0.transform.parent = doraChalkboardStills.transform;
                newChalkboardItem0.transform.localPosition = new Vector3(-310f + 155f * (counter % 5), 355f + counter / 5 * -185, 0);
                newChalkboardItem0.GetComponent<UISprite>().spriteName = "props_" + category + "_" + (string)randomColorOptions [i % 5];
                allChalkboardItems.Add(newChalkboardItem0);
                counter++;
                if (counter == 5)
                    counter++;
            }
            allChalkboardItems = shuffleArrayListPlacement(allChalkboardItems);
        }
    }

    // Makes all chalkboard items active, and all the previous draggable items inactive
    void setupNextLevel()
    {
        foreach (Transform t in draggableItems.transform)
            Destroy(t.gameObject);
        if (minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
        {
            foreach (GameObject g in allChalkboardItems)
                g.SetActive(true);
        } else
        { // Medium or hard
            foreach (GameObject g in allChalkboardItems)
                Destroy(g);
            setupChalkboard();
        }
		//setDraggableItems(draggableItems, dropSpotLocations);
		teacherScript.gameStarted = false;
    }

    void cleanupPreviousLevel()
    {
        foreach (Transform t in draggableItems.transform)
        {
            Destroy(t.gameObject);
        }
        if (minigame.difficulty != MinigameDifficulty.Difficulty.EASY)
        {
            foreach (GameObject g in allChalkboardItems)
            {
                Destroy(g);
            }
        }
    }

    // Shuffles items in an arraylist
    ArrayList shuffleArrayList(ArrayList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count - 1);
            object randomIndexObject = list [randomIndex];
            list [randomIndex] = list [i];
            list [i] = randomIndexObject;
        }
        return list;
    }

    // Given an arraylist of gameobjects with transforms, shuffles their positions
    // Used to randomize the items' positions on the board
    ArrayList shuffleArrayListPlacement(ArrayList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count - 1);
            GameObject randomIndexObject = (GameObject)list [randomIndex];
            float randomIndexObjectX = randomIndexObject.transform.localPosition.x;
            float randomIndexObjectY = randomIndexObject.transform.localPosition.y;
            float randomIndexObjectZ = randomIndexObject.transform.localPosition.z;
            randomIndexObject.transform.localPosition = new Vector3(
             ((GameObject)list [i]).transform.localPosition.x,
             ((GameObject)list [i]).transform.localPosition.y,
             ((GameObject)list [i]).transform.localPosition.z);
            ((GameObject)list [i]).transform.localPosition = new Vector3(randomIndexObjectX, randomIndexObjectY, randomIndexObjectZ);
            list [randomIndex] = list [i];
            list [i] = randomIndexObject;
        }
        return list;
    }

    ArrayList getAllItemsOfType(string typeName)
    {
        ArrayList spriteNamesList = new ArrayList();
        foreach (string s in chalkboardItemAtlas.GetListOfSprites())
        {
            if (s.Contains(typeName))
            {
                spriteNamesList.Add(s);
            }
        }
        return spriteNamesList;
    }

    // Gets all chalkboard items in the list that contains the spriteName string, such as "beach"
    ArrayList getChalkboardItemsFromChalkboardOfType(string spriteName)
    {
        ArrayList spriteNamesList = new ArrayList();
        foreach (GameObject g in allChalkboardItems)
        {
            string currentSpriteName = g.GetComponent<UISprite>().spriteName;
            if (currentSpriteName.Contains(spriteName))
                spriteNamesList.Add(g);
        }
        return spriteNamesList;
    }

    public GameObject getChalkboardItemFromChalkboardThatMatches(string spriteName)
    {
        foreach (GameObject g in allChalkboardItems)
        {
            string currentSpriteName = g.GetComponent<UISprite>().spriteName;
            if (currentSpriteName == spriteName)
                return g;
        }
        return null;
    }

    // Given a gameobject, gets the index of that gameobject
    int getIndexOfChalkboardItem(GameObject chalkboardItem)
    {
        int itemIndex = -1;
        foreach (GameObject g in allChalkboardItems)
        {
            itemIndex++;
            if (g == chalkboardItem)
                return itemIndex;
        }
        return itemIndex;
    }

    public void showHint()
    {
        foreach (GameObject g in allChalkboardItems)
        {
            g.SetActive(true);
        }
        foreach (Transform t in draggableItems.transform)
        {
            t.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        Invoke("hideHint", 3f);
    }

    void hideHint()
    {
        ArrayList chalkboardItems = null;
        if (minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
        {
            chalkboardItems = allChalkboardItems;
        } else
        {
            chalkboardItems = getChalkboardItemsFromChalkboardOfType((string)randomCategoryOptions [currLevel]);
        }
        foreach (Transform t in draggableItems.transform)
        {
            GameObject g = t.gameObject;
            if (!g.GetComponent<DraggableObjectDora>().hasBeenSolved)
            {
                g.GetComponent<BoxCollider>().enabled = true;
                foreach (GameObject child in chalkboardItems)
                {
                    if (child.GetComponent<UISprite>().spriteName == g.GetComponent<UISprite>().spriteName)
                    {
                        child.SetActive(false);
                    }
                }
            }
        }
        teacherScript.isShowing = false;
    }

    int FindInstructionDialogue(string category)
    {
        for (int i = 0 ; i < teacherInstructions.Count ; i++)
        {
            if (category == teacherInstructions[i].categroy)
                return i;
        }
        return -1;
    }
    
    // Gets the dialogue ending corresponding to the current type in the level
    void setDialogueInstruction(Dialogue dialogue)
    {
        string newText = null;
        int index = -1;
        if (minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
        {
            index = FindInstructionDialogue("all");
        } else
        {
            index = FindInstructionDialogue((string)randomCategoryOptions [currLevel]);
        }
        
        if (index > -1)
        {
            dialogue.text = teacherInstructions[index].text;
            dialogue.voiceOver = teacherInstructions[index].audioClip;
        }
    }
/*
    void startDialogue()
    {
        setTeacherAnimation(NPCAnimations.AnimationIndex.INSTRUCTING);
        Respond(introTeach);
    }*/
 
    /// <summary>
    /// Start coroutine that waits for input
    /// </summary>
    /// <param name='method'>
    /// The method to be invoked once the player has clicked somewhere
    /// </param>
    public void Wait(string method)
    {
        StartCoroutine(WaitForInput(method));
    }
    /// <summary>
    /// Waits until the user clicks the mouse, then calls the function method (passed in)
    /// </summary>
    /// <param name='method'>
    /// The method to be invoked once the player has clicked somewhere
    /// </param>
    IEnumerator WaitForInput(string method)
    {
        while (!InputManager.Instance.HasReceivedClick())
        {
            yield return 0;
        }
        Invoke(method, 0);
    }

    public void numSlotsCorrect()
    {
        //AudioManager.Instance.Play(rightAnswerFX, transform, 1, false);
        numSlotsCor++;
        checkGameComplete();
    }

    public void wrongResponseRightObject()
    {
        //AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
        setTeacherAnimation(NPCAnimations.AnimationIndex.DOUBTFUL);
        Respond(WrongAnswerRightObject);
    }

    public void wrongResponseWrongObject()
    {
        //AudioManager.Instance.Play(wrongAnswerFX, transform, 1, false);
        setTeacherAnimation(NPCAnimations.AnimationIndex.DOUBTFUL);
        Respond(WrongAnswerWrongObject);
    }

    public void TeacherTap()
    {
        StopAllCoroutines();
        //allowDialogueCoroutine = false;
        Respond(playerReady); 
        //teacherGlow.SetActive(false);
        iconBG.SetActive(true); 
     
        if (currLevel == 0)
        {
            //Invoke ("startGame", minigame.GetCurrentDialogueDuration());
            setDraggableItems(draggableItems, dropSpotLocations);
			teacherScript.teacherCollider.enabled = true;
            draggableItems.SetActive(true);
        } else if (currLevel == 1)
        {
            //Respond(playerReady);
            setDraggableItems(draggableItems, dropSpotLocations);
			teacherScript.teacherCollider.enabled = true;
            draggableItems.SetActive(true);
        } else if (currLevel == 2)
        { 
            //Respond(playerReady); 
            setDraggableItems(draggableItems, dropSpotLocations);
			teacherScript.teacherCollider.enabled = true;
            draggableItems.SetActive(true);
        }
    }

    void checkGameComplete()
    {
        if (numSlotsCor == 3 && minigame.difficulty == MinigameDifficulty.Difficulty.EASY)
        {
            draggableItems.SetActive(false);
			teacherScript.teacherCollider.enabled = false;
            currLevel++;
            if (currLevel < numLevels)
            {
                switchLevel();
            } else
            {
                Respond(endMinigame);
                Invoke("CompleteMinigame", minigame.GetCurrentDialogueComulativeDuration());
            }
            //Invoke ("switchLevel", minigame.GetCurrentDialogueDuration());
            //StartCoroutine(WaitForInput("clearBoard")); 
        } else if (numSlotsCor == 5 && minigame.difficulty == MinigameDifficulty.Difficulty.MEDIUM)
        {
            draggableItems.SetActive(false);
			teacherScript.teacherCollider.enabled = false;
            currLevel++;
            if (currLevel < numLevels)
            {
                switchLevel();
            } else
            {
                Respond(endMinigame);
                Invoke("CompleteMinigame", minigame.GetCurrentDialogueComulativeDuration());
            }
            //Invoke ("switchLevel", minigame.GetCurrentDialogueDuration());
            //StartCoroutine(WaitForInput("clearBoard")); 
        } else if (numSlotsCor == 9 && minigame.difficulty == MinigameDifficulty.Difficulty.HARD)
        {
            draggableItems.SetActive(false);
			teacherScript.teacherCollider.enabled = false;
            currLevel++;
            if (currLevel < numLevels)
            {
                switchLevel();
            } else
            {
                Respond(endMinigame);
                Invoke("CompleteMinigame", minigame.GetCurrentDialogueComulativeDuration());
            }
            //Invoke ("switchLevel", minigame.GetCurrentDialogueDuration());
            //StartCoroutine(WaitForInput("clearBoard")); 
        } else
        {
            Respond(correctAnswer);
        }
    }

    void startGame()
    {    
        if (currLevel == 0)
        {
            draggableItems.SetActive(true);
			teacherScript.teacherCollider.enabled = true;
            Respond(teacherInstruction);
        }
    }

    void level1()
    { 
        iconBG.SetActive(false);
		teacherScript.teacherCollider.enabled = false;
        setupNextLevel();
		teacherScript.teacherCollider.enabled = true;
        Respond(teacherInstruction);
    }

    void level1Response()
    {
        Respond(teacherInstruction);
    }

    void level2()
    { 
        iconBG.SetActive(false);
		teacherScript.teacherCollider.enabled = false;
        setupNextLevel();
		teacherScript.teacherCollider.enabled = true;
        Respond(teacherInstruction);
    }

    void level2Response()
    {
        Respond(teacherInstruction);
    }

    void glowOn()
    {
        teacherGlow.SetActive(true);
    }

    /*void clearBoard()
 {

 }*/

    void switchLevel()
    {
        switch (currLevel)
        {
            case 0:
                break;
            case 1:
                Respond(roundEnd); 
                numSlotsCor = 0;
                Invoke("level1", minigame.GetCurrentDialogueComulativeDuration());
                break;
            case 2:
                Respond(roundEnd);
                numSlotsCor = 0;
                Invoke("level2", minigame.GetCurrentDialogueComulativeDuration());
                break;
            case 3:
                Respond(roundEnd);
                numSlotsCor = 0;
                break;
        }
    }

    //This was for the quiz version of this game. If we do not return to that, get rid of this and the related icons
    /*
 public void doraNextSquare()
 {
     currLevel++;
     switch(currLevel)
     {
     case 0:
         break;
     case 1:
         //imageSlots.SetActive(true); 
         item1.SetActive(true); 
         square1.SetActive(false);
         square2.SetActive(true);
         buttonRow1.SetActive(false);
         buttonRow2.SetActive(true);
         break;
     case 2:
         item2.SetActive(true);
         square2.SetActive(false);
         square3.SetActive(true);
         buttonRow2.SetActive(false);
         buttonRow3.SetActive(true);
         break;
     case 3:
         item3.SetActive(true);
         square3.SetActive(false);
         square4.SetActive(true);
         buttonRow3.SetActive(false);
         buttonRow4.SetActive(true);
         break;
     case 4:
         item4.SetActive(true);
         square4.SetActive(false);
         buttonRow4.SetActive(false);
         break;
     }*/

 
    public void Respond(Dialogue response)
    {
        currentDialogue = response;
        if (response == teacherInstruction)
        {
            setDialogueInstruction(response);
        }

        if (response.speaker != Dialogue.Speaker.NPC)
        {
            hideTeacherSpeechText();
        } else if (response == WrongAnswerRightObject || response == WrongAnswerWrongObject)
        {
            setTeacherAnimation(NPCAnimations.AnimationIndex.DOUBTFUL);
            showTeacherSpeech(response.text);
        } else if (response == correctAnswer)
        {
            setTeacherAnimation(NPCAnimations.AnimationIndex.HAPPY);
            showTeacherSpeech(response.text);
        } else if (response.speaker == Dialogue.Speaker.NPC)
        {
            setTeacherAnimation(NPCAnimations.AnimationIndex.INSTRUCTING);
            showTeacherSpeech(response.text);
        }

        // Shows and plays the dialogue
        if (minigame != null && minigame.ContinueConversation(response) == false)
        {
            Debug.Log(response.text);
        }

        if (response.nextDialogue == null)
        {
            CancelInvoke("hideTeacherSpeechText");
            Invoke("hideTeacherSpeechText", minigame.GetCurrentDialogueDuration());
        } else if (response.nextDialogue.speaker == Dialogue.Speaker.SHERLOK)
        {
            CancelInvoke("hideTeacherSpeechText");
            Invoke("hideTeacherSpeechText", minigame.GetCurrentDialogueDuration());
            StartCoroutine(setSherlockResponse(minigame.GetCurrentDialogueDuration(), response.nextDialogue));
        } else if (response.nextDialogue.speaker == Dialogue.Speaker.NPC)
        {
            CancelInvoke("hideTeacherSpeechText");
            StartCoroutine(setTeacherResponse(minigame.GetCurrentDialogueDuration(), response.nextDialogue));
        }
    }

    IEnumerator setSherlockResponse(float waitTime, Dialogue response)
    {
        yield return new WaitForSeconds(waitTime);
        if (response != null)
        {
            if (allowDialogueCoroutine)
            {
                Sherlock.Instance.SetDialogue(response);
                forceHideTeacherSpeechText();
                Respond(response);
            } else
            {
                allowDialogueCoroutine = true;
            }
        }
    }

    IEnumerator setTeacherResponse(float waitTime, Dialogue response)
    {
        yield return new WaitForSeconds(waitTime);
        if (response != null)
        {
            if (allowDialogueCoroutine)
            {
                Respond(response);
            } else
            {
                allowDialogueCoroutine = true;
            }
        }
    }

    public void showTeacherSpeech(string text)
    {
        Sherlock.Instance.HideDialogue();
        teacherSpeechBubble.SetActive(true);
        teacherSpeechText.GetComponent<UILabel>().text = text;
        teacherSpeechText.SetActive(true);
    }

    public void hideTeacherSpeechText()
    {
        if (hideTeacherDialogueQueue.Count > 0)
        {
            if (currentDialogue == hideTeacherDialogueQueue.Peek())
            {
                teacherSpeechBubble.SetActive(false);
                teacherSpeechText.SetActive(false);
                setTeacherIdleAnimation();
            }
            hideTeacherDialogueQueue.Dequeue();
        } else
        {
            teacherSpeechBubble.SetActive(false);
            teacherSpeechText.SetActive(false);
            setTeacherIdleAnimation();
        }
    }

    public void forceHideTeacherSpeechText()
    {
        teacherSpeechBubble.SetActive(false);
        teacherSpeechText.SetActive(false);
        setTeacherIdleAnimation();
        hideTeacherDialogueQueue.Clear();
    }

    public void CompleteMinigame()
    {
        minigame.CompleteIfStandalone();
    }

    IEnumerator startTutorial()
    {
        //disable tapping on teacher
        teacherTapHolder.GetComponent<BoxCollider>().enabled = false;
        //set up tutorial hand, cow and ship.
        TutorialHand tutorialHandHelper = tutorialHand.GetComponent<TutorialHand>();
        tutorialCow.SetActive(true);
        tutorialMoon.SetActive(true);
        tutorialHand.SetActive(true);
        tutorialHandHelper.moveInterval = 1.0f;
        //"In this game, pay attention to the chalkboard..."
        Respond(tutorialLines [0]);
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
        Respond(tutorialLines [1]);
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
        Respond(tutorialLines [2]);
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());
        Respond(tutorialLines [3]);
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

        tutorialHandHelper.nextWayPoint();
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
        tutorialHandHelper.isPointing(true); //tap on teacher
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval/2);
        tutorialCow.transform.localPosition = new Vector3(-190, -130, 0); //cow moves to bottom

        Respond(tutorialLines [4]);
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

        tutorialHandHelper.isPointing(false);
        tutorialHandHelper.nextWayPoint();
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
        tutorialHandHelper.isPointing(true); //tap on cow
        tutorialCow.transform.parent = tutorialHand.transform;
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval/2);
 
        tutorialHandHelper.nextWayPoint(); //move cow to its original bovine position
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
        tutorialHandHelper.isPointing(false);
        tutorialCow.transform.parent = tutorialHand.transform.parent;

        Respond(tutorialLines [5]); //if you need help, tap the teacher
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

        tutorialHandHelper.nextWayPoint();
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval);
        tutorialHandHelper.isPointing(true); //tap on teacher
        yield return new WaitForSeconds(tutorialHandHelper.moveInterval/2);

        Respond(tutorialLines [6]); //let's start
        yield return new WaitForSeconds(minigame.GetCurrentDialogueDuration());

        tutorialCow.SetActive(false);
        tutorialMoon.SetActive(false);
        tutorialHand.SetActive(false);

        //enable tapping on teacher
        teacherTapHolder.GetComponent<BoxCollider>().enabled = true;
     
        Invoke("setupGame", 0.5f);
    }

}

