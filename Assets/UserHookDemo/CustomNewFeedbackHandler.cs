using System;

public class CustomNewFeedbackHandler : IUserHookNewFeedbackHandler
{
	public CustomNewFeedbackHandler () {}

	public void handleResponse ()
	{
		UserHook.Instance.DisplayFeedbackPrompt ("You have a new response to your recently submitted feedback. Do you want to read it now?", "Yes", "No");
	}
}


