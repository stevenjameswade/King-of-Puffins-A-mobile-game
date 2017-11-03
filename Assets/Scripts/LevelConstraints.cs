using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConstraints : MonoBehaviour {

	public int[] hatchGoals = new int[3];

	//public int fishToHatchFirstEgg;
	//public int fishToHatchSecondEgg;
	//public int fishToHatchThirdEgg;

	//public int currentHatchRequirement;
	//public int fishNeededFor3Hatches;

	public static LevelConstraints instance = null;

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
		//fishNeededFor3Hatches = fishToHatchFirstEgg + fishToHatchSecondEgg + fishToHatchThirdEgg;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
