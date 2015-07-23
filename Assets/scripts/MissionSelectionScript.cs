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

  private bool isMobilePlatform = true;
  private GUISkin skin;

  private static RuntimePlatform platform;

  int world = 1;
	// Use this for initialization
	void Start () {
				// Load a skin for the buttons
	 skin = Resources.Load("GUISkin") as GUISkin;
	 isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);
	 exitTexture = Resources.Load("menu") as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CheckPreferences() {
	  //this gives the world to check
	   world = PlayerPrefs.GetInt(GameConstants.MISSION_SELECT_WORLD_SELECTED_KEY,1);
	   string worldKey = GameConstants.MISSION_1_KEY;

	   switch(world) {
		 case 1: worldKey = GameConstants.MISSION_1_KEY;
		 oneUnlocked = true;
		 break;
		 case 2: worldKey = GameConstants.MISSION_2_KEY;
		 oneUnlocked = PlayerPrefs.GetInt(GameConstants.MISSION_2_KEY,0)>0;
		 break;
		 case 3: worldKey = GameConstants.MISSION_3_KEY;
		 oneUnlocked = PlayerPrefs.GetInt(GameConstants.MISSION_3_KEY,0)>0;
		 break;
		 case 4: worldKey = GameConstants.MISSION_4_KEY;
		 oneUnlocked = PlayerPrefs.GetInt(GameConstants.MISSION_4_KEY,0)>0;
		 break;
	   }


	   twoUnlocked = PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_TWO_KEY);
	   threeUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_THREE_KEY);
	   fourUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_FOUR_KEY);
	   fiveUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_FIVE_KEY);
	   sixUnlocked =  PlayerPrefs.HasKey(worldKey + GameConstants.MISSION_SELECT_LEVEL_SIX_KEY);

	}

	void Awake() {
		
	  GUIResolutionHelper.Instance.CheckScreenResolution();
	  CheckPreferences();

	}

	void OnGUI() {
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


		if(Event.current.type==EventType.Repaint) {

					oneLockRect = new Rect(width / 2-300,height -500,128,128);
				    twoLockRect = new Rect(width / 2-80,height-500,128,128);
					threeLockRect = new Rect(width / 2+150,height -500,128,128);
					fourLockRect = new Rect(width / 2-300,height -300,128,128);
					fiveLockRect = new Rect(width / 2-80,height-300,128,128);
					sixLockRect = new Rect(width / 2+150,height-300,128,128);
					
						/*oneLockRect = new Rect(width / 2-300,height -500,128,128);
						twoLockRect = new Rect(width / 2+150,height-500,128,128);
						threeLockRect = new Rect(width / 2-300,height -300,128,128);
						fourLockRect = new Rect(width / 2+150,height -300,128,128);
						fiveLockRect = new Rect(width / 2-80,height-400,128,128);
						sixLockRect = new Rect(width / 2-80,height-400,128,128);*/


					GUI.DrawTexture(oneLockRect,oneUnlocked ? oneUnlock : oneLock);
					GUI.DrawTexture(twoLockRect,twoUnlocked ? twoUnlock : twoLock);
					GUI.DrawTexture(threeLockRect,threeUnlocked ? threeUnlock : threeLock);
					GUI.DrawTexture(fourLockRect,fourUnlocked ? fourUnlock : fourLock);
					GUI.DrawTexture(fiveLockRect,fiveUnlocked ? fiveUnlock : fiveLock);
					GUI.DrawTexture(sixLockRect,sixUnlocked ? sixUnlock : sixLock);


				    exitTextureRect = new Rect(width-110,30,96,96);
				    GUI.DrawTexture(exitTextureRect,exitTexture);

			

		}

				//********************* CLICK / TOUCH CHECKS *******************
				//desktop checks
		if(Event.current.type == EventType.MouseUp && !isMobilePlatform) {

			Vector2 mousePosition = Event.current.mousePosition;

			    if(oneUnlocked && oneLockRect.Contains(mousePosition) )
				{
					LoadNextLevel(1);
				}
				else if(twoUnlocked && twoLockRect.Contains(mousePosition) )
				{

				    LoadNextLevel(2);

				}
				else if(threeUnlocked && threeLockRect.Contains(mousePosition) )
				{
					LoadNextLevel(3);
				}
				else if(fourUnlocked && fourLockRect.Contains(mousePosition) )
				{
					LoadNextLevel(4);


				}
				else if(fiveUnlocked && fiveLockRect.Contains(mousePosition) )
				{
					LoadNextLevel(5);
				}
				else if(sixUnlocked && sixLockRect.Contains(mousePosition) )
				{
					LoadNextLevel(6);
				}
				else if(exitTextureRect.Contains(mousePosition)) {
				  Application.LoadLevel("SettingsScene");
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


				if(isWideScreen) {
				//do extra computation?? really necessary???
					fingerPos.x = fingerPos.x + (GUIResolutionHelper.Instance.scaleX - GUIResolutionHelper.Instance.scaleVector.y) / 2 * width;
				}

				if(oneUnlocked && oneLockRect.Contains(fingerPos) )
				{
					LoadNextLevel(1);
				}
				else if(twoUnlocked && twoLockRect.Contains(fingerPos) )
				{

				    LoadNextLevel(2);

				}
				else if(threeUnlocked && threeLockRect.Contains(fingerPos) )
				{
					LoadNextLevel(3);
				}
				else if(fourUnlocked && fourLockRect.Contains(fingerPos) )
				{
					LoadNextLevel(4);


				}
				else if(fiveUnlocked && fiveLockRect.Contains(fingerPos) )
				{
					LoadNextLevel(5);
				}
				else if(sixUnlocked && sixLockRect.Contains(fingerPos) )
				{
					LoadNextLevel(6);
				}
				else if(exitTextureRect.Contains(fingerPos)) {
				  Application.LoadLevel("SettingsScene");
				}

			}
		}
			
		//restore the matrix	
		GUI.matrix = svMat;	
	}

	 void LoadNextLevel(int scene) {
		
		Application.LoadLevel("World" + world + "Scene" + scene);
	}
}
