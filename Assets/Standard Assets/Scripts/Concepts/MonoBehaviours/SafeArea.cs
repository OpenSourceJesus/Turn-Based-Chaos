using UnityEngine;
using System.Collections.Generic;

namespace GridGame
{
	public class SafeArea : MonoBehaviour
	{
		public Rect cameraRect;
		public List<SafeArea> surroundingSafeAreas = new List<SafeArea>();
		public Vector3Int[] unexploredCellPositions = new Vector3Int[0];
	}
}