using UnityEngine;
using Extensions;

namespace GridGame
{
	public class SpeederBot : Enemy
	{
		bool isInitialized;
		[HideInInspector]
		public float maxTurnAngle;

		public override void OnEnable ()
		{
			base.OnEnable ();
			if (isInitialized)
				return;
			trs.up = GameManager.GetSingleton<GameManager>().possibleMoves[Random.Range(0, GameManager.GetSingleton<GameManager>().possibleMoves.Length)];
			maxTurnAngle = Vector2.Angle(GameManager.GetSingleton<GameManager>().possibleMoves[0], GameManager.GetSingleton<GameManager>().possibleMoves[1]);
			isInitialized = true;
		}
		
		public override bool Move (Vector2 move)
		{
			if (!moveIsReady)
				return false;
			move = trs.up.RotateTo(move, maxTurnAngle) * move.magnitude;
			trs.up = move;
			if (base.Move(move))
				return true;
			else
				return false;
		}
	}
}