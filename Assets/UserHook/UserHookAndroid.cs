using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UHSimpleJSON;

public class UserHookAndroid : UserHookNativeInterface
{

	public UserHookAndroid() {
		
	}

	public void SetApplicationIdAndKey (string appId, string apiKey)
	{
		// initialization of appId and appKey automatically occurs in UserHookUnityApplication class

	}


	protected AndroidJavaClass getUserHook() {
		return new AndroidJavaClass("com.userhook.unity.UserHookUnityProxy");
	}

	public void DisplayFeedback ()
	{
			getUserHook().CallStatic("showFeedback");
	}

	public void RateThisApp ()
	{
		getUserHook().CallStatic("rateThisApp");
	}

	public void FetchHookPoints ()
	{
		getUserHook().CallStatic("fetchHookPoints");
	}

	public void SetFeedbackScreenTitle (string title)
	{
		getUserHook().CallStatic("setFeedbackScreenTitle", title);
	}

	public void SetFeedbackCustomFields (Dictionary<string,string> fields)
	{
		if (fields != null && fields.Count > 0) {
			JSONClass json = new JSONClass();

			foreach (string key in fields.Keys) {
				json.Add(key, new JSONData(fields[key]));
			}

			getUserHook().CallStatic("setFeedbackCustomFields", json.ToString());
		}
	}

	public void DisplayFeedbackPrompt (string message, string positiveTitle, string negativeTitle)
	{
		getUserHook().CallStatic("showFeedbackPrompt", message, positiveTitle, negativeTitle);
	}

	public void DisplayRatePrompt (string message, string positiveTitle, string negativeTitle)
	{
		getUserHook().CallStatic("showRatingPrompt", message, positiveTitle, negativeTitle);
	}

	public void FetchPageNames ()
	{
		getUserHook().CallStatic("fetchPageNames");
	}

	public void DisplayStaticPage (string slug, string title)
	{
		getUserHook().CallStatic("displayStaticPage", slug, title);
	}

	public void DisplayPrompt (string message, UserHookMessageButton button1, UserHookMessageButton button2)
	{
		getUserHook().CallStatic("displayPrompt", message, button1.toJsonString(), button2.toJsonString());
	}

	public void UpdateCustomFields (Dictionary<string, string> fields, UserHookResponseHandler responseHandler)
	{
		JSONClass fieldsJson = new JSONClass();
		foreach (string key in fields.Keys) {
			fieldsJson.Add(key, new JSONData(fields[key]));
		}

		JSONClass handlerJson = new JSONClass();
		if (responseHandler != null) {
			handlerJson = responseHandler.addHandlerToJson(handlerJson);
		}

		string fieldsString = fieldsJson.ToString();
		string handlerString = handlerJson.ToString();

		getUserHook().CallStatic("updateCustomFields", fieldsString, handlerString);
	}

	public void UpdatePurchasedItem (string sku, double price, UserHookResponseHandler responseHandler)
	{
		JSONClass handlerJson = new JSONClass();
		if (responseHandler != null) {
			handlerJson = responseHandler.addHandlerToJson(handlerJson);
		}


		string handlerString = handlerJson.ToString();

		AndroidJavaClass doubleClass = new AndroidJavaClass("java.lang.Double");
		AndroidJavaObject doubleObject = doubleClass.CallStatic<AndroidJavaObject>("valueOf", price);

		getUserHook().CallStatic("updatePurchasedItem", sku, doubleObject, handlerString);

	}

	public void RegisterPushToken (string sku)
	{
	}

	public void SetPushNotificationIcon(string filename) {
		getUserHook().CallStatic("setPushNotificationIcon", filename);
	}

}

