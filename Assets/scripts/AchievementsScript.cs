using UnityEngine;
using System.Collections;
using RescueJelly;
using UnityEngine.SocialPlatforms;

public class AchievementsScript : MonoBehaviour {

	private Texture2D exitTexture;
	private Rect exitTextureRect;
	GUISkin skin;

	private static RuntimePlatform platform= Application.platform;
	bool isMobilePlatform = false;
	// Use this for initialization

	bool guruEnabled = false;
	bool legendEnabled = false;
	bool braveEnabled = false;
	bool heroEnabled = false;
	bool newbieEnabled = false;
	bool rookieEnabled = false;


	private SpriteRenderer guru;
	private SpriteRenderer legend;
	private SpriteRenderer brave;
	private SpriteRenderer hero;
	private SpriteRenderer newbie;
	private SpriteRenderer rookie;

	//IOS game center
	private bool gameCenterAvailable = false;
	private bool isAuthenticating = false;
	private bool isAuthenticated = false;

	private IAchievement[] gameAchievements;

	GUIResolutionHelper resolutionHelper;
	void Start () {
		skin = Resources.Load("GUISkin") as GUISkin;
		exitTexture = Resources.Load("menu") as Texture2D;
		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android);

		//we will use the local saved values first
		guruEnabled = PlayerPrefs.GetInt(GameConstants.ACHIEVEMENT_GURU_KEY,0) > 0;
		legendEnabled = PlayerPrefs.GetInt(GameConstants.ACHIEVEMENT_LEGEND_KEY,0) > 0;
		heroEnabled = PlayerPrefs.GetInt(GameConstants.ACHIEVEMENT_HERO_KEY,0) > 0;
		braveEnabled = PlayerPrefs.GetInt(GameConstants.ACHIEVEMENT_BRAVE_KEY,0) > 0;

		newbieEnabled = PlayerPrefs.GetInt(GameConstants.ACHIEVEMENT_NEWBIE_KEY,0) > 0;
		rookieEnabled = PlayerPrefs.GetInt(GameConstants.ACHIEVEMENT_ROOKIE_KEY,0) > 0;

		//get the references to the sprite renderers
		hero = GameObject.FindGameObjectWithTag("Hero").GetComponent<SpriteRenderer>();
		legend = GameObject.FindGameObjectWithTag("Legend").GetComponent<SpriteRenderer>();
		brave = GameObject.FindGameObjectWithTag("Brave").GetComponent<SpriteRenderer>();
		guru = GameObject.FindGameObjectWithTag("Guru").GetComponent<SpriteRenderer>();

		newbie = GameObject.FindGameObjectWithTag("Newbie").GetComponent<SpriteRenderer>();
		rookie = GameObject.FindGameObjectWithTag("Rookie").GetComponent<SpriteRenderer>();

		//TODO for debug purposes only, the editor part
		gameCenterAvailable = (platform == RuntimePlatform.IPhonePlayer || platform==RuntimePlatform.OSXEditor);
		isAuthenticating = false;
		isAuthenticated = Social.localUser.authenticated;

		Debug.Log("Inside achievements script...game center available: " + gameCenterAvailable + " platform is: " + platform.ToString());

		// Authenticate and register a ProcessAuthentication callback
		// This call needs to be made before we can proceed to other calls in the Social API
		if(!gameCenterAvailable) {
			
		  Debug.Log("Game center not available for this platform!");
		}
		else if(!isAuthenticated) {
			Debug.Log("calling game center authentication..");
			Social.localUser.Authenticate(ProcessAuthentication);
		}
		else {
		    //already authenticated
			Debug.Log("Already authenticated... Loading achievements!");
			Social.LoadAchievements (ProcessLoadAchievements);
		}
	}

	void Awake() {
	    GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	    resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
		resolutionHelper.CheckScreenResolution();
	}

		// This function gets called when Authenticate completes
	// Note that if the operation is successful, Social.localUser will contain data from the server. 
	public void ProcessAuthentication (bool success) {

		if (success) {

			Debug.Log("Game center authentication OK...");
			isAuthenticated = true;
			// Request loaded achievements, and register a callback for processing them
			Social.LoadAchievements (ProcessLoadAchievements);
			//Social.LoadAchievementDescriptions(ProcessLoadAchievementsDescriptions);
			//LoadScores();
		}
		else {
			Debug.Log ("Failed to authenticate on Game center");
			isAuthenticated = false;
		}
			
	}

	public void ReportAchievement(string id, float percentageCompleted) {
		IAchievement achievement = Social.CreateAchievement();
		achievement.id = id;
		achievement.percentCompleted = percentageCompleted;
		achievement.ReportProgress(CreateAchievementResult);
	}

	public void CreateAchievementResult(bool success) {
		if (success)
			Debug.Log ("Successfully reported achievement progress");
		else
			Debug.Log ("Failed to report achievement progress");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/**
	* Load the game achievements
	*/
	public void ProcessLoadAchievements(IAchievement[] achievements) {
	
		gameAchievements = achievements;
		Debug.Log("adding achievements...");
		
		if (achievements.Length > 0) {

				Debug.Log (" Got " + achievements.Length + " achievements");
				string myAchievements = "My achievements:\n";
				foreach (IAchievement achievement in achievements) {
						myAchievements += "\t" +
						achievement.id + " " +
						achievement.percentCompleted + " " +
						achievement.completed + " " +
						achievement.lastReportedDate;

						//when i load them from game center, i check if any of these
						//is already achievement locally, if true i report the progress
						if(achievement.id.Equals(GameConstants.ACHIEVEMENT_BRAVE_KEY) &&
						achievement.percentCompleted < 100f && braveEnabled) {
						  ReportAchievement(achievement.id,100f);
						}
						else if(achievement.id.Equals(GameConstants.ACHIEVEMENT_LEGEND_KEY) &&
						achievement.percentCompleted < 100f && legendEnabled) {
						  ReportAchievement(achievement.id,100f);
						}
						else if(achievement.id.Equals(GameConstants.ACHIEVEMENT_GURU_KEY) &&
						achievement.percentCompleted < 100f && guruEnabled) {
						  ReportAchievement(achievement.id,100f);
						}
						else if(achievement.id.Equals(GameConstants.ACHIEVEMENT_HERO_KEY) &&
						achievement.percentCompleted < 100f && heroEnabled) {
						  ReportAchievement(achievement.id,100f);
						}
						else if(achievement.id.Equals(GameConstants.ACHIEVEMENT_NEWBIE_KEY) &&
						achievement.percentCompleted < 100f && newbieEnabled) {
						  ReportAchievement(achievement.id,100f);
						}
						else if(achievement.id.Equals(GameConstants.ACHIEVEMENT_ROOKIE_KEY) &&
						achievement.percentCompleted < 100f && rookieEnabled) {
						  ReportAchievement(achievement.id,100f);
						}
				}
				Debug.Log (myAchievements);
			
		} 
		else {
			Debug.Log("No achievements found!!");
		}

	}

	bool IsMobilePlatform(){
		return platform ==	RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android; 
	}

	void OnGUI() {
		//BuildLargerLabelStyle();
	   
		GUI.skin = skin;
		
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		
		bool isWideScreen = resolutionHelper.isWidescreen;
		Vector3 scaleVector = resolutionHelper.scaleVector;
		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (resolutionHelper.scaleX - scaleVector.y) / 2 * resolutionHelper.screenWidth, 0, 0), Quaternion.identity, scaleVector);

		}
		else {
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
			
		}
		
		//GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,resolutionHelper.scaleVector);
	   		
		int width = resolutionHelper.screenWidth; 
		int height = resolutionHelper.screenHeight;

		if(Event.current.type==EventType.Repaint) {
				//menu button
			//if(isWideScreen) {
			//	exitTextureRect = new Rect(Screen.width - 100 * (resolutionHelper.scaleX - scaleVector.y) / 2,20,96,96);
			//}
			//else {
			    exitTextureRect = new Rect(width-110,30,96,96);
			//}
			
		
			GUI.DrawTexture(exitTextureRect,exitTexture);

		}


		//mobile touches
		if(isMobilePlatform && Input.touchCount == 1 && exitTexture!=null)
		{
			Touch touch = Input.touches[0]; 
			Vector2 fingerPos = new Vector2(0,0);
			fingerPos = touch.position;
			
			fingerPos.y =  height - (touch.position.y / Screen.height) * height;
			fingerPos.x = (touch.position.x / Screen.width) * width;
			
			if(isWideScreen) {
				//do extra computation
				fingerPos.x = fingerPos.x + (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * width;
			}
			
			if(touch.phase == TouchPhase.Began)
				
			{
				// do amazing things
				if(exitTextureRect.Contains(fingerPos)) {
				  //Application.LoadLevel("StoreScene");
				  Application.LoadLevel("SettingsScene");
				}
				
			}
		}
		//desktop platform
		else if(!isMobilePlatform && Event.current.type == EventType.MouseUp ) {
				
			if(exitTextureRect.Contains(Event.current.mousePosition) ) {
				//show the game over when clicked the exit
				Application.LoadLevel("SettingsScene");
			}
		}

		if(guruEnabled) {
		  guru.sprite = guru.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
		  guru.sprite = guru.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(legendEnabled) {
		  legend.sprite = legend.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
		  legend.sprite = legend.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(braveEnabled) {
		  brave.sprite = brave.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
		  brave.sprite = brave.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(heroEnabled) {
		  hero.sprite = hero.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
		  hero.sprite = hero.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(newbieEnabled) {
		  newbie.sprite = newbie.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
		  newbie.sprite = newbie.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(rookieEnabled) {
		  rookie.sprite = rookie.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
		  rookie.sprite = rookie.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}
			
		GUI.matrix = svMat;
	}
}
