using UnityEngine;
using UnityEngine.UI;
using Extensions;
using System.Collections.Generic;

namespace GridGame
{
	public class IconForSafeZone : MonoBehaviour
	{
		public Transform trs;
		public RectTransform rectTrs;
		public Image image;
		// public static IconForSafeZone[] instances = new IconForSafeZone[0];
		public static List<IconForSafeZone> instances = new List<IconForSafeZone>();

		void Awake ()
		{
			rectTrs.SetParent(GameManager.GetSingleton<Player>().cameraCanvasRectTrs);
			rectTrs.localScale = Vector3.one;
		}

		void OnEnable ()
		{
			// instances = instances.Add(this);
			instances.Add(this);
		}

		void OnDisable ()
		{
			// instances = instances.Remove(this);
			instances.Remove(this);
		}
	}
}