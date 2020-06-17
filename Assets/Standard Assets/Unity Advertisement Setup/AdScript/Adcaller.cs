using UnityEngine;
using System.Collections;
using UnityEngine.Monetization;

public class AdsManager : MonoBehaviour
{
	string gameId = "XXXXXXX";
	string videoad = "video";
	void Start ()
	{
#if UNITY_IOS
		gameId = "3659648";
#elif UNITY_ANDROID
		gameId = "3659649";
#endif
		Monetization.Initialize(gameId, true);
	}

	public void Adshower ()
	{
		if (Monetization.IsReady(videoad))
		{
			ShowAdPlacementContent ad = Monetization.GetPlacementContent(videoad) as ShowAdPlacementContent;
			if (ad != null)
				ad.Show();
		}
	}
}