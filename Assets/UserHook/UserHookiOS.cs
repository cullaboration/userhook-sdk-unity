using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UHSimpleJSON;

public class UserHookiOS : UserHookNativeInterface {

	public UserHookiOS() {
		
	}

	[DllImport("__Internal")]
	private static extern void _uhSetApplicationIdAndKey(string appId, string apiKey);

	[DllImport("__Internal")]
	private static extern void _uhDisplayFeedback();


	[DllImport("__Internal")]
	private static extern void _uhRateThisApp();


	[DllImport("__Internal")]
	private static extern void _uhFetchHookPoints();


	[DllImport("__Internal")]
	private static extern void _uhSetFeedbackScreenTitle(string title);


	[DllImport("__Internal")]
	private static extern void _uhSetFeedbackCustomFields(string fields);

	[DllImport("__Internal")]
	private static extern void _uhDisplayFeedbackPrompt(string message, string positiveTitle, string negativeTitle);

	[DllImport("__Internal")]
	private static extern void _uhDisplayRatePrompt(string message, string positiveTitle, string negativeTitle);


	[DllImport("__Internal")]
	private static extern void _uhFetchPageNames();

	[DllImport("__Internal")]
	private static extern void _uhDisplayStaticPage(string slug, string title);


	[DllImport("__Internal")]
	private static extern void _uhDisplayPrompt(string message, string button1, string button2);


	[DllImport("__Internal")]
	private static extern void _uhUpdateCustomFields(string fields, string responseHandler);


	[DllImport("__Internal")]
	private static extern void _uhUpdatePurchasedItem(string sku, double price, string responseHandler);


	[DllImport("__Internal")]
	private static extern void _uhRegisterPushToken(string slug);

	public void SetApplicationIdAndKey (string appId, string apiKey) {
		_uhSetApplicationIdAndKey(appId, apiKey);
	}

	public void DisplayFeedback () {
		_uhDisplayFeedback();
	}

	public void RateThisApp () {
		_uhRateThisApp();
	}

	public void FetchHookPoints () {
		_uhFetchHookPoints();
	}


	public void SetFeedbackScreenTitle(string title) {
		_uhSetFeedbackScreenTitle(title);
	}

	public void SetFeedbackCustomFields(Dictionary<string,string> fields) {

		if (fields != null && fields.Count > 0) {
			JSONClass json = new JSONClass();

			foreach (string key in fields.Keys) {
				json.Add(key, new JSONData(fields[key]));
			}

			_uhSetFeedbackCustomFields(json.ToString());
		}
	}


	public void DisplayFeedbackPrompt(string message, string positiveTitle, string negativeTitle) {
		_uhDisplayFeedbackPrompt(message, positiveTitle, negativeTitle);
	}

	public void DisplayRatePrompt(string message, string positiveTitle, string negativeTitle) {
		_uhDisplayRatePrompt(message, positiveTitle, negativeTitle);
	}

	public void FetchPageNames() {
		_uhFetchPageNames();
	}

	public void DisplayStaticPage(string slug, string title) {
		_uhDisplayStaticPage(slug, title);
	}

	public void DisplayPrompt(string message, UserHookMessageButton button1, UserHookMessageButton button2) {
		
		_uhDisplayPrompt(message, button1.toJsonString(), button2.toJsonString());
	}

	public void UpdateCustomFields(Dictionary<string, string> fields, UserHookResponseHandler responseHandler) {


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

		_uhUpdateCustomFields(fieldsString, handlerString);

	}

	public void UpdatePurchasedItem(string sku, double price, UserHookResponseHandler responseHandler) {

		JSONClass handlerJson = new JSONClass();
		if (responseHandler != null) {
			handlerJson = responseHandler.addHandlerToJson(handlerJson);
		}


		string handlerString = handlerJson.ToString();

		_uhUpdatePurchasedItem(sku, price, handlerString);

	}

	public void RegisterPushToken(string sku) {
		_uhRegisterPushToken(sku);
	}

	public void SetPushNotificationIcon(string filename) {}
}
