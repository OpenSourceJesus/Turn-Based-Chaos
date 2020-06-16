#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TilemapEditorScript : EditorScript
{
    public Tilemap tilemap;

    public override void DoEditorUpdate ()
    {
    }
}

[CustomEditor(typeof(TilemapEditorScriptEditor))]
public class TilemapEditorScriptEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		EditorScript editorScript = (EditorScript) target;
		editorScript.UpdateHotkeys ();
	}

	public virtual void OnSceneGUI ()
	{
		EditorScript editorScript = (EditorScript) target;
		editorScript.UpdateHotkeys ();
	}
}
#endif
#if !UNITY_EDITOR
using UnityEngine;

public class TilemapScript : MonoBehaviour
{
}
#endif