using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// This script demonstrates how to use BlackBerryIAP.
/// </summary>
public class BlackBerryIAPTest : MonoBehaviour
{    
    const string ITEM_1_SKU = "Item1";
    const string ITEM_2_SKU = "Item2";
    const string ITEM_3_SKU = "Item3";
    
    bool localTesting = true; // Turn to false for real world testing
    bool waiting = false;
    string message = "";

    void Start()
    {
        BlackBerryIAP.SetConnectionMode(localTesting);

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
    }

    void OnGUI()
    {
        GUI.enabled = !waiting;

        if (GUI.Button(new Rect(10, 5, 300, 100), "Purchase SKU 1"))
        {
            BuySKU(ITEM_1_SKU);
        }
        if (GUI.Button(new Rect(10, 110, 300, 100), "Purchase SKU 2"))
        {
            BuySKU(ITEM_2_SKU);
        }
        if (GUI.Button(new Rect(10, 220, 300, 100), "Get Price"))
        {
            GetPrice(ITEM_1_SKU);
        }
        if (GUI.Button(new Rect(10, 330, 300, 100), "Is Subscription Active"))
        {
            waiting = true;
            BlackBerryIAP.IsSubscriptionActive(ITEM_1_SKU);
        }
        if (GUI.Button(new Rect(10, 440, 300, 100), "Cancel Subscription"))
        {
            waiting = true;
            BlackBerryIAP.CancelSubscription(ITEM_1_SKU);
        }
        if (GUI.Button(new Rect(10, 550, 300, 100), "Get Existing Purchases"))
        {
            waiting = true;
            BlackBerryIAP.GetExistingPurchases();
        }
        GUI.Label(new Rect(360, 5, Screen.width - 360, Screen.height), new GUIContent(message));
    }

    void BuySKU(string sku)
    {
        if (!waiting)
        {
            waiting = true;
            BlackBerryIAP.Purchase(sku, null, null, null, null, null, null);
        }
    }

    void GetPrice(string sku)
    {
        if (!waiting)
        {
            waiting = true;
            BlackBerryIAP.GetPrice(sku);
        }
    }

    void PurchaseSuccessful(BlackBerryIAP.PurchaseEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Successfully Purchased!\n");
        sb.AppendFormat("SKU:       {0}\n", args.DigitalGoodSKU);
        sb.AppendFormat("requestId: {0}\n", args.RequestId);
        message = sb.ToString();
        waiting = false;
    }

    void PurchaseFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Purchase Failed: {0}\n", args.Error);
        sb.AppendFormat("errorText: {0}\n", args.ErrorText);
        sb.AppendFormat("requestId: {0}\n", args.RequestId);
        sb.AppendFormat("result:    {0}\n", args.Error);
        message = sb.ToString();
        waiting = false;
    }

    void GetPriceSuccessful(BlackBerryIAP.PriceEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Price:         {0}\n", args.Price);
        sb.AppendFormat("SKU:           {0}\n", args.DigitalGoodSKU);
        sb.AppendFormat("requestId:     {0}\n", args.RequestId);
        sb.AppendFormat("renewalPrice:  {0}\n", args.RenewalPrice);
        sb.AppendFormat("initialPeriod: {0}\n", args.InitialPeriod);
        message = sb.ToString();
        waiting = false;
    }

    void GetPriceFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Get Price Failed: {0}\n", args.Error);
        sb.AppendFormat("errorText:       {0}\n", args.ErrorText);
        sb.AppendFormat("requestId:       {0}\n", args.RequestId);
        message = sb.ToString();
        waiting = false;
    }

    void ExistingPurchasesSuccessful(BlackBerryIAP.ExistingPurchasesEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Existing Purchases:     {0}\n", args.ExistingPurchases.Count);
        sb.AppendFormat("Existing Subscriptions: {0}\n", args.ExistingSubscriptions.Count);
        foreach (var item in args.ExistingPurchases)
        {
            sb.AppendFormat("Purchase:        {0}\n", item.DigitalGoodSku);
            sb.AppendFormat("    purchase_id: {0}\n", item.PurchaseId);
            sb.AppendFormat("    date:        {0}\n", item.Date);
            sb.AppendFormat("    license_key: {0}\n", item.LicenseKey);
            sb.AppendFormat("    metadata:    {0}\n", item.Metadata);
        }
        foreach (var item in args.ExistingSubscriptions)
        {
            sb.AppendFormat("Subscription:    {0}\n", item.DigitalGoodSku);
            sb.AppendFormat("    purchase_id: {0}\n", item.PurchaseId);
            sb.AppendFormat("    date:        {0}\n", item.Date);
            sb.AppendFormat("    license_key: {0}\n", item.LicenseKey);
            sb.AppendFormat("    metadata:    {0}\n", item.Metadata);
            sb.AppendFormat("    item_state:  {0}\n", item.ItemState);
            sb.AppendFormat("    start_date:  {0}\n", item.StartDate);
            sb.AppendFormat("    end_date:    {0}\n", item.EndDate);
            sb.AppendFormat("    initial_period: {0}\n", item.InitialPeriod);
        }
        message = sb.ToString();
        waiting = false;
    }

    void ExistingPurchasesFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        switch (args.Error)
        {
            case BlackBerryIAP.ErrorCode.UserCancelled:
                sb.Append("User cancelled existing purchases query.\n");
                sb.AppendFormat("errorText:  {0}\n", args.ErrorText);
                sb.AppendFormat("requestId:  {0}\n", args.RequestId);
                break;
            default:
                sb.Append("Error while getting existing purchases.\n");
                sb.AppendFormat("errorText:  {0}\n", args.ErrorText);
                sb.AppendFormat("requestId:  {0}\n", args.RequestId);
                break;
        }
        message = sb.ToString();
        waiting = false;
    }

    void CancelSubscriptionSuccessful(BlackBerryIAP.CancelSubscriptionEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Cancel Subscription Successful: \n");
        sb.AppendFormat("requestId:  {0}\n", args.RequestId);
        sb.AppendFormat("cancelled:  {0}\n", args.Cancelled);
        sb.AppendFormat("purchaseId: {0}\n", args.PurchaseId);
        message = sb.ToString();
        waiting = false;
    }

    void CancelSubscriptionFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Cancel Subscription Error: {0}\n", args.Error);
        sb.AppendFormat("errorText:  {0}\n", args.ErrorText);
        sb.AppendFormat("requestId:  {0}\n", args.RequestId);
        message = sb.ToString();
        waiting = false;
    }

    void IsSubscriptionActiveSuccessful(BlackBerryIAP.IsSubscriptionActiveEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Is Subscription Active Successful:\n");
        sb.AppendFormat("requestId: {0}\n", args.RequestId);
        sb.AppendFormat("isActive:  {0}\n", args.Active);
        message = sb.ToString();
        waiting = false;
    }

    void IsSubscriptionActiveFailed(BlackBerryIAP.ErrorEventArgs args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Is Subscription Active Failed: {0}\n", args.Error);
        sb.AppendFormat("errorText:  {0}\n", args.ErrorText);
        sb.AppendFormat("requestId:  {0}\n", args.RequestId);
        message = sb.ToString();
        waiting = false;
    }

    void OnDestroy()
    {
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
    }

}
