using UnityEngine;
using System.Collections;
using System;
using Extensions;

namespace GridGame
{
	[Serializable]
	public class TemporaryActiveGameObject
	{
		public GameObject go;
		public float duration;
		public bool realtime;
		public static TemporaryActiveGameObject[] activeInstances = new TemporaryActiveGameObject[0];

		public virtual void Do ()
		{
			GameManager.GetSingleton<GameManager>().StartCoroutine(DoRoutine ());
		}

		public virtual void Stop ()
		{
			GameManager.GetSingleton<GameManager>().StopCoroutine(DoRoutine ());
		}
		
		public virtual IEnumerator DoRoutine ()
		{
			Activate ();
			if (realtime)
				yield return new WaitForSecondsRealtime(duration);
			else
				yield return new WaitForSeconds(duration);
			Deactivate ();
		}

		public virtual void Activate ()
		{
			if (activeInstances.Contains(this))
				return;
			if (go != null)
				go.SetActive(true);
			activeInstances = activeInstances.Add(this);
		}

		public virtual void Deactivate ()
		{
			if (go != null)
				go.SetActive(false);
			activeInstances = activeInstances.Remove(this);
		}
	}
}