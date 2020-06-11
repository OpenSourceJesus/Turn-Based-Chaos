// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GridGame;
// using Rewired;
// using UnityEngine.UI;
// using InputManager = GridGame.InputManager;

// public class ButtonMapper : MonoBehaviour
// {
// 	public Transform trs;
// 	[HideInInspector]
// 	public string actionName;
// 	[HideInInspector]
// 	public ControllerType controllerType;
// 	[HideInInspector]
// 	public Pole axisContribution;
// 	[HideInInspector]
// 	public AxisRange axisRange;
// 	public Text actionNameText;
// 	public Text buttonNameText;
// 	public _Selectable selectable;

// 	public virtual void PollInput ()
// 	{
// 		StopAllCoroutines();
// 		StartCoroutine(PollInputRoutine ());
// 	}

// 	public virtual IEnumerator PollInputRoutine ()
// 	{
// 		yield return new WaitForEndOfFrame();
// 		yield return new WaitForEndOfFrame();
// 		IEnumerable<ControllerPollingInfo> pollInfos;
// 		while (true)
// 		{
// 			if (InputManager.UsingGamepad)
// 			{
// 				pollInfos = ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Joystick, ReInput.controllers.Joysticks[0].id);
// 				foreach (ControllerPollingInfo pollInfo in pollInfos)
// 				{
// 					if (pollInfo.success)
// 					{
// 						OnPollInputSuccess (pollInfo);
// 						yield break;
// 					}
// 				}
// 			}
// 			pollInfos = ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Keyboard, ReInput.controllers.Keyboard.id);
// 			foreach (ControllerPollingInfo pollInfo in pollInfos)
// 			{
// 				if (pollInfo.success)
// 				{
// 					OnPollInputSuccess (pollInfo);
// 					yield break;
// 				}
// 			}
// 			pollInfos = ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Mouse, ReInput.controllers.Mouse.id);
// 			foreach (ControllerPollingInfo pollInfo in pollInfos)
// 			{
// 				if (pollInfo.success)
// 				{
// 					OnPollInputSuccess (pollInfo);
// 					yield break;
// 				}
// 			}
// 			yield return new WaitForEndOfFrame();
// 		}
// 		yield break;
// 	}

// 	public virtual void OnPollInputSuccess (ControllerPollingInfo pollInfo)
// 	{
// 		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
// 		{
// 			foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(actionName))
// 			{
// 				if (actionElementMap.axisContribution == axisContribution && actionElementMap.axisRange == axisRange)
// 				{
// 					ElementAssignment elementAssignment = new ElementAssignment(pollInfo.controllerType, ControllerElementType.Button, pollInfo.elementIdentifierId, axisRange, pollInfo.keyboardKey, ModifierKeyFlags.None, actionElementMap.actionId, axisContribution, false, actionElementMap.id);
// 					if (pollInfo.controllerType != controllerMap.controllerType)
// 					{
// 						Debug.Log(pollInfo.controllerType);
// 						controllerMap.DeleteElementMapsWithAction(actionName);
// 						foreach (ControllerMap otherControllerMap in InputManager.inputter.controllers.maps.GetAllMaps(pollInfo.controllerType))
// 							otherControllerMap.CreateElementMap(elementAssignment);
// 					}
// 					else
// 						controllerMap.ReplaceElementMap(elementAssignment);
// 					buttonNameText.text = pollInfo.elementIdentifierName;
// 					controllerType = pollInfo.controllerType;
// 					Save ();
// 					return;
// 				}
// 			}
// 		}
// 	}

// 	public virtual void Save ()
// 	{
// 		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps(controllerType))
// 		{
// 			foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(actionName))
// 			{
// 				if (actionElementMap.axisContribution == axisContribution && actionElementMap.axisRange == axisRange)
// 				{
// 					string elementAssignmentData = actionElementMap.elementIndex + ControlsMapper.VALUE_SEPARATOR;
// 					elementAssignmentData += actionElementMap.keyCode.GetHashCode() + ControlsMapper.VALUE_SEPARATOR;
// 					elementAssignmentData += controllerType.GetHashCode() + ControlsMapper.VALUE_SEPARATOR;
// 					elementAssignmentData += axisContribution.GetHashCode() + ControlsMapper.VALUE_SEPARATOR;
// 					elementAssignmentData += axisRange.GetHashCode() + ControlsMapper.VALUE_SEPARATOR;
// 					PlayerPrefs.SetString(actionNameText.text + AccountManager.lastUsedAccountIndex, elementAssignmentData);
// 					return;
// 				}
// 			}
// 		}
// 	}
// }