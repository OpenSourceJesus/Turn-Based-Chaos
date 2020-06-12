using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridGame
{
	public class Scroll : MonoBehaviour
	{
		public Transform trs;
		[Multiline(10)]
		public string text;
		public _Text displayText;
	}
}