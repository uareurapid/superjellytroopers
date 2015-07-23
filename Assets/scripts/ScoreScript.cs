using UnityEngine;
using System.Collections;

public class ScoreScript : MonoBehaviour {

	//void OnGUI(){
	//	guiText.text = "Score: 0";
	//}
	public int score = 0;
	Vector3 position;
	public Font font;
	public int fontSize;
	GUISkin skin;
	//private HealthScript healthScript;
	// Use this for initialization
	void Start () {
	
		//healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthScript>();
		//position = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		skin = Resources.Load("GUISkin") as GUISkin;
	}
	
	void Awake() {
		GUIResolutionHelper.Instance.CheckScreenResolution();
	}

	// Update is called once per frame
	void Update () {
		//position = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
	 // Set the score text.

	}
	
	void OnGUI() {
		Matrix4x4 svMat = GUI.matrix;//save current matrix
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,GUIResolutionHelper.Instance.scaleVector);
		
		if(Event.current.type==EventType.Repaint) {
			DrawText( "Score: " + score,fontSize,GUIResolutionHelper.Instance.screenWidth-200,20,200,40);
		}
		
		GUI.matrix = svMat;
	}

	public void IncreaseScore(int valuePassed) {
		score += valuePassed;
	}
	
	public int GetScore() {
	  return score;
	}
	
	//save current score
	public void SaveHighScore() {
	print ("inside...." + score);
		print (" read from prefs: " + PlayerPrefs.GetInt("HighScore"));
		if(score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore",score );
		}
	}
	
	public void DrawText(string text, int fontSize, int x, int y,int width,int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = font;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label (new Rect(x, y, width, height), text);
	}
}
