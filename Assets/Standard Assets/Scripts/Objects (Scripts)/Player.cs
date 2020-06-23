using UnityEngine;
using Extensions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.UI;

namespace GridGame
{
	public class Player : Entity, ISaveableAndLoadable
	{
		int moveInput;
		int previousMoveInput;
		public delegate void OnMoved();
		public event OnMoved onMoved;
		Transform hpIcon;
		public Transform hpIconParent;
		public LayerMask whatIsSafeZone;
		public LayerMask whatIsDangerZone;
		public LayerMask whatIsScroll;
		public LayerMask whatIsSavePoint;
		public LayerMask whatIsBullet;
		Scroll currentlyReading;
		public static DangerArea currentDangerArea;
		bool inSafeZone;
		public int uniqueId;
		[SaveAndLoadValue(false)]
		public Vector2 SpawnPosition
		{
			get
			{
				return trs.position;
			}
			set
			{
				if (GameManager.GetSingleton<Survival>() == null)
					trs.position = value;
			}
		}
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
		Vector2[] possibleMoves = new Vector2[0];
		public RectTransform cameraCanvasRectTrs;
		public bool runOnMovedEvent = true;
		public LayerMask whatIsUnexploredTile;

		public override void OnEnable ()
		{
			base.OnEnable ();
			hp = maxHp;
			hpIcon = hpIconParent.GetChild(0);
			int hpIconCount = hpIconParent.childCount;
			for (int i = hpIconCount; i < hp; i ++)
				Instantiate(hpIcon, hpIconParent);
			moveTimer.Reset ();
			possibleMoves = GameManager.GetSingleton<GameManager>().possibleMoves.Add(Vector2.zero);
		}

		public override void DoUpdate ()
		{
			if (GameManager.paused)
				return;
			moveInput = InputManager.MoveInput;
			HandleMoving ();
			previousMoveInput = moveInput;
		}

		public override void HandleMoving ()
		{
			if (moveInput > previousMoveInput)
			{
#if UNITY_EDITOR
				if (InputManager.LeftClickInput || InputManager.RightClickInput)
				{
					for (int i = 0; i < moveInput - previousMoveInput; i ++)
					{
						Vector2 desiredMove = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(InputManager.MousePosition) - trs.position;
						int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(possibleMoves);
						Vector2 move = possibleMoves[indexOfClosestPossibleMove];
						if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null)
							Move (move);
						else
							Move (Vector2.zero);
					}
				}
#endif
				foreach (Touch touch in Input.touches)
				{
					if (touch.phase == UnityEngine.TouchPhase.Began)
					{
						Vector2 desiredMove = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(touch.position) - trs.position;
						int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(possibleMoves);
						Vector2 move = possibleMoves[indexOfClosestPossibleMove];
						if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null)
							Move (move);
						else
							Move (Vector2.zero);
					}
				}
			}
			// foreach (TouchControl touch in Touchscreen.current.touches)
			// {
			// 	if (touch.phase.ReadValue() == TouchPhase.Began)
			// 	{
			// 		Vector2 desiredMove = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(touch.position.ToVec2()) - trs.position;
			// 		int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(possibleMoves);
			// 		Vector2 move = possibleMoves[indexOfClosestPossibleMove];
			// 		if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null)
			// 			Move (move);
			// 	}
			// }
		}

		public override bool Move (Vector2 move)
		{
			moveIsReady = true;
			if (base.Move(move))
			{
				OnMove ();
				return true;
			}
			else
				OnMove ();
			return false;
		}

		public void OnMove ()
		{
			if (runOnMovedEvent && onMoved != null)
				onMoved ();
			inSafeZone = CheckForSafeZone ();
			if (inSafeZone)
			{
				CheckForSavePoint ();
			}
			else
			{
				CheckForDangerZone ();
				CheckForBullet ();
			}
			// GameManager.GetSingleton<GameCamera>().camera.Render();
			// Canvas.ForceUpdateCanvases();
			CheckForScroll ();
			// HandeSafeZoneIcons ();
			GameManager.GetSingleton<GameCamera>().camera.Render();
			Canvas.ForceUpdateCanvases();
		}

		// void HandeSafeZoneIcons ()
		// {
		// 	foreach (IconForSafeZone iconForSafeZone in IconForSafeZone.instances)
		// 	{
		// 		if (inSafeZone)
		// 			iconForSafeZone.rectTrs.localPosition = cameraCanvasRectTrs.sizeDelta.Multiply(GameManager.GetSingleton<GameCamera>().camera.WorldToViewportPoint(iconForSafeZone.trs.position)) - cameraCanvasRectTrs.sizeDelta / 2;
		// 		iconForSafeZone.image.enabled = inSafeZone;
		// 	}
		// }

		bool CheckForSafeZone ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsSafeZone);
			if (hitCollider != null)
			{
				foreach (Enemy enemy in Enemy.enemiesInArea)
				{
					if (enemy != null)
					{
						enemy.Reset ();
						enemy.enabled = false;
					}
				}
				Enemy.enemiesInArea = new Enemy[0];
				foreach (Trap trap in Trap.trapsInArea)
				{
					trap.Reset ();
					trap.enabled = false;
				}
				Trap.trapsInArea = new Trap[0];
                for (int i = 0; i < Bullet.activeBullets.Count; i ++)
                {
                    Bullet bullet = Bullet.activeBullets[i];
                    GameManager.GetSingleton<ObjectPool>().Despawn (bullet.prefabIndex, bullet.gameObject, bullet.trs);
					i --;
                }
				RedDoor.redDoorsInArea = new RedDoor[0];
                SafeArea safeArea = hitCollider.GetComponent<SafeZone>().safeArea;
				foreach (ConveyorBelt conveyorBelt in ConveyorBelt.conveyorBeltsInArea)
					conveyorBelt.enabled = false;
				foreach (ConveyorBelt conveyorBelt in safeArea.conveyorBelts)
					conveyorBelt.enabled = true;
				ConveyorBelt.conveyorBeltsInArea = safeArea.conveyorBelts;
				GameManager.GetSingleton<GameCamera>().trs.position = safeArea.cameraRect.center.SetZ(GameManager.GetSingleton<GameCamera>().trs.position.z);
				GameManager.GetSingleton<GameCamera>().viewSize = safeArea.cameraRect.size;
				GameManager.GetSingleton<GameCamera>().HandleViewSize ();
				return true;
			}
			return false;
		}

		bool CheckForDangerZone ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsDangerZone);
			if (hitCollider != null)
			{
				if (Enemy.enemiesInArea.Length == 0)
				{
					currentDangerArea = hitCollider.GetComponent<DangerZone>().dangerArea;
					if (currentDangerArea == null)
						return true;
					Enemy.enemiesInArea = currentDangerArea.enemies;
					foreach (Enemy enemy in Enemy.enemiesInArea)
						enemy.enabled = true;
					Trap.trapsInArea = currentDangerArea.traps;
					foreach (Trap trap in Trap.trapsInArea)
						trap.enabled = true;
					foreach (ConveyorBelt conveyorBelt in ConveyorBelt.conveyorBeltsInArea)
						conveyorBelt.enabled = false;
					foreach (ConveyorBelt conveyorBelt in currentDangerArea.conveyorBelts)
						conveyorBelt.enabled = true;
					ConveyorBelt.conveyorBeltsInArea = currentDangerArea.conveyorBelts;
					RedDoor.redDoorsInArea = currentDangerArea.redDoors;
					GameManager.GetSingleton<GameCamera>().trs.position = currentDangerArea.cameraRect.center.SetZ(GameManager.GetSingleton<GameCamera>().trs.position.z);
					GameManager.GetSingleton<GameCamera>().viewSize = currentDangerArea.cameraRect.size;
					GameManager.GetSingleton<GameCamera>().HandleViewSize ();
				}
				
				return true;
			}
			return false;
		}

		bool CheckForScroll ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsScroll);
			if (hitCollider != null)
			{
				currentlyReading = hitCollider.GetComponent<Scroll>();
				currentlyReading.displayText.text.text = currentlyReading.text;
				currentlyReading.displayText.text.enabled = true;
				return true;
			}
			else if (currentlyReading != null)
				currentlyReading.displayText.text.enabled = false;
			return false;
		}

		bool CheckForSavePoint ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsSavePoint);
			if (hitCollider != null)
			{
				FullHeal ();
				SpawnPosition = trs.position;
				GameManager.GetSingleton<SaveAndLoadManager>().Save ();
				return true;
			}
			return false;
		}

		bool CheckForBullet ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsBullet);
			if (hitCollider != null)
			{
				Bullet bullet = hitCollider.GetComponent<Bullet>();
				TakeDamage (bullet.damage);
				bullet.Death ();
				return true;
			}
			return false;
		}

		public override void TakeDamage (float amount)
		{
			if (isDead)
				return;
			if ((int) (hp - amount) < (int) hp)
			// for (int i = (int) (hp - amount); i < (int) hp; i ++)
				Destroy(hpIconParent.GetChild(0).gameObject);
			base.TakeDamage (amount);
		}

		public void FullHeal ()
		{
			if (hp <= 0)
			{
				Death ();
				return;
			}
			Transform hpIconTrs = hpIconParent.GetChild(0);
			for (int i = (int) hp; i < maxHp; i ++)
				Instantiate(hpIconTrs, hpIconParent);
			hp = maxHp;
		}

		public override void Death ()
		{
			base.Death ();
			GameManager.paused = true;
			AudioClip deathSound = GameManager.GetSingleton<AudioManager>().deathSounds[Random.Range(0, GameManager.GetSingleton<AudioManager>().deathSounds.Length)];
			SoundEffect soundEffect = GameManager.GetSingleton<AudioManager>().PlaySoundEffect (GameManager.GetSingleton<AudioManager>().deathSoundEffectPrefab, new SoundEffect.Settings(deathSound));
			DeathSoundEffect deathSoundEffect = soundEffect as DeathSoundEffect;
			deathSoundEffect.killer = GameManager.GetSingleton<Enemy>();
			// for (int i = 0; i < EventManager.events.Count; i ++)
			// 	EventManager.events[i].time = Mathf.Infinity;
			EventManager.events.Clear();
			Enemy.enemiesInArea = new Enemy[0];
			Trap.trapsInArea = new Trap[0];
			SaveAndLoadManager.lastUniqueId = SaveAndLoadManager.INIT_LAST_UNIQUE_ID;
			GameManager.GetSingleton<GameOverScreen>().Open ();
		}
	}
}