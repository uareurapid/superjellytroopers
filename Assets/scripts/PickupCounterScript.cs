using UnityEngine;
using System.Collections;

public class PickupCounterScript : MonoBehaviour {

	public int numberPickups = 0;
	public bool showMax = false;
	public Font font;
	public int fontSize;
	public Texture2D icon;
	
	public int textureXPosition;
	public int textureYPosition;
	public int textureWidth = 48;
	public int textureHeight = 48; 
	// Use this for initialization
	GUISkin skin;
	void Start () {
		skin = Resources.Load("GUISkin") as GUISkin;
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake() {
	  GUIResolutionHelper.Instance.CheckScreenResolution();
	  if(showMax) {
		  numberPickups = 0;
	  }
	  else {
		  numberPickups = 5;
	  }
	  
	}

	public void AddPickup() {
		numberPickups++;
	}
	
	public void AddMultiplePickups(int count) {
		numberPickups+=count;
	}
	
	public void ResetPickups() {
		numberPickups=0;
	}
	
	void OnGUI() {
	    
	    GUI.skin = skin;
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,GUIResolutionHelper.Instance.scaleVector);
		
		if(Event.current.type==EventType.Repaint) {
		
		    //we only show max for star pickups
			//is a star counter, since there are no limits for the max of shoots player can have
			if(showMax) {
				int numMax = GameControllerScript.Instance.GetNumberEnergyPickups();//number of energy pickups in level
				DrawText( "X " + numberPickups +  " / " + numMax , fontSize, textureXPosition+40,textureYPosition,120,40);
			}
			else {
			 //is shots
				DrawText( "X " + numberPickups, fontSize, textureXPosition+40,textureYPosition,120,40);
			}
			
			
			
			         
			GUI.DrawTexture(new Rect(textureXPosition,textureYPosition,textureWidth,textureHeight),icon);
		}
		GUI.matrix = svMat;
	}
	
	public void DrawText(string text, int fontSize, int x, int y,int width,int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = font;
		centeredStyleSmaller.fontSize = fontSize ;
		
		GUI.Label (new Rect(x, y, width, height), text);
	}
	
	public void RemovePickup() {
		if(numberPickups>0)
		   numberPickups--;
		
	}
}
