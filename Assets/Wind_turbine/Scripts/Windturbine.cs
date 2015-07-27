using UnityEngine;
using System.Collections;

public class Windturbine : MonoBehaviour {
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
//          
//                                              Windturbine rotator
//
//										by Andre "AEG" Bürger / VIS-Games 2012
//
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------

    public bool stopped = true;
    private bool started = false;
    public float speed = 80.0f;
    //start by itself
    public bool autoStart = true;
    
    //delay before start
    public float startTimeDelay = 3.0f;
    public float motionDuration = 8f;
	public float intervalBetweenLoops = 5f;
    
    private float startTime;
	private float stopTime;
    
    float angle;

	bool isBloody = false;
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
void Start()
{
    angle = 0.0f;
    startTime = 0f;
	stopTime = 0f;
	//intervalBetweenStartStop = 0
	isBloody = gameObject.tag!=null && gameObject.tag.Equals("Bloody");
    
	if(autoStart) {
		Invoke("StartRotor",startTimeDelay);
	 }
}
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
void Update()
{
  if(!stopped && started) {
	angle += (Time.deltaTime * speed) % 360.0f;
	transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
  }
  
  if(started) {
  
		startTime +=Time.deltaTime;
		if(startTime>=motionDuration) {
		 StopRotor();
		}
  }
  else if(stopped) {
		stopTime +=Time.deltaTime;
		if(stopTime>=intervalBetweenLoops) {
			StartRotor();
		}
  }
    
}
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------

	public void StopRotor() {
		stopped = true;
		started = false;
		stopTime = 0f;
	}
	
	public void StartRotor() {
	    startTime = 0f;
		stopped = false;
		started = true;
		//only do this on start
		ApplyWindForce();
	}
	
	
	void ApplyWindForce() {
	  
	  GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
	  if(jelly!=null) {
	    
		Vector3 currentPosition = Camera.main.WorldToScreenPoint (transform.position);
		Vector3 jellyPosition = Camera.main.WorldToScreenPoint (jelly.transform.position);
		
		if(jellyPosition.x < currentPosition.x) {
			jelly.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10f,0f),ForceMode2D.Force);
		}
		else {
			jelly.GetComponent<Rigidbody2D>().AddForce(new Vector2(10f,0f),ForceMode2D.Force);
		}
	    
	  }
	}

	void OnTriggerEnter2D(Collider2D collision)
	{

	//just handle collision with Jelly
	   if(collision.gameObject.GetComponent<JellyScript>()!=null && !stopped) {
	
			HandleCollisionWithJelly(collision.gameObject);
	   }
	   else if(collision.gameObject.GetComponent<ParachuteScript>()!=null && !stopped) {
	    //hit the parachute, release it
		GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
		if(jelly!=null) {
			jelly.GetComponent<JellyScript>().Release();
		}
	   }
		
	}


	void HandleCollisionWithJelly(GameObject toDestroy) {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			if(player!=null) {
				
				PlayerScript script = player.GetComponent<PlayerScript>();
				if(script!=null) {
				    
					script.TakeLife();
				    
				}

				if(isBloody) {
					SpecialEffectsHelper.Instance.PlayBloodSplaterEffect(toDestroy.transform.position);
				}
				
			}
			
			//Kill the Jelly, but without increase saves
			SoundEffectsHelper.Instance.PlayHitDeadSound();
			SpecialEffectsHelper.Instance.PlayJellyHitDeadEffect(toDestroy.transform.position);
			GameControllerScript.Instance.JellyDied();
			SpecialEffectsHelper.Instance.PlayJellySoulEffect(transform.position);
			Destroy(toDestroy);
	}
}



