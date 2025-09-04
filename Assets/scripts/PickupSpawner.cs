using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour
{
	public GameObject[] pickups;				// Array of pickup prefabs with the bomb pickup first and health second.
	public float pickupDeliveryTime = 5f;		// Delay on delivery.
	public float dropRangeLeft = 0f;					// Smallest value of x in world coordinates the delivery can happen at.
	public float dropRangeRight = 0f;				// Largest value of x in world coordinates the delivery can happen at.
	//public float highHealthThreshold = 75f;		// The health of the player, above which only bomb crates will be delivered.
	//public float lowHealthThreshold = 25f;		// The health of the player, below which only health crates will be delivered.

	//private HealthScript playerHealth;			// Reference to the PlayerHealth script.
	//GameObject player;

	public int maxSpawns = 1;
	public bool autoDestroyAfterMax = false;
	private int countSpawns = 0;

	public float startDelay = 0f;

	void Awake ()
	{
		// Setting up the reference.
		//playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
		//player = GameObject.FindGameObjectWithTag("Jelly");
	}


	void Start ()
	{
	    countSpawns = 0;
	    if(startDelay>0f) {
			Invoke("InvokeWithDelay",startDelay);
	    }
	    else {
			// Start the first delivery.
			StartCoroutine(DeliverPickup());
	    }
		
	}

	void InvokeWithDelay() {
	  StartCoroutine(DeliverPickup());
	}

	public IEnumerator DeliverPickup()
	{
		// Wait for the delivery delay.
		yield return new WaitForSeconds(pickupDeliveryTime);
		
		if(CanDeliver()) {

		   countSpawns+=1;
			// Create a random x coordinate for the delivery in the drop range.
			float dropPosX = Random.Range(transform.position.x - dropRangeLeft, transform.position.x + dropRangeRight);
			
			// Create a position with the random x coordinate.
			Vector3 dropPos =  new Vector3(dropPosX, transform.position.y,transform.position.z);
			
			// If the player's health is above the high threshold...
			/*if(playerHealth.health >= highHealthThreshold)
			// ... instantiate a bomb pickup at the drop position.
			Instantiate(pickups[0], dropPos, Quaternion.identity);
		// Otherwise if the player's health is below the low threshold...
		else if(playerHealth.health <= lowHealthThreshold)
			// ... instantiate a health pickup at the drop position.
			Instantiate(pickups[1], dropPos, Quaternion.identity);*/
			// Otherwise...
			//else
			//{
			// ... instantiate a random pickup at the drop position.
			int pickupIndex = Random.Range(0, pickups.Length);
			Instantiate(pickups[pickupIndex], dropPos, Quaternion.identity);
		}

		if(autoDestroyAfterMax && countSpawns>= maxSpawns) {
		   Destroy(gameObject);
		}
		//}
	}
	

	private bool CanDeliver() {
		//we are still in front of the player, and the distance is shorter than half screen size
		//bool canDeliver = (player!=null && (player.transform.position.x <= transform.position.x) 
		//&& (transform.position.x - player.transform.position.x <= Screen.width/2)  );
		//return canDeliver;
	   return true;
	}

}
