using UnityEngine;
using System.Collections;

public class ParachuteScript : MonoBehaviour {

    public bool parachuteEnabled = false;

    //this is needed for the last level
    //if we are following the jelly we cannot destroy the parachute
    //even when outside the camera (might be positioning itself)
	SmoothFollow2D scriptFollow;
	CameraZoomInOutScript zoomScript;
	GameObject myCamera;

	public Sprite failSafeParachute;
	public bool isFailsafe = false;

	// Use this for initialization
	void Start () {
	  myCamera = GameObject.FindGameObjectWithTag("MainCamera");
	  if(myCamera!=null) {
		 scriptFollow = myCamera.GetComponent<SmoothFollow2D>();
		 zoomScript = myCamera.GetComponent<CameraZoomInOutScript>();
	  }
	
	}
	
	// Update is called once per frame
	void Update () {

	 gameObject.GetComponent<Renderer>().enabled = parachuteEnabled;
	 if(GetComponent<Renderer>().enabled && scriptFollow==null) {

	    if(myCamera!=null) {

		     if(zoomScript!=null) {
		     	//if not visible and i´m not zooming, than destroy the parachute, otherwise keep it
				if (GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false && !zoomScript.IsCameraZoming()) {		
					Destroy(gameObject);
				}
		     }
			 else {
				if (GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false) {
			
					Destroy(gameObject);
				}
			 }
	    }//cannot find the camera object
	    else {
			if (GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false) {
				Destroy(gameObject);
			}
	    }


		
	 }
	 
		
	  
	}
	
	public void Release() {
	  SpecialEffectsHelper.Instance.PlayParachuteReleaseEffect(transform.position);
	  if(isFailsafe) {
	   //destroy completely
		Destroy(gameObject);
	  }
	  else {
	    //will avoid rendering
	    parachuteEnabled = false;
	    //change sprite for when rendering will be enabled again
		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		if(renderer!=null) {
			renderer.sprite = failSafeParachute;
		}
						
	    //just disable the renderer and the collider
	    gameObject.GetComponent<Collider2D>().enabled = false;
	  }



	}

	public void LaunchFailsafe() {

	      isFailsafe=true;
		  //enable rendering again
		  parachuteEnabled = true;
		  //enable collider
		  gameObject.GetComponent<Collider2D>().enabled = true;
	}

	public void SetFailsafe(bool failsafe) {
	  isFailsafe = failsafe;
	}



}
