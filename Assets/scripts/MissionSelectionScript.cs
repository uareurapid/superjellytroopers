using UnityEngine;
using System.Collections;
using RescueJelly;

public class MissionSelectionScript : MonoBehaviour {


  //public Texture2D panel;
  private Texture2D exitTexture;
  private Rect exitTextureRect;

  public Texture2D oneUnlock;
  public Texture2D twoUnlock;
  public Texture2D threeUnlock;
  public Texture2D fourUnlock;
  public Texture2D fiveUnlock;
  public Texture2D sixUnlock;

  public Texture2D oneLock;
  public Texture2D twoLock;
  public Texture2D threeLock;
  public Texture2D fourLock;
  public Texture2D fiveLock;
  public Texture2D sixLock;

  public Texture2D bonusTextureUnlocked;
  public Texture2D bonusTexture;
  public Texture2D acceptBonusTexture;
  public Texture2D denyBonusTexture;
  //public string rewardText = "Watch a video to unlock \r\nthis level!";

  private bool oneUnlocked = false;
  private bool twoUnlocked = false;
  private bool threeUnlocked = false;
  private bool fourUnlocked = false;
  private bool fiveUnlocked = false;
  private bool sixUnlocked = false;

  private Rect oneLockRect;
  private Rect twoLockRect;
  private Rect threeLockRect;
  private Rect fourLockRect;
  private Rect fiveLockRect;
  private Rect sixLockRect;
  //private Rect panelRect;

  private Rect acceptBonusRect;
  private Rect denyBonusRect;

  private bool isMobilePlatform = true;
  private GUISkin skin;
  private GUIResolutionHelper resolutionHelper;
  private TextLocalizationManager translationManager;

  private static RuntimePlatform platform = Application.platform;

  private bool showRewards = false;

  private string levelID = "";

  private bool hasAcceptedReward = false;

  public UnityEngine.UI.Text noticeText;

  private bool canShowAds = false;

  private bool isShowingAds = false;

  private bool interactionEnabled = false;

  int world = 1;
  string worldKey = "";


	// Use this for initialization
	void Start () {
	 interactionEnabled = false;
	 isShowingAds = false;
				// Load a skin for the buttons
	 skin = Resources.Load("GUISkin") as GUISkin;
	 isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);
	 exitTexture = Resources.Load("menu") as Texture2D;

	 Invoke("EnableInteraction",1.2f);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void EnableInteraction() {
	 interactionEnabled = true;
	}

	public void CheckPreferences() {
	  //this gives the world to check
	   world = PlayerPrefs.GetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,1);
	   worldKey = GameConstants.MISSION_1_KEY;

	   switch(world) {
		 case 1: worldKey = GameConstants.MISSION_1_KEY;
		   oneUnlocked = true;
		 break;
		 case 2: worldKey = GameConstants.MISSION_2_KEY;
			oneUnlocked = PlayerPrefs.GetInt(GameConstants.MISSION_2_KEY,0)>0 || PlayerPrefs.GetInt(GameConstants.MISSION_1_KEY,0)>0;
		 break;
		 case 3: worldKey = GameConstants.MISSION_3_KEY;
			oneUnlocked = PlayerPrefs.GetInt(GameConstants.MISSION_3_KEY,0)>0 || PlayerPrefs.GetInt(GameConstants.MISSION_2_KEY,0)>0;
		 break;
		 case 4: worldKey = GameConstants.MISSION_4_KEY;
			oneUnlocked = PlayerPrefs.GetInt(GameConstants.MISSION_4_KEY,0)>0 || PlayerPrefs.GetInt(GameConstants.MISSION_3_KEY,0)>0;
		 break;
	   }
	   //do not unlock mission 4 with ads
	   canShowAds = world < 4;

	   noticeText.text = canShowAds ? "Unlock blocked missions by watching a small video!" : "The last mission is only for braves, no cheat allowed!";

	   twoUnlocked = PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_TWO_KEY);
	   threeUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_THREE_KEY);
	   fourUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_FOUR_KEY);
	   fiveUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_FIVE_KEY);
	   sixUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_SIX_KEY);

	}

	void Awake() {
		
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
		
		resolutionHelper.CheckScreenResolution();
		//translations
		translationManager.LoadSystemLanguage(Application.systemLanguage);
	    CheckPreferences();

	}

	void OnGUI() {
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
		int width = resolutionHelper.screenWidth; 
		int height = resolutionHelper.screenHeight;

		if(Event.current.type==EventType.Repaint) {
			
			oneLockRect = new Rect(width / 2-300,height -500,128,128);
			twoLockRect = new Rect(width / 2-80,height-500,128,128);
			threeLockRect = new Rect(width / 2+150,height -500,128,128);
			fourLockRect = new Rect(width / 2-300,height -300,128,128);
			fiveLockRect = new Rect(width / 2-80,height-300,128,128);
			sixLockRect = new Rect(width / 2+150,height-300,128,128);
			
			GUI.DrawTexture(oneLockRect,oneUnlocked ? oneUnlock : oneLock);
			GUI.DrawTexture(twoLockRect,twoUnlocked ? twoUnlock : twoLock);
			GUI.DrawTexture(threeLockRect,threeUnlocked ? threeUnlock : threeLock);
			GUI.DrawTexture(fourLockRect,fourUnlocked ? fourUnlock : fourLock);
			GUI.DrawTexture(fiveLockRect,fiveUnlocked ? fiveUnlock : fiveLock);
			GUI.DrawTexture(sixLockRect,sixUnlocked ? sixUnlock : sixLock);
			
			exitTextureRect = new Rect(width-110,30,96,96);
			GUI.DrawTexture(exitTextureRect,exitTexture);

			if(showRewards) {
				if(!hasAcceptedReward) {
				    GUI.DrawTexture(new Rect(width / 2-238,height -700,476,421),bonusTexture);
				}
				else {
					GUI.DrawTexture(new Rect(width / 2-238,height -700,476,421),bonusTextureUnlocked);
				}

				denyBonusRect = new Rect(width / 2-200,height -370,171,72);
				GUI.DrawTexture(denyBonusRect,denyBonusTexture);

				acceptBonusRect = new Rect(width / 2  + 25,height -370,171,72);
				GUI.DrawTexture(acceptBonusRect,acceptBonusTexture);

			}
	
		}
		
		//********************* CLICK / TOUCH CHECKS *******************
		//GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);	
		//desktop checks
		if(interactionEnabled && (Event.current.type == EventType.MouseUp && !isMobilePlatform) ) {
			
			Vector2 mousePosition = Event.current.mousePosition;

			if(showRewards && !isShowingAds){
				if(acceptBonusRect.Contains(mousePosition) ) {
			       AcceptShowAds();
			    }
				else if(denyBonusRect.Contains(mousePosition) ) {
			       DenyShowAds();
			    }
			}
			else if(oneLockRect.Contains(mousePosition) )
			{
				if(oneUnlocked) {
					LoadNextLevel(1);
				}
				else {
				  //TODO show ad question
				  ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_ONE_KEY);
				}

			}
			else if(twoLockRect.Contains(mousePosition) )
			{
				if(twoUnlocked) {
					LoadNextLevel(2);
				}
				else {
				 //TODO show ad question
					ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_TWO_KEY);
				}

				
			}
			else if(threeLockRect.Contains(mousePosition) )
			{
				if(threeUnlocked) {
					LoadNextLevel(3);
				}
				else {
				 //TODO show ad question
					ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_THREE_KEY);
				}
			}
			else if(fourLockRect.Contains(mousePosition) )
			{
				if(fourUnlocked) {
					LoadNextLevel(4);
				}
				else {
				 //TODO show ad question
					ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_FOUR_KEY);
				}
				
			}
			else if(fiveLockRect.Contains(mousePosition) )
			{
				if(fiveUnlocked) {
					LoadNextLevel(5);
				}
				else {
				 //TODO show ad question
					ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_FIVE_KEY);
				}
			}
			else if(sixLockRect.Contains(mousePosition) )
			{
				if(sixUnlocked) {
				  LoadNextLevel(6);
				}
				else {
				 //TODO show ad question
					ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_SIX_KEY);
				}
			}
			else if(exitTextureRect.Contains(mousePosition)) {
				Application.LoadLevel("SettingsScene");
			}
			
		}
		//mobile checks
		else if(interactionEnabled && (isMobilePlatform && Input.touchCount == 1) )
		{
			
			Touch touch = Input.touches[0];
			if(touch.phase == TouchPhase.Began) {

				Vector2 fingerPos = new Vector2(0,0);
				fingerPos = touch.position;
				
				fingerPos.y =  height - (touch.position.y / Screen.height) * height;
				fingerPos.x = (touch.position.x / Screen.width) * width;
				
				if(isWideScreen) {
					//do extra computation
					fingerPos.x = fingerPos.x + (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * width;
				}

				if(showRewards && !isShowingAds){
					if(acceptBonusRect.Contains(fingerPos) ) {
				       AcceptShowAds();
				    }
					else if(denyBonusRect.Contains(fingerPos) ) {
				       DenyShowAds();
				    }
				}
				else if(oneLockRect.Contains(fingerPos) )
				{
					if(oneUnlocked) {
						LoadNextLevel(1);
					}
					else {
				 	//TODO show ad question
						ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_ONE_KEY);
					}
				}
				else if(twoLockRect.Contains(fingerPos) )
				{
					if(twoUnlocked) {
						LoadNextLevel(2);
					}
					else {
				 	//TODO show ad question
						ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_TWO_KEY);
					}
					
				}
				else if(threeLockRect.Contains(fingerPos) )
				{
					if(threeUnlocked) {
						LoadNextLevel(3);
					}
					else {
				 	//TODO show ad question
						ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_THREE_KEY);
					}
				}
				else if(fourLockRect.Contains(fingerPos) )
				{
					if(fourUnlocked) {
						LoadNextLevel(4);
					}
					else {
				 	//TODO show ad question
						ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_FOUR_KEY);
					}
					
				}
				else if(fiveLockRect.Contains(fingerPos) )
				{
					if(fiveUnlocked) {
						LoadNextLevel(5);
					}
					else {
				 	//TODO show ad question
						ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_FIVE_KEY);
					}
				}
				else if(sixLockRect.Contains(fingerPos) )
				{
					if(sixUnlocked) {
						LoadNextLevel(6);
					}
					else {
				 	//TODO show ad question
						ShowRewardController(worldKey + GameConstants.MISSION_SELECT_LEVEL_SIX_KEY);
					}
				}
				else if(exitTextureRect.Contains(fingerPos)) {
					Application.LoadLevel("SettingsScene");
				}
				
			}
		}
		
		
		
		//restore the matrix	
		GUI.matrix = svMat;	
	}

	void ShowRewardController(string level) {

	  //only if the world 
	  if(canShowAds) {
		showRewards = true;
		levelID = level;
	  }
		
	}

	void DenyShowAds() {
	  showRewards = false;
	}

	void AcceptShowAds() {
	//insane double check here!
	 if(showRewards && !isShowingAds) {
		UnityAdsScript ads = FindObjectOfType<UnityAdsScript>();
		if(ads!=null) {
		  isShowingAds = true;
		  ads.ShowAd(levelID);
		}
	 }
		
	}

	public void SetAcceptedReward(string rewardID) {

	  isShowingAds = false;
	  PlayerPrefs.SetInt(rewardID,1);
	  PlayerPrefs.Save();
	  CheckPreferences();
	  hasAcceptedReward = true;
	  StartCoroutine(Deactivate());
	  //play a cool sound
	  GameObject obj = GameObject.FindGameObjectWithTag("Scripts");
	  if(obj!=null) {
	    SoundEffectsHelper sounds = obj.GetComponent<SoundEffectsHelper>();
	    sounds.PlayPowerupSound();
	  }


	}

	IEnumerator Deactivate(){
	 yield return new WaitForSeconds(1.5f);
	  DisableRewardComponents();

	}
	//just hide the panel
	public void DisableRewardComponents() {
	  showRewards = false;
	  hasAcceptedReward = false;
	  isShowingAds = false;
	}

	 void LoadNextLevel(int scene) {
		
		Application.LoadLevel("World" + world + "Scene" + scene);
	}
}
