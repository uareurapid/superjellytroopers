using UnityEngine;
using System.Collections;

public class MyBasicLaser : MonoBehaviour {


	public Transform StartPoint ;
	
	public string LaserDirection = "X";
	public enum Status { A, B, C };
	
	public bool LaserOn = true;    
	
	public bool UseUVPan = true;
	
	public float EndFlareOffset = 0.0f;
	
	public LensFlare SourceFlare;
	public LensFlare EndFlare;
	
	public bool AddSourceLight = true;
	public bool AddEndLight = true;
	
	public Color LaserColor = new Color(1f,1f,1f,0.5f);
	
	public float StartWidth = 0.1f;
	public float EndWidth = 0.1f;
	public float LaserDist = 20.0f;
	public float TexScrollX = -0.1f;
	public float TexScrollY = 0.1f;
	
	public float switchInterval = 2.0f;
	public float lastSwitchTime = 0.0f;
	public float duration = 1.5f;
	private float durationCounter = 0.0f;

	//laser sound
	private AudioSource laserAudio;
	
	private int SectionDetail  = 2;       
	private LineRenderer lineRenderer;
	private Ray2D ray;
	private Vector3 EndPos;
	private RaycastHit2D hit ;
	private GameObject SourceLight ;
	private GameObject EndLight ;
	private float ViewAngle;
	private Vector3 LaserDir ;
	
	
	private BoxCollider2D rayCollider;
	
	// Use this for initialization
	void Start () {
	
		Vector3 startVector = new Vector3(0f,0f,0f);
		Vector3 distanceVector = new Vector3(0f,1f,0f);
		ray = new Ray2D(startVector,distanceVector );	
	
		laserAudio = GetComponent<AudioSource>();
		rayCollider = GetComponent<BoxCollider2D>();
		//RaycastCollider.enabled = true;
		
		lineRenderer = GetComponent<LineRenderer>();
		if(lineRenderer.material.Equals("none"))
			lineRenderer.GetComponent<Renderer>().material = new Material (Shader.Find("LaserAdditive"));
		
		lineRenderer.castShadows = false;
		lineRenderer.receiveShadows = false;
		
		lineRenderer.SetVertexCount(SectionDetail);
		lineRenderer = GetComponent<LineRenderer>();
		
		// Make a lights
		if(AddSourceLight){
			StartPoint.gameObject.AddComponent<Light>();
			StartPoint.GetComponent<Light>().intensity = 1.5f;
			StartPoint.GetComponent<Light>().range = 0.5f;
		}
		
		if(AddEndLight){
			if(EndFlare){
				EndFlare.gameObject.AddComponent<Light>();
				EndFlare.GetComponent<Light>().intensity = 1.5f;
				EndFlare.GetComponent<Light>().range = 0.5f;		
			}
			else{Debug.Log("To use End Light, please assign an End Flare");}
		}
		
		
		if(LaserDirection.Equals("x") || LaserDirection.Equals("y") || LaserDirection.Equals("z") 
		   || LaserDirection.Equals("X") || LaserDirection.Equals("Y") || LaserDirection.Equals("Z")){     
		}
		else{
			Debug.Log("Laser Direction can only be X, Y or Z");
		}
	}
	
	// Update is called once per frame
	void Update () {
		lastSwitchTime +=Time.deltaTime;
		
		if(lastSwitchTime >= switchInterval) {

		    //time to switch
			LaserOn=!LaserOn;
			lastSwitchTime = 0.0f;
		}

		if(LaserOn) {
		  durationCounter+=Time.deltaTime;
		  if(durationCounter >=duration) {
		      LaserOn = false;
		      durationCounter = 0.0f;
		  }
		}


		if(LaserOn && laserAudio.enabled) {
			//make the sound
			AudioSource.PlayClipAtPoint(laserAudio.clip, StartPoint.position,0.03f);
		}
		
		
		float CamDistSource = Vector3.Distance(StartPoint.position, Camera.main.transform.position);
		float CamDistEnd = Vector3.Distance(EndPos, Camera.main.transform.position);		
		ViewAngle = Vector3.Angle(StartPoint.forward, Camera.main.transform.forward);		 
		
		if(LaserOn){
			lineRenderer.enabled = true;               
			lineRenderer.SetWidth(StartWidth,EndWidth);
			lineRenderer.material.color = LaserColor;
			
			//Flare Control
			if(SourceFlare){
				SourceFlare.color = LaserColor;
				SourceFlare.transform.position = StartPoint.position;
				
				if(ViewAngle > 155 && CamDistSource < 20 && CamDistSource > 0){
					SourceFlare.brightness = Mathf.Lerp(SourceFlare.brightness,20.0f,.001f);	
				}
				else{
					SourceFlare.brightness = Mathf.Lerp(SourceFlare.brightness,0.1f,.05f);
				}        
			}
			
			if(EndFlare){
				EndFlare.color = LaserColor;
				
				if(CamDistEnd > 20){
					EndFlare.brightness = Mathf.Lerp(EndFlare.brightness,0.0f,.1f);
				}
				else{
					EndFlare.brightness = Mathf.Lerp(EndFlare.brightness,5.0f,.1f);
				}
			}// end flare        
			
			//Light Control
			if(AddSourceLight)
				StartPoint.GetComponent<Light>().color = LaserColor;
			
			if(AddEndLight){
				if(EndFlare){
					EndFlare.GetComponent<Light>().color = LaserColor;
				}
			}       
			
			
			if(LaserDirection.Equals("x") || LaserDirection.Equals("X")){		
				LaserDir = StartPoint.right;
			}
			else if(LaserDirection.Equals("y") || LaserDirection.Equals("Y")){
				LaserDir = StartPoint.up;
			}
			else if(LaserDirection.Equals("z") || LaserDirection.Equals("Z")){
				LaserDir = StartPoint.forward;
			}
			else{
				LaserDir = StartPoint.forward;
			}
			
			/////////////////////Ray Hit
			//this was ok
			//ray = new Ray(StartPoint.position, LaserDir); 
			//rayCollider.enabled = true;
			
			//for 3D is:
			//(Physics.Raycast(ray, out hit, LaserDist)){	
			//ray = new Ray(StartPoint.position, LaserDir); 
			
			//and for 2D
			hit = Physics2D.Raycast(StartPoint.position, LaserDir); 
			ray = new Ray2D(StartPoint.position, LaserDir);
			if (hit && hit.collider.gameObject.tag.Equals("Jelly") ){	//why not player?
				      
				//EndPos = hit.point;	 		    
				
				/*if(EndFlare){
					EndFlare.enabled = true;
					
					if(AddEndLight){
						if(EndFlare){
							EndFlare.light.enabled = true;		    
						}
					}
					
					if(EndFlareOffset > 0)
						EndFlare.transform.position = hit.point + hit.normal * EndFlareOffset;
					else
						EndFlare.transform.position = EndPos;
				}	*/
				
					    
			    //check if player
				//if(hit.collider.gameObject.tag.Equals("Player") && LaserOn) {
				
				//Debug.Log("PLAYER HIT BY LASER!!!!!!!!!");

					SpecialEffectsHelper.Instance.PlayLaserExplosionEffect(hit.collider.transform.position);
					//SoundEffectsHelper.Instance.MakeExplosionSound();
					
					//leave a smoke trail
					SpecialEffectsHelper.Instance.PlaySmokeTrailEffect(hit.collider.transform.position);
					Destroy(hit.collider.gameObject);
					
				//}
				GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
				if(playerObj!=null) {
					PlayerScript script = playerObj.GetComponent<PlayerScript>();
					script.TakeAllLifes();
				}
					    	    	    	    
			}
			
			/**
			if (Physics.Raycast(ray, out hit, LaserDist)){	   
				     
				Debug.Log("SOMETHING WS HIT BY THE RAY");
				EndPos = hit.point;	 
				
				
				if(EndFlare){
					EndFlare.enabled = true;
					
					if(AddEndLight){
						if(EndFlare){
							EndFlare.light.enabled = true;		    
						}
					}
					
					if(EndFlareOffset > 0)
						EndFlare.transform.position = hit.point + hit.normal * EndFlareOffset;
					else
						EndFlare.transform.position = EndPos;
				}		    
			}*/
			else{
				
				if(EndFlare)
					EndFlare.enabled = false;	        
				
				if(AddEndLight){
					if(EndFlare){
						EndFlare.GetComponent<Light>().enabled = false;
					}
				}
				
				EndPos = ray.GetPoint(LaserDist);	        	        	        	        
			}//end Ray       
			
			
			//Debug.DrawLine (StartPoint.position, EndPos, Color.red);
			
			//Find Distance
			float dist = Vector3.Distance(StartPoint.position, EndPos);
			
			//Line Render Positions
			lineRenderer.SetPosition(0,StartPoint.position);
			lineRenderer.SetPosition(1,EndPos);
			
			//Texture Scroller
			if(UseUVPan){
				lineRenderer.material.SetTextureScale("_Mask", new Vector2(dist/4, .1f));
				lineRenderer.material.SetTextureOffset ("_Mask", new Vector2(TexScrollX*Time.time, TexScrollY*Time.time));
			}
			
		}
		else{
			//stop sound
			//rayCollider.enabled = true;
			if(laserAudio.enabled) {
				laserAudio.Stop();
			}
			
			
			lineRenderer.enabled = false;
			
			if(SourceFlare)
				SourceFlare.enabled = false;
			
			if(EndFlare)
				EndFlare.enabled = false;
			
			if(AddSourceLight)
				StartPoint.GetComponent<Light>().enabled = false;
			
			if(AddEndLight)
			if(EndFlare){
				EndFlare.GetComponent<Light>().enabled = false;	
			}	 
			
			
			
		}//end Laser On   
	}
	
	/////Icon
	void OnDrawGizmos () {
		Gizmos.DrawIcon(transform.position, "LaserIcon.psd", true);
	}
	
	/**
	/////////////////////Ray Hit
       if(Use2D){
        hit2D = Physics2D.Raycast(StartPoint.position, LaserDir); 
        var ray2 = new Ray2D(StartPoint.position, LaserDir);
        if (hit2D){	        
		       EndPos = hit2D.point;	 		    
			    
			   if(EndFlare){
			   EndFlare.enabled = true;
			    
			   if(AddEndLight){
			   if(EndFlare){
			   EndFlare.light.enabled = true;		    
		     }
		    }
		      		    
		    if(EndFlareOffset > 0)
		    EndFlare.transform.position = hit2D.point + hit2D.normal * EndFlareOffset;
		    else
		    EndFlare.transform.position = EndPos;
		    }		    
          }
        else{
	        if(EndFlare)
	        EndFlare.enabled = false;	        
	        
	        if(AddEndLight){
		     if(EndFlare){
		     EndFlare.light.enabled = false;
		     }
		    }
		    
           EndPos = ray2.GetPoint(LaserDist);	        	        	        	        
         }
       	}
       	///Else 3D Ray
	*/
}






