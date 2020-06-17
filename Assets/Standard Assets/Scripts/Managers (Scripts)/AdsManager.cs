using UnityEngine;
using System.Collections;
using UnityEngine.Monetization;
using GridGame;

public class AdsManager : SingletonMonoBehaviour<AdsManager>
{
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

	public void ShowAdd ()
	{
		if (GameManager.GetSingleton<AdsManager>() != this)
		{
			GameManager.GetSingleton<AdsManager>().ShowAdd ();
			return;
		}
		if (showAddRoutine == null)
			showAddRoutine = StartCoroutine(ShowAddRoutine ());
	}

	public IEnumerator ShowAddRoutine ()
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