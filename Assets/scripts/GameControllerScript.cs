using UnityEngine;
using System.Collections;
using RescueJelly;

public class GameControllerScript : MonoBehaviour {

	private static RuntimePlatform platform = Application.platform;
	public bool isMobilePlatform = false;
	private static GameControllerScript instance;
	
	//number of energy star pickups spread in level
	public int numberEnergyPickups = 20;

	public Texture2D pauseIcon;
	public Texture2D playIcon;

	private Texture2D rateTexture;
	private Texture2D exitTexture;
	private Texture2D helpMeTexture;
	public Texture2D clock;

	private Texture2D smallTrooper;
	Rect smallTrooperRect;
	
	public int screenWidth;
	public int screenHeight;
	
	Rect pausePlayRect;
	Rect exitTextureRect;
	Rect leaderboardsRect;
	Rect rateRect;
	
	public bool isGamePaused = true;
	private bool isGameStarted = false;
	private bool isGameOver = true;
	private bool isGameComplete = false;

	private GUITexture redTexture;
	
	//public bool isRestart = false;
	
	public Font messagesFont;
	public int messagesFontSizeSmaller;
	public int messagesFontSizeLarger;
	
	//when hurry up, increase scrolling speed of platforms by 1.8
	const float HURRY_UP_SPEED_INCREASE_FACTOR = 1.8f;
	
	//whne to hurry up 1/4 of total mission seconds
	const float HURRY_UP_START_FACTOR = 0.25f;
	
	
	private bool appliedHurryUpFactor = false;
	
	private string hurryUpMessage = "Hurry Up!";
    private float initialHurryUpMessageTime = 0f;
	private bool isShowingHurryUpMessage = false;
    private bool hasMovedSpikesLine = false;
	
	//controll first level howTo

	private float lastHowToTime = 0f;
	private float initialHowToTime = 0f;
	private bool isShowingHowTo = false;

	private GUISkin skin;
	
	public int numWorlds = 4;
	public int numberOfLevels = 10;
	public int currentLevel = 1;
	public int currentWorld = 1;
	//number of minutes to complete the mission
	private int missionTimeInMinutes = 0;
	public int missionTimeInSeconds = 120;

	//display time
	private int elapsedMissionMinutes = 0;
	private int elapsedMissionSeconds = 0;
	private int totalRemainingMissionTimeInSeconds = 0;
	
	public int numberOfJelliesToRescue = 10;
	public int numberOfSavedJellies = 0;
	
	//show in app for level xxx?
	private bool showUnlockLevel = true;
	
	public bool isJellyFalling = false;
	
	//private Rect screenshotTextureRect;
	//private Texture2D screenshotTexture;
	
	private int currentTime = 0;

	
	//ads stuff
	
	//private BannerView bannerView;


	public bool isRollingFinalCredits = false;

	PlayerScript player;
	GameObject motherShip;
	
	bool buyedPremium;
	bool buyedNoads;

	MainPlatformScript jumpPlatform;
	
	private bool openedPlatform = false;
	GUIResolutionHelper resolutionHelper;

	private TextLocalizationManager translationManager;

	private int highScore = 0;

	Texture2D leaderBoardTexture;


	private SocialAPI socialAPIInstance;
	private	bool buyedExtraLifes = false;
	private bool buyedExtraTime = false;
	private bool buyedExtraSpeed = false;
	private bool buyedInfiniteLifes = false;

	
	void Awake()
	{
	
	    //DontDestroyOnLoad(this);
	    
	    if(instance!=null) {
	      Debug.Log("There is another instance gamecontroller running");
	    }
	    else {
		  instance = this;
	    } 
		

		//check screen size, this was breaking, probably some place we are calling Instance
		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		if(scripts!=null) {
			resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
			translationManager = scripts.GetComponent<TextLocalizationManager>();
		}
		else {
			resolutionHelper = GUIResolutionHelper.Instance;
			//handle translation language
		    translationManager = TextLocalizationManager.Instance;
		
		}
		screenWidth = resolutionHelper.screenWidth;
		screenHeight = resolutionHelper.screenHeight;
				
		//translations
		translationManager.LoadSystemLanguage(Application.systemLanguage);
		
		InitPlayer();

	
		//get a reference to the object
		socialAPIInstance = SocialAPI.Instance;
	  
	  	 
	  	//set the score key pref, if not set yet
		if(!PlayerPrefs.HasKey(GameConstants.HIGH_SCORE_KEY)) {
			PlayerPrefs.SetInt(GameConstants.HIGH_SCORE_KEY,0);
		}
		else {
			//0 is the default highScore (only show if not game started yet)
			highScore = PlayerPrefs.GetInt(GameConstants.HIGH_SCORE_KEY,0);
		}
	
		PlayerPrefs.SetInt(GameConstants.CURRENT_WORLD_KEY,currentWorld);
		PlayerPrefs.SetInt(GameConstants.CURRENT_LEVEL_KEY,currentLevel);

		openedPlatform = false;
		
		showUnlockLevel = false;
		isGameOver = true;
		isGameStarted = false;
		//this was true before
		isGamePaused = true;
		isRollingFinalCredits = false;

		//we need to do this before we do the time math, so we can update in case of an existing in app purchase
		CheckInAppPurchases();

		missionTimeInMinutes = (int)missionTimeInSeconds / 60 ;

		//minutes to display
		elapsedMissionMinutes = missionTimeInMinutes;
		//seconds to display
	    elapsedMissionSeconds = (int)missionTimeInSeconds % 60;

		totalRemainingMissionTimeInSeconds = missionTimeInSeconds;

		isJellyFalling = false;
		CheckPause();

		lastHowToTime = 0f;
		initialHowToTime = 0f;
		isShowingHowTo = false;

	}
	
	// Use this for initialization
	void Start () {
	
		skin = Resources.Load("GUISkin") as GUISkin;
		exitTexture	= Resources.Load("button_playstart") as Texture2D;
		helpMeTexture = Resources.Load("tapme") as Texture2D;
		clock = Resources.Load("relogio") as Texture2D;
		rateTexture = Resources.Load("button_rate") as Texture2D;
		smallTrooper = Resources.Load("small_trooper") as Texture2D;

		GameObject temp = GameObject.FindGameObjectWithTag("RedTexture");
		if(temp!=null) {
			redTexture =  temp.GetComponent<GUITexture>();
		    redTexture.color = new Color32(255, 255, 255, 0);
		}
		#if UNITY_IPHONE || UNITY_STANDALONE_OSX && !UNITY_EDITOR
		leaderBoardTexture = Resources.Load("gamecenter") as Texture2D;
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		leaderBoardTexture = Resources.Load("play_games_green") as Texture2D;
		#endif

		isGameComplete = false;
		//??
		currentTime = missionTimeInMinutes;
		
		appliedHurryUpFactor = false;
		hasMovedSpikesLine = false;
		
		CenterJellyAndSpawner();
		isGameOver = true;
		if(isGameOver && currentWorld==1 && currentLevel==1 && helpMeTexture!=null) {

			initialHowToTime = Time.realtimeSinceStartup;
			lastHowToTime = initialHowToTime;
		}

		//isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);

		
	}

	/**
	* Check if we have bought any boosters
	*/
	void CheckInAppPurchases() {
	   buyedExtraLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
	   buyedExtraTime = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
	   buyedExtraSpeed = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
	   //infinite lifes
	   buyedInfiniteLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);

	   //On desktop i can have a single life added, when i colide with it, and it stays like an in app purchase
	   bool addSingleLife = isMobilePlatform && PlayerPrefs.HasKey(GameConstants.JELLY_TROOPERS_EXTRA_LIFE_SINGLE_PRODUCT_ID);

	   if(buyedExtraTime) {
	     missionTimeInSeconds+=GameConstants.IN_APP_PURCHASE_EXTRA_TIME_IN_SECONDS;
	   }

	   if(buyedExtraSpeed) {
	     MoveScript move = player.gameObject.GetComponent<MoveScript>();
	     if(move!=null) {
	       move.speed.x = move.speed.x * GameConstants.IN_APP_PURCHASE_EXTRA_SPEED_INCREASE_FACTOR;
	     }
	   }


		GameObject obj = GameObject.FindGameObjectWithTag("Player");
		if (obj != null) {
				HealthScript health = obj.GetComponent<HealthScript>();
				if(health!=null) {
				//make sure is set to 4
				health.hitPoints = GameConstants.NUM_LIFES_PER_LEVEL;

				//than increase if necessary
					if(buyedInfiniteLifes) {
						health.AddHitPoints(GameConstants.IN_APP_PURCHASE_INFINITE_LIFES_COUNT);
					}
					else if(buyedExtraLifes) {
						health.AddHitPoints(GameConstants.IN_APP_PURCHASE_EXTRA_LIFES_COUNT);
					}
					else if(addSingleLife) {
					    health.AddHitPoints(1);
					}
			}
		}
	    
	}

	IEnumerator Fade (float start,float end, float length) {

	 Color aux = redTexture.color;
		  //define Fade parmeters
		if (aux.a == start){

		  for (float i = 0.0f; i < 1.0f; i += Time.deltaTime*(1/length)) { 
		   //for the length of time
		   aux.a = Mathf.Lerp(start, end, i); 
		   //lerp the value of the transparency from the start value to the end value in equal increments
		   yield return null;
		   aux.a = end;
		  // ensure the fade is completely finished (because lerp doesn't always end on an exact value)
          redTexture.color = aux;
          } //end for
 
		} //end if
	
 
	} //end Fade

	/*
	The above example (in FlashWhenHit) will fade your texture from 100% transparent 
	(invisible) to 80% opaque (just slightly transparent) over 1/2 second. 
	It checks to make sure the texture is 100% transparent before attempting the fade 
	to eliminate visual errors, and at the end ensures it is at exactly 80% opacity. 
	It will then wait 1/100th second, and fade the texture back out to transparent. 
	You can, of course, adjust the starting and ending opacity by changing the start 
	and end values in the function call, as well as how long the fade takes and what object if affects. 
	The WaitForSeconds is in there so the texture will stay at its max opacity momentarily 
	(to make it more visually obvious); the length of time is adjustable there too. 
	Also, if you want the screen to flash a certain number of times, 
	you could use a for loop with a counter that goes to 0 from, say, 3, to get the screen to flash 3 times, etc.
	*/

	void FlashWhenInHurryUpMessage (){


		StartCoroutine(Fade (0f, 0.1f, 0.5f));
		StartCoroutine(MyWaitMethod());
		StartCoroutine(Fade (0.1f, 0f, 0.5f));
	
    	
    }

	IEnumerator MyWaitMethod() {
		yield return new WaitForSeconds(.01f);
	}
	
	//setup player stuff
	void InitPlayer() {
	  GameObject obj = GameObject.FindGameObjectWithTag("Player");
	  if(obj!=null) {
		 player = obj.GetComponent<PlayerScript>();
	  }
	  
		
	}
	
	void CenterJellyAndSpawner() {
	

		GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
		if(spawner!=null) {
			spawner.transform.localPosition =  Camera.main.ScreenToWorldPoint(
					new Vector3(Screen.width/2,Screen.height-120 ,Camera.main.nearClipPlane+25));//

			 GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
			 if(jelly==null) {
		       spawner.GetComponent<SpawnerScript>().SpawnFirstJelly();
			 
		     }
		}

	}
		
	
	public void JellyLanded() {
		isJellyFalling = false;
		numberOfSavedJellies+=1;
		
	}
	
	public void JellyDied() {
		isJellyFalling = false;
		
	}
	
	//invoked every second
	void CheckMissionTime() {

	if(!isGamePaused && player!=null && player.IsPlayerAlive()) {

	 totalRemainingMissionTimeInSeconds -=1;


		//time is up! player dead!
	    if(elapsedMissionMinutes==0 && elapsedMissionSeconds==0) {
	       //dead for good!!!!
	       player.HandleLooseAllLifes();
	    }
	    else {
					
					bool changeMinutes = false;
					elapsedMissionSeconds-=1;
					
					if(elapsedMissionSeconds<0) {
						
						if(elapsedMissionMinutes>0) {
							elapsedMissionSeconds = 59;
							changeMinutes = true;
						}
						else {
						 changeMinutes = false;
						 elapsedMissionSeconds = 0;
						}
						
					}
					
					if(changeMinutes && elapsedMissionMinutes>0) {
						elapsedMissionMinutes-=1;
					}
	    }
	 
		
		
		
	 //}
		 
						
	 }//if !gamePaused
		
		
		
	}
	/**
	* Add extra seconds
	*/
	public void	IncreaseTimeSecondsBy(int seconds) {
	//do we overlap the min?
		if(elapsedMissionSeconds + seconds > 59) {

		   bool changeMinutes = true;
		   int overlapAmount = elapsedMissionSeconds + seconds - 59;

		   if(overlapAmount==1) {
		     elapsedMissionSeconds = 0;
		   }
	       else {
			 elapsedMissionSeconds=overlapAmount;
	       }

		   elapsedMissionMinutes+=1;
		  
		}
	    else {
	    //increase normally, since we are still on the same minute
			elapsedMissionSeconds+=seconds;
	    }

	  
	}
	
	public int GetCurrentLevel() {
	  return currentLevel;
	}
	
	public int GetCurrentWorld() {
		return currentWorld;
	}
	
	public void SetCurrentLevel(int level) {
	  currentLevel = level;
	}
	
	public int GetNumberOfLevels() {
	  return numberOfLevels;
	}
	
	public static GameControllerScript Instance {

		get
		{
			if (instance == null)
			{
				GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
			    if(scripts!=null) {
					instance = scripts.GetComponentInChildren<GameControllerScript>();
			    }
			    else {
					instance = (GameControllerScript)FindObjectOfType(typeof(GameControllerScript));
					if (instance == null)
						instance = (new GameObject("GameControllerScript")).AddComponent<GameControllerScript>();
			    }
				
			}
			return instance;
		}
	}
	
	public bool IsGameStarted() {
	  return isGameStarted;
	}
	
	public bool IsShowUnlockNextLevel() {
	  return showUnlockLevel;
	}


	//#########  music handling ################

	public void StartMusic() {
    
	   AudioSource source = GetGameMusic();
	   if(source!=null) {
		  source.Play();
	   }

    }

	public void PauseMusic() {
    
		AudioSource source = GetGameMusic();
		if(source!=null) {
			source.mute = true;
		}
		

    }

	public void ResumeMusic() {
    
		AudioSource source = GetGameMusic();
		if(source!=null) {
			source.mute = false;
		}

    }
    
	public void StopMusic() {		
	
		AudioSource source = GetGameMusic();
		if(source!=null) {
			source.Stop();
		}

		foreach(AudioSource sourceAudio in GetGameAudios()) {
		  if(sourceAudio.isPlaying) {
		   sourceAudio.Stop();
		  }
		}

		
	
	}

	private AudioSource[] GetGameAudios() {
		AudioSource []audios = FindObjectsOfType<AudioSource>() as AudioSource[];
		return audios;
	}

	private AudioSource GetGameMusic() {
		GameObject music = GameObject.FindGameObjectWithTag("GameMusic");
		if(music!=null) {
			AudioSource source = music.GetComponentInChildren<AudioSource>();
			return source;
		}
	    return null;
	}

	//############################


	
	void FixedUpdate()
	{
		
	}
		
	
	//can we spawn?
	public bool SpawnAllowed() {
	  GameObject jelly = GetJellyObject();
	  if(jelly==null) {
		return (numberOfSavedJellies < numberOfJelliesToRescue) && !isGameOver && !isGamePaused && !isGameComplete;
	  }
	  //cannot spawn because there is a jelly in scene
	  return false;


	}
	
	// Update is called once per frame
	void Update () {

		if(isGameStarted && player!=null && player.IsPlayerAlive() && !isGameComplete) {
		
		   if(numberOfSavedJellies==numberOfJelliesToRescue) {
		     ShowNextScreen();
		     return;
		   }
		   
		   //check hurry up factor
			if(totalRemainingMissionTimeInSeconds>0 && !appliedHurryUpFactor) {
				float elapsedPortion = (float)totalRemainingMissionTimeInSeconds / missionTimeInSeconds;
				if(elapsedPortion <=HURRY_UP_START_FACTOR) {

				    SpeedUpForegroundPlatforms();
					
				}
			}

			if(appliedHurryUpFactor) {
              initialHurryUpMessageTime += Time.deltaTime;

              //blink hurry up message every 2 seconds
				if(initialHurryUpMessageTime >= 2.0f ) {
					
					initialHurryUpMessageTime = 0f;
					isShowingHurryUpMessage = !isShowingHurryUpMessage;
					
				}

			  //YOU´RE DEAD DUDE!!!
			 //get the spikes line moving up!!!!
			 if(totalRemainingMissionTimeInSeconds <=6 && !hasMovedSpikesLine) {
			   GameObject spikesLine = GameObject.FindGameObjectWithTag("SpikesLine");
			   if(spikesLine!=null) {
			     ScrollingScript scroll = spikesLine.GetComponent<ScrollingScript>();
			     if(scroll!=null && !scroll.enabled) {
				   StartCoroutine(ShowMessage(GetTranslationKey(GameConstants.MSG_INTRUSION_ALERT), 1.5f));
			       scroll.enabled = true;
			       hasMovedSpikesLine = true;
				   
			     }
			   }
			 }
			
			}
			
		
		
		
		   
			// 5 - Shooting
			bool jump = false;

			bool isMobileEnv = IsMobilePlatform();
			//if not mobile get keyboard strokes
			if(!isMobileEnv) {

	
				jump = Input.GetButtonDown("Fire1");//press and release, GetButton is no release needed
				jump |= Input.GetButtonDown("Fire2");

				bool moveRight = Input.GetAxis("Horizontal") > 0; // gets right
				bool moveLeft = Input.GetAxis("Horizontal") < 0; // gets left

				// Careful: For Mac users, ctrl + arrow is a bad idea
				
				if(jump && !isJellyFalling ) { //&& !openedPlatform
				   StartJellyFall();
				}
				else if(isJellyFalling) {
					if(DetectDesktopJellyTouches(moveLeft,moveRight)) {
						ReleaseParachute();
					}
				   
				}


				if(Application.platform == RuntimePlatform.IPhonePlayer) {
				  DetectJellyTouches();
				}
				
				
				
			}
			//else {
			//  DetectJellyTouches();
			//}
			//touches on textures are handled on OnGUI()
			
		}//end is game started


		if(!isMobilePlatform && Input.GetKeyDown(KeyCode.Escape)) {
		  Application.Quit();
		}
		

	}

	private void SpeedUpForegroundPlatforms() {
		GameObject foreground = GameObject.FindGameObjectWithTag("Foreground");
		if(foreground!=null) {
			ScrollingScript script = foreground.GetComponent<ScrollingScript>();
			if(script!=null && script.enabled) {
				script.speed.x = script.speed.x * HURRY_UP_SPEED_INCREASE_FACTOR;
				appliedHurryUpFactor = true;

			}
		}
	 //also speedup any speedable object
	 SpeedUpSpeedables();
	}

	private void SpeedUpSpeedables() {
		GameObject [] allSpeedables = GameObject.FindGameObjectsWithTag("SpeedableRotator");
		foreach(GameObject speedable in allSpeedables) {
			Rotator script = speedable.GetComponent<Rotator>();
			if(script!=null && script.enabled) {
			  script.rotateSpeed = script.rotateSpeed * HURRY_UP_SPEED_INCREASE_FACTOR;
			}
		}
	}


	//invoked when final boss is destroyed
	public void CompletedGame() {
	  isGameComplete = true;
	  //disable camera follow
	  SmoothFollow2D cameraFollow = Camera.main.GetComponent<SmoothFollow2D>();
	  if(cameraFollow!=null) {
	    cameraFollow.enabled = false;
	  }
	  ShowNextScreen();
	}
	
	private void ShowNextScreen() {
		
		bool showNext = (currentLevel < numberOfLevels  || currentWorld < numWorlds);
		EndGame(showNext);

		//either we died or reached last level, guru time!
		if(!showNext) {


		   //save before adding game over, so they are available immediatelly on Awake()
		   PlayerPrefs.SetInt(GameConstants.PLAYING_WORLD,currentWorld);
		   PlayerPrefs.SetInt(GameConstants.PLAYING_LEVEL,currentLevel);
		   //PlayerPrefs.Save();

		   gameObject.AddComponent<GameOverScript>();

			//Congratulations, You are officially a Guru!!!!
			if(IsFinalLevel()) {
				PlayerPrefs.SetInt(GameConstants.ACHIEVEMENT_GURU_KEY,1);
				PlayerPrefs.SetInt(GameConstants.MISSION_4_KEY,1);
				if(socialAPIInstance.isAuthenticated) {
					socialAPIInstance.AddAchievement(GameConstants.ACHIEVEMENT_GURU_KEY,100f);
				}
					
			}
			SaveTotalNumberOfTroopers(numberOfSavedJellies);
			PerformFinalComputation(true);

		}
		else {

		  //this is for the high score
		  SaveTotalNumberOfTroopers(numberOfSavedJellies);

		  if(currentLevel < numberOfLevels) {
		  	//just increase the level on the same world
		    currentLevel+=1;

		  }
		  else {
		  		//save the mission, completed the world
			    switch(currentWorld) {
				 case 1: PlayerPrefs.SetInt(GameConstants.MISSION_1_KEY,1);//COMPLETED MISSION 1
			     	break;
			     case 2: PlayerPrefs.SetInt(GameConstants.MISSION_2_KEY,1);//COMPLETED MISSION 2
			     	break;
			     case 3: PlayerPrefs.SetInt(GameConstants.MISSION_3_KEY,1);//COMPLETED MISSION 3
					break;
				 case 4: PlayerPrefs.SetInt(GameConstants.MISSION_4_KEY,1);//COMPLETED MISSION4
				 	break;
			    }

			  //increase world, set first level
		      currentWorld+=1;
		      currentLevel=1;
		    }
				
		  //------------- for the missions selection -----------------
		  string worldKey = GetWorldKey ();	
		  string levelKey = GetLevelKey ();
		  //this will be retrieved in the missions selection
		  PlayerPrefs.SetString(worldKey+levelKey,"worldKey+levelKey");

		  //these values keep the next in line
		  PlayerPrefs.SetInt(GameConstants.PLAYING_WORLD,currentWorld);
		  PlayerPrefs.SetInt(GameConstants.PLAYING_LEVEL,currentLevel);
		    
		  PerformFinalComputation(false);
		  //show board and do the math :-)
		  Application.LoadLevel("NextLevelScene");


		  }
		  	
		
	}

	void SaveTotalNumberOfTroopers(int toAdd) {
		int currentSavedSoldiers= PlayerPrefs.GetInt(GameConstants.TOTAL_SAVED_TROOPERS_KEY,0);
		//TOTAL_SAVED_TROOPERS_KEY

		string wk = GetWorldKey ();	
		string lk = GetLevelKey ();
		//this will be retrieved in the missions selection
		bool containsThisLevelAndWorld = PlayerPrefs.HasKey(wk+lk+"completed");
		if(!containsThisLevelAndWorld){

			currentSavedSoldiers += toAdd;
			//OK to add, we do not have this key yet
			PlayerPrefs.SetInt(wk+lk+"completed",currentSavedSoldiers);
			//and we increase the number of saved soldiers
			PlayerPrefs.SetInt(GameConstants.TOTAL_SAVED_TROOPERS_KEY,currentSavedSoldiers);

			if(socialAPIInstance.isAuthenticated && toAdd>0) {
				socialAPIInstance.ReportScore(currentSavedSoldiers,GameConstants.LEADERBOARD_MORE_SAVED_TROOPERS);
			}
		}
	}

	//get the world key
	string GetWorldKey() {
		//default is 1
		string worldKey = GameConstants.MISSION_1_KEY;
		switch(currentWorld) {
		case 1: worldKey = GameConstants.MISSION_1_KEY;
			break;
		case 2: worldKey = GameConstants.MISSION_2_KEY;
			break;
		case 3: worldKey = GameConstants.MISSION_3_KEY;
			break;
		case 4: worldKey = GameConstants.MISSION_4_KEY;
			break;
		}
		return worldKey;
	}

	string GetLevelKey() {
		//default is 1
		string levelKey = GameConstants.MISSION_SELECT_LEVEL_ONE_KEY;

		switch(currentLevel) {
		case 2: levelKey = GameConstants.MISSION_SELECT_LEVEL_TWO_KEY;
			break;
		case 3: levelKey = GameConstants.MISSION_SELECT_LEVEL_THREE_KEY;
			break;
		case 4: levelKey = GameConstants.MISSION_SELECT_LEVEL_FOUR_KEY;
			break;
		case 5: levelKey = GameConstants.MISSION_SELECT_LEVEL_FIVE_KEY;
			break;
		case 6: levelKey = GameConstants.MISSION_SELECT_LEVEL_SIX_KEY;
			break;

		}
		return levelKey;
	}
	/**
	*performs some level calculations and report any achiviement reached
	*/
	void PerformFinalComputation(bool finishedGame) {
		 
		 /************************************************/
		 //for the score calculation (just for this level)


		 //these are the global values
	  	 int totalSaved = PlayerPrefs.GetInt(GameConstants.TOTAL_SAVED_JELLIES_KEY,0);
	  	 int totalSpentTime = PlayerPrefs.GetInt(GameConstants.TOTAL_ELAPSED_TIME_SECS_KEY,0);
	     int totalLostLifes = PlayerPrefs.GetInt(GameConstants.TOTAL_LOST_LIFES_KEY,0);
	     int levelLostLifes=0;
	  	 
		 //sum with the totals of previous levels
		 int playerLifesCount = player.GetRemainingLifes();
		 if(buyedExtraLifes) {    
		    // 3 + 2 - remaining (example : 3 + 2 - 2 = 3 lifes lost)
			levelLostLifes = GameConstants.NUM_LIFES_PER_LEVEL + GameConstants.IN_APP_PURCHASE_EXTRA_LIFES_COUNT - playerLifesCount; 
			totalLostLifes+= levelLostLifes;

			//to not adulterate scores
			if(playerLifesCount>GameConstants.NUM_LIFES_PER_LEVEL) {
				playerLifesCount = GameConstants.NUM_LIFES_PER_LEVEL;
			}

		 }
		 else if(buyedInfiniteLifes){
			  
		    // 3 + 980 - remaining (example : 3 + 1000 - 980 = 23 lifes lost)
			levelLostLifes = GameConstants.NUM_LIFES_PER_LEVEL + GameConstants.IN_APP_PURCHASE_INFINITE_LIFES_COUNT - playerLifesCount; 
			totalLostLifes+= levelLostLifes;

			//to not adulterate scores
			if(playerLifesCount>GameConstants.NUM_LIFES_PER_LEVEL) {
				playerLifesCount = GameConstants.NUM_LIFES_PER_LEVEL;
			}
		 
		 }
		 else {
			levelLostLifes=GameConstants.NUM_LIFES_PER_LEVEL - playerLifesCount;
			//if i lost 2 is 3-1 = 2
			totalLostLifes+= levelLostLifes;
		 }

		 totalSpentTime+= missionTimeInSeconds - totalRemainingMissionTimeInSeconds;
		 totalSaved+=numberOfSavedJellies;

		 //Save new totals
		 PlayerPrefs.SetInt(GameConstants.TOTAL_SAVED_JELLIES_KEY,totalSaved);
	  	 PlayerPrefs.SetInt(GameConstants.TOTAL_ELAPSED_TIME_SECS_KEY,totalSpentTime);
	     PlayerPrefs.SetInt(GameConstants.TOTAL_LOST_LIFES_KEY,totalLostLifes);
	  	 
	     int time = totalRemainingMissionTimeInSeconds;
	     int total = 0;

         /************* SOME MATH HERE ********************/
		 total+= (numberOfSavedJellies*10); //10 point for each saved Jelly
		 PlayerPrefs.SetInt(GameConstants.LEVEL_SAVED_JELLIES,numberOfSavedJellies);
		 PlayerPrefs.SetInt(GameConstants.LEVEL_SAVED_JELLIES_SCORE,numberOfSavedJellies*10);


		 if(levelLostLifes==0) {//saved all lifes
			total+= (GameConstants.NUM_LIFES_PER_LEVEL*10); //add 10 points for each life
			PlayerPrefs.SetInt(GameConstants.LEVEL_REMAINING_LIFES_SCORE,GameConstants.NUM_LIFES_PER_LEVEL*10);
		 }
		 else {
			total-= (levelLostLifes*10); //subtract 10 points for each lost life
			PlayerPrefs.SetInt(GameConstants.LEVEL_REMAINING_LIFES_SCORE,-(levelLostLifes*10));
		 }
		 PlayerPrefs.SetInt(GameConstants.LEVEL_REMAINING_LIFES,playerLifesCount);



		 float elapsedAmountOfTime = (float)totalRemainingMissionTimeInSeconds / missionTimeInSeconds;
		 if(elapsedAmountOfTime <=HURRY_UP_START_FACTOR) {
			total-=(time*5); //After hurry hup factor, subtract remaining time * 5!
			PlayerPrefs.SetInt(GameConstants.LEVEL_REMAINING_TIME_SECS_SCORE,-(time*5));
		 }
         else {
			total+=(time*5);//still before hurry up factor, add time * 5!
			PlayerPrefs.SetInt(GameConstants.LEVEL_REMAINING_TIME_SECS_SCORE,time*5);
		 }
		 PlayerPrefs.SetInt(GameConstants.LEVEL_REMAINING_TIME_SECS,totalRemainingMissionTimeInSeconds);
		 

         /*************************************************/
		 
		 //Check if we beat our previous highscore
		 //get current value and update with the total for the level
		 int updatedGameRunValue = PlayerPrefs.GetInt(GameConstants.TOTAL_SCORE_KEY,0);
		 updatedGameRunValue+=total;
		 PlayerPrefs.SetInt(GameConstants.TOTAL_SCORE_KEY,updatedGameRunValue);

		 //TODO we are currently not showing the Score anywhere
		 //get the high score, defaults to total of the level
		 if(updatedGameRunValue > highScore) {
			//we have a new highscore
		    highScore = updatedGameRunValue;
			PlayerPrefs.SetInt(GameConstants.HIGH_SCORE_KEY,highScore);
		 }
	     /**
	     * Report progress and scores to GAME CENTER!!!
	     */
		 bool auth = socialAPIInstance.isAuthenticated;
	     if(auth) {
			
			//only now report these 2
			if(finishedGame){
				//report less lost lifes (minus is better)
			    socialAPIInstance.ReportScore(totalLostLifes,GameConstants.LEADERBOARD_LESS_DEATHS);
			    //report spent time (minus is better)
			    socialAPIInstance.ReportScore(totalSpentTime,GameConstants.LEADERBOARD_BEST_TIME);
			}
			    

			//report main score to game center! (more is better)
			socialAPIInstance.ReportScore(highScore,GameConstants.LEADERBOARD_MAIN_SCORE);

			  
	     }
		 //check if any achievement checkpoint was reached

		int currentSavedSoldiers= PlayerPrefs.GetInt(GameConstants.TOTAL_SAVED_TROOPERS_KEY,0);
		CheckIfReachedAnyAchievementCheckpoint(currentSavedSoldiers,auth);
	        

	}

	/**
	* Check the achievements checkpoints
	*/
	void CheckIfReachedAnyAchievementCheckpoint(int totalSaved,bool authenticated) {

	 if(totalSaved >= GameConstants.ACHIEVEMENT_NEWBIE_CHECKPOINT) {
	    //write the achievement
		PlayerPrefs.SetInt(GameConstants.ACHIEVEMENT_NEWBIE_KEY,1);
		if(authenticated)
			socialAPIInstance.AddAchievement(GameConstants.ACHIEVEMENT_NEWBIE_KEY,100f);
		
	  }
	  if(totalSaved >= GameConstants.ACHIEVEMENT_ROOKIE_CHECKPOINT) {
	    //write the achievement
		PlayerPrefs.SetInt(GameConstants.ACHIEVEMENT_ROOKIE_KEY,1);
		if(authenticated)
			socialAPIInstance.AddAchievement(GameConstants.ACHIEVEMENT_ROOKIE_KEY,100f);
		
	  }

	//saved more than 100 already?
	  if(totalSaved >= GameConstants.ACHIEVEMENT_BRAVE_CHECKPOINT) {
	    //write the achievement
		PlayerPrefs.SetInt(GameConstants.ACHIEVEMENT_BRAVE_KEY,1);
		if(authenticated)
			socialAPIInstance.AddAchievement(GameConstants.ACHIEVEMENT_BRAVE_KEY,100f);
		
	  }
	  //saved more than 150 already?
	  if(totalSaved >= GameConstants.ACHIEVEMENT_HERO_CHECKPOINT) {
	    //write the achievement
		PlayerPrefs.SetInt(GameConstants.ACHIEVEMENT_HERO_KEY,1);
		if(authenticated)
			socialAPIInstance.AddAchievement(GameConstants.ACHIEVEMENT_HERO_KEY,100f);
	  }
				//saved more than 100 already?
	  if(totalSaved >= GameConstants.ACHIEVEMENT_LEGEND_CHECKPOINT) {
	    //write the achievement
		PlayerPrefs.SetInt(GameConstants.ACHIEVEMENT_LEGEND_KEY,1);
		if(authenticated)
			socialAPIInstance.AddAchievement(GameConstants.ACHIEVEMENT_LEGEND_KEY,100f);
	  }


	}

	/**
	* Are we on the last level??
	*/
	public bool IsFinalLevel() {
	   return currentWorld==numWorlds && currentLevel==numberOfLevels;
	}
	
	private void ReleaseParachute() {
		GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
		if(jelly!=null) {
			JellyScript script = jelly.GetComponent<JellyScript>();
			//its here the issue: && script.isFalling
			if(script!=null && script.isFalling && !script.isReleased &&!script.isLanded) {
				script.Release();
			}
				
		}
	}
	
	private bool DetectJellyTouches() {

	  if(isGamePaused) {
	    return false;
	  }

		for (int i = 0; i < Input.touchCount; ++i) {
			if (Input.GetTouch(i).phase == TouchPhase.Began) {
				Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
				RaycastHit2D hitInfo = Physics2D.Raycast(touchPosition, Vector2.zero);
				// RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
				if(hitInfo)
				{
					if(hitInfo.transform.gameObject.CompareTag("Jelly") || hitInfo.transform.gameObject.CompareTag("Parachute")) {
					  return true;
					}
					// Here you can check hitInfo to see which collider has been hit, and act appropriately.
				}
				else {
			    //OPtION move to the clik position
				//Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
				   GameObject jelly = GetJellyObject();
				   if(jelly!=null && isJellyFalling) {
											
				     MoveScript movement = jelly.GetComponent<MoveScript>();
				     if(movement!=null && movement.enabled) {
						 Vector3 jellyPos = jelly.transform.position;
						 if(touchPosition.x > jellyPos.x) {
						 //move right
						    movement.direction.x=1;
						 }
						 else if(touchPosition.x < jellyPos.x) {
						 //move left
							movement.direction.x=-1;
						 }
				     }
					 
				   }
				}
			}
		}
		return false;
	}
	
	//desktop click on Jelly
	private bool DetectDesktopJellyTouches(bool moveLeft, bool moveRight) {

	  if(isGamePaused) {
	    return false;
	  }

		if(Input.GetMouseButtonDown(0)){
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
			
			if(hitCollider){
				if(hitCollider.transform.gameObject.CompareTag("Jelly") || hitCollider.transform.gameObject.CompareTag("Parachute")) {
			      return true;
			    }
			
			}
	   }
	   else {
			   //OPtION move to the clik position
				//Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
			   GameObject jelly = GetJellyObject();
			   if(jelly!=null && isJellyFalling) {
										
			     MoveScript movement = jelly.GetComponent<MoveScript>();
			     if(movement!=null && movement.enabled) {

		
					if(moveRight) {
						movement.direction.x=1;
					}
					else if(moveLeft) {
						movement.direction.x=-1;
					}
					 /*Vector3 jellyPos = jelly.transform.position;

					 if(mousePosition.x > jellyPos.x) {
					 //move right
					    movement.direction.x=1;
					 }
					 else if(mousePosition.x < jellyPos.x) {
					 //move left
						movement.direction.x=-1;
					 }*/
			     }
				 
			   }

		}
		return false;
	}
	
	public GameObject[] GetJellyObjects() {
	
		GameObject[] jellies = GameObject.FindGameObjectsWithTag("Jelly");
		if(jellies.Length>0) {
		 return jellies;
		}
		
		return null;
	}

	public GameObject GetJellyObject() {
	
		GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
		return jelly;
	}
	
	/*
	public GameObject GetJellyContainer() {
		GameObject jellyContainer = GameObject.FindGameObjectWithTag("JellyContainer");
		return jellyContainer;
	}*/
	
	//check if the have the mothership on screen
	private bool IsMotherShipVisibleOnScreen() {
		return motherShip!=null && motherShip.GetComponent<Renderer>().IsVisibleFrom(Camera.main);
	}
	
	private IEnumerator Wait(long seconds)
		
	{		
		yield return new WaitForSeconds(seconds);

	}
	
	public bool IsGameOver() {
	  return isGameOver;
	}
	
	//check if we are on the last level
	//this is important because the mothership
	//will have different behaviours
	public bool IsLastLevel() {
	  return currentLevel == numberOfLevels;
	}
	
	void CheckPause() {
		Time.timeScale = isGamePaused ? 0f : 1.0f; 
	}
	
	public void PauseGame() {
	   ScreenShotScript screenshot = GetComponent<ScreenShotScript>();
	   if(screenshot!=null) {
		  screenshot.EnableScreenshots();
	   }
	   isGamePaused = true;
	   CheckPause();
	   PauseMusic();


		
	}

	public void ResumeGame() {

		ScreenShotScript screenshot = GetComponent<ScreenShotScript>();
		if(screenshot!=null) {
		  screenshot.DisableScreenshots();
		}
	    isGameOver = false;
		isGamePaused = false;
		isGameStarted = true;
		showUnlockLevel = false;
		CheckPause();
		ResumeMusic();
	}
	
	public void StartGame() {
	
		isGamePaused = false;
		isGameStarted = true;
		isGameOver = false;
		showUnlockLevel = false;

		ScreenShotScript screenshot = GetComponent<ScreenShotScript>();
		if(screenshot!=null) {
		  screenshot.DisableScreenshots();
		}

		CheckPause();
		StartMusic();

		if(currentLevel==1 && currentWorld==1) {

		   //This only shows up on level 1
		   GameObject brandLogo = GameObject.FindGameObjectWithTag("brand_logo");
		   if(brandLogo!=null) {
		    brandLogo.GetComponent<SpriteRenderer>().enabled = false;
		   }

		  //if we are on level 1, clear the history
			//ClearPlayerPrefs();
			//stop invoking the increase function
			if(currentWorld==1) {
				CancelInvoke("IncreaseTimeForHowToTexture");
			}
		}
		
		InvokeRepeating("CheckMissionTime", 1.0f, 1.0f);
			
	 }
	 
	
	

	//i shoul stop the scroll of the level
	public void EndGame(bool showUnlockNextLevel) {

		isGameStarted = false;
		isGameOver = true;
		isGamePaused = false;
		StopMusic();
		showUnlockLevel = showUnlockNextLevel;
		//EnableScreenshots();
		CheckPause();
				
		
	}

	
	void OnGUI() {


			// Set the skin to use
			GUI.skin = skin;
			//We can reduce the draw calls from OnGUI() function by 
			//enclosing all the contents inside a if loop like this one
			//draw level
			skin.label.normal.textColor = Color.white;
			
			Matrix4x4 svMat = GUI.matrix;//save current matrix

			Vector3 scaleVector = resolutionHelper.scaleVector;
			bool isWideScreen = resolutionHelper.isWidescreen;
			int width = resolutionHelper.screenWidth;
			int height = resolutionHelper.screenHeight;

			Matrix4x4 normalMatrix;
			Matrix4x4 wideMatrix;
			//we use the center matrix for the buttons
			wideMatrix = Matrix4x4.TRS(new Vector3( (resolutionHelper.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			normalMatrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);

			//assign normal matrix by default
			GUI.matrix = normalMatrix;
						


			
			
		    if(Event.current.type==EventType.Repaint && !isGameOver) {

			 DrawText(GetTranslationKey(GameConstants.MSG_WORLD) + " " + currentWorld 
						+  " / " + GetTranslationKey(GameConstants.MSG_LEVEL) 
						+ " " + currentLevel, messagesFontSizeSmaller +12, 15, 5,220,60);
	
			
			if (elapsedMissionMinutes>=1) {
			    if(elapsedMissionSeconds>=10) {
					DrawText(GetTranslationKey(GameConstants.MSG_TIME)+ " 0" + elapsedMissionMinutes +":" + elapsedMissionSeconds , messagesFontSizeSmaller +12, 300, 10,200,50);
				}
			    else {
					DrawText(GetTranslationKey(GameConstants.MSG_TIME)+ " 0" + elapsedMissionMinutes +":0" + elapsedMissionSeconds , messagesFontSizeSmaller +12, 300, 10,200,50);
				}
				
			}
			else {
			   if(elapsedMissionSeconds>=10) {
					DrawText(GetTranslationKey(GameConstants.MSG_TIME)+ " 0:" + elapsedMissionSeconds , messagesFontSizeSmaller +12,300, 10,200,50);
				}
			   else {
			   
			        //red color
					skin.label.normal.textColor = Color.red;
					DrawText(GetTranslationKey(GameConstants.MSG_TIME)+ " 0:0" + elapsedMissionSeconds , messagesFontSizeSmaller +12, 300, 10,200,50);
					//reset to white again
					skin.label.normal.textColor = Color.white;
				}
				
			}



			//-------------------------------------------
			//PLAY A BUZZ SOUND AND FLASH RED SCREEN!!!
				if(isShowingHurryUpMessage && appliedHurryUpFactor) {


					//we need to center this on screen		
					if(isWideScreen){
						GUI.matrix = wideMatrix;
					}
					else{
						GUI.matrix = normalMatrix;
					}
					//red color
					skin.label.normal.textColor = Color.red;
					DrawText(GetTranslationKey(GameConstants.MSG_HURRY_UP) , messagesFontSizeSmaller + 15, width/2-50, height/2-50,200,50);
					//reset to white again
					skin.label.normal.textColor = Color.white;
					SoundEffectsHelper.Instance.PlayRedAlertSound();
					FlashWhenInHurryUpMessage();

					//use the normal matrix again
					GUI.matrix = normalMatrix;

				}
		
         //Draw the final boss hits instead
         //TODO on last level show new instructions, like on first level
		 if(IsFinalLevel()) {

		 	HealthScript bossHealth = GetFinalBossHealth();
		 	if(bossHealth!=null && bossHealth.hitPoints>0) {

			 int initialHealth = bossHealth.GetInitialHealth();
			 DrawText("Boss: " + (initialHealth - bossHealth.hitPoints) + " / "  + initialHealth, messagesFontSizeSmaller +10,520, 10,200,50);
		    }
		 }
		 else {
			DrawText(GetTranslationKey(GameConstants.MSG_RESCUED) + " " + numberOfSavedJellies + " / "  + numberOfJelliesToRescue, messagesFontSizeSmaller +10,520, 10,200,50);
		 }
		  
			
		}
		    
			

					
			if(Event.current.type==EventType.Repaint) {

				if(isGameStarted) {
			
								//instantiate the first time we reference it
		
					if(clock!=null) {
						Rect clockRect = new Rect(248,8,48,48);
						GUI.DrawTexture(clockRect, clock);
					}
			  
				
					//we need this to put the play/pause at right
					if(isWideScreen){
						GUI.matrix = wideMatrix;
					}
					else{
						GUI.matrix = normalMatrix;
					}

					//pausePlayRect = new Rect(width-60 ,15,64,64);
					pausePlayRect = new Rect(width-70 ,15,64,64);


					if(isGamePaused) {
					//if not running
					   GUI.DrawTexture(pausePlayRect, playIcon);
					 
					}
					//game is not paused
				    //draw pause icon
				    else {
						
						GUI.DrawTexture(pausePlayRect, pauseIcon);
					}
	

				}
				else {

				
				  //Debug.Log("Not started yet");
				  //if null means it was destroyd, is game over
				  //besides i cannot start with a null player, and if not a restart
				  //neither if i'm rolling credits
			     if(player!=null && !showUnlockLevel && !isGameComplete) {
				  
					//make sure we draw this at the center of the screen
					if(isWideScreen){
						GUI.matrix = wideMatrix;
					}
					else{
						GUI.matrix = normalMatrix;
					}

					exitTextureRect = new Rect( width/2 - 100,screenHeight/2-60,200,80);
					GUI.DrawTexture(exitTextureRect, exitTexture);

					#if UNITY_ANDROID && !UNITY_EDITOR
					rateRect = new Rect( width/2 - 100,screenHeight/2+40,200,80);
					GUI.DrawTexture(rateRect, rateTexture);
					#endif
					//start playing //screenWidth

					//---------------------------------------------------------------------------------					
					#if !UNITY_EDITOR
					leaderboardsRect = new Rect(width/2-50,screenHeight / 3 * 2 + 10 ,96,96);
					GUI.DrawTexture(leaderboardsRect, leaderBoardTexture,ScaleMode.ScaleToFit);
					#endif
					//---------------------------------------------------------------------------------

					//SHOW HELP INFO ABUT GAMEPLAY, ONLY IF AT FIRST OR LAST LEVEL
					if( ( (currentWorld==1 && currentLevel==1) ||  IsFinalLevel() )  && helpMeTexture!=null ) {

					float aux = Time.realtimeSinceStartup;
					lastHowToTime+=  aux - initialHowToTime;

			
					if(lastHowToTime - initialHowToTime >= 1.0f) {

						isShowingHowTo = !isShowingHowTo;
						initialHowToTime = lastHowToTime;
					}

					//TODO check dictionary, if mobile tap, othewise click
						skin.label.normal.textColor = Color.grey;

						if(isShowingHowTo) {
							Rect helpMeTextureRect = new Rect(70,screenHeight/2-320,70,70);
						    GUI.DrawTexture(helpMeTextureRect, helpMeTexture);
						}

						if(IsFinalLevel()) {
						 //TODO
							DrawText(GetTranslationKey(GameConstants.MSG_HOW_TO_PLAY) , messagesFontSizeSmaller+2, 80, screenHeight/2-240,500,40);
							DrawText(GetTranslationKey(GameConstants.MSG_HOW_TO_PLAY_LAST_LEVEL) , messagesFontSizeSmaller, 80, screenHeight/2-200,500,40);
						}
						else {
							DrawText(GetTranslationKey(GameConstants.MSG_HOW_TO_PLAY) , messagesFontSizeSmaller+2, 80, screenHeight/2-260,600,40);

							if( !(Application.platform == RuntimePlatform.OSXPlayer) ) {
								DrawText(GetTranslationKey(GameConstants.MSG_TAP_TROOPER) , messagesFontSizeSmaller+2, 80, screenHeight/2-225,600,40);
								DrawText(GetTranslationKey(GameConstants.MSG_TAP_LEFT_RIGHT) , messagesFontSizeSmaller+2, 80, screenHeight/2-190,600,40);
							}
							else {
								DrawText(GetTranslationKey(GameConstants.MSG_CLICK_TROOPER) , messagesFontSizeSmaller+2, 80, screenHeight/2-225,600,40);
								DrawText(GetTranslationKey(GameConstants.MSG_CLICK_LEFT_RIGHT) , messagesFontSizeSmaller+2, 80, screenHeight/2-190,600,40);
							}

							DrawText(GetTranslationKey(GameConstants.MSG_LAND_ALL) , messagesFontSizeSmaller+2, 80, screenHeight/2-155,600,40);
							DrawText(GetTranslationKey(GameConstants.MSG_USE_FAILSAFE) , messagesFontSizeSmaller+2, 80, screenHeight/2-120,600,40);

						}		
						
						//restore white
						skin.label.normal.textColor = Color.white;
						
		
					}

					if(highScore > 0) {
						
						DrawText("High Score: " + highScore, messagesFontSizeSmaller +10,740, 10,220,40);

						smallTrooperRect = new Rect(740,50,48,48);
						GUI.DrawTexture(smallTrooperRect,smallTrooper);
						//DRAW SMALL TROOPER and x saved
						//todo draw the number of saved troopers
						int numTroopers = PlayerPrefs.GetInt(GameConstants.TOTAL_SAVED_TROOPERS_KEY,0);
						DrawText("X " + numTroopers, messagesFontSizeSmaller +10,790, 50,220,40);
					}
								
					
				 }
				
			   }
				
						
			}//end repaint

				

		//---------------------------------------------------------
		//*************** CHEK TEXTURE CLICKS *********************
		//---------------------------------------------------------
		//before checking the clicks we put the correct matrix

		if(isWideScreen){
			GUI.matrix = wideMatrix;
		}
		else{
			GUI.matrix = normalMatrix;
		}
		//---------------------------------------------

		if(!isMobilePlatform) { //desktop

			if(Event.current.type == EventType.MouseUp ) {
				
				if(isGameOver) {
						#if UNITY_ANDROID && !UNITY_EDITOR
						if(rateRect.Contains(Event.current.mousePosition) && player!=null) {
						  Application.OpenURL("market://details?id=com.pcdreams.superjellytroopers");
					    }
					    #endif

						if(exitTextureRect.Contains(Event.current.mousePosition) ) {
							StartGame();
							StartJellyFall();
						}
						else if(leaderboardsRect!=null && player!=null && leaderboardsRect.Contains(Event.current.mousePosition) ) {
						  if(socialAPIInstance.isAuthenticated) {
							socialAPIInstance.ShowLeaderBoards();
						  }
						  else {
							StartCoroutine(ShowMessage(GetTranslationKey(GameConstants.MSG_GAME_CENTER_ERROR), 1.5f));
						  }
							
						}

			    }
				else {

				//Did i paused the game???
				  if(pausePlayRect.Contains(Event.current.mousePosition)) {
					isGamePaused = !isGamePaused;
					if(isGamePaused) {
						PauseGame();
					}
					else {
						ResumeGame();
					}
			      } 

			    }
			      
			}

		  }
		  //if mobile platform
		  else {
		   //----------------------------------------------------------------
		      //detect touches on leaderboards
		      //for this we need the normal matrix

		    bool touchedLeaderBoard = false;
		    if (Input.touches.Length ==1) {
			    
				Touch touch = Input.touches[0];
			    
				if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)  {

					Vector2 fingerPos = GetFingerPosition(touch,isWideScreen);

					if(isGameOver) {

			
						#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_ANDROID && !UNITY_EDITOR
			
					 if(leaderboardsRect.Contains(fingerPos) && player!=null) {
						  touchedLeaderBoard = true;

						  if(socialAPIInstance.isAuthenticated) {
							 socialAPIInstance.ShowLeaderBoards();
						  }
						  else {
							StartCoroutine(ShowMessage(GetTranslationKey(GameConstants.MSG_GAME_CENTER_ERROR), 1.5f));
						  }
					 }
				
					#endif

						//is game over, maybe not started yet?
						if(exitTextureRect.Contains(fingerPos) ) {
							StartGame();
							StartJellyFall();
						}

					    
					    #if UNITY_ANDROID && !UNITY_EDITOR
					    if(rateRect.Contains(fingerPos) && player!=null) {
							Application.OpenURL("market://details?id=com.pcdreams.superjellytroopers");
					    }
					    #endif
						
				   }
				   else if(pausePlayRect.Contains(fingerPos) ) {	
						//already started
						//Did i paused the game???			

						isGamePaused = !isGamePaused;
						if(isGamePaused) {
							PauseGame();
						}
						else {
							ResumeGame();
						}

				  }

				 }
					
				}  
		    }
	
	  //******************** MOBILE TOUCHES ARE HANDLED ON UPDATE() ??? *************
	  //restore the matrix
	  GUI.matrix = svMat;
	
}	
	/**
	*Get the correct finger touch position
	*/
	Vector2 GetFingerPosition(Touch touch, bool isWideScreen) {

	  
		Vector2 fingerPos = new Vector2(0,0);
		float diference = 0f;
					
		fingerPos.y =  screenHeight - (touch.position.y / Screen.height) * screenHeight;
		fingerPos.x = (touch.position.x / Screen.width) * screenWidth;

	    return fingerPos;
	}

	IEnumerator ShowMessage (string message, float delay) {
      GameObject textObj = GameObject.FindGameObjectWithTag("JellyTxt");
      if(textObj!=null) {
		GUIText guiText = textObj.GetComponent<GUIText>();
		if(guiText!=null) {
			guiText.text = message;
     		guiText.enabled = true;
     		yield return new WaitForSeconds(delay);
     		guiText.enabled = false;
		}

      }

 	}
	//get final boss health, we show the hits on final level
	//instead of counting how many solders we rescued (player still can loose for time running out)
	HealthScript GetFinalBossHealth() {
		GameObject boss = GameObject.FindGameObjectWithTag("FinalBoss");
		if(boss!=null) {
		  return boss.GetComponent<HealthScript>(); //could be null
		}
		return null;
	}

	string GetTranslationKey(string key) {
		return	translationManager.GetText(key);
	}
	/**
	* release parachute and fall
	*/	
	public void StartJellyFall() {

		if(!isJellyFalling) {
			//already open, jump now
			GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
			if(jelly!=null) {
				JellyScript script = jelly.GetComponent<JellyScript>();
				if(script!=null) {
					script.Jump();
					//maybe enable collider here?;
				}
			}
		}
	} 
	
	//this is to reset the clicks
	public void PlatformClosed() {
	  openedPlatform = false;
	}
	
	public bool IsMobilePlatform() {
		return isMobilePlatform;
	}
	
	public bool IsIOSPlatform() {
		return platform == RuntimePlatform.IPhonePlayer; 
	}
	
	public bool IsAndroidPlatform() {
		return platform == RuntimePlatform.Android;
	}


	public bool IsMacOSXPlatform() {
	    return platform == RuntimePlatform.OSXPlayer;
	}
	
	public int GetNumberEnergyPickups() {
	   return numberEnergyPickups;
	}
	
	public void DrawLargerText(string text) {
	    DrawText(text,messagesFontSizeLarger);
	}
	
	public void DrawSmallerText(string text) {
		DrawText(text,messagesFontSizeSmaller);
	}
	
	public void DrawText(string text, int fontSize) {
	

		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = messagesFont;
		centeredStyleSmaller.fontSize = fontSize;
		GUI.Label (new Rect(screenWidth/2-200, screenHeight/2, 400, 50), text);
	}
	
	public void DrawText(string text, int fontSize, int x, int y,int width,int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = messagesFont;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label(new Rect(x, y, width, height), text);
	}

	public void DrawText(string text, int fontSize, float x, float y,int width,int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = messagesFont;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label(new Rect(x, y, width, height), text);
	}
	
	//release banner resources
	void OnDestroy() {
	
	
		
	}

	/*public void TakeScreenShot() {
		ScreenShotScript screenshot = GameObject.FindGameObjectWithTag("Scripts").GetComponent<ScreenShotScript>();
		if (screenshot != null) {
			screenshot.TakeScreenshotBeforeGameOver (this);
		} 

	}
	
	//callback for the screenshot script
	public void SetScreenshotTexture(Texture2D texture) {
	  screenshotTexture = texture;
	}*/
	
	//only spwan and shoot if player is in sight
	public bool IsPlayerVisible() {
		bool checkPlayerVisible = (player==null) ? false : player.GetComponent<Renderer>().IsVisibleFrom(Camera.main);
		return checkPlayerVisible;
	}

	
}
