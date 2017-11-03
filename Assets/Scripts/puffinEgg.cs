using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puffinEgg : MonoBehaviour {


	Animator anim;
	private bool actvEgg;
	public bool hatched{ get; set;}
		
	public bool activeEgg{ //{ get; set;}
		get
		{
			return actvEgg;
		} 
		set
		{
			actvEgg = value;
			print ("ActiveEgg cycled");
			if (nestManager.instance.totalFishPoints >= nestManager.instance.currentHatchGoal)
			{
				hatched = true;
				growEgg();
				GameManager.instance.currentPuffletsHatched++;
				actvEgg = false;
			}
		}

	}
	public CircleCollider2D circle;
	private int fishAte;
	public int FishAte
	{
		get
		{
			return fishAte;
		}
		set
		{
			fishAte = value;
			print ("Fed the puffin egg " + fishAte);
			if (nestManager.instance.totalFishPoints >= nestManager.instance.currentHatchGoal)
			{
				hatched = true;
				growEgg();
				GameManager.instance.currentPuffletsHatched++;
				activeEgg = false;
			}
		}

	}
	public GameObject BabyPuffin;
		


	public void growEgg()
	{
		gameObject.transform.localScale = Vector3.Lerp (gameObject.transform.localScale, 2f * Vector3.one, .5f);

		//anim.SetInteger ("eggSize", fishAte);

		if (hatched == true) //this just makes the egg hatch when it has the needed requirements
		{
			anim.SetInteger ("eggSize", 3);
		}
	}
		
	void Start () 
	{
	/*	BabyPuffin = GetComponentInChildren<GameObject> ();

		BabyPuffin.transform.localScale = Vector3.one * 4f;
		BabyPuffin.transform.position = BabyPuffin.transform.position + Vector3.up * 10f;*/
		//BabyPuffin.SetActive(true);
		hatched = false;
		anim = GetComponent<Animator>();
		circle = GetComponent<CircleCollider2D> ();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (activeEgg)
		{
			gameObject.transform.localScale = Vector3.Lerp (gameObject.transform.localScale, 10f * Vector3.one, .5f);
		}
	}
}
