using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.UI;
#endif

namespace GridGame
{
	//[ExecuteInEditMode]
	public class BuildManager : MonoBehaviour
	{
#if UNITY_EDITOR
		public BuildAction[] buildActions;
		static BuildPlayerOptions buildOptions;
		public Text versionNumberText;
#endif
		public int versionIndex;
		public string versionNumberPrefix;
		public GameObject devOptionsGo;
		
		public virtual void Start ()
		{
			print(SystemInfo.deviceUniqueIdentifier);
			string[] myDeviceUniqueIdentifiers = new string[2];
			myDeviceUniqueIdentifiers[0] = "a43c6de0f2f86a8c5798f4a46e938fc62982ed00";
			myDeviceUniqueIdentifiers[1] = "A023E908-FF06-5675-B0AD-D40EAFF5AF11";
			devOptionsGo.SetActive(false);
			foreach (string myDeviceUniqueIdentifier in myDeviceUniqueIdentifiers)
			{
				if (SystemInfo.deviceUniqueIdentifier == myDeviceUniqueIdentifier)
				{
					devOptionsGo.SetActive(true);
					break;
				}
			}
		}

#if UNITY_EDITOR
		public static string[] GetScenePathsInBuild ()
		{
			List<string> scenePathsInBuild = new List<string>();
			string scenePath = null;
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i ++)
			{
				scenePath = EditorBuildSettings.scenes[i].path;
				if (EditorBuildSettings.scenes[i].enabled)
					scenePathsInBuild.Add(scenePath);
			}
			return scenePathsInBuild.ToArray();
		}

		public static string[] GetAllScenePaths ()
		{
			List<string> scenePathsInBuild = new List<string>();
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i ++)
				scenePathsInBuild.Add(EditorBuildSettings.scenes[i].path);
			return scenePathsInBuild.ToArray();
		}
		
		[MenuItem("Build/Make Builds")]
		public static void Build ()
		{
			GameManager.GetSingleton<BuildManager>()._Build ();
		}

		public virtual void _Build ()
		{
			GameManager.GetSingleton<BuildManager>().versionIndex ++;
			PreBuildScript[] preBuildScripts = FindObjectsOfType<PreBuildScript>();
			foreach (PreBuildScript preBuildScript in preBuildScripts)
			{
				if (preBuildScript.enabled)
					preBuildScript.Do ();
			}
			foreach (BuildAction buildAction in buildActions)
			{
				if (buildAction.enabled)
					buildAction.Do ();
			}
			PostBuildScript[] postBuildScripts = FindObjectsOfType<PostBuildScript>();
			foreach (PostBuildScript postBuildScript in postBuildScripts)
			{
				if (postBuildScript.enabled)
					postBuildScript.Do ();
			}
		}
		
		[Serializable]
		public class BuildAction
		{
			public string name;
			public bool enabled;
			public BuildTarget target;
			public string locationPath;
			public BuildOptions[] options;
			public bool makeZip;
			public string directoryToZip;
			public string zipLocationPath;
			public bool moveCrashHandler;
			
			public virtual void Do ()
			{
				EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
				EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
				if (GameManager.GetSingleton<BuildManager>().versionNumberText != null)
					GameManager.GetSingleton<BuildManager>().versionNumberText.text = GameManager.GetSingleton<BuildManager>().versionNumberPrefix + DateTime.Now.Date.ToString("MMdd");
				if (GameManager.GetSingleton<ConfigurationManager>() != null)
					GameManager.GetSingleton<ConfigurationManager>().canvas.gameObject.SetActive(false);
				EditorSceneManager.MarkAllScenesDirty();
				EditorSceneManager.SaveOpenScenes();
				buildOptions = new BuildPlayerOptions();
				buildOptions.scenes = GetScenePathsInBuild();
				buildOptions.target = target;
				buildOptions.locationPathName = locationPath;
				foreach (BuildOptions option in options)
					buildOptions.options |= option;
				BuildPipeline.BuildPlayer(buildOptions);
				if (GameManager.GetSingleton<ConfigurationManager>() != null)
					GameManager.GetSingleton<ConfigurationManager>().canvas.gameObject.SetActive(true);
				AssetDatabase.Refresh();
				if (moveCrashHandler)
				{
					string extrasPath = locationPath + "/Extras";
					string crashHandlerFileName = "UnityCrashHandler64.exe";
					if (!Directory.Exists(extrasPath))
						Directory.CreateDirectory(extrasPath);
					if (File.Exists(extrasPath + "/" + crashHandlerFileName))
						File.Delete(extrasPath + "/" + crashHandlerFileName);
					else
					{
						crashHandlerFileName = "UnityCrashHandler32.exe";
						if (File.Exists(extrasPath + "/" + crashHandlerFileName))
							File.Delete(extrasPath + "/" + crashHandlerFileName);
					}
					File.Move(locationPath + "/" + crashHandlerFileName, extrasPath + "/" + crashHandlerFileName);
				}
				if (makeZip)
				{
					File.Delete(zipLocationPath);
					DirectoryCompressionOperations.CompressDirectory (directoryToZip, zipLocationPath);
				}
			}
		}
#endif
	}
}