using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {


	private Animator animator;
	// Use this for initialization
	void Start () {
	
	 animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D(Collision2D collision)
	{

		if(animator!=null){
		  animator.enabled = true;
		  Invoke("StopPlaying",2f);
		}
		
		//just handle collision with Jelly
		if(collision.gameObject.GetComponent<JellyScript>()!=null) {
			
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			if(player!=null) {
				
				PlayerScript script = player.GetComponent<PlayerScript>();
				if(script!=null) {
					script.TakeLife();
				}
				
			}
			
			//Kill the Jelly, but without increase saves

			GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");

			SoundEffectsHelper sounds = scripts.GetComponent<SoundEffectsHelper>();
			sounds.PlayHitDeadSound();

			SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
			effects.PlayJellyHitDeadEffect(collision.gameObject.transform.position);


			GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
			controller.JellyDied();

			effects.PlayJellySoulEffect(transform.position);
			Destroy(collision.gameObject);
		}
		
		
	}
	
	void StopPlaying() {
	
		if(animator!=null){
			animator.enabled = false;
		}
	}
}
