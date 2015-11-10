using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainManager : MonoBehaviour {

	//Number of objects that will appear a certain amount of time
	public int numCows = 1;
	public int numDucks = 1;
	public int numClouds = 1;
	public int numRainbows = 1;
	public int numHorses = 1;
	public int numGophers = 1;
	public int numWindmills = 1;
	public int numTrees = 1;
	public int numSheep = 1;
	public int numBarns = 1;

	//For calculating sky height
	public GameObject firstCloud; 

	//Prefabs for instantiation
	public GameObject cowPrefab;
	//public GameObject treasurePrefab;
	//public GameObject rockPrefab;
	public GameObject duckPrefab; 
	public GameObject cloudPrefab;
	public GameObject rainbowPrefab;
	public GameObject horsePrefab;
	public GameObject gopherPrefab;
	public GameObject windmillPrefab;
	public GameObject treePrefab;
	public GameObject sheepPrefab;
	public GameObject barnPrefab;

	//Parents for the prefabs
	public GameObject cowParent;
	public GameObject duckParent;
	public GameObject cloudParent;
	public GameObject rainbowParent;
	public GameObject horseParent;
	public GameObject gopherParent;
	public GameObject windmillParent;
	public GameObject treeParent;
	public GameObject sheepParent;
	public GameObject barnParent;

	//Area in which the objects will randomly appear
	//public BoxCollider spawnArea;
	//public Transform spawnArea;
	public BoxCollider spawnAreaDucks;
	public BoxCollider spawnAreaWindmill;
	
	public List<Transform> spawnAreas;
	List<float> spawnTimes;

	//bool IsOdd;
	//int calculate; 

	//How fast the objects will move down
	//public float verticalSpeedIncrease = 0.2f;

	[System.Serializable]
	public class TrainWagon{
		public string name;
		public List<Material> materials;
	}
	public List<TrainWagon> trainWagons;

	void Awake()
	{
		SelectedTrain selectedTrain = GameObject.FindObjectOfType (typeof(SelectedTrain)) as SelectedTrain;

		if (selectedTrain != null) {
			int wagonCount = 0;
			foreach(SelectedTrain.TrainColor trainColor in selectedTrain.trainWagons)
			{
				foreach (Material mat in trainWagons[wagonCount].materials)
					mat.color = trainColor.color;
				wagonCount++;
			}
			Destroy (selectedTrain.gameObject);
		}
		spawnTimes = new List<float>();
		for (int i = 0; i < spawnAreas.Count ; i++)
			spawnTimes.Add(Time.time);
	}

	// Use this for initialization
	public void startTrain () {
		//alarms for the object instantiation
		InvokeRepeating("InstantiateCows", 5f, 15f);
		InvokeRepeating("InstantiateDucks", 9f, 7f);
		InvokeRepeating("InstantiateClouds", 4f, 8f);
		InvokeRepeating("InstantiateRainbows", 7f, 25f);
		InvokeRepeating("InstantiateHorses", 2f, 36f);
		InvokeRepeating("InstantiateGophers", 3f, 6f);
		InvokeRepeating("InstantiateWindmills", 1f, 30f);
		InvokeRepeating("InstantiateTrees", 10f, 18f);
		InvokeRepeating("InstantiateSheep", 6f, 8f);
		InvokeRepeating("InstantiateBarns", 0f, 45f);
//		IsOdd = false;
//		calculate = 0; 
	}

	Transform selectSpawnArea(bool outerLine)
	{
		int count = 6;
		while (count > 0)
		{
			bool IsOdd;
			int calculate = Random.Range(0, 2); 
			
			if (calculate % 2 != 0) {IsOdd = true; }
			else {IsOdd = false; }
			
			if(outerLine)
			{
				if(IsOdd) {
					if (Time.time - spawnTimes[0] > 0.1f)
					{
						spawnTimes[0] = Time.time;
						return spawnAreas[0];
					}
				}
				else {
					if (Time.time - spawnTimes[5] > 0.1f)
					{
						spawnTimes[5] = Time.time;
						return spawnAreas[5];
					}
				}
			}
			
			else
			{
				if(IsOdd)	{calculate = Random.Range(0, 3);}
				else {calculate = Random.Range(3, 6);}
				
				if (Time.time - spawnTimes[calculate] > 0.1f)	
				{
					spawnTimes[calculate] = Time.time;
					return spawnAreas[calculate];
				}
			}
			count--;
		}
		return null;
		
	}

	//Instantiate cows inside the spawn area
	void InstantiateCows()
	{
		//Cows instantiation
		for(int i = 0; i < numCows; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnArea.transform.position;
			//GameObject go = AddChild(cowParent.gameObject, cowPrefab);
			GameObject go = AddChild(spawnArea.gameObject, cowPrefab);
			go.name = "Cow";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateDucks()
	{
		//Cows instantiation
		for(int i = 0; i < numDucks; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnAreaDucks.transform.position;
			//GameObject go = AddChild(duckParent.gameObject, duckPrefab);
			GameObject go = AddChild(spawnArea.gameObject, duckPrefab);
			go.name = "Duck";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate clouds inside the spawn area
	void InstantiateClouds()
	{
		//Cows instantiation
		for(int i = 0; i < numClouds; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnArea.transform.position;
			//GameObject go = AddChild(cloudParent.gameObject, cloudPrefab);
			GameObject go = AddChild(spawnArea.gameObject, cloudPrefab);
			go.name = "Cloud";
			go.transform.position = SkyGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate clouds inside the spawn area
	void InstantiateRainbows()
	{
		//Cows instantiation
		for(int i = 0; i < numRainbows; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnArea.transform.position;
			//GameObject go = AddChild(rainbowParent.gameObject, rainbowPrefab);
			GameObject go = AddChild(spawnArea.gameObject, rainbowPrefab);
			go.name = "Rainbow";
			go.transform.position = SkyGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateHorses()
	{
		//Cows instantiation
		for(int i = 0; i < numHorses; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnArea.transform.position;
			//GameObject go = AddChild(horseParent.gameObject, horsePrefab);
			GameObject go = AddChild(spawnArea.gameObject, horsePrefab);
			go.name = "Horse";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateGophers()
	{
		//Cows instantiation
		for(int i = 0; i < numGophers; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnAreaDucks.transform.position;
			//GameObject go = AddChild(gopherParent.gameObject, gopherPrefab);
			GameObject go = AddChild(spawnArea.gameObject, gopherPrefab);
			go.name = "Gopher";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateWindmills()
	{
		//Cows instantiation
		for(int i = 0; i < numWindmills; i++)
		{
			Transform spawnArea = selectSpawnArea(true);
			if (spawnArea == null) return;
			Vector3 center = spawnAreaWindmill.transform.position;
			//GameObject go = AddChild(windmillParent.gameObject, windmillPrefab);
			GameObject go = AddChild(spawnArea.gameObject, windmillPrefab);
			go.name = "Windmill";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateTrees()
	{
		//Cows instantiation
		for(int i = 0; i < numTrees; i++)
		{
			Transform spawnArea = selectSpawnArea(true);
			if (spawnArea == null) return;
			Vector3 center = spawnArea.transform.position;
			//GameObject go = AddChild(treeParent.gameObject, treePrefab);
			GameObject go = AddChild(spawnArea.gameObject, treePrefab);
			go.name = "Tree";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateSheep()
	{
		//Cows instantiation
		for(int i = 0; i < numSheep; i++)
		{
			Transform spawnArea = selectSpawnArea(false);
			if (spawnArea == null) return;
			Vector3 center = spawnAreaDucks.transform.position;
			//GameObject go = AddChild(sheepParent.gameObject, sheepPrefab);
			GameObject go = AddChild(spawnArea.gameObject, sheepPrefab);
			go.name = "Sheep";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Instantiate ducks inside the spawn area
	void InstantiateBarns()
	{
		//Cows instantiation
		for(int i = 0; i < numBarns; i++)
		{
			Transform spawnArea = selectSpawnArea(true);
			if (spawnArea == null) return;
			Vector3 center = spawnArea.transform.position;
			//GameObject go = AddChild(barnParent.gameObject, barnPrefab);
			GameObject go = AddChild(spawnArea.gameObject, barnPrefab);
			go.name = "Barn";
			go.transform.position = CalculateGameObjectNewPosition(go.transform.position, spawnArea);
			
		}
	}

	//Different calculation for sky objects
	Vector3 SkyGameObjectNewPosition(Vector3 newPos, Transform spawnZone)
	{
		Vector3 center = spawnZone.transform.position;
		Vector3 height = firstCloud.transform.position; 
		//newPos.x = Random.Range (center.x - 1.5f, center.x + 1.5f); 
		newPos.x = center.x;
		newPos.y = height.y;  
		newPos.z = center.z;

		
		return newPos;
	}

	//Random position inside a spawn area (Inside rectangle area)
	Vector3 CalculateGameObjectNewPosition(Vector3 newPos, Transform spawnZone)
	{
		Vector3 center = spawnZone.transform.position;
		newPos = center;
		
		
//		Vector3 center = spawnZone.transform.position;
//		calculate = Random.Range(0, 2);  
//		if (calculate % 2 != 0)
//		{
//			IsOdd = true; 
//		}
//		else
//		{
//			IsOdd = false; 
//		}
//		//do
//		//{
//			/*if(isRock)
//				newPos.x = ship.transform.position.x;
//			else*/
//		//newPos.x = Random.Range((center.x - 0.1f) - 0.5f, (center.x + 0.1f) + 0.5f); 
//		if (IsOdd)
//		{
//			newPos.x = Random.Range(center.x - 1.5f, center.x - 0.15f); 
//		}
//		else
//		{
//			newPos.x = Random.Range(center.x + 0.15f, center.x + 1.5f);
//		}
//		//newPos.x = Random.Range(center.x - 0.5f, center.x - 0.1f); 
//		//newPos.x = Random.Range(center.x + 0.1f, center.x + 0.5f);
//		//newPos.x = Random.Range(center.x - ((spawnZone.size.x)/2), center.x + ((spawnZone.size.x)/2));
//			
//			newPos.y = center.y;  //Random.Range(center.y - ((spawnZone.size.y)/2), center.y + ((spawnZone.size.y)/2));
//			newPos.z = center.z ;
//		/*if (newPos.x < 0.3 && newPos.x > -0.3)
//		{
//		Debug.Log (newPos.x); 
//		}*/
//		//}while(Physics.CheckSphere(newPos, 0.1f));	
		
		return newPos;
	}

	//child newly created object to an object
	public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		
		if (go != null && parent != null)
		{
			go.layer = parent.layer;
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = prefab.transform.localScale;
			
		}
		
		return go;
	}
}
