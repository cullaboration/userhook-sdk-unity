using System;

using UHSimpleJSON;


public class UserHookResponseHandler
{

	public string gameObjectName;
	public string functionName;

	public UserHookResponseHandler (string gameObjectName, string functionName)
	{
		this.gameObjectName = gameObjectName;
		this.functionName = functionName;
	}

	public JSONClass addHandlerToJson(JSONClass json) {

		json.Add(UserHook.HANDLER_GAME_OBJECT_KEY, new JSONData(gameObjectName));
		json.Add(UserHook.HANDLER_FUNCTION_KEY, new JSONData(functionName));

		return json;
	}
}


