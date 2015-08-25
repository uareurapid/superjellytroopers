using UnityEngine;
using System.Collections;
using RescueJelly;


/// <summary>
/// Player controller and behavior
/// </summary>
public class PlayerScript : MonoBehaviour
{
	
	//ScoreScript scoreScript;

	//rect with the texture/direction icons
	bool isDead = false;
	public Texture2D lifeIcon;
	private HealthScript playerHealth;
	private GameControllerScript controller;
	private GUIResolutionHelper resolutionHelper;

	public Texture2D failSafeIcon;
	private Rect failSafeRect;
	public bool failSafeUsed = false;

	//this is needed for the pickup speed
	//the jelly must inherit player speed
	//at every new level the value is reset to deafut speed (1)
	public float speedX = 1;

	Transform cachedTransform;
	
	
	private bool isVisible=false;
	
	private Vector3 startingPos;
	
	private GUISkin skin;
	
	private TextLocalizationManager translationManager;

	private bool buyedInfiniteLifes = false;
	private bool isMobilePlatform = false;
	private static RuntimePlatform platform;

	void Start() {

		skin = Resources.Load("GUISkin") as GUISkin;
		isDead = false;		
		playerHealth = gameObject.GetComponent<HealthScript>();

		cachedTransform = transform;
		//Save starting position
		startingPos = cachedTransform.position;

		isMobilePlatform = (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.BlackBerryPlayer);
		
	}
	
	void Awake() {
		GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
		if(scripts!=null) {
		  controller = scripts.GetComponent<GameControllerScript>();
		  resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
		  translationManager = scripts.GetComponent<TextLocalizationManager>();
		}
		else {
		  controller = GameControllerScript.Instance;
		  resolutionHelper = GUIResolutionHelper.Instance;
		  translationManager = TextLocalizationManager.Instance;
		}

		translationManager.LoadSystemLanguage(Application.systemLanguage);
		//changeSpriteCounter = Time.deltaTime;
		//make sure we have this updated
		resolutionHelper.CheckScreenResolution();

		CheckInAppPurchases();
	}

	//todo, code me
	public void LaunchFailsafe() {
		GameObject jelly = GameObject.FindGameObjectWithTag("Jelly");
		if(jelly!=null) {
		  JellyScript script = jelly.GetComponent<JellyScript>();
		  if(script!=null){
		    script.LaunchFailsafe();
		  }
		}
	}

	void CheckInAppPurchases() {
	   //infinite lifes
	   buyedInfiniteLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);
	 }
	
	void OnGUI() {
	
		GUI.skin = skin;
		
		if(Event.current.type==EventType.Repaint && !controller.IsGameOver()) {

			
			Matrix4x4 svMat = GUI.matrix;//save current matrix
			GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,resolutionHelper.scaleVector);
			
			int num = buyedInfiniteLifes ? 1 : playerHealth.hitPoints;
			
			DrawText(GetTranslationKey(GameConstants.MSG_LIFES) + " ",20,20,45,120,40);

		    int x = 70; int y=50;
			//just draw 1 x N
			if(buyedInfiniteLifes) {
				  Rect life = new Rect(x,y,48,48);
				  GUI.DrawTexture(life, lifeIcon);
				  DrawText(" X " + GetTranslationKey(GameConstants.MSG_INFINITE_LIFES),20,120,50,140,40);
			}
			else {
				for(int i=0; i < num; i++) {
				  Rect life = new Rect(x,y,48,48);
				  GUI.DrawTexture(life, lifeIcon);
				  x+=48;
			    }
			}



			if(failSafeIcon!=null && !failSafeUsed) {
			  failSafeRect = new Rect(40, resolutionHelper.screenHeight-100,48,48);
			  GUI.DrawTexture(failSafeRect,failSafeIcon);
			  DrawText(GetTranslationKey(GameConstants.MSG_FAILSAFE),20,35,resolutionHelper.screenHeight-130,120,40);
			  if(!isMobilePlatform) {
				 DrawText(GetTranslationKey(GameConstants.MSG_PRESS_FAILSAFE_KEY),20,35,resolutionHelper.screenHeight-70,240,40);
			  }
			}


			GUI.matrix = svMat;
		}
	}

	//TODO this should be an interface, somewhere
	string GetTranslationKey(string key) {
		return	translationManager.GetText(key);
	}
	
	GUIStyle BuildSmallerLabelStyle() {
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		//centeredStyleSmaller.font = scrollFont;
		centeredStyleSmaller.fontSize = 20 * (int)GUIResolutionHelper.Instance.scaleVector.x;
		return centeredStyleSmaller;
	}
	
	public void DrawText(string text, int fontSize, int x, int y,int width,int height) {
		
		
		GUIStyle centeredStyleSmaller = GUI.skin.GetStyle("Label");
		centeredStyleSmaller.alignment = TextAnchor.MiddleLeft;
		centeredStyleSmaller.font = controller.messagesFont;
		centeredStyleSmaller.fontSize = fontSize;
		
		GUI.Label (new Rect(x, y, width, height), text);
	}

	/*
	* Increase swing speed to double value
	*/
	public void IncreaseMovementSpeedBy(int speedFactor) {
		speedX = speedX * speedFactor;
	}
	
	void Update()
	{

	 if(IsPlayerAlive()) {

		GameObject jellyObj = GameObject.FindGameObjectWithTag("Jelly");
		if (jellyObj != null) {
			JellyScript jelly = jellyObj.GetComponent<JellyScript> ();
			if(jelly!=null) {
				failSafeUsed = !jelly.CanLaunchFailSafe();
			}
			else {
				failSafeUsed = true;
			}
		}

		


		if (!failSafeUsed && Input.touches.Length ==1 && isMobilePlatform) {
			    
			Touch touch = Input.touches[0];
			    
			if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)  {

				Vector2 fingerPos = new Vector2(0,0);
				fingerPos.y =  resolutionHelper.screenHeight - (touch.position.y / Screen.height) * resolutionHelper.screenHeight;
				fingerPos.x = (touch.position.x / Screen.width) * resolutionHelper.screenWidth;

				if(failSafeRect!=null && failSafeRect.Contains(fingerPos) && playerHealth.hitPoints!=null) {
				  	LaunchFailsafe();
				}

			}

		}
		else if(!isMobilePlatform && !failSafeUsed) {

		  if(Input.GetKeyDown(KeyCode.R)) {
		     LaunchFailsafe();
		   }
		}
	 }
		

		if(playerHealth.hitPoints==0) {
		  HandleLooseAllLifes();
		}
		
	}
	
		

	
	void FixedUpdate()
	{
		// 5 - Move the game object
		//if(speed.x==speed.y && speed.x==0)

		  
		
	}

	
	
	public static Vector3 ClampVector3(Vector3 vec, Vector3 min, Vector3 max)
	{
		return Vector3.Min(max,Vector3.Max(min,vec));
	}


	
	void OnBecameVisible() {
		isVisible = true;
	}
	
	void OnBecameInvisible() {
		isVisible = false;
	}
	
	//reset for a new game/level
	public void ResetPlayer() {
	
		//HealthScript playerHealth = gameObject.GetComponent<HealthScript>();
		if(playerHealth!=null) {
			playerHealth.hitPoints = 3;	
		}
		isDead = false;
	}
	

	
	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		
	}

	//handle the collision with another sprite (not other trigger)
	void OnCollisionEnter2D(Collision2D collision)
	{

	}
	

	//takes a life

	public void TakeLife() {

		if(playerHealth!=null) {
			playerHealth.Damage(1);

			if(playerHealth.hitPoints>0) {
				controller.isJellyFalling = false;

			}
		
		}
	}
	
	public void TakeAllLifes() {
		
		if(playerHealth!=null) {
			playerHealth.Damage(20);//20 is enough
			
		}
	}

	public int GetRemainingLifes() {
	  return playerHealth.hitPoints;
	}
	
	
	//wait for 3 seconds before show next level
	IEnumerator PrepareForNextLevel()
	{
		yield return new WaitForSeconds(3);
		ShowGameOver(true);
	}



	//called by the engine when player dies
	void OnDestroy()
	{
	  
		if(controller.IsGameStarted()) {
			ShowGameOver(false);
		}
		//else just ignore this stuff
		
	}
	
	void ShowGameOver(bool showNextlevel) {
	
	    controller.EndGame(showNextlevel);
		// Game Over.
		// Add the script to the parent because the current game
		// object is likely going to be destroyed immediately.
		transform.parent.gameObject.AddComponent<GameOverScript>();
		
		
	}
	
	
	//when player dies, we save the score and end the game
	public void HandleLooseAllLifes() {
		
		isDead = true;
		playerHealth.hitPoints=0;
		//set the score key pref
		//PlayerPrefs.SetInt("HighScore",scoreScript.score);
		Destroy(gameObject);
	}
	
	public bool IsPlayerAlive() {
	  return !isDead && playerHealth.hitPoints > 0;
	}
	
	//increase the player health by x
	public void IncreaseHealthBy(int amount) {

		if(playerHealth!=null) {
		  playerHealth.hitPoints += amount;
		}
	}

	//self destroy method
	IEnumerator SelfDestroyPlayer() {
	 
		yield return new WaitForSeconds(3);
		ShowGameOver(false);
	}

	
}


// * - Awake() is called once when the object is created. See it as replacement of a classic constructor method.
// * - Start() is executed after Awake(). The difference is that the Start() method is not called if the script is not enabled (remember the checkbox on a component in the "Inspector").
// * - Update() is executed for each frame in the main game loop.
// * - FixedUpdate() is called at every fixed framerate frame. You should use this method over Update() when dealing with physics ("RigidBody" and forces).
// * - Destroy() is invoked when the object is destroyed. It's your last chance to clean or execute some code.
  
// * OnCollisionEnter2D(CollisionInfo2D info) is invoked when another collider is touching this object collider.
// * OnCollisionExit2D(CollisionInfo2D info) is invoked when another collider is not touching this object collider anymore.
// * OnTriggerEnter2D(Collider2D otherCollider) is invoked when another collider marked as a "Trigger" is touching this object collider.
// * OnTriggerExit2D(Collider2D otherCollider)

