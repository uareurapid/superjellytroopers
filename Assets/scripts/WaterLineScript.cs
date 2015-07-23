using UnityEngine;

/// <summary>
/// Enemy generic behavior
/// </summary>
public class WaterLineScript : MonoBehaviour
{

	
	private bool isVisible = false;
	
	void Awake()
	{
		// Retrieve the weapon only once, which is inside the enemy-->weaponObject->weaponScript
		//weapons = GetComponentsInChildren<WeaponScript>();


	}

	// 1 - Disable everything
	void Start()
	{
		
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
		isVisible = false;
	}
	
	void OnCollisionEnter2D(Collision2D collision)
	{
	
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
			//http://www.freesfx.co.uk credit


			GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");

			SoundEffectsHelper sounds = scripts.GetComponent<SoundEffectsHelper>();
			SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
			GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
	

			sounds.PlayWaterSplashSound();
			effects.PlayWaterSplashEffect(collision.gameObject.transform.position);
			controller.JellyDied();
			effects.PlayJellySoulEffect(transform.position);
			Destroy(collision.gameObject);
	   }
	   else if(collision.gameObject.GetComponent<EnemyScript>()!=null) {
	   //if is an eneby (like a barrel, for instance, it destroys it

			GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");

			SoundEffectsHelper sounds = scripts.GetComponent<SoundEffectsHelper>();
			SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();


			sounds.PlayWaterSplashSound();
			effects.PlayWaterSplashEffect(collision.gameObject.transform.position);
			Destroy(collision.gameObject);
	   }
		
		
	}
	
	
}
