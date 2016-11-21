using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UHSimpleJSON;

public class UserHook : MonoBehaviour
{


	protected static UserHook instance;

	// used to bridge between Unity and native SDK
	protected static UserHookNativeInterface nativeInstance;


	protected IUserHookPayloadHandler payloadHandler;

	protected IUserHookStaticPageHandler staticPageHandler;

	protected IUserHookNewFeedbackHandler newFeedbackHandler;


	public static string HANDLER_GAME_OBJECT_KEY = "onClickGameObject";
	public static string HANDLER_FUNCTION_KEY = "onClickFunction";


	// flag used to see if User Hook should check for an iOS push token
	protected bool shouldCheckForPushToken = false;
	protected bool pushTokenSent = false;

	public static UserHook Instance {
		get {
			if (!instance) {

				UserHook[] instances = Object.FindObjectsOfType (typeof(UserHook)) as UserHook[];
				if (instances != null && instances.Length > 0) {
					instance = instances [0];
				} else {
					Debug.Log ("User Hook component is missing");
				}
			}

			return instance;
		}
	}


	protected void Init ()
	{
		
		Debug.Log ("init user hook");

		UserHookConfig config = UserHookConfig.Instance;

		Debug.Log ("User Hook: app = " + config.appId + ", key = " + config.appKey);

		#if UNITY_EDITOR
		nativeInstance = new UserHookNativeDummy ();
		#elif UNITY_IOS
		nativeInstance = new UserHookiOS ();
		#elif UNITY_ANDROID
		nativeInstance = new UserHookAndroid();
		#else
		nativeInstance = new UserHookNativeDummy();
		#endif

		nativeInstance.SetApplicationIdAndKey (config.appId, config.appKey);
	}

	void Awake ()
	{
		this.gameObject.name = "UserHook";

		if (!instance) {
			instance = this;
			instance.Init ();
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (this.gameObject);
		}
	}

	public void Update ()
	{

		if (shouldCheckForPushToken) {
			this.getPushToken ();
		}
	}

	public void setPayloadHandler (IUserHookPayloadHandler handler)
	{
		payloadHandler = handler;
	}

	public void setNewFeedbackHandler (IUserHookNewFeedbackHandler handler)
	{
		this.newFeedbackHandler = handler;
	}

	public void displayFeedback ()
	{
		nativeInstance.DisplayFeedback ();
	}

	public void showNewFeedback ()
	{
		if (this.newFeedbackHandler != null) {
			this.newFeedbackHandler.handleResponse ();
		}
	}

	public void rateThisApp ()
	{
		nativeInstance.RateThisApp ();
	}

	public void fetchHookPoints ()
	{
		nativeInstance.FetchHookPoints ();
	}


	public void handlePayload (string payloadString)
	{
		// convert payload to dictionary
		JSONNode json = JSON.Parse (payloadString);
		Dictionary<string,string> payload = jsonToDictionary (json.AsObject);

		if (payloadHandler != null) {
			payloadHandler.handlePayload (payload);
		}
	}

	public void setFeedbackScreenTitle (string title)
	{
		nativeInstance.SetFeedbackScreenTitle (title);
	}

	public void setFeedbackCustomFields (Dictionary<string,string> fields)
	{
		nativeInstance.SetFeedbackCustomFields (fields);
	}

	public void DisplayFeedbackPrompt (string message, string positiveTitle, string negativeTitle)
	{
		nativeInstance.DisplayFeedbackPrompt (message, positiveTitle, negativeTitle);
	}

	public void DisplayRatePrompt (string message, string positiveTitle, string negativeTitle)
	{
		nativeInstance.DisplayRatePrompt (message, positiveTitle, negativeTitle);
	}

	public void DisplayPrompt (string message, UserHookMessageButton button1, UserHookMessageButton button2)
	{
		nativeInstance.DisplayPrompt (message, button1, button2);
	}

	public void handleFetchedPageNames (string messageString)
	{

		JSONNode json = JSON.Parse (messageString);

		List<UserHookPage> pages = new List<UserHookPage> ();

		foreach (JSONNode node in json.AsArray) {

			JSONClass pageObject = node.AsObject;
			Dictionary<string,string> dict = jsonToDictionary (pageObject);

			UserHookPage page = new UserHookPage (dict ["name"], dict ["slug"]);
			pages.Add (page);
			
		}

		if (staticPageHandler != null) {
			staticPageHandler.handlePages (pages);
		}

	}

	public void handleResponse (string messageString)
	{

		Debug.Log ("handling response: " + messageString);

		JSONNode json = JSON.Parse (messageString);

		if (json.AsObject [UserHook.HANDLER_GAME_OBJECT_KEY] != null && json.AsObject [UserHook.HANDLER_FUNCTION_KEY] != null) {

			string gameObjectName = json.AsObject [UserHook.HANDLER_GAME_OBJECT_KEY].Value;
			string functionName = json.AsObject [UserHook.HANDLER_FUNCTION_KEY].Value;

			GameObject targetObject = GameObject.Find (gameObjectName);
			if (targetObject != null) {
				targetObject.SendMessage (functionName);
			}

		}

	}

	public void updateCustomFields (Dictionary<string,string> fields, UserHookResponseHandler responseHandler)
	{
		nativeInstance.UpdateCustomFields (fields, responseHandler);
	}

	public void updatePurchasedItems (string sku, double price, UserHookResponseHandler responseHandler)
	{
		nativeInstance.UpdatePurchasedItem (sku, price, responseHandler);
	}


	public void fetchPageNames (IUserHookStaticPageHandler callback)
	{
		this.staticPageHandler = callback;
		nativeInstance.FetchPageNames ();
	}

	public void DisplayStaticPage (string slug, string title)
	{
		nativeInstance.DisplayStaticPage (slug, title);
	}


	public Dictionary<string,string> jsonToDictionary (JSONClass json)
	{

		Dictionary<string,string> dict = new Dictionary<string,string> ();

		if (json != null) {
			foreach (KeyValuePair<string, JSONNode> N in json) {
				dict.Add (N.Key, N.Value.Value);
			}
		}
		return dict;
	}

	public void registerForPushNotifications ()
	{

		#if UNITY_IOS

		UnityEngine.iOS.NotificationServices.RegisterForNotifications (
			UnityEngine.iOS.NotificationType.Alert |
			UnityEngine.iOS.NotificationType.Badge |
			UnityEngine.iOS.NotificationType.Sound, true);


		shouldCheckForPushToken = true;

		#endif

	}

	public void getPushToken ()
	{

		#if UNITY_IOS

		byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
		if (token != null) {
			string hexToken = System.BitConverter.ToString (token).Replace ("-", "");

			if (!pushTokenSent) {
				Debug.Log ("push token = " + hexToken);
				nativeInstance.RegisterPushToken (hexToken);
			}
			pushTokenSent = true;
			shouldCheckForPushToken = false;
		}

		#endif

	}

	public void setPushNotificationIcon (string filename)
	{

		#if UNITY_ANDROID
		nativeInstance.SetPushNotificationIcon (filename);
		#endif
	}
}
