using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	
	//with a negative value rotates counter clock
	public float rotateSpeed = 5f;
	public string angle="y";


	void Start() {
	 if(!angle.Equals("z") && !angle.Equals("y") && !angle.Equals("x")) {
	  //wrong angle/axis
	  angle = "z";
	 }
	}
	
	void Update () {

	    if(angle.Equals("y")){
			transform.Rotate (new Vector3(0, rotateSpeed*Time.deltaTime,0));
	    }
		else if(angle.Equals("x")){
			transform.Rotate (new Vector3(rotateSpeed*Time.deltaTime,0,0));
	    }
		else if(angle.Equals("z")){
			transform.Rotate (new Vector3(0, 0,rotateSpeed*Time.deltaTime));
	    }
	}
}
