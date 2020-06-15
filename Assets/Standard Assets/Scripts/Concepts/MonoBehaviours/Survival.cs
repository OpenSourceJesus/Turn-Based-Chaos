using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace GridGame
{
	public class Survival : MonoBehaviour
	{
		public SpawnEntry[] spawnEntries = new SpawnEntry[0];
		public int wave;
		public int difficulty;
		public int addToDifficulty;
		int moveInput;
		int previousMoveInput;
		List<Enemy> enemies = new List<Enemy>();
		public _Text infoText;
		public int HighestWave
		{
			get
			{
				return PlayerPrefs.GetInt("Wave", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Wave", value);
			}
		}

		IEnumerator Start ()
		{
			yield return new WaitForEndOfFrame();
			GameManager.GetSingleton<Player>().onMoved += CheckForWaveEnd;
			NextWave ();
		}

		void SpawnEnemies ()
		{
			float remainingDifficulty = difficulty;
			List<SpawnEntry> remainingSpawnEntries = new List<SpawnEntry>();
			remainingSpawnEntries.AddRange(spawnEntries);
			do
			{
				int indexOfSpawnEntry = Random.Range(0, remainingSpawnEntries.Count);
				SpawnEntry spawnEntry = remainingSpawnEntries[indexOfSpawnEntry];
				if (remainingDifficulty - spawnEntry.difficulty < 0)
					remainingSpawnEntries.RemoveAt(indexOfSpawnEntry);
				else
				{
					Enemy enemy = spawnEntry.Spawn();
					if (enemy != null)
						enemies.Add(enemy);
					remainingDifficulty -= spawnEntry.difficulty;
				}
			} while (remainingSpawnEntries.Count > 0);
		}

		IEnumerator SpawnPlayerRoutine ()
		{
#if UNITY_EDITOR
			do
			{
				moveInput = InputManager.MoveInput;
				if (moveInput > previousMoveInput)
				{
					Vector2 spawnPosition = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(InputManager.MousePosition);
					if (Physics2D.OverlapPoint(spawnPosition, GameManager.GetSingleton<GameManager>().whatIsEnemy) == null)
					{
						SpawnPlayer (spawnPosition);
						yield break;
					}
				}
				else
				{
					foreach (Touch touch in Input.touches)
					{
						if (touch.phase == UnityEngine.TouchPhase.Began)
						{
							Vector2 spawnPosition = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(touch.position);
							if (Physics2D.OverlapPoint(spawnPosition, GameManager.GetSingleton<GameManager>().whatIsEnemy) == null)
							{
								SpawnPlayer (spawnPosition);
								yield break;
							}
						}
					}
				}
				previousMoveInput = moveInput;
				yield return new WaitForEndOfFrame();
			} while (true);
#else
			do
			{
				// foreach (TouchControl touch in Touchscreen.current.touches)
				// {
				// 	if (touch.phase.ReadValue() == TouchPhase.Began)
				// 	{
				// 		Vector2 spawnPosition = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(touch.position.ToVec2());
				// 		if (Physics2D.OverlapPoint(spawnPosition, GameManager.GetSingleton<GameManager>().whatIsEnemy) == null)
				// 		{
				// 			SpawnPlayer (spawnPosition);
				// 			yield break;
				// 		}
				// 	}
				// }
				foreach (Touch touch in Input.touches)
				{
					if (touch.phase == UnityEngine.TouchPhase.Began)
					{
						Vector2 desiredMove = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(touch.position) - trs.position;
						int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(possibleMoves);
						Vector2 move = possibleMoves[indexOfClosestPossibleMove];
						if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null)
							Move (move);
					}
				}
				yield return new WaitForEndOfFrame();
			} while (true);
#endif
		}

		void SpawnPlayer (Vector2 spawnPosition)
		{
			spawnPosition = GameManager.GetSingleton<GameManager>().grid.GetCellCenterWorld(GameManager.GetSingleton<GameManager>().grid.WorldToCell(spawnPosition));
			GameManager.GetSingleton<Player>().trs.position = spawnPosition;
			GameManager.GetSingleton<Player>().gameObject.SetActive(true);
			foreach (Enemy enemy in enemies)
				enemy.enabled = true;
		}

		void CheckForWaveEnd ()
		{
			for (int i = 0; i < enemies.Count; i ++)
			{
				Enemy enemy = enemies[i];
				if (enemy.isDead)
				{
					enemies.RemoveAt(i);
					i --;
					if (enemies.Count == 0)
						NextWave ();
				}
			}
		}

		void NextWave ()
		{
			wave ++;
			if (wave > HighestWave)
				HighestWave = wave;
			infoText.text.text = "Wave: " + wave + "\nHighest Wave: " + HighestWave;
			difficulty += addToDifficulty;
			SpawnEnemies ();
			GameManager.GetSingleton<Player>().gameObject.SetActive(false);
			previousMoveInput = InputManager.MoveInput;
			StartCoroutine(SpawnPlayerRoutine ());
		}

		void OnDestroy ()
		{
			if (wave > HighestWave)
				HighestWave = wave;
		}

		[Serializable]
		public class SpawnEntry
		{
			public Enemy enemyPrefab;
			public float difficulty;

			public virtual Enemy Spawn ()
			{
				List<DangerZone> dangerZones = new List<DangerZone>();
				dangerZones.AddRange(GameManager.GetSingleton<DangerArea>().dangerZones);
				do
				{
					int indexOfDangerZone = Random.Range(0, dangerZones.Count);
					DangerZone dangerZone = dangerZones[indexOfDangerZone];
					if (Physics2D.OverlapPoint(dangerZone.trs.position, GameManager.GetSingleton<GameManager>().whatIsEnemy) != null)
						dangerZones.RemoveAt(indexOfDangerZone);
					else
					{
						Enemy enemy = GameManager.GetSingleton<ObjectPool>().SpawnComponent<Enemy>(enemyPrefab, dangerZone.trs.position);
						return enemy;
					}
				} while (dangerZones.Count > 0);
				return null;
			}
		}
	}
}