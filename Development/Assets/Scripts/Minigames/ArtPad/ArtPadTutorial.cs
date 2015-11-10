using UnityEngine;
using System.Collections;

public class ArtPadTutorial : MinigameManager {

	AudioManager manager;
	public AudioClip[] tutorial;
	/* 0: This is your coloring book.
	 * 1: The crayons, markers and pencils you find in story mode can be used here to draw!
	 * 2: To draw, tap on the tool you want to use.
	 * 3: Then choose a color to draw with.
	 * 4: You can collect more tools by playing Story Mode!
	 * 5: Once you've selected what you want to draw with, you can use it to draw on the white page with your finger.
	 * 6: There are other tools at the top of your screen that you can use to erase...
	 * 7: Undo...
	 * 8: and Redo your actions.
	 * 9: You can also tap "Save" to save your pictures to your iPad.
	 * 10: To start over, tap on the "New Page" button.
	 * 11: Have fun!
	 */

	TutorialHand hand;
	public GameObject crayonmenu, markermenu, redCrayon, tutorialOptions, PaintUI;
	public UISprite tutorialObject;
	Drawing canvas;

	// Use this for initialization
	void Start () 
	{
		canvas = GameObject.Find("Canvas").GetComponent<Drawing>();
		manager = gameObject.GetComponent<AudioManager>();
		hand = gameObject.GetComponentInChildren<TutorialHand>();
		canvas.canDraw = false;
		foreach(BoxCollider button in PaintUI.GetComponentsInChildren<BoxCollider>())
		{
			if(button.gameObject.transform.parent.name != "MarkerMenu" && button.gameObject.name != "Markers")
				button.enabled = false;
		}

	}

	public override void yesTutorial()
	{
		Destroy(tutorialOptions);
		StartCoroutine("runTutorial");
	}
	
	public override void noTutorial()
	{
		Destroy(tutorialOptions);
		canvas.canDraw = true;
		foreach(BoxCollider button in PaintUI.GetComponentsInChildren<BoxCollider>())
		{
			button.enabled = true;
		}
	}

	IEnumerator runTutorial()
	{
		//TO DO:
		 /* disable player drawing during tutorial <- under Camera/UICamera, by changing event receiver mask you can turn off buttons (not canvas)
		 * ^- under TopLeft/Canvas/Drawing, by turning off this script you can prevent drawing
		 * tutorial hand draws */
		
		//disable player drawing during tutorial
		canvas.canDraw = false;
		//disable button press during tutorial
		GameObject.Find("Camera").GetComponent<UICamera>().eventReceiverMask =  (1 << LayerMask.NameToLayer("")); 
	

		manager.Play(tutorial[0],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[0].length);

		manager.Play(tutorial[1],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[1].length);

		manager.Play(tutorial[2],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[2].length);

		//tutorial hand appears and moves to crayon menu. hand taps on crayon.
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);//hand taps on crayon.



		//show crayon menu
		//hide marker menu
		markermenu.SetActive(false);
		crayonmenu.SetActive(true);
		//change tool
	 	canvas.changeTool(Drawing.ToolType.Crayon);

		manager.Play(tutorial[3],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[3].length);

		//hand moves to red crayon and taps it.
		hand.isPointing(false);
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);

		//change tool to red crayon
		canvas.setColor(redCrayon.GetComponent<UIButtonArt>().myColor);
		if(canvas.currentToolInstance != null && canvas.currentToolInstance.name != "Eraser")
			canvas.currentToolInstance.position = canvas.currentToolInstance.position - new Vector3(0.0f,0.1f, 0.0f);
		redCrayon.transform.position = redCrayon.transform.position + new Vector3(0.0f,0.1f, 0.0f);;
		canvas.currentToolInstance = redCrayon.transform;

		manager.Play(tutorial[4],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[4].length);

		manager.Play(tutorial[5],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[5].length);

		//hand moves to canvas, taps, and drags, drawing a shape
		hand.isPointing(false);
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);

		tutorialObject.fillAmount = 0;
		tutorialObject.gameObject.SetActive(true);

		hand.nextWayPoint(); 
		tutorialObject.fillAmount = .08f;
		for(int i = 0; i<100; i++)
		{
			tutorialObject.fillAmount += .22f/100.0f;
			yield return new WaitForSeconds(hand.moveInterval/100.0f);
		}

		hand.nextWayPoint(); 
		for(int i = 0; i<100; i++)
		{
			yield return new WaitForSeconds(hand.moveInterval/100.0f);
			tutorialObject.fillAmount += .26f/100.0f;

		}

		hand.nextWayPoint(); 
		for(int i = 0; i<100; i++)
		{
			yield return new WaitForSeconds(hand.moveInterval/100.0f);
			tutorialObject.fillAmount += .263f/100.0f;
		}
		//82.3

		hand.nextWayPoint(); 
		for(int i = 0; i<100; i++)
		{
			yield return new WaitForSeconds(hand.moveInterval/100.0f);
			tutorialObject.fillAmount += .177f/100.0f;
		}
		hand.isPointing(false);

		manager.Play(tutorial[6],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[6].length);

		//hand moves to eraser, taps on it, then moves to canvas and erases some of the shape

		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);

		yield return new WaitForSeconds(hand.moveInterval); //hand stays tapped on eraser for a small amount of time
		hand.isPointing(false);
		hand.nextWayPoint(); //moving back to the canvas
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);

		tutorialObject.invert = true;

		hand.nextWayPoint(); 
		tutorialObject.fillAmount = .92f;
		for(int i = 0; i<100; i++)
		{
			tutorialObject.fillAmount -= .142f/100.0f;
			yield return new WaitForSeconds(hand.moveInterval/100.0f);
		}
		hand.isPointing(false);

		manager.Play(tutorial[7],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[7].length);

		//hand moves to undo button, taps it. the erased part of the circle comes back
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);
		tutorialObject.fillAmount = 1.0f;
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing (false);

		manager.Play(tutorial[8],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[8].length);

		//hand moves to redo button, taps it. the erased part of the circle goes away again
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);
		tutorialObject.fillAmount = .778f;
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing (false);

		manager.Play(tutorial[9],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[9].length);

		//hand hovers over save
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);

		manager.Play(tutorial[10],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[10].length);

		//hand moves to new page, taps it, the canvas is cleared.
		hand.nextWayPoint(); 
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing(true);
		tutorialObject.gameObject.SetActive(false);
		yield return new WaitForSeconds(hand.moveInterval);
		hand.isPointing (false);

		manager.Play(tutorial[11],transform, 1.0f, false);
		yield return new WaitForSeconds(tutorial[11].length);

		//hand disappears
		hand.gameObject.SetActive(false);

		//give back player control over buttons and drawing
		GameObject.Find("Camera").GetComponent<UICamera>().eventReceiverMask =  (1 << LayerMask.NameToLayer("GUI")); 
		canvas.canDraw = true;
		foreach(BoxCollider button in PaintUI.GetComponentsInChildren<BoxCollider>())
		{
			button.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
