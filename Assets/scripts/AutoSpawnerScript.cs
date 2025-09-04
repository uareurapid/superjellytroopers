using UnityEngine;
using System.Collections;

public class AutoSpawnerScript : MonoBehaviour
{
	public float spawnTime = 5f;		// The amount of time between each spawn.
	public float spawnDelay = 2f;		// The amount of time before spawning starts.
	public GameObject[] enemies;		// Array of enemy prefabs.


    public bool canSpawn = false;
    
	//margin regarding the transform position
	public int marginUp = 5;
	public int marginDown = 5;
	public int marginLeft = 0;
	public int marginRight = 0;
	
	GUISkin skin;
	
	private GameControllerScript controller;
	

	void Start ()
	{
		// Start calling the Spawn function repeatedly after a delay .
		skin = Resources.Load("GUISkin") as GUISkin;

		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		controller = scripts.GetComponent<GameControllerScript>();
		InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}
	
	void Awake() {
	   
	   GUIResolutionHelper.Instance.CheckScreenResolution();
	}
	


	public void Spawn ()
	{
	  //check also if controller action is not stopped
	
	  //stop spawining if we are stopped
	  if(canSpawn && !controller.IsGameOver()) {
	 
	  

		int enemyIndex = Random.Range(0, enemies.Length);
		Instantiate(enemies[enemyIndex], transform.position, transform.rotation);
			

			
			//canSpawn = false;
			// Play the spawning effect (if any) from all of the particle systems.
			foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
			{		    
				p.transform.position = transform.position;
				p.Play();
			}
			//SoundEffectsHelper.Instance.MakeRandomizeSound();
	  }
		
	}
	
	void Update() {
		/*if(controller.numberOfSavedJellies < controller.numberOfJelliesToRescue) {
			canSpawn = true;
		}
		else {
		    canSpawn = false;
		}*/
	}
	
	void OnGUI(){
		//GUI.skin = skin;
		
		//Matrix4x4 svMat = GUI.matrix;//save current matrix
		
		//int width = GUIResolutionHelper.Instance.screenWidth;
		//Vector3 scaleVector = GUIResolutionHelper.Instance.scaleVector;
		//bool isWideScreen = GUIResolutionHelper.Instance.isWidescreen;
		
		//if(isWideScreen) {
		//	GUI.matrix = Matrix4x4.TRS(new Vector3( (GUIResolutionHelper.Instance.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			
			
		//}
		//else {
			
		//	GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
		//}
		
		//DrawText("Next: ", controller.messagesFontSizeSmaller +10,800, 10,200,50);
		
		//if(nextThumbnail>-1) {//TODO &&player still alive
		//	Rect next = new Rect(870,20,32,32);
		//	GUI.DrawTexture(next, thumbnails[nextThumbnail]);
		//}
		
		
		//restore the matrix
		//GUI.matrix = svMat;

	}
	
	public void DrawText(string text, int fontSize, int x, int y,int width,int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = controller.messagesFont;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label (new Rect(x, y, width, height), text);
	}
	
	void OnBecameVisible() {
		canSpawn = true;
	}
	
	void OnBecameInvisible() {
		canSpawn = false;
	}
}
