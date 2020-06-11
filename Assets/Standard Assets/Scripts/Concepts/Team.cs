using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace GridGame
{
	public class Team : MonoBehaviour
	{
		public Color color;
		public Team Opponent
		{
			get
			{
				return opponents[0];
			}
			set
			{
				opponents[0] = value;
			}
		}
		public Team[] opponents;
	}
}