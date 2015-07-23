using UnityEngine;
using System.Collections;
using RescueJelly;
using Soomla.MyStore;
//for the apps purchases
public class RenderTextureScript : MonoBehaviour {

  public Texture2D boosterNormal;
  public Texture2D boosterGray;
  public Texture2D timeNormal;
  public Texture2D timeGray;
  public Texture2D lifesNormal;
  public Texture2D lifesGray;

  private GUIResolutionHelper resolutionHelper;

  private GameControllerScript controller;

  private string msg = "Helps:";

  GUISkin skin;

 
	// Use this for initialization
	void Start () {
	  skin = Resources.Load("GUISkin") as GUISkin;
	}

	void Awake() {
	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	  TextLocalizationManager localization;

	  if(scripts!=null) {
		resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
		controller = scripts.GetComponent<GameControllerScript>();
		localization = scripts.GetComponent<TextLocalizationManager>();


	  }
	  else {
		resolutionHelper = GUIResolutionHelper.Instance;
		controller = GameControllerScript.Instance;
		localization = TextLocalizationManager.Instance;
	  }
	
	  msg = localization.GetText(GameConstants.MSG_HELPS_BOOSTERS);
	  resolutionHelper.CheckScreenResolution();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		int width = resolutionHelper.screenWidth;
	    int height = resolutionHelper.screenHeight;
	    
		Vector3 scaleVector = resolutionHelper.scaleVector;
		bool isWideScreen = resolutionHelper.isWidescreen;
	    
	    GUI.skin = skin;
		Matrix4x4 svMat = GUI.matrix;//save current matrix

		
		if(isWideScreen) {
			GUI.matrix = Matrix4x4.TRS(new Vector3( (resolutionHelper.scaleX - scaleVector.y) / 2 * width, 0, 0), Quaternion.identity, scaleVector);
			
			
		}
		else {
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scaleVector);
			
		}

		if(Event.current.type==EventType.Repaint && !controller.IsGameOver()) {

		    GUI.Label(new Rect(760,height-40,100,40),msg);

			Rect booster = new Rect(810,height-40,32,32);
			Rect time = new Rect(858,height-40,32,32);
			Rect life = new Rect(906,height-40,32,32);

			if(PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID)) {
				GUI.DrawTexture(booster,boosterNormal,ScaleMode.ScaleToFit);
			}
			else {
				GUI.DrawTexture(booster,boosterGray,ScaleMode.ScaleToFit);
			}

			if(PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID)) {
				GUI.DrawTexture(time,timeNormal,ScaleMode.ScaleToFit);
			}
			else {
				GUI.DrawTexture(time,timeGray,ScaleMode.ScaleToFit);
			}

			if(PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID)) {
				GUI.DrawTexture(life,lifesNormal,ScaleMode.ScaleToFit);
			}
			else {
				GUI.DrawTexture(life,lifesGray,ScaleMode.ScaleToFit);
			}


		}

		GUI.matrix = svMat;
	}
}
