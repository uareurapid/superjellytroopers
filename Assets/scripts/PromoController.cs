using UnityEngine;
using System.Collections;
using RescueJelly;

public class PromoController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	  #if UNITY_ANDROID || UNITY_IPHONE
	  //only show if not already done!
	  if(!PlayerPrefs.HasKey(GameConstants.OPENED_PROMO)) {
		gameObject.GetComponent<UnityEngine.UI.Image>().enabled = true;
	  }
	  #endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenStoreURL() {

      #if UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.pcdreams.surroundit");
	  #endif

	  #if UNITY_IPHONE
		Application.OpenURL("https://itunes.apple.com/us/app/surround-it/id1082025867?ls=1&mt=8");
      #endif

	  #if UNITY_ANDROID || UNITY_IPHONE
      //save for next time!!
      gameObject.GetComponent<UnityEngine.UI.Image>().enabled = false;
      PlayerPrefs.SetInt(GameConstants.OPENED_PROMO,1);
      PlayerPrefs.Save();
	  #endif
	}
}
