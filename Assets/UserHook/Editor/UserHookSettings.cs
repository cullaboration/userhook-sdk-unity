using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using UHSimpleJSON;

public class UserHookSettings
{

	[MenuItem ("User Hook/Enter App Id and App Key")]
	public static void EnterAppIdAndKey ()
	{

		UserHookKeyWindow.Init ();

	}

	[MenuItem ("User Hook/Prepare For Android Build")]
	public static void PrepareForBuild ()
	{


		if (updateManifest ()) {

			if (updateGoogleServices ()) {
				EditorUtility.DisplayDialog ("User Hook", "Complete. You may now build for the Android platform.", "Close");
			}

		}


	}


	protected static bool updateManifest ()
	{

		try {

			UserHookConfig config = UserHookConfig.Instance;

			// find and replace variables in manifest files
			string androidDir = Path.Combine (Directory.GetCurrentDirectory (), "Assets/Plugins/Android");
			if (Directory.Exists (androidDir)) {
				string[] subdirs = Directory.GetDirectories (androidDir);

				for (int i = 0; i < subdirs.Length; i++) {

					// manifest files that start with "_" have variables that need to be merged
					string manifestPath = Path.Combine (subdirs [i], "_AndroidManifest.xml");
					if (File.Exists (manifestPath)) {

						string contents = File.ReadAllText (manifestPath);
						if (contents.IndexOf ("${applicationId}") > -1) {
							contents = contents.Replace ("${applicationId}", PlayerSettings.bundleIdentifier);
						}

						if (contents.IndexOf ("${userhookAppId}") > -1) {
							contents = contents.Replace ("${userhookAppId}", config.appId);
						}

						if (contents.IndexOf ("${userhookAppKey}") > -1) {
							contents = contents.Replace ("${userhookAppKey}", config.appKey);
						}


						// write final manifest file with merged variables
						File.WriteAllText (Path.Combine (subdirs [i], "AndroidManifest.xml"), contents);

					}
				}

				AssetDatabase.Refresh ();

				return true;

			} else {
				EditorUtility.DisplayDialog ("User Hook Error", "Missing Android plugins directory at Assets/Plugins/Android", "Close");
				return false;
			}




		} catch (Exception e) {
			EditorUtility.DisplayDialog ("User Hook Error", "Error preparing for build:" + e.Message, "Close");
			return false;
		}

	}

	protected static bool updateGoogleServices ()
	{



		try {


			// write data for google-services.json
			string jsonPath = Path.Combine (Directory.GetCurrentDirectory (), "Assets/Plugins/Android/google-services.json");
			if (File.Exists (jsonPath)) {
				
				string jsonContents = File.ReadAllText (jsonPath);
				JSONNode json = JSONNode.Parse (jsonContents);
			
				JSONClass project_info = json ["project_info"].AsObject;

				string gcm_defaultSenderId = project_info ["project_number"].Value;
				string firebase_database_url = project_info ["firebase_url"].Value;
				string google_storage_bucket = project_info ["storage_bucket"].Value;

				string google_app_id = "";
				string google_api_key = "";
				string default_web_client_id = "";

				// find client entry
				foreach (JSONClass client in json["client"].AsArray) {

					JSONClass clientInfo = client ["client_info"].AsObject;
					JSONClass androidClientInfo = clientInfo ["android_client_info"].AsObject;

					string package = androidClientInfo ["package_name"].Value;

					if (package.Equals (PlayerSettings.bundleIdentifier)) {


						google_app_id = clientInfo ["mobilesdk_app_id"].Value;

						JSONArray api_key = client ["api_key"].AsArray;
						foreach (JSONClass key in api_key) {
							google_api_key = key ["current_key"].Value;
						}

						JSONArray oauth_client = client ["oauth_client"].AsArray;
						foreach (JSONClass key in oauth_client) {
							default_web_client_id = key ["client_type"].Value;
						}

						break;
					}
				}

				// create values file
				string resourcesPathDir = Path.Combine (Directory.GetCurrentDirectory (), "Assets/Plugins/Android/google-services/res/values");
				if (!Directory.Exists(resourcesPathDir)) {
					Directory.CreateDirectory(resourcesPathDir);
				}
				string resourcesPath = Path.Combine (Directory.GetCurrentDirectory (), "Assets/Plugins/Android/google-services/res/values/values.xml");


				string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
				xml += "<resources>\n";
				xml += "<string name=\"default_web_client_id\" translatable=\"false\">" + default_web_client_id + "</string>\n";
				xml += "<string name=\"gcm_defaultSenderId\"   translatable=\"false\">" + gcm_defaultSenderId + "</string>\n";
				xml += "<string name=\"firebase_database_url\" translatable=\"false\">" + firebase_database_url + "</string>\n";
				xml += "<string name=\"google_app_id\"         translatable=\"false\">" + google_app_id + "</string>\n";
				xml += "<string name=\"google_api_key\"        translatable=\"false\">" + google_api_key + "</string>\n";
				xml += "<string name=\"google_storage_bucket\" translatable=\"false\">" + google_storage_bucket + "</string>\n";
				xml += "</resources>";

				File.WriteAllText (resourcesPath, xml);


				AssetDatabase.Refresh ();

				return true;
			} else {
				EditorUtility.DisplayDialog ("User Hook Error", "Missing google-services.json in Assets/Plugins/Android directory.", "Close");
				return false;
			}

		} catch (Exception e) {
			EditorUtility.DisplayDialog ("User Hook Error", "Error preparing for build:" + e.Message, "Close");
			return false;
		}


	}
}
