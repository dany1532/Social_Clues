using UnityEngine;
using System.Collections;

public class ScoreBoard : IgnoreTimeScale 
{
	
	int timeScore;
	int scoreEnemies;
	int scoreTreasures;
	int score;
	string min;
	string sec;
	float prevTime;
	int round = 0;
	float currTime = 0;
	float duration = 0;
	bool updateTime = false;
	float scoreTime_offset = 0.4f;
	
	//Score board
	public UILabel scoreBoard_time;
	public UILabel scoreBoard_enemy;
	public UILabel scoreBoard_treasure;
	public UILabel scoreBoard_score;
	public TweenScale scoreboard_scaler;
	//public ScaleAnimationFX scoreBoard_scaler;
	
	void Start(){
		transform.localScale = Vector3.zero;
	}
	
	public void SetScoreBoard(int treasures, int enemies, int time, int totalScore)
	{
		timeScore = time;
		scoreEnemies = enemies;
		scoreTreasures = treasures;
		score = totalScore;
		
		
		DisplayScore1();
		//StartCoroutine("DisplayScoreBoard");
		//Invoke("DisplayScore1", 0);
	}
	
	void DisplayScore1()
	{
		float minutes = Mathf.Floor(timeScore / 60);
		float seconds= Mathf.RoundToInt(timeScore % 60);
 
		if(minutes < 10) {
    		min = "0" + minutes.ToString();
		}
		else{
			min = minutes.ToString();
		}
		
		if(seconds < 10) {
    		sec = "0" + Mathf.RoundToInt(seconds).ToString();
		}
		else{
			sec = seconds.ToString();
		}
		
		duration = 1.2f;
		currTime = 0;
		updateTime = true;
		//Invoke("DisplayScore2", 1.2f);
	}
	
	void DisplayScore2()
	{
		scoreBoard_enemy.text = "";
		scoreBoard_treasure.text = "";
		scoreBoard_time.text = "";
		scoreBoard_score.text = "";
		
		scoreboard_scaler.enabled = true;
//		scoreBoard_scaler.IntializeScaleLerp(Vector3.zero, scoreBoard_scaler.transform.localScale);
//		scoreBoard_scaler.PlayAnimation();
		
		duration = 1.2f;
		currTime = 0;
		updateTime = true;

		
		//Invoke("DisplayTime", 0.2f);
	}
	
	void DisplayTime()
	{
		scoreBoard_time.text = "" + min + ":" + sec;
		duration = scoreTime_offset;
		currTime = 0;
		updateTime = true;
		
		//Invoke("DisplayEnemies", 0.2f);
	}
	
		void DisplayEnemies()
	{
		scoreBoard_enemy.text = scoreEnemies.ToString();
		duration = scoreTime_offset;
		currTime = 0;
		updateTime = true;
		
		//DisplayTreasures();
		//Invoke("DisplayTreasures", 0.2f);
	}
	
	
	void DisplayTreasures()
	{
		scoreBoard_treasure.text = scoreTreasures.ToString();
		duration = scoreTime_offset;
		currTime = 0;
		updateTime = true;
		
		//DisplayScore();
		//Invoke("DisplayScore", 0.2f);
	}
	

	void DisplayScore()
	{
		scoreBoard_score.text = score.ToString();
		
	}
	
//	IEnumerator DisplayScoreBoard()
//	{
//		float minutes = Mathf.Floor(timeScore / 60);
//		float seconds= Mathf.RoundToInt(timeScore % 60);
//		string min;
//		string sec;
// 
//		if(minutes < 10) {
//    		min = "0" + minutes.ToString();
//		}
//		else{
//			min = minutes.ToString();
//		}
//		
//		if(seconds < 10) {
//    		sec = "0" + Mathf.RoundToInt(seconds).ToString();
//		}
//		else{
//			sec = seconds.ToString();
//		}
//		
//
//		yield return new WaitForSeconds(1.5f);
//
//		scoreBoard_enemy.text = "";
//		scoreBoard_treasure.text = "";
//		scoreBoard_time.text = "";
//		scoreBoard_score.text = "";
//		scoreBoard_scaler.IntializeScaleLerp(Vector3.zero, scoreBoard_scaler.transform.localScale);
//		scoreBoard_scaler.PlayAnimation();
//		
//		yield return new WaitForSeconds(1.2f);
//		
//		scoreBoard_time.text = "" + min + ":" + sec;
//		
//		yield return new WaitForSeconds(0.2f);
//		
//		scoreBoard_enemy.text = scoreEnemies.ToString();
//		
//		yield return new WaitForSeconds(0.2f);
//		
//		scoreBoard_treasure.text = scoreTreasures.ToString();
//		
//		yield return new WaitForSeconds(0.2f);
//		
//		scoreBoard_score.text = score.ToString();
//		
//	}
	
	void LateUpdate()
	{	
		if(updateTime)
		{
			currTime += UpdateRealTimeDelta();
			
			if(currTime >= duration){
				round++;
				updateTime = false;
				
				if(round == 1){
					DisplayScore2();
				}
				
				else if(round == 2){
					DisplayTime();
				}
				
				else if(round == 3)
					DisplayEnemies();
				
				else if(round == 4)
					DisplayTreasures();
				
				else if(round == 5)
					DisplayScore();
				
			}
		}

	}
}
