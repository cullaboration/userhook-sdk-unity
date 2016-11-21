using System;
using UHSimpleJSON;
using UnityEngine;
using System.IO;

public class UserHookConfig
{

	public string appId;
	public string appKey;

	private static UserHookConfig instance;

	public static UserHookConfig Instance {

		get {
			if (instance == null) {
				instance = new UserHookConfig ();
				instance.readConfigFile ();
			}

			return instance;
		}

	}

	protected void readConfigFile ()
	{
		string contents;
		#if UNITY_EDITOR
		string configPath = Path.Combine (Directory.GetCurrentDirectory (), "Assets/Resources/userhook.txt");

		if (File.Exists (configPath)) {

			contents = File.ReadAllText (configPath);


		} else {
			throw new Exception ("User Hook settings file is missing. Please set App Id and App Key using the User Hook editor menu.");
		}

		#else

		TextAsset textAsset = Resources.Load("userhook") as TextAsset;
		contents = textAsset.text;

		#endif

		if (contents != null && contents != "") {
			JSONNode json = JSON.Parse (contents).AsObject;

			appId = json ["appId"].Value;
			appKey = json ["appKey"].Value;

		} else {
			throw new Exception ("User Hook settings file is empty.");
		}
	}

	public static void saveConfigFile (string appId, string appKey)
	{
		
		// make sure resources directory exists
		string resourcesDir = Path.Combine (Directory.GetCurrentDirectory (), "Assets/Resources");
		if (!Directory.Exists (resourcesDir)) {
			Directory.CreateDirectory (resourcesDir);
		}

		Debug.Log ("writing appId: " + appId + " appKey: " + appKey);

		JSONClass json = new JSONClass ();
		json.Add ("appId", new JSONData (appId));
		json.Add ("appKey", new JSONData (appKey));

		// write userhook.json config file
		File.WriteAllText (Path.Combine (resourcesDir, "userhook.txt"), json.ToString ());

		// reload values
		instance.readConfigFile ();


	}
}


