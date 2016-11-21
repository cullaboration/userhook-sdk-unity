using System;
using UHSimpleJSON;


public class UserHookMessageButton
{

	public string title;

	public UserHookResponseHandler responseHandler;

	public UserHookMessageButton ()
	{
	}

	public UserHookMessageButton (string title, UserHookResponseHandler responseHandler)
	{
		this.title = title;
		this.responseHandler = responseHandler;
	}

	public string toJsonString() {

		JSONClass json = new JSONClass();
		json.Add("title", new JSONData(title));

		if (responseHandler != null) {
			json = responseHandler.addHandlerToJson(json);
		}

		return json.ToString();
	}
}


