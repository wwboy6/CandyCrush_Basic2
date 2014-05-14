using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	void Start () {
		
	}
	
	
	void OnMouseDown () 
	{
		Application.LoadLevel(1);
		Debug.Log("Game Start.");
		
	}
}
