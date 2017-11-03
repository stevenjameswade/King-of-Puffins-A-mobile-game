using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[RequireComponent(typeof(Camera))]
	public class CameraControl : MonoBehaviour 
	{
		public GameObject target; //public Renderer target;
		public Rect deadzone;
		public Vector3 smoothPos;

		public Rect[] limits;

		protected Camera _camera;
		protected Vector3 _currentVelocity;


		private Vector2 deadZoneposition;
		private Vector3 touchMove;
		private Vector3 constraint;


		



		public float perspectiveZoomSpeed = .5f;
		public float orthoZoomSpeed = .5f;
		public float panSpeed = .01f;

		private float maxZoom;
	public Rect camWorldRect;


		

		public void Start()
		{
		target = ClickManager.instance.puffin;

		//Screen.SetResolution (1280, 720, true);
			smoothPos = target.transform.position;
			smoothPos.z = transform.position.z;
			_currentVelocity = Vector3.zero;

			constraint = target.transform.position;

			_camera = GetComponent<Camera>();
			if(!_camera.orthographic)
			{
				Debug.LogError("deadzone script require an orthographic camera!");
				Destroy(this);
			}

		//maxZoom = 
		}

	public void Update()
	{

		if(Input.GetKeyDown(KeyCode.Space))
		{
			print(Camera.main.orthographicSize);
		}
		/*if (GameManager.instance.gameStart && Input.GetTouch (0).phase == TouchPhase.Moved && Input.touchCount == 1)
		{
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			touchMove = new Vector3 (-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0f);
			GameManager.instance.panning = true;
		}*/
		if (target != null)
		{
			constraint = target.transform.position;
		} else
		{
			try
			{
				target = ClickManager.instance.puffin;

			} 
			catch
			{
				print ("NullReference to puffin");
			}
		}
				
				

		/*try
		{
			constraint = target.transform.position;
			
		} 
		catch (NullReferenceException ex)
		{
			print ("NullReference");
			target = ClickManager.instance.puffin;
			constraint = target.transform.position;
		}
		*/

		//constraint = !GameManager.instance.panning ? target.transform.position : touchMove;
		//print (constraint + " target: "+target.transform.position);


		float localX = constraint.x - transform.position.x;
		float localY = constraint.y - transform.position.y;
		if (!GameManager.instance.panning) //checks to see if moving camera, if so, no dead zone
		{
			if (localX < deadzone.xMin)
			{
				smoothPos.x += localX - deadzone.xMin;
			} else if (localX > deadzone.xMax)
			{
				smoothPos.x += localX - deadzone.xMax;
			}

			if (localY < deadzone.yMin)
			{
				smoothPos.y += localY - deadzone.yMin;
			} else if (localY > deadzone.yMax)
			{
				smoothPos.y += localY - deadzone.yMax;
			}
		} 
		/*else if (GameManager.instance.panning && Input.GetMouseButton(0) )
		{
			smoothPos = ClickManager.instance.mousePos2d * panSpeed;
			GameManager.instance.panning = true;
		}*/
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && GameManager.instance.panning)
		{
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			smoothPos= (new Vector3 (transform.position.x -(touchDeltaPosition.x * panSpeed),transform.position.y -(touchDeltaPosition.y * panSpeed), 0f));
			GameManager.instance.panning = true;

		}

	
			//Rect camWorldRect = new Rect();
			camWorldRect.min = new Vector2(smoothPos.x - _camera.aspect * _camera.orthographicSize, smoothPos.y - _camera.orthographicSize);
			camWorldRect.max = new Vector2(smoothPos.x + _camera.aspect * _camera.orthographicSize, smoothPos.y + _camera.orthographicSize);

			for (int i = 0; i < limits.Length; ++i)
			{

				Vector3 localOffsetMin = limits [i].min + camWorldRect.size * 0.5f;
				Vector3 localOffsetMax = limits [i].max - camWorldRect.size * 0.5f;
			if (limits [i].Contains (constraint))
				{

					localOffsetMin.z = localOffsetMax.z = smoothPos.z = -10f;

					smoothPos = Vector3.Max (smoothPos, localOffsetMin);
					smoothPos = Vector3.Min (smoothPos, localOffsetMax);

					break;
			} else if (!limits [i].Contains (constraint))//check to see if the camera is no longer in the limits, if so camera position is the last position
				{
				localOffsetMin.z = localOffsetMax.z = smoothPos.z;

					smoothPos = Vector3.Max (smoothPos, localOffsetMin);
					smoothPos = Vector3.Min (smoothPos, localOffsetMax);
				}
			}
	
			//try to constrain panning in box
		/*
			if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved && GameManager.instance.gameStart == true)
			{
				Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			  touchMove = new Vector3 (-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0f);

			}
			*/

				Vector3 current = transform.position;
				current.x = smoothPos.x; // we don't smooth horizontal movement
				current.y = smoothPos.y; // I added this because the vertical movement was choppy
				current.z = -10f;
	
			transform.position = Vector3.SmoothDamp (current, smoothPos, ref _currentVelocity, 0.1f);

	
	


			// This is now code for panning the camera with touch
			//touch panning
		/*	if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved && GameManager.instance.gameStart == true)
			{
				Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
				transform.Translate (new Vector3 (-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0f));

			} */
			/* This is unsuccessfulelse if (Input.GetMouseButton (0) && GameManager.instance == true)
			{
				
				Vector2 mouseposition = _camera.ScreenToWorldPoint(Input.mousePosition);
				Rect mousespecialrect = new Rect();
				mousespecialrect.min = new Vector2(mouseposition.x - _camera.aspect * _camera.orthographicSize, mouseposition.y - _camera.orthographicSize);
				mousespecialrect.max = new Vector2(mouseposition.x + _camera.aspect * _camera.orthographicSize, mouseposition.y + _camera.orthographicSize);

				for (int i = 0; i < limits.Length; ++i)
				{
					if (limits[i].Contains(target.transform.position))
					{
						Vector3 localOffsetMin = limits[i].min + mousespecialrect.size * 0.5f;
						Vector3 localOffsetMax = limits[i].max - mousespecialrect.size * 0.5f;

						localOffsetMin.z = localOffsetMax.z = smoothPos.z;

						mouseposition = Vector3.Max(mouseposition, localOffsetMin);
						mouseposition = Vector3.Min(mouseposition, localOffsetMax);

						break;
					}
				}
				transform.Translate (mouseposition);

				//transform.Translate (new Vector3 (-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0f));
			}*/

			//this is code for zooming
			//two finger zooming control
			if (Input.touchCount == 3)// normally 2, I switched to 3 because I was testing boostin
			{
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;

				if (Camera.main.orthographic)
				{
					Camera.main.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;
					Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 6.57f, 43.37006f);
					deadzone.size = (new Vector2( Camera.main.orthographicSize* Camera.main.aspect *.45f, Camera.main.orthographicSize*.3f));
					deadZoneposition = (new Vector2 (-deadzone.width / 2, -deadzone.height / 2));
					deadzone.position = deadZoneposition;
				} else
				{
					Camera.main.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
					Camera.main.fieldOfView = Mathf.Clamp (Camera.main.fieldOfView, 6.57f, 20.61f);
				}

			} //else 
			if (Input.GetAxis ("Mouse ScrollWheel") != 0f)
			{
				float scrollSpeed = 10* -Input.GetAxis ("Mouse ScrollWheel");
				if (Camera.main.orthographic)
				{
					Camera.main.orthographicSize += scrollSpeed * orthoZoomSpeed*10f;
					//Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 6.57f);
					Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 6.57f, 43.37006f);
				// make this controllable outside of the script
				deadzone.size = (new Vector2( Camera.main.orthographicSize* Camera.main.aspect *.45f, Camera.main.orthographicSize*.3f));//Camera.main.aspect *1.25f, Camera.main.orthographicSize*.8f
					//Vector3 deadzoneCenter = Camera.main.ViewportToWorldPoint (new Vector3 (.5f, .5f, 0f));
					deadZoneposition = (new Vector2 (-deadzone.width / 2, -deadzone.height / 2));
					deadzone.position = deadZoneposition;
					//target.gameObject.GetComponent<Rigidbody2D>().position = deadzoneCenter;
				//	print (deadZoneposition);
				} else
				{
					Camera.main.fieldOfView += scrollSpeed * perspectiveZoomSpeed;
					Camera.main.fieldOfView = Mathf.Clamp (Camera.main.fieldOfView, 6.57f, 20.61f);
				}
			}
			//deadzone.size = (new Vector2( Camera.main.orthographicSize* Camera.main.aspect *1.5f, Camera.main.orthographicSize));

			}
	}
	
	#if UNITY_EDITOR

	[CustomEditor(typeof(CameraControl))]
	public class CamDeadZonEditor : Editor
	{
		public void OnSceneGUI()
		{
		CameraControl cam = target as CameraControl;

			Vector3[] vert = 
			{
				cam.transform.position + new Vector3(cam.deadzone.xMin, cam.deadzone.yMin, 0),
				cam.transform.position + new Vector3(cam.deadzone.xMax, cam.deadzone.yMin, 0),
				cam.transform.position + new Vector3(cam.deadzone.xMax, cam.deadzone.yMax, 0),
				cam.transform.position + new Vector3(cam.deadzone.xMin, cam.deadzone.yMax, 0)
			};

			Color transp = new Color(0, 0, 0, 0);
			Handles.DrawSolidRectangleWithOutline(vert, transp, Color.red);

			for(int i = 0; i < cam.limits.Length; ++i)
			{
				Vector3[] vertLimit =
				{
					new Vector3(cam.limits[i].xMin, cam.limits[i].yMin, 0),
					new Vector3(cam.limits[i].xMax, cam.limits[i].yMin, 0),
					new Vector3(cam.limits[i].xMax, cam.limits[i].yMax, 0),
					new Vector3(cam.limits[i].xMin, cam.limits[i].yMax, 0)
				};

				Handles.DrawSolidRectangleWithOutline(vertLimit, transp, Color.green);
			}


		//I added this to debug what this rect is and where it is placed
		Vector3[] camWorld = 
		{
			
			cam.transform.position + new Vector3(cam.camWorldRect.xMin, cam.camWorldRect.yMin, 0),
			cam.transform.position + new Vector3(cam.camWorldRect.xMax, cam.camWorldRect.yMin, 0),
			cam.transform.position + new Vector3(cam.camWorldRect.xMax, cam.camWorldRect.yMax, 0),
			cam.transform.position + new Vector3(cam.camWorldRect.xMin, cam.camWorldRect.yMax, 0)
		};

		Handles.DrawSolidRectangleWithOutline(camWorld, transp, Color.black);
		}


	}
	#endif