using UnityEngine;

namespace GridGame
{
	public class StationaryBullet : Bullet
	{
		public AttackPoint[] attackPoints = new AttackPoint[0];
		public float attackDuration;
		EventManager.Event _event;

		public override void OnEnable ()
		{
			base.OnEnable ();
			trs.rotation = Quaternion.identity;
		}

		public override bool Move (Vector2 move)
		{
			if (!moveIsReady)
				return false;
			moveIsReady = false;
			Attack ();
			return true;
		}

		public virtual void Attack ()
		{
			foreach (AttackPoint attackPoint in attackPoints)
				attackPoint.enabled = true;
			_event = new EventManager.Event(DisableAttackPoints, Time.time + attackDuration);
			EventManager.events.Add(_event);
		}

		public virtual void DisableAttackPoints ()
		{
			foreach (AttackPoint attackPoint in attackPoints)
				attackPoint.enabled = false;
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			if (_event != null)
				_event.Remove ();
		}
	}
}