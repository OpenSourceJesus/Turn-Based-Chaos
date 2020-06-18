using UnityEngine;

namespace GridGame
{
	public class PauseMenu : SingletonMonoBehaviour<PauseMenu>
	{
		public override void Awake ()
		{
			base.Awake ();
			gameObject.SetActive(false);
		}

		public void Open ()
		{
			GameManager.GetSingleton<GameManager>().PauseGame (false);
			gameObject.SetActive(true);
		}

		public void Close ()
		{
			gameObject.SetActive(false);
			GameManager.GetSingleton<GameManager>().PauseGame (false);
		}
	}
}