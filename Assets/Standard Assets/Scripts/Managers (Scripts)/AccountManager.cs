using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace GridGame
{
	//[ExecuteInEditMode]
	public class AccountManager : MonoBehaviour
	{
		public static Account[] Accounts
		{
			get
			{
				List<Account> output = new List<Account>();
				output.Add(new Account());
				return output.ToArray();
			}
		}
		public static Account CurrentlyPlaying
		{
			get
			{
				if (lastUsedAccountIndex == -1)
					return null;
				return Accounts[lastUsedAccountIndex];
			}
		}
		public static int lastUsedAccountIndex = -1;
	}
}
