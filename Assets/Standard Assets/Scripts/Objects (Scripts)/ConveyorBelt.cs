using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GridGame
{
	public class ConveyorBelt : MonoBehaviour, ITurnTaker
	{
		public Transform trs;
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
		public LayerMask whatIMove;
		public static ConveyorBelt[] conveyorBeltsInArea = new ConveyorBelt[0];

		public virtual void OnEnable ()
		{
			turnCooldown = -1 + turnReloadRate;
			TakeTurn ();
			GameManager.GetSingleton<Player>().onMoved += TakeTurn;
		}

		public virtual void TakeTurn ()
		{
			turnCooldown -= turnReloadRate;
			int turnCount = Mathf.CeilToInt(turnCooldown);
			for (int turnNumber = 0; turnNumber > turnCount; turnNumber --)
			{
				HandleMoving ();
				turnCooldown ++;
			}
		}

		public virtual void HandleMoving ()
		{
			Collider2D hitCollider = Physics2D.OverlapPoint(trs.position, whatIMove);
			if (hitCollider != null)
			{
				Entity hitEntity = hitCollider.GetComponent<Entity>();
				Vector2 desiredMove = trs.up;
				int indexOfClosestPossibleMove = desiredMove.GetIndexOfClosestPoint(GameManager.GetSingleton<GameManager>().possibleMoves);
				Vector2 move = GameManager.GetSingleton<GameManager>().possibleMoves[indexOfClosestPossibleMove];
				Player player = hitEntity as Player;
				if (player != null)
				{
					player.runOnMovedEvent = false;
					player.Move (move);
					player.runOnMovedEvent = true;
				}
				else
					hitEntity.Move (move);
			}
		}

		void OnDisable ()
		{
			GameManager.GetSingleton<Player>().onMoved -= TakeTurn;
		}
	}
}