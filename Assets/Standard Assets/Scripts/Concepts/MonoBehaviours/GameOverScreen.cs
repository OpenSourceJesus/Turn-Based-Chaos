using UnityEngine;

namespace GridGame
{
	public class GameOverScreen : SingletonMonoBehaviour<GameOverScreen>
	{
		public override void Awake ()
		{
			base.Awake ();
			gameObject.SetActive(false);
		}

		public virtual void Open ()
		{
			GameManager.GetSingleton<GameManager>().PauseGame (true);
			gameObject.SetActive(true);
		}

		public virtual void Close ()
		{
			gameObject.SetActive(false);
			GameManager.GetSingleton<GameManager>().PauseGame (false);
		}
	}
}