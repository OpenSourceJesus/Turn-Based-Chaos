using UnityEngine;

namespace GridGame
{
	public class Android : Enemy
	{
		public AttackPoint[] attackPoints = new AttackPoint[0];
		public float attackDuration;
		EventManager.Event _event;

		public override bool Move (Vector2 move)
		{
			if (base.Move(move))
			{
				Attack ();
				return true;
			}
			else
			{
				Attack ();
				return false;
			}
		}

		public virtual void Attack ()
		{
			foreach (AttackPoint attackPoint in attackPoints)
			{
				attackPoint.enabled = true;
				_event = new EventManager.Event(delegate { attackPoint.enabled = false; }, Time.time + attackDuration);
				EventManager.events.Add(_event);
			}
		}

		public override void Death ()
		{
			base.Death ();
			if (_event != null)
				_event.Remove ();
		}
	}
}