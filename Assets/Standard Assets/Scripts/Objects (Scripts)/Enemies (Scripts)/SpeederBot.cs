using UnityEngine;
using Extensions;

namespace GridGame
{
	public class SpeederBot : Enemy
	{
		public float maxTurnAngle;
		public float initRotation;

		public override void Init ()
		{
			initRotation = trs.eulerAngles.z;
			base.Init ();
			if (GameManager.GetSingleton<Survival>() != null)
				trs.up = GameManager.GetSingleton<GameManager>().possibleMoves[Random.Range(0, GameManager.GetSingleton<GameManager>().possibleMoves.Length)];
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

		public override void Reset ()
		{
			trs.eulerAngles = Vector3.forward * initRotation;
			base.Reset ();
		}
	}
}