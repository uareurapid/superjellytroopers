using UnityEngine;
using System.Collections;
using RescueJelly;

#if !UNITY_BLACKBERRY
using Soomla.Store;
using Soomla.MyStore;
#endif
public class StoreScript : MonoBehaviour {

	private static RuntimePlatform platform;

	//for android only
	private bool inAppBillingStarted = false;

	private GUISkin skin;

	//check if running premium version
	private bool buyedExtraLifes = false;
	private bool buyedExtraTime = false;
	private bool buyedExtraSpeed = false;
	private bool buyedInfiniteLifes = false;

	private TextLocalizationManager	translationManager;
	private GUIResolutionHelper resolutionHelper;

	public Texture2D lifesIconTexture;
	public Texture2D extraTimeIconTexture;
	public Texture2D exitIconTexture;
	public Texture2D storeIconTexture;
	public Texture2D restoreIconTexture;
	public Texture2D speedIconTexture;

	private GUIStyle style;
	public Font textFont;
	public int fontSize;

	Rect extraTimeTextureRect;
	Rect extraLifeTextureRect;
	Rect infiniteLifesTextureRect;
	Rect exitTextureRect;
	Rect extraSpeedTextureRect;

	Rect buyInfiniteLifesStoreTextureRect;
	Rect buyLifesStoreTextureRect;
	Rect buyTimeStoreTextureRect;
	Rect buySpeedStoreTextureRect;
	Rect restoreTextureRect;

	//only fetch the price once, not on every call
	string priceTime = "0.99";
	string priceLifes = "0.99";
	string priceSpeed = "0.99";
	string priceInfiniteLifes = "3.99";

	private bool purchaseInProgress = false;

	// Use this for initialization
	void Start () {

		skin = Resources.Load("GUISkin") as GUISkin;
		platform = Application.platform;

		//SOOMLA EVENT HANDLING STUFF
		#if !UNITY_BLACKBERRY && !UNITY_EDITOR
		StoreEvents.OnMarketPurchase += onMarketPurchase;
		StoreEvents.OnMarketRefund += onMarketRefund;
		StoreEvents.OnItemPurchased += onItemPurchased;
		StoreEvents.OnGoodEquipped += onGoodEquipped;
		StoreEvents.OnGoodUnEquipped += onGoodUnequipped;
		StoreEvents.OnGoodUpgrade += onGoodUpgrade;
		StoreEvents.OnBillingSupported += onBillingSupported;
		StoreEvents.OnBillingNotSupported += onBillingNotSupported;
		StoreEvents.OnMarketPurchaseStarted += onMarketPurchaseStarted;
		StoreEvents.OnItemPurchaseStarted += onItemPurchaseStarted;
		StoreEvents.OnUnexpectedErrorInStore += onUnexpectedErrorInStore;
	    StoreEvents.OnCurrencyBalanceChanged += onCurrencyBalanceChanged;
		StoreEvents.OnGoodBalanceChanged += onGoodBalanceChanged;
		StoreEvents.OnMarketPurchaseCancelled += onMarketPurchaseCancelled;
		StoreEvents.OnRestoreTransactionsStarted += onRestoreTransactionsStarted;
		StoreEvents.OnRestoreTransactionsFinished += onRestoreTransactionsFinished;
		StoreEvents.OnSoomlaStoreInitialized += onStoreControllerInitialized;
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
	
		StoreEvents.OnIabServiceStarted += onIabServiceStarted;
		StoreEvents.OnIabServiceStopped += onIabServiceStopped;


		#endif

		#if UNITY_BLACKBERRY
		//BlackBerryIAP.SetConnectionMode(false); //false is for real world testing
        // Register for events
        BlackBerryIAP.PurchaseSuccessfulEvent += PurchaseSuccessful;
        BlackBerryIAP.PurchaseFailedEvent += PurchaseFailed;
        BlackBerryIAP.GetPriceSuccessfulEvent += GetPriceSuccessful;
        BlackBerryIAP.GetPriceFailedEvent += GetPriceFailed;
        BlackBerryIAP.ExistingPurchasesSuccessfulEvent += ExistingPurchasesSuccessful;
        BlackBerryIAP.ExistingPurchasesFailedEvent += ExistingPurchasesFailed;
        BlackBerryIAP.IsSubscriptionActiveSuccessfulEvent += IsSubscriptionActiveSuccessful;
        BlackBerryIAP.IsSubscriptionActiveFailedEvent += IsSubscriptionActiveFailed;
        BlackBerryIAP.CancelSubscriptionSuccessfulEvent += CancelSubscriptionSuccessful;
        BlackBerryIAP.CancelSubscriptionFailedEvent += CancelSubscriptionFailed;
		#endif

		CheckInAppPurchases();

	}


	void LoadStyle() {
		style = GUI.skin.GetStyle ("Label");
		style.alignment = TextAnchor.MiddleLeft;
		style.font = textFont;
		style.fontSize = fontSize;
		style.normal.textColor = Color.white;
		
	}
	
	void Awake() {
	  
	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	  if(scripts!=null){
	    resolutionHelper = scripts.GetComponent<GUIResolutionHelper>();
		translationManager = scripts.GetComponent<TextLocalizationManager>();			
		
	  }
	  else{
	    resolutionHelper = GUIResolutionHelper.Instance;
		translationManager = TextLocalizationManager.Instance;
	  }
	  resolutionHelper.CheckScreenResolution();
	  translationManager.LoadSystemLanguage(Application.systemLanguage);

	}
	
	// Update is called once per frame
	void Update () {
				if ( IsMobilePlatform() && Input.touches.Length ==1) {

					//
					int screenHeight = resolutionHelper.screenHeight;
					int screenWidth = resolutionHelper.screenWidth;

				    bool touchedPause = false;
					Touch touch = Input.touches[0];
					if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)  {
				
					
						Vector2 fingerPos = new Vector2(0,0);
						fingerPos = touch.position;
						
						fingerPos.y =  screenHeight - (touch.position.y / Screen.height) * screenHeight;
						fingerPos.x = (touch.position.x / Screen.width) * screenWidth;



						if(resolutionHelper.isWidescreen) {
						//texture rect is at 712
						//wich is more or less a finger of 830	

						   //this is my 0 (zero) on a wideScree Matrix
						   float wideScreenOrigin = (resolutionHelper.scaleX - resolutionHelper.scaleVector.y) / 2 * screenWidth;
						   fingerPos.x = fingerPos.x + wideScreenOrigin - GameConstants.WIDESCREEN_CORRECTION_VALUE;
						}

						//Debug.Log("exitTextureRect.x:" + exitTextureRect.x);
						//Debug.Log("buyLifesStoreTextureRect.x:" + buyLifesStoreTextureRect.x);
						//Debug.Log("Finger position.x: " + fingerPos.x);
					
					
						//rect 712, dif 240, finger 550
						if( buyLifesStoreTextureRect.Contains(fingerPos) && !buyedExtraLifes )
						{	
						  if(!purchaseInProgress) {
							purchaseInProgress = true;
							BuyExtraLifes();
						  }

						}
						else if(buyTimeStoreTextureRect.Contains(fingerPos) && !buyedExtraTime )
						{	
						  if(!purchaseInProgress) {
							purchaseInProgress = true;
							BuyExtraTime();
						  }
						}
						else if(buyInfiniteLifesStoreTextureRect.Contains(fingerPos) && !buyedInfiniteLifes )
						{	
						  if(!purchaseInProgress) {
							purchaseInProgress = true;
							BuyInfiniteLifes();
						  }
						}
						else if(buySpeedStoreTextureRect.Contains(fingerPos) && !buyedExtraSpeed)
						{	
							if(!purchaseInProgress) {
							  purchaseInProgress = true;
							  BuyExtraSpeed();
							}
						}
						#if UNITY_IOS
						else if(restoreTextureRect.Contains(fingerPos) )
						{	
						    SoomlaStore.RestoreTransactions();
						}
						#endif
						else if(exitTextureRect.Contains(fingerPos) )
						{	
						    Application.LoadLevel("SettingsScene");
						}

					  }
				 }

				 //paint time

	}
		
		//check if we have the keys
	void CheckInAppPurchases() {
	   buyedExtraLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
	   buyedExtraTime = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
	   buyedExtraSpeed = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
	   buyedInfiniteLifes = PlayerPrefs.HasKey(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);


	   #if UNITY_BLACKBERRY
	   priceSpeed = ""+BlackBerryIAP.GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
	   priceLifes = ""+BlackBerryIAP.GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
	   priceTime = ""+BlackBerryIAP.GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
	   priceInfiniteLifes = ""+BlackBerryIAP.GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);
	   #endif

	   //get the market prices and currency
	   #if !UNITY_BLACKBERRY
	   priceSpeed = GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
	   priceLifes = GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
	   priceTime = GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
	   priceInfiniteLifes = GetPrice(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);
	   #endif

	   purchaseInProgress = false;

	}

		void OnGUI()
		{
		  


				if (style == null) {
					LoadStyle ();
				}
				// Set the skin to use
				GUI.skin = skin;

				Matrix4x4 svMat = GUI.matrix;//save current matrix

				int width = resolutionHelper.screenWidth;
				int height = resolutionHelper.screenHeight;
				Vector3 scaleVector = resolutionHelper.scaleVector;

				bool isWideScreen = resolutionHelper.isWidescreen;

				float wideScreenOrigin = (resolutionHelper.scaleX - scaleVector.y) / 2 * width;
				
				if (isWideScreen) {
					GUI.matrix = Matrix4x4.TRS (new Vector3 (wideScreenOrigin, 0, 0), Quaternion.identity, scaleVector);
				}
				else {
					GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scaleVector);
				}

				//check clicks, when playing on desktop env
				  if (Event.current.type == EventType.MouseUp && !IsMobilePlatform()) {
					  if(exitTextureRect.Contains(Event.current.mousePosition)) {
					     Application.LoadLevel("SettingsScene");
					  } 
					  else if(buyLifesStoreTextureRect.Contains(Event.current.mousePosition) && !buyedExtraLifes ) {	
					    //start from level 1 of current played world
						//Maybe have a rating system/share system to unlock this!!
						//we have already the share screenshot stuff
						//LoadNextLevel(lastWorld,1);
						if(!purchaseInProgress) {
						  purchaseInProgress = true;
						  BuyExtraLifes();
						}


					 }
					 else if(buyTimeStoreTextureRect.Contains(Event.current.mousePosition) && !buyedExtraTime) {	
					    //start from level 1 of current played world
						//Maybe have a rating system/share system to unlock this!!
						//we have already the share screenshot stuff
						//LoadNextLevel(lastWorld,1);
						if(!purchaseInProgress) {
						  purchaseInProgress = true;
						  BuyExtraTime();
						}


					 }
					 else if(buyInfiniteLifesStoreTextureRect.Contains(Event.current.mousePosition) && !buyedInfiniteLifes) {	
			
						if(!purchaseInProgress) {
						  purchaseInProgress = true;
						  BuyInfiniteLifes();
						}


					 }
					 else if(buySpeedStoreTextureRect.Contains(Event.current.mousePosition) && !buyedExtraSpeed ) {	
					    //start from level 1 of current played world
						//Maybe have a rating system/share system to unlock this!!
						//we have already the share screenshot stuff
						//LoadNextLevel(lastWorld,1);
						if(!purchaseInProgress) {
						  purchaseInProgress = true;
						  BuyExtraSpeed();
						}


					 }
					 #if UNITY_IOS
					 else if(restoreTextureRect.Contains(Event.current.mousePosition) ) {	
					   SoomlaStore.RestoreTransactions();
					 }
					 #endif
		 
		          }

				 

				if(Event.current.type==EventType.Repaint) {
		
					//time
					GUI.Label (new Rect(width/2-250, height/2-120, 450, 60), 
					translationManager.GetText(GameConstants.MSG_BUY_EXTRA_TIME),style);

					extraTimeTextureRect = new Rect(width/2-320,height/2-120,64,64);
					GUI.DrawTexture(extraTimeTextureRect,extraTimeIconTexture);

					if(!buyedExtraTime) {
								//width/2+200
						buyTimeStoreTextureRect = new Rect(width-312,height/2-120,64,64);
						GUI.DrawTexture(buyTimeStoreTextureRect,storeIconTexture);		
						GUI.Label (new Rect(width/2+265, height/2-120, 120, 60),priceTime,style);

					}

					//speed
					GUI.Label (new Rect(width/2-250, height/2-20, 450, 60), 
					translationManager.GetText(GameConstants.MSG_BUY_EXTRA_SPEED),style);
					extraSpeedTextureRect = new Rect(width/2-320,height/2-20,64,64);
					GUI.DrawTexture(extraSpeedTextureRect,speedIconTexture);

					if(!buyedExtraSpeed) {
								//width/2+200
						buySpeedStoreTextureRect = new Rect(width-312,height/2-20,64,64);
						GUI.DrawTexture(buySpeedStoreTextureRect,storeIconTexture);
						GUI.Label (new Rect(width/2+265, height/2-20, 120, 60),priceSpeed,style);
					}

					//---------------------------
					//lifes
					GUI.Label (new Rect(width/2-250, height/2+80, 450, 60), 
					translationManager.GetText(GameConstants.MSG_BUY_EXTRA_LIFES),style);
					extraLifeTextureRect = new Rect(width/2-320,height/2+80,64,64);
					GUI.DrawTexture(extraLifeTextureRect,lifesIconTexture);

					if(!buyedExtraLifes) {
								//width/2+200
						buyLifesStoreTextureRect = new Rect(width-312,height/2+80,64,64);
						GUI.DrawTexture(buyLifesStoreTextureRect,storeIconTexture);
						GUI.Label (new Rect(width/2+265, height/2+80, 120, 60),priceLifes,style);
					}
					//--------------------------
					//infinite lifes
					GUI.Label (new Rect(width/2-250, height/2+180, 450, 60), 
					translationManager.GetText(GameConstants.MSG_BUY_INFINITE_LIFES),style);
					infiniteLifesTextureRect = new Rect(width/2-320,height/2+180,64,64);
					GUI.DrawTexture(infiniteLifesTextureRect,lifesIconTexture);

					if(!buyedInfiniteLifes) {
								//width/2+200
						buyInfiniteLifesStoreTextureRect = new Rect(width-312,height/2+180,64,64);
						GUI.DrawTexture(buyInfiniteLifesStoreTextureRect,storeIconTexture);
						GUI.Label (new Rect(width/2+265, height/2+180, 120, 60),priceInfiniteLifes,style);
					}
					//---------------------------
					#if UNITY_IOS
					restoreTextureRect = new Rect(100,100,214,72);
					GUI.DrawTexture(restoreTextureRect,restoreIconTexture);
					#endif

					exitTextureRect = new Rect(width-110,30,96,96);
					GUI.DrawTexture(exitTextureRect,exitIconTexture);

				}
			//restore matrix
			GUI.matrix = svMat;
	  }

	#if !UNITY_BLACKBERRY
	public void onMarketPurchase(PurchasableVirtualItem pvi, string purchaseToken, string other, string orderId) {
		
		Debug.Log("onMarketPurchase: " + pvi.ItemId);
		//set to true when buy this item
		PlayerPrefs.SetString(pvi.ItemId,"true");
		CheckInAppPurchases();
		
		//onGUI needs to be udated afterwrds

	}

	//Get the market item price, no reference to currency
	public string GetPrice(string itemId) {
		string result = "0.99";
        PurchasableVirtualItem item = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(itemId);
         if( item.PurchaseType.GetType () == typeof(PurchaseWithVirtualItem) ){
           PurchaseWithVirtualItem purchaseType = (PurchaseWithVirtualItem)item.PurchaseType;
           result = ""+purchaseType.Amount;
         }
         else {
            PurchaseWithMarket purchaseType = (PurchaseWithMarket)item.PurchaseType;
            result = purchaseType.MarketItem.MarketPriceAndCurrency;
         }
       return result;
	}
	
	public void onMarketRefund(PurchasableVirtualItem pvi) {
		
		//Debug.Log("onMarketRefund");
	}
	
	public void onItemPurchased(PurchasableVirtualItem pvi,string payload) {
	   purchaseInProgress = false;

	}
	
	public void onGoodEquipped(EquippableVG good) {

	}
	
	public void onGoodUnequipped(EquippableVG good) {

	}
	
	public void onGoodUpgrade(VirtualGood good, UpgradeVG currentUpgrade) {

	}
	
	public void onBillingSupported() {

	}
	
	public void onBillingNotSupported() {
		
	}
	
	public void onMarketPurchaseStarted(PurchasableVirtualItem pvi) {
		purchaseInProgress = true;
	}
	
	public void onItemPurchaseStarted(PurchasableVirtualItem pvi) {

	}
	
	public void onMarketPurchaseCancelled(PurchasableVirtualItem pvi) {
		purchaseInProgress = false;
	}
	
	public void onUnexpectedErrorInStore(string message) {
		purchaseInProgress = false;

	}
	
	public void onCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded) {
		//ExampleLocalStoreInfo.UpdateBalances();
		//Debug.Log("onCurrencyBalanceChanged");
	}
	
	public void onGoodBalanceChanged(VirtualGood good, int balance, int amountAdded) {

	}
	
	public void onRestoreTransactionsStarted() {

	}
	
	public void onRestoreTransactionsFinished(bool success) {
		CheckInAppPurchases();
		StartCoroutine(ShowMessage("In-App Purchases Restored!", 1.5f));
	}

	public void onStoreControllerInitialized() {
	
	
	}
	#endif //end if !UNITY_BLACKBERRY

	#if UNITY_BLACKBERRY
		void PurchaseSuccessful(BlackBerryIAP.PurchaseEventArgs args)
    {
        Debug.Log("Purchase successfull!");
    }

    void PurchaseFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        Debug.Log("Failed to purchase item!");
    }

    void GetPriceSuccessful(BlackBerryIAP.PriceEventArgs args)
    {
		Debug.Log("Get price successfull!");
    }

    void GetPriceFailed(BlackBerryIAP.ErrorEventArgs args)
    {
		Debug.Log("Failed to get price!");
    }

    void ExistingPurchasesSuccessful(BlackBerryIAP.ExistingPurchasesEventArgs args)
    {
        Debug.Log("Check existing purchases sucessfull");
    }

    void ExistingPurchasesFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        switch (args.Error)
        {
            case BlackBerryIAP.ErrorCode.UserCancelled:
                Debug.Log("User cancelled existing purchases query.");
                break;
            default:
                Debug.Log("Error while getting existing purchases.");
                break;
        }
       
    }

    void CancelSubscriptionSuccessful(BlackBerryIAP.CancelSubscriptionEventArgs args)
    {
        Debug.Log("Subscription canceled!");
    }

    void CancelSubscriptionFailed(BlackBerryIAP.ErrorEventArgs args)
    {
		Debug.Log("Unable to cancel subscription!");
    }

    void IsSubscriptionActiveSuccessful(BlackBerryIAP.IsSubscriptionActiveEventArgs args)
    {
		Debug.Log("Subscription is activated!");
    }

    void IsSubscriptionActiveFailed(BlackBerryIAP.ErrorEventArgs args)
    {
		Debug.Log("Failed to activate subscription");
    }
	#endif



	IEnumerator ShowMessage (string message, float delay) {
      GameObject textObj = GameObject.FindGameObjectWithTag("JellyTxt");
      if(textObj!=null) {
		GUIText guiText = textObj.GetComponent<GUIText>();
		if(guiText!=null) {
			guiText.text = message;
     		guiText.enabled = true;
     		yield return new WaitForSeconds(delay);
     		guiText.enabled = false;
		}

      }

 	}

	private bool IsMobilePlatform() {
		return (platform == RuntimePlatform.IPhonePlayer) || (platform==RuntimePlatform.BlackBerryPlayer) 
		|| (platform == RuntimePlatform.Android);
	}



	void OnDestroy() {
			
	  #if UNITY_ANDROID && !UNITY_EDITOR
		//if not closed yet, close now
		if(inAppBillingStarted) {
			SoomlaStore.StopIabServiceInBg();
		}
	       
	  #endif

	  #if UNITY_BLACKBERRY
		
        // Unregister for events
        BlackBerryIAP.PurchaseSuccessfulEvent -= PurchaseSuccessful;
        BlackBerryIAP.PurchaseFailedEvent -= PurchaseFailed;
        BlackBerryIAP.GetPriceSuccessfulEvent -= GetPriceSuccessful;
        BlackBerryIAP.GetPriceFailedEvent -= GetPriceFailed;
        BlackBerryIAP.ExistingPurchasesSuccessfulEvent -= ExistingPurchasesSuccessful;
        BlackBerryIAP.ExistingPurchasesFailedEvent -= ExistingPurchasesFailed;
        BlackBerryIAP.IsSubscriptionActiveSuccessfulEvent -= IsSubscriptionActiveSuccessful;
        BlackBerryIAP.IsSubscriptionActiveFailedEvent -= IsSubscriptionActiveFailed;
        BlackBerryIAP.CancelSubscriptionSuccessfulEvent -= CancelSubscriptionSuccessful;
        BlackBerryIAP.CancelSubscriptionFailedEvent -= CancelSubscriptionFailed;
	  #endif
	}

	#if UNITY_ANDROID && !UNITY_EDITOR
	public void onIabServiceStarted() {
		inAppBillingStarted = true;
	}
	public void onIabServiceStopped() {
		inAppBillingStarted = false;	
	}
	#endif


	//buy action

	void BuyExtraLifes() {
		
		Debug.Log("buying extra lifes");
	    #if !UNITY_BLACKBERRY
		StoreInventory.BuyItem(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID);
		#endif
		#if UNITY_BLACKBERRY
		BlackBerryIAP.Purchase(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID, null, null, null, null, null, null);
		#endif
	}

	void BuyInfiniteLifes() {
		
		Debug.Log("buying infinite lifes");
		#if !UNITY_BLACKBERRY
		StoreInventory.BuyItem(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);
		#endif
		#if UNITY_BLACKBERRY
		BlackBerryIAP.Purchase(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID);
		#endif
		
	}

	void BuyExtraTime() {
		
		Debug.Log("buying extra time");
		#if !UNITY_BLACKBERRY
		StoreInventory.BuyItem(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
		#endif
		#if UNITY_BLACKBERRY
		BlackBerryIAP.Purchase(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID);
		#endif
		
	}

	void BuyExtraSpeed() {
		
		Debug.Log("buying extra speed");
		#if !UNITY_BLACKBERRY
		StoreInventory.BuyItem(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
		#endif
		#if UNITY_BLACKBERRY
		BlackBerryIAP.Purchase(Soomla.MyStore.JellyTrooperAssets.JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID);
		#endif
		
	}
}
