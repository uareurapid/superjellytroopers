using UnityEngine;
using System.Collections;

public class EnlargeShrinkScript : MonoBehaviour {


    public bool enlarge = true;
	//otherwise shrink

	public float percentage = 0.25f;//25%
	// Use this for initialization

	private Vector3 initialScale ;	
	void Start () {
		initialScale = gameObject.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {

	  Vector3 currentScale = gameObject.transform.localScale;

	  if(enlarge && (currentScale.x < initialScale.x + percentage)){
		currentScale.x = currentScale.x + (currentScale.x * Time.deltaTime);
		currentScale.y = currentScale.y + (currentScale.y * Time.deltaTime);
	    //enlarge
	  }
	  else if(!enlarge && (currentScale.x > initialScale.x + percentage)){
		//shrink
	    currentScale.x = currentScale.x - (currentScale.x * Time.deltaTime);
		currentScale.y = currentScale.y - (currentScale.y * Time.deltaTime);
	  }

	  gameObject.transform.localScale = currentScale;
	   
	  
	}
}
