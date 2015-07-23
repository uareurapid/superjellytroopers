using UnityEngine;
using System.Collections;
/// <summary>
/// Simply moves the current game object
/// </summary>
public class PunchScript : MonoBehaviour
{
	// 1 - Designer variables
	
	/// <summary>
	/// Object speed
	/// </summary>
	public Vector2 speed = new Vector2(10, 10);
	
	/// <summary>
	/// Moving direction
	/// </summary>
	public Vector2 direction = new Vector2(-1, 0);
	
	private Vector2 movement;
	
	public Vector2 initialDirection = new Vector2(-1, 0);
	public Vector2 initialSpeed = new Vector2(10, 10);
	//go back at half initial speed
	public bool returnHalfSpeed = true;
	public bool startDelayed = false;
	public float delay = 0f;
	private bool start = true;

	public float distance = 100f;

	private bool revert = false;
	
	//initial movement position
	private Vector3 initialposition;
	private Vector3 revertPosition;
	//allows the object to move even if not visible
	//by default is always false, so must be explicit set to true
	//like on asteroid spawner (of level 1) for instance
	bool isVisible = false;
	
	void Start() {

		revert = false;
		initialposition = Camera.main.WorldToScreenPoint(transform.position);
		if(startDelayed && delay>0) {
		  start = false;
		  Invoke("StartMovement",delay);
		}
	}
	
	void Awake() {
		
	}

	void StartMovement() {
	 start = true;
	}
	
	void Update()
	{

	   if(start) {

	   //move in y axis??
	   bool moveInYAxis = initialDirection.y!=0 && initialDirection.x==0;


		Vector3 currentPosition = Camera.main.WorldToScreenPoint (transform.position);


		//normal move in X axis
		if(!moveInYAxis) {
		   MoveInXAxis(currentPosition);
		}
		else {
		   MoveInYAxis(currentPosition);
		}



			
		if(revert) {

		  //revert in X
		  if(!moveInYAxis) {
			direction.x = direction.x * -1;
			if(direction.x!=initialDirection.x) {
			//go back at half velocity
			 if(returnHalfSpeed) {
				speed.x = initialSpeed.x/2;
			 }
			  
			}
			else {
			//normal velocity to original direction
			  speed.x = initialSpeed.x;
			}
		    revert = false;
		  }
		  else {
			//revert in Y						
			direction.y = direction.y * -1;
			if(direction.y!=initialDirection.y) {
			//go back at half velocity
			 if(returnHalfSpeed) {
				speed.y = initialSpeed.y/2;
			 }
			  
			}
			else {
			//normal velocity to original direction
			  speed.y = initialSpeed.y;
			}
		    revert = false;
		  }


			
		}
		
		
		
		
		
		
		//transform.RotateAround(transform.position, Vector3.forward, 20 * Time.deltaTime);
		// 2 - Movement
		movement = new Vector2(speed.x * direction.x,speed.y * direction.y);
 
     }
					

	}
		
	void MoveInXAxis(Vector3 currentPosition) {

	 if(direction.x < 0 ) {
				//moving left
				
			if(!revert) {
				if(currentPosition.x <= initialposition.x - distance ) {
					revert = true;
					//adjustement to avoid go beyhond
					currentPosition.x = initialposition.x - distance;
					initialposition.x = currentPosition.x;
				}
	
			}
				
				
				
		}
		else if(direction.x > 0) {
				//moving right
			if(!revert) {
				if(currentPosition.x >= initialposition.x + distance) {
					revert = true;
					//adjustment
					currentPosition.x = initialposition.x + distance;
					initialposition.x = currentPosition.x;
				}

			}	
				
		}
	}	

	void MoveInYAxis(Vector3 currentPosition) {

	 if(direction.y < 0 ) {
				//moving down
				
			if(!revert) {
				if(currentPosition.y <= initialposition.y - distance ) {
					revert = true;
					//adjustement to avoid go beyhond
					currentPosition.y = initialposition.y - distance;
					initialposition.y = currentPosition.y;
				}
	
			}
				
				

		}
		else if(direction.y > 0) {
				//moving up
			if(!revert) {
				if(currentPosition.y >= initialposition.y + distance) {
					revert = true;
					//adjustment
					currentPosition.y = initialposition.y + distance;
					initialposition.y = currentPosition.y;
				}

			}	
				
		}
	}		
	
	void FixedUpdate()
	{
	  //we only move the enemy or asteroi if the player is on scene,
	  //otherwise we see a lot of space without enemies, more close to the end of the level
	  
		// Apply movement to the rigidbody
	    //check if in camera
	    if(start)
		  GetComponent<Rigidbody2D>().velocity = movement;
		
	  
		
	}
	
	
	
	void OnBecameVisible() {
		isVisible = true;

	}
	
	void OnBecameInvisible() {
		isVisible = false;
		
	}
}
