using UnityEngine;
using System.Collections;

public class StorySceneScript : MonoBehaviour {

   GUISkin skin;
   public Texture2D exitTexture;
   private Rect exitTextureRect;
   int screenHeight;
   int screenWidth;
   bool alreadyIncreasedPlaneSpeed = false;

   GUIResolutionHelper resolutionHelper;
   private bool isMobilePlatform = false;
   private static RuntimePlatform platform = Application.platform;

	// Use this for initialization
	void Start () {
		skin = Resources.Load("GUISkin") as GUISkin;
		Debug.Log("StorySceneScript start()");
		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);

	}
	
	// Update is called once per frame

	void StartGame() {
		Application.LoadLevel("World1Scene1");
	}

	void Awake() {
	  //check screen settings
	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");


	  resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
	  resolutionHelper.CheckScreenResolution();
	  screenHeight = resolutionHelper.screenHeight;
	  screenWidth = resolutionHelper.screenWidth;

	  //just to initiate the script, and try authentication if was destroyed by accident
	  SocialAPI.Instance.LoadMe();
	  
	}

	void Update() {

	 if(Time.timeSinceLevelLoad >=10f && !alreadyIncreasedPlaneSpeed) {
	    GameObject plane = GameObject.FindGameObjectWithTag("Plane");
	    if(plane!=null) {
	        MoveScript script = plane.GetComponent<MoveScript>();
		    if(script!=null) {
		      script.speed.x = script.speed.x *6;
		      script.speed.y=3f;
		      script.direction.y=1f;
		      alreadyIncreasedPlaneSpeed = true;
		    }
	    }
	  }

	  if(isMobilePlatform) {
		if (Input.touches.Length ==1) {
	

			    bool touchedPause = false;
				Touch touch = Input.touches[0];
				if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)  {
				
					
				Vector2 fingerPos = new Vector2(0,0);
				fingerPos = touch.position;
					
				fingerPos.y =  screenHeight - (touch.position.y / Screen.height) * screenHeight;
					fingerPos.x = touch.position.x - screenWidth;  //(touch.position.x / Screen.width) * screenWidth;

				// if(resolutionHelper.isWidescreen) {
						
				// 	fingerPos.x = fingerPos.x + (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * screenWidth;
				// }
					

					if(exitTextureRect.Contains(fingerPos) )
					{	//load next scene			
						StartGame();
					}
				}
			}
		}

	}
	
	void OnGUI() {
	
	    int width = resolutionHelper.screenWidth;
	    int height = resolutionHelper.screenHeight;
	    
		Vector3 scaleVector = resolutionHelper.scaleVector;
		bool isWideScreen = resolutionHelper.isWidescreen;
	    
	    GUI.skin = skin;
		Matrix4x4 svMat = GUI.matrix;//save current matrix

		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (resolutionHelper.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			
			
		}
		else {
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
			
		}




		if(Time.timeSinceLevelLoad >=25f) {
		      StartGame();
		}

		if(Time.timeSinceLevelLoad >=12f) {
		      exitTextureRect = new Rect(width - 120,50, 96,96);
		      GUI.DrawTexture(exitTextureRect,exitTexture);
		}

		//disable the speech bubble
		if(Time.timeSinceLevelLoad > 6f) {


			if(Time.timeSinceLevelLoad < 12f) {

				GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
				if(jelly!=null) {
				   SpeechBubbleScript speech = jelly.GetComponent<SpeechBubbleScript>();
				   if(speech!=null && speech.enabled) {
				     speech.enabled = false;
				     //now enable the other one
						GameObject jellyBlue = GameObject.FindGameObjectWithTag("Brave");//i tagged it Brave
						if(jellyBlue!=null) {
				   			SpeechBubbleScript speechBlue = jellyBlue.GetComponent<SpeechBubbleScript>();
				   			if(speechBlue!=null && !speechBlue.enabled) {

								speechBlue.enabled = true;
				   			}
				   		}
				     //----------------------

				   }
				}
			}
			else {
			    //now disable the blue one speech
				GameObject jellyBlue = GameObject.FindGameObjectWithTag("Brave");//i tagged it Brave
				if(jellyBlue!=null) {
				   	SpeechBubbleScript speechBlue = jellyBlue.GetComponent<SpeechBubbleScript>();
				   	if(speechBlue!=null && speechBlue.enabled) {

						speechBlue.enabled = false;
				   	}
				}
			}
		  


		}
		
		if(!isMobilePlatform ) {

		  if(Event.current.type == EventType.MouseUp) {
			  if(exitTextureRect.Contains(Event.current.mousePosition)) {
					//load next scene			
					StartGame();
			  }
		  }

		  if(Input.GetKeyDown(KeyCode.Escape)) {
		  	Application.Quit();
		  }


		}


		GUI.matrix = svMat;
	}

	//TODO credit cvhttp://www.freesfx.co.uk/soundeffects/airplanes/


}
