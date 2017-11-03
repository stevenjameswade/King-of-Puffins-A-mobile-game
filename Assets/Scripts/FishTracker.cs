using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishTracker : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		GameManager.instance.Fish.Add (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
