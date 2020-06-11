#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class RandomUniqueId : EditorScript
{
    public bool update;

	public virtual void Update ()
	{
        if (!update)
            return;
        update = false;
        GetComponent<IIdentifiable>().UniqueId = Random.Range(int.MinValue, int.MaxValue);
	}
}
#endif