using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoopArray<T>
{
	public static IEnumerator<T> GetEnumerator (T[] values)
	{
		for (int i = 0; i < values.Length; i ++)
			yield return values[i];
		yield break;
	}

	public static int Count (T[] values)
	{
		return values.Length;
	}

	public static T[] Sort<T> (T[] values, int step)
	{
		int originalValuesIndex = 0;
		List<T> remainingValues = new List<T>();
		int currentValueIndex = 0;
		int valueCount = values.Length;
		do
		{
			remainingValues.Add(values[originalValuesIndex]);
			currentValueIndex ++;
			originalValuesIndex += step;
			originalValuesIndex %= remainingValues.Count;
		} while (remainingValues.Count < valueCount);
		return remainingValues.ToArray();
	}
}