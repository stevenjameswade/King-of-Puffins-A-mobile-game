using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class CameraZoom : MonoBehaviour 
{
	public float perspectiveZoomSpeed = .5f;
	public float orthoZoomSpeed = .5f;
	public float panSpeed = .1f;

	//private Camera myCamera;

	void Start()
	{
		//myCamera = GetComponent<Camera> ();
	}


	void Update () 
	{
		//touch panning
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && GameManager.instance.gameState == GameManager.GameState.start )
		{
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			transform.Translate (new Vector3 (-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0f));
			GameManager.instance.panning = true;
			
		}


		//two finger zooming control
		if (Input.touchCount == 2)
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
			} else
			{
				Camera.main.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
				Camera.main.fieldOfView = Mathf.Clamp (Camera.main.fieldOfView, 6.57f, 20.61f);
			}

		} else if (Input.GetAxis ("Mouse ScrollWheel") != 0f)
		{
			float scrollSpeed = 10* -Input.GetAxis ("Mouse ScrollWheel");
			if (Camera.main.orthographic)
			{
					Camera.main.orthographicSize += scrollSpeed * orthoZoomSpeed;
				//Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 6.57f);
				Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 6.57f, 43.37006f);
			} else
			{
					Camera.main.fieldOfView += scrollSpeed * perspectiveZoomSpeed;
				Camera.main.fieldOfView = Mathf.Clamp (Camera.main.fieldOfView, 6.57f, 20.61f);
			}
		}
		
	}
}
