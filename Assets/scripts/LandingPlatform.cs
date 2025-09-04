using UnityEngine;
using System.Collections;

public class LandingPlatform : MonoBehaviour {

    public bool isDetachable = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		  //go down
		  HandleCollision(collision.gameObject);
		
	}

	private void HandleCollision(GameObject gameObject) {

	 JellyScript jelly = gameObject.GetComponent<JellyScript>();
		if(jelly!=null && !jelly.isLanded) {
				//go down
		  if(isDetachable) { //only used on last level
			 transform.parent = null;
			 GetComponent<Rigidbody2D>().isKinematic = false;
			 GetComponent<Rigidbody2D>().gravityScale=1;
			 //invoke later, put spawn on hold first
			 LateHandleJellyLanding(jelly,gameObject);
		  }
		  else {
				
			    
			    //handle normal landing flow
				HandleJellyLanding(jelly,gameObject);

				//now check if the camera need adjustments
				SmoothFollow2D scriptFollow = Camera.main.GetComponent<SmoothFollow2D>();
	 			if(scriptFollow!=null) {
	 				//handle follow + spawn hold
					GameObject obj = GameObject.FindGameObjectWithTag("Spawner");
					if(obj!=null) {
						SpawnerScript spawner = obj.GetComponent<SpawnerScript>();
						if(spawner!=null) {
		  					spawner.PutSpawnOnHold(true);
							spawner.UnlockSpawning(scriptFollow);
						}
					}
		  		}//scriptFollow !=null
		  		
		  }

		}
	}

	void OnTriggerEnter2D(Collider2D collision) {
		
		  HandleCollision(collision.gameObject);
		
	}

	void LateHandleJellyLanding(JellyScript jelly, GameObject jellyObject) {

		//will remove the hold when the enemy boss is hit
		GameObject obj = GameObject.FindGameObjectWithTag("Spawner");
		if(obj!=null) {
			SpawnerScript spawner = obj.GetComponent<SpawnerScript>();
			if(spawner!=null) {
		  		spawner.PutSpawnOnHold(true);
				
			}

		 HandleJellyLanding(jelly,jellyObject);
		 CameraFollowPlatform(spawner);
		}


	}

	//Handle normal Jelly Landing
	void HandleJellyLanding(JellyScript jelly,GameObject jellyObject) {

		//not falling anymore
		jelly.isFalling = false;
		//spawn and increase saves counter
		jelly.Land();
		//kill the Jelly
		Destroy(jellyObject);

	}

	//The camera follow the platfom, until hit destroys itsef
	//then is focused again on the spawned jelly
	void CameraFollowPlatform(SpawnerScript spawner) {

	 //stop following the jelly
	 spawner.cameraFollowSpawned = false;
	 SmoothFollow2D scriptFollow = Camera.main.GetComponent<SmoothFollow2D>();
	 if(scriptFollow!=null) {
	 	//start following the platform instead
		scriptFollow.target = gameObject.transform;
	 }
	}

}
