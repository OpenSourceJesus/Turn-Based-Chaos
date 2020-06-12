using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GridGame
{
	public class Spear : Trap
	{
		public float initTimeRemaining;
		public Timer attackTimer;
		[HideInInspector]
		public bool attackIsReady;
		public AttackPoint attackPoint;
		public float attackDuration;
		EventManager.Event _event;

		public virtual void OnEnable ()
		{
			attackTimer.onFinished += OnMoveReady;
			Init ();
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void Init ()
		{
			// moveTimer.Reset ();
			attackTimer.timeRemaining = initTimeRemaining;
			attackTimer.Start ();
		}

		public virtual void OnMoveReady (params object[] args)
		{
			attackIsReady = true;
		}

		public override void DoUpdate ()
		{
			if (attackIsReady)
			{
				attackIsReady = false;
				attackPoint.enabled = true;
				_event = new EventManager.Event(delegate { attackPoint.enabled = false; }, Time.time + attackDuration);
				EventManager.events.Add(_event);
			}
		}

		public virtual void OnDisable ()
		{
			if (this == null)
			{
				attackTimer.onFinished -= OnMoveReady;
				attackTimer.Stop ();
			}
			if (_event != null)
				_event.Remove ();
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}