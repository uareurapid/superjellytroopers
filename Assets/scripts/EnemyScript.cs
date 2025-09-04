using UnityEngine;
using System.Collections;


/// <summary>
/// Enemy generic behavior
/// </summary>
public class EnemyScript : MonoBehaviour
{

  public bool autoDestroy = false;
	
	private bool isVisible = false;
	public bool takeAllLifes = false;

	private bool isBloody = false;

	public bool isBoss = false;

	private GameObject scripts;

	
	void Awake()
	{
		// Retrieve the weapon only once, which is inside the enemy-->weaponObject->weaponScript
		//weapons = GetComponentsInChildren<WeaponScript>();


	}

	// 1 - Disable everything
	void Start()
	{

	  isBloody = gameObject.tag!=null && gameObject.tag.Equals("Bloody");
	  scripts = GameObject.FindGameObjectWithTag("Scripts");

	}

	void Update()
	{
		
			// Auto-fire
			/*foreach (WeaponScript weapon in weapons)
			{
				if (weapon != null && weapon.enabled && weapon.CanAttack(isVisible))
				{
					weapon.Attack(true);

					//sound of shot
					SoundEffectsHelper.Instance.MakeEnemyShotSound();
				}
			}*/
			
			// 4 - Out of the camera ? Destroy the game object.
			//if (renderer.IsVisibleFrom(Camera.main) == false)
			//{
			//	Destroy(gameObject);
			//}
		
	}

	
	
	void OnBecameVisible() {

		isVisible = true;
		//if(GameControllerScript.Instance.IsGameOver()) {
		//  isVisible = false;
		//}
	}
	
	void OnBecameInvisible() {
		
		if(isVisible && autoDestroy) {
		  Destroy(gameObject);
		}
		isVisible = false;
		
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
	  if(collision.gameObject.GetComponent<JellyScript>()!=null) {
		 HandleCollisionWithJelly(collision.gameObject);
	  }
	  else if(collision.gameObject.GetComponent<ParachuteScript>()!=null) {
	    //hit the parachute, release it
		GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
		if(jelly!=null) {
			jelly.GetComponent<JellyScript>().Release();
		}
	   }
	}

	void OnCollisionEnter2D(Collision2D collision)
	{

	//just handle collision with Jelly
	   if(collision.gameObject.GetComponent<JellyScript>()!=null) {
	
			HandleCollisionWithJelly(collision.gameObject);
	   }
	   else if(collision.gameObject.GetComponent<ParachuteScript>()!=null) {
	    //hit the parachute, release it
			GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
			if(jelly!=null) {
				jelly.GetComponent<JellyScript>().Release();
			}
	   }
	   else {
			//is the boss and collided with a landing platform (last level only)
			LandingPlatform landing = collision.gameObject.GetComponent<LandingPlatform>();

			if(landing!=null && landing.isDetachable) {

			  if(isBoss) {
			
			  Destroy(landing.gameObject);
			  HealthScript health = GetComponent<HealthScript>();

			  //play hit sound
			  if(GetComponent<AudioSource>()!=null) {
			   GetComponent<AudioSource>().Play();
			  }

			  if(health!=null) {
				health.Damage(1);
				FlashRedWhenHit();

			  }//end if health!=null

				//unlock spawning gain
				UnlockSpawning();

			}//end if is boss
            else if(IsLava()) {
				//just destroy the landing platform on the lava
				SoundEffectsHelper soundHelper = scripts.GetComponent<SoundEffectsHelper>();
				SpecialEffectsHelper effectsHelper = scripts.GetComponent<SpecialEffectsHelper>();
				soundHelper.PlayWaterSplashSound();
				effectsHelper.PlayLavaSplashEffect(landing.transform.position);
				Destroy(landing.gameObject);


				//unlock spawning gain
				UnlockSpawning();
            }

				
		   }
		   //and if detachable
	   }
		
	}
	//alows spawning again
	void UnlockSpawning() {
		GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
		if(spawnerObj!=null) {
			SpawnerScript spawner = spawnerObj.GetComponent<SpawnerScript>();

			//or maybe as workaround have a dummy object in same position of spawner
			SmoothFollow2D cameraFollow = Camera.main.GetComponent<SmoothFollow2D>();
			if(cameraFollow!=null && cameraFollow.enabled) {
			  cameraFollow.ResetCameraPosition();
			}

			if(spawner!=null && spawner.IsSpawningOnHold()) {
				//allow follow the jelly again (was following the platform)
				spawner.cameraFollowSpawned = true;
		  		spawner.PutSpawnOnHold(false);
			}
		}
	}

	IEnumerator Fade (float start,float end, float length) {

	 GUITexture redTexture = GetComponentInChildren<GUITexture>();
	 Color aux = redTexture.color;
		  //define Fade parmeters
		if (aux.a == start){

		  for (float i = 0.0f; i < 1.0f; i += Time.deltaTime*(1/length)) { 
		   //for the length of time
		   aux.a = Mathf.Lerp(start, end, i); 
		   //lerp the value of the transparency from the start value to the end value in equal increments
		   yield return null;
		   aux.a = end;
		  // ensure the fade is completely finished (because lerp doesn't always end on an exact value)
          redTexture.color = aux;
          } //end for
 
		} //end if
	
 
	} //end Fade


	void FlashRedWhenHit (){


		StartCoroutine(Fade (0f, 0.1f, 0.5f));
		StartCoroutine(MyWaitMethod());
		StartCoroutine(Fade (0.1f, 0f, 0.5f));
	
    	
    }

	IEnumerator MyWaitMethod() {
		yield return new WaitForSeconds(.01f);
	}
		

	void HandleCollisionWithJelly(GameObject toDestroy) {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			if(player!=null) {
				
				PlayerScript script = player.GetComponent<PlayerScript>();
				if(script!=null) {
				    if(takeAllLifes) {
						script.TakeAllLifes();
				    }
				    else {
						script.TakeLife();
				    }
				}

				if(isBloody && scripts!=null) {
					SpecialEffectsHelper effectsHelper = scripts.GetComponent<SpecialEffectsHelper>();
					effectsHelper.PlayBloodSplaterEffect(toDestroy.transform.position);
				}
				
			}

			//Kill the Jelly, but without increase saves

			if(scripts!=null) {

				SoundEffectsHelper soundHelper = scripts.GetComponent<SoundEffectsHelper>();
				SpecialEffectsHelper effectsHelper = scripts.GetComponent<SpecialEffectsHelper>();
				GameControllerScript controller = scripts.GetComponent<GameControllerScript>();


				if(IsLava()) {
					soundHelper.PlayWaterSplashSound();
					effectsHelper.PlayLavaSplashEffect(toDestroy.transform.position);
				}
				else {
					soundHelper.PlayHitDeadSound();
					effectsHelper.PlayJellyHitDeadEffect(toDestroy.transform.position);
				}

				controller.JellyDied();
				effectsHelper.PlayJellySoulEffect(transform.position);
						
			}


			SmoothFollow2D cameraFollow = Camera.main.GetComponent<SmoothFollow2D>();
			if(cameraFollow!=null && cameraFollow.enabled) {
			  cameraFollow.ResetCameraPosition();
			}

			Destroy(toDestroy);
	}

	//TODO, the effects are not very clear yet
	//TODO present credits, show some message...
	void OnDestroy() {


	 if(isBoss) {
						//6.506495,-33.42391
						//red 6.582625, -33.66986
		if(scripts!=null) {
		  GameObject audioExplosionBig = GameObject.FindGameObjectWithTag("Finish");
		  SpecialEffectsHelper effectsHelper = scripts.GetComponent<SpecialEffectsHelper>();
		  if(audioExplosionBig!=null && effectsHelper!=null) {
		    AudioSource source = audioExplosionBig.GetComponent<AudioSource>();
			effectsHelper.PlayExplosionEffect(transform.position);

			//this explosion is a bigger one!
			source.Play();
			
		  }

								//HURRAY!!!!!!!!!!!

		 GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
		 controller.CompletedGame();
  

		}





	 }


	}

	//is lava enemy?
	private bool IsLava() {
	 return tag!=null && CompareTag("Lava");
	}
	
}
