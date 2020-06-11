using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridGame
{
	//[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraScript : MonoBehaviour
	{
		public Transform trs;
		public new Camera camera;
		public Vector2 viewSize;
		protected Rect normalizedScreenViewRect;
		protected float screenAspect;
		[HideInInspector]
		public Rect viewRect;
		
		public virtual void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (camera == null)
					camera = GetComponent<Camera>();
				EditorApplication.update += DoEditorUpdate;
				return;
			}
			else
				EditorApplication.update -= DoEditorUpdate;
#endif
			trs.SetParent(null);
			trs.localScale = Vector3.one;
			viewRect.size = viewSize;
			HandlePosition ();
			HandleViewSize ();
		}

		public virtual void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.singletons[typeof(CameraScript)] = this;
		}

// 		public virtual void OnDisable ()
// 		{
// #if UNITY_EDITOR
// 			if (!Application.isPlaying)
// 				return;
// #endif
// 			GameManager.singletons[typeof(CameraScript)] = Camera.allCameras[0].GetComponent<CameraScript>();
// 		}

#if UNITY_EDITOR
		public virtual void DoEditorUpdate ()
		{
			// HandleViewSize ();
		}

		public virtual void OnDestroy ()
		{
			if (Application.isPlaying)
				return;
			EditorApplication.update -= DoEditorUpdate;
		}

		public virtual void OnDisable ()
		{
			if (Application.isPlaying)
				return;
			EditorApplication.update -= DoEditorUpdate;
		}
#endif

		public virtual void DoUpdate ()
		{
			HandlePosition ();
			HandleViewSize ();
		}
		
		public virtual void HandlePosition ()
		{
			viewRect.center = trs.position;
		}
		
		public virtual void HandleViewSize ()
		{
			screenAspect = (float) Screen.width / Screen.height;
			camera.aspect = viewSize.x / viewSize.y;
			camera.orthographicSize = Mathf.Max(viewSize.x / 2 / camera.aspect, viewSize.y / 2);
			normalizedScreenViewRect = new Rect();
			normalizedScreenViewRect.size = new Vector2(camera.aspect / screenAspect, Mathf.Min(1, screenAspect / camera.aspect));
			normalizedScreenViewRect.center = Vector2.one / 2;
			camera.rect = normalizedScreenViewRect;
			viewRect.size = viewSize;
		}
	}
}