using UnityEngine;
using System.Collections;
/// <summary>
/// Simply moves the current game object
/// </summary>
public class MoveScript : MonoBehaviour
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
	
	public bool alternateYMovement = false;
	public bool alternateXMovement = false;
	
	public bool chasePlayer = false;
	
	private float lastDirectionChange = 0f;
	private float changeDirectionInterval = 1.0f;
	
	//only for X axis
	private float lastXDirectionChange = 0f;
	private float changeXDirectionInterval = 1.0f;

	
	public int limitedMovementXInterval = 0;
	public int limitedMovementYInterval = 0;
	
	public bool allowRevertXDirection = false;
	public bool allowRevertYDirection = false;
	//revert the transform, not just the direction
	public bool revertAlsoTransform = false;

	private bool revert = false;
	
	private GameObject target;
	
	private Vector3 startPosition;
	
	
	//allows the object to move even if not visible
	//by default is always false, so must be explicit set to true
	//like on asteroid spawner (of level 1) for instance
	public bool allowMoveIfNotVisible = false;
	bool isVisible = false;
	
	void Start() {
	
		revert = false;
		startPosition =  Camera.main.WorldToScreenPoint (transform.position);
	}
	
	void Awake() {
		target = GameObject.FindGameObjectWithTag("Player");
		
	}
	
	void CheckPositions() {

		Vector3 currentPosition = Camera.main.WorldToScreenPoint (transform.position);
		
		//check on X
		if(limitedMovementXInterval>0) {

			if(direction.x < 0 ) {
			//moving left
			
			  if(!revert) {
				if(currentPosition.x < startPosition.x - (0.0f + limitedMovementXInterval ) ){
					revert = true;
					startPosition.x = currentPosition.x;
				 }
		
			  }
			
			
			
		   }
		   else if(direction.x > 0) {
			//moving right
			  if(!revert) {
				if(currentPosition.x > startPosition.x + (0.0f+ limitedMovementXInterval) ){
					revert = true;
					startPosition.x = currentPosition.x;
				}
		
				
			  }	
			
		  }
		}
        else {
        //check on Y
		if(direction.y > 0 ) { 
			//is actually moving down

			  
			if(!revert) {
				if(currentPosition.y > startPosition.y + (0.0f- limitedMovementYInterval) ){
					revert = true;
					startPosition.y = currentPosition.y;
				}
		
				
			  }
			
			
		   }
		   else if(direction.y < 0) {

			if(!revert) {
				if(currentPosition.y < startPosition.y - (0.0f - limitedMovementYInterval ) ){
					revert = true;
					startPosition.y = currentPosition.y;
				 }
		
			  }
		
		   }
        }//end check on y


		
		
	}

	bool AllowRevertDirection() {
		return allowRevertXDirection || allowRevertYDirection;
	}

	bool HasLimitedMovementInterval() {
		return limitedMovementXInterval > 0 || limitedMovementYInterval>0;
	}
	
	void Update()
	{

		if(!revert && ( AllowRevertDirection() &&  HasLimitedMovementInterval() ) ) {
		
		   CheckPositions();
		   if(revert) {

		       if(limitedMovementXInterval>0) {
					// 1 - revert current direction on x axis
				    direction.x = direction.x*-1;
		       }
			   else {
		        	//revert is on y axis, instead
					direction.y = direction.y*-1;
		       }
				
				// 3 - Define new movement, based on new direction
				movement = new Vector2(
					speed.x * direction.x,
					speed.y * direction.y);
				revert = false;
		   }
	
		
			
		}

	  else if(allowMoveIfNotVisible && allowRevertXDirection && revert) {

	     
				// 1 - revert current direction on x axis
	            direction.x = direction.x*-1;
				// 3 - Define new movement, based on new direction
				movement = new Vector2(
					speed.x * direction.x,
					speed.y * direction.y);
				revert = false;

				if(revertAlsoTransform) {
					transform.localScale = new Vector3(transform.localScale.x* -1, transform.localScale.y, transform.localScale.z );
				}
	  }
	  else {
	  
			if( allowMoveIfNotVisible || isVisible ) {
				
				lastDirectionChange += Time.deltaTime;
				lastXDirectionChange += Time.deltaTime;
				
				if(alternateYMovement && (lastDirectionChange >= changeDirectionInterval) ) {
					
					if(chasePlayer && target!=null) {
						Vector3 targetPosition = target.transform.position;
						if(targetPosition.y > transform.position.y) {
							direction.y = 1;
						}
						else if(targetPosition.y < transform.position.y) {
							direction.y = -1;
						}
						else {
							direction.y = 0;
						}
						
					}
					else {
						GetRandomYDirection();
					}
					
					lastDirectionChange = 0f;
					
					//transform.rotation = Rotate2D.SmoothLookAt(transform, Vector3 target, Vector3 axis, float speed);
					
					
				}
				else if(alternateXMovement && (lastXDirectionChange >= changeXDirectionInterval) ) {
					GetRandomXDirection();
					lastXDirectionChange = 0f;
				}
				
				
				//transform.RotateAround(transform.position, Vector3.forward, 20 * Time.deltaTime);
				// 2 - Movement
				movement = new Vector2(
					speed.x * direction.x,
					speed.y * direction.y);
					
					
				//lastXDirection = direction.x;
				
			}
	  }
	
		
		
	}
	
	void FixedUpdate()
	{
	  //we only move the enemy or asteroi if the player is on scene,
	  //otherwise we see a lot of space without enemies, more close to the end of the level
	  
		// Apply movement to the rigidbody
	    //check if in camera
		if( allowMoveIfNotVisible || isVisible  ) {
			 GetComponent<Rigidbody2D>().velocity = movement;
		}
	  
		
	}
	//gets a random y direction, either 0,-1 or 1
	private void GetRandomYDirection() {
		direction.y = Random.Range(-1,2);

	}
	//gets a random x direction, either 0,-1 or 1
	private void GetRandomXDirection() {
		direction.x = Random.Range(-1,2);
		
	}
	
	//called from outside, to allow or not, move if not vivible
	public void AllowMoveWhenInvisible(bool allowInvisibleMove) {
		allowMoveIfNotVisible = allowInvisibleMove;
	}
	
	
	
	void OnBecameVisible() {
		isVisible = true;

	}

	//TODO; IS NOT TURNING BACK ANYMORE
	void OnBecameInvisible (){
		
		if(isVisible) {
			if (allowMoveIfNotVisible && allowRevertXDirection && limitedMovementXInterval == 0) {
			  revert = true;
			}

		}
		
		isVisible = false;
	}
}
