using UnityEngine;
using System.Collections;

public class ShootSpawner : MonoBehaviour {


	public GameObject shootPrefab;
    public float startDelay;
    public float spawnInterval;
    
    public Vector2 velocityVector = new Vector2(0f,0f);
    
	public Vector3 prefabRotation = new Vector3(0f,0f,0f);
    
    
	GameObject shoot;
	// Use this for initialization
	void Start () {
	
	  InvokeRepeating("Spawn",startDelay,spawnInterval);
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(shoot!=null) {
			shoot.GetComponent<Rigidbody2D>().velocity = velocityVector;
			
			//Vector2 dir = velocity;//rigidbody2D..velocityVector;
			if (velocityVector != Vector2.zero) {
				float angle = Mathf.Atan2(velocityVector.y, velocityVector.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle, Vector2.right);//??? check 
			}
	    }
		
	}
	
	void Spawn() {
	
		shoot = (GameObject)Instantiate(shootPrefab, transform.position, Quaternion.Euler(prefabRotation));	  
	}
}
