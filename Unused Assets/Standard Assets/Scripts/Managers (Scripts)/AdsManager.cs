using UnityEngine;
using System.Collections;
using UnityEngine.Monetization;
using GridGame;

public class AdsManager : SingletonMonoBehaviour<AdsManager>
{
	public static bool UseAds
	{
		get
		{
			return PlayerPrefs.GetInt("Use ads", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("Use ads", value.GetHashCode());
		}
	}
	public float timeWithoutAds;
	string gameId = "XXXXXXX";
	string placementId = "video";
	Coroutine showAddRoutine;
	bool testMode;

	public override void Awake ()
	{
		base.Awake ();
#if UNITY_IOS
		gameId = "3659648";
#elif UNITY_ANDROID
		gameId = "3659649";
#endif
#if UNITY_EDITOR
		testMode = true;
#endif
		Monetization.Initialize(gameId, testMode);
	}

	public void ShowAd ()
	{
		if (!UseAds)
		{
			if (AccountManager.CurrentlyPlaying.PlayTime > timeWithoutAds)
				UseAds = true;
			GameManager.GetSingleton<GameManager>().PauseGame (false);
			GameManager.initialized = true;
			return;
		}
		if (GameManager.GetSingleton<AdsManager>() != this)
		{
			GameManager.GetSingleton<AdsManager>().ShowAd ();
			return;
		}
		if (showAddRoutine == null)
			showAddRoutine = StartCoroutine(ShowAdRoutine ());
	}

	public IEnumerator ShowAdRoutine ()
	{
		GameManager.GetSingleton<GameManager>().PauseGame (true);
		yield return new WaitUntil(() => (Monetization.IsReady(placementId)));
		ShowAdPlacementContent ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
		if (ad != null)
		{
			ad.Show();
			yield return new WaitUntil(() => (!ad.showing));
		}
		GameManager.GetSingleton<GameManager>().PauseGame (false);
		GameManager.initialized = true;
		showAddRoutine = null;
	}
}