using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridGame
{
	//[ExecuteInEditMode]
	public class EventManager : SingletonMonoBehaviour<EventManager>, IUpdatable
	{
		public static List<Event> events = new List<Event>();
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}

		public virtual void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		Event _event;
		public virtual void DoUpdate ()
		{
			for (int i = 0; i < events.Count; i ++)
			{
				_event = events[i];
				if (Time.time >= _event.time)
				{
					_event.onEvent ();
					events.RemoveAt(i);
					i --;
				}
			}
		}

		public class Event
		{
			public Action onEvent;
			public float time;

			public Event (Action onEvent, float time = 0)
			{
				this.onEvent = onEvent;
				this.time = time;
			}

			public virtual void Remove ()
			{
				time = Mathf.Infinity;
				events.Remove(this);
			}
		}

		public class Event<T> : Event
		{
			public T arg;

			public Event (Action onEvent, T arg = default(T), float time = 0) : base (onEvent, time)
			{
				this.arg = arg;
			}
		}
	}
}