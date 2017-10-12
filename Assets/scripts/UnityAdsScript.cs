using UnityEngine;
using System.Collections;//for inumerator

//Using the package from the store we do not need to import this
//#if UNITY_IPHONE || UNITY_ANDROID
//using UnityEngine.Advertisements;
//#endif
public class UnityAdsScript : MonoBehaviour
{

  public string zone = null;
  private string rewardId;

	/*
	 public void Start() {

	 }
  */
	 public void ShowAd(string reward) {

	    Debug.Log("Show ad for mission:" + reward);
	    //mission + level key
	    rewardId = reward;
		// Place this where you want to call the ads from
		//StartCoroutine (showAdWhenReady());

	 }
	/*
	public void ShowAdSimple() {

		// Place this where you want to call the ads from
		StartCoroutine (showAdWhenReadySimple());

	 }

	IEnumerator showAdWhenReadySimple() {
		while (!Advertisement.IsReady(zone)) {
			yield return null;
		}
		Advertisement.Show(zone);
	}

	// Use this coroutine to wait until the ads are ready before showing them
	IEnumerator showAdWhenReady() {
		while (!Advertisement.IsReady(zone)) {
			yield return null;
		}
		var options = new ShowOptions { resultCallback = HandleShowResult };
		Advertisement.Show(zone, options);
	}*/

    /*public void ShowRewardedAd() {

	    if (Advertisement.IsReady("rewardedVideo"))
	    {
	      var options = new ShowOptions { resultCallback = HandleShowResult };
	      Advertisement.Show("rewardedVideo", options);
	    }
   }*/
/*
  private void HandleShowResult(ShowResult result)
  {

	MissionSelectionScript rewardsController = FindObjectOfType<MissionSelectionScript>();

    switch (result)
    {
      case ShowResult.Finished:
        Debug.Log("The ad was successfully shown. Adding reward " + rewardId);
        //
        // YOUR CODE TO REWARD THE GAMER
        // Give coins etc.
        if(rewardsController!=null) {
         rewardsController.SetAcceptedReward(rewardId);
        }
        break;
      case ShowResult.Skipped:
        Debug.Log("The ad was skipped before reaching the end.");
		if(rewardsController!=null) {
          rewardsController.DisableRewardComponents();
        }
        break;
      case ShowResult.Failed:
        Debug.LogError("The ad failed to be shown.");
		if(rewardsController!=null) {
			rewardsController.DisableRewardComponents();
        }
        break;
    }
  }*/


  
}