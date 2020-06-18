using UnityEngine;
using Extensions;

namespace GridGame
{
	public class SpeederBot : Enemy
	{
		[HideInInspector]
		public float maxTurnAngle;

		public override void Init ()
		{
			base.Init ();
			trs.up = GameManager.GetSingleton<GameManager>().possibleMoves[Random.Range(0, GameManager.GetSingleton<GameManager>().possibleMoves.Length)];
			maxTurnAngle = Vector2.Angle(GameManager.GetSingleton<GameManager>().possibleMoves[0], GameManager.GetSingleton<GameManager>().possibleMoves[1]);
		}

		public override bool Move (Vector2 move)
		{
			if (!moveIsReady)
				return false;
			move = trs.up.RotateTo(move, maxTurnAngle) * move.magnitude;
			trs.up = move;
			if (Physics2D.OverlapPoint((Vector2) trs.position + move, whatICantMoveTo) == null && base.Move(move))
				return true;
			else
				return false;
		}
	}
}