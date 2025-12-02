using UnityEngine;
using System.Collections;
// #if !UNITY_BLACKBERRY
// using Soomla.Store;
// #endif
using System.Text;
//using Soomla.MyStore;
using RescueJelly;
#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
#endif

public class LogoSceneScript : MonoBehaviour {

    public Font font;
    public int fontSize;
	public Font otherFont;
    
	GameObject superTxt;
	GameObject jellyTxt;
	GameObject troopersTxt;
	// Use this for initialization
	GUISkin skin;
	GUIStyle style;
	bool explodedAlready = false;

	GUIResolutionHelper resolutionHelper;

	const string companyName = "PC Dreams Software";
	const string presents = "Presents...";
	int charCounterFirst = 0;
	int charCounterSecond = 0;
	bool changeLine = false;
	StringBuilder firstLine;
	StringBuilder secondLine;

	//skip story button
	public Texture2D exitTexture;
    private Rect exitTextureRect;

	private bool passToNextScene = false;
    private bool disabledStory = false;

	AudioSource audioType;
	private bool isMobilePlatform = false;
	private static RuntimePlatform platform = Application.platform;
	
	void Start () {
	
		Time.timeScale = 1.0f;
		skin = Resources.Load("GUISkin") as GUISkin;
		//init soomla store
		// #if !UNITY_BLACKBERRY
		// SoomlaStore.Initialize(new JellyTrooperAssets());
		// #endif

		superTxt = GameObject.FindGameObjectWithTag("SuperTxt");
		jellyTxt = GameObject.FindGameObjectWithTag("JellyTxt");
		troopersTxt = GameObject.FindGameObjectWithTag("TroopersTxt");

		DisableRenderers();
		firstLine = new StringBuilder();
		secondLine = new StringBuilder();
		charCounterFirst = -1;
		charCounterSecond = -1;
		changeLine = false;

		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		if(scripts!=null) {
			audioType = GetComponent<AudioSource>();
	    }

	    passToNextScene = false;

		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);

		Debug.Log("IS MOBILE PLATFORM: " + isMobilePlatform);
		#if UNITY_ANDROID && !UNITY_EDITOR
	
		// recommended for debugging:
    	PlayGamesPlatform.DebugLogEnabled = true;

    	// Activate the Google Play Games platform
    	PlayGamesPlatform.Activate();
	    #endif
	
        float timeToWrite = 6f; //6 seconds
        float typeRate = timeToWrite / (companyName.Length + presents.Length);
        InvokeRepeating("CheckTyping",21f,typeRate);
		//disable story after 20 seconds
		Invoke("DisableStoryRendering",20f);
	}

	void DisableRenderers() {
	  if(superTxt!=null && jellyTxt!=null && troopersTxt!=null) {

		foreach(SpriteRenderer renderer in superTxt.GetComponentsInChildren<SpriteRenderer>()) {
		  renderer.enabled = false;
		}
		foreach(SpriteRenderer renderer in jellyTxt.GetComponentsInChildren<SpriteRenderer>()) {
		  renderer.enabled = false;
		}
		foreach(SpriteRenderer renderer in troopersTxt.GetComponentsInChildren<SpriteRenderer>()) {
		  renderer.enabled = false;
		}
	  }

	}

	void EnableRenderer(GameObject objectTxt) {

	 if(objectTxt!=null) {
	
		foreach(SpriteRenderer renderer in objectTxt.GetComponentsInChildren<SpriteRenderer>()) {
		  renderer.enabled = true;
		}
	  }

	}

	//stop writing the story scroll
	void DisableStoryRendering() {
	//if still not called
	 if(!disabledStory) {
	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	  if(scripts!=null) {
	    TextScrollScript scroll = scripts.GetComponent<TextScrollScript>();
	    if(scroll!=null) {
	      scroll.enabled = false;
	      disabledStory = true;
	    }
	  }
	 }

	  
	}

	void EnableGravityScale(GameObject objectTxt) {

	 if(objectTxt!=null) {
	
		  foreach(Rigidbody2D body in objectTxt.GetComponentsInChildren<Rigidbody2D>()) {
		  body.gravityScale=1.0f;
		}
	  }

	}

	void ShowParachutesScene(){
		Application.LoadLevel("StoryScene");
	}
	
	void Awake() {
	  //check screen settings
	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		if (scripts != null)
		{
			resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();

		}
		else
		{
			resolutionHelper = GUIResolutionHelper.Instance;


		}
	  resolutionHelper.CheckScreenResolution();
	  ClearPlayerPrefs();
	  
	}

		//clear the saved keys
	 void ClearPlayerPrefs() {

		//put them all to zero
		PlayerPrefs.SetInt(GameConstants.TOTAL_ELAPSED_TIME_SECS_KEY,0);
		PlayerPrefs.SetInt(GameConstants.TOTAL_LOST_LIFES_KEY,0);
		PlayerPrefs.SetInt(GameConstants.TOTAL_SAVED_JELLIES_KEY,0);
		//this is the total key for this game run
		PlayerPrefs.SetInt(GameConstants.TOTAL_SCORE_KEY,0);


	}
	// Update is called once per frame
	void Update()
	{
		//is the story still scrolling?

		// // TODO IndexOutOfRangeException: Index was outside the bounds of the array.
		 if (isMobilePlatform)
		 {
		 	if ((!disabledStory || !passToNextScene) && Input.touches.Length >= 1)
		 	{

		 		Touch touch = Input.touches[0];
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				{

					Vector2 fingerPos = new Vector2(0, 0);
					fingerPos = touch.position;

					//Vector2 fingerPositionOther = new Vector2(0, 0);
					//fingerPositionOther = touch.position;

					// Y is OK
					fingerPos.y = resolutionHelper.screenHeight - (touch.position.y / Screen.height) * resolutionHelper.screenHeight;
					fingerPos.x = touch.position.x - resolutionHelper.screenWidth;// (touch.position.x / Screen.width) * resolutionHelper.screenWidth;

					// Debug.Log("AFTER CHANGE X, Y: " + fingerPos);

					// fingerPositionOther.y =  screenHeight - (touch.position.y / Screen.height) * screenHeight;
					// fingerPositionOther.x = touch.position.x - screenWidth;

					// Debug.Log("FINGER POSITION OTHER: " + fingerPositionOther);

					// // 			// OLD
					// // 			// if (resolutionHelper.isWidescreen)
					// // 			// {

					// // 			// 	fingerPos.x = fingerPos.x + (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * resolutionHelper.screenWidth;
					// // 			// }

					// if (resolutionHelper.isWidescreen)
					// {

					// 	Debug.Log("WIDESCREEN STUFF");

					// 	float wideScreenOrigin = (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * resolutionHelper.screenWidth;
					// 	fingerPos.x = fingerPos.x + wideScreenOrigin - GameConstants.WIDESCREEN_CORRECTION_VALUE;

					// }


					if (exitTextureRect.Contains(fingerPos))
					{
						Debug.Log("first OK");
						DisableStoryRendering();
						CancelInvoke("CheckTyping");
						passToNextScene = true;
						ShowParachutesScene();
					}
					// else if (exitTextureRect.Contains(fingerPositionOther))
		 			// {
					// 	Debug.Log("other OK");
		 			// 	DisableStoryRendering();
		 			// 	CancelInvoke("CheckTyping");
		 			// 	passToNextScene = true;
		 			// 	ShowParachutesScene();
		 			// }
		 		}
		 	}
		 }

	}

	//invoked every 0.5 seconds
	void CheckTyping() {

	  //still on firts line?
	  if(charCounterFirst < companyName.Length-1) {
	    firstLine.Append(companyName.Substring(++charCounterFirst,1));
	  }
	  else {
	    //change line
	    changeLine = true;
	  }
	    
	  //already processed the first line and not finished with second? then append!
      if(changeLine && (charCounterSecond < presents.Length-1) ) {
        secondLine.Append(presents.Substring(++charCounterSecond,1));
      }

	  
	 
	 //start playing typing sound
	  if(audioType!=null && charCounterFirst==0){

	  //if first time stop the spooky one and start the typewriter other
		GameObject spooky = GameObject.FindGameObjectWithTag("SpookyAudio");
      	if(spooky!=null) {
        	AudioSource spookyAudio = spooky.GetComponent<AudioSource>();
        	spookyAudio.Stop();

      	}

		 audioType.Play();
	  }
	
	  
	}
	
	void OnGUI() {


		GUI.skin = skin;
		// -------------------------
		Matrix4x4 svMat = GUI.matrix;//save current matrix

		Vector3 scaleVector = resolutionHelper.scaleVector;
		bool isWideScreen = resolutionHelper.isWidescreen;
		int width = resolutionHelper.screenWidth;
		int height = resolutionHelper.screenHeight;

		Matrix4x4 normalMatrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
		Matrix4x4 wideMatrix = Matrix4x4.TRS(new Vector3( (resolutionHelper.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
		//we use the center matrix for the buttons

		//assign normal matrix by default
		GUI.matrix = normalMatrix;
		// -------------------------
	
		
		BuilGUIStyle();

		//
		if (Event.current.type == EventType.Repaint)
		{
			if (passToNextScene && disabledStory)
			{

				style.normal.textColor = Color.grey;
				GUI.Label(new Rect(width / 2 - 100, height / 2 - 100, 300, 70), "Loading...", style);
				style.normal.textColor = Color.black;

			}
			else
			{

				style.normal.textColor = Color.black;

				//draw exit button

				//if (Event.current.type == EventType.Repaint)
				//{
				if (isWideScreen)
				{
					GUI.matrix = wideMatrix;
				}
				else
				{
					GUI.matrix = normalMatrix;
				}

				exitTextureRect = new Rect(width - 100, 50, 96, 96);
				GUI.DrawTexture(exitTextureRect, exitTexture);
				//}

				if (Time.timeSinceLevelLoad >= 31)
				{

					if (!passToNextScene)
					{
						//just to avoid call this again
						passToNextScene = true;
						ShowParachutesScene();
					}

				}
				else if (Time.timeSinceLevelLoad >= 21 && Time.timeSinceLevelLoad <= 27)
				{

					TypeWrite(width, height, style, Event.current.type);

				}
				else if (Time.timeSinceLevelLoad > 27 && Time.timeSinceLevelLoad < 27.2)
				{


					if (audioType != null && audioType.isPlaying)
					{
						audioType.mute = true;
					}
				}
				else if (Time.timeSinceLevelLoad >= 27.2)
				{

					//don´t need to call this anymore	
					CancelInvoke("CheckTyping");

					if (Time.timeSinceLevelLoad <= 27.5 && !explodedAlready)
					{

						GameObject audio = GameObject.FindGameObjectWithTag("Bomb");
						if (audio != null)
						{

							SoundEffectsHelper.Instance.PlayExplosionSound();
							SpecialEffectsHelper.Instance.PlayExplosionEffect(audio.transform.position);
						}
						explodedAlready = true;

					}

					else if (Time.timeSinceLevelLoad <= 28)
					{
						EnableRenderer(superTxt);
					}

					else if (Time.timeSinceLevelLoad <= 29)
					{
						EnableRenderer(jellyTxt);
					}
					else if (Time.timeSinceLevelLoad <= 29.5)
					{
						//enable gravity scale for upper row
						EnableGravityScale(superTxt);
						EnableRenderer(troopersTxt);
					}

					else if (Time.timeSinceLevelLoad <= 30)
					{
						//enable gravity scale for upper row
						EnableGravityScale(jellyTxt);
					}
					else if (Time.timeSinceLevelLoad <= 30.5)
					{
						//enable gravity scale for upper row
						EnableGravityScale(troopersTxt);
					}

					/*
					style.normal.textColor =  Color.blue;
					style.fontSize+=5;
					style.font = otherFont;
					GUI.Label(new Rect(width/2 - 400,height/2 - 100,800,70),"Super Jelly Troopers!",style);
					style.fontSize+=5;
					*/
				}
			}
		} // end repaint

		//---------------------------------------------------------
		//*************** CHEK TEXTURE CLICKS *********************
		//---------------------------------------------------------
		//before checking the clicks we put the correct matrix

		// if (isWideScreen)
		// {
		// 	GUI.matrix = wideMatrix;
		// }
		// else
		// {
		// 	GUI.matrix = normalMatrix;
		// }
		//---------------------------------------------

		// TODO IndexOutOfRangeException: Index was outside the bounds of the array.
	// 	if (isMobilePlatform)
	// 	{

	// 		if (Input.touches.Length >= 1)
	// 		{
	// 			if (!disabledStory || !passToNextScene)
	// 			{

	// 				Touch touch = Input.touches[0];
	// 				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
	// 				{

	// 					// NEW 	
	// 					int screenHeight = resolutionHelper.screenHeight;
	// 					int screenWidth = resolutionHelper.screenWidth;

	// 					Vector2 fingerPos = new Vector2(0, 0);
	// 					fingerPos = touch.position;

	// 					Debug.Log("finger position: " + fingerPos);

	// 					fingerPos.y = resolutionHelper.screenHeight - (touch.position.y / Screen.height) * resolutionHelper.screenHeight;
	// 					fingerPos.x = (touch.position.x / Screen.width) * resolutionHelper.screenWidth;

	// 					Debug.Log("finger position adjusted: " + fingerPos);



	// 					if (exitTextureRect.Contains(fingerPos))
	// 					{
	// 						Debug.Log("GOT CORRECT position: " + fingerPos);
	// 						DisableStoryRendering();
	// 						CancelInvoke("CheckTyping");
	// 						passToNextScene = true;
	// 						ShowParachutesScene();
	// 					}
	// 				}
	// 		}
	// 	}
	// }


      //check desktop touches
      if(!isMobilePlatform) {
			if(Event.current.type == EventType.MouseUp){ //!disabledStory || !passToNextScene && (
	  		//not mobile
				if(exitTextureRect.Contains(Event.current.mousePosition)) {
					DisableStoryRendering();
					CancelInvoke("CheckTyping");
					passToNextScene = true;
					ShowParachutesScene();
				}		
	 		}

			if(Input.GetKeyDown(KeyCode.Escape)) {
		  		Application.Quit();
			}
      }
		
	GUI.matrix = svMat;
	
  }

	//type the given string
	void TypeWrite(int width,int height,GUIStyle style, EventType type) {

		if(type==EventType.Repaint) {
		  if(!changeLine) {
		  //draw just the first line
			GUI.Label(new Rect(width/2 - 300, height/2 - 200,600,70),firstLine.ToString(),style);
		  }
		  else {
		  //draw both lines
			 GUI.Label(new Rect(width/2 - 300, height/2 - 200,600,70),firstLine.ToString(),style);
			 GUI.Label(new Rect(width/2 - 200,height/2 + 50, 400,70),secondLine.ToString(),style);
		  }
		}
	}
		
	//TODO credit cvhttp://www.freesfx.co.uk/soundeffects/airplanes/
	
	void BuilGUIStyle() {
		style = GUI.skin.GetStyle("Label");
		style.alignment = TextAnchor.UpperCenter;
		style.font = font;
		style.fontSize = fontSize;
	}
	
	
	void LoadIntroScene()
	{

	    //put the color back as white
		style.normal.textColor =  Color.white;
		Application.LoadLevel("World1Scene1");
		
	}


}