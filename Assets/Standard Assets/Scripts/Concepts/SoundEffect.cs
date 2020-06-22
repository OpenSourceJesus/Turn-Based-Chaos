using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;

namespace GridGame
{
	[RequireComponent(typeof(AudioSource))]
	public class SoundEffect : Spawnable
	{
		public AudioSource audioSource;

		public virtual void OnDisable ()
		{
			AudioManager.soundEffects.Remove(this);
		}

		[Serializable]
		public class Settings
		{
			public AudioClip clip;
			public float volume = 1;
			public float pitch = 1;

			public Settings ()
			{
			}

			public Settings (AudioClip clip)
			{
				this.clip = clip;
			}

			public Settings (AudioClip clip, float volume, float pitch)
			{
				this.clip = clip;
				this.volume = volume;
				this.pitch = pitch;
			}
		}
	}
}