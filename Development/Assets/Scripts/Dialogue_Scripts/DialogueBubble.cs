using UnityEngine;
using System.Collections;

public class DialogueBubble : MonoBehaviour {
	
	
//	public Color amyColor;
//	public Color maxColor;
	public Color playerColor;
	public Color neutral1Color;
	public Color neutral2Color;
	
	public bool isActiveColor = false;
	public bool isActiveLocLerp = false;
	public float duration = 2.5f;
	public UITexture background;
	
	//Dialogue.DialogueLocation currentLoc;
	Color finalColor;
	Color initialColor;
	float playerLocUV = 0f;
	float npc1LocUV = -0.35f;
	float npc2LocUV = -0.55f;
	float neutralLocUV = 0.2f;
	float finalLoc;
	float tColor;
	float tLoc;
	
	
	// Update is called once per frame
	void Update () {
		if(isActiveColor)
			ApplyColorChangeAnim(Time.deltaTime);
		
		if(isActiveLocLerp)
			ApplyLocLerpAnim(Time.deltaTime);
	}

	//Clears the dialogue bubble
	public void ClearTexture(){
		Rect newUVRect = background.uvRect;
		newUVRect.x = neutralLocUV;
		background.uvRect = newUVRect;
		
		background.color = neutral2Color;
		
		background.enabled = false;	
	}
	
	/// <summary>
	/// Sets the dialogue bubble, depending on speaker
	/// </summary>
	/// <param name='dialogue'>
	/// Dialogue: to check who is speaking
	/// </param>/
	public void SetDialogueBubble(Dialogue dialogue){
		background.enabled = true;
		
		//if the speaker is the player
		if(dialogue.dialogueBubbleType == Dialogue.DialogueLocation.PLAYER){
			finalLoc = playerLocUV;
			finalColor = playerColor;
			
			PlayLocLerpAnimation();
		}
		
		//if the speaker is the npc1
		else if(dialogue.dialogueBubbleType == Dialogue.DialogueLocation.NPC1){
			finalLoc = npc1LocUV + dialogue.dialogueBubbleOffset;
			finalColor = dialogue.owner.npc1Color;
			
			PlayLocLerpAnimation();
		}
		
		//if the spekaer is the npc2
		else if(dialogue.dialogueBubbleType == Dialogue.DialogueLocation.NPC2){
			finalLoc = npc2LocUV + dialogue.dialogueBubbleOffset;
			finalColor = dialogue.owner.npc2Color;
			//finalColor = npc2Color;
			
			PlayLocLerpAnimation();	
		}
     
         //if the spekaer is the npc3
         else if(dialogue.dialogueBubbleType == Dialogue.DialogueLocation.NPC3){
             finalLoc = npc2LocUV + dialogue.dialogueBubbleOffset;
             finalColor = dialogue.owner.npc3Color;
             //finalColor = npc2Color;
             
             PlayLocLerpAnimation(); 
         }
		
		//no speaker, use neutral dialogue bubble
		else if(dialogue.dialogueBubbleType == Dialogue.DialogueLocation.NEUTRAL1){
			finalColor = neutral1Color;
			
			Rect newUVRect = background.uvRect;
			newUVRect.x = neutralLocUV;
			background.uvRect = newUVRect;		
		}
		
		//no speaker, use neutral dialogue bubble
		else if(dialogue.dialogueBubbleType == Dialogue.DialogueLocation.NEUTRAL2){
			finalColor = neutral2Color;
			
			Rect newUVRect = background.uvRect;
			newUVRect.x = neutralLocUV;
			background.uvRect = newUVRect;		
		}
		
		//finalColor = dialogue.dialogueColor;
		
		PlayColorAnimation();
	}
	
	//Play the location lerp animaion
	public void PlayLocLerpAnimation(){
		isActiveLocLerp = true;
	}
	
	//Change color animation
	public void PlayColorAnimation(){
		isActiveColor = true;
	}
	
	//apply the location lerp animation
	void ApplyLocLerpAnim(float delta){
		tLoc += delta/duration;
		tLoc = Mathf.Clamp(tLoc, 0, 1);
		
		Rect newUVRect = background.uvRect;
		
		newUVRect.x = Mathf.Lerp(background.uvRect.x, finalLoc, tLoc);
		
		background.uvRect = newUVRect;
	}
	
	//apply the color change animation
	void ApplyColorChangeAnim(float delta){
		
		Color color = background.color;
		
		tColor += delta/duration;
		tColor = Mathf.Clamp(tColor, 0, 1);
		
		//scSprite.color = NGUIMath.SpringLerp(scSprite.color, colorChange_Anim.to, 
													//colorChange_Anim.strength, delta);
		
		background.color = Color.Lerp(color, finalColor, tColor);
	
		if(color == finalColor){
			isActiveColor = false;
			isActiveLocLerp = false;
			tColor = 0f;
			tLoc = 0f;
		}
		
	}
}
