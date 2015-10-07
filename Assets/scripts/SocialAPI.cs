using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using RescueJelly;
#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
#endif

/**
 * Implement Game Center
 * */
public class SocialAPI : MonoBehaviour {

	[DllImport("__Internal")]
	private static extern void _ReportAchievement( string achievementID, float progress );

	public bool isAuthenticated = false;
	public bool isAuthenticating = false;
	public bool gameCenterAvailable = false;

	private static SocialAPI instance;

	//this is on achievements script
	private IAchievement[] gameAchievements;
	private IScore[] gameScores;
	private GUISkin skin;
	
	//achievement ids

	
	public Texture2D [] failedMissions;
	public Texture2D [] successMissions;
	private bool loadingGame = false;
	
	private const string PREVIOUS_ACTION_ADD_ACHIEVEMENT = "ADD_ACHIEVEMENT";
	private const string PREVIOUS_ACTION_AUTHENTICATION = "AUTHENTICATION";
	private const string PREVIOUS_ACTION_LOAD_ACHIEVEMENTS = "LOAD_ACHIEVEMENTS";
	
	private string previousAction = null;
	
	public bool paintGUI = true;


	// Use this for initialization
	void Start () {

	     GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	     if(scripts!=null) {
	      GameControllerScript controller = scripts.GetComponent<GameControllerScript>();
	      if(controller!=null) {
			gameCenterAvailable = controller.IsIOSPlatform() || controller.IsAndroidPlatform() || controller.IsMacOSXPlatform();
	      }
	     }
		
		isAuthenticating = false;
		isAuthenticated = Social.localUser.authenticated;
		loadingGame = false;
		skin = Resources.Load("GUISkin") as GUISkin;
		

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
			Social.LoadAchievements (ProcessLoadAchievements);
		}
	}
	
	void Awake() {

		// Register the singleton
		if(instance!=null) {
			Debug.Log("There is another instance of social api running");
			//DestroyImmediate(gameObject);
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	//dummy method
	public void LoadMe() {
	  Debug.Log("Social API Loaded");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static SocialAPI Instance {
		
		get
		{
			if (instance == null)
			{
				GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
				if(scripts!=null) {
				 instance = scripts.GetComponent<SocialAPI>();
				}
				if(instance==null) {
					instance = (SocialAPI)FindObjectOfType(typeof(SocialAPI));
				    if (instance == null) {
				    	//final chance, just create a new object
						instance = new GameObject("SocialAPI").AddComponent<SocialAPI>();
				    }
				      
				}

				
			}
			return instance;
		}
	}
	
	// This function gets called when Authenticate completes
	// Note that if the operation is successful, Social.localUser will contain data from the server. 
	public void ProcessAuthentication (bool success) {

		if (success) {

			isAuthenticated = true;
			// Request loaded achievements, and register a callback for processing them
			Social.LoadAchievements (ProcessLoadAchievements);
			//Social.LoadAchievementDescriptions(ProcessLoadAchievementsDescriptions);
			//LoadScores();
		}
		else {
			Debug.Log ("Failed to authenticate");
			isAuthenticated = false;
		}
			
	}
	
	public IAchievement[] GetGameAchievements() {
		if(gameAchievements==null && isAuthenticated) {
			Social.LoadAchievements (ProcessLoadAchievements);
	    }
	  
	    return gameAchievements;
	}
	
	public IScore[] GetGameScores() {
		if(gameScores==null && isAuthenticated) {
			LoadScores(GameConstants.LEADERBOARD_MAIN_SCORE);
		}
		
		return gameScores;
	}
		

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
						}
						Debug.Log (myAchievements);
			
		} 
		else {
			Debug.Log("No achievements found!!");
		}

	}
	
	private void SetPreviousActionAddAchievement() {
	  previousAction = PREVIOUS_ACTION_ADD_ACHIEVEMENT;
	}
	
	/**
	*
	*/
	public void AddAchievement(string id, float percentageCompleted) {

		#if UNITY_ANDROID && !UNITY_EDITOR
		  id = GameConstants.ANDROID_DICTIONARY[id];
		#endif
		_ReportAchievement(id,percentageCompleted);
	}
	
	public void CreateAchievementResult(bool success) {
		if (success)
			Debug.Log ("Successfully reported progress");
		else
			Debug.Log ("Failed to report progress");
	}

	
	public void LoadScores(string leaderBoardID) {
		Social.LoadScores(leaderBoardID,LoadScoresCallback);
	}
	
	public void LoadScoresCallback(IScore[] scores) {
	
		if (scores.Length > 0) {
			Debug.Log ("Got " + scores.Length + " scores");
			string myScores = "Leaderboard:\n";
			foreach (IScore score in scores)
				myScores += "\t" + score.userID + " " + score.formattedValue + " " + score.date + "\n";
			Debug.Log (myScores);
			gameScores = scores;
		}
		else
			Debug.Log ("No scores loaded!");
	
	}
	
	GUIStyle BuildLabelStyle() {
		
		GUIStyle centeredStyleLarger = GUI.skin.GetStyle("Label");
		centeredStyleLarger.alignment = TextAnchor.MiddleCenter;
		//centeredStyleLarger.font = scrollFont;
		centeredStyleLarger.fontSize = 25;
		centeredStyleLarger.normal.textColor = Color.white;
		return centeredStyleLarger;
	}
	
	void OnGUI() {
	

	}

	
	//search for a given achievement;
	private bool foundAchievement(string ach) {
	
		bool found = false;
		foreach(IAchievement achievement in gameAchievements) {
		
			if(achievement.id.Equals(ach)) {
				return true;
			}
			
		}
		return found;
	}

	public void ReportScore (long score, string leaderboardID) {
					
	   #if UNITY_ANDROID && !UNITY_EDITOR
		leaderboardID = GameConstants.ANDROID_DICTIONARY[leaderboardID];
	   #endif
		Debug.Log ("Reporting score " + score + " on leaderboard " + leaderboardID);
		Social.ReportScore (score, leaderboardID, ReportScoreCallback);
	}
	
	void ReportScoreCallback(bool success) {
		Debug.Log(success ? "Reported score successfully" : "Failed to report score");
	}

	public void ShowLeaderBoards() {

	 #if UNITY_ANDROID && !UNITY_EDITOR
		string toShow = GameConstants.ANDROID_DICTIONARY[GameConstants.LEADERBOARD_MAIN_SCORE];
		((PlayGamesPlatform) Social.Active).ShowLeaderboardUI(toShow);
	 #endif

	 #if UNITY_IPHONE && !UNITY_EDITOR
	 Social.ShowLeaderboardUI();
	 #endif
	
	}

}
