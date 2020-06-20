using UnityEngine;
using System.Collections.Generic;

namespace GridGame
{
	public class SafeArea : MonoBehaviour
	{
		public Rect cameraRect;
		public List<SafeArea> surroundingSafeAreas = new List<SafeArea>();
		public Vector2Int[] unexploredCellPositions = new Vector2Int[0];
	}
}