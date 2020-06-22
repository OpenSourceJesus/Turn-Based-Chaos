using UnityEngine;
using Extensions;
using System.Collections.Generic;

namespace GridGame
{
	public class Enemy : Entity, IResetable
	{
		public Vector2 initPosition;
		public float initRotation;
		public static List<Enemy> activeEnemies = new List<Enemy>();
		public static Enemy[] enemiesInArea = new Enemy[0];
		bool initialized;
		
		public override void OnEnable ()
		{
			base.OnEnable ();
			if (!initialized)
				Init ();
			activeEnemies.Add(this);
		}

		public virtual void Init ()
		{
			initPosition = trs.position;
			initRotation = trs.eulerAngles.z;
			initialized = true;
		}

		public virtual void Reset ()
		{
			trs.position = initPosition;
			trs.eulerAngles = Vector3.forward * initRotation;
			gameObject.SetActive(true);
		}

		public override void HandleMoving ()
		{
			Vector2 desiredMove = GameManager.GetSingleton<Player>().trs.position - trs.position;
			int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(GameManager.GetSingleton<GameManager>().possibleMoves);
			Vector2 move = GameManager.GetSingleton<GameManager>().possibleMoves[indexOfClosestPossibleMove];
			if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null)
				Move (move);
		}

		public override void Death ()
		{
			base.Death ();
			gameObject.SetActive(false);
			if (activeEnemies.Count == 0 && Player.currentDangerArea != null)
				Player.currentDangerArea.IsDefeated = true;
			AudioClip deathSound = GameManager.GetSingleton<AudioManager>().deathSounds[Random.Range(0, GameManager.GetSingleton<AudioManager>().deathSounds.Length)];
			SoundEffect soundEffect = GameManager.GetSingleton<AudioManager>().PlaySoundEffect (GameManager.GetSingleton<AudioManager>().deathSoundEffectPrefab, new SoundEffect.Settings(deathSound));
			DeathSoundEffect deathSoundEffect = soundEffect as DeathSoundEffect;
			deathSoundEffect.killer = GameManager.GetSingleton<Player>();
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			activeEnemies.Remove(this);
		}
	}
}