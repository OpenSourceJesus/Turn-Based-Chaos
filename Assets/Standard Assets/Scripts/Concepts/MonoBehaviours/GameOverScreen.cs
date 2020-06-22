using UnityEngine;
using System.Collections;

namespace GridGame
{
	public class GameOverScreen : SingletonMonoBehaviour<GameOverScreen>
	{
		public float showMenuOptionsDelay;
		public GameObject skipShowMenuOptionsDelayGo;
		public GameObject menuOptionsGo;
		int gameCornersTappedCount;

		public override void Awake ()
		{
			base.Awake ();
			GameManager.singletons.Remove(GetType());
			GameManager.singletons.Add(GetType(), this);
			gameObject.SetActive(false);
		}

		public void Open ()
		{
			gameCornersTappedCount = 0;
			GameManager.GetSingleton<GameManager>().PauseGame (true);
			gameObject.SetActive(true);
			StartCoroutine(ShowMenuOptionsRoutine ());
		}

		IEnumerator ShowMenuOptionsRoutine ()
		{
			yield return new WaitForSecondsRealtime(showMenuOptionsDelay);
			menuOptionsGo.SetActive(true);
			skipShowMenuOptionsDelayGo.SetActive(false);
		}

		public void OnGameCornerTapped ()
		{
			gameCornersTappedCount ++;
			if (gameCornersTappedCount >= 2)
			{
				StopCoroutine(ShowMenuOptionsRoutine ());
				menuOptionsGo.SetActive(true);
				skipShowMenuOptionsDelayGo.SetActive(false);
			}
		}

		public void Close ()
		{
			gameObject.SetActive(false);
			GameManager.GetSingleton<GameManager>().PauseGame (false);
		}
	}
}