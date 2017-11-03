using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.SceneManagement;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.AI;
//using UnityEditor;


public class LevelManager : MonoBehaviour {
	public static LevelManager instance; 

	//public GameObject[] Levels;
	//public Scene[] scenes;
	//public Object[] Levels;

	public string[] Levelss;
	[SerializeField]
	public List<string> Levels = new List<string> (); 
	public List<GameObject> go = new List<GameObject> ();
	[SerializeField]
	public List<testClass> people = new List<testClass>();


	// Use this for initialization
	void Start () 
	{
		//print (Levels.Length);
		int numberOfScenes = SceneManager.sceneCountInBuildSettings;
		/*Scene[] scenes = new Scene[numberOfScenes];
		for (int i = 0; i < numberOfScenes; i++)
		{
			//scenes[i] =  SceneManager.GetSceneAt (i); // this is only for currently loaded scenes
			scenes[i] =  SceneManager.GetSceneByBuildIndex(i);

		}

		foreach (Scene sc in scenes)
		{
			print (sc.name);
		}
		*/


	}


	void Awake()
	{

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		//DontDestroyOnLoad (gameObject);


	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public class testClass
	{
		string name;
		int age;
	}
}
