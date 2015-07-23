using UnityEngine;
using System.Collections;
using RescueJelly;

public class MissionsScript : MonoBehaviour {

	public int scrollFontSize = 22;
	public Font scrollFont;
	public int lineHeight = 1;

	private GUIStyle centeredStyleLarger;
	
	private Texture2D exitTexture;
	private Rect exitTextureRect;


	public bool missionOneCompleted = false;
	public bool missionTwoCompleted = false;
	public bool missionThreeCompleted = false;
	public bool missionFourCompleted = false;
	//public bool missionFiveCompleted = false;
	
	//these are the lock icons
	private SpriteRenderer missionTwoLock;
	private SpriteRenderer missionThreeLock;
	private SpriteRenderer missionFourLock;

	//these are the mission buttons
	private SpriteRenderer missionOne;
	private SpriteRenderer missionTwo;
	private SpriteRenderer missionThree;
	private SpriteRenderer missionFour;


	private static RuntimePlatform platform;
	bool isMobilePlatform = false;
	private bool loading = false;
	private bool isShowingText = false;
	GUISkin skin;

	private GUIResolutionHelper resolutionHelper;
	private TextLocalizationManager translationManager;

	//We will not use game center for the missions (only for achievements and leaderboards)
	//missions could have been, but were not, designed on Game Center as achievements
	void Start () {
	
	    //add a delay because of the touches stored
		StartCoroutine(Wait(3));
		
		//Invoke("LoadNextScene",20); 
		//loads main screen after 20 seconds
		
		//make sure we are not stopped
		Time.timeScale = 1.0f; 
		skin = Resources.Load("GUISkin") as GUISkin;
		exitTexture = Resources.Load("menu") as Texture2D;
		missionFourCompleted=true;

		missionOne = GameObject.FindGameObjectWithTag("Mission1").GetComponent<SpriteRenderer>();
		missionTwo = GameObject.FindGameObjectWithTag("Mission2").GetComponent<SpriteRenderer>();
		missionThree = GameObject.FindGameObjectWithTag("Mission3").GetComponent<SpriteRenderer>();
		missionFour = GameObject.FindGameObjectWithTag("Mission4").GetComponent<SpriteRenderer>();

		missionTwoLock = GameObject.FindGameObjectWithTag("lockworld2").GetComponent<SpriteRenderer>();
		missionThreeLock = GameObject.FindGameObjectWithTag("lockworld3").GetComponent<SpriteRenderer>();
		missionFourLock = GameObject.FindGameObjectWithTag("lockworld4").GetComponent<SpriteRenderer>();

		//isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);
		//check the conquered missions
		

		CheckMissions();

		//the interval is hardcoded
		InvokeRepeating("ChangeTextVisibility",1.0f,1.0f);

		
	}
	//called before start
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
		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android);
	}

	void CheckMissions() {
		bool missionTwoLocked = true;
		bool missionThreeLocked = true;
		bool missionFourLocked = true;

		missionOneCompleted = PlayerPrefs.GetInt(GameConstants.MISSION_1_KEY,0)>0;
	    missionTwoCompleted = PlayerPrefs.GetInt(GameConstants.MISSION_2_KEY,0)>0;
		missionThreeCompleted = PlayerPrefs.GetInt(GameConstants.MISSION_3_KEY,0)>0;
		missionFourCompleted = PlayerPrefs.GetInt(GameConstants.MISSION_4_KEY,0)>0;

		if(missionOneCompleted) {
		    missionOne.sprite = missionOne.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;

			//get the lock of mission 2 and show it
			missionTwoLocked = false;
		}
		else {
			missionOne.sprite = missionOne.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}
		    
		if(missionTwoCompleted) {
		    missionTwo.sprite = missionTwo.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
			//get the lock of mission 3 and show it
			missionThreeLocked = false;
		}
		else {
			missionTwo.sprite = missionTwo.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(missionThreeCompleted) {
		    missionThree.sprite = missionThree.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
			//get the lock of mission 4 and show it
			missionFourLocked = false;
		}
		else {
			missionThree.sprite = missionThree.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}

		if(missionFourCompleted) {
		    missionFour.sprite = missionFour.gameObject.GetComponent<DoubleSpriteScript>().spriteEnabled;
		}
		else {
			missionFour.sprite = missionFour.gameObject.GetComponent<DoubleSpriteScript>().spriteDisabled;
		}


		//PlayerPrefs.GetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,1);
		missionTwoLock.enabled = missionTwoLocked;
		missionThreeLock.enabled = missionThreeLocked;
		missionFourLock.enabled = missionFourLocked;
	}

	private string DetectMissionTouchesDesktop() {


		if(Input.GetMouseButtonDown(0)){
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
			
			if(hitCollider){
			loading = true;
			  return hitCollider.transform.gameObject.tag ;
			 
			}
		}
		return "xxx";
	}

	private string DetectMissionTouchesMobile() {


		for (int i = 0; i < Input.touchCount; ++i) {
			if (Input.GetTouch(i).phase == TouchPhase.Began) {
				Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
				RaycastHit2D hitInfo = Physics2D.Raycast(touchPosition, Vector2.zero);
				// RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
				if(hitInfo)
				{
										loading = true;
					return hitInfo.transform.gameObject.tag;
					 
					// Here you can check hitInfo to see which collider has been hit, and act appropriately.
				}
			}
		}
		return "xxx";
	}
	
	//delay to clear touches
	private IEnumerator Wait(long seconds)
		
	{
		
		yield return new WaitForSeconds(seconds);

		
	}
	
	void Update() {

	if(!loading) {
	  
	  string tagTouched = isMobilePlatform ?  DetectMissionTouchesMobile() : DetectMissionTouchesDesktop();
	  bool load=  false;
	  if(tagTouched.Equals("Mission1")) {
	    PlayerPrefs.SetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,1);
	    load=true;
	  }
		else if(tagTouched.Equals("Mission2")) {
		PlayerPrefs.SetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,2);
		load=true;
	  }
		else if(tagTouched.Equals("Mission3")) {
		PlayerPrefs.SetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,3);
		load=true;
	  }
		else if(tagTouched.Equals("Mission4")) {
		PlayerPrefs.SetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,4);
		load=true;
	  }
	  if(load)
	    Application.LoadLevel("MissionSelection");
	  }
	}
	
	
	void BuildLargerLabelStyle() {
	
		centeredStyleLarger = GUI.skin.GetStyle("Label");
		centeredStyleLarger.alignment = TextAnchor.MiddleLeft;
		centeredStyleLarger.font = scrollFont;
		centeredStyleLarger.fontSize = scrollFontSize;
	}

	void ChangeTextVisibility() {

		isShowingText = !isShowingText;
	}
	
	
	// Update is called once per frame
	void OnGUI () {
	
	   GUI.skin = skin;

	//if(Event.current.type==EventType.Repaint) {
	
		if (centeredStyleLarger == null) {
			BuildLargerLabelStyle();
		}
	    
	
		
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		
		bool isWideScreen = resolutionHelper.isWidescreen;
		Vector3 scaleVector = resolutionHelper.scaleVector;
		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (resolutionHelper.scaleX - scaleVector.y) / 2 * resolutionHelper.screenWidth, 0, 0), Quaternion.identity, scaleVector);

		}
		else {
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
			
		}
		
		//GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,GUIResolutionHelper.Instance.scaleVector);
	   		
		int width = resolutionHelper.screenWidth; 
		int height = resolutionHelper.screenHeight;
		
			
		
		if(Event.current.type==EventType.Repaint) {

			exitTextureRect = new Rect(width-110,30,96,96);
			GUI.DrawTexture(exitTextureRect,exitTexture);

			
			if(isShowingText) {
				//reset color back to white
				centeredStyleLarger.normal.textColor =  Color.white;
				GUI.Label(new Rect(width/2-165, 100, 600, 50), 
		          GetTranslationKey(GameConstants.MSG_TAP_UNLOCKED_MISSION),centeredStyleLarger);
			}	
			

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
				fingerPos.x = fingerPos.x + (GUIResolutionHelper.Instance.scaleX - GUIResolutionHelper.Instance.scaleVector.y) / 2 * width;
			}
			
			if(touch.phase == TouchPhase.Began)
				
			{
				// do amazing things
				if(exitTextureRect.Contains(fingerPos)) {
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

		//restor the matrix	
		GUI.matrix = svMat;
	  
				
		
	}
	
	string GetTranslationKey(string key) {
		return	translationManager.GetText(key);
	}
	
	//equivalent to menu
	private void LoadNextScene() {
		 
		Application.LoadLevel("StoreScene");
	}
	
	
}
