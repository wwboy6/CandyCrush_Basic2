using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnMouseDown () 
    {
        Application.LoadLevel(0);
	
	}
}
