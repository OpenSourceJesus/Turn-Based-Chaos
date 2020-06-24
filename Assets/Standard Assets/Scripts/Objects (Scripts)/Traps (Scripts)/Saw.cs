using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridGame
{
	public class Saw : Trap, ITurnTaker
	{
		public ObjectWithWaypoints objectWithWaypoints;
		public int currentWayPoint;
		public Transform trs;
		public int order;
		public int Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
			}
		}
		public float turnReloadRate;
		// public IntOrReciprocal turnReloadRate;
		public float TurnReloadRate
		{
			get
			{
				return turnReloadRate;
				// return turnReloadRate.GetValue();
			}
			set
			{
				turnReloadRate = value;
				// turnReloadRate.SetClosestValue (value);
			}
		}
		public float turnCooldown;
		public float TurnCooldown
		{
			get
			{
				return turnCooldown;
			}
			set
			{
				turnCooldown = value;
			}
		}
		public bool isBacktracking;
		public WrapMode wrapMode;
		bool isMoving = true;
		public float damage;
		GradientColorKey[] gradientColorKeys = new GradientColorKey[0];
		GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[0];
		public Color normalWaypointColor;
		public Color nextWaypointColor;
		Gradient gradient = new Gradient();
		public Vector2 initPosition;
		public int initWayPoint;
#if UNITY_EDITOR
		[Header("Editor Helpers")]
		public float testDistanceBetweenWaypoints;
#endif

		void Awake ()
		{
			objectWithWaypoints.line.colorGradient.mode = GradientMode.Fixed;
			gradientColorKeys = new GradientColorKey[objectWithWaypoints.wayPoints.Length];
			gradientAlphaKeys = new GradientAlphaKey[objectWithWaypoints.wayPoints.Length];
			for (int i = 0; i < objectWithWaypoints.wayPoints.Length; i ++)
			{
				if (currentWayPoint == i)
				{
					gradientColorKeys[i] = new GradientColorKey(nextWaypointColor, 1f / objectWithWaypoints.wayPoints.Length * i);
					gradientAlphaKeys[i] = new GradientAlphaKey(nextWaypointColor.a, 1f / objectWithWaypoints.wayPoints.Length * i);
				}
				else
				{
					gradientColorKeys[i] = new GradientColorKey(normalWaypointColor, 1f / objectWithWaypoints.wayPoints.Length * i);
					gradientAlphaKeys[i] = new GradientAlphaKey(normalWaypointColor.a, 1f / objectWithWaypoints.wayPoints.Length * i);
				}
			}
			gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
			objectWithWaypoints.line.colorGradient = gradient;
			initPosition = trs.position;
			initWayPoint = currentWayPoint;
		}

		public virtual void OnEnable ()
		{
			isMoving = true;
			turnCooldown = -1 + turnReloadRate;
			TakeTurn ();
			TakeTurn ();
			GameManager.GetSingleton<Player>().onMoved += TakeTurn;
		}

		public virtual void TakeTurn ()
		{
			if (!isMoving)
			{
				HandleApplyDamage ();
				return;
			}
			turnCooldown -= turnReloadRate;
			int turnCount = Mathf.CeilToInt(turnCooldown);
			for (int turnNumber = 0; turnNumber > turnCount; turnNumber --)
			{
				HandleApplyDamage ();
				HandleMoving ();
				HandleApplyDamage ();
				turnCooldown ++;
			}
		}

		public virtual void HandleMoving ()
		{
			trs.position = objectWithWaypoints.wayPoints[currentWayPoint].position;
			SetNextWaypoint ();
		}
		
		void SetNextWaypoint ()
		{
			int previousWaypoint = currentWayPoint;
			if (isBacktracking)
			{
				currentWayPoint --;
				if (currentWayPoint == -1)
				{
					if (wrapMode == WrapMode.Once)
						isMoving = false;
					else if (wrapMode == WrapMode.Loop)
						currentWayPoint = objectWithWaypoints.wayPoints.Length - 1;
					else
					{
						currentWayPoint += 2;
						isBacktracking = !isBacktracking;
					}
				}
			}
			else
			{
				currentWayPoint ++;
				if (currentWayPoint == objectWithWaypoints.wayPoints.Length)
				{
					if (wrapMode == WrapMode.Once)
						isMoving = false;
					else if (wrapMode == WrapMode.Loop)
						currentWayPoint = 0;
					else
					{
						currentWayPoint -= 2;
						isBacktracking = !isBacktracking;
					}
				}
			}
			for (int i = 0; i < objectWithWaypoints.wayPoints.Length; i ++)
			{
				if (currentWayPoint == i || previousWaypoint == i)
				{
					gradientColorKeys[i] = new GradientColorKey(nextWaypointColor, 1f / objectWithWaypoints.wayPoints.Length * i);
					gradientAlphaKeys[i] = new GradientAlphaKey(nextWaypointColor.a, 1f / objectWithWaypoints.wayPoints.Length * i);
				}
				else
				{
					gradientColorKeys[i] = new GradientColorKey(normalWaypointColor, 1f / objectWithWaypoints.wayPoints.Length * i);
					gradientAlphaKeys[i] = new GradientAlphaKey(normalWaypointColor.a, 1f / objectWithWaypoints.wayPoints.Length * i);
				}
			}
			gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
			objectWithWaypoints.line.colorGradient = gradient;
		}

		public virtual void HandleApplyDamage ()
		{
			if ((GameManager.GetSingleton<Player>().trs.position - trs.position).sqrMagnitude < .7f)
				GameManager.GetSingleton<Player>().TakeDamage (damage);
		}

		void OnDisable ()
		{
			GameManager.GetSingleton<Player>().onMoved -= TakeTurn;
		}

		public override void Reset ()
		{
			trs.position = initPosition;
			currentWayPoint = initWayPoint;
		}

#if UNITY_EDITOR		
		[MenuItem("\"Turn-Based\" Chaos/Teleport to current waypoint")]
		public static void MakeWaypointsBetweenObjects ()
		{
			Saw saw = SelectionExtensions.GetInstance<Saw>();
			Transform[] selectedTransforms = SelectionExtensions.GetSelected<Transform>();
			foreach (Transform selectedTrs in selectedTransforms)
			{
				foreach (Transform selectedTrs2 in selectedTransforms)
				{
					if (selectedTrs != selectedTrs2)
					{
						LineSegment2D lineSegment = new LineSegment2D(selectedTrs.position, selectedTrs2.position);
						float distance = 0;
						do
						{
							Vector2 spawnPosition = lineSegment.GetPointWithDirectedDistance(distance);
							Instantiate(saw.objectWithWaypoints.wayPointsParent.GetChild(0), spawnPosition, Quaternion.identity, saw.trs);
							distance += saw.testDistanceBetweenWaypoints;
						} while (distance <= lineSegment.GetLength());
					}
				}
			}
		}
		
		[MenuItem("\"Turn-Based\" Chaos/Unrotate and jump to current waypoint")]
		public static void UnrotateAndJumpToCurrentWaypoint ()
		{
			// PrefabUtility.;
			Saw saw = SelectionExtensions.GetInstance<Saw>();
			foreach (Transform waypoint in saw.objectWithWaypoints.wayPoints)
				waypoint.SetParent(null);
			saw.trs.position = saw.objectWithWaypoints.wayPoints[saw.currentWayPoint].position;
			saw.trs.eulerAngles = Vector3.zero;
			foreach (Transform waypoint in saw.objectWithWaypoints.wayPoints)
				waypoint.SetParent(saw.trs);
		}
#endif

		public enum WrapMode
		{
			PingPong,
			Loop,
			Once
		}
	}
}