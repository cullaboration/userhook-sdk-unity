using System;
using System.Collections.Generic;

public interface UserHookNativeInterface
{


	void SetApplicationIdAndKey (string appId, string apiKey);

	void DisplayFeedback ();

	void RateThisApp ();

	void FetchHookPoints ();

	void SetFeedbackScreenTitle(string title);

	void SetFeedbackCustomFields(Dictionary<string,string> fields);

	void DisplayFeedbackPrompt(string message, string positiveTitle, string negativeTitle);

	void DisplayRatePrompt(string message, string positiveTitle, string negativeTitle);

	void FetchPageNames();

	void DisplayStaticPage(string slug, string title);

	void DisplayPrompt(string message, UserHookMessageButton button1, UserHookMessageButton button2);

	void UpdateCustomFields(Dictionary<string, string> fields, UserHookResponseHandler responseHandler);

	void UpdatePurchasedItem(string sku, double price, UserHookResponseHandler responseHandler);

	void RegisterPushToken(string sku);

	void SetPushNotificationIcon(string filename);

}

