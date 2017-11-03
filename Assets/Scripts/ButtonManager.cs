using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void LoadLevel(string str)
	{
		GameManager.instance.LoadLevel(str);
	}

	public void LoadLevel(int ind)
	{
		GameManager.instance.LoadLevel(ind);
	}

	public void LoadLevel() // this loads the level by the the name of the button
	{
		print (EventSystem.current.currentSelectedGameObject.name);
		GameManager.instance.LoadLevel (EventSystem.current.currentSelectedGameObject.name);
	}
}
