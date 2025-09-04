using UnityEngine;
using System.Collections;


/// <summary>
/// Title screen script
/// </summary>
public class MenuScript : MonoBehaviour {

	private GUISkin skin;
	
	public int fontSize = 10;
	public Font font;
	//clip to play when hit next
	public AudioClip nextButtonAudio;
    AudioSource audioSource;
	// Use this for initialization
	private bool loading=false;
	
	public Texture2D exitTexture;
	public Rect exitTextureRect;
	
	void Start()
	{
		// Load a skin for the buttons
		skin = Resources.Load("GUISkin") as GUISkin;//CenteredSkin
		//SoomlaStore.Initialize(new ALFIERushAssets());
	}
	
	void Awake() {
		GameObject audioObject = GameObject.FindGameObjectWithTag("GameMusic");
		if(audioObject!=null) {
			audioSource = audioObject.GetComponent<AudioSource>();
			if(audioSource!=null) {
				audioSource.Play();
			}
		}
		
		GUIResolutionHelper.Instance.CheckScreenResolution();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.touchCount == 1 && exitTexture!=null)
		{
			Touch touch = Input.touches[0]; 
			
			if(touch.phase == TouchPhase.Began)
				
			{
				
				int width = GUIResolutionHelper.Instance.screenWidth;
				int height = GUIResolutionHelper.Instance.screenHeight;
				
				Vector2 fingerPos = new Vector2(0,0);
				fingerPos = touch.position;
				
				fingerPos.y =  height - (touch.position.y / Screen.height) * height;
				
				if(GUIResolutionHelper.Instance.isWidescreen) {
					fingerPos.x = (touch.position.x / Screen.width) * width;
					fingerPos.x = fingerPos.x + (GUIResolutionHelper.Instance.scaleX - GUIResolutionHelper.Instance.scaleVector.y) / 2 * width;
				}
				else {
					fingerPos.x = (touch.position.x / Screen.width) * width;
				}
				
				
				
				
				// do amazing things
				if(exitTextureRect.Contains(fingerPos)) {
					if(audioSource!=null) {
						//stop the background music and play the other sound
						audioSource.Stop();
						//play some random sound here
						audioSource.clip = nextButtonAudio;
						audioSource.Play();
					}
					Application.LoadLevel("StoreScene");
				}
				
			}
		}
	
	}
	

	void OnGUI()
	{
	
		GUI.skin = skin;
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		
		int width = GUIResolutionHelper.Instance.screenWidth;
		Vector3 scaleVector = GUIResolutionHelper.Instance.scaleVector;
		bool isWideScreen = GUIResolutionHelper.Instance.isWidescreen;
		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (GUIResolutionHelper.Instance.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			
			
		}
		else {
			
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
		}

		
		//if(Event.current.type==EventType.Repaint) {
			
			GUIStyle style=BuildGUIStyle();
			style.normal.textColor = Color.white;
			style.fontSize = 40 ;//*(int)GUIResolutionHelper.Instance.scaleVector.x;
			GUI.Label(new Rect((int)GUIResolutionHelper.Instance.screenWidth / 2 - 220,300,450,50),
			"Angry Alfie",style);
			
			style.normal.textColor = new Color(223,168,43,255);
			style.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(50,400,300,50),"How to play:",style);
			
			style.fontSize = 30;
			style.normal.textColor = Color.white;
			GUI.Label(new Rect(50,470,650,50),"Tap and move you finger around the screen to move Alfie",style);
			

			GUI.Label(new Rect(50,550,650,50),"Tap over the missile circle to shoot against the enemies:",style);
			
			if(isWideScreen) {
			  exitTextureRect = new Rect(Screen.width - 150 * (GUIResolutionHelper.Instance.scaleX - scaleVector.y) / 2 ,40,64,64);//874,40
			}
			else {
			  exitTextureRect = new Rect(GUIResolutionHelper.Instance.screenWidth - 150,40,64,64);//874,40
			}
			
			
			GUI.DrawTexture(exitTextureRect,exitTexture);
		//}
		
		GUI.matrix = svMat;

		
	}
	
	GUIStyle BuildGUIStyle() {
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.UpperCenter;
		centeredStyleSmaller.font = font;
		centeredStyleSmaller.fontSize = fontSize;
		return centeredStyleSmaller;
	}
	
	
	


	//Note: The OnGUI method is called every frame and should embed all 
	//the code that display a GUI element: lifebar, menus, interface, etc. 
	//The GUI object allows you to quickly create GUI components from the code, 
	//like a button with the GUI.Button method.

	/**
	 * Tip: The Application.LoadLevel() method job is to clear the current scene and to 
	 * instantiate all the game objects of the new one. Sometimes, you want to keep a game 
	 * object of a first scene into a second (e.g., to have a continuous music between two menus). 

     Unity provides a DontDestroyOnLoad(aGameObject) method for these cases. 
     Just call it on a game object and it won't be cleared when a new scene is loaded. 
     In fact, it won't be cleared at all. So if you want to remove it in a further scene, 
     you have to manually destroy it.
	 * */

}
