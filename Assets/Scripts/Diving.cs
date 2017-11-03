using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using NUnit.Framework.Internal;
using System;
using UnityEngine.Events;


using UnityEngine.SceneManagement;



public class Diving : MonoBehaviour {

	private Rigidbody2D rbd2d;
	public float diveForce = 200f;
	public float speedBurst = 500f;
	public float rotSpeed = 3f;
	public InputField massField;
	private float mass;
	private string massString;
	public Text massPlaceholderText;
	public Slider massSlider;
	public Slider diveSlider;
	public Text diveForceText;
	public Slider kickSlider;
	public Text kickForceText;

	private Vector2 startPosition;
	private int attemptCount;


	public Text scoreText;


	private SpriteRenderer spriteRenderer;



	public float setMass
	{
		get
		{
			return mass;
		}
		set
		{
			mass = float.Parse(massField.text);
			mass = massSlider.value;// value;
				
			print (mass);
			rbd2d.mass = mass;
			massPlaceholderText.text = Convert.ToString(rbd2d.mass);
		}

	}

	// Use this for initialization
	void Start () {

		rbd2d = GetComponent<Rigidbody2D> ();
		mass = rbd2d.mass;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		startPosition = rbd2d.position;
	}



	void Update ()
	{
		if (GameManager.instance.gameState == GameManager.GameState.diving)
		{
			if (Input.GetMouseButton (0))
			{
				rbd2d.AddForce (Vector2.down * diveForce);

			}

			if (Input.GetMouseButtonDown (1))
			{
				/*if (rbd2d.velocity.y >= 0)
			{
				rbd2d.velocity = new Vector2(rbd2d.velocity.x, 0f);
			}*/

				//rbd2d.AddForce (Vector2.down * speedBurst);

				rbd2d.AddForce (rbd2d.velocity * speedBurst);
			}
			if (Input.GetMouseButton (3) || Input.GetButton ("Jump"))
			{
				rbd2d.AddForce (Vector2.right * 50);
				//rbd2d.AddForce (rbd2d.velocity * 50);

			}
		}
	}
	void FixedUpdate()
	{
		if (GameManager.instance.gameState == GameManager.GameState.diving)
		{
			float angle = Mathf.Atan2 (rbd2d.velocity.y, rbd2d.velocity.x) * Mathf.Rad2Deg;
			//transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			rbd2d.MoveRotation (Mathf.LerpAngle (rbd2d.rotation, angle, rotSpeed * Time.deltaTime));

		
		}
		if (rbd2d.rotation < 0)
		{
			rbd2d.rotation = rbd2d.rotation + 360;
		} else if (rbd2d.rotation > 360)
		{
			rbd2d.rotation = rbd2d.rotation - 360;
		}
		if (Mathf.Abs(rbd2d.rotation) > 90 && Mathf.Abs(rbd2d.rotation) < 270)
		{
			print (true);
			spriteRenderer.flipY = true;
			//puffin.transform.localScale = new Vector3 (1, -1, 1);
		} else if (Mathf.Abs(rbd2d.rotation) < 90 || Mathf.Abs(rbd2d.rotation) > 270)
		{
			spriteRenderer.flipY = false;
		}

	}
	/*public void setMass()
	{
		string massFloat = massfield.text;
		rbd2d.mass = int.Parse(string massFloat);
		print(rbd2d.mass);
		}*/


	public void MassSliderChange()
	{
		rbd2d.mass = massSlider.value;
		print (rbd2d.mass);
		massPlaceholderText.text = Convert.ToString(rbd2d.mass);
	}

	public void MassFieldEdit()
	{
		rbd2d.mass = float.Parse (massField.text);
		print (rbd2d.mass);
		massPlaceholderText.text = Convert.ToString(rbd2d.mass);
	}

	public void KickSliderEdit()
	{
		speedBurst = kickSlider.value;
		kickForceText.text = Convert.ToString(speedBurst);

	}

	public void DiveSliderEdit()
	{
		diveForce = diveSlider.value;
		diveForceText.text = Convert.ToString(diveForce);
	}


	public void LoadByIndex (int sceneIndex)
	{
		SceneManager.LoadScene (sceneIndex);
	}

	public void DropPlayer()
	{
		rbd2d.velocity = Vector2.zero;
		rbd2d.MovePosition (new Vector2(0f, 27.5f));
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "Boundary")
		{
			rbd2d.position = (new Vector2 (-39.1f, rbd2d.position.y));
		} else if (other.name == "Boundary2")
		{
			rbd2d.position = (new Vector2 (39.1f, rbd2d.position.y));
		}

		if (other.name == "killzone")
		{
			rbd2d.isKinematic = true;
			rbd2d.velocity = (Vector2.zero);
			GameManager.instance.gameState = GameManager.GameState.start;
			rbd2d.position =  (startPosition);
			attemptCount++;
			//Invoke ("deadZonefunctioncall", .2f);

			//Camera.main.GetComponent<DeadzoneCamera> ().enabled = false;
			if (attemptCount > 5)
			{
				LoadByIndex (0);
			}
			

		} 




		if (other.CompareTag("pickup"))
		{
			other.gameObject.SetActive(false);
			GameManager.instance.score ++;
			scoreText.text = "You have collected " + GameManager.instance.score + " collectibles.";
		}
		//if(other.CompareTag("boundary"))


	}
	public void deadZonefunctioncall()
	{
		GameManager.instance.DeadZoneSwitch ();
	}
}

