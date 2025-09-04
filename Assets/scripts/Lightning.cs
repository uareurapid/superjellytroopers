using UnityEngine;
using System.Collections;
using RescueJelly;

public class Lightning : MonoBehaviour {

	public GameObject targetObject;
	private LineRenderer lineRenderer;

	private Ray2D ray;
	private Vector3 EndPos;
	private RaycastHit2D hit ;
	public bool startEnabled = true;
	public float interval = 2.0f;//every 2 seconds
	public float duration =2.0f;
	private float initialTime=0f;

	void Start() {
		lineRenderer = GetComponent<LineRenderer>();
		initialTime=0f;

		if(startEnabled) {
			InvokeRepeating("ShootLaser",0.3f,interval);
		}
		else {
			InvokeRepeating("ShootLaser",interval,interval);
		}

		//lineRenderer.enabled = startEnabled;

	}

	void ShootLaser(){
	//TODO
		lineRenderer.enabled=true;//!lineRenderer.enabled;
		SoundEffectsHelper.Instance.PlayElectricitySound();
		//initialTime+=Time.deltaTime;
	}

	void Update () {

	  if(lineRenderer.enabled){
		initialTime	+= Time.deltaTime;

		 if(initialTime > duration ){

		  //time to swap
		  lineRenderer.enabled=false;
		  initialTime=0f; 
		  
		}
	  }

		/*initialTime	+= Time.deltaTime;
		if(initialTime >  ){

		  //time to swap
		  lineRenderer.enabled=!lineRenderer.enabled;
		  initialTime=0f; 

		  if(lineRenderer.enabled){
		    SoundEffectsHelper.Instance.PlayElectricitySound();
		  }
		  
		}*/

		if(lineRenderer.enabled){




			lineRenderer.SetPosition(0,transform.localPosition);
		
			for(int i=1;i<4;i++)
			{
				Vector3 pos = Vector3.Lerp(transform.localPosition,targetObject.transform.localPosition,i/4.0f);
				
				pos.x += Random.Range(-0.4f,0.4f);
				pos.y += Random.Range(-0.4f,0.4f);
				
				lineRenderer.SetPosition(i,pos);
			}
			
			lineRenderer.SetPosition(4,targetObject.transform.localPosition);

			// To get a ray from point A in the direction of point B you can use point A as origin 
			//and (pointB - pointA).normalized as the direction.
			  Vector3 direction = targetObject.transform.localPosition - transform.localPosition;
			  direction = direction.normalized;
			  hit = Physics2D.Raycast(transform.localPosition, direction); 
				//ray = new Ray2D(StartPoint.position, LaserDir);

				if (hit && hit.collider.gameObject.tag.Equals("Jelly") ){
				    GameObject obj = GameObject.FindGameObjectWithTag("Player");
				    if(obj!=null) {
				      PlayerScript s = obj.GetComponent<PlayerScript>(); 
				      s.TakeLife();
					  Destroy(hit.collider.gameObject);
				    }
							
				}

		}



	}
}