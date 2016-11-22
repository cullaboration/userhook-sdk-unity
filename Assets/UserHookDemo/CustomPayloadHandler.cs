using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomPayloadHandler : IUserHookPayloadHandler
	{
		public CustomPayloadHandler ()
		{
		}

	public void handlePayload(Dictionary<string, string> payload) {
		
		// do custom logic on the payload
		foreach (string key in payload.Keys) {
			Debug.Log("payload entry ==> " + key +" = " + payload[key]);
		}

	}
}


