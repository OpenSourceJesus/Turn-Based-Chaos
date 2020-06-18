using UnityEngine;
using UnityEngine.SceneManagement;

namespace GridGame
{
	public class PauseMenu : SingletonMonoBehaviour<PauseMenu>
	{
		public GameObject[] sceneButtonsGos = new GameObject[0];

		public override void Awake ()
		{
			base.Awake ();
			GameManager.singletons.Remove(GetType());
			GameManager.singletons.Add(GetType(), this);
			gameObject.SetActive(false);
		}

		public void Open ()
		{
			if (GameManager.GetSingleton<PauseMenu>() != this)
			{
				GameManager.GetSingleton<PauseMenu>().Open ();
				return;
			}
			GameManager.GetSingleton<GameManager>().PauseGame (true);
			foreach (GameObject sceneButtonsGo in sceneButtonsGos)
			{
				if (sceneButtonsGo.name.Contains(SceneManager.GetActiveScene().name))
				{
					sceneButtonsGo.SetActive(false);
					break;
				}
			}
			gameObject.SetActive(true);
		}

		public void Close ()
		{
			if (GameManager.GetSingleton<PauseMenu>() != this)
			{
				GameManager.GetSingleton<PauseMenu>().Close ();
				return;
			}
			gameObject.SetActive(false);
			GameManager.GetSingleton<GameManager>().PauseGame (false);
		}
	}
}