using UnityEngine;
using Extensions;

namespace GridGame
{
	public class Player : Entity
	{
		int moveInput;
		int previousMoveInput;
		public delegate void OnMoved();
		public event OnMoved onMoved;
		public Transform hpIcon;
		public Transform hpIconParent;
		public LayerMask whatIsSafeZone;
		public LayerMask whatIsDangerZone;

		public override void OnEnable ()
		{
			base.OnEnable ();
			hp = maxHp;
			for (int i = 1; i < hp; i ++)
				Instantiate(hpIcon, hpIconParent);
		}

		public override void DoUpdate ()
		{
			if (GameManager.paused || this == null)
				return;
			moveInput = InputManager.MoveInput;
			HandleMoving ();
			previousMoveInput = moveInput;
		}

		public override void HandleMoving ()
		{
			if (moveInput > previousMoveInput)
			{
				for (int i = 0; i < moveInput - previousMoveInput; i ++)
				{
					Vector2 desiredMove = GameManager.GetSingleton<GameCamera>().camera.ScreenToWorldPoint(InputManager.MousePosition) - trs.position;
					int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(GameManager.GetSingleton<GameManager>().possibleMoves);
					Vector2 move = GameManager.GetSingleton<GameManager>().possibleMoves[indexOfClosestPossibleMove];
					if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null)
						Move (move);
				}
			}
		}

		public override bool Move (Vector2 move)
		{
			if (base.Move(move))
			{
				OnMove ();
				return true;
			}
			return false;
		}

		public virtual void OnMove ()
		{
			if (onMoved != null)
				onMoved ();
			if (!CheckForSafeZone ())
				CheckForDangerZone ();
		}

		public virtual bool CheckForSafeZone ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsSafeZone);
			if (hitCollider != null)
			{
				foreach (Enemy enemy in Enemy.enemiesInArea)
				{
					enemy.Reset ();
					enemy.enabled = false;
				}
				SafeArea safeArea = hitCollider.GetComponent<SafeZone>().safeArea;
				GameManager.GetSingleton<GameCamera>().trs.position = safeArea.cameraRect.center.SetZ(GameManager.GetSingleton<GameCamera>().trs.position.z);
				GameManager.GetSingleton<GameCamera>().viewSize = safeArea.cameraRect.size;
				GameManager.GetSingleton<GameCamera>().HandleViewSize ();
				return true;
			}
			return false;
		}

		public virtual bool CheckForDangerZone ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIsDangerZone);
			if (hitCollider != null)
			{
				DangerZone dangerZone = hitCollider.GetComponent<DangerZone>();
				if (dangerZone != null)
				{
					DangerArea dangerArea = dangerZone.dangerArea;
					if (dangerArea != null)
					{
						Enemy.enemiesInArea = dangerArea.enemies;
						foreach (Enemy enemy in Enemy.enemiesInArea)
							enemy.enabled = true;
						GameManager.GetSingleton<GameCamera>().trs.position = dangerArea.cameraRect.center.SetZ(GameManager.GetSingleton<GameCamera>().trs.position.z);
						GameManager.GetSingleton<GameCamera>().viewSize = dangerArea.cameraRect.size;
						GameManager.GetSingleton<GameCamera>().HandleViewSize ();
						return true;
					}
				}
			}
			return false;
		}

		public override void TakeDamage (float amount)
		{
			if ((int) (hp - amount) < (int) hp)
				Destroy(hpIconParent.GetChild(0).gameObject);
			base.TakeDamage (amount);
		}

		public override void Death ()
		{
			base.Death ();
			GameManager.GetSingleton<GameManager>().ReloadActiveScene ();
		}
	}
}