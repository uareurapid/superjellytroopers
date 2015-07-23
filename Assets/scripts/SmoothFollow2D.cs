
using UnityEngine;
 using System.Collections;
 
 public class SmoothFollow2D : MonoBehaviour 
 {
	public float dampTime  = 0.3f; //offset from the viewport center to fix damping
    private Vector3 velocity = new Vector3(0f,0f,0f);
	public Transform target ;

	private Camera camera;
	public float xLocked = 6.459072f;

	Vector3 originalPosition;

	void Start() {
	  camera = Camera.main;
	  originalPosition = camera.transform.position;
	}
  
 void LateUpdate() {
     if(target!=null) {
		 Vector3 point = camera.WorldToViewportPoint(target.position);
		 Vector3 delta  = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
		 Vector3 destination = transform.position + delta;
  
        // Set this to the X position you want the camera locked to
		 destination.x = xLocked;// 6.459072f; 
  
         transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
     }
         

 } 

 public void ResetCameraPosition() {

    transform.position = originalPosition;
 }

     /*
     public float xMargin = 1f;        // Distance in the x axis the target can move before the camera follows.
     public float yMargin = 1f;        // Distance in the y axis the target can move before the camera follows.
     public float xSmooth = 8f;        // How smoothly the camera catches up with it's target movement in the x axis.
     public float ySmooth = 8f;        // How smoothly the camera catches up with it's target movement in the y axis.
     private Vector2 maxXAndY;        // The maximum x and y coordinates the camera can have.
     private Vector2 minXAndY;        // The minimum x and y coordinates the camera can have.
 
 
     public Transform target;        // Reference to the target's transform.
     private GameObject sky;
     public static bool cameraInit;
 
     void Awake ()
     {
         // Setting up the reference.
         //target = GameObject.FindGameObjectWithTag("target").transform;
         
         // Determine the bounds of the camera
         Camera cam = Camera.main;
         float camHeight = 2.0f * cam.orthographicSize;
         float camWidth = camHeight * cam.aspect;
 
        // sky = GameObject.FindGameObjectWithTag("sky");
         minXAndY.x = 100f;//sky.renderer.bounds.min.x + 0.5f * camWidth;
         maxXAndY.x = 100.0f;//sky.renderer.bounds.max.x - 0.5f * camWidth;
         minXAndY.y = 100.0f;//sky.renderer.bounds.min.y + 0.5f * camHeight;
         maxXAndY.y = 100.0f;//sky.renderer.bounds.max.y - 0.5f * camHeight;
 
         cameraInit = true;
 
     }
 
 
     bool CheckXMargin()
     {
         // Returns true if the distance between the camera and the target in the x axis is greater than the x margin.
         return Mathf.Abs(transform.position.x - target.position.x) > xMargin;
     }
 
 
     bool CheckYMargin()
     {
         // Returns true if the distance between the camera and the target in the y axis is greater than the y margin.
         return Mathf.Abs(transform.position.y - target.position.y) > yMargin;
     }
 
 
     void Update ()
     {
         // By default the target x and y coordinates of the camera are it's current x and y coordinates.
         float targetX = target.position.x;
         float targetY = target.position.y;
     
         // If the target has moved beyond the x margin...
         if(CheckXMargin())
             // ... the target x coordinate should be a Lerp between the camera's current x position and the target's current x position.
             targetX = Mathf.Lerp(transform.position.x, target.position.x, xSmooth * Time.deltaTime);
 
         // If the target has moved beyond the y margin...
         if(CheckYMargin())
             // ... the target y coordinate should be a Lerp between the camera's current y position and the target's current y position.
             targetY = Mathf.Lerp(transform.position.y, target.position.y, ySmooth * Time.deltaTime);
                 
 
         // The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
         targetX = Mathf.Clamp(targetX, minXAndY.x , maxXAndY.x);
         targetY = Mathf.Clamp(targetY, minXAndY.y , maxXAndY.y);
         
         
 
         // Set the camera's position to the target position with the same z component.
         transform.position = new Vector3(targetX, targetY, transform.position.z);
 
         if (cameraInit == true && Mathf.Abs(transform.position.x - minXAndY.x) < 0.01f)
         {
             cameraInit = false;
         }
     }*/
 
 }

 /*
http://answers.unity3d.com/questions/763270/issues-with-camera-follow-script-large-jitter-when.html

http://answers.unity3d.com/questions/38526/smooth-follow-camera.html

Unity 3.5.3: Assets->Import Package->Scripts. At the dialog that appears select all the scripts, or just the smooth follow one and hit Import button. Now this script is in your project. Simple.

#pragma strict
 
 var smooth : float = 3;
 var zOffset : float = 5000;
 var planetFollow : boolean = false;
 var FollowOn : boolean = false;
 private var shootObject : GameObject;
 private var planetScript : fire_planet;
 private var planetGone : setShotTrue;
 private var planetBorder : GameObject;
 var myCamPos : Vector3;
 
 
 function Start () {
 shootObject = GameObject.FindGameObjectWithTag("launcher");
 planetScript = shootObject.GetComponent(fire_planet);
 planetBorder = GameObject.FindGameObjectWithTag("blackHole");
 planetGone = shootObject.GetComponent(setShotTrue);
 myCamPos = transform.position;
 }
 
 function Update () {
  if (planetScript.planetShot == true){
     Follow();
     FollowOn = true;
     }
 }
 
 function Follow () {
 if (FollowOn == true){
     yield WaitForSeconds(2);
     var planetObject = GameObject.FindGameObjectWithTag("target");
     var planetPos : Vector3 = planetObject.transform.position;
     var targetPos : Vector3 = Vector3( planetPos.x, planetPos.y, planetPos.z + zOffset);
     transform.position = Vector3.Lerp ( transform.position, targetPos, smooth );
     transform.LookAt( planetObject.transform );
 }
 else if (FollowOn == false){
     transform.position = myCamPos;
     }
 }


 #pragma strict
 
 var target : GameObject;
 var spawnPoint : Transform;
 
 function OnTriggerEnter(other : Collider){
     Destroy(other.gameObject);
     var P : GameObject = Instantiate(target, spawnPoint.position, Quaternion.identity);
     var sf = Camera.main.GetComponent(SmoothFollow2);
     sf.target = P.transform;
 }
 
 
I die in my 2D game but my camera stays where i died and my target doesnt respawn?

spawncollider 2drespawner
more ▼
asked Jan 01 at 02:38 AM
shanereichenfeld gravatar image
shanereichenfeld 
0 ● 5 ● 10 ● 36
target points to the target Prefab? But this is in the Prefab itself, so it points to itself? Sure you point to the prefab and this isn't just a target pointing to itself? In which case it would respawn a dead target.
 */