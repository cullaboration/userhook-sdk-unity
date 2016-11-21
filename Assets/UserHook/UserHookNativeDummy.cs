using System;
using System.Collections.Generic;
using UnityEngine;

public class UserHookNativeDummy : UserHookNativeInterface
{
	public UserHookNativeDummy() {
		
	}

	public void SetApplicationIdAndKey (string appId, string apiKey) {}

	public void DisplayFeedback () {}

	public void RateThisApp () {}

	public void FetchHookPoints () {}

	public void SetFeedbackScreenTitle(string title) {}

	public void SetFeedbackCustomFields(Dictionary<string,string> fields) {}

	public void DisplayFeedbackPrompt(string message, string positiveTitle, string negativeTitle) {}

	public void DisplayRatePrompt(string message, string positiveTitle, string negativeTitle) {}

	public void FetchPageNames() {}

	public void DisplayStaticPage(string slug, string title) {}

	public void DisplayPrompt(string message, UserHookMessageButton button1, UserHookMessageButton button2) {}

	public void UpdateCustomFields(Dictionary<string, string> fields, UserHookResponseHandler responseHandler) {}

	public void UpdatePurchasedItem(string sku, double price, UserHookResponseHandler responseHandler) {}

	public void RegisterPushToken(string sku) {}

	public void SetPushNotificationIcon(string filename) {}
}


