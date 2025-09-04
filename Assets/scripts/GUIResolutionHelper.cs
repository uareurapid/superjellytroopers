using UnityEngine;
using System.Collections;

/**
This is a helper class to allow GUI screen resolution independency for GUI elements
**/
public class GUIResolutionHelper : MonoBehaviour {

	private static GUIResolutionHelper instance;
	
	//the "target" resolution (iphone 4, bq aquaris on landscape mode)
	
	public int screenWidth = 1024;
	public int screenHeight = 768;
	
	public Vector3 scaleVector = new Vector3(1f,1f,1f);
	
	public bool isWidescreen = false;
	public float aspectRatio = 0.0f;
	
	public float scaleX = 0.0f;
	// Use this for initialization
	
	/*
	
	
	I improve you solve by still keep the ratio of GUI.

	scale.y = Screen.height/originalHeight; // calculate vert scale
	
	scale.x = scale.y; // this will keep your ratio base on Vertical scale
	
	scale.z = 1;
	
	float scaleX = Screen.witgh/originalWidth; // store this for translate
	
	Matrix4x4 svMat = GUI.matrix; // save current matrix // substitute matrix - only scale is altered from standard
	
	GUI.matrix = Matrix4x4.TRS(new Vector3( (scaleX - scale.y) / 2 * originalWidth, 0, 0), Quaternion.identity, scale);
	*/
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake()
	{
		// Register the singleton
		if(instance!=null) {
			Debug.Log("There is another instance gui resolution helper running");
		}
		else {
			instance = this;
		}
		
	}
	
	public void CheckScreenResolution() {

		//set the ppropriate scale factor
		scaleVector.x = (Screen.width + 0.0f)/screenWidth;
		scaleVector.y = (Screen.height + 0.0f) /screenHeight;
		scaleVector.z = 1f;
		
		aspectRatio = (float) (Screen.width + 0.0f) / Screen.height;
		if(aspectRatio > 1.5f) {//4:3 is 1.333, 16:9 is 1.7
		  //is 16:9
		  isWidescreen = true;
		  //grab the "normal" scale ration, for 4:3 screens
		  scaleX = scaleVector.x;
		  //keep the ratio equal to the vertical one
		  scaleVector.x = scaleVector.y;
		  //if widescreen than it scales more on x than on y
		}

	}
	
	public static GUIResolutionHelper Instance {
		get
		{
			if (instance == null)
			{
				GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
				if(scripts!=null) {
					instance = scripts.GetComponent<GUIResolutionHelper>();
				}
				else {
					instance = (GUIResolutionHelper)FindObjectOfType(typeof(GUIResolutionHelper));
					if (instance == null)
						instance = (new GameObject("GUIResolutionHelper")).AddComponent<GUIResolutionHelper>();
				}
				
			}

			return instance;
		}
	}
	
}
