using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace GridGame
{
	public class DangerArea : MonoBehaviour, ISaveableAndLoadable
	{
		public Enemy[] enemies = new Enemy[0];
		public Trap[] traps = new Trap[0];
		public ITurnTaker[] turnTakers = new ITurnTaker[0];
		public Rect cameraRect;
		public DangerZone[] dangerZones = new DangerZone[0];
		[SaveAndLoadValue(false)]
		public bool IsDefeated
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
					OnDefeated ();
			}
		}
		public SafeArea correspondingSafeArea;
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}

		public virtual void OnDefeated ()
		{
			for (int i = 0; i < dangerZones.Length; i ++)
			{
				DangerZone dangerZone = dangerZones[i];
				dangerZone.correspondingSafeZone.trs.gameObject.SetActive(true);
				dangerZone.correspondingSafeZone.trs.SetParent(null);
				Destroy(dangerZone.gameObject);
			}
			List<SafeArea> remainingSafeAreas = new List<SafeArea>();
			List<SafeArea> updatedSafeAreas = new List<SafeArea>();
			remainingSafeAreas.Add(correspondingSafeArea);
			do
			{
				SafeArea safeArea = remainingSafeAreas[0];
				Rect[] cameraRects = new Rect[safeArea.surroundingSafeAreas.Count + 1];
				for (int i = 0; i < cameraRects.Length - 1; i ++)
				{
					SafeArea surroundingSafeArea = safeArea.surroundingSafeAreas[i];
					cameraRects[i] = surroundingSafeArea.cameraRect;
					if (!updatedSafeAreas.Contains(surroundingSafeArea))
						remainingSafeAreas.Add(surroundingSafeArea);
				}
				cameraRects[cameraRects.Length - 1] = safeArea.cameraRect;
				safeArea.cameraRect = RectExtensions.Combine(cameraRects);
				updatedSafeAreas.Add(safeArea);
				remainingSafeAreas.RemoveAt(0);
			} while (remainingSafeAreas.Count > 0);
			Enemy.enemiesInArea = new Enemy[0];
			Trap.trapsInArea = new Trap[0];
			GameManager.GetSingleton<Player>().OnMove ();
		}
	}
}