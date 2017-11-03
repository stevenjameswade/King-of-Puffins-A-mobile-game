using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveMovement : MonoBehaviour {

	private Rigidbody2D rd2d;
	public bool scrollRight = true;
	public int direction = 1;

	// Use this for initialization
	void Start () {

		rd2d = GetComponent<Rigidbody2D> ();
		//rd2d.velocity = new Vector2 (GameControl.instance.scrollSpeed, 0);

	}

	// Update is called once per frame
	void Update () 
	{
		direction = scrollRight== true ? 1 : -1;
	
		rd2d.velocity = new Vector2 (GameManager.instance.waveSpeed* direction, 0);


	}
}

