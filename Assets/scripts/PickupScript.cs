using UnityEngine;
using System.Collections;
using RescueJelly;

public class PickupScript : MonoBehaviour {

	public int units = 1;
    public bool isHealth = false;
	public bool isTime = false;
	public bool isSpeed = false;
	public bool autoDestroy = true;
    public float timeToLive = 15f;//autodestroy after 15 seconds
	// Use this for initialization

	private static RuntimePlatform platform = Application.platform;
	private bool isMobilePlatform = false;
    
	void Start () {

	  //if is desktop, i don´t have any in app purchase, so if i collide with a pickup i stick with it, like if it was a real purchase
	  isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);

      if(autoDestroy) {
       Invoke("AutoDestroy",timeToLive);
      }
	}
	
	// Update is called once per frame
	void Update () {
	  

	}

    void OnTriggerEnter2D(Collider2D collision) {
 
      bool destroyed = false;
	  JellyScript jelly = collision.gameObject.GetComponent<JellyScript>();
	  if(jelly!=null) {

		  AudioSource audioEffect = GetComponent<AudioSource>();
		  //play any audio
		  if(audioEffect!=null && audioEffect.clip!=null) {
		   AudioSource.PlayClipAtPoint(audioEffect.clip,transform.position);
		  }
		   
		  //hide it
		  GetComponent<Renderer>().enabled = false;

			if(isTime) {
			   GameControllerScript.Instance.IncreaseTimeSecondsBy(units);//10 seconds hardcoded
			   if(!isMobilePlatform) {
				 PlayerPrefs.SetString(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID,"true");
			   }
				
			}
		    else {
				GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		 		if(playerObj!=null) {
					PlayerScript player = playerObj.GetComponent<PlayerScript>();

					if(isHealth) {
						player.IncreaseHealthBy(units);
						if(!isMobilePlatform) {
							PlayerPrefs.SetString(GameConstants.JELLY_TROOPERS_EXTRA_LIFE_SINGLE_PRODUCT_ID,"true");
						}
		 		
					}
					else if(isSpeed) {
		       			jelly.IncreaseMovementSpeedBy(units);
						player.IncreaseMovementSpeedBy(units);
						if(!isMobilePlatform) {
							PlayerPrefs.SetString(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID,"true");
						}
		 			}


		 		}

		    }

		  //play any effect
		  foreach(ParticleSystem part in GetComponentsInChildren<ParticleSystem>()) {
		    
	    	part.Play();
			//destroy pickup
		    Destroy(gameObject,part.duration);  
		    destroyed = true;
	  	  }

		  //still around? kill it now!
		  if(!destroyed) {
			Destroy(gameObject); 
		  }
		 
	  }
	 
	  //otherwise ignore the colision
	}

	void AutoDestroy() {

	  foreach(ParticleSystem part in GetComponentsInChildren<ParticleSystem>()) {
	    part.Play();
	  }

	  Destroy(gameObject);
	}


}
