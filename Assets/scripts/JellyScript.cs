using UnityEngine;
using System.Collections;

public class JellyScript : MonoBehaviour {


    private ParachuteScript parachute;
    private MoveScript moveScript;
    public bool isFalling = false;
	public bool isReleased = false;
	public bool isLanded = false;
	private bool hasInvokedSpawn = false;
	private SpawnerScript spawner;

	//if is a demonstration, like on intro scene we do no clamp positions
	public bool isDemonstrationJelly = false;
	public bool failSafeUsed = false;

    private GameObject scripts;

    //when the failsafe is activated we go up 30 pixels for instance
    private bool startPullingUP = false;
	private Vector2 previousMoveDirection;
	private Vector2 previousMoveSpeed;

	private bool isPullingUpMovement = false;

	// Use this for initialization
	void Start () {
	
	   isFalling = true;
	   isReleased = false;
	   isLanded = false;
	   
	   parachute = GetComponentInChildren<ParachuteScript>();
	   moveScript = GetComponent<MoveScript>();
	   
	   if(moveScript!=null) {
	       moveScript.enabled = true;
		   //save previous values for late restoration
		   previousMoveDirection = new Vector2(moveScript.direction.x,moveScript.direction.y);
		   previousMoveSpeed = new Vector2(moveScript.speed.x,moveScript.speed.y);
	   }
	   
	   GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
	   if(spawnerObj!=null) {
	     spawner = spawnerObj.GetComponent<SpawnerScript>();
	   }

	   isPullingUpMovement = false;

	}
	
	void Awake()
	{
	  hasInvokedSpawn = false;
	  scripts = GameObject.FindGameObjectWithTag("Scripts");

		
	
	}

	//todo, code me
	public void LaunchFailsafe() {

		if(CanLaunchFailSafe()) {

		  isReleased = false;
	
		  ParachuteScript parachute = GetComponentInChildren<ParachuteScript>();
		  if(parachute!=null) {

			 SoundEffectsHelper sounds = scripts.GetComponent<SoundEffectsHelper>();
	         sounds.PlayReserveParachuteSound();

			 SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
	         effects.PlayReserveParachuteEffect(transform.position);

			 parachute.LaunchFailsafe();

		  }
		  failSafeUsed = true;
		  //then add the failsafe
		  //There is a bug, if i try to release before ending the pull up movement
		  StartPullUpMovement();
		}

	}

	public bool CanLaunchFailSafe() {
	 return isFalling && !isLanded && !failSafeUsed && isReleased;
	}
	//when the failsafe is launched, we pull up a bit the jelly
	//to simulate the force
	void StartPullUpMovement() {

	   moveScript.direction.x = 0;
	   moveScript.direction.y = 1;//direction up
	   moveScript.speed.x = 0;
	   moveScript.speed.y = 4;
	   gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
	   isPullingUpMovement = true;
	   Invoke("StopPullUpMovement",0.5f);
	}

	void StopPullUpMovement() {

	  //save previous values for late restoration
	   moveScript.direction.x = previousMoveDirection.x;
	   moveScript.direction.y = previousMoveDirection.y;
	   moveScript.speed.x = previousMoveSpeed.x; 
	   moveScript.speed.y = previousMoveSpeed.y;
	   moveScript.alternateXMovement = true;
	   moveScript.enabled = true;
	   isPullingUpMovement = false;

	}
	
	// Update is called once per frame
	void Update () {

	  if(!isFalling) {
	      parachute.parachuteEnabled = false;
	  }
	  else {
		  //is falling
	      parachute.parachuteEnabled = !isReleased;

	  }

	//Limit the movement inside camera bounds
	if(moveScript!=null && moveScript.enabled && isFalling) {

	   if(!isDemonstrationJelly) {

	    var dist = (transform.position - Camera.main.transform.position).z;
		
		var leftBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 0, dist)
			).x;
		
		var rightBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(1, 0, dist)
			).x;
		
		var topBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 0, dist)
			).y;
		
		var bottomBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 1, dist)
			).y;

	    
			//perform a clamp
		    transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
			Mathf.Clamp(transform.position.y, topBorder, bottomBorder),
			transform.position.z
			);

		}


	  }
		
			
	}
	
	public void Jump() {

	  isFalling = true;
	  	  
	  if(moveScript!=null) {
	  //now is the move script that assumes control
	    gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
	    moveScript.enabled = true;
	    
	  }

	  GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
	  controller.isJellyFalling = true;
	  
	}
	
	public void Land() {
	   
	   isLanded = true;
	   isReleased = true;
	   isFalling = false;

	   SoundEffectsHelper sounds = scripts.GetComponent<SoundEffectsHelper>();
	   sounds.PlayLandSafeSound();

	   SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
	   effects.PlayJellyLandedEffect(transform.position);

	   GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
	   controller.JellyLanded();
		
	   
	}

	public bool IsFailsafeUsed() {
	 return failSafeUsed;
	}
	
	//release parachute
	public void Release() {

	    if(isPullingUpMovement) {
		  CancelInvoke("StopPullUpMovement");
	      StopPullUpMovement();
	    }

	    isFalling = true;
		isReleased = true;
		
		gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
		//Just free fall from now on
	    if(moveScript!=null) {
	    
	        moveScript.alternateXMovement = false;
			moveScript.enabled = true;
			moveScript.speed.y = 8;
			moveScript.speed.x = 0;
			
	    }
	    
	    ParachuteScript script = GetComponentInChildren<ParachuteScript>();
	    if(script!=null) {
	      script.Release();
	    }
	    
		
	}
		
	/*
	* Increase swing speed to double value
	*/
	public void IncreaseMovementSpeedBy(int speedFactor) {
		if(moveScript!=null) {
	    
			moveScript.speed.x = moveScript.speed.x * speedFactor;
			
	    }
	}

	public void SetSpeedX(float speed) {
		if(moveScript!=null) {
			moveScript.speed.x = speed;
	    }
	}
	
	public bool GetIsMovementEnabled() {
		if(moveScript!=null) {
		  return moveScript.enabled;
		}
		return false;
	}
	
	void OnDestroy()
	{
		GameControllerScript controller;
		if(scripts!=null) {
			controller = scripts.GetComponent<GameControllerScript>();
		}
		else {
		 controller = GameControllerScript.Instance;
		}

		if(spawner!=null &&!spawner.IsSpawningOnHold() && controller.SpawnAllowed()) {
		  spawner.canSpawn = true;
		  spawner.Spawn();
		}
		

	}
	
	
}
