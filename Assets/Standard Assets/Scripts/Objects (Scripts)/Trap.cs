using UnityEngine;
using Extensions;
using System.Collections.Generic;

namespace GridGame
{
	public class Trap : MonoBehaviour, IUpdatable, IResetable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}

		public virtual void DoUpdate ()
		{
		}

		public virtual void Reset ()
		{
		}
	}
}