using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridGame
{
	[ExecuteInEditMode]
	public class AttackPointGroup : MonoBehaviour
	{
		public bool destroy;
		public AttackPoint[] attackPoints = new AttackPoint[0];
		public LineRenderer[] lineRenderers = new LineRenderer[0];
#if UNITY_EDITOR
		public bool update;
		public Material lineRenderersMaterial;
		public Color lineRenderersColor;
		public float lineRenderersWidth;
		
		public virtual void Update ()
		{
			if (!update)
				return;
			update = false;
			for (int i = 1; i < lineRenderers.Length; i ++)
			{
				LineRenderer lineRenderer = lineRenderers[i];
				if (lineRenderer != null)
					DestroyImmediate(lineRenderer.gameObject);
			}
			lineRenderers = new LineRenderer[attackPoints.Length + 1];
			Transform previousAttackPointTrs = attackPoints[0].trs;
			for (int i = 1; i < lineRenderers.Length; i ++)
			{
				LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
				lineRenderer.SetWidth(lineRenderersWidth, lineRenderersWidth);
				Transform attackPointTrs = attackPoints[i % attackPoints.Length].trs;
				lineRenderer.SetPositions(new Vector3[2] { attackPointTrs.position, previousAttackPointTrs.position });
				lineRenderer.SetColors(lineRenderersColor, lineRenderersColor);
				lineRenderer.sharedMaterial = lineRenderersMaterial;
				lineRenderer.GetComponent<Transform>().SetParent(previousAttackPointTrs);
				lineRenderer.useWorldSpace = false;
				lineRenderers[i] = lineRenderer;
				previousAttackPointTrs = attackPointTrs;
			}
		}
#endif
	}
}