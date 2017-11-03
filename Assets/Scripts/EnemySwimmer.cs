using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwimmer : MonoBehaviour {

	//private CircleCollider2D enemyZone; //parented circle collider that determines approach distance
	private Rigidbody2D rb2d; 
	private GameObject puffin;

	private Vector2 direction;
	private Vector2 startPosition;
	public float patrolSpeed = 5f;
	public float pursuitSpeed = 15f;
	public float pursuitRange = 20f;
	private Vector2 enemyToPuffin;
	private float idleTimer = 10f;

	private bool enemyInWater = true;
	private bool enemyInAir = false;
	private bool changeDir = false;

	private SpriteRenderer spriteRenderer;

	// the enemy checks if puffin is in radius. If so pursue. If out of radius for a certain time

	public enum waterEnemyState
	{
		idle,
		patrol,
		pursue,
		goHome,
		waterSurface,
	}
	public waterEnemyState state = waterEnemyState.patrol;


	// Use this for initialization
	void Start () 
	{
		rb2d = GetComponent<Rigidbody2D> ();	
		//enemyZone = GetComponentInChildren<CircleCollider2D>();
		startPosition = transform.position;
		direction = patrolDirection ();//new Vector2(1f,0f);
		spriteRenderer = GetComponent<SpriteRenderer> ();

		puffin = GameObject.Find ("Puffin");
	}

	// Update is called once per frame
	void Update () 
	{
		idleTimer = (idleTimer > 0 && (Vector2)transform.position != startPosition)? idleTimer - Time.deltaTime : 0f;
		enemyToPuffin = (puffin.transform.position - transform.position);


		flipSprite ();

		if (enemyToPuffin.sqrMagnitude < pursuitRange * pursuitRange && enemyInWater && GameManager.instance.score>0)
		{

			state = setEnemyState (waterEnemyState.pursue);
			idleTimer = 10f;
			//print ("pursue");
			//print (enemyToPuffin.sqrMagnitude);
		}


		if (idleTimer == 0f && ((Vector2)transform.position - startPosition).sqrMagnitude > 5f)			
		{


			//state = setEnemyState (waterEnemyState.goHome);
		}

		/*if (enemyInWater)
		{
			state = setEnemyState (airEnemyState.idle);

		}*/
		switch (state)
		{
		case waterEnemyState.idle:
			rb2d.velocity = Vector2.zero;
			if (idleTimer == 0 && ((Vector2)transform.position - startPosition).sqrMagnitude > 5f*5f)
			{
				state = setEnemyState (waterEnemyState.goHome);
			}
			break;

		case waterEnemyState.patrol:
			rb2d.velocity = direction * patrolSpeed;
			if (((Vector2)transform.position - startPosition).sqrMagnitude > 10f * 10f && changeDir == false)
			{
				direction = -direction;
				//print ("Switch");
				changeDir = true;
				//startPosition = rb2d.position;
			}
			if (enemyInAir)
			{
				state = setEnemyState (waterEnemyState.goHome);
				//direction.y = 1;
			}
			if (((Vector2)transform.position - startPosition).sqrMagnitude < 10f * 10f)
			{
				changeDir = false;
			}
			break;
		case waterEnemyState.pursue: // right now it is normalized. Try to think of another way to get direction, this is expensive?
			direction = enemyToPuffin.normalized;
			rb2d.velocity = direction * pursuitSpeed;
			idleTimer = 5f;
			if (enemyToPuffin.sqrMagnitude > pursuitRange * pursuitRange || GameManager.instance.score== 0)
			{
				state = setEnemyState (waterEnemyState.idle);
			}

			if (enemyInAir)
			{
				state = setEnemyState (waterEnemyState.waterSurface);
			}
			break;

		case waterEnemyState.goHome:
			direction = (startPosition - (Vector2)transform.position).normalized;
			rb2d.velocity = direction*pursuitSpeed;
			if (((Vector2)transform.position - startPosition).sqrMagnitude <1f)//((Vector2)transform.position - startPosition).sqrMagnitude <5f) //(Vector2)transform.position == startPosition   
			{
				state = setEnemyState (waterEnemyState.patrol);
			}
			break;


		case waterEnemyState.waterSurface:
			
			if (enemyToPuffin.sqrMagnitude < pursuitRange * pursuitRange && enemyToPuffin.y < 0 && GameManager.instance.score>0)
			{
				state = setEnemyState (waterEnemyState.pursue);
			}
			else if(idleTimer ==0f)
			{
				state = setEnemyState (waterEnemyState.goHome);
			}
			break;	
		}
	}


	public waterEnemyState setEnemyState ( waterEnemyState newState)
	{
		switch (newState)
		{
		case waterEnemyState.idle:
			//rb2d.gravityScale = 0f;
			rb2d.velocity = Vector2.zero;
			break;

		case waterEnemyState.patrol:
			startPosition = rb2d.position;
			direction = patrolDirection ();
			rb2d.velocity = direction * patrolSpeed;
			break;

		case waterEnemyState.pursue:
			idleTimer = 10f;
			break;

		case waterEnemyState.goHome:
			break;

		case waterEnemyState.waterSurface:
			if (enemyToPuffin.y > 0)
			{
				Vector2 noY = new Vector2 (rb2d.velocity.x, 0f);
				rb2d.velocity = noY;
			}
			break;	

		}


		return newState;
	}

	public Vector2 patrolDirection()
	{
		Vector2 Rand = new Vector2 (UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
		return Rand.normalized;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		/*
		if (other.gameObject.CompareTag("Player") && enemyInAir)
		{
			print ("Pursue!");
			state = setEnemyState (airEnemyState.pursue);
			idleTimer = 10f;

		}*/

		if (other.name == "Water")
		{
			enemyInWater = true;

		}
		if (other.name == "Air")
		{
			enemyInAir = true;

		}

	}
	void OnTriggerExit2D(Collider2D other)
	{
		/*if (other.gameObject.CompareTag("Player") )
		{
			state = setEnemyState (airEnemyState.idle);

		}*/

		if (other.name == "Water")
		{
			enemyInWater = false;
		}

		if (other.name == "Air")
		{
			enemyInAir = false;

		}
	}

	public void flipSprite()
	{
		if (state == waterEnemyState.pursue)
		{
			if (enemyToPuffin.x < 0)
			{
				spriteRenderer.flipX = true;
			} else if (enemyToPuffin.x > 0)
			{
				spriteRenderer.flipX = false;
			}
		} else if (direction.x < 0)
		{
			spriteRenderer.flipX = true;
		}else if (direction.x > 0)
		{
			spriteRenderer.flipX = false;
		}


	}
}
