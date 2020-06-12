using System;
using Extensions;
using UnityEngine;

namespace GridGame
{
	[Serializable]
	public class Account : IDefaultable<Account>//, ISaveableAndLoadable
	{
		// public int uniqueId;
		// public int UniqueId
		// {
		// 	get
		// 	{
		// 		return uniqueId;
		// 	}
		// 	set
		// 	{
		// 		uniqueId = value;
		// 	}
		// }
		// public string Name
		// {
		// 	get
		// 	{
		// 		return name;
		// 	}
		// 	set
		// 	{
		// 		name = value;
		// 	}
		// }
		public int index;
		// [SaveAndLoadValue(true)]
		// public string name;
		public string Name
		{
			get
			{
				return PlayerPrefs.GetString("Account " + index + " name", "");
			}
			set
			{
				PlayerPrefs.SetString("Account " + index + " name", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public float playTime;
		public float PlayTime
		{
			get
			{
				return PlayerPrefs.GetFloat("Account " + index + " play time", 0);
			}
			set
			{
				PlayerPrefs.SetFloat("Account " + index + " play time", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public int totalMoney;
		public int TotalMoney
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " total money", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " total money", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public int currentMoney;
		public int CurrentMoney
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " current money", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " current money", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public int obelisksTouched;
		public int ObelisksTouched
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " obelisks touched", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " obelisks touched", value);
			}
		}
		public int MostRecentlyUsedSaveEntryIndex
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " most recently used save entry index", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " most recently used save entry index", value);
			}
		}
		public int LastSaveEntryIndex
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " last save entry index", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " last save entry index", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public bool isPlaying;
		
		public Account GetDefault ()
		{
			Name = "";
			PlayTime = 0;
			TotalMoney = 0;
			CurrentMoney = 0;
			ObelisksTouched = 0;
			// isPlaying = false;
			return this;
		}
	}
}