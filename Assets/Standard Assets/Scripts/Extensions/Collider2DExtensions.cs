using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridGame;

namespace Extensions
{
	public static class Collider2DExtensions
	{
		public static Rect GetRect (this Collider2D collider)
		{
			return collider.GetRect(collider.GetComponent<Transform>());
		}

		public static Rect GetRect (this Collider2D collider, Transform trs)
		{
			Rect output = new Rect();
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				output = Rect.MinMaxRect(trs.position.x + (boxCollider.offset.x * trs.lossyScale.x) - (boxCollider.size.x / 2 + boxCollider.edgeRadius) * trs.lossyScale.x, trs.position.y + (boxCollider.offset.y * trs.lossyScale.y) - (boxCollider.size.y / 2 + boxCollider.edgeRadius) * trs.lossyScale.y, trs.position.x + (boxCollider.offset.x * trs.lossyScale.x) + (boxCollider.size.x / 2 + boxCollider.edgeRadius) * trs.lossyScale.x, trs.position.y + (boxCollider.offset.y * trs.lossyScale.y) + (boxCollider.size.y / 2 + boxCollider.edgeRadius) * trs.lossyScale.y);
			else
			{
				PolygonCollider2D polygonCollider = collider as PolygonCollider2D;
				if (polygonCollider != null)
				{
					Rect localRect = new Rect();
					foreach (Vector2 point in polygonCollider.points)
					{
						if (point.x < localRect.xMin)
							localRect.xMin = point.x;
						if (point.x > localRect.xMax)
							localRect.xMax = point.x;
						if (point.y < localRect.yMin)
							localRect.yMin = point.y;
						if (point.y > localRect.yMax)
							localRect.yMax = point.y;
					}
					output = Rect.MinMaxRect(trs.position.x + polygonCollider.offset.x * trs.lossyScale.x - localRect.size.x / 2 * trs.lossyScale.x, trs.position.y + polygonCollider.offset.y * trs.lossyScale.y - localRect.size.y / 2 * trs.lossyScale.y, trs.position.x + polygonCollider.offset.x * trs.lossyScale.x + localRect.size.x / 2 * trs.lossyScale.x, trs.position.y + polygonCollider.offset.y * trs.lossyScale.y + localRect.size.y / 2 * trs.lossyScale.y);
				}
				else
					output = collider.bounds.ToRect();
			}
			return output.SetToPositiveSize();
		}

		public static Vector2 GetCenter (this Collider2D collider)
		{
			return collider.GetCenter(collider.GetComponent<Transform>());
		}

		public static Vector2 GetCenter (this Collider2D collider, Transform trs)
		{
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				return new Vector2(trs.position.x + boxCollider.offset.x * trs.lossyScale.x, trs.position.y + boxCollider.offset.y * trs.lossyScale.y);
			else
				return collider.GetRect(trs).center;
		}

		public static Vector2 GetSize (this Collider2D collider)
		{
			return collider.GetSize(collider.GetComponent<Transform>());
		}

		public static Vector2 GetSize (this Collider2D collider, Transform trs)
		{
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				return new Vector2((boxCollider.size.x + boxCollider.edgeRadius * 2) * trs.lossyScale.x, (boxCollider.size.y + boxCollider.edgeRadius * 2) * trs.lossyScale.y);
			else
				return collider.GetRect(trs).size;
		}
	}
}