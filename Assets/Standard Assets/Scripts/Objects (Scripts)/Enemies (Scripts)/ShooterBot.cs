using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridGame
{
	public class ShooterBot : Android, ITurnTaker
	{
		public Bullet bulletPrefab;
		public int order;
		public int Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
			}
		}
		public float turnReloadRate;
		// public IntOrReciprocal turnReloadRate;
		public float TurnReloadRate
		{
			get
			{
				return turnReloadRate;
				// return turnReloadRate.GetValue();
			}
			set
			{
				turnReloadRate = value;
				// turnReloadRate.SetClosestValue (value);
			}
		}
		public float turnCooldown;
		public float TurnCooldown
		{
			get
			{
				return turnCooldown;
			}
			set
			{
				turnCooldown = value;
			}
		}

		public override void OnEnable ()
		{
			base.OnEnable ();
			TakeTurn ();
			turnCooldown = -1 - turnCooldown;
			moveTimer.Reset ();
			GameManager.GetSingleton<Player>().onMoved += TakeTurn;
		}

		public virtual void TakeTurn ()
		{
			turnCooldown -= turnReloadRate;
			int turnCount = Mathf.CeilToInt(turnCooldown);
			for (int turnNumber = 0; turnNumber > turnCount; turnNumber --)
			{
				moveIsReady = true;
				HandleMoving ();
				turnCooldown ++;
			}
		}

		public override bool Move (Vector2 move)
		{
			if (!moveIsReady)
				return false;
			bool output = base.Move(move);
			Quaternion toPlayer = Quaternion.LookRotation(Vector3.forward, GameManager.GetSingleton<Player>().trs.position - trs.position);
			GameManager.GetSingleton<ObjectPool>().SpawnComponent<Bullet>(bulletPrefab.prefabIndex, trs.position, toPlayer);
			return output;
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			GameManager.GetSingleton<Player>().onMoved -= TakeTurn;
		}
	}
}