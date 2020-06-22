using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GridGame
{
	public class DeathSoundEffect : SoundEffect
	{
		public Entity killer;

		public override void OnDisable ()
		{
			base.OnDisable ();
			AudioClip deathResponse = GameManager.GetSingleton<AudioManager>().deathResponses[Random.Range(0, GameManager.GetSingleton<AudioManager>().deathResponses.Length)];
			if (killer.deathResponse == null || !killer.deathResponse.gameObject.activeInHierarchy)
				killer.deathResponse = GameManager.GetSingleton<AudioManager>().PlaySoundEffect (GameManager.GetSingleton<AudioManager>().deathSoundEffectPrefab, new SoundEffect.Settings(deathResponse));
		}
	}
}