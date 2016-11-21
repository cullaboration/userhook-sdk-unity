using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPageHandler : IUserHookStaticPageHandler
{
	public CustomPageHandler ()
	{
	}

	public void handlePages (List<UserHookPage> pages)
	{
		// setup a button for the static page
		// this demo is only setup to handle 1 page in the response, 
		// but the server will return all the pages that are defined for your app

		GameObject gameObject = GameObject.Find("Static Page Button");

		foreach (UserHookPage page in pages) {
			if (gameObject != null) {

				Button button = gameObject.GetComponent<Button>();
				button.onClick.AddListener(delegate {
					UserHook.Instance.DisplayStaticPage(page.slug, page.name);
				});

				Text text = gameObject.GetComponentInChildren<Text>();
				text.text = page.name;
			}

		}
			
	}
}


