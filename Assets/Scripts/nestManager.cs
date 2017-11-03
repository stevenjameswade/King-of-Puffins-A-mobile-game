using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nestManager : MonoBehaviour {
	public static nestManager instance = null;

	void Awake()
	{
		//print ("Awake Called");
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	/*public class puffinEgg
	{
		Animator anim;
		int fishFed;
		public puffinEgg (GameObject gameobject)
		{
			anim = gameobject.GetComponent<Animator>();
		}
		public void growEgg()
		{

			anim.SetInteger ("eggSize", fishFed);

		}
	}
	*/
	puffinEgg activeEgg;
	public int activeEggIndex = 0;

	public int totalFishPoints{ get; set;}
	public int currentHatchIndex;
	public int currentHatchGoal;

	public GameObject[] puffinEggs;

	public List<puffinEgg> Eggs = new List<puffinEgg>();


	// Use this for initialization
	void Start () 
	{
		foreach (GameObject egg in puffinEggs)
		{
			puffinEgg currentEgg = egg.GetComponent<puffinEgg>();
			Eggs.Add (currentEgg);
		}

		print (Eggs.Count);
		currentHatchIndex = 0;
		currentHatchGoal = LevelConstraints.instance.hatchGoals[currentHatchIndex];
		activeEgg = Eggs [0];
		activeEgg.activeEgg = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if (activeEgg.hatched)
		{
			if (currentHatchIndex < LevelConstraints.instance.hatchGoals.Length-1)
			{
				currentHatchIndex++;
				currentHatchGoal = LevelConstraints.instance.hatchGoals [currentHatchIndex];
			}
			activeEgg.circle.enabled = false;
			if (activeEggIndex < Eggs.Count - 1)
			{
				activeEggIndex++;
				GameManager.instance.scoreText.text = (activeEggIndex+ " Pufflets fed! hooray!");
				activeEgg = Eggs [activeEggIndex];
				activeEgg.activeEgg = true;


			} else
			{
				GameManager.instance.scoreText.text = ("All Pufflets fed! hooray!");
			}

		}
		
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("deadFish"))
		{
			int pointValue = other.gameObject.GetComponent<fishMovement> ().pointValue;
			totalFishPoints = totalFishPoints + pointValue;
			activeEgg.FishAte = activeEgg.FishAte + pointValue;
			activeEgg.growEgg ();
			/*if (totalFishPoints > LevelConstraints.instance.hatchGoals[currentHatchIndex])
			{
				currentHatchIndex++;
				currentHatchGoal = LevelConstraints.instance.hatchGoals [currentHatchIndex];
			}*/
			/*if (activeEgg.hatched)
			{
				activeEgg.circle.enabled = false;
				if (activeEggIndex < Eggs.Count - 1)
				{
					activeEggIndex++;
					GameManager.instance.scoreText.text = (activeEggIndex+ " Pufflets fed! hooray!");
					activeEgg = Eggs [activeEggIndex];
					activeEgg.activeEgg = true;
				} else
				{
					GameManager.instance.scoreText.text = ("All Pufflets fed! hooray!");
				}

			}*/

			other.gameObject.SetActive (false);

		}

	}

	public void nextActiveEgg()
	{

	}
}
