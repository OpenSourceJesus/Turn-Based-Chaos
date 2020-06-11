using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Random = UnityEngine.Random;
using Extensions;

namespace GridGame
{
	public class NetworkManager : SingletonMonoBehaviour<NetworkManager>
	{
		public string websiteUri;
		public Text notificationText;
		public TemporaryActiveText notificationTextObject;
		public string serverName;
		public string serverUsername;
		public string serverPassword;
		public string databaseName;
		public const string DEBUG_INDICATOR = "﬩";
		public static WWWForm defaultDatabaseAccessForm;

		public override void Awake ()
		{
			base.Awake ();
			defaultDatabaseAccessForm = new WWWForm();
			defaultDatabaseAccessForm.AddField("serverName", serverName);
			defaultDatabaseAccessForm.AddField("serverUsername", serverUsername);
			defaultDatabaseAccessForm.AddField("serverPassword", serverPassword);
			defaultDatabaseAccessForm.AddField("databaseName", databaseName);
		}

		public virtual IEnumerator PostFormToResource (string resourceName, WWWForm form)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Post(websiteUri + "/" + resourceName + "?", form))
			{
				yield return webRequest.SendWebRequest();
				if (webRequest.isHttpError || webRequest.isNetworkError)
				{
					notificationText.text = webRequest.error;
					notificationTextObject.Do ();
					yield return new Exception(notificationText.text);
					yield break;
				}
				else
				{
					yield return webRequest.downloadHandler.text;
					yield break;
				}
				webRequest.Dispose();
			}
			notificationText.text = "Unknown error";
			notificationTextObject.Do ();
			yield return new Exception(notificationText.text);
		}
	}
}