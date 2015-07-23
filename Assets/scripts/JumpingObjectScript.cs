using UnityEngine;
using System.Collections;

public class JumpingObjectScript : MonoBehaviour {

 //todo this is an object for which we apply force
	public Vector2 force = new Vector2(0f,0f);
	public bool startDelayed = false;
	public float delay = 0f;
	// Use this for initialization
	void Start () {

	 if(startDelayed && delay>0) {
	  Invoke("ApplyForce",delay);
	 }
	else {
      ApplyForce();
	}
	   
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ApplyForce() {
	  transform.GetComponent<Rigidbody2D>().AddForce(force);
	}
}
