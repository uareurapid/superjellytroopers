using UnityEngine;
using System.Collections;

public class CameraZoomInOutScript : MonoBehaviour {

    public Transform target;
	public float cameraMin = 5f;
	public float cameraMax = 10f;

	public bool isCameraZoomingIn = false;
	public bool isCameraZoomingOut = false;

	private bool isZoomComplete = false;
	// Use this for initialization

	Vector3 targetPosition;
	Vector3 velocity = Vector3.zero;
	public float timeToReachTarget = 4f; //seconds

	private float speed = 1.0f;
    private float cameraOriginalOrthographicSize = 0f;
    private Vector3 cameraOriginalPosition;
    //minimum distance between the camera and the object
    public float minDistance = 1f;

    public bool showMessageOnZoom = false;
	public string messageKey = "What is this thing in the back???"; 

	Texture2D helpMeTexture;

    private GUISkin skin;


	Vector3 damagerVelocity = Vector3.zero;

	void Start () {
	  // save the current values
	  cameraOriginalOrthographicSize = Camera.main.orthographicSize;
	  cameraOriginalPosition = Camera.main.transform.position; //or local position??
	  skin = Resources.Load("GUISkin") as GUISkin;
	  helpMeTexture = Resources.Load("tapme") as Texture2D;
	  Invoke("ZoomIN",4f);
	  Invoke("ZoomOUT",15f);
				//Invoke("ZoomIN2",4f);		
	}

	void Awake() {
		GUIResolutionHelper.Instance.CheckScreenResolution();
	}


	void FixedUpdate() {
	    //only move camera if zoom already stopped
		if(isCameraZoomingIn ) {//&& !isZoomComplete
			transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, timeToReachTarget);
						//http://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html
		}
		else if(isCameraZoomingOut) {
			transform.position = Vector3.SmoothDamp(transform.position, cameraOriginalPosition, ref velocity, timeToReachTarget);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	//OK

	 Vector3 transformPosition = transform.position;

	 //transformPosition.y = target.position.y;
	 //transformPosition.x = target.position.x;
	 //transformPosition.z = target.position.z - 2f;
	 //transform.position = transformPosition;

	//ZOOM IN
		if(Camera.main.orthographicSize > cameraMin && isCameraZoomingIn) {

			 Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraMin, Time.deltaTime * speed);

		}
		//ZOOM OUT, was cameraMax
		else if(Camera.main.orthographicSize < cameraOriginalOrthographicSize && isCameraZoomingOut) {
			 
		     Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraOriginalOrthographicSize, Time.deltaTime * speed);

			 if( Mathf.Abs(Camera.main.orthographicSize - cameraOriginalOrthographicSize) < 0.00015 ){

				isCameraZoomingOut = false;
				isCameraZoomingIn = false;
	        	isZoomComplete = true;
				EnableSpawn();
				EnableJellyMovement();
				EnablePunchMovement();
		     }
		}
	    else {
	  
	       if(isCameraZoomingIn) {
	        isCameraZoomingIn = false;
	        isZoomComplete = true;
	        isCameraZoomingOut = false;
	        //at this point i can move towards the object
	
	       }
	   
	    }

   }
	
	void DisableJellyMovement() {
	 GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
	 if(jelly!=null) {
	   MoveScript movement = jelly.GetComponent<MoveScript>();
	   if(movement!=null) {
	     movement.enabled = false;
	   }
	   jelly.GetComponent<Rigidbody2D>().isKinematic = true;
	 }
	}

	void DisablePunchMovement() {
	 GameObject []punch = GameObject.FindGameObjectsWithTag("Damager");
	 foreach(GameObject obj in punch) {
	
	   PunchScript movement = obj.GetComponent<PunchScript>();
	   if(movement!=null) {
	     movement.enabled = false;
	     obj.GetComponent<Rigidbody2D>().isKinematic=true;
	     //save to put back
	     damagerVelocity = obj.GetComponent<Rigidbody2D>().velocity;
	     obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	   }
	 }
	}

	void EnablePunchMovement() {
	 GameObject []punch = GameObject.FindGameObjectsWithTag("Damager");
	 foreach(GameObject obj in punch) {

	   PunchScript movement = obj.GetComponent<PunchScript>();
	   if(movement!=null) {
	     movement.enabled = true;
	     obj.GetComponent<Rigidbody2D>().isKinematic=true;
	     obj.GetComponent<Rigidbody2D>().velocity = damagerVelocity;
	   }
	 }
	}

	void EnableJellyMovement() {
		GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
		if(jelly!=null) {
			MoveScript movement = jelly.GetComponent<MoveScript>();
			if(movement!=null) {
			    movement.enabled = true;
			}
			jelly.GetComponent<Rigidbody2D>().isKinematic = false;
		}
	}
		

	void EnableSpawn() {
	 GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
	 if(spawner!=null) {
	  SpawnerScript script = spawner.GetComponent<SpawnerScript>();
	  if(script!=null) {
		script.PutSpawnOnHold(false);
	  }
	 }
	}

	void DisableSpawn() {
	 GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
	 if(spawner!=null) {
	  SpawnerScript script = spawner.GetComponent<SpawnerScript>();
	  if(script!=null) {
	    script.PutSpawnOnHold(true);
	  }
	 }
	}

	public void ZoomIN() {

	 isCameraZoomingIn = true;
	 isCameraZoomingOut = false;
	 isZoomComplete = false;
	 DisableJellyMovement();
	 DisablePunchMovement();
	 DisableSpawn();

	}

	public void ZoomOUT() {
	isCameraZoomingOut = true;
	isCameraZoomingIn = false;
	isZoomComplete = false;
	Debug.Log("START ZOOM OUT");


  }

  void OnGUI() {

      if(isCameraZoomingIn) {
		GUI.skin = skin;
		
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		
	    int width = GUIResolutionHelper.Instance.screenWidth;
		int height = GUIResolutionHelper.Instance.screenHeight;
		Vector3 scaleVector = GUIResolutionHelper.Instance.scaleVector;
		
		bool isWideScreen = GUIResolutionHelper.Instance.isWidescreen;
		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (GUIResolutionHelper.Instance.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			
			
		}
		else {
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
			
		}

		if(Event.current.type==EventType.Repaint) {

		}

		if(showMessageOnZoom && messageKey!=null) {
			GUI.Label (new Rect(width/2-190, height/2-200, 400, 50), "What is this thing on the back???");
		    Rect helpMeTextureRect = new Rect(width / 3 - 180,height/2-200,64,64);
		    GUI.DrawTexture(helpMeTextureRect, helpMeTexture);
		}

		GUI.matrix = svMat;

						
      }
		
  }

  public bool IsCameraZoming() {
   return isCameraZoomingIn || isCameraZoomingOut;
  }

/*
    Smooth zoom in
    using UnityEngine;
 using System.Collections;
 
 public class ZoomInOut : MonoBehaviour 
 {
     public float distance;
     private float sensitivityDistance = -7.5f;
     private float damping = 2.5f;
     private float min = -15f;
     private float max = -80f;
     private Vector3 zdistance;
     
     void  Start ()
     {
         distance = -20f;
         distance = transform.localPosition.z;
     }
     void  Update ()
     {
         distance -= Input.GetAxis("Mouse ScrollWheel") * sensitivityDistance;
         distance = Mathf.Clamp(distance, min, max);
         zdistance.z = Mathf.Lerp(transform.localPosition.z, distance, Time.deltaTime * damping);
         transform.localPosition = zdistance;
     }
 }
  */  
}
