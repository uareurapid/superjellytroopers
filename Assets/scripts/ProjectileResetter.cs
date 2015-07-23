using UnityEngine;
using System.Collections;

public class ProjectileResetter : MonoBehaviour {

	public Rigidbody2D projectile;			//	The rigidbody of the projectile
	public float resetSpeed = 0.025f;		//	The angular velocity threshold of the projectile, below which our game will reset
	
	private float resetSpeedSqr;			//	The square value of Reset Speed, for efficient calculation
	private SpringJoint2D spring;			//	The SpringJoint2D component which is destroyed when the projectile is launched
	ProjectileScript projectileScript;
	void Start ()
	{
		//	Calculate the Resset Speed Squared from the Reset Speed
		resetSpeedSqr = resetSpeed * resetSpeed;

		//	Get the SpringJoint2D component through our reference to the GameObject's Rigidbody
		spring = projectile.GetComponent <SpringJoint2D>();
		projectileScript = projectile.gameObject.GetComponent<ProjectileScript>();
	}
	
	void Update () {

		//	If the spring had been destroyed (indicating we have launched the projectile) and our projectile's velocity is below the threshold...
		if (spring == null && projectile.velocity.sqrMagnitude < resetSpeedSqr) {
			//	... call the Reset() function
			Reset ();
		}
		else if (projectile.GetComponent<Renderer>().enabled && projectile.GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false) {
		    Reset();
		}
	}
	
	
	void OnTriggerExit2D (Collider2D other) {
		//	If the projectile leaves the Collider2D boundary...
		if (other.GetComponent<Rigidbody2D>() == projectile) {
			//	... call the Reset() function
			Reset ();
		}
	}
	
	
	
	void Reset () {
		
		projectileScript.ResetProjectile();

		
		
	}
}
