using UnityEngine;
using System.Collections;

public class ClosingWallScript : MonoBehaviour {

	// Use this for initialization
	private	MoveScript wallMove = null;
	void Start () {
		wallMove = gameObject.GetComponent<MoveScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	  void OnCollisionEnter2D(Collision2D collision) {
	
	   //just handle collision with another ClosingWallScript, stop them!
	   if(collision.gameObject.GetComponent<ClosingWallScript>()!=null) {
		   MoveScript otherWalll = collision.gameObject.GetComponent<MoveScript>();
		   if(otherWalll!=null) {
		     otherWalll.enabled = false;
		   }

		   if(wallMove!=null) {
		     wallMove.enabled = false;
		   }
		}

	  }
}
