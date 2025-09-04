using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

namespace Soomla.MyStore {
	
	/// <summary>
	/// This class defines our game's economy, which includes virtual goods, virtual currencies
	/// and currency packs, virtual categories, and non-consumable items.
	/// </summary>
	public class JellyTrooperAssets : IStoreAssets{
		
		
		/** Static Final Members **/
		
		
		//---------------------------------------------------------
		//this is the one
		public const string JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID = "superjelly.troopers.extra.lifes";//2 lifes
		public const string JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID = "superjelly.troopers.extra.time";//30 secs
		public const string JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID = "superjelly.troopers.extra.speed";//swing left/right faster

		public const string JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID = "superjelly.troopers.infinite.lifes";//infinite lifes
		//---------------------------------------------------------
		
		public const string JELLY_TROOPERS_CURRENCY_ITEM_ID = "currency_id";
		
		public const string FOUR_HUNDRED_ALFIE_PACK_ID = "400ID";
		
		/** Virtual Currencies **/
		
		public static VirtualCurrency JELLY_TROOPERS_CURRENCY = new VirtualCurrency(
			"jelly_troopers_currency",							// name
			"currency_description",						// description
			JELLY_TROOPERS_CURRENCY_ITEM_ID						// item id
			);
		
		/** virtual goods **/
		public static VirtualGood VIRTUAL_GOOD = new SingleUseVG(
			"Pavlova",                                         			// name
			"Gives customers a sugar rush and they call their friends", // description
			"pavlova",                                          		// item id
			new PurchaseWithVirtualItem(JELLY_TROOPERS_CURRENCY_ITEM_ID, 175)); // the way this virtual good is purchased
		
		/** virtual currency packs **/
		public static VirtualCurrencyPack FOUR_HUNDRED_ALFIE_PACK = new VirtualCurrencyPack(
			"400 ALFIEs",                                  // name
			"Test purchase of an item",                 	// description
			"ALFIEs_400",                                  // item id
			400,                                            // number of currencies in the pack
			JELLY_TROOPERS_CURRENCY_ITEM_ID,                        // the currency associated with this pack
			new PurchaseWithMarket(FOUR_HUNDRED_ALFIE_PACK_ID, 4.99)
			);
		
		/** Virtual Categories **/
		// The ALFIE rush theme doesn't support categories, so we just put everything under a general category.
		
		public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
			"General", new List<string>(new string[] { "alfie dummy category" })
			);
		
		
		/** Market MANAGED Items **/
		
			public static VirtualGood JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_PACK  = new LifetimeVG(
			"Super Jelly Troopers (Extra Lifes)",
			"Test purchase of MANAGED item: Super Jelly Troopers(Extra Lifes).",
			JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID,
			new PurchaseWithMarket(new MarketItem(JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_ID, MarketItem.Consumable.NONCONSUMABLE , 0.99))
			);

			public static VirtualGood JELLY_TROOPERS_EXTRA_TIME_PRODUCT_PACK  = new LifetimeVG(
			"Super Jelly Troopers (Extra Time)",
			"Test purchase of MANAGED item: Super Jelly Troopers(Extra Time).",
			JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID,
			new PurchaseWithMarket(new MarketItem(JELLY_TROOPERS_EXTRA_TIME_PRODUCT_ID, MarketItem.Consumable.NONCONSUMABLE , 0.99))
			);

				public static VirtualGood JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_PACK  = new LifetimeVG(
			"Super Jelly Troopers (Extra Speed)",
			"Test purchase of MANAGED item: Super Jelly Troopers(Extra Speed).",
			JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID,
			new PurchaseWithMarket(new MarketItem(JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_ID, MarketItem.Consumable.NONCONSUMABLE , 0.99))
			);

				public static VirtualGood JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_PACK  = new LifetimeVG(
			"Super Jelly Troopers (Infinite Lifes)",
			"Test purchase of MANAGED item: Super Jelly Troopers(Infinite Lifes).",
			JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID,
				new PurchaseWithMarket(new MarketItem(JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_ID, MarketItem.Consumable.NONCONSUMABLE , 3.99))
			);
		
		
		/// <summary>
		/// see parent.
		/// </summary>
		public int GetVersion() {
			return 0;
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCurrency[] GetCurrencies() {
			return new VirtualCurrency[]{JELLY_TROOPERS_CURRENCY};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualGood[] GetGoods() {
			return new VirtualGood[] {JELLY_TROOPERS_EXTRA_LIFES_PRODUCT_PACK,JELLY_TROOPERS_INFINITE_LIFES_PRODUCT_PACK,
			JELLY_TROOPERS_EXTRA_SPEED_PRODUCT_PACK,JELLY_TROOPERS_EXTRA_TIME_PRODUCT_PACK};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCurrencyPack[] GetCurrencyPacks() {
			return new VirtualCurrencyPack[] {FOUR_HUNDRED_ALFIE_PACK};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCategory[] GetCategories() {
			return new VirtualCategory[]{GENERAL_CATEGORY};
		}
		
		/// <summary>
		/// see parent.
		/// </summary>
		//public NonConsumableItem[] GetNonConsumableItems() {
		//	return new NonConsumableItem[]{JELLY_TROOPERS_PREMIUM_PRODUCT_PACK};
		//}
			
		
	}
	
}

