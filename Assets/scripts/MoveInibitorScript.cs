using UnityEngine;
using System.Collections;

//enables/disables movement
public class MoveInibitorScript : MonoBehaviour {

    //time before activating other move script
    public float delayBeforeActivate = 0f;
    public bool activate = false;
	// Use this for initialization
	void Start () {

	   Invoke("EnableMovement",delayBeforeActivate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void EnableMovement() {
	  MoveScript move = GetComponent<MoveScript>();
	  if(move!=null) {
		move.enabled = activate;
	  }
	}
}
