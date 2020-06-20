using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

namespace GridGame
{
	[Serializable]
	public class TemporaryActiveText : TemporaryActiveGameObject
	{
		public TMP_Text text;
		public float durationPerCharacter;
		
		public override IEnumerator DoRoutine ()
		{
			duration = text.text.Length * durationPerCharacter;
			yield return GameManager.GetSingleton<GameManager>().StartCoroutine(base.DoRoutine ());
		}
	}
}