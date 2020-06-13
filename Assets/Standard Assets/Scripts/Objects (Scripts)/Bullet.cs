using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GridGame
{
	public class Bullet : Entity, ISpawnable
	{
		[HideInInspector]
		public Vector2 moveDirection;
		public int range;
		int rangeRemaining;
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		public static List<Bullet> activeBullets = new List<Bullet>();

		public override void OnEnable ()
		{
			base.OnEnable ();
			moveDirection = ((Vector2) trs.up).GetClosestPoint(GameManager.GetSingleton<GameManager>().possibleMoves);
			trs.up = moveDirection;
			rangeRemaining = range;
			activeBullets.Add(this);
		}

		public override void HandleMoving ()
		{
			Move (moveDirection);
		}

		public override bool Move (Vector2 move)
		{
			if (base.Move(move))
			{
				if (Physics2D.OverlapPoint(trs.position, whatICantMoveTo) != null)
				{
					Death ();
					return false;
				}
				else
				{
					rangeRemaining --;
					if (rangeRemaining == 0)
						Death ();
					return true;
				}
			}
			else
			{
				// Death ();
				return false;
			}
		}

		public override void Death ()
		{
			base.Death ();
			GameManager.GetSingleton<ObjectPool>().Despawn(prefabIndex, gameObject, trs);
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			activeBullets.Remove(this);
		}
	}
}