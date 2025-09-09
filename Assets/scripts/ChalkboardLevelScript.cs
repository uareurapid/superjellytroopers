using UnityEngine;
using System.Collections;
using RescueJelly;
#if !UNITY_BLACKBERRY
using Soomla.Store;
#endif
using Soomla.MyStore;

public class ChalkboardLevelScript : MonoBehaviour {

	private GUISkin skin; 
	public Texture2D playTexture;
	public Texture2D reloadTexture;
	public Texture2D storeTexture;
	public Texture2D jellyIconTexture;
	public Texture2D lifesIconTexture;
	private GUIStyle style;
	public Font textFont;
	public int fontSize;
	Rect playTextureRect;
	Rect reloadTextureRect;
	Rect storeTextureRect;

	//default values here
	int world = 1;
	int level = 1;
	int lastWorld=1;
	private static RuntimePlatform platform = Application.platform;

	private TextLocalizationManager	translationManager;

	private bool buyedExtraLifes = false;
	private bool buyedExtraTime = false;
	private bool buyedExtraSpeed = false;
	private bool buyedInfiniteLifes = false;

	private int lifes = 0;
	private int time = 0;
	private int saved = 0;

	private int lifesScore = 0;
	private int timeScore = 0;
	private int savedScore = 0;

    GUIResolutionHelper resolutionHelper;
    SocialAPI socialAPIInstance;

	//to avoid destroy preferences
	private bool clickedStore = false;

	// Use this for initialization
	void Start () {
		skin = Resources.Load("GUISkin") as GUISkin;

		//these are the next world/level
		world = PlayerPrefs.GetInt(GameConstants.PLAYING_WORLD,1);
		level = PlayerPrefs.GetInt(GameConstants.PLAYING_LEVEL,1);
		platform = Application.platform;

		if(IsMobilePlatform()) {
			CheckInAppPurchases();
		}

		translationManager = TextLocalizationManager.Instance;
		translationManager.LoadSystemLanguage(Application.systemLanguage);

	}

	//check if we have the keys
	void CheckInAppPurchases() {
	   buyedExtraLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
	   buyedExtraTime = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
	   buyedExtraSpeed = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
	   buyedInfiniteLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);
	}

	void LoadStyle() {
		style = GUI.skin.GetStyle ("Label");
		style.alignment = TextAnchor.MiddleLeft;
		style.font = textFont;
		style.fontSize = fontSize;
		style.normal.textColor = Color.white;
		
	}
	
	void Awake() {

	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	  if(scripts!=null) {
		resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
		socialAPIInstance = scripts.GetComponent<SocialAPI>();
	  }
	  else {
	    resolutionHelper = GUIResolutionHelper.Instance;
	    socialAPIInstance = SocialAPI.Instance;
	  }

	  resolutionHelper.CheckScreenResolution();

	  //get the level just saved values
	  //number of saved jellies
	  saved = PlayerPrefs.GetInt(GameConstants.LEVEL_SAVED_JELLIES,0);
	  savedScore = PlayerPrefs.GetInt(GameConstants.LEVEL_SAVED_JELLIES_SCORE,0);
	  //number of remaining lifes
	  lifes = PlayerPrefs.GetInt(GameConstants.LEVEL_REMAINING_LIFES,0);
	  lifesScore = PlayerPrefs.GetInt(GameConstants.LEVEL_REMAINING_LIFES_SCORE,0);
	  //remaining level time
	  time = PlayerPrefs.GetInt(GameConstants.LEVEL_REMAINING_TIME_SECS,0);
	  timeScore = PlayerPrefs.GetInt(GameConstants.LEVEL_REMAINING_TIME_SECS_SCORE,0);

	  //get the last played world from the player prefs,
	  //if not set than player was still on first world (1)
	  lastWorld = GetLastPlayedWorld();

	}


	// Update is called once per frame  
	void Update() {



		if (Input.touches.Length ==1) {

				//
				int screenHeight = resolutionHelper.screenHeight;
				int screenWidth = resolutionHelper.screenWidth;

			    bool touchedPause = false;
				Touch touch = Input.touches[0];
				if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)  {
				
					
					Vector2 fingerPos = new Vector2(0,0);
					fingerPos = touch.position;
					
					fingerPos.y =  screenHeight - (touch.position.y / Screen.height) * screenHeight;
					fingerPos.x = (touch.position.x / Screen.width) * screenWidth;

					if(resolutionHelper.isWidescreen) {
						
					 float wideScreenOrigin = (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * screenWidth;
					 fingerPos.x = fingerPos.x + wideScreenOrigin - GameConstants.WIDESCREEN_CORRECTION_VALUE;

					}

					if(playTextureRect.Contains(fingerPos) )
					{	//load next level			
						LoadNextLevel(world,level);
					}
					else if(reloadTextureRect.Contains(fingerPos) )
					{	
					 //replay last world/level again
					  LoadNextLevel(lastWorld, (level-1 <=0) ? 1 : level-1);
		

					}
					else if(storeTextureRect.Contains(fingerPos) )
					{	
					  clickedStore=true;
					  Application.LoadLevel("StoreScene");
					}


				}
			}
	}

	/*
	* Gets the value of the last played world
	*/

	int GetLastPlayedWorld() {

		if(PlayerPrefs.GetInt(GameConstants.MISSION_4_KEY,0)>0) {
		  return 4;
		}
		if(PlayerPrefs.GetInt(GameConstants.MISSION_3_KEY,0)>0) {
		  return 3;
		}

		if(PlayerPrefs.GetInt(GameConstants.MISSION_2_KEY,0)>0) {
		  return 2;
		}

		if(PlayerPrefs.GetInt(GameConstants.MISSION_1_KEY,0)>0) {
		  return 1;
		}
		  

		return 1;
	}

	void OnGUI()
		{
		  


				if (style == null) {
					LoadStyle ();
				}
				// Set the skin to use
				GUI.skin = skin;

				Matrix4x4 svMat = GUI.matrix;//save current matrix

				int width = resolutionHelper.screenWidth;
				int height = resolutionHelper.screenHeight;
				Vector3 scaleVector = resolutionHelper.scaleVector;

				bool isWideScreen = resolutionHelper.isWidescreen;
				
				if (isWideScreen) {
					GUI.matrix = Matrix4x4.TRS (new Vector3 ((resolutionHelper.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);


				}
				else {
					GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scaleVector);

				}

				//check clicks, when playing on desktop env
				  if (Event.current.type == EventType.MouseUp && !IsMobilePlatform()) {
					  if(playTextureRect.Contains(Event.current.mousePosition)) {
					    LoadNextLevel(world,level);
					  } 
					  else if(reloadTextureRect.Contains(Event.current.mousePosition) ) {	
					    //start from last level again
						LoadNextLevel(lastWorld,(level - 1 <=0) ? 1 : level-1);
					 }
					else if(storeTextureRect.Contains(Event.current.mousePosition) ) {	
						clickedStore=true;
						Application.LoadLevel("StoreScene");
					 }
		 
		          }


				if(Event.current.type==EventType.Repaint) {
		
					style.fontSize -=5;

					GUI.Label (new Rect(width/2-150, height/3-120, 400, 60), 
					translationManager.GetText(GameConstants.MSG_CONGRATULATIONS),style);

					//number of saved jellies
					Rect jellyTextureRect = new Rect(width/2-220,height/3-40,64,64);
					GUI.DrawTexture(jellyTextureRect,jellyIconTexture);


					int levelTotal = 0 ;
					levelTotal = levelTotal + savedScore + lifesScore + timeScore;

					GUI.Label (new Rect(width/2-150,height/3-30 , 120, 40), " X " + saved ,style);

					style.normal.textColor = Color.green;
					GUI.Label (new Rect(width/2+150,height/3-30 , 140, 40),  "+ " + savedScore,style);

					style.normal.textColor = Color.white;
			

					//number of lives left
					Rect lifesTextureRect = new Rect(width/2-220,height/3 +30 ,64,64);

				
					GUI.DrawTexture(lifesTextureRect,lifesIconTexture);
					GUI.Label (new Rect(width/2-150,height/3+40 , 120, 40), " X " + lifes + "/" + GameConstants.NUM_LIFES_PER_LEVEL ,style);
					if(lifesScore>0) {//all lifes?
					  style.normal.textColor = Color.green;
					  GUI.Label (new Rect(width/2+150,height/3+40 , 140, 40), "+ " + lifesScore ,style);
					}
					else {
					  style.normal.textColor = Color.red;
					  GUI.Label (new Rect(width/2+150,height/3+40 , 120, 40), "- " + (lifesScore*-1) ,style);
					}

					style.normal.textColor = Color.white;


					//time left (seconds)
					GUI.Label (new Rect(width/2-220,height/3 +100 , 400, 50), " Time left: "+ time + " secs",style);
					if(timeScore>0) {
					    style.normal.textColor = Color.green;
						GUI.Label (new Rect(width/2+150,height/3 +100 , 400, 50), "+ " + timeScore,style);
					}
					else {
						style.normal.textColor = Color.red;
						GUI.Label (new Rect(width/2+150,height/3 +100 , 400, 50), "- " + (timeScore*-1),style);

					}



					style.normal.textColor = Color.white;

					GUI.Label (new Rect(width/2+130,height/3 +120 , 250, 50), "_______",style);

					style.fontSize -=15;
					GUI.Label (new Rect(width/2+130,height/3 +165 , 100, 50), "Score: ",style);
					style.fontSize +=15;
					GUI.Label (new Rect(width/2+220,height/3 +165 , 100, 50), ""+ levelTotal,style);



					//normal flow
					  GUI.Label (new Rect(width/2-220, (height/3 * 2)-50, 200, 50), "Next:",style);

					  style.fontSize +=5;
					  GUI.Label (new Rect(width/2-80, (height/3 * 2)-50, 200, 50), "World " + world,style);
					  GUI.Label (new Rect(width/2-80, (height/3 * 2), 200, 50), "Level " + level,style);
					//}

				  //Any In-App still available for purchase????
				  if( (!buyedExtraTime || !buyedExtraLifes || !buyedExtraSpeed || !buyedInfiniteLifes) && IsMobilePlatform()) {

					storeTextureRect = new Rect(width-110,30,96,96);
					GUI.DrawTexture(storeTextureRect,storeTexture);
				  }
				
				  
				  //play next without issues, 682 == width/3*2 = 1024/3*2 = 682 = 1024-682 = 342
				  playTextureRect = new Rect(width - 342,(height/3 * 2)-30,96,96);
				  GUI.DrawTexture(playTextureRect,playTexture);
					
				  //show always the replay
				  reloadTextureRect = new Rect(width - 342,(height/3)-150,96,96);
				  GUI.DrawTexture(reloadTextureRect,reloadTexture);



			

				 }



		    //restore the matrix
			GUI.matrix = svMat;
			    
		
		}
		
	void LoadNextLevel(int world, int scene) {
		//StartActivityMonitor();
		Application.LoadLevel("World" + world + "Scene" + scene);
		Destroy(gameObject);
	}

	/*
	void StartActivityMonitor()
    {
		#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.Gray);
        #elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle);
        #endif

        Handheld.StartActivityIndicator();

    }
    */

	//call this on Destroy
	void ClearPreferencesKeys() {

	  if(!clickedStore) {
	      //if is store click we will return to this screen, so we need to keep this
		  PlayerPrefs.DeleteKey(GameConstants.PLAYING_WORLD);
		  PlayerPrefs.DeleteKey(GameConstants.PLAYING_LEVEL);
	  }
		 

	  //for the score calculation, recreated every time the script is called
	  PlayerPrefs.DeleteKey(GameConstants.LEVEL_REMAINING_TIME_SECS);
	  PlayerPrefs.DeleteKey(GameConstants.LEVEL_REMAINING_LIFES);
	  PlayerPrefs.DeleteKey(GameConstants.LEVEL_SAVED_JELLIES);
	  PlayerPrefs.DeleteKey(GameConstants.LEVEL_REMAINING_TIME_SECS_SCORE);
	  PlayerPrefs.DeleteKey(GameConstants.LEVEL_REMAINING_LIFES_SCORE);
	  PlayerPrefs.DeleteKey(GameConstants.LEVEL_SAVED_JELLIES_SCORE);
	}

	private bool IsMobilePlatform() {

	  return platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android 
	  || platform == RuntimePlatform.BlackBerryPlayer || platform == RuntimePlatform.BlackBerryPlayer;
	}

	void OnDestroy() {
	  ClearPreferencesKeys();
	
	}


	


}
