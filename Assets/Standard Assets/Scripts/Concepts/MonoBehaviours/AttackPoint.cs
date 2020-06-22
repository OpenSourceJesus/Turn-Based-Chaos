using UnityEngine;
using System.Collections.Generic;

namespace GridGame
{
	public class AttackPoint : Spawnable
	{
		public LayerMask whatIDamage;
		public float damage;
		public SpriteRenderer spriteRenderer;
		public AttackPointGroup[] attackPointGroups = new AttackPointGroup[0];

		public virtual void OnEnable ()
		{
			spriteRenderer.enabled = true;
			GameManager.GetSingleton<Player>().onMoved += Attack;
			Attack ();
		}

		public virtual void Attack ()
		{
			Collider2D[] hitColliders = Physics2D.OverlapPointAll(trs.position, whatIDamage);
			if (hitColliders.Length > 0)
			{
				foreach (Collider2D hitCollider in hitColliders)
					hitCollider.GetComponentInParent<IDestructable>().TakeDamage (damage);
				enabled = false;
				foreach (AttackPointGroup attackPointGroup in attackPointGroups)
				{
					if (attackPointGroup.enabled)
					{
						attackPointGroup.enabled = false;
						foreach (LineRenderer lineRenderer in attackPointGroup.lineRenderers)
							lineRenderer.enabled = true;
						foreach (AttackPoint attackPoint in attackPointGroup.attackPoints)
						{
							if (attackPointGroup.destroy)
								Destroy(attackPoint.gameObject);
							else
								attackPoint.enabled = false;
						}
					}
					else
					{
						foreach (LineRenderer lineRenderer in attackPointGroup.lineRenderers)
							lineRenderer.enabled = false;
					}
				}
			}
		}

		public virtual void OnDisable ()
		{
			spriteRenderer.enabled = false;
			GameManager.GetSingleton<Player>().onMoved -= Attack;
		}
	}
}