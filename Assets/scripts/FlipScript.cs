using UnityEngine;
using System.Collections;

public class FlipScript : MonoBehaviour {

    public bool flipHorizontal = false;
    public float flipInterval = 3.0f;
    private bool flip = false;
    private float currentTime = 0f;

    //private Vector3 flipVerticalVector = new Vector3(0f,-1f,0f);
	//private Vector3 flipHorizontalVector = new Vector3(-1f,0f,0f);
	// Use this for initialization
	void Start () {

	   currentTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

	  currentTime+=Time.deltaTime;
	  if(currentTime>flipInterval) {
	   flip = !flip;

	  }

	  if(flip) {
		currentTime = 0f;
		Vector3 flipVector = gameObject.transform.localScale;
		//Vector3 pos = gameObject.transform.position;//was local position
	    if(flipHorizontal) {
	       flipVector.x*=-1;
	    }
	    else {
	    //vertical
			flipVector.y*=-1;
	    }
		gameObject.transform.localScale = flipVector;
		//pos.y-=gameObject.transform.renderer.bounds.size.y;
		//gameObject.transform.position = pos;
		flip =  false;
	  }
	
	}
}
