using UnityEngine;
using Extensions;

namespace GridGame
{
	public class StationaryBullet : Bullet
	{
		public SpriteRenderer spriteRenderer;
		public AttackPoint[] attackPoints = new AttackPoint[0];
		public float attackDuration;
		EventManager.Event _event;

		public override void OnEnable ()
		{
			base.OnEnable ();
			trs.rotation = Quaternion.identity;
			spriteRenderer.enabled = true;
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
			spriteRenderer.enabled = false;
		}

		public virtual void DisableAttackPoints ()
		{
			foreach (AttackPoint attackPoint in attackPoints)
				attackPoint.enabled = false;
			GameManager.GetSingleton<ObjectPool>().Despawn (prefabIndex, gameObject, trs);
		}

		public virtual void OnDestroy ()
		{
			if (_event != null)
				_event.Remove ();
		}
	}
}