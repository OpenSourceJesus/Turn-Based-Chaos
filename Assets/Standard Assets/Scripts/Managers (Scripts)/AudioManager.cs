using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GridGame
{
	public class AudioManager : MonoBehaviour, ISaveableAndLoadable
	{
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		[SaveAndLoadValue(false)]
		public float volume;
		[SaveAndLoadValue(false)]
		public bool mute;
		public SoundEffect soundEffectPrefab;
		public static SoundEffect[] soundEffects = new SoundEffect[0];

		public virtual void Awake ()
		{
			UpdateAudioListener ();
			soundEffects = new SoundEffect[0];
		}

		public virtual void UpdateAudioListener ()
		{
			if (mute)
				AudioListener.volume = 0;
			else
				AudioListener.volume = volume;
		}

		public virtual void ToggleMute ()
		{
			if (GameManager.GetSingleton<AudioManager>() != this)
			{
				GameManager.GetSingleton<AudioManager>().ToggleMute ();
				return;
			}
			mute = !mute;
			UpdateAudioListener ();
		}
		
		public virtual SoundEffect PlaySoundEffect (SoundEffect.Settings settings, Vector2 position = new Vector2())
		{
			SoundEffect output = GameManager.GetSingleton<ObjectPool>().SpawnComponent<SoundEffect>(soundEffectPrefab.prefabIndex, position);
			output.audioSource.clip = settings.clip;
			output.audioSource.volume = settings.volume;
			output.audioSource.pitch = settings.pitch;
			output.audioSource.Play();
			GameManager.GetSingleton<ObjectPool>().DelayDespawn (output.prefabIndex, output.gameObject, output.trs, settings.clip.length);
			soundEffects = soundEffects.Add(output);
			return output;
		}
	}
}