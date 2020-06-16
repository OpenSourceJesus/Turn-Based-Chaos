#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Extensions;
using UnityEditor.SceneManagement;
using System;

namespace GridGame
{
	public class WorldMakerWindow : EditorWindow
	{
		public static WorldMakerWindow instance;
		public static bool piecesAreActive;
		public static bool worldIsActive;
		public const float SMALL_DISTANCE = .1f;

		[MenuItem("Window/World")]
		public static void Init ()
		{
			instance = (WorldMakerWindow) EditorWindow.GetWindow(typeof(WorldMakerWindow));
			instance.Show();
		}

		public virtual void OnGUI ()
		{
			GUIContent guiContent = new GUIContent();
			guiContent.text = "Rebuild";
			bool rebuild = EditorGUILayout.DropdownButton(guiContent, FocusType.Passive);
			if (rebuild)
				Rebuild ();
			guiContent.text = "Make Pieces";
			bool makePieces = EditorGUILayout.DropdownButton(guiContent, FocusType.Passive);
			if (makePieces)
				MakePieces ();
			guiContent.text = "Remove Pieces";
			bool removePieces = EditorGUILayout.DropdownButton(guiContent, FocusType.Passive);
			if (removePieces)
				RemovePieces ();
		}

		[MenuItem("World/Rebuild %&r")]
		public static void Rebuild ()
		{
			RemovePieces ();
			MakePieces ();
			SetWorldActive (false);
			SetPiecesActive (true);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetSceneByName("Game"));
		}

		public static void SetWorldActive (bool active)
		{
			GameManager.GetSingleton<World>().trs.gameObject.SetActive(active);
			worldIsActive = active;
		}

		public static void SetPiecesActive (bool active)
		{
			GameManager.GetSingleton<World>().piecesParent.gameObject.SetActive(active);
			piecesAreActive = active;
		}
		
		[MenuItem("World/Make pieces")]
		public static void MakePieces ()
		{
			foreach (Tilemap tilemap in GameManager.GetSingleton<GameManager>().tilemaps)
			{
				foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
				{
					Tile tile = tilemap.GetTile(position) as Tile;
					if (tile != null)
						Instantiate(tile.gameObject, tilemap.GetCellCenterWorld(position), Quaternion.identity, GameManager.GetSingleton<World>().piecesParent);
				}
			}
			GameManager.GetSingleton<GameManager>().MakeDangerAreas ();
			GameManager.GetSingleton<GameManager>().MakeSafeAreas ();
		}

		[MenuItem("World/Remove pieces")]
		public static void RemovePieces ()
		{
			for (int i = 0; i < GameManager.GetSingleton<World>().piecesParent.childCount; i ++)
			{
				DestroyImmediate(GameManager.GetSingleton<World>().piecesParent.GetChild(i).gameObject);
				i --;
			}
		}
		
		[MenuItem("World/Select World %&w")]
		public static void SelectWorld ()
		{
			Selection.activeTransform = GameManager.GetSingleton<World>().trs;
		}
	}
}
#endif