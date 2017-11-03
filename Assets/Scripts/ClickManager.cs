using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Security.Policy;
//using UnityEngine.Analytics;

public class ClickManager : MonoBehaviour {

	//public enum	gameState {enter, hoverDrag, diving, surface};
	//public bool gameStart = true;
	//public bool aiming = false;

	public static ClickManager instance = null;
	public float maxDive = 3.0f;
	public float maxDiveSqr;
	public float launchSpeed = 200f;
	//private Ray2D rayToMouse;

	//public GameObject mainCamera;


	public Rigidbody2D rb2d;
//	private GameObject clickedObject;
	public GameObject puffin;

	private Vector3 mousePos;
	public Vector2 mousePos2d;
	public Vector2 mouseDeltaPos2d;
	public Vector2 puffinToMouse;
	public Vector2 puffintoMouseDirection;

	//private Vector2 firstClick;

	public Vector2 startPos;
	public Vector2 direction;
	public bool directionChosen;

	//private SpriteRenderer spriteRenderer;


	// Use this for initialization
	void Awake()
	{

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		//DontDestroyOnLoad (gameObject); //im trying a new instance for every level
		//DontDestroyOnLoad (puffin);
	}
	void OnLevelFinishedLoading()
	{
		//this isn't called. I don't want another listener. I make the actions happen in game manager instead. 
		//For example, I search for the puffin if the instance is null
		
	}

	void Start () 
	{
		puffin = GameObject.Find ("Puffin");
		if (ClickManager.instance.puffin != null)
		{
			rb2d = puffin.GetComponent<Rigidbody2D> ();
		}
		//spriteRenderer = GetComponent<SpriteRenderer> ();
		maxDiveSqr = maxDive * maxDive;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		#if UNITY_EDITOR 
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		#endif


		//print (mousePos2d);
		//consider moving the raycast to on click to improve performance
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			mousePos = Camera.main.ScreenToWorldPoint(touch.position);

			// Handle finger movements based on touch phase.
			switch (touch.phase)
			{
			// Record initial touch position.
			case TouchPhase.Began:
				startPos = touch.position;
				directionChosen = false;
				break;

				// Determine direction by comparing the current touch position with the initial one.
			case TouchPhase.Moved:
				direction = touch.position - startPos;
				break;

				// Report that a direction has been chosen when the finger is lifted.
			case TouchPhase.Ended:
				directionChosen = true;
				break;
			}
		}
		mouseDeltaPos2d = new Vector2 ((mousePos.x-mousePos2d.x), (mousePos.y-mousePos2d.y));
		mousePos2d =  (new Vector2 (mousePos.x, mousePos.y));
		if (Input.touchCount == 0)
		{

		}
		if (rb2d != null)
		{
			puffinToMouse = mousePos2d - rb2d.position; 
		}
		puffintoMouseDirection = puffinToMouse.normalized;
		//this sets the puffin
		/*if(!Input.GetMouseButton (0) ) //(Input.touchCount == 0 || (!Input.GetMouseButton (0) && !Input.GetMouseButton (1)))
		{
			mousePos = rb2d.transform.position;//my attempt to make the puffin reference the previous click when swimming.
			puffinToMouse = Vector2.zero;
			puffintoMouseDirection = Vector2.zero;
		}*/

	

		//first game state. This checks to see if Puffin is clicked on
		//this makes puffin swim towards click



		//this makes it so the direction doesn't change based on puffin postion
		//puffinToMouse = mousePos2d - firstClick; 

		//print (puffinToMouse.magnitude);
	

		if (Input.GetMouseButtonDown (0))
		{
			//print ("Mouse Clicked");

			//firstClick = mousePos2d;

			int layerMask = 1 << 11; // this makes it
			RaycastHit2D hit = Physics2D.Raycast (mousePos2d, Vector2.zero, Mathf.Infinity, layerMask);
			if (hit.collider != null)
			{
				print ("You clicked on " + hit.collider.gameObject.name);
				//clickedObject = hit.collider.gameObject;

				//print (clickedObject.name);
			}


			/*if (GameManager.instance.gameState == GameManager.GameState.start)//hit.collider.gameObject.name == "Puffin") //I took away the need to click on puffin to start
			{
				
				//firstClick = mousePos2d;
				//print (firstClick);
				
				//rb2d = clickedObject.GetComponent<Rigidbody2D>();
				//rb2d.velocity = new Vector2 (0f, -300f);
				//rayToMouse = new Ray2D (mousePos2d,   (mousePos2d - rb2d.position));
				//print (rb2d.name);
				GameManager.instance.panning = false;
				GameManager.instance.gameState = GameManager.GameState.aiming;



			} else if (hit.collider.gameObject.name != "Puffin")
			{

				//I'm deactivating panning to test it here
				//GameManager.instance.panning = true;
			}*/

		}
	


		//second game state to handle aiming and release of Puffin
		if (GameManager.instance.gameState == GameManager.GameState.aiming && Input.touchCount == 1)
		{

			Aiming ();

		}

		#if UNITY_EDITOR
		if (GameManager.instance.gameState == GameManager.GameState.aiming)
		{

			Aiming ();

		}

		#endif
	}

	void FixedUpdate()
	{


	}

	public void Aiming()
	{
		//rb2d.position

		//Vector2 puffinToMouse = mousePos2d - firstClick; 
		float angle = Mathf.Atan2 (puffinToMouse.y, puffinToMouse.x) * Mathf.Rad2Deg;

		rb2d.MoveRotation(Mathf.LerpAngle(rb2d.rotation, angle, 5f*Time.deltaTime));
		//rayToMouse = new Ray2D (mousePos2d,   (puffinToMouse));
		//transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		//print(Mathf.Abs(angle));
		//print (rb2d.rotation);


		
		//print("Mouse Pos = " +mousePos+" This was where first clicked : " + firstClick+ "This is mouse pos - first click " + puffinToMouse);
		 
		if (Input.GetMouseButtonUp(0))
		{
			rb2d.isKinematic = false;
			// I took away the launch force as this happens in puffin control

			//this clamp Magnitude feature makes a max Vector length, insuring puffin is not launched too fast.
			rb2d.velocity =  Vector2.ClampMagnitude(puffinToMouse, 20f) * launchSpeed;

			GameManager.instance.gameState = GameManager.GameState.diving;

		}


	}
}
