using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RescueJelly;

public class RewardVideoController : MonoBehaviour {

    public UnityEngine.UI.Image acceptImage;//accept button
	public UnityEngine.UI.Image denyImage;//deny video button

	public UnityEngine.UI.Text rewardText;//reward text

	private const string originalRewardText = "Watch a video to unlock a this level!";
	public UnityEngine.UI.Image bonusImage;//bonus image (virus or health)


	private UnityAdsScript adsManager;


	//public const string MISSION_4_KEY = "mission_4";
    //public const string MISSION_SELECT_LEVEL_ONE_KEY = "_level_1";



	// Use this for initialization
	void Start () {

		adsManager = GameObject.FindObjectOfType<UnityAdsScript>();
	}
	
	// Update is called once per frame
	void Update () {

	}


	//control what user already has accepted
	public bool HasAcceptedReward(string key) {
		return PlayerPrefs.HasKey(key);
	}

	public void SetAcceptedReward(string rewardId) {

	  PlayerPrefs.SetInt(rewardId,1);
	  PlayerPrefs.Save();
	  //play some sound
		//TODO save the key
		//call the mission selection script
		MissionSelectionScript script = FindObjectOfType<MissionSelectionScript>();
		if(script!=null) {
			script.CheckPreferences();
		}
		StartCoroutine(Deactivate());


	}

	IEnumerator Deactivate(){
	 yield return new WaitForSeconds(1.5f);
	 DisableAllComponents();

	}


	public void SetBonusText(string text) {
	 rewardText.text = text;
	}

	public void EnableBonusImage() {
	 bonusImage.enabled = true;
	}

	public void EnableRewardText() {
	 rewardText.text = originalRewardText;
	 rewardText.enabled = true;
	}

	public void EnableAcceptButton() {
	 acceptImage.enabled = true;
	}

	public void EnableDenyButton() {
	 denyImage.enabled = true;
	}

	public void EnableComponentImage() {
	 GetComponent<UnityEngine.UI.Image>().enabled =  true;
	}

	//DISABLE
	public void DisableBonusImage() {
	 bonusImage.enabled = false;
	}

	public void DisableRewardText() {
	 rewardText.enabled = false;
	}

	public void DisableAcceptButton() {
	 acceptImage.enabled = false;
	}

	public void DisableDenyButton() {
	 denyImage.enabled = false;
	}

	public void DisableComponentImage() {
	 GetComponent<UnityEngine.UI.Image>().enabled =  false;
	}

	public void EnableAllComponents() {
	  EnableDenyButton();
	  EnableAcceptButton();
	  EnableBonusImage();
	  EnableRewardText();
	  EnableComponentImage();
	}

	public void ChangeRewardText() {
	  rewardText.text = "Congratulations. You unlocked a new level!";
	}

	public void DisableAllComponents() {
	  DisableDenyButton();
	  DisableAcceptButton();
	  DisableBonusImage();
	  DisableRewardText();
	  DisableComponentImage();


	}
}
