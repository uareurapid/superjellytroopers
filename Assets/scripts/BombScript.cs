using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour
{
	public float bombRadius = 10f;			// Radius within which enemies are killed.
	public float bombForce = 100f;			// Force that enemies are thrown from the blast.
	public AudioClip boom;					// Audioclip of explosion.
	public AudioClip fuse;					// Audioclip of fuse.
	public float fuseTime = 1.5f;
	//public GameObject explosion;			// Prefab of explosion effect.




	void Awake ()
	{
		// Setting up references.
	}

	void Start ()
	{
		
		StartCoroutine(BombDetonation());
	}


	IEnumerator BombDetonation()
	{
		// Play the fuse audioclip.
		AudioSource.PlayClipAtPoint(fuse, transform.position);

		// Wait for 2 seconds.
		yield return new WaitForSeconds(fuseTime);

		// Explode the bomb.
		Explode();
	}


	public void Explode()
	{
		//play explosion sound
		AudioSource.PlayClipAtPoint(boom, transform.position);

		// Find all the colliders on the PlayerLayer layer within the bombRadius.
		Collider2D[] elements = Physics2D.OverlapCircleAll(transform.position, bombRadius, 1 << LayerMask.NameToLayer("PlayerLayer"));

		// For each collider...
		foreach(Collider2D en in elements)
		{
			// Check if it has a rigidbody (since there is only one per enemy, on the parent).
			Rigidbody2D rb = en.GetComponent<Rigidbody2D>();
			
			if(rb != null )
			{
				GameObject theObject = rb.gameObject;
				// Find the Enemy script and set the enemy's health to zero.
				HealthScript script =  theObject.GetComponent<HealthScript>();
				
				if(script!=null) {
				
					script.Damage(1);
					

				}

				// Find a vector from the bomb to the player.
				Vector3 deltaPos = rb.transform.position - transform.position;
					
				// Apply a force in this direction with a magnitude of bombForce.
				Vector3 force = deltaPos.normalized * bombForce;
				rb.AddForce(force);
				
			}
		}

		// Set the explosion effect's position to the bomb's position and play the particle system.

		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		if(scripts!=null) {
		  SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
			effects.PlayExplosionEffect(transform.position);
		}
		// Destroy the bomb itself.
		Destroy (gameObject);
	}
}
