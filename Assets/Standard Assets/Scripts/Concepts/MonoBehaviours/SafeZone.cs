using UnityEngine;

namespace GridGame
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SafeZone : MonoBehaviour
	{
		public Transform trs;
		public SafeArea safeArea;
		public SpriteRenderer spriteRenderer;
	}
}