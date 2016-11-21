using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DemoScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		UserHook.Instance.setFeedbackScreenTitle("Feedback Title");

		Dictionary<string,string> feedbackFields = new Dictionary<string, string>();
		feedbackFields.Add("username","unity user 1");
		UserHook.Instance.setFeedbackCustomFields(feedbackFields);

		// set handler for all payloads (hook points and push notifications)
		UserHook.Instance.setPayloadHandler(new CustomPayloadHandler());

		// load static page names and handle response
		UserHook.Instance.fetchPageNames(new CustomPageHandler());

		// set handler to prompt user when new feedback response arrives
		UserHook.Instance.setNewFeedbackHandler(new CustomNewFeedbackHandler());

		// setup push notifications
		#if UNITY_IOS
		UserHook.Instance.registerForPushNotifications();
		#endif

		// set custom push notification icon for android only
		// image file must be placed in the Assets/Plugings/Android/custom-push-icon/res/drawable folder
		// only enter file name without the file extension
		#if UNITY_ANDROID
		UserHook.Instance.setPushNotificationIcon("notification");
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void clickedFeedback() {
		// displays the feedback screen
		UserHook.Instance.displayFeedback();
	}

	public void clickedRateApp() {
		// send the user directly to the app store to leave a review
		UserHook.Instance.rateThisApp();
	}


	public void clickedFetchHookPoints() {
		// fetches hook points from the server
		UserHook.Instance.fetchHookPoints();
	}

	public void clickedCustomFeedbackPrompt() {
		// prompts the user if they want to open the feedback screen
		UserHook.Instance.DisplayFeedbackPrompt("Do you want to leave us some feedback?","Yes","No");
	}

	public void clickedCustomRatePrompt() {
		// prompts the user first if they want to leave a review. Clicking "Yes" will go to the app store.
		// clicking "No" will close the prompt
		UserHook.Instance.DisplayRatePrompt("Do you want to rate our app?","Yes","No");
	}


	// used for a custom prompt button
	public void clickedPromptButton() {

		UserHookMessageButton button1 = new UserHookMessageButton("Yes", new UserHookResponseHandler(this.gameObject.name, "onClickedButton1"));
		UserHookMessageButton button2 = new UserHookMessageButton("No", new UserHookResponseHandler(this.gameObject.name, "onClickedButton2"));

		UserHook.Instance.DisplayPrompt("We are glad you downloaded the app. Are you enjoying using it?", button1, button2);

	}

	public void onClickedButton1() {
		UserHook.Instance.DisplayRatePrompt("Do you mind leaving us a rating and review in the App Store?", "Yes","Not Now");
	}

	public void onClickedButton2() {
		UserHook.Instance.DisplayFeedbackPrompt("We are sorry to hear that you aren't enjoying the app. Do you mind sending us some feedback on how to make it better?","Sure","Not Now");
	}


	public void clickedCustomField() {
		SceneManager.LoadScene("CustomFieldScene");
	}

	public void clickedInAppPurchases() {
		SceneManager.LoadScene("InAppPurchasesScene");
	}
}
