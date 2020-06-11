using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThreadingUtilities
{
	public class CoroutineWithData
	{
		public Coroutine coroutine { get; private set; }
		public object result;
		IEnumerator target;

		public CoroutineWithData ()
		{
		}

		public CoroutineWithData (MonoBehaviour owner, IEnumerator target)
		{
			this.target = target;
			this.coroutine = owner.StartCoroutine(Run ());
		}
	 
		IEnumerator Run ()
		{
			while (target.MoveNext())
			{
				result = target.Current;
				yield return result;
			}
	    }
	}

	public class CoroutineWithData<T> : CoroutineWithData
	{
	}
}