using UnityEngine;

namespace GridGame
{
	public class Scorp : Enemy
	{
		public AttackPoint[] attackPoints = new AttackPoint[0];
		public float attackDuration;

		public override bool Move (Vector2 move)
		{
			if (base.Move(move))
			{
				foreach (AttackPoint attackPoint in attackPoints)
				{
					attackPoint.enabled = true;
					EventManager.events.Add(new EventManager.Event(delegate { attackPoint.enabled = false; }, Time.time + attackDuration));
				}
				return true;
			}
			return false;
		} 
	}
}