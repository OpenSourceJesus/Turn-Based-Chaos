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
			guiContent.text = "Activate World";
			bool activateWorld = GameManager.GetSingleton<World>().trs.gameObject.activeSelf;
			activateWorld = EditorGUILayout.Toggle(guiContent.text, activateWorld);
			GameManager.GetSingleton<World>().trs.gameObject.SetActive(activateWorld);
			guiContent.text = "Activate Pieces";
			bool activatePieces = GameManager.GetSingleton<World>().piecesParent.gameObject.activeSelf;
			activatePieces = EditorGUILayout.Toggle(guiContent.text, activatePieces);
			GameManager.GetSingleton<World>().piecesParent.gameObject.SetActive(activatePieces);
		}

		[MenuItem("World/Rebuild %&r")]
		public static void Rebuild ()
		{
			SetWorldActive (true);
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
			MakeDangerAreas ();
			MakeSafeAreas ();
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

		static void MakeDangerAreas ()
		{
			GameObject[] gos = FindObjectsOfType<GameObject>();
			List<Vector2> allPositions = new List<Vector2>();
			List<Vector2> dangerZonePositions = new List<Vector2>();
			List<Vector3Int> _unexploredPositions = new List<Vector3Int>();
			Vector3Int[] unexploredPositions = new Vector3Int[0];
			List<DangerArea> dangerAreas = new List<DangerArea>();
			List<ConveyorBelt> conveyorBelts = new List<ConveyorBelt>();
			foreach (Vector3Int cellPosition in GameManager.GetSingleton<GameManager>().zonesTilemap.cellBounds.allPositionsWithin)
			{
				_unexploredPositions.Add(cellPosition);
				Vector2 position = GameManager.GetSingleton<GameManager>().zonesTilemap.GetCellCenterWorld(cellPosition);
				if (!ContainsPoint(dangerZonePositions, position, .7f) && !ContainsPoint(allPositions, position, .7f))
				{
					allPositions.Add(position);
					DangerZone dangerZone;
					if (GetComponent<DangerZone>(gos, position, out dangerZone, .7f))
					{
						DangerArea dangerArea = new GameObject().AddComponent<DangerArea>();
						dangerArea.GetComponent<Transform>().SetParent(GameManager.GetSingleton<World>().piecesParent);
						dangerArea.correspondingSafeArea = new GameObject().AddComponent<SafeArea>();
						dangerArea.correspondingSafeArea.GetComponent<Transform>().SetParent(GameManager.GetSingleton<World>().piecesParent);
						List<DangerZone> dangerZones = new List<DangerZone>();
						dangerZone.correspondingSafeZone.spriteRenderer = dangerZone.correspondingSafeZone.GetComponent<SpriteRenderer>();
						dangerZone.correspondingSafeZone.spriteRenderer.color = dangerZone.correspondingSafeZone.spriteRenderer.color.DivideAlpha(2);
						dangerZone.correspondingSafeZone.safeArea = dangerArea.correspondingSafeArea;
						dangerZone.dangerArea = dangerArea;
						dangerZones.Add(dangerZone);
						List<Vector2> dangerAreaPositions = new List<Vector2>();
						dangerAreaPositions.Add(position);
						List<Vector2> positionsRemaining = new List<Vector2>();
						List<Vector2> positionsTested = new List<Vector2>();
						positionsTested.Add(position);
						List<Enemy> enemies = new List<Enemy>();
						Enemy enemy;
						if (GetComponent<Enemy>(gos, position, out enemy, .7f))
							enemies.Add(enemy);
						List<Trap> traps = new List<Trap>();
						traps.AddRange(GetComponents<Trap>(gos, position, .7f));
						List<RedDoor> redDoors = new List<RedDoor>();
						RedDoor redDoor;
						if (GetComponent<RedDoor>(gos, position, out redDoor, .7f))
							redDoors.Add(redDoor);
						ConveyorBelt conveyorBelt;
						if (GetComponent<ConveyorBelt>(gos, position, out conveyorBelt, .7f))
							conveyorBelts.Add(conveyorBelt);
						foreach (Vector2 possibleMove in GameManager.GetSingleton<GameManager>().possibleMoves)
							positionsRemaining.Add(position + possibleMove);
						do
						{
							position = positionsRemaining[0];
							if (GetComponent<DangerZone>(gos, position, out dangerZone, .7f))
							{
								dangerZone.correspondingSafeZone.safeArea = dangerArea.correspondingSafeArea;
								dangerZone.correspondingSafeZone.spriteRenderer = dangerZone.correspondingSafeZone.GetComponent<SpriteRenderer>();
								dangerZone.correspondingSafeZone.spriteRenderer.color = dangerZone.correspondingSafeZone.spriteRenderer.color.DivideAlpha(2);
								dangerZone.dangerArea = dangerArea;
								dangerZones.Add(dangerZone);
								foreach (Vector2 possibleMove in GameManager.GetSingleton<GameManager>().possibleMoves)
								{
									Vector2 positionToTest = position + possibleMove;
									if (!ContainsPoint(positionsRemaining, positionToTest, .7f) && !ContainsPoint(positionsTested, positionToTest, .7f))
										positionsRemaining.Add(positionToTest);
								}
								dangerAreaPositions.Add(position);
								if (GetComponent<Enemy>(gos, position, out enemy, .7f))
									enemies.Add(enemy);
								traps.AddRange(GetComponents<Trap>(gos, position, .7f));
								if (GetComponent<RedDoor>(gos, position, out redDoor, .7f))
									redDoors.Add(redDoor);
								if (GetComponent<ConveyorBelt>(gos, position, out conveyorBelt, .7f))
									conveyorBelts.Add(conveyorBelt);
							}
							positionsTested.Add(position);
							allPositions.Add(position);
							positionsRemaining.RemoveAt(0);
						} while (positionsRemaining.Count > 0);
						dangerZonePositions.AddRange(dangerAreaPositions);
						dangerArea.enemies = enemies.ToArray();
						dangerArea.traps = traps.ToArray();
						dangerArea.dangerZones = dangerZones.ToArray();
						dangerArea.redDoors = redDoors.ToArray();
						dangerArea.conveyorBelts = conveyorBelts.ToArray();
						dangerArea.cameraRect = RectExtensions.FromPoints(dangerAreaPositions.ToArray()).Expand(Vector2.one * GameManager.WORLD_SCALE * 3);
						dangerArea.correspondingSafeArea.cameraRect = dangerArea.cameraRect;
						dangerArea.correspondingSafeArea.conveyorBelts = dangerArea.conveyorBelts;
						SaveAndLoadManager.lastUniqueId ++;
						dangerArea.uniqueId = SaveAndLoadManager.lastUniqueId;
						dangerArea.gameObject.AddComponent<SaveAndLoadObject>();
						dangerAreas.Add(dangerArea);
					}
				}
			}
			unexploredPositions = _unexploredPositions.ToArray();
			for (int i = 0; i < dangerAreas.Count; i ++)
				dangerAreas[i].unexploredCellPositions = unexploredPositions;
		}

		static void MakeSafeAreas ()
		{
			GameObject[] gos = FindObjectsOfType<GameObject>();
			List<Vector2> allPositions = new List<Vector2>();
			List<Vector2> safeZonePositions = new List<Vector2>();
			List<Vector3Int> _unexploredPositions = new List<Vector3Int>();
			Vector3Int[] unexploredPositions = new Vector3Int[0];
			List<SafeArea> safeAreas = new List<SafeArea>();
			foreach (Vector3Int cellPosition in GameManager.GetSingleton<GameManager>().zonesTilemap.cellBounds.allPositionsWithin)
			{
				_unexploredPositions.Add(cellPosition);
				Vector2 position = GameManager.GetSingleton<GameManager>().zonesTilemap.GetCellCenterWorld(cellPosition);
				if (!ContainsPoint(safeZonePositions, position, .7f) && !ContainsPoint(allPositions, position, .7f))
				{
					allPositions.Add(position);
					SafeZone safeZone;
					if (GetComponent<SafeZone>(gos, position, out safeZone, .7f))
					{
						SafeArea safeArea = new GameObject().AddComponent<SafeArea>();
						safeArea.GetComponent<Transform>().SetParent(GameManager.GetSingleton<World>().piecesParent);
						safeZone.safeArea = safeArea;
						List<Vector2> safeAreaPositions = new List<Vector2>();
						safeAreaPositions.Add(position);
						List<Vector2> positionsRemaining = new List<Vector2>();
						List<Vector2> positionsTested = new List<Vector2>();
						positionsTested.Add(position);
						List<DangerArea> dangerAreas = new List<DangerArea>();
						List<ConveyorBelt> conveyorBelts = new List<ConveyorBelt>();
						ConveyorBelt conveyorBelt;
						if (GetComponent<ConveyorBelt>(gos, position, out conveyorBelt, .7f))
							conveyorBelts.Add(conveyorBelt);
						foreach (Vector2 possibleMove in GameManager.GetSingleton<GameManager>().possibleMoves)
							positionsRemaining.Add(position + possibleMove);
						do
						{
							position = positionsRemaining[0];
							if (GetComponent<SafeZone>(gos, position, out safeZone, .7f))
							{
								safeZone.safeArea = safeArea;
								foreach (Vector2 possibleMove in GameManager.GetSingleton<GameManager>().possibleMoves)
								{
									Vector2 positionToTest = position + possibleMove;
									if (!ContainsPoint(positionsRemaining, positionToTest, .7f) && !ContainsPoint(positionsTested, positionToTest, .7f))
										positionsRemaining.Add(positionToTest);
								}
								safeAreaPositions.Add(position);
							}
							else
							{
								DangerZone dangerZone;
								if (GetComponent<DangerZone>(gos, position, out dangerZone, .7f))
								{
									DangerArea dangerArea = dangerZone.dangerArea;
									dangerAreas.Add(dangerArea);
									if (!safeArea.surroundingSafeAreas.Contains(dangerArea.correspondingSafeArea))
										safeArea.surroundingSafeAreas.Add(dangerArea.correspondingSafeArea);
									if (!dangerArea.correspondingSafeArea.surroundingSafeAreas.Contains(safeArea))
										dangerArea.correspondingSafeArea.surroundingSafeAreas.Add(safeArea);
								}
							}
							if (GetComponent<ConveyorBelt>(gos, position, out conveyorBelt, .7f))
								conveyorBelts.Add(conveyorBelt);
							positionsTested.Add(position);
							allPositions.Add(position);
							positionsRemaining.RemoveAt(0);
						} while (positionsRemaining.Count > 0);
						safeZonePositions.AddRange(safeAreaPositions);
						safeArea.cameraRect = RectExtensions.FromPoints(safeAreaPositions.ToArray()).Expand(Vector2.one * GameManager.WORLD_SCALE * 3);
						Rect[] dangerAreaCameraRects = new Rect[dangerAreas.Count];
						for (int i = 0; i < dangerAreas.Count; i ++)
							dangerAreaCameraRects[i] = dangerAreas[i].cameraRect;
						safeArea.cameraRect = RectExtensions.Combine(dangerAreaCameraRects.Add(safeArea.cameraRect));
						safeArea.conveyorBelts = conveyorBelts.ToArray();
						safeAreas.Add(safeArea);
					}
				}
			}
			unexploredPositions = _unexploredPositions.ToArray();
			for (int i = 0; i < safeAreas.Count; i ++)
				safeAreas[i].unexploredCellPositions = unexploredPositions;
		}

		static bool ContainsPoint (List<Vector2> points, Vector2 point, float threshold = 0)
		{
			foreach (Vector2 _point in points)
			{
				if ((_point - point).sqrMagnitude <= threshold)
					return true;
			}
			return false;
		}

		static bool GetComponent<T> (GameObject[] gos, Vector2 position, out T output, float threshold = 0)
		{
			output = default(T);
			foreach (GameObject go in gos)
			{
				if (((Vector2) go.GetComponent<Transform>().position - position).sqrMagnitude <= threshold)
				{
					output = go.GetComponent<T>();
					if (output != null)
						return true;
				}
			}
			return false;
		}

		static T[] GetComponents<T> (GameObject[] gos, Vector2 position, float threshold = 0)
		{
			List<T> output = new List<T>();
			foreach (GameObject go in gos)
			{
				if (((Vector2) go.GetComponent<Transform>().position - position).sqrMagnitude <= threshold)
				{
					T t = go.GetComponent<T>();
					if (t != null)
						output.Add(t);
				}
			}
			return output.ToArray();
		}
	}
}
#endif