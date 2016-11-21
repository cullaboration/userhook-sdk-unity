using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomFieldScene : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
		updateScoreLabel ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected void updateScoreLabel ()
	{

		int score = PlayerPrefs.GetInt ("score");
		GameObject scoreObject = GameObject.Find ("Score");
		scoreObject.GetComponent<Text> ().text = score + "";
	}


	public void clickedIncrementScore ()
	{

		int score = PlayerPrefs.GetInt ("score");
	
		// increment score
		score = score + 1;

		PlayerPrefs.SetInt ("score", score);

		updateScoreLabel ();

		Dictionary<string, string> fields = new Dictionary<string,string> ();
		fields.Add ("score", score + "");

		// send updated info to the User Hook servers
		UserHook.Instance.updateCustomFields (fields, new UserHookResponseHandler (this.gameObject.name, "afterUpdateFields"));
	}

	public void clickedBack ()
	{
		
		SceneManager.LoadScene ("DemoScene");
	}


	public void afterUpdateFields ()
	{
		Debug.Log ("after updating custom fields");
		UserHook.Instance.fetchHookPoints ();
	}
}
