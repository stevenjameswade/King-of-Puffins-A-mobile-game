using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class fishControl : MonoBehaviour
{

	public class fishInMouth  
	{

		public fishMovement movement;

		public Rigidbody2D rb;

		public fishInMouth ( fishMovement m, Rigidbody2D r)
		{
			movement = m;
			rb = r;
		}

		public fishInMouth()
		{
			//rb = GetComponent<Rigidbody2D> ();
			//movement = GetComponent<fishMovement>();
		}

		public void turnOffMovement()
		{
			movement.enabled = false;
		}



	}




	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
