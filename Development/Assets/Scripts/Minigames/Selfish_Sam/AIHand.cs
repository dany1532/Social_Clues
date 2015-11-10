using UnityEngine;
using System.Collections;

public class AIHand : MonoBehaviour {
	public enum TypeHand
	{
		TREASURE,
		ENEMY,
		WHEEL
	}
	
	public enum CharacterHand
	{
		LUIS,
		SAM
	}
	
	public enum HandAIState
	{
		FOLLOWING,
		PRESSING,
		RELEASING,
		NONE
	}
	
	public TypeHand myHandType;
	public CharacterHand characterHand;
	
	public Texture selectHandTexture;
	public Texture unselectHandTexture;
	public Texture luisHand;
	public Texture samHand;
	
	HandAIState handAIState;
	Pirate_Treasure treasureToFollow;
	Enemy_Minigame enemyToFire;
	
	bool reachedDestination = false;
	bool enabled = false;
	
	public Pirate_Ship ship;
	public Selfish_Sam_Manager manager;
	public float handSpeed = 1.0f;
	
	void Start()
	{
		//myHandType = TypeHand.TREASURE;
		
		handAIState = HandAIState.NONE;
	}
	
	public void SetLuisHand()
	{
		selectHandTexture = luisHand;
		GetComponent<UITexture>().pivot = UIWidget.Pivot.Left;
		characterHand = CharacterHand.LUIS;
	}
	
	public void SetSamHand()
	{
		selectHandTexture = samHand;
		GetComponent<UITexture>().pivot = UIWidget.Pivot.Right;
		characterHand = CharacterHand.SAM;
	}
	
	public void FollowTreasure(Pirate_Treasure treasure)
	{
		if(treasureToFollow == null){
			treasureToFollow = treasure;
			handAIState = HandAIState.FOLLOWING;
		}
		
	}
	
	public void FollowEnemy(Enemy_Minigame enemy)
	{
		
		if(handAIState == HandAIState.NONE)
		{
			
			enemyToFire = enemy;
			handAIState = HandAIState.FOLLOWING;
		}
	}
	
	public void UnselectTreasure()
	{
		GetComponent<UITexture>().mainTexture = unselectHandTexture;
		GetComponent<UITexture>().enabled = false;
		handAIState = HandAIState.NONE;
		reachedDestination = false;
		treasureToFollow = null;
	}
	
	public void UnpressEnemy()
	{
		GetComponent<UITexture>().enabled = false;
		GetComponent<UITexture>().mainTexture = unselectHandTexture;
		reachedDestination = false;
		enemyToFire = null;
		handAIState = HandAIState.NONE;
	}
	
	public void Reset()
	{
		enabled = false;
		UnselectTreasure();
		UnpressEnemy();
		GetComponent<UITexture>().enabled = false;
	}
	
	public void EnableAI(){
		enabled = true;
	}
	
	public void ChangePressedHandTexture(){
		GetComponent<UITexture>().enabled = true;
		GetComponent<UITexture>().mainTexture = selectHandTexture;
		
		if(characterHand == CharacterHand.LUIS){
			GetComponent<UITexture>().pivot = UIWidget.Pivot.Left;
		}
		else{
			GetComponent<UITexture>().pivot = UIWidget.Pivot.Right;
		}
	}
	
	public void ChangeUnselectHandTexture(){
		GetComponent<UITexture>().enabled = false;
		GetComponent<UITexture>().mainTexture = unselectHandTexture;
		GetComponent<UITexture>().pivot = UIWidget.Pivot.Center;
	}
	
	void Update()
	{
		if(enabled && manager.activateTreasureAI)
			TreasureAIUpdate();
		
		if(enabled && manager.activateShootingAI)
			EnemyAIUpdate();
		
	}
	
	void EnemyAIUpdate()
	{
		if(myHandType == TypeHand.ENEMY)
		{
			//if(!GetComponent<UITexture>().enabled) { GetComponent<UITexture>().enabled = true; }
			
			if(handAIState == HandAIState.FOLLOWING)
			{
				float dist = Vector2.Distance ((Vector2) ship.transform.position, 
								(Vector2)enemyToFire.transform.position);
				
				Vector3 enemyPos = enemyToFire.transform.position;
				if(dist > 0.75f)
					enemyPos.y -= 0.25f;
				
				Vector3 dir = enemyPos - transform.position;
				dir.Normalize();
				transform.Translate(new Vector3(dir.x, dir.y, 0) * handSpeed * Time.deltaTime);
				
				if(!reachedDestination && Vector2.Distance ((Vector2) transform.position, (Vector2)enemyPos) < 0.1f)
				{
					reachedDestination = true;
					
					
					if(dist < 0.75f)
					{
						ship.ShootAtPosition(enemyToFire.transform.position);
					}
					
					else{
						ship.ShootAtPosition(enemyPos);
					}
					
					handAIState = HandAIState.PRESSING;
					//treasureToFollow.TreasurePressed(true);
					//if(!GetComponent<UITexture>().enabled) { GetComponent<UITexture>().enabled = true; }
					GetComponent<UITexture>().enabled = true;
					GetComponent<UITexture>().mainTexture = selectHandTexture;
					Invoke("UnpressEnemy", 0.5f);
				}
			}
		}
	}
	
	void TreasureAIUpdate()
	{
		if(myHandType == TypeHand.TREASURE)
		{
			//if(!GetComponent<UITexture>().enabled) { GetComponent<UITexture>().enabled = true; }
			
			if(handAIState == HandAIState.FOLLOWING)
			{
				Vector3 dir = treasureToFollow.transform.position - transform.position;
				dir.Normalize();
				transform.Translate(new Vector3(dir.x, dir.y, 0) * handSpeed * Time.deltaTime);
				
				if(!reachedDestination &&Vector2.Distance
					((Vector2) transform.position, (Vector2)treasureToFollow.transform.position) < 0.1f)
				{
					reachedDestination = true;
					treasureToFollow.TreasurePressed(true);
					GetComponent<UITexture>().enabled = true;
					GetComponent<UITexture>().mainTexture = selectHandTexture;
				}
			}
		}//End Treasure AI
	}
}
