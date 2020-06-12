using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Extensions;
using Utf8Json;
using System;
using Random = UnityEngine.Random;
using System.IO;

namespace GridGame
{
	//[ExecuteInEditMode]
	public class SaveAndLoadManager : SingletonMonoBehaviour<SaveAndLoadManager>
	{
		// [HideInInspector]
		public List<SaveAndLoadObject> saveAndLoadObjects = new List<SaveAndLoadObject>();
		public static SaveEntry[] saveEntries = new SaveEntry[0];
		// public static Dictionary<string, SaveAndLoadObject> saveAndLoadObjectTypeDict = new Dictionary<string, SaveAndLoadObject>();
		public TemporaryActiveText displayOnSave;
		public int pastSaveEntryPreserveCount;
		public bool usePlayerPrefs;
		[Multiline]
		public string savedData;
		
#if UNITY_EDITOR
		public virtual void OnEnable ()
		{
			if (Application.isPlaying)
			{
				// displayOnSave.obj.SetActive(false);
				return;
			}
			saveAndLoadObjects.Clear();
			saveAndLoadObjects.AddRange(FindObjectsOfType<SaveAndLoadObject>());
			foreach (SaveAndLoadObject saveAndLoadObject in saveAndLoadObjects)
			{
				saveAndLoadObject.saveables = saveAndLoadObject.GetComponentsInChildren<ISaveableAndLoadable>();
				if (saveAndLoadObject.uniqueId == MathfExtensions.NULL_INT || saveAndLoadObject.uniqueId == 0)
					saveAndLoadObject.uniqueId = Random.Range(int.MinValue, int.MaxValue);
				foreach (ISaveableAndLoadable saveableAndLoadable in saveAndLoadObject.saveables)
				{
					if (saveableAndLoadable.UniqueId == MathfExtensions.NULL_INT || saveableAndLoadable.UniqueId == 0)
						saveableAndLoadable.UniqueId = Random.Range(int.MinValue, int.MaxValue);
				}
			}
		}
#endif
		
		public virtual void Start ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
			if (!usePlayerPrefs)
				print(Application.persistentDataPath);
#endif
			Setup ();
		}

		public virtual void Setup ()
		{
			saveAndLoadObjects.Clear();
			saveAndLoadObjects.AddRange(FindObjectsOfType<SaveAndLoadObject>());
			// saveAndLoadObjectTypeDict.Clear();
			SaveAndLoadObject saveAndLoadObject;
			List<SaveEntry> _saveEntries = new List<SaveEntry>();
			for (int i = 0; i < saveAndLoadObjects.Count; i ++)
			{
				saveAndLoadObject = saveAndLoadObjects[i];
				saveAndLoadObject.Setup ();
				_saveEntries.AddRange(saveAndLoadObject.saveEntries);
			}
			saveEntries = _saveEntries.ToArray();
		}
		
		public virtual void Save ()
		{
			if (GameManager.GetSingleton<SaveAndLoadManager>() != this)
			{
				GameManager.GetSingleton<SaveAndLoadManager>().Save ();
				return;
			}
			// Setup ();
			if (!usePlayerPrefs)
			{
				savedData = "";
				if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt"))
					File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
				else
				{
					savedData = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
					string[] valueGroups = savedData.Split(new string[] { SaveEntry.VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
					for (int i = 0; i < valueGroups.Length; i += 2)
					{
						string valueGroup = valueGroups[i];
						if (valueGroup.StartsWith("" + AccountManager.lastUsedAccountIndex))
							savedData = savedData.RemoveEach(valueGroup + SaveEntry.VALUE_GROUP_SEPERATOR + valueGroups[i + 1] + SaveEntry.VALUE_GROUP_SEPERATOR);
					}
				}
			}
			if (AccountManager.lastUsedAccountIndex != -1)
			{
				AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex ++;
				if (AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex > AccountManager.CurrentlyPlaying.LastSaveEntryIndex)
					AccountManager.CurrentlyPlaying.LastSaveEntryIndex ++;
				for (int i = 0; i < saveEntries.Length; i ++)
				{
					SaveEntry saveEntry = saveEntries[i];
					if (AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex > pastSaveEntryPreserveCount)
						saveEntry.Delete (AccountManager.lastUsedAccountIndex, AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex - pastSaveEntryPreserveCount - 1);
					saveEntry.Save (AccountManager.lastUsedAccountIndex, AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex);
				}
			}
			else
			{
				for (int i = 0; i < saveEntries.Length; i ++)
					saveEntries[i].Save (-1, -1);
			}
			if (!usePlayerPrefs)
				File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt", savedData);
			if (displayOnSave.obj != null)
				displayOnSave.Do ();
		}
		
		public virtual void Load (int saveEntryIndex)
		{
			if (GameManager.GetSingleton<SaveAndLoadManager>() != this)
			{
				GameManager.GetSingleton<SaveAndLoadManager>().Load (saveEntryIndex);
				return;
			}
			if (!usePlayerPrefs)
			{
				if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt"))
					savedData = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
				else
					File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saved Data.txt");
			}
			Setup ();
			if (AccountManager.lastUsedAccountIndex != -1)
				AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex = saveEntryIndex;
			for (int i = 0; i < saveEntries.Length; i ++)
				saveEntries[i].Load (AccountManager.lastUsedAccountIndex, saveEntryIndex);
			OnLoaded ();
		}

		public virtual void OnLoaded ()
		{
			GameManager.GetSingleton<GameManager>().SetGosActive ();
			// GameManager.GetSingleton<AudioManager>().Awake ();
			// GameManager.GetSingleton<Player>().trs.position = GameManager.GetSingleton<Player>().SpawnPosition;
			GameManager.GetSingleton<GameCamera>().Awake ();
			// GameManager.GetSingleton<World>().Init ();
			// GameManager.GetSingleton<WorldMap>().Init ();
			// AccountSelectMenu.Init ();
			// foreach (Collectible collectible in Collectible.instances)
			// {
			// 	if (collectible.collected)
			// 		collectible.OnCollected ();
			// }
			// Obelisk.instances = FindObjectsOfType<Obelisk>();
			// foreach (Obelisk obelisk in Obelisk.instances)
			// {
			// 	if (obelisk.found)
			// 		obelisk.foundIndicator.SetActive(true);
			// }
			// Perk.instances = FindObjectsOfType<Perk>();
			// foreach (Perk perk in Perk.instances)
			// 	perk.Init ();
			// if (AccountManager.lastUsedAccountIndex != -1)
			// 	GameManager.GetSingleton<Player>().AddMoney (0);
		}

		public virtual void Delete (int saveEntryIndex)
		{
			if (GameManager.GetSingleton<SaveAndLoadManager>() != this)
			{
				GameManager.GetSingleton<SaveAndLoadManager>().Delete (saveEntryIndex);
				return;
			}
			for (int i = 0; i < saveEntries.Length; i ++)
				saveEntries[i].Delete (AccountManager.lastUsedAccountIndex, saveEntryIndex);
			Save ();
		}

		public static void ResetPersistantValues ()
		{
			GameManager.enabledGosString = "";
			GameManager.disabledGosString = "";
		}

		public virtual void DeleteAll ()
		{
			PlayerPrefs.DeleteAll();
			if (!usePlayerPrefs)
			{
				for (int i = -1; i < AccountManager.Accounts.Length; i ++)
					Delete (i);
			}
			ResetPersistantValues ();
		}
		
		public virtual void LoadMostRecent ()
		{
			if (AccountManager.lastUsedAccountIndex != -1)
				Load (AccountManager.CurrentlyPlaying.MostRecentlyUsedSaveEntryIndex);
			else
				Load (-1);
		}

		public static string Serialize (object value, Type type)
		{
			return JsonSerializer.NonGeneric.ToJsonString(type, value);
		}

		public static object Deserialize (string serializedState, Type type)
		{
			return JsonSerializer.NonGeneric.Deserialize(type, serializedState);
		}
		
		public class SaveEntry
		{
			public ISaveableAndLoadable saveableAndLoadable;
			public MemberEntry[] memberEntries = new MemberEntry[0];
			public const string VALUE_SEPERATOR = "Ⅰ";
			public const string VALUE_GROUP_SEPERATOR = "@";
			
			public SaveEntry ()
			{
			}
			
			public virtual void Save (int accountIndex, int saveEntryIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					if (!memberEntry.isField)
					{
						PropertyInfo property = memberEntry.member as PropertyInfo;
						if (GameManager.GetSingleton<SaveAndLoadManager>().usePlayerPrefs)
							PlayerPrefs.SetString(GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry), Serialize(property.GetValue(saveableAndLoadable, null), property.PropertyType));
						else
							GameManager.GetSingleton<SaveAndLoadManager>().savedData += GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry) + VALUE_GROUP_SEPERATOR + Serialize(property.GetValue(saveableAndLoadable, null), property.PropertyType) + VALUE_GROUP_SEPERATOR;
					}
					else
					{
						FieldInfo field = memberEntry.member as FieldInfo;
						if (GameManager.GetSingleton<SaveAndLoadManager>().usePlayerPrefs)
							PlayerPrefs.SetString(GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry), Serialize(field.GetValue(saveableAndLoadable), field.FieldType));
						else
							GameManager.GetSingleton<SaveAndLoadManager>().savedData += GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry) + VALUE_GROUP_SEPERATOR + Serialize(field.GetValue(saveableAndLoadable), field.FieldType) + VALUE_GROUP_SEPERATOR;
					}
				}
			}
			
			public virtual void Load (int accountIndex, int saveEntryIndex)
			{
				object value;
				foreach (MemberEntry memberEntry in memberEntries)
				{
					if (!memberEntry.isField)
					{
						PropertyInfo property = memberEntry.member as PropertyInfo;
						if (GameManager.GetSingleton<SaveAndLoadManager>().usePlayerPrefs)
						{
							value = Deserialize(PlayerPrefs.GetString(GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry), Serialize(property.GetValue(saveableAndLoadable, null), property.PropertyType)), property.PropertyType);
							property.SetValue(saveableAndLoadable, value, null);
						}
						else
						{
							string[] valueGroups = GameManager.GetSingleton<SaveAndLoadManager>().savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
							for (int i = 0; i < valueGroups.Length; i += 2)
							{
								string valueGroup = valueGroups[i];
								if (valueGroup == GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry))
								{
									valueGroup = valueGroups[i + 1];
									value = Deserialize(valueGroup, property.PropertyType);
									property.SetValue(saveableAndLoadable, value, null);
								}
							}
						}
					}
					else
					{
						FieldInfo field = memberEntry.member as FieldInfo;
						if (GameManager.GetSingleton<SaveAndLoadManager>().usePlayerPrefs)
						{
							value = Deserialize(PlayerPrefs.GetString(GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry), Serialize(field.GetValue(saveableAndLoadable), field.FieldType)), field.FieldType);
							field.SetValue(saveableAndLoadable, value);
						}
						else
						{
							string[] valueGroups = GameManager.GetSingleton<SaveAndLoadManager>().savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
							for (int i = 0; i < valueGroups.Length; i += 2)
							{
								string valueGroup = valueGroups[i];
								if (valueGroup == GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry))
								{
									valueGroup = valueGroups[i + 1];
									value = Deserialize(valueGroup, field.FieldType);
									field.SetValue(saveableAndLoadable, value);
								}
							}
						}
					}
				}
			}

			public virtual void Delete (int accountIndex, int saveEntryIndex)
			{
				foreach (MemberEntry memberEntry in memberEntries)
				{
					if (GameManager.GetSingleton<SaveAndLoadManager>().usePlayerPrefs)
						PlayerPrefs.DeleteKey(GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry));
					else
					{
						string[] valueGroups = GameManager.GetSingleton<SaveAndLoadManager>().savedData.Split(new string[] { VALUE_GROUP_SEPERATOR }, StringSplitOptions.None);
						for (int i = 0; i < valueGroups.Length; i += 2)
						{
							string valueGroup = valueGroups[i];
							if (valueGroup == GetKeyNameForMemberEntry(accountIndex, saveEntryIndex, memberEntry))
								GameManager.GetSingleton<SaveAndLoadManager>().savedData = GameManager.GetSingleton<SaveAndLoadManager>().savedData.RemoveEach(valueGroup + VALUE_GROUP_SEPERATOR + valueGroups[i + 1] + VALUE_GROUP_SEPERATOR);
						}
					}
				}
			}

			public virtual string GetKeyNameForMemberEntry (int accountIndex, int saveEntryIndex, MemberEntry memberEntry)
			{
				// if (memberEntry.isShared)
				// 	return VALUE_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR + memberEntry.member.Name;
				// else
					return accountIndex + VALUE_SEPERATOR + saveEntryIndex + VALUE_SEPERATOR + saveableAndLoadable.UniqueId + VALUE_SEPERATOR + memberEntry.member.Name;
			}

			public class MemberEntry
			{
				public MemberInfo member;
				public bool isField;
				// public bool isShared;
			}
		}
	}
}