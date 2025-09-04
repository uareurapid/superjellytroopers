using UnityEngine;

/// <summary>
/// Handle hitpoints and damages
/// </summary>
public class HealthScript : MonoBehaviour
{
	/// <summary>
	/// Total hitpoints
	/// </summary>
	public int hitPoints = 1;

	private int initialHealth;
	/// <summary>
	/// Enemy or player?
	/// </summary>
	public bool isEnemy = true;
	private bool collisionsEnabled = true;
	//ScoreScript scoreScript;
	
	void Start() {

	//TODO REMOVE THIS ON RELEASE
	    /*#if DEBUG_BUILD
		if(gameObject.tag!=null && gameObject.tag.Equals("Player") ) {
		  hitPoints = 10;
		}
		#endif*/
		initialHealth = hitPoints;
	}
	
	/// <summary>
	/// Inflicts damage and check if the object should be destroyed
	/// </summary>
	/// <param name="damageCount"></param>
	public void Damage(int damageCount)
	{

		//collisionsEnabled = GameControllerScript.Instance.IsGameStarted();
		//Debug.Log("Collisions enabled: " + collisionsEnabled);
	    //if(collisionsEnabled) {
	    
			hitPoints -= damageCount;
			if(hitPoints<0) {
				hitPoints = 0;
			}
			
			if (hitPoints == 0) {				
				// Dead!
				Destroy (gameObject);				
				
			} 

	
	    
		
	}
	
	void OnTriggerEnter2D(Collider2D otherCollider)
	{
	   
	
	 
	  //}	
	}
	
	public void AddHitPoints(int sum){
	 hitPoints+=sum;
	}
    //might need to know in the middle, how much was at beginning
    public int GetInitialHealth(){
      return initialHealth;
    }
	

}
