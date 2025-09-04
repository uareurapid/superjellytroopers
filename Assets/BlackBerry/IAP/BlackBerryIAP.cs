using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// This script provides an interface to the BlackBerry In-App Purchasing Service in order to purchase digital goods through BlackBerry World.
/// </summary>
/// <remarks>
/// This script needs to be attached to a game object with the exact name "BlackBerryIAP".
/// It is suggested you create the game object from the prefab "BlackBerryIAP".
/// </remarks>
public class BlackBerryIAP : MonoBehaviour
{
    /// <summary>
    /// Sets the connection mode for offline testing.
    /// </summary>
    /// <remarks>
    /// When local connection mode is enabled:
    /// <ul>
    /// <li>You won't be charged for digital goods.</li>
    /// <li>Purchases will not be saved.</li>
    /// <li>You will not connect to the server. Therefore it will not test that the SKUs are correct.</li>
    /// </ul>
    /// Remove any calls to this method when your game is ready for public distribution.
    /// </remarks>
    /// <param name="local">True to enable local connection mode; false to disable.</param>
    public static void SetConnectionMode(bool local)
    {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
        SetConnectionModeImpl(local);
#endif
    }

    // Asynchronous Requests

    /// <summary>
    /// Purchases a digital good or subscription.
    /// </summary>
    /// <remarks>
    /// The delegate PurchaseSuccessfulEvent will be called on success and PurchaseFailedEvent will be called on failure.
    /// </remarks>
    /// <param name="digitalGoodSKU">The digital good SKU string that was entered in BlackBerry World.</param>
    /// <param name="digitalGoodName">The name of the digital good to purchase. (Optional)</param>
    /// <param name="metadata">The metadata parameter is used to identify multiple digital goods that use the same SKU. (Optional)</param>
    /// <param name="appName"> The name of the application through which the purchase is being made. (Optional)
    /// If provided, this name will be displayed in a banner along the top of the purchase confirmation screen that shall be presented to the user.</param>
    /// <param name="appIconURL">The URL to the icon that will be shown in the purchase dialog. (Optional)</param>
    /// <param name="vendorCustomerID">The value that the vendor uses to identify the customer. (Optional)</param>
    /// <param name="vendorContentID">The value that the vendor uses to identify this purchase. (Optional)</param>
    /// <returns>The request ID number. The response will contain the same request number.</returns>
    public static int Purchase(string digitalGoodSKU, string digitalGoodName = null, string metadata = null, string appName = null,
                               string appIconURL = null, string vendorCustomerID = null, string vendorContentID = null)
    {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
        int requestId = RequestPurchaseImpl(digitalGoodSKU, digitalGoodName, metadata, appName, appIconURL, vendorCustomerID, vendorContentID);
        requestDictionary[requestId] = RequestTypes.Purchase;
        return requestId;
#else
        if (PurchaseFailedEvent != null)
        {
            PurchaseFailedEvent(new ErrorEventArgs(ErrorCode.Error, editorErrorText));
        }
        return 0;
#endif
    }

    /// <summary>
    /// Gets the price of a digital good.
    /// </summary>
    /// <remarks>
    /// The delegate GetPriceSuccessfulEvent will be called on success and GetPriceFailedEvent will be called on failure.
    /// </remarks>
    /// <param name="digitalGoodSKU">The SKU of the digital good.</param>
    /// <returns>The request ID number. The response will contain the same request number.</returns>
    public static int GetPrice(string digitalGoodSKU)
    {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
        int requestId = RequestPriceImpl(digitalGoodSKU);
        requestDictionary[requestId] = RequestTypes.Price;
        requestToSkuDictionary[requestId] = digitalGoodSKU;
        return requestId;
#else
        if (GetPriceFailedEvent != null)
        {
            GetPriceFailedEvent(new ErrorEventArgs(ErrorCode.Error, editorErrorText));
        }
        return 0;
#endif
    }

    /// <summary>
    /// Gets the existing purchases.
    /// </summary>
    /// <remarks>
    /// The delegate ExistingPurchasesSuccessfulEvent will be called on success and ExistingPurchasesFailedEvent will be called on failure.
    /// </remarks>
    /// <returns>The request ID number. The response will contain the same request number.</returns>
    public static int GetExistingPurchases()
    {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
        int requestId = RequestExistingPurchasesImpl();
        requestDictionary[requestId] = RequestTypes.Existing;
        return requestId;
#else
        if (ExistingPurchasesFailedEvent != null)
        {
            ExistingPurchasesFailedEvent(new ErrorEventArgs(ErrorCode.Error, editorErrorText));
        }
        return 0;
#endif
    }

    /// <summary>
    /// Checks if the user has an active subscription for the given SKU.
    /// </summary>
    /// <remarks>
    /// The delegate IsSubscriptionActiveSuccessfulEvent will be called on success and IsSubscriptionActiveFailedEvent will be called on failure.
    /// </remarks>
    /// <param name="digitalGoodSKU">The SKU of the subscription to query.</param>
    /// <returns>The request ID number. The response will contain the same request number.</returns>
    public static int IsSubscriptionActive(string digitalGoodSKU)
    {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
        int requestId = RequestIsSubscriptionValidImpl(digitalGoodSKU);
        requestDictionary[requestId] = RequestTypes.SubscriptionActive;
        return requestId;
#else
        if (IsSubscriptionActiveFailedEvent != null)
        {
            IsSubscriptionActiveFailedEvent(new ErrorEventArgs(ErrorCode.Error, editorErrorText));
        }
        return 0;
#endif
    }

    /// <summary>
    /// Cancels a subscription with the given purchase ID.
    /// </summary>
    /// <remarks>
    /// The delegate CancelSubscriptionSuccessfulEvent will be called on success and CancelSubscriptionFailedEvent will be called on failure.
    /// </remarks>
    /// <param name="purchaseId">The purchase ID number. This is not the same as the SKU. The purchaseId is retrieved via GetExistingPurchases.</param>
    /// <returns>The request ID number. The response will contain the same request number.</returns>
    public static int CancelSubscription(string purchaseId)
    {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
        int requestId = RequestCancelSubscriptionImpl(purchaseId);
        requestDictionary[requestId] = RequestTypes.CancelSubscrition;
        return requestId;
#else
        if (CancelSubscriptionFailedEvent != null)
        {
            CancelSubscriptionFailedEvent(new ErrorEventArgs(ErrorCode.Error, editorErrorText));
        }
        return 0;
#endif
    }

    /// <summary>
    /// Fired when a purchase request is successful.
    /// </summary>
    public static event Action<PurchaseEventArgs> PurchaseSuccessfulEvent;
    /// <summary>
    /// Fired when a purchase request fails.
    /// </summary>
    public static event Action<ErrorEventArgs> PurchaseFailedEvent;
    /// <summary>
    /// Fired when a get price request is successful.
    /// </summary>
    public static event Action<PriceEventArgs> GetPriceSuccessfulEvent;
    /// <summary>
    /// Fired when a get price request fails.
    /// </summary>
    public static event Action<ErrorEventArgs> GetPriceFailedEvent;
    /// <summary>
    /// Fired when an existing purchases request is successful.
    /// </summary>
    public static event Action<ExistingPurchasesEventArgs> ExistingPurchasesSuccessfulEvent;
    /// <summary>
    /// Fired when an existing purchases request fails.
    /// </summary>
    public static event Action<ErrorEventArgs> ExistingPurchasesFailedEvent;
    /// <summary>
    /// Fired when an IsSubscriptionActive request is successful.
    /// </summary>
    public static event Action<IsSubscriptionActiveEventArgs> IsSubscriptionActiveSuccessfulEvent;
    /// <summary>
    /// Fired when an IsSubscriptionActive request fails.
    /// </summary>
    public static event Action<ErrorEventArgs> IsSubscriptionActiveFailedEvent;
    /// <summary>
    /// Fired when cancel subscription request is successful.
    /// </summary>
    public static event Action<CancelSubscriptionEventArgs> CancelSubscriptionSuccessfulEvent;
    /// <summary>
    /// Fired when a cancel subscription request fails.
    /// </summary>
    public static event Action<ErrorEventArgs> CancelSubscriptionFailedEvent;

    /// <summary>
    /// The possible error results of a request.
    /// </summary>
    public enum ErrorCode
    {
        UserCancelled = 1,
        Busy = 2,
        Error = 3,
        NotFound = 4,
        AlreadyPurchased = 5,
    }

    // Event Arguments

    /// <summary>
    /// Provides data for the PurchaseSuccessfulEvent.
    /// </summary>
    public class PurchaseEventArgs
    {
        public int RequestId;
        public string DigitalGoodSKU = "";
        public string ErrorText = "";
    }

    /// <summary>
    /// Provides data for the GetPriceSuccessfulEvent.
    /// </summary>
    public class PriceEventArgs
    {
        public int RequestId;
        public string DigitalGoodSKU = "";
        public string Price = "";
        public string RenewalPrice = "";
        public string InitialPeriod = "";
        public string ErrorText = "";
    }

    /// <summary>
    /// Provides data for the ExistingPurchasesSuccessfulEvent.
    /// </summary>
    public class ExistingPurchasesEventArgs
    {
        public int RequestId;
        public List<ExistingPurchase> ExistingPurchases = new List<ExistingPurchase>();
        public List<ExistingSubscription> ExistingSubscriptions = new List<ExistingSubscription>();
        public string ErrorText = "";
    }

    /// <summary>
    /// Provides data for the IsSubscriptionActiveSuccessfulEvent.
    /// </summary>
    public class IsSubscriptionActiveEventArgs
    {
        public int RequestId;
        public bool Active;
        public string ErrorText = "";
    }

    /// <summary>
    /// Provides data for the CancelSubscriptionSuccessfulEvent.
    /// </summary>
    public class CancelSubscriptionEventArgs
    {
        public int RequestId;
        public bool Cancelled;
        public string PurchaseId = "";
        public string ErrorText = "";
    }

    /// <summary>
    /// Provides data for the error events.
    /// </summary>
    public class ErrorEventArgs
    {
        public ErrorCode Error = ErrorCode.Error;
        public int RequestId;
        public string ErrorText = "";

        public ErrorEventArgs(ErrorCode error, string errorText)
        {
            Error = error;
            ErrorText = errorText;
        }

        public ErrorEventArgs(ErrorCode error, int requestId, string errorText)
        {
            Error = error;
            RequestId = requestId;
            ErrorText = errorText;
        }
    }

    /// <summary>
    /// Represents an existing digital good purchase.
    /// </summary>
    public class ExistingPurchase
    {
        public string PurchaseId = "";
        public string Date = "";
        public string LicenseKey = "";
        public string DigitalGoodSku = "";
        public string Metadata = "";
    }

    /// <summary>
    /// Represents an existing subscription.
    /// </summary>
    public class ExistingSubscription : ExistingPurchase
    {
        public string StartDate = "";
        public string EndDate = "";
        public string InitialPeriod = "";
        public State ItemState = State.Unknown;

        /// <summary>
        /// The possible subscription states.
        /// </summary>
        public enum State
        {
            NewSubscription = 1,
            SubscriptionRefunded = 2,
            SubscriptionCancelled = 3,
            SubscriptionRenewed = 4,
            Unknown = 5
        };
    }

    // External functions

    [DllImport("BlackBerryIAP")]
    static extern int RequestPurchaseImpl(string digitalGoodSKU, string digitalGoodName, string metadata,
                                                  string appName, string appIcon, string vendorCustomerID, string vendorContentID);

    [DllImport("BlackBerryIAP")]
    static extern int RequestPriceImpl(string digitalGoodSKU);

    [DllImport("BlackBerryIAP")]
    static extern int RequestExistingPurchasesImpl();

    [DllImport("BlackBerryIAP")]
    static extern int RequestCancelSubscriptionImpl(string purchaseId);

    [DllImport("BlackBerryIAP")]
    static extern int RequestIsSubscriptionValidImpl(string digitalGoodSKU);

    [DllImport("BlackBerryIAP")]
    static extern bool SetConnectionModeImpl(bool local);

    // Callbacks from Native

    void RequestPurchaseSucceeded(string json)
    {
        if (PurchaseSuccessfulEvent != null)
        {
            var N = JSON.Parse(json);
            var args = new PurchaseEventArgs
            {
                RequestId = N["request_id"].AsInt,
                DigitalGoodSKU = N["digital_good_sku"]
            };
            PurchaseSuccessfulEvent(args);
        }
    }

    void RequestPriceSucceeded(string json)
    {
        if (GetPriceSuccessfulEvent != null)
        {
            var N = JSON.Parse(json);
            var args = new PriceEventArgs
            {
                RequestId = N["request_id"].AsInt,
                Price = N["price"],
                RenewalPrice = N["renewal_price"],
                InitialPeriod = N["initial_period"]
            };
            string digital_good_sku;
            if (requestToSkuDictionary.TryGetValue(args.RequestId, out digital_good_sku))
            {
                args.DigitalGoodSKU = digital_good_sku;
                requestToSkuDictionary.Remove(args.RequestId);
            }
            GetPriceSuccessfulEvent(args);
        }
    }

    void RequestCancelSubscriptionSucceeded(string json)
    {
        if (CancelSubscriptionSuccessfulEvent != null)
        {
            var N = JSON.Parse(json);
            var args = new CancelSubscriptionEventArgs
            {
                RequestId = N["request_id"].AsInt,
                Cancelled = N["cancelled"].AsBool,
                PurchaseId = N["purchase_id"]
            };
            CancelSubscriptionSuccessfulEvent(args);
        }
    }

    void RequestIsSubscriptionValidSucceeded(string json)
    {
        if (IsSubscriptionActiveSuccessfulEvent != null)
        {
            var N = JSON.Parse(json);
            var args = new IsSubscriptionActiveEventArgs
            {
                RequestId = N["request_id"].AsInt,
                Active = N["valid"].AsBool
            };
            IsSubscriptionActiveSuccessfulEvent(args);
        }
    }

    void RequestExistingPurchasesSucceeded(string json)
    {
        if (ExistingPurchasesSuccessfulEvent != null)
        {
            var N = JSON.Parse(json);
            var args = new ExistingPurchasesEventArgs { RequestId = N["request_id"].AsInt };

            int purchasesCount = N["purchases"].Count;
            for (int i = 0; i < purchasesCount; ++i)
            {
                var p = N["purchases"][i];
                var purchase = new ExistingPurchase
                {
                    PurchaseId = p["purchase_id"],
                    Date = p["date"],
                    LicenseKey = p["license_key"],
                    DigitalGoodSku = p["digital_good_sku"],
                    Metadata = p["metadata"]
                };
                args.ExistingPurchases.Add(purchase);
            }
            int subscriptionCount = N["subscriptions"].Count;
            for (int i = 0; i < subscriptionCount; ++i)
            {
                var p = N["subscriptions"][i];
                var subscription = new ExistingSubscription
                {
                    PurchaseId = p["purchase_id"],
                    Date = p["date"],
                    LicenseKey = p["license_key"],
                    DigitalGoodSku = p["digital_good_sku"],
                    Metadata = p["metadata"],
                    ItemState = (ExistingSubscription.State)p["item_state"].AsInt,
                    StartDate = p["start_date"],
                    EndDate = p["end_date"],
                    InitialPeriod = p["initial_period"]
                };
                args.ExistingSubscriptions.Add(subscription);
            }
            ExistingPurchasesSuccessfulEvent(args);
        }
    }

    void GeneralError(string json)
    {
        var N = JSON.Parse(json);
        int requestId = N["request_id"].AsInt;
        int errorId = N["error_id"].AsInt;
        string errorText = N["error_text"];
        ErrorCode error = (ErrorCode)errorId;

        RequestTypes requestType;
        if (requestDictionary.TryGetValue(requestId, out requestType))
        {
            requestDictionary.Remove(requestId);
            var args = new ErrorEventArgs(error, requestId, errorText);
            switch (requestType)
            {
                case RequestTypes.Purchase:
                    if (PurchaseFailedEvent != null)
                        PurchaseFailedEvent(args);
                    break;
                case RequestTypes.Price:
                    if (GetPriceFailedEvent != null)
                    {
                        GetPriceFailedEvent(args);
                    }
                    requestToSkuDictionary.Remove(requestId);
                    break;
                case RequestTypes.Existing:
                    if (ExistingPurchasesFailedEvent != null)
                    {
                        ExistingPurchasesFailedEvent(args);
                    }
                    break;
                case RequestTypes.SubscriptionActive:
                    if (IsSubscriptionActiveFailedEvent != null)
                    {
                        IsSubscriptionActiveFailedEvent(args);
                    }
                    break;
                case RequestTypes.CancelSubscrition:
                    if (CancelSubscriptionFailedEvent != null)
                    {
                        CancelSubscriptionFailedEvent(args);
                    }
                    break;
            }
        }
    }

    enum RequestTypes
    {
        Purchase, Price, Existing, SubscriptionActive, CancelSubscrition
    }

    /// <summary>
    /// The request dictionary maps a requestId to a RequestType for the purpose of determining which delegate to call when an error is detected.
    /// </summary>
    static Dictionary<int, RequestTypes> requestDictionary = new Dictionary<int, RequestTypes>();

    static Dictionary<int, string> requestToSkuDictionary = new Dictionary<int, string>();

#if UNITY_EDITOR || !UNITY_BLACKBERRY
    /// <summary>
    /// The error text message that is returned when attempting to use the IAP Service in the editor.
    /// </summary>
    static readonly string editorErrorText = "The BlackBerry In-App Purchasing Service is only available when running on a BlackBerry device.";
#endif

/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JSONArray), objects(JSONClass) and values(JSONData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed JSON is parsed the result is undefined
 * 
 * 
 * 2012-12-17 Update:
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * * * * */
    enum JSONBinaryTag
    {
        Array = 1,
        Class = 2,
        Value = 3,
        IntValue = 4,
        DoubleValue = 5,
        BoolValue = 6,
        FloatValue = 7,
    }

    class JSONNode
    {
        #region common interface
        public virtual void Add(string aKey, JSONNode aItem) { }
        public virtual JSONNode this[int aIndex] { get { return null; } set { } }
        public virtual JSONNode this[string aKey] { get { return null; } set { } }
        public virtual string Value { get { return ""; } set { } }
        public virtual int Count { get { return 0; } }

        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        public virtual JSONNode Remove(string aKey) { return null; }
        public virtual JSONNode Remove(int aIndex) { return null; }
        public virtual JSONNode Remove(JSONNode aNode) { return aNode; }

        public virtual IEnumerable<JSONNode> Childs { get { yield break; } }
        public IEnumerable<JSONNode> DeepChilds
        {
            get
            {
                foreach (var C in Childs)
                    foreach (var D in C.DeepChilds)
                        yield return D;
            }
        }

        public override string ToString()
        {
            return "JSONNode";
        }
        public virtual string ToString(string aPrefix)
        {
            return "JSONNode";
        }

        #endregion common interface

        #region typecasting properties
        public virtual int AsInt
        {
            get
            {
                int v = 0;
                if (int.TryParse(Value, out v))
                    return v;
                return 0;
            }
            set
            {
                Value = value.ToString();
            }
        }
        public virtual float AsFloat
        {
            get
            {
                float v = 0.0f;
                if (float.TryParse(Value, out v))
                    return v;
                return 0.0f;
            }
            set
            {
                Value = value.ToString();
            }
        }
        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value, out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString();
            }
        }
        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value) ? "true" : "false";
            }
        }
        public virtual JSONArray AsArray
        {
            get
            {
                return this as JSONArray;
            }
        }
        public virtual JSONClass AsObject
        {
            get
            {
                return this as JSONClass;
            }
        }


        #endregion typecasting properties

        #region operators
        public static implicit operator JSONNode(string s)
        {
            return new JSONData(s);
        }
        public static implicit operator string(JSONNode d)
        {
            return (d == null) ? null : d.Value;
        }
        public static bool operator ==(JSONNode a, object b)
        {
            if (b == null && a is JSONLazyCreator)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return System.Object.ReferenceEquals(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        #endregion operators

        internal static string Escape(string aText)
        {
            string result = "";
            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\': result += "\\\\"; break;
                    case '\"': result += "\\\""; break;
                    case '\n': result += "\\n"; break;
                    case '\r': result += "\\r"; break;
                    case '\t': result += "\\t"; break;
                    case '\b': result += "\\b"; break;
                    case '\f': result += "\\f"; break;
                    default: result += c; break;
                }
            }
            return result;
        }

        public static JSONNode Parse(string aJSON)
        {
            Stack<JSONNode> stack = new Stack<JSONNode>();
            JSONNode ctx = null;
            int i = 0;
            string Token = "";
            string TokenName = "";
            bool QuoteMode = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        stack.Push(new JSONClass());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }

                        stack.Push(new JSONArray());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (Token != "")
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        TokenName = Token;
                        Token = "";
                        break;

                    case '"':
                        QuoteMode ^= true;
                        break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        if (Token != "")
                        {
                            if (ctx is JSONArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (QuoteMode)
                            Token += aJSON[i];
                        break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = aJSON[i];
                            switch (C)
                            {
                                case 't': Token += '\t'; break;
                                case 'r': Token += '\r'; break;
                                case 'n': Token += '\n'; break;
                                case 'b': Token += '\b'; break;
                                case 'f': Token += '\f'; break;
                                case 'u':
                                    {
                                        string s = aJSON.Substring(i + 1, 4);
                                        Token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        i += 4;
                                        break;
                                    }
                                default: Token += C; break;
                            }
                        }
                        break;

                    default:
                        Token += aJSON[i];
                        break;
                }
                ++i;
            }
            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }

        public virtual void Serialize(System.IO.BinaryWriter aWriter) { }

        public void SaveToStream(System.IO.Stream aData)
        {
            var W = new System.IO.BinaryWriter(aData);
            Serialize(W);
        }

#if USE_SharpZipLib
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
            {
                gzipOut.IsStreamOwner = false;
                SaveToStream(gzipOut);
                gzipOut.Close();
            }
        }
 
        public void SaveToCompressedFile(string aFileName)
        {
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using(var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToCompressedStream(F);
            }
        }
        public string SaveToCompressedBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToCompressedStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }
 
#else
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public string SaveToCompressedBase64()
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public void SaveToFile(string aFileName)
        {
#if UNITY_BLACKBERRY && !UNITY_EDITOR
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using (var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToStream(F);
            }
#endif
        }
        public string SaveToBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }
        public static JSONNode Deserialize(System.IO.BinaryReader aReader)
        {
            JSONBinaryTag type = (JSONBinaryTag)aReader.ReadByte();
            switch (type)
            {
                case JSONBinaryTag.Array:
                    {
                        int count = aReader.ReadInt32();
                        JSONArray tmp = new JSONArray();
                        for (int i = 0; i < count; i++)
                            tmp.Add(Deserialize(aReader));
                        return tmp;
                    }
                case JSONBinaryTag.Class:
                    {
                        int count = aReader.ReadInt32();
                        JSONClass tmp = new JSONClass();
                        for (int i = 0; i < count; i++)
                        {
                            string key = aReader.ReadString();
                            var val = Deserialize(aReader);
                            tmp.Add(key, val);
                        }
                        return tmp;
                    }
                case JSONBinaryTag.Value:
                    {
                        return new JSONData(aReader.ReadString());
                    }
                case JSONBinaryTag.IntValue:
                    {
                        return new JSONData(aReader.ReadInt32());
                    }
                case JSONBinaryTag.DoubleValue:
                    {
                        return new JSONData(aReader.ReadDouble());
                    }
                case JSONBinaryTag.BoolValue:
                    {
                        return new JSONData(aReader.ReadBoolean());
                    }
                case JSONBinaryTag.FloatValue:
                    {
                        return new JSONData(aReader.ReadSingle());
                    }

                default:
                    {
                        throw new Exception("Error deserializing JSON. Unknown tag: " + type);
                    }
            }
        }

#if USE_SharpZipLib
        public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
            return LoadFromStream(zin);
        }
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            using(var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromCompressedStream(F);
            }
        }
        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromCompressedStream(stream);
        }
#else
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public static JSONNode LoadFromStream(System.IO.Stream aData)
        {
            using (var R = new System.IO.BinaryReader(aData))
            {
                return Deserialize(R);
            }
        }
        public static JSONNode LoadFromFile(string aFileName)
        {
            using (var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromStream(F);
            }
        }
        public static JSONNode LoadFromBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromStream(stream);
        }
    } // End of JSONNode

    class JSONArray : JSONNode, IEnumerable
    {
        private List<JSONNode> m_List = new List<JSONNode>();
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return new JSONLazyCreator(this);
                return m_List[aIndex];
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }
        public override JSONNode this[string aKey]
        {
            get { return new JSONLazyCreator(this); }
            set { m_List.Add(value); }
        }
        public override int Count
        {
            get { return m_List.Count; }
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            m_List.Add(aItem);
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
            JSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }
        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (JSONNode N in m_List)
                    yield return N;
            }
        }
        public IEnumerator GetEnumerator()
        {
            foreach (JSONNode N in m_List)
                yield return N;
        }
        public override string ToString()
        {
            string result = "[ ";
            foreach (JSONNode N in m_List)
            {
                if (result.Length > 2)
                    result += ", ";
                result += N.ToString();
            }
            result += " ]";
            return result;
        }
        public override string ToString(string aPrefix)
        {
            string result = "[ ";
            foreach (JSONNode N in m_List)
            {
                if (result.Length > 3)
                    result += ", ";
                result += "\n" + aPrefix + "   ";
                result += N.ToString(aPrefix + "   ");
            }
            result += "\n" + aPrefix + "]";
            return result;
        }
        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Array);
            aWriter.Write(m_List.Count);
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].Serialize(aWriter);
            }
        }
    } // End of JSONArray

    class JSONClass : JSONNode, IEnumerable
    {
        private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                    return m_Dict[aKey];
                else
                    return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey, value);
            }
        }
        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return null;
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }
        public override int Count
        {
            get { return m_Dict.Count; }
        }


        public override void Add(string aKey, JSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = aItem;
                else
                    m_Dict.Add(aKey, aItem);
            }
            else
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
            JSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }
        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            var item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }
        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                var item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                    yield return N.Value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                yield return N;
        }
        public override string ToString()
        {
            string result = "{";
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (result.Length > 2)
                    result += ", ";
                result += "\"" + Escape(N.Key) + "\":" + N.Value.ToString();
            }
            result += "}";
            return result;
        }
        public override string ToString(string aPrefix)
        {
            string result = "{ ";
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (result.Length > 3)
                    result += ", ";
                result += "\n" + aPrefix + "   ";
                result += "\"" + Escape(N.Key) + "\" : " + N.Value.ToString(aPrefix + "   ");
            }
            result += "\n" + aPrefix + "}";
            return result;
        }
        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Class);
            aWriter.Write(m_Dict.Count);
            foreach (string K in m_Dict.Keys)
            {
                aWriter.Write(K);
                m_Dict[K].Serialize(aWriter);
            }
        }
    } // End of JSONClass

    class JSONData : JSONNode
    {
        private string m_Data;
        public override string Value
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
        public JSONData(string aData)
        {
            m_Data = aData;
        }
        public JSONData(float aData)
        {
            AsFloat = aData;
        }
        public JSONData(double aData)
        {
            AsDouble = aData;
        }
        public JSONData(bool aData)
        {
            AsBool = aData;
        }
        public JSONData(int aData)
        {
            AsInt = aData;
        }

        public override string ToString()
        {
            return "\"" + Escape(m_Data) + "\"";
        }
        public override string ToString(string aPrefix)
        {
            return "\"" + Escape(m_Data) + "\"";
        }
        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            var tmp = new JSONData("");

            tmp.AsInt = AsInt;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.IntValue);
                aWriter.Write(AsInt);
                return;
            }
            tmp.AsFloat = AsFloat;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.FloatValue);
                aWriter.Write(AsFloat);
                return;
            }
            tmp.AsDouble = AsDouble;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.DoubleValue);
                aWriter.Write(AsDouble);
                return;
            }

            tmp.AsBool = AsBool;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.BoolValue);
                aWriter.Write(AsBool);
                return;
            }
            aWriter.Write((byte)JSONBinaryTag.Value);
            aWriter.Write(m_Data);
        }
    } // End of JSONData

    class JSONLazyCreator : JSONNode
    {
        private JSONNode m_Node = null;
        private string m_Key = null;

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }
        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        private void Set(JSONNode aVal)
        {
            if (m_Key == null)
            {
                m_Node.Add(aVal);
            }
            else
            {
                m_Node.Add(m_Key, aVal);
            }
            m_Node = null; // Be GC friendly.
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                return new JSONLazyCreator(this);
            }
            set
            {
                var tmp = new JSONArray();
                tmp.Add(value);
                Set(tmp);
            }
        }

        public override JSONNode this[string aKey]
        {
            get
            {
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                var tmp = new JSONClass();
                tmp.Add(aKey, value);
                Set(tmp);
            }
        }
        public override void Add(JSONNode aItem)
        {
            var tmp = new JSONArray();
            tmp.Add(aItem);
            Set(tmp);
        }
        public override void Add(string aKey, JSONNode aItem)
        {
            var tmp = new JSONClass();
            tmp.Add(aKey, aItem);
            Set(tmp);
        }
        public static bool operator ==(JSONLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return System.Object.ReferenceEquals(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "";
        }
        public override string ToString(string aPrefix)
        {
            return "";
        }

        public override int AsInt
        {
            get
            {
                JSONData tmp = new JSONData(0);
                Set(tmp);
                return 0;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override float AsFloat
        {
            get
            {
                JSONData tmp = new JSONData(0.0f);
                Set(tmp);
                return 0.0f;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override double AsDouble
        {
            get
            {
                JSONData tmp = new JSONData(0.0);
                Set(tmp);
                return 0.0;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override bool AsBool
        {
            get
            {
                JSONData tmp = new JSONData(false);
                Set(tmp);
                return false;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }
        public override JSONArray AsArray
        {
            get
            {
                JSONArray tmp = new JSONArray();
                Set(tmp);
                return tmp;
            }
        }
        public override JSONClass AsObject
        {
            get
            {
                JSONClass tmp = new JSONClass();
                Set(tmp);
                return tmp;
            }
        }
    } // End of JSONLazyCreator

    static class JSON
    {
        public static JSONNode Parse(string aJSON)
        {
            return JSONNode.Parse(aJSON);
        }
    }
}
