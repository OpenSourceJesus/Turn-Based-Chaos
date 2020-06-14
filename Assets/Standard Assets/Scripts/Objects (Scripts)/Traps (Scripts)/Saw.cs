using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GridGame
{
	public class Saw : Trap, ITurnTaker
	{
		public ObjectWithWaypoints objectWithWaypoints;
		public int currentWayPoint;
		public Transform trs;
		// public AttackPoint attackPoint;
		public float attackInterval;
		EventManager.Event _event;
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
		public bool isBacktracking;
		public WrapMode wrapMode;
		bool isMoving = true;
		public float damage;
		Coroutine applyDamageRoutine;
		public float initTimeRemaining;
		public Timer moveTimer;
		bool moveIsReady;
		public bool useTimer;

		public virtual void Init ()
		{
			moveTimer.timeRemaining = initTimeRemaining;
			moveTimer.Start ();
		}

		public virtual void OnMoveReady (params object[] args)
		{
			moveIsReady = true;
		}

		public virtual void OnEnable ()
		{
			turnCooldown = 0;
			isMoving = true;
			if (!useTimer)
			{
				GameManager.GetSingleton<Player>().onMoved += TakeTurn;
				TakeTurn ();
			}
			else
			{
				moveTimer.onFinished += OnMoveReady;
				Init ();
				GameManager.updatables = GameManager.updatables.Add(this);
			}
		}

		public override void DoUpdate ()
		{
			if (moveIsReady)
			{
				moveIsReady = false;
				TakeTurn ();
			}
		}

		public virtual void TakeTurn ()
		{
			if (!isMoving)
				return;
			turnCooldown -= turnReloadRate;
			int turnCount = Mathf.CeilToInt(turnCooldown);
			for (int turnNumber = 0; turnNumber > turnCount; turnNumber --)
			{
				HandleApplyDamage ();
				HandleMoving ();
				HandleApplyDamage ();
				turnCooldown ++;
			}
		}

		public virtual void HandleMoving ()
		{
			trs.position = objectWithWaypoints.wayPoints[currentWayPoint].position;
			if (isBacktracking)
			{
				currentWayPoint --;
				if (currentWayPoint == -1)
				{
					if (wrapMode == WrapMode.Once)
						isMoving = false;
					else if (wrapMode == WrapMode.Loop)
						currentWayPoint = objectWithWaypoints.wayPoints.Length - 1;
					else
					{
						currentWayPoint += 2;
						isBacktracking = !isBacktracking;
					}
				}
			}
			else
			{
				currentWayPoint ++;
				if (currentWayPoint == objectWithWaypoints.wayPoints.Length)
				{
					if (wrapMode == WrapMode.Once)
						isMoving = false;
					else if (wrapMode == WrapMode.Loop)
						currentWayPoint = 0;
					else
					{
						currentWayPoint -= 2;
						isBacktracking = !isBacktracking;
					}
				}
			}
		}

		public virtual void HandleApplyDamage ()
		{
			if ((GameManager.GetSingleton<Player>().trs.position - trs.position).sqrMagnitude < .7f)
			{
				GameManager.GetSingleton<Player>().TakeDamage (damage);
				applyDamageRoutine = StartCoroutine(ApplyDamageRoutine ());
			}
			else
			{
				if (applyDamageRoutine != null)
					StopCoroutine(applyDamageRoutine);
			}
		}

		public virtual IEnumerator ApplyDamageRoutine ()
		{
			do
			{
				yield return new WaitForSeconds(attackInterval);
				GameManager.GetSingleton<Player>().TakeDamage (damage);
			} while (true);
		}

		public virtual void OnDisable ()
		{
			if (this == null && useTimer)
			{
				moveTimer.onFinished -= OnMoveReady;
				moveTimer.Stop ();
			}
			if (_event != null)
				_event.Remove ();
			if (useTimer)
				GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public enum WrapMode
		{
			PingPong,
			Loop,
			Once
		}
	}
}