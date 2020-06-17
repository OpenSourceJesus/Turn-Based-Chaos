using UnityEngine;
using System.Collections;
using UnityEngine.Monetization;
using GridGame;

public class AdsManager : MonoBehaviour
{
	string gameId = "XXXXXXX";
	string placementId = "video";
	void Start ()
	{
#if UNITY_IOS
		gameId = "3659648";
#elif UNITY_ANDROID
		gameId = "3659649";
#endif
		Monetization.Initialize(gameId, true);
		ShowAdd ();
	}

	public void ShowAdd ()
	{
		if (GameManager.GetSingleton<AdsManager>() != this)
		{
			GameManager.GetSingleton<AdsManager>().ShowAdd ();
			return;
		}
		StartCoroutine(ShowAddRoutine ());
	}

	IEnumerator ShowAddRoutine ()
	{
		yield return new WaitUntil(() => (Monetization.IsReady(placementId)));
		ShowAdPlacementContent ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
		if (ad != null)
			ad.Show();
	}
}