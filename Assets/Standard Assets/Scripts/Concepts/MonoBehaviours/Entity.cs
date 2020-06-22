using UnityEngine;
using Extensions;

namespace GridGame
{
	public class Entity : MonoBehaviour, IUpdatable, IDestructable
	{
		public Transform trs;
		public Timer moveTimer;
		[HideInInspector]
		public bool moveIsReady;
		public LayerMask whatICantMoveTo;
		public LayerMask whatIDamage;
		public float damage;
		public uint maxHp;
		[HideInInspector]
		public float hp;
		[HideInInspector]
		public bool isDead;
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public uint MaxHp
		{
			get
			{
				return maxHp;
			}
			set
			{
				maxHp = value;
			}
		}
		public float Hp
		{
			get
			{
				return hp;
			}
			set
			{
				hp = value;
			}
		}
		public SoundEffect deathResponse;

		public virtual void OnEnable ()
		{
			moveTimer.onFinished += OnMoveReady;
			moveTimer.timeRemaining = 0;
			moveTimer.Start ();
			isDead = false;
			hp = maxHp;
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void OnMoveReady (params object[] args)
		{
			moveIsReady = true;
		}

		public virtual void DoUpdate ()
		{
			if (GameManager.paused || this == null)
				return;
			HandleMoving ();
		}

		public virtual void HandleMoving ()
		{
		}

		public virtual bool Move (Vector2 move)
		{
			if (!moveIsReady)
				return false;
			moveIsReady = false;
			Collider2D hitCollider = Physics2D.OverlapPoint((Vector2) trs.position + move, whatIDamage);
			if (hitCollider != null)
				hitCollider.GetComponentInParent<IDestructable>().TakeDamage (damage);
			else
			{
				trs.position += (Vector3) move;
				return true;
			}
			return false;
		}

		public virtual void OnDisable ()
		{
			if (this != null)
			{
				moveTimer.onFinished -= OnMoveReady;
				moveTimer.Stop ();
			}
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public virtual void TakeDamage (float amount)
		{
			hp -= amount;
			if (!isDead && hp <= 0)
				Death ();
		}

		public virtual void Death ()
		{
			isDead = true;
		}
	}
}