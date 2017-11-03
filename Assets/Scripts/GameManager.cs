using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;
//using UnityEditor;
using System.Xml.Linq;
//using System.Security.Cryptography;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public bool panning = false;

	public int score;
	private int[] highscores;
	public int attemptCount;
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI highScoreText;
	public List<GameObject> Fish = new List<GameObject>();
	public float waveSpeed = 2f;
	public int fishCloseToPuffin = 0;
	public float  puffinToMouthRange = 3f;
	private Scene currentScene;
	//private int numberofLevels = 5;
	public int currentHighScore;
	public int currentPuffletsHatched;

	public Slider airMeter;
	public GameObject slider;
	[HideInInspector]
	public saveData data1 = new saveData(1000); //prepopulate the list to 1000 so there is potential to grow

	public int currentLevelIndex;
	public int previousLevelIndex;

	private bool gameInitialized = false; //this ensures that onLeveLFinished Loading isn't called when the game first starts


	public enum GameState
	{
		start,
		aiming,
		diving,
		panning,
		gameWin,
		gameOver,
	}

	public GameState gameState = GameState.diving;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}


	void Awake()
	{
		//print ("Awake Called");
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);



	}

	// Use this for initialization
	void Start () 
	{
		//print ("Start Called");
		airMeter = slider.GetComponent<Slider> ();



		if (data1 != null)
		{
			//loadFile (data1);
		} else
		{
			/*
			//data1.level.Add( new Level());
			//data.level [0].scene = currentScene;
			data1.level [0].worldNumber = 420000;
			data1.number = 25.35f;
			data1.scores = new float[numberofLevels];
			data1.level [currentScene.buildIndex].levelHighScore = 0 ;*/

		}


		//this loads the save data and populates the list of levels.
		data1 = loadFile (data1); //this is already checking if the file exists.
		print(data1.level.Count);
		/* if (data1.level.Count == LevelManager.instance.Levels.Count ) //this is for when Levels was an array if (data1.level.Count == LevelManager.instance.Levels.Length )
		{
			print ("There are no new levels");

		}
		else if (data1.level.Count < LevelManager.instance.Levels.Count)//this is for when Levels was an array if (data1.level.Count < LevelManager.instance.Levels.Length )
		{
			print ("Initializing LeveLs");
			//this should be polished, so only levels that aren't already in the list are added

			foreach (string scene in LevelManager.instance.Levels)//UnityEngine.Object
			{
				data1.level.Add( new Level(scene));

			}


		} */

		gameInitialized = true;
		print ("First attempt to get level index");
		currentLevelIndex = getLevelIndex (data1);
		print ("Level index is " + currentLevelIndex);
		//currentLevelIndex	= data1.level.FindIndex (x => x.levelName == currentScene.name); //duplicate to on LevelFinished Loading

	}


	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		currentScene = scene;

		if (gameInitialized) //these lines only happen after the first time the game is run
		{
			previousLevelIndex = currentLevelIndex;
			print ("Attempting to get level index");
			currentLevelIndex = getLevelIndex (data1); // I uyse a try catch
			scoreText.text = "";

			Fish.Clear ();
			if (currentScene.name == "puffinNestattempt")
			{
				highScoreText.text = "Highscore : " + data1.level[previousLevelIndex].levelHighScore; 
				currentHighScore = data1.level[previousLevelIndex].levelHighScore;
			} else
			{
				highScoreText.text = "Highscore : " + data1.level[currentLevelIndex].levelHighScore;
				currentHighScore = data1.level[currentLevelIndex].levelHighScore;
			}


			print ("Current HighScore" + currentHighScore);
			Debug.Log ("Level Loaded." + " Level List index is " + currentLevelIndex + " . Build index is " + currentScene.buildIndex + " . level count is " + data1.level.Count);

			/*currentScene = scene;
			currentLevelIndex	= data1.level.FindIndex (x => x.levelName == currentScene.name);
			print (currentScene.name);
			Debug.Log ("Level Loaded." + " Level List index is " + currentLevelIndex + " . Build index is " + currentScene.buildIndex + " . level count is " + data1.level.Count);
			*/
			/*Debug.Log(scene.name);
		Debug.Log (scene.buildIndex);
		Debug.Log(mode);*/


			if (ClickManager.instance.puffin == null)
			{
				print ("No puffin Reference in clickmanager");
				ClickManager.instance.puffin = GameObject.Find ("Puffin");
				if (ClickManager.instance.puffin != null)
				{
					ClickManager.instance.rb2d = ClickManager.instance.puffin.GetComponent<Rigidbody2D> ();
				}
				print ("attempted to find the puffin again");
			}
		}
	
	}

	public int getLevelIndex(saveData data) // Use a try catch block to check if the level exists in the list, if not, add it to the list and increase the initialized level count
	{
		currentLevelIndex	= data.level.FindIndex (x => x.levelName == currentScene.name);
		if (currentLevelIndex<0)
		{
			print ("Adding " + currentScene.name + " to the save data Level list.");
			data.level [data.initializedLevelCount].levelName = currentScene.name;
			currentLevelIndex = data.initializedLevelCount;
			data.initializedLevelCount++;
		}
		return currentLevelIndex;
		/* try
		{

			currentLevelIndex	= data.level.FindIndex (x => x.levelName == currentScene.name);
			print("Try code suceeded");
		}
		catch
		{
			print ("Adding " + currentScene.name + " to the save data Level list.");
			data.level [data.initializedLevelCount].levelName = currentScene.name;
			currentLevelIndex = data.initializedLevelCount;
			data.initializedLevelCount++;
		}
		return currentLevelIndex;*/
	}

	public void LevelEnded() // I call this when a level ends (after exit the nest)
	{
		print ("level ended");

		saveFile (data1);
		score = 0;//reset the score for the level.
		currentPuffletsHatched = 0; // reset the number of pufflets hatched.
		Destroy(LevelConstraints.instance.gameObject);//destroy level constraint

	}

	public void levelToNest()//called when puffin enters nest portal
	{
		saveFile(data1);//save the data when the puffin goes to the nest so the highscore is displayed
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.S))
		{
			print ("Pressed S");
			saveFile (data1);

		}
		
		if (Input.GetKeyDown (KeyCode.L))
		{
			print ("Pressed L");
			print(currentScene.name);
			data1 = loadFile (data1);
			//int currentLevelIndex = 0;//data1.level.FindIndex (c => c.levelName.Equals (currentScene.ToString ()));
			 

			//currentLevelIndex	=  data1.level.FindIndex (x => x.levelName == currentScene.name);
			//currentLevelIndex	=  data1.level.FindIndex (x => x.levelName.Equals(currentScene.name, StringComparison.Ordinal));

			//var ind = data1.level.Find(

			print("Level List index is " + currentLevelIndex + " . Build index is " +currentScene.buildIndex + " . level count is " + data1.level.Count);
			highScoreText.text = "Highscore : " + data1.level[currentLevelIndex].levelHighScore;
		}

		/*if (score == Fish.Count )
		{
			scoreText.text = score + " Eels collected!  You Win!";
			gameState = GameState.gameWin;
		}*/

		if (Input.touchCount == 5 && gameState == GameState.gameWin)
		{
			RestartLevel();
		}


	}

	public void saveFile(saveData data)
	{
		
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + data.ToString()+".dat");
		print ("This was saved to "+ Application.persistentDataPath + data.ToString () + ".dat");
		/*
		 * This area was to test the save data, what now should happen is the data is already in existance, and this just saves it.
		 * 
		 */
		//data.scores[0] = 1.1111f;
		/*data.level.Add( new Level());
		//data.level [0].scene = currentScene;
		data.level [0].worldNumber = 420000;
		data.number = 25.35f;
		data.scores = new float[numberofLevels];
		data.scores [currentScene.buildIndex] = score;
		*/
		int index;
		if (currentScene.name == "puffinNestattempt")
		{
			index = previousLevelIndex;
		} else
		{
			index = currentLevelIndex;
		}
		if (score > data.level [index].levelHighScore)
		{
			data.level [index].levelHighScore = score;
		}
		if (currentPuffletsHatched > data.level [index].puffletsHatched)
		{
			data.level [index].puffletsHatched = currentPuffletsHatched;
		}

		/*
		 * here I create an instance of class and give it the data to store
		 * saveData data = new saveData();
			data.health = health;
			data.experience = experience;

		 
		 */
	/*	if (data.level [currentLevelIndex].levelHighScore < score)
		{
			data.level [currentLevelIndex].levelHighScore = score;
		}*/
		bf.Serialize (file, data);
		file.Close ();


	}

	public saveData loadFile(saveData data)
	{
	
		if (File.Exists (Application.persistentDataPath + data.ToString () + ".dat"))
		{
			//try{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + data.ToString () + ".dat", FileMode.Open);  //persistant data path that Unity makes// playerInfo is specific file //filemode
			data = (saveData)bf.Deserialize (file); //cast this to the object we need. It is stored as a generic object
			file.Close ();

			print (data.name + "'s file was loaded");
			print ("Save file located at" + Application.persistentDataPath + data.ToString () + ".dat");


			return data;
	

			/*print(data.number);
			print (data.scores [currentScene.buildIndex]);
			scoreText.text = data.scores[currentScene.buildIndex] + " Eels collected!";
			print(data.level[0].worldNumber);
			//print(data.level [0].scene.name);*/

			/*
			 * this is where you give your script the data that was just loaded
			health = data.health;
			experience = data.experience;
			*/
			//}
			/*catch(ArgumentOutOfRangeException r)
			{


			}*/
		} 
		else
		{
			print ("File Doesn't exist, so it can't be loaded");
			return data;
		}

	}


	public void LoadLevel(int sceneIndex)
	{
		SceneManager.LoadScene (sceneIndex);
	}

	public void LoadLevel(string sceneName)
	{
		SceneManager.LoadScene(sceneName); 
	}

	public void RestartLevel ()
	{
		Scene scenceLoaded = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (scenceLoaded.buildIndex);

	}


	public void DeadZoneSwitch()
	{
		Camera.main.GetComponent<DeadzoneCamera>().enabled = Camera.main.GetComponent<DeadzoneCamera>().enabled == true? false: true;
	}
	[Serializable]
	public class saveData
	{
		public string name = "Bob";
		public float[] scores;
		public float number;
		public List<Level> level = new List<Level>();
		public int initializedLevelCount = 0;
		//Dictionary<int, int> levelScores = new Dictionary<int, int> ();

		public saveData (int numberOfLevels) // this is a constructor to initialize a certain number of levels into the list.
		{
			for (int i = 0; i < numberOfLevels; i++) 
			{
				level.Add(new Level());
			}
		}

		public saveData()
		{
		}

	
	}

	[Serializable]
	public class Level
	{
		//public SceneAsset scene;
		public string levelName;
		public int worldNumber;
		public int levelHighScore;
		public int puffletsHatched = 0;
		public int sceneIndex;
		//public Scene scene;

		public Level(string sc)//UnityEngine.Object sc)
		{
			levelName = sc;//sc.name;
			print(levelName);
			levelHighScore = 0;
			worldNumber = 1;//figure out how to reference level number and stuff

		}
		public Level()
		{
			levelName = null;
			levelHighScore = 0;
			worldNumber = 1;
		}

	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
}
