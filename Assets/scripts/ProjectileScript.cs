using UnityEngine;

public class ProjectileScript : MonoBehaviour {
	public float maxStretch = 3.0f;
	public LineRenderer catapultLineFront;
	public LineRenderer catapultLineBack;  
	
	private SpringJoint2D spring;
	private Transform catapult;
	private Ray rayToMouse;
	private Ray leftCatapultToProjectile;
	private float maxStretchSqr;
	private float circleRadius;
	private bool clickedOn;
	private Vector2 prevVelocity;
	private bool isVisible = false;
	public Vector3 initialPosition = new Vector3(0f,0f,0f);
	GameObject shooter;
	
	void Awake () {
		spring = GetComponent <SpringJoint2D> ();
		catapult = spring.connectedBody.transform;
	}
	
	void Start () {
		//LineRendererSetup ();
		rayToMouse = new Ray(catapult.position, Vector3.zero);
		leftCatapultToProjectile = new Ray(catapultLineFront.transform.position, Vector3.zero);
		maxStretchSqr = maxStretch * maxStretch;
		CircleCollider2D circle = GetComponent<Collider2D>() as CircleCollider2D;
		circleRadius = circle.radius;
		
		initialPosition = transform.position;
		shooter = GameObject.FindGameObjectWithTag("Shooter");
		PlayParticle();
		Invoke("Shoot",3f);
	}
	
	void Update () {
		//if (clickedOn)
		//	Dragging ();
		
		if (spring != null && spring.enabled) {
		
			if (!GetComponent<Rigidbody2D>().isKinematic && prevVelocity.sqrMagnitude > GetComponent<Rigidbody2D>().velocity.sqrMagnitude) {
				spring.enabled=false;
				GetComponent<Rigidbody2D>().velocity = prevVelocity;
			}
			
			if (!clickedOn) {
				prevVelocity = GetComponent<Rigidbody2D>().velocity;
			}
				
			
			LineRendererUpdate ();
			
		}

		/*else {
			catapultLineFront.enabled = false;
			catapultLineBack.enabled = false;
		}*/
	}
	
	/*void LineRendererSetup () {
		catapultLineFront.SetPosition(0, catapultLineFront.transform.position);
		catapultLineBack.SetPosition(0, catapultLineBack.transform.position);
		
		catapultLineFront.sortingLayerName = "Foreground";
		catapultLineBack.sortingLayerName = "Foreground";
		
		catapultLineFront.sortingOrder = 3;
		catapultLineBack.sortingOrder = 1;
	}*/
	
	
	
	void Shoot() {

		//renderer.enabled=true;
		
		if(spring!=null) {
			spring.enabled = true;
		}
			
		GetComponent<Rigidbody2D>().isKinematic = false;
		clickedOn = false;
	}
	
	void Dragging () {
		/**
		Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 catapultToMouse = mouseWorldPoint - catapult.position;
		
		if (catapultToMouse.sqrMagnitude > maxStretchSqr) {
			rayToMouse.direction = catapultToMouse;
			mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
		}
		
		mouseWorldPoint.z = 0f;
		transform.position = mouseWorldPoint;*/
	}
	
	void LineRendererUpdate () {
		Vector2 catapultToProjectile = transform.position - catapultLineFront.transform.position;
		leftCatapultToProjectile.direction = catapultToProjectile;
		//Vector3 holdPoint = leftCatapultToProjectile.GetPoint(catapultToProjectile.magnitude + circleRadius);
		//catapultLineFront.SetPosition(1, holdPoint);
		//catapultLineBack.SetPosition(1, holdPoint);
	}
	
	void PlayParticle() {
	
	if(shooter!=null)
		foreach(ParticleSystem p in shooter.GetComponentsInChildren<ParticleSystem>())
		{		    
			p.Play();
		}
	}
	
	public void ResetProjectile() {
	
	  
	  //put it back on the original position
	  transform.position = initialPosition;
	  //
	  prevVelocity.x = 0f; prevVelocity.y=0f;
	  GetComponent<Rigidbody2D>().velocity=prevVelocity;
	  
	  //hide it from screen
	  GetComponent<Renderer>().enabled = true;
	  GetComponent<Rigidbody2D>().isKinematic = true;
	  
	  
	  PlayParticle();
	  
	  Invoke("ResetAndShoot",2f);
	}
	
	void OnBecameVisible() {
		
		isVisible = true;
	
	}
	
	void ResetAndShoot() {
	  Shoot();
	}
	
	void OnBecameInvisible() {
	    isVisible = false;
	}
	
	void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("collided");
		//just handle collision with Jelly
		if(collision.gameObject.GetComponent<JellyScript>()!=null) {
			
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			if(player!=null) {
				
				PlayerScript script = player.GetComponent<PlayerScript>();
				if(script!=null) {
					script.TakeLife();
				}
				
			}

			GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");

			SoundEffectsHelper sounds = scripts.GetComponent<SoundEffectsHelper>();
			//Kill the Jelly, but without increase saves
			sounds.PlayHitDeadSound();

			SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
			effects.PlayJellyHitDeadEffect(collision.gameObject.transform.position);

			GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
			controller.JellyDied();

			effects.PlayJellySoulEffect(transform.position);
			Destroy(collision.gameObject);
		}
		else if(collision.gameObject.GetComponent<ParachuteScript>()!=null) {
	    	//hit the parachute, release it
			GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
			if(jelly!=null) {
				jelly.GetComponent<JellyScript>().Release();
			}
	   }
	}
}

