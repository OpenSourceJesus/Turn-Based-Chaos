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
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}

		public override void OnEnable ()
		{
			base.OnEnable ();
			moveDirection = ((Vector2) trs.up).GetClosestPoint(GameManager.GetSingleton<GameManager>().possibleMoves);
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
					return true;
			}
			else
			{
				Death ();
				return false;
			}
		}

		public override void Death ()
		{
			base.Death ();
			GameManager.GetSingleton<ObjectPool>().Despawn(prefabIndex, gameObject, trs);
		}
	}
}