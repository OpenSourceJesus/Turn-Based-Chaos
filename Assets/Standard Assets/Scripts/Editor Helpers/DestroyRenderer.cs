using UnityEngine;

namespace GridGame
{
	public class DestroyRenderer : MonoBehaviour
	{
		public virtual void Start ()
		{
			if (Application.isPlaying)
			{
				Destroy(GetComponent<Renderer>());
				Destroy(this);
			}
		}
	}
}