using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class ScreenShotScript : MonoBehaviour
{
	private int count = 0;
	string fileName;
	
	public Texture2D takeScreenshot;
	private Rect screenshotRect;
	GUISkin skin;
	private bool textureEnabled = true;
	//native definition
	[DllImport ("__Internal")]
	private static extern void _TakeScreenshot(string path);
	GUIResolutionHelper resolutionHelper;

	void Start() {

		skin = Resources.Load("GUISkin") as GUISkin;
	}
	
	void Awake() {

	GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	if(scripts!=null) {
	 resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
	}
	else {
	  resolutionHelper = GUIResolutionHelper.Instance;
	}

	  resolutionHelper.CheckScreenResolution();
	  #if UNITY_ANDROID || UNITY_IOS
	     textureEnabled = true;
	  #endif
	  #if UNITY_BLACKBERRY
	     textureEnabled = false;
	  #endif
	}
	
	public static void TakeScreenshot(string path)
	{
		// Call plugin only when running on real device
		if (Application.platform != RuntimePlatform.OSXEditor)
			_TakeScreenshot(path);
	}
	
	void Update()
	{
		#if !UNITY_BLACKBERRY
		if (Input.touchCount == 1 && takeScreenshot!=null && textureEnabled)
		{
			Touch touch = Input.touches[0]; 
			
			if(touch.phase == TouchPhase.Began)
				
			{
				// do amazing things
				Vector2 fingerPos = new Vector2(0,0);
				fingerPos = touch.position;
				
				int width = resolutionHelper.screenWidth; 
				int height = resolutionHelper.screenHeight;
				
				fingerPos.y =  height - (touch.position.y / Screen.height) * height;
				fingerPos.x = (touch.position.x / Screen.width) * width;
				
				if(screenshotRect.Contains(fingerPos)) {
				
				   //hide the texture
				    textureEnabled = false;
					StartCoroutine(ScreenshotEncode());
					
				}
				
			}
		}
		#endif
			
	}

	
	void OnGUI() {
	  #if !UNITY_BLACKBERRY
	  GUI.skin = skin;
	  
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,resolutionHelper.scaleVector);
		
		if(Event.current.type==EventType.Repaint && textureEnabled) {
		
		
		  GUIStyle style = BuildGUIStyle();
		  
			GUI.Label(new Rect(110,(int)resolutionHelper.screenHeight / 3 * 2 -20,200,40),"Share");
		
			screenshotRect = new Rect(70,(int)resolutionHelper.screenHeight / 3 * 2,128,128);
		    GUI.DrawTexture(screenshotRect,takeScreenshot);
		}

		GUI.matrix = svMat;
	  #endif
	}
	
	GUIStyle BuildGUIStyle() {
	
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		//centeredStyleSmaller.font = font;
		//centeredStyleSmaller.fontSize = 22;
		return centeredStyleSmaller;
	}
	
	/**
	Application.dataPath
	Contains the path to the game data folder (Read Only).

The value depends on which platform you are running on:

Unity Editor: <path to project folder>/Assets

Mac player: <path to player app bundle>/Contents

iPhone player: <path to player app bundle>/<AppName.app>/Data

Win player: <path to executablename_Data folder>

Web player: The absolute url to the player data file folder (without the actual data file name)

Flash: The absolute url to the player data file folder (without the actual data file name)
	*/
	
	IEnumerator ScreenshotEncode()
	{
		// wait for graphics to render
		yield return new WaitForEndOfFrame();
		
		// create a texture to pass to encoding
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		
		// put buffer into texture
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		texture.Apply();
		
		// split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
		yield return 0;
		
		byte[] bytes = texture.EncodeToPNG();
		
		// save our test image (could also upload to WWW)
		fileName = Application.persistentDataPath + "/screenshot-" + count + ".png";
		
		Debug.Log("Trying to save screenshot to " + fileName);
		File.WriteAllBytes(fileName, bytes);
		count++;
		
		Debug.Log("SavedScreenshot to " + fileName);
		// Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
		DestroyObject( texture );
		
		#if UNITY_IOS && !UNITY_EDITOR	
			TakeScreenshot(fileName);
		#endif
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		Debug.Log("calling android openShare");
		AndroidJNIHelper.debug = true; 
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.pcdreams.superjellytroopers.MainActivity")) { 
			Debug.Log("AndroidJavaClass " + javaClass!=null ? " CLASS OK" : "CLASS NULL");

			//AndroidJavaObject javaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
			//Debug.Log("AndroidJavaObject " + javaObject!=null ? " OBJECT OK" : "OBJECT NULL");
			
			//Debug.Log ("Call the non static version of openShare");
			//javaObject.Call("openShare",fileName);
			
			Debug.Log ("Call the static version of openShareStatic");
			javaClass.CallStatic("openShareStatic",fileName); 

		} 

		#endif
		//Debug.Log( Application.dataPath + "/../testscreen-" + count + ".png" );
	}
	
	public void HelloFromAndroid(string dataReceived) 
	{
		Debug.Log("ScreenshotScript: Received data from Android plugin: " + dataReceived);
	}
	
	//enable / disable screenshots
	public void EnableScreenshots() {
	 textureEnabled = true;
	}
	public void DisableScreenshots() {
	 textureEnabled = false;
	}
}
