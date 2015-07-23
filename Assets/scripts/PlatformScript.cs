using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {

	public float rotationDegreesPerSecond = 25f;
	public float rotationDegreesAmount = 45f;
	private float totalRotation = 0;
	
	
    public bool isLeft = false;
    private bool isOpening = false;
    private bool isClosing = false;
    
    private bool rotated = false;
	// Use this for initialization

	private GameControllerScript controller;
	void Start () {
		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		controller = scripts.GetComponent<GameControllerScript>();
	}
	
	// Update is called once per frame
	/*void Update () {
	  if(isOpening) {
			// Spin the object around the world origin at 20 degrees/second.
			//transform.RotateAround (transform.position, Vector3.down, 20 * Time.deltaTime);
		 if(!rotated) {
				//transform.Rotate(0, 90, 0);
				if(isLeft) {
					transform.Rotate (0, 0,-90.0f * Time.deltaTime);
				}
				else {
					transform.Rotate (0, 0,90.0f * Time.deltaTime);
				}
				
		 }
			
	  }
	  else {
			// Spin the object around the world origin at 20 degrees/second.
			//transform.RotateAround (transform.position, Vector3.down, -20 * Time.deltaTime);
	  }
	  
		
	}*/
	
	public void Open() {
		isOpening = true;
		isClosing = false;
	
	}
	
	public void Close() {
		isClosing = true;
		isOpening = false;
		
	}


	
	// Update is called once per frame
	void Update () {
		//if we haven't reached the desired rotation, swing
	  if(isOpening && !isClosing) {
			if(Mathf.Abs(totalRotation) < Mathf.Abs(rotationDegreesAmount)) {
				SwingOpen();
			}
			else {
			  isOpening = false;
			  rotated = true;
			  totalRotation = 0;			  
			  
			  //already open, jump now
				GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
				if(jelly!=null) {
					JellyScript script = jelly.GetComponent<JellyScript>();
					if(script!=null && !controller.isJellyFalling) {
						script.Jump();
						//maybe enable collider here?;
					}
				}
				
				
			}
				
	  }
	  else if(rotated && !isClosing) {
			GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
			if(jelly!=null) {
			  MoveScript move = jelly.GetComponent<MoveScript>();
			  if(move!=null) {
			    move.enabled = true;
			  }
			  ParachuteScript parachute = jelly.GetComponent<ParachuteScript>();
			  if(parachute!=null) {
			     parachute.parachuteEnabled = true;
			  }
			}
	  }
	  else if(isClosing && !isOpening) {
	  
			/*Debug.Log("Mathf.Abs(totalRotation): " + Mathf.Abs(totalRotation));
			Debug.Log("Mathf.Abs(rotationDegreesAmount): " +Mathf.Abs(rotationDegreesAmount));

			float currentAngle = transform.rotation.eulerAngles.z;*/
			if(Mathf.Abs(totalRotation) < Mathf.Abs(rotationDegreesAmount)) {
				SwingClose();
			}
			else {
				isClosing = false;
				rotated = true;
				totalRotation = 0;
				//notify closure, because of the release click TODO... not exactly here
				controller.PlatformClosed();
			}
	  }
	  
		
	}
	
	void SwingOpen()
	{   
		float currentAngle = transform.rotation.eulerAngles.z;
		if(isLeft) {
			transform.rotation = 
				Quaternion.AngleAxis(currentAngle + (Time.deltaTime * rotationDegreesPerSecond),Vector3.forward);
			totalRotation -= Time.deltaTime * rotationDegreesPerSecond;
		}
		else {
			transform.rotation = 
				Quaternion.AngleAxis(currentAngle + (Time.deltaTime * rotationDegreesPerSecond),  Vector3.forward);
			totalRotation += Time.deltaTime * rotationDegreesPerSecond;
		}
	
		/**
		back	Shorthand for writing Vector3(0, 0, -1).
down	Shorthand for writing Vector3(0, -1, 0).
forward	Shorthand for writing Vector3(0, 0, 1).
		*/
	}
	
	void SwingClose()
	{   

		
		//rotationDegreesAmount*=-1;
		//rotationDegreesPerSecond*=-1;
		
		float currentAngle = transform.rotation.eulerAngles.z;
		
		if(isLeft) {
			transform.rotation = 
				Quaternion.AngleAxis(currentAngle - (Time.deltaTime * rotationDegreesPerSecond),Vector3.forward);
			totalRotation += Time.deltaTime * rotationDegreesPerSecond;

		}
		else {
			transform.rotation = 
				Quaternion.AngleAxis(currentAngle - (Time.deltaTime * rotationDegreesPerSecond),  Vector3.forward);
			totalRotation -= Time.deltaTime * rotationDegreesPerSecond;
		}
		
		
		/**
		back	Shorthand for writing Vector3(0, 0, -1).
down	Shorthand for writing Vector3(0, -1, 0).
forward	Shorthand for writing Vector3(0, 0, 1).
		*/
	}
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		
	}
	

}
