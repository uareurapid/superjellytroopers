using UnityEngine;
using System.Collections;

public class TextScrollScript : MonoBehaviour {

	public string[] scrollIntro;
	public float offset = 0;
	public float speed = 2.5f;
	public int mode = 2;
	public int scrollFontSize = 22;
	public Font scrollFont;
	public Font labelFont;
	public int lineHeight = 1;

	private GUIStyle centeredStyleLarger;
	
	private Texture2D exitTexture;
	private Rect exitTextureRect;

	private static RuntimePlatform platform = Application.platform;
	private bool isMobilePlatform = false;

	//show copyright notice
	public bool showStoryOnly = false; 
	public Color32 defaultTextColor =  new Color32(223,168,43,255); 
	
	GUISkin skin;
	GUIResolutionHelper resolutionHelper;
	void Start () {
	
	    //add a delay because of the touches stored
		StartCoroutine(Wait(3));
		
		//Invoke("LoadNextScene",20); 
		//loads main screen after 20 seconds
		
		//make sure we are not stopped
		Time.timeScale = 1.0f; 
		skin = Resources.Load("GUISkin") as GUISkin;
		exitTexture = Resources.Load("menu") as Texture2D;
		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android);
		
	}
	//called before start
	void Awake() {
		
		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
		resolutionHelper.CheckScreenResolution();
		offset = resolutionHelper.screenHeight + 200; //was 200
	}
	
	//delay to clear touches
	private IEnumerator Wait(long seconds)
		
	{
		
		yield return new WaitForSeconds(seconds);

		
	}
	
	void Update() {
	
		
	}

	//TODO SHOW something on next release (until first update on app store, we will not do nothing special)
	private bool HaveJustFinishedGame() {
		return false;
	}
		
	
	
	void BuildLargerLabelStyle() {
	
		centeredStyleLarger = GUI.skin.GetStyle("Label");
		centeredStyleLarger.alignment = TextAnchor.MiddleLeft;
		centeredStyleLarger.font = scrollFont;
		centeredStyleLarger.fontSize = scrollFontSize;
	}
	
	
	// Update is called once per frame
	void OnGUI () {
	
	GUI.skin = skin;

	
	    BuildLargerLabelStyle();
	   
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

		if(!showStoryOnly) {
			//reset color back to white
			centeredStyleLarger.normal.textColor =  Color.white;

			centeredStyleLarger.fontSize = 25;
			GUI.Label(new Rect(width/3,height-100,500,40),"Copyright ©2015-2019",centeredStyleLarger);		
			GUI.Label(new Rect(width/3,height-70,500,40),"http://www.pcdreams-software.com",centeredStyleLarger);
		}



		//yellow scroll colour
		centeredStyleLarger.font = scrollFont;

		if(showStoryOnly) {
			centeredStyleLarger.normal.textColor =  Color.black;
		}
		else {
			centeredStyleLarger.normal.textColor = defaultTextColor;//  new Color32(223,168,43,255);
		}


		if (mode == 1){ 
			offset += Time.deltaTime * speed; 
		} 
		else if (mode == 2) { 
			offset -= Time.deltaTime * speed; 
		}


			int max = scrollIntro.Length-1;
			
			for (int i = max; i >=0 ; i--) {
				float roff = (scrollIntro.Length*-45) + (i*45 + offset);

				float alph = Mathf.Sin((roff/height)*180*Mathf.Deg2Rad);
				GUI.color = new Color(1,1,1, alph);

				GUI.Label(new Rect(width/3-40,roff,width-120, 70),scrollIntro[i],centeredStyleLarger);
				GUI.color = new Color(1,1,1,1);
			}
			
			//if we have scrolled all text, we move to next scene
			if (offset > height + 200) { //both were 200
				mode = 2;
			} else if (offset < -200) { 
				mode = 1;
			}


			if(Event.current.type==EventType.Repaint && !showStoryOnly) {
			
				//menu button
				//if(isWideScreen) {
				//  exitTextureRect = new Rect(Screen.width - 100 * (resolutionHelper.scaleX - scaleVector.y) / 2,20,96,96);
				//}
				//else {
				  exitTextureRect = new Rect(width-110,30,96,96);
				//}


				GUI.DrawTexture(exitTextureRect,exitTexture);

			}



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

		//restore the original matrix
		GUI.matrix = svMat;
	
		
	}
	
	
	
	//equivalent to menu
	private void LoadNextScene() {
		 
		Application.LoadLevel("StoreScene");
	}
	
	
}
