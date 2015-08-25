using UnityEngine;
using UnityEngine.SocialPlatforms;
using RescueJelly;
using System.Collections;
using ChartboostSDK;


/// <summary>
/// Start or quit the game
/// </summary>
public class GameOverScript : MonoBehaviour
{

	//custom GUI skin
	private GUISkin skin;
	public Font freeTextFont;
	public int freeTextFontSize = 40;
	GUIStyle style;
	float initialTime = 0f;
	float interval = 2f;
	bool isShowingMessage = false;
	const int buttonWidth = 170;
	const int buttonHeight = 60;

	Texture2D resumeTexture ;
	Texture2D startTexture ;
	Texture2D exitTexture ;
	Texture2D achievementsTexture;
	Texture2D creditsTexture ;
	Texture2D missionsTexture ;
	Texture2D moreLifesTexture ;

	Texture2D storeTexture;
	Rect storeTextureRect;

	Rect startTextureRect ;
	Rect exitTextureRect ;
	Rect achievementsRect;
	Rect creditsTextureRect ;
	Rect missionsTextureRect ;
	Rect resumeTextureRect ;
	Rect moreLifesTextureRect ;

	//GameControllerScript controller;
	int currentLevel = 1;
	int currentWorld = 1;

	//showGameName used on own SettingsScene
	public bool settingsScene = false;
	private bool isMobilePlatform = false;
	private static RuntimePlatform platform;

	private TextLocalizationManager translationManager;

	private bool showStore = false;
	private bool showAds = true;

	private bool drawMoreLifesButton = true;

	void Start() {
	
		// Load a skin for the buttons
		skin = Resources.Load("GUISkin") as GUISkin;
		//exitTexture = Resources.Load("menu") as Texture2D;
		startTexture = Resources.Load("button_playstart") as Texture2D;
		resumeTexture = Resources.Load("button_playresume") as Texture2D;
		exitTexture = Resources.Load("button_quit") as Texture2D;
		achievementsTexture = Resources.Load("button_achievements") as Texture2D;
		creditsTexture = Resources.Load("button_credits") as Texture2D;
		missionsTexture = Resources.Load("button_missions") as Texture2D;
		storeTexture = Resources.Load("store") as Texture2D;
		moreLifesTexture = Resources.Load("need_more_lifes") as Texture2D;
		
		initialTime = 0f;
		isShowingMessage = true;

				//handle translation language
		translationManager = TextLocalizationManager.Instance;
		translationManager.LoadSystemLanguage(Application.systemLanguage);

		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);

		//means is really game over
		if(!settingsScene) {
			Invoke("PauseGame", 4f);

			if(showAds && isMobilePlatform) {
			   Invoke("ShowInterstitial",0.6f);
			}

		}

		
	}
	void ShowInterstitial() {
		Chartboost.showInterstitial(CBLocation.HomeScreen);
	}

	void Awake() {

	  
	  GUIResolutionHelper.Instance.CheckScreenResolution();

	  if(!settingsScene) {
	    //means is really game over

		//get the previous saved values (this is what i was playing before showing game over)
		//are set on Awake() method of GameControllerScript

			if (PlayerPrefs.HasKey (GameConstants.PLAYING_LEVEL)) {
				//this is what i was playing before seeing this screen
				//if i came here after a game over secreen, these are goe already
				//but if i came here after going to the store and then back to settings scene, they will still be present

				currentWorld = PlayerPrefs.GetInt (GameConstants.PLAYING_WORLD, 1);
				currentLevel = PlayerPrefs.GetInt (GameConstants.PLAYING_LEVEL, 1);
			} 
			else {
				currentWorld = PlayerPrefs.GetInt(GameConstants.CURRENT_WORLD_KEY,1);
				currentLevel = PlayerPrefs.GetInt(GameConstants.CURRENT_LEVEL_KEY,1);
			}
		

		//check if we show the store button or not
		CheckInAppPurchases();
		//if i just died for real, then i clear all the other keys, about time, lifes, etc...
		ClearPlayerPrefs();

		//if this is really a game over and i have these keys, 
		//then i should delete them to
		if(PlayerPrefs.HasKey(GameConstants.PLAYING_LEVEL)) {
			//this avoids that after a game over scene i go to settings scene, and then have this available again
			PlayerPrefs.DeleteKey(GameConstants.PLAYING_WORLD);
		    PlayerPrefs.DeleteKey(GameConstants.PLAYING_LEVEL);
		}


	  }
	  //IS SETTINGS SCENE get this from player preferences
	  else if(PlayerPrefs.HasKey(GameConstants.PLAYING_LEVEL)) {
	     //this is what i was playing before seeing this screen
	     //if i came here after a game over secreen, these are goe already
	     //but if i came here after going to the store and then back to settings scene, they will still be present
		 currentWorld = PlayerPrefs.GetInt(GameConstants.PLAYING_WORLD,1);
		 currentLevel = PlayerPrefs.GetInt(GameConstants.PLAYING_LEVEL,1);
	  }	
	  else {
		currentWorld = PlayerPrefs.GetInt(GameConstants.CURRENT_WORLD_KEY,1);
		currentLevel = PlayerPrefs.GetInt(GameConstants.CURRENT_LEVEL_KEY,1);
	  }

	}

	void ClearPlayerPrefs() {

		//put them all to zero
		PlayerPrefs.SetInt(GameConstants.TOTAL_ELAPSED_TIME_SECS_KEY,0);
		PlayerPrefs.SetInt(GameConstants.TOTAL_LOST_LIFES_KEY,0);
		PlayerPrefs.SetInt(GameConstants.TOTAL_SAVED_JELLIES_KEY,0);
		//this is the total key for this game run
		PlayerPrefs.SetInt(GameConstants.TOTAL_SCORE_KEY,0);
		//this is the permanent key, only updated when total score is greater

		/*PlayerPrefs.SetInt(GameConstants.MISSION_4_KEY,0);
		PlayerPrefs.SetInt(GameConstants.MISSION_3_KEY,0);
		PlayerPrefs.SetInt(GameConstants.MISSION_2_KEY,0);
		PlayerPrefs.SetInt(GameConstants.MISSION_1_KEY,0);*/

	   

	}
	
	void CheckInAppPurchases() {
	  bool buyedExtraLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
	  bool buyedExtraTime = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
	  bool buyedExtraSpeed = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
	  bool buyedInfiniteLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);

	  //any purchase will remove ads!
	  if(buyedExtraTime || buyedInfiniteLifes || buyedExtraSpeed || buyedExtraLifes) {
	    showAds = false;
	  }

	  //show store whenever something is still available to purchase
	  if(!buyedExtraTime || !buyedExtraSpeed || !buyedExtraLifes || !buyedInfiniteLifes) {
	    showStore = true;
	  }
	  else {
	    showStore = false;
	  }

	  if (buyedExtraLifes || buyedInfiniteLifes || settingsScene) {
			drawMoreLifesButton = false;
	  } 
	  else {
			InvokeRepeating("AlternateRenderLifesButton",0.5f,1.0f);
	  }
	 		
	  
	}

	
	void LoadStyle() {
		style = GUI.skin.GetStyle("Label");
		style.alignment = TextAnchor.MiddleLeft;
		style.font = freeTextFont;
		style.fontSize = freeTextFontSize;
		style.normal.textColor = Color.white;
	}

    void Update() {
    
		initialTime += Time.deltaTime;
		
		
		//blink game over message every 2 seconds
		if(initialTime >= interval ) {
			
			initialTime = 0f;
			isShowingMessage = !isShowingMessage;
			
		}

		if(!isMobilePlatform && Input.GetKeyDown(KeyCode.Escape)) {
		  Application.Quit();
		}
    
    }
    //Load next scene, showing an activity indicator
	/*void StartActivityMonitor()
    {
        #if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.Gray);
        #elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle);
        #endif

        Handheld.StartActivityIndicator();
    }*/

	void AlternateRenderLifesButton() {
		drawMoreLifesButton = !drawMoreLifesButton;
	}

	void OnGUI()
	{
		
				
	  
		if(style==null) {
			LoadStyle();
		}
		// Set the skin to use
		GUI.skin = skin;
		
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		
	    int width = GUIResolutionHelper.Instance.screenWidth;
		int height = GUIResolutionHelper.Instance.screenHeight;
		Vector3 scaleVector = GUIResolutionHelper.Instance.scaleVector;
		
		bool isWideScreen = GUIResolutionHelper.Instance.isWidescreen;
		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (GUIResolutionHelper.Instance.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			
			
		}
		else {
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
			
		}


		   // bool showNextLevel = false;

		   // if(!settingsScene) {

			//	showNextLevel = controller.IsShowUnlockNextLevel() && currentLevel < controller.GetNumberOfLevels();
			GameObject playerPlaying = GameObject.FindGameObjectWithTag ("Player");
			//means player is dead
			bool playerAlive = false;
			if(playerPlaying!=null) {
			 PlayerScript playerScript = playerPlaying.GetComponent<PlayerScript>();
			 playerAlive = playerScript!=null && playerScript.IsPlayerAlive();
			}



			
			
			if(Event.current.type==EventType.Repaint) {

			//only used on the settings scene
			  //if(settingsScene) {
			  //		GUI.Label (new Rect(width/2-140, height/2-300, 450, 50), "Super Jelly Troopers",style);
			  //}
			 if(!settingsScene) { 

					if(isShowingMessage /*&& !showNextLevel*/) {
						

						//It means we have finished the GAME, and we are official a SJT GAME GURU
						if(playerAlive && PlayerPrefs.HasKey(GameConstants.ACHIEVEMENT_GURU_KEY)) {
						   GUI.Label (new Rect(width/2-190, height/2-300, 500, 50), 
									GetTranslationKey(GameConstants.MSG_CONGRATULATIONS) + " SJT GURU!!",style);
									//automatically show credits after 5 seconds!!!!

							
							StartCoroutine(ShowCredits());
							
			
						}
						//if is game over and nothing else to show, print Game Over message only
						else {
						   
							 GUI.Label (new Rect(width/2-90, height/2-300, 200, 50), "Game Over!!!",style);
						   
							
						}


					}
		
			  }
				



					//*******************************

				    startTextureRect = new Rect(width / 2-100,height -600,200,80);
				    resumeTextureRect = new Rect(width / 2-100,height-500,200,80);
					missionsTextureRect = new Rect(width / 2-100,height -400,200,80);
					achievementsRect = new Rect(width / 2-100,height -300,200,80);
					creditsTextureRect = new Rect(width / 2-100,height-200,200,80);


					//}

					GUI.DrawTexture(startTextureRect,startTexture);
					GUI.DrawTexture(missionsTextureRect,missionsTexture);
					GUI.DrawTexture(achievementsRect,achievementsTexture);
					GUI.DrawTexture(creditsTextureRect,creditsTexture);
					GUI.DrawTexture(resumeTextureRect,resumeTexture);

					if(!settingsScene && drawMoreLifesButton) {
						moreLifesTextureRect = new Rect(width / 2-100,height-100,200,80);
						GUI.DrawTexture(moreLifesTextureRect,moreLifesTexture);
					}

					if(!settingsScene && showStore) {
						storeTextureRect = new Rect(width -110,30,96,96);
					    GUI.DrawTexture(storeTextureRect,storeTexture);
					}
					    

			}//end repaint
			
			
		//********************* CLICK / TOUCH CHECKS *******************
				//desktop checks
		if(Event.current.type == EventType.MouseUp && !isMobilePlatform) {

			Vector2 mousePosition = Event.current.mousePosition;

			    if(startTextureRect.Contains(mousePosition) )
				{
					LoadNextLevel(1,1);
				}
				else if(resumeTextureRect.Contains(mousePosition) )
				{

				  if(settingsScene && PlayerPrefs.HasKey(GameConstants.PLAYING_LEVEL)) {
					LoadNextLevel(currentWorld,currentLevel);
				  }
				  else {
				    LoadNextLevel(currentWorld,1);

				  }
					
				  
				}
				else if(storeTextureRect.Contains(mousePosition) )
				{
					Application.LoadLevel("StoreScene");
				}
				else if(drawMoreLifesButton && moreLifesTextureRect!=null && moreLifesTextureRect.Contains(mousePosition) )
				{
					Application.LoadLevel("StoreScene");
				}
				else if(missionsTextureRect.Contains(mousePosition) )
				{
					Application.LoadLevel("MissionsScene");

				}
				else if(achievementsRect.Contains(mousePosition) )
				{

					Application.LoadLevel("AchievementsScene");
				}
				else if(creditsTextureRect.Contains(mousePosition) )
				{
					Application.LoadLevel("CreditsScene");


				}
		}
		//mobile checks
		else if(isMobilePlatform && Input.touchCount == 1 )
		{

			Touch touch = Input.touches[0];
			if(touch.phase == TouchPhase.Began) {

				Vector2 fingerPos = new Vector2(0,0);
				fingerPos = touch.position;
				
				fingerPos.y =  height - (touch.position.y / Screen.height) * height;
				fingerPos.x = (touch.position.x / Screen.width) * width;


				if(GUIResolutionHelper.Instance.isWidescreen) {
				//do extra computation
					fingerPos.x = fingerPos.x + (GUIResolutionHelper.Instance.scaleX - GUIResolutionHelper.Instance.scaleVector.y) / 2 * width;
				}

				if(startTextureRect.Contains(fingerPos) )
				{
					LoadNextLevel(1,1);
				}
				else if(resumeTextureRect.Contains(fingerPos) )
				{
				   //if settings and previously was not game over, than i can resume normally (means i come from store scene)
				   if(settingsScene && PlayerPrefs.HasKey(GameConstants.PLAYING_LEVEL)) {
					 LoadNextLevel(currentWorld,currentLevel);
				   }
				   else {
					 LoadNextLevel(currentWorld,1);
				   }
				}
				else if(missionsTextureRect.Contains(fingerPos) )
				{
					//StartActivityMonitor();
					Application.LoadLevel("MissionsScene");
				}
				else if(achievementsRect.Contains(fingerPos) )
				{
					Application.LoadLevel("AchievementsScene");
				}
				else if(creditsTextureRect.Contains(fingerPos) )
				{
					//StartActivityMonitor();
					Application.LoadLevel("CreditsScene");
				}
				else if(storeTextureRect.Contains(fingerPos) )
				{
					//StartActivityMonitor();
					Application.LoadLevel("StoreScene");
				}
				else if(drawMoreLifesButton && moreLifesTextureRect!=null && moreLifesTextureRect.Contains(fingerPos) )
				{
					Application.LoadLevel("StoreScene");
				}
			}
		}
			
		//restore the matrix	
		GUI.matrix = svMat;	
				   
	  
		

	}


	IEnumerator ShowCredits() {
 
		yield return new WaitForSeconds(3f);
		Application.LoadLevel("CreditsScene");
	}


	string GetTranslationKey(string key) {
		return	translationManager.GetText(key);
	}
	
	//this needs to be called about 3 seconds after showing something
	private void PauseGame() {
		Time.timeScale = 0f; 
	}
	
	void LoadNextLevel(int world, int scene) {

		//StartActivityMonitor();
		Application.LoadLevel("World" + world + "Scene" + scene);
		Destroy(gameObject);
	}
	
	void LoadFinalScene(string sceneName) {
		//StartActivityMonitor();
		Application.LoadLevel("FinalScene");
		Destroy(gameObject);
	}
	
	//Draw text on screen
	public void DrawText(string text, int fontSize) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = freeTextFont;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label (new Rect(Screen.width/2-150, Screen.height/2 -50, 400, 50), text);
	}
	
	//Draw text on screen
	public void DrawText(string text, int fontSize, int x, int y, int width, int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = freeTextFont;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label (new Rect(x, y, width, height), text);
	}
	


	
	void OnDestroy() {
		//Handheld.StopActivityIndicator();
	}

	void OnEnable() {
		// Listen to all impression-related events
		Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += didDismissInterstitial;
		Chartboost.didCloseInterstitial += didCloseInterstitial;
		Chartboost.didClickInterstitial += didClickInterstitial;
		Chartboost.didCacheInterstitial += didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += didDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps += didFailToLoadMoreApps;
		Chartboost.didDismissMoreApps += didDismissMoreApps;
		Chartboost.didCloseMoreApps += didCloseMoreApps;
		Chartboost.didClickMoreApps += didClickMoreApps;
		Chartboost.didCacheMoreApps += didCacheMoreApps;
		Chartboost.shouldDisplayMoreApps += shouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps += didDisplayMoreApps;
		Chartboost.didFailToRecordClick += didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo += didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
		Chartboost.didCacheInPlay += didCacheInPlay;
		Chartboost.didFailToLoadInPlay += didFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
		Chartboost.willDisplayVideo += willDisplayVideo;
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow += didCompleteAppStoreSheetFlow;
		#endif
	}

	void OnDisable() {
		// Remove event handlers
		Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= didDismissInterstitial;
		Chartboost.didCloseInterstitial -= didCloseInterstitial;
		Chartboost.didClickInterstitial -= didClickInterstitial;
		Chartboost.didCacheInterstitial -= didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps -= didFailToLoadMoreApps;
		Chartboost.didDismissMoreApps -= didDismissMoreApps;
		Chartboost.didCloseMoreApps -= didCloseMoreApps;
		Chartboost.didClickMoreApps -= didClickMoreApps;
		Chartboost.didCacheMoreApps -= didCacheMoreApps;
		Chartboost.shouldDisplayMoreApps -= shouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps -= didDisplayMoreApps;
		Chartboost.didFailToRecordClick -= didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
		Chartboost.didCacheInPlay -= didCacheInPlay;
		Chartboost.didFailToLoadInPlay -= didFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
		Chartboost.willDisplayVideo -= willDisplayVideo;
		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
		#endif
	}

	void didFailToLoadInterstitial(CBLocation location, CBImpressionError error) {
		Debug.Log(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
	}
	
	void didDismissInterstitial(CBLocation location) {
		//Debug.Log("didDismissInterstitial: " + location);
	}
	
	void didCloseInterstitial(CBLocation location) {
		Debug.Log("didCloseInterstitial: " + location);
	}
	
	void didClickInterstitial(CBLocation location) {
		//Debug.Log("didClickInterstitial: " + location);
	}
	
	void didCacheInterstitial(CBLocation location) {
		//Debug.Log("didCacheInterstitial: " + location);
	}
	
	bool shouldDisplayInterstitial(CBLocation location) {
		Debug.Log("shouldDisplayInterstitial: " + location);
		return true;
	}
	
	void didDisplayInterstitial(CBLocation location){
		//Debug.Log("didDisplayInterstitial: " + location);
	}

	void didFailToLoadMoreApps(CBLocation location, CBImpressionError error) {
		//Debug.Log(string.Format("didFailToLoadMoreApps: {0} at location: {1}", error, location));
	}
	
	void didDismissMoreApps(CBLocation location) {
		//Debug.Log(string.Format("didDismissMoreApps at location: {0}", location));
	}
	
	void didCloseMoreApps(CBLocation location) {
		//Debug.Log(string.Format("didCloseMoreApps at location: {0}", location));
	}
	
	void didClickMoreApps(CBLocation location) {
		//Debug.Log(string.Format("didClickMoreApps at location: {0}", location));
	}
	
	void didCacheMoreApps(CBLocation location) {
		//Debug.Log(string.Format("didCacheMoreApps at location: {0}", location));
	}
	
	bool shouldDisplayMoreApps(CBLocation location) {
		//Debug.Log(string.Format("shouldDisplayMoreApps at location: {0}", location));
		return true;
	}

	void didDisplayMoreApps(CBLocation location){
		//Debug.Log("didDisplayMoreApps: " + location);
	}

	void didFailToRecordClick(CBLocation location, CBImpressionError error) {
		//Debug.Log(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
	}
	
	void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error) {
		//Debug.Log(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
	}
	
	void didDismissRewardedVideo(CBLocation location) {
		//Debug.Log("didDismissRewardedVideo: " + location);
	}
	
	void didCloseRewardedVideo(CBLocation location) {
		//Debug.Log("didCloseRewardedVideo: " + location);
	}
	
	void didClickRewardedVideo(CBLocation location) {
		//Debug.Log("didClickRewardedVideo: " + location);
	}
	
	void didCacheRewardedVideo(CBLocation location) {
		//Debug.Log("didCacheRewardedVideo: " + location);
	}
	
	bool shouldDisplayRewardedVideo(CBLocation location) {
		//Debug.Log("shouldDisplayRewardedVideo: " + location);
		return true;
	}
	
	void didCompleteRewardedVideo(CBLocation location, int reward) {
		//Debug.Log(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
	}

	void didDisplayRewardedVideo(CBLocation location){
		//Debug.Log("didDisplayRewardedVideo: " + location);
	}
	
	void didCacheInPlay(CBLocation location) {
		//Debug.Log("didCacheInPlay called: "+location);
	}

	void didFailToLoadInPlay(CBLocation location, CBImpressionError error) {
		//Debug.Log(string.Format("didFailToLoadInPlay: {0} at location: {1}", error, location));
	}

	void didPauseClickForConfirmation() {
		//Debug.Log("didPauseClickForConfirmation called");
	}

	void willDisplayVideo(CBLocation location) {
		//Debug.Log("willDisplayVideo: " + location);
	}
	#if UNITY_IPHONE
	void didCompleteAppStoreSheetFlow() {
		//Debug.Log("didCompleteAppStoreSheetFlow");
	}
	#endif
}
