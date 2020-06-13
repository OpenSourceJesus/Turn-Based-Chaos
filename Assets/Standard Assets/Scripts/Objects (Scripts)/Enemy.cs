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
			{
				initPosition = trs.position;
				initRotation = trs.eulerAngles.z;
				initialized = true;
			}
			activeEnemies.Add(this);
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
			if (activeEnemies.Count == 0)
				Player.currentDangerArea.IsDefeated = true;
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			activeEnemies.Remove(this);
		}
	}
}