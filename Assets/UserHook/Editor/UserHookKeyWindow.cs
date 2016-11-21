using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UHSimpleJSON;

public class UserHookKeyWindow : EditorWindow
{

	string appId;
	string appKey;

	public static void Init ()
	{
		UserHookKeyWindow window = (UserHookKeyWindow)EditorWindow.GetWindow (typeof(UserHookKeyWindow));


		UserHookConfig config = UserHookConfig.Instance;

		window.appId = config.appId;
		window.appKey = config.appKey;

		window.Show ();
	}

	void OnGUI ()
	{

		appId = EditorGUILayout.TextField ("App Id", appId);
		appKey = EditorGUILayout.TextField ("App Key", appKey);

	}

	void OnLostFocus ()
	{
		// update config file
		UserHookConfig.saveConfigFile(appId, appKey);

		AssetDatabase.Refresh ();
	}

}

