using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor.Animations;
//using UnityEditor;
//using UnityEngine.Networking.Types;


public class puffinControl : MonoBehaviour {
	public class fishInMouth  
	{
		
		public fishMovement movement;

		public Rigidbody2D rb;
		public CircleCollider2D collider;

		public fishInMouth ( fishMovement m, Rigidbody2D r)
		{
			movement = m;
			rb = r;
			collider = rb.GetComponent<CircleCollider2D>();
		}

		public void turnOffMovement()
		{
			movement.enabled = false;
			collider.enabled = false;

		}

		public void turnOnMovement()
		{
			movement.enabled = true;
			rb.transform.parent = null;
			//rb.isKinematic = true;
			rb.simulated = true;
			collider.enabled = true;

		}
		public void feedBaby()
		{
			rb.transform.parent = null;
			//rb.isKinematic = true;
			rb.simulated = true;
			collider.enabled = true;
		}
		public void moveToMouth(Transform holder)
		{
			rb.transform.SetParent(holder);
			rb.simulated = false;
			rb.gameObject.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			rb.position = holder.position;
			rb.gameObject.transform.localPosition = Vector3.zero;
			rb.gameObject.transform.localRotation = Quaternion.identity;
			rb.gameObject.transform.localScale = Vector3.one;

		}





	}


	public float rotSpeed = 10f;


	private Rigidbody2D rb2d;
	private Collider2D capsule;

	private Vector2 startPosition;
	private Vector2 direction; //the normal of puffin to mouse

	private Vector2 startClick;
	private Vector2 endClick;

	public bool puffinInWater = false;
	public bool puffinInAir = true;
	public float puffinSwimSpeed = 20f;
	public float puffinFlySpeed = 30f;
	public float puffinMaxSpeed = 50f;
	public float puffinOutOfAirSpeed = 10f;
	public bool stuckOnGround = false;
	private bool facingRight = true;
	private float currentSpeed;

	public GameObject[] fishHolders;
	//public fishInMouth[] ateFish;
	[SerializeField]
	public List<fishInMouth> ateFish = new List<fishInMouth>();


	private float airSupply = 100f;
	private float lungCapacity = 5f; //time to run out of air
	private float breatheRate = 1f; //reference velocity for smooth damp
	private float speedBoostAirRate = 30f;

	public bool rotate = true;
	private Vector3 rotation;
	private Animator anim;

	public float surfacingSpeed = .5f;

	public bool controllable = true;



	public enum playerState
	{
		flying,
		swimming,
		surface,
		outOfAir,
		walking,
	}

	public playerState puffinState = playerState.surface;



	void Start () 
	{
		//print (airSupply);
		rb2d = GetComponent<Rigidbody2D> ();
		capsule = GetComponent<Collider2D> ();
		anim = GetComponent<Animator> ();
		startPosition = rb2d.position;
	}
	

	void Update () 
	{
		// this is just for testing the Fish eating code
		if (Input.GetKeyDown ("space") || Input.touchCount == 5)
		{
			loseFish ();
		}

		
		//this makes sure puffin doesn't go too fast
		Vector2.ClampMagnitude(rb2d.velocity, puffinMaxSpeed);
		currentSpeed = rb2d.velocity.magnitude;
		direction = ClickManager.instance.puffintoMouseDirection;
		if (GameManager.instance.slider != null)
		{
			
			if (airSupply < 5f)
			{
				puffinState = setPuffinState (playerState.outOfAir);
			}
			if (airSupply > 95f)
			{
				GameManager.instance.slider.SetActive (false);
			} else if (airSupply < 95)
			{
				GameManager.instance.slider.SetActive (true);
			}

			GameManager.instance.airMeter.value = airSupply;
		}
		puffinStateDependentActions ();
		if (GameManager.instance.gameState == GameManager.GameState.diving || GameManager.instance.gameState == GameManager.GameState.gameWin)
		{
			// this code checks if a Fish is close to puffin beak, and opens the beak if so
			beakControl();

			if (puffinInAir )
			{
				//this refills the breathe meter
				airSupply =    Mathf.Lerp(airSupply, 100f, .9f*Time.deltaTime); //airSupply + 80f*Time.deltaTime;


			}

			if (puffinState != playerState.outOfAir && controllable)
			{
				if (Input.touchCount == 1 || Input.GetMouseButton (0)) // this might make problems so puffin still swims when  multiple touches on game.
				{

					//Debug.DrawLine (rb2d.transform.position, (Vector3)ClickManager.instance.mousePos2d, Color.black, 5f);
					if (puffinState == playerState.swimming)// !puffinInAir && puffinInWater)
					{

						rb2d.AddForce (direction * 500f, ForceMode2D.Impulse);


						//these statements check the puffins speed, if going below the minimum, speeds it up, otherwise it stays where it is
						if (currentSpeed > puffinSwimSpeed)
						{
							rb2d.velocity = Vector2.ClampMagnitude (rb2d.velocity, currentSpeed);
						} else if (currentSpeed <= puffinSwimSpeed)
						{
							rb2d.velocity = Vector2.ClampMagnitude (rb2d.velocity, puffinSwimSpeed);
						}
					}
						
						
					
				
					if (puffinState == playerState.flying)
					{
						//if click below puffin
						if (direction.y < 0)
						{
							rb2d.AddForce (direction * 50f, ForceMode2D.Impulse);
						}
						//if click above puffin
						if (direction.y > 0 && Input.GetMouseButton (0))
						{
							
							rb2d.AddForce (new Vector2 (direction.x * 100f, direction.y * 70f), ForceMode2D.Impulse);
		
						}
						rb2d.velocity = Vector2.ClampMagnitude (rb2d.velocity, puffinFlySpeed);
					}
					/*
					 
					 if (puffinState == playerState.swimming)// !puffinInAir && puffinInWater) //this is the original swimming code, I'm trying to improve it
					{

						//these statements check the puffins speed, if going below the minimum, speeds it up, otherwise it stays where it is
						if (currentSpeed > puffinSwimSpeed)
						{
							rb2d.velocity = direction * currentSpeed;
						} else if (currentSpeed <= puffinSwimSpeed)
						{
							rb2d.velocity = direction * puffinSwimSpeed;
						}
					}


					 */
					/*if (puffinState == playerState.flying) //this is the saved working code, so I can try to make some changes
					{
						//if click below puffin
						if (direction.y < 0 )
						{
							rb2d.AddForce (direction*5000f, ForceMode2D.Force);
						}
						//if click above puffin
						if (direction.y > 0 && Input.GetMouseButton (0))
						{
							
							rb2d.AddForce (new Vector2 (direction.x * 1000f, direction.y* 300f), ForceMode2D.Force);
		
							if (Input.GetMouseButtonDown (0))
							{
								float upforce = direction.y * 1000f;
								print ("upforce applied");
								rb2d.AddForce (new Vector2 (0f, upforce), ForceMode2D.Impulse);
							
							}
						}

						//makes it so the puffin doesn't fly past the max fly speed
						rb2d.velocity = Vector2.ClampMagnitude (rb2d.velocity, puffinFlySpeed);


					} */
					if (puffinState == playerState.surface )//|| puffinState == playerState.walking)// !puffinInAir && puffinInWater)
						
					{



						rb2d.velocity = new Vector2(direction.x * puffinSwimSpeed, 0f);
					
					}
					if (puffinState == playerState.walking)
					{
						Vector2 velocity = rb2d.velocity;
						velocity.x = direction.x *puffinSwimSpeed;
						rb2d.velocity = velocity ;
						//rb2d.velocity = new Vector2 (direction.x * puffinSwimSpeed, 0f);
					}
						



				}
			}
			/*if (puffinState == playerState.swimming && (Input.touchCount == 2 ) && controllable)//|| Input.GetMouseButton(1)) //I'm changing this up
			{
				//when two fingers are on the screen, this make puffin move to max speed
				rb2d.velocity = Vector2.Lerp (rb2d.velocity, direction * puffinMaxSpeed, .3f);

				//make it use up your air faster
				airSupply = airSupply- speedBoostAirRate*Time.deltaTime;
			} */

			if (puffinState == playerState.swimming && (Input.touchCount == 2 ) && controllable) 
			{
				//when two fingers are on the screen, this make puffin move to max speed
				rb2d.AddForce (direction * 1000f, ForceMode2D.Impulse);

				//make it use up your air faster
				airSupply = airSupply- speedBoostAirRate*Time.deltaTime;
				rb2d.velocity = Vector2.ClampMagnitude (rb2d.velocity, puffinSwimSpeed);
			}


			if (Input.touchCount == 0)
			{
				//capsule.density = 3f;
			}

	//changes puffin state to the surface or slows puffin down

			if (puffinInAir && puffinInWater && Input.touchCount == 0 && puffinState != playerState.surface)
			{
				//this slow velocity at surface
				rb2d.velocity = Vector2.Lerp (rb2d.velocity, Vector2.zero, 5f * Time.deltaTime);
			}

		}


		if (Input.GetMouseButtonDown (1) || (Input.touchCount == 2 && Input.GetTouch(1).phase==TouchPhase.Began))
		{
			if (puffinState != playerState.flying)
			{
			//	speedBoost ();
			}
		}
	}

	void FixedUpdate()
	{
		if (rotate)
		{
			fwdRotate ();
		} else if (!rotate && !rb2d.freezeRotation)
		{
			//print ("No More Rotate");
			//stopRotate ();
		}

		flip ();


	}





	public void fwdRotate()
	{
		if (GameManager.instance.gameState == GameManager.GameState.diving || GameManager.instance.gameState == GameManager.GameState.gameWin)
		{
			float angle = Mathf.Atan2 (rb2d.velocity.y, rb2d.velocity.x) * Mathf.Rad2Deg;
			rb2d.MoveRotation (Mathf.LerpAngle (rb2d.rotation, angle, rotSpeed * Time.deltaTime));


		}
		if (rb2d.rotation < 0)
		{
			rb2d.rotation = rb2d.rotation + 360;
		} else if (rb2d.rotation > 360)
		{
			rb2d.rotation = rb2d.rotation - 360;
		}


	}

	public void  stopRotate()// this doesn't work %100 because of lerp
	{
		rotate = false;
		 float angle = 0f;
		//print ("Stop Rotate Called. Facing Right = " + facingRight);
		if (facingRight)
		{
			angle = 0f;
		} else if (!facingRight)//facing left
		{
			if (rb2d.rotation < 0)
			{
				angle = -180f;
			} else
			{
				angle = 180f;
			}

		}
		StartCoroutine (RotateToSurface (angle));
		//print (angle);
		//rb2d.velocity = new Vector2 (rb2d.velocity.x, 0f);

	}

	IEnumerator RotateToSurface (float angle)
	{
		//print ("CoRoutine. Angle is " + angle);
	
		rotate = false;
		rb2d.freezeRotation = true;
		if (rotate)
		{
			//print ("broke from Co-routine");
			yield break;
		}

		//pay attention to this statement. I accidentally made it infinite once and it crashed unity. This checks 2 statements, it only runs when both are true. It is either 2 less than 0, or 360
		while (Mathf.Abs (rb2d.rotation - angle) > 2f && Mathf.Abs (rb2d.rotation - angle - 360) > 2)// && puffinState == playerState.surface)
		{
			rb2d.rotation = Mathf.LerpAngle (rb2d.rotation, angle, 3f * Time.deltaTime);
				//rb2d.MoveRotation (Mathf.LerpAngle (rb2d.rotation, angle, 10*rotSpeed * Time.deltaTime));
				//print (rb2d.rotation);
				//	print (Mathf.Abs (rb2d.rotation - angle));
			//print("Iteration" + (Mathf.Abs (rb2d.rotation - angle-360)));
			if (rotate || Input.GetMouseButton(0)) // I added the break when the mouse is clicked, otherwise the puffin tries to rotate to the first rotation even if you told it to go elsewhere later
			{
				//print ("broke from Co-routine");
				yield break;
			}
			yield return new WaitForFixedUpdate ();		
		}
		rb2d.MoveRotation (angle);
		

	}
	public void startRotate()
	{
		//print ("Start Rotate");
		rotate = true;
		if (facingRight)
		{
			rb2d.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
		} else
		{
			rb2d.transform.localEulerAngles = new Vector3 (0f, 0f, 180f);
		}
		rotate = true;
		rb2d.freezeRotation = false;


	}

	public void flip()
	{
		//print ("flip called. Rotate is " + rotate);
		if (rotate)
		{
			
			if (Mathf.Abs (rb2d.rotation) > 91 && Mathf.Abs (rb2d.rotation) < 269)
			{
				rb2d.transform.localScale = new Vector3 (1f, -1f, 1f);
				facingRight = false;

			} else if (Mathf.Abs (rb2d.rotation) < 89 || Mathf.Abs (rb2d.rotation) > 271)
			{
				rb2d.transform.localScale = new Vector3 (1f, 1f, 1f);
				facingRight = true;
			}
		} 
		if (!rotate)
		{
			

			if (rb2d.velocity.x < -2f)
			{
				rb2d.transform.localEulerAngles = new Vector3 (0f, 0f, 180f); // when you set the local eulers and scale directly, it doesn't create a weird rotation effect
				rb2d.transform.localScale = new Vector3 (1f, -1f, 1f);

			
				facingRight = false;
			} else if (rb2d.velocity.x > 2f)
			{
				rb2d.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
				rb2d.transform.localScale = new Vector3 (1f, 1f, 1f);
				facingRight = true;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("pickup"))
		{
			fishInMouth currentFish =  new fishInMouth (other.gameObject.GetComponent<fishMovement> (), other.gameObject.GetComponent<Rigidbody2D> ());

			ateFish.Add (currentFish);
			currentFish.turnOffMovement();
			currentFish.moveToMouth (fishHolders [ateFish.Count-1].transform);//[GameManager.instance.score].transform);

			print ("Ate Fish");
			print (ateFish.Count);
			//currentFish.turn


			GameManager.instance.fishCloseToPuffin = GameManager.instance.fishCloseToPuffin > 0 ? GameManager.instance.fishCloseToPuffin - 1: GameManager.instance.fishCloseToPuffin; // removes the fish from proximity list if list is greater than 0
			GameManager.instance.score = GameManager.instance.score + 1 ; 
			if (GameManager.instance.score > GameManager.instance.currentHighScore)
			{
				GameManager.instance.highScoreText.text = "Highscore : " + GameManager.instance.score;
			}
				
			GameManager.instance.scoreText.text = "You have collected " + GameManager.instance.score + " sandeels.";
		}
		if (other.gameObject.CompareTag ("enemy"))
		{
			loseFish ();
			foreach (ContactPoint2D contact in other.contacts)
			{
				rb2d.AddForce (contact.normal * 500f, ForceMode2D.Impulse);
			}

		}
		/*if (other.gameObject.name == "Nest")
		{
			feedBaby ();
		}*/

		if (puffinState == playerState.flying) //This simply checks if the puffin hit something lower than it when it is flying, if so change to puffin state walking.
		{
			foreach (ContactPoint2D contact in other.contacts)
			{
				if (contact.point.y < rb2d.transform.position.y)
				{
					puffinState = setPuffinState(playerState.walking);
					print ("Fly to walking state");
				}
			}

		}
		if (puffinState == playerState.surface) //This simply checks if the puffin hit something lower than it when it is on surface, if so change to puffin state walking.
		{
			foreach (ContactPoint2D contact in other.contacts)
			{
				if (contact.point.y < rb2d.transform.position.y)
				{
					if (!puffinInWater)
					{
						puffinState = setPuffinState (playerState.walking);
						print ("surface to walking state");
					}
				}
			}

		}


	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "Boundary")
		{
			rb2d.position = (new Vector2 (-39.1f, rb2d.position.y));
		} else if (other.name == "Boundary2")
		{
			rb2d.position = (new Vector2 (39.1f, rb2d.position.y));
		}

		if (other.name == "killzone")
		{
			airSupply = 100f;
			//rb2d.isKinematic = true;
			rb2d.velocity = (Vector2.zero); 
			rb2d.rotation = 0f;
			rb2d.angularVelocity = 0f;
			GameManager.instance.gameState = GameManager.GameState.diving;
			rb2d.position = (startPosition);
			GameManager.instance.attemptCount++;
			//Invoke ("deadZonefunctioncall", .2f);

			if (GameManager.instance.attemptCount > 1)
			{
				GameManager.instance.RestartLevel();
			}


		} 

		if (other.name == "Water")
		{
			puffinInWater = true;
			print ("Puffin in Water");

		}
		if (other.name == "Air")
		{
			puffinInAir = true;

		}
		if (other.name == "NestPortal")
		{
			GameManager.instance.levelToNest ();
			DontDestroyOnLoad (gameObject);
			GameManager.instance.LoadLevel ("puffinNestAttempt");
			rb2d.position = new Vector2 (60f, 2.3f);

		}
		if (other.name == "NestExit")
		{
			

			Destroy (gameObject);
			GameManager.instance.LevelEnded ();
			//Destroy (GameManager.instance.gameObject);
		
			GameManager.instance.LoadLevel ("MainMenu");//GameManager.instance.LoadLevel ("Jbackground");

		}




		/*if (other.CompareTag ("pickup"))
		{
			print ("collected pickup");
			other.gameObject.transform.SetParent (fishHolders [GameManager.instance.score].transform);
			Rigidbody2D rb = other.GetComponent<Rigidbody2D> ();
			rb.isKinematic = true;
			rb.simulated = false;
			other.gameObject.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			rb.position = fishHolders [GameManager.instance.score].transform.position;
			other.gameObject.transform.localPosition = Vector3.zero;
			rb.transform.localScale = new Vector3 (1f, 1f, 1f);
			other.gameObject.transform.localRotation = Quaternion.identity;
			//Collider2D colid = other.GetComponent<Collider2D>();
			//colid.enabled = false;

			fishMovement movement = other.GetComponent<fishMovement> ();
			movement.enabled = false;
			//other.gameObject.
			//other.gameObject.SetActive (false);
			GameManager.instance.fishCloseToPuffin = GameManager.instance.fishCloseToPuffin > 0 ? GameManager.instance.fishCloseToPuffin - 1: GameManager.instance.fishCloseToPuffin; // removes the fish from proximity list if list is greater than 0
			GameManager.instance.score = GameManager.instance.score + 1 ; 
			GameManager.instance.scoreText.text = "You have collected " + GameManager.instance.score + " sandeels.";
		}*/
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.name == "Water")
		{
			if (other.enabled)
			{
				print("Collider active");
				puffinInWater = false;
				print ("Puffin not in Water");
			}
		}

		if (other.name == "Air")
		{
			if (other.enabled)
			{
				puffinInAir = false;
			}
		}

	}

	/*void OnTriggerStay2D(Collider2D other) // had to change to trigger stay as the collider is changing. trigger exit is called when collider inactivated
	{
		if (other.name == "Water")
		{
			puffinInWater = true;
			//print ("Puffin staying in Water");

		}

		if (other.name == "Air")
		{
			puffinInAir = true;

		}

	} */

	void OnCollisionStay2D(Collision2D other)
	{

		//this frees the bird when stuck on the ocean floor
		if (stuckOnGround)
		{
			rb2d.position = new Vector2 (rb2d.position.x, rb2d.position.y + .2f);
			stuckOnGround = false;
			print (stuckOnGround);

		}
		if (other.gameObject.name == "ocean bottom")
		{
			stuckOnGround = true;
			print (stuckOnGround);
		}

		if (other.gameObject.name == "Nest")
		{
			StartCoroutine ("ReleaseFish");
			//feedBaby ();
		}
		//print ("Puffin is Stuck!!!");
	}

	#region PuffinState Transitions
	public playerState setPuffinState ( playerState newState)
	{
		switch (newState)
		{
		case playerState.flying:
			anim.SetBool ("underwater", false);
			anim.SetTrigger ("flying");
			anim.SetInteger ("puffinState", 1);
			break;

		case playerState.swimming:
			anim.SetTrigger ("dive");
			anim.SetInteger ("puffinState", 2);
			break;

		case playerState.surface:
			//print ("Set surface State");
			anim.SetTrigger ("toStanding");
			anim.SetInteger ("puffinState", 3);
		
			stopRotate ();

			break;

		case playerState.walking:
			//print ("Walking player State");
			anim.SetTrigger ("toStanding");
			anim.SetInteger ("puffinState", 3);

			stopRotate ();
			break;

		case playerState.outOfAir:
			anim.SetBool ("outOfAir", true);
			break;

		
		default:
			print ("default switch statement");
			break;
		}
		return newState;
	}
	#endregion

	#region #PuffinActions
	public void puffinStateDependentActions()
	{
		switch (puffinState)
		{
		case playerState.flying:

			if (puffinInWater && direction.y < 0)// && Input.touchCount>0)
			{
				puffinState = setPuffinState (playerState.swimming);
				print ("fly to swim");
			}

			if (Input.touchCount < 1 && puffinInWater && rb2d.velocity.sqrMagnitude < surfacingSpeed) //if no one is touching, make puffin land on surface
			{
				
				puffinState = setPuffinState (playerState.surface);
				print ("fly to surface");
			}

			
			break;

		case playerState.swimming:
			airSupply = Mathf.SmoothDamp (airSupply, 0f, ref breatheRate, lungCapacity); //this drains the air reserves over time

			if (puffinInAir && direction.y > 0)// && Input.touchCount>0)
			{
				puffinState = setPuffinState (playerState.flying);

				print ("swim tp fly");
			}

			if (Input.touchCount < 1 && puffinInAir && puffinInWater && rb2d.velocity.sqrMagnitude < surfacingSpeed) //if no one is touching, make puffin surface
			{
				puffinState = setPuffinState (playerState.surface);
				print ("swim to surface");
			}
			break;

		case playerState.surface: // next thing to work onpuffinInWater && !puffinInAir && direction.y<Mathf.Abs( direction.x)



			anim.SetFloat ("swimSpeed", Mathf.Abs (currentSpeed));
			if (Input.touchCount == 0) //this makes it so the puffin doesn't bob too much, or slide off of the screen.
			{
				rb2d.velocity = Vector2.Lerp (rb2d.velocity, Vector2.zero, .8f * Time.deltaTime);
			}
			
			if (ClickManager.instance.puffinToMouse.y < -6f && Input.GetMouseButton (0))//direction.y < -.7f 
			{
				anim.SetTrigger ("dive");
				rb2d.AddForce (direction * 20, ForceMode2D.Impulse);
				puffinState = setPuffinState (playerState.swimming);
				startRotate ();
				print ("surface to swim");
			} else if (ClickManager.instance.puffinToMouse.y > 6f && Input.GetMouseButton (0))
			{
				rb2d.velocity = (direction * 5);
				rb2d.AddForce (direction * 100, ForceMode2D.Impulse);
				puffinState = setPuffinState (playerState.flying);
				startRotate ();
				print ("surface tp fly");
			}
			/*if (!puffinInWater)
			{
				puffinState = setPuffinState (playerState.walking);
			}*/
			break;

		case playerState.outOfAir:
			if (Input.GetMouseButton (0) && controllable)
			{
				rb2d.AddForce ((new Vector2 (direction.x * 20f, direction.y * 10f)), ForceMode2D.Impulse);

			}
			Vector2.ClampMagnitude (rb2d.velocity, puffinOutOfAirSpeed);
			if (airSupply > 5f)
			{
				anim.SetBool ("outOfAir", false);
				if (Input.touchCount ==0 && rb2d.velocity.sqrMagnitude < surfacingSpeed) //if no one is touching, make puffin surface
				{
					puffinState = setPuffinState (playerState.surface);
				}

				if (puffinInAir && direction.y > 0f && Input.touchCount < 3 && Input.touchCount > 0) //if in the air and directing up, fly
				{
					
					puffinState = setPuffinState (playerState.flying);
				}
				if (puffinInWater && direction.y < 0f && Input.touchCount < 3 && Input.touchCount>0) //if in the air and directing up, fly
				{

					puffinState = setPuffinState (playerState.swimming);
				}
			}
			break;

		case playerState.walking:
			anim.SetFloat ("swimSpeed", Mathf.Abs (currentSpeed));
			if (Input.touchCount == 0) //this makes it so the puffin doesn't bob too much, or slide off of the screen.
			{
				//rb2d.velocity = Vector2.Lerp (rb2d.velocity, Vector2.zero, .8f * Time.deltaTime);
			}
			if (ClickManager.instance.puffinToMouse.y > 6f && Input.GetMouseButton (0))
			{
				rb2d.velocity = (direction * 5);
				rb2d.AddForce (direction * 100, ForceMode2D.Impulse);
				puffinState = setPuffinState (playerState.flying);
				startRotate ();
				print ("walk to fly");
			}
			if (rb2d.velocity.y < -5f && Input.GetMouseButton (0))// if puffin is falling fast enough, change to flying
			{
				
				puffinState = setPuffinState (playerState.flying);
				startRotate ();
			
			}
			if (puffinInWater)
			{
				puffinState = setPuffinState (playerState.surface);
			}
			

			break;

		}
	}
	#endregion


	public void beakControl()
	{

		// this code checks if a Fish is close to puffin beak, and opens the beak if so

		if (GameManager.instance.fishCloseToPuffin >= 1)
		{
			anim.SetBool ("BeakOpen", true);
		} else if (GameManager.instance.fishCloseToPuffin == 0)
		{
			anim.SetBool ("BeakOpen", false);
		}

	}

	public void speedBoost()
	{
		rb2d.AddForce ( direction * 1000f, ForceMode2D.Impulse);
		print ("boost");
	}

	public void loseFish()
	{
		if (GameManager.instance.score > 0)
		{
			ateFish [ateFish.Count-1].turnOnMovement ();
			ateFish.Remove (ateFish [ateFish.Count-1]);
			//ateFish[GameManager.instance.score-1].rb.transform.parent = null;
			//ateFish [GameManager.instance.score];
			GameManager.instance.score -= 1;

			if (GameManager.instance.score < GameManager.instance.currentHighScore) //if your score is less than highscore, highscore is displayed
			{
				GameManager.instance.highScoreText.text = "Highscore : " + GameManager.instance.currentHighScore;
			}
			GameManager.instance.scoreText.text = "You lost one! You have " + GameManager.instance.score + " sandeels.";
		} else
			print ("No fish currently in mouth");
	}

	public void feedBaby()
	{
		print (ateFish.Count);
		if (ateFish.Count > 0)
		{
			
			ateFish [ateFish.Count-1].feedBaby ();
			ateFish.Remove (ateFish [ateFish.Count-1]);
			//ateFish[GameManager.instance.score-1].rb.transform.parent = null;
			//ateFish [GameManager.instance.score];
			//GameManager.instance.score -= 1;
			//GameManager.instance.scoreText.text = "Yum! Thanks Pops! " + GameManager.instance.score + " sandeels.";
		} else
			print ("I'm hungry! What's the matter with you?");
	}

	IEnumerator ReleaseFish ()
	{
		while(ateFish.Count > 0)
		{
			feedBaby();
			yield return new WaitForSeconds(1f);
		}
		
		/*foreach(fishInMouth fish in ateFish) // It doesn't like it if the collection is modified sill
		{
			feedBaby();
			yield return new WaitForSeconds(.5f);
		}*/
	

		//yield return new WaitForFixedUpdate ();		




	}





}
	