// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GridGame;
// using Rewired;
// using UnityEngine.UI;
// using InputManager = GridGame.InputManager;
// using Extensions;
// using System;

// public class ControlsMapper : SingletonMonoBehaviour<ControlsMapper>
// {
// 	public ButtonMapper buttonMapperPrefab;
// 	public Transform buttonMappersParent;
// 	ButtonMapper[] buttonMappers = new ButtonMapper[0];
// 	public const string VALUE_SEPARATOR = "⧫";
// 	public Scrollbar scrollbar;
// 	public RectTransform viewport;

// 	public override void Awake ()
// 	{
// 		base.Awake ();
// 		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
// 		{
// 			foreach (ActionElementMap actionElementMap in controllerMap.AllMaps)
// 			{
// 				InputAction inputAction = ReInput.mapping.GetAction(actionElementMap.actionId);
// 				if (inputAction.userAssignable && ReInput.mapping.ActionCategories[inputAction.categoryId].userAssignable)
// 				{
// 					ButtonMapper buttonMapper = Instantiate(buttonMapperPrefab, buttonMappersParent);
// 					buttonMapper.trs.SetSiblingIndex(0);
// 					buttonMapper.actionName = inputAction.name;
// 					if (inputAction.type == InputActionType.Button)
// 						buttonMapper.actionNameText.text = inputAction.name;
// 					else if (actionElementMap.axisContribution == Pole.Positive)
// 						buttonMapper.actionNameText.text = inputAction.positiveDescriptiveName;
// 					else
// 						buttonMapper.actionNameText.text = inputAction.negativeDescriptiveName;
// 					buttonMapper.buttonNameText.text = actionElementMap.elementIdentifierName;
// 					buttonMapper.controllerType = controllerMap.controllerType;
// 					buttonMapper.axisContribution = actionElementMap.axisContribution;
// 					buttonMapper.axisRange = actionElementMap.axisRange;
// 					buttonMapper.selectable.scrollbarThatMovesMe = scrollbar;
// 					buttonMapper.selectable.container = viewport;
// 					buttonMappers = buttonMappers.Add(buttonMapper);
// 				}
// 			}
// 		}
// 	}

// 	public static void Load ()
// 	{
// 		foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
// 		{
// 			for (int i = 0; i < controllerMap.AllMaps.Count; i ++)
// 			{
// 				ActionElementMap actionElementMap = controllerMap.AllMaps[i];
// 				InputAction inputAction = ReInput.mapping.GetAction(actionElementMap.actionId);
// 				string actionName;
// 				if (inputAction.type == InputActionType.Button)
// 					actionName = inputAction.name;
// 				else if (actionElementMap.axisContribution == Pole.Positive)
// 					actionName = inputAction.positiveDescriptiveName;
// 				else
// 					actionName = inputAction.negativeDescriptiveName;
// 				string elementAssignmentData = PlayerPrefs.GetString(actionName + AccountManager.lastUsedAccountIndex, "");
// 				if (string.IsNullOrEmpty(elementAssignmentData))
// 					return;
// 				string[] elementAssignmentDataPieces = elementAssignmentData.Split(new string[] { VALUE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
// 				int elementIdentifierId = int.Parse(elementAssignmentDataPieces[0]);
// 				KeyCode keyCode = (KeyCode) Enum.ToObject(typeof(KeyCode), int.Parse(elementAssignmentDataPieces[1]));
// 				ControllerType controllerType = (ControllerType) Enum.ToObject(typeof(ControllerType), int.Parse(elementAssignmentDataPieces[2]));
// 				Pole axisContribution = (Pole) Enum.ToObject(typeof(Pole), int.Parse(elementAssignmentDataPieces[3]));
// 				AxisRange axisRange = (AxisRange) Enum.ToObject(typeof(AxisRange), int.Parse(elementAssignmentDataPieces[4]));
// 				ElementAssignment elementAssignment = new ElementAssignment(controllerType, ControllerElementType.Button, elementIdentifierId, axisRange, keyCode, ModifierKeyFlags.None, actionElementMap.actionId, axisContribution, false, actionElementMap.id);
// 				if (controllerType != controllerMap.controllerType)
// 				{
// 					controllerMap.DeleteButtonMapsWithAction(inputAction.name);
// 					foreach (ControllerMap otherControllerMap in InputManager.inputter.controllers.maps.GetAllMaps(controllerType))
// 						otherControllerMap.CreateElementMap(elementAssignment);
// 				}
// 				else
// 					controllerMap.ReplaceElementMap(elementAssignment);
// 				Debug.Log(elementAssignmentData);
// 			}
// 		}
// 	}

// 	public virtual void ResetToDefaults ()
// 	{
// 		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
// 		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
// 		InputManager.inputter.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
// 		foreach (ButtonMapper buttonMapper in buttonMappers)
// 		{
// 			foreach (ControllerMap controllerMap in InputManager.inputter.controllers.maps.GetAllMaps())
// 			{
// 				foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(buttonMapper.actionName))
// 				{
// 					if (actionElementMap.axisContribution == buttonMapper.axisContribution && actionElementMap.axisRange == buttonMapper.axisRange)
// 					{
// 						buttonMapper.buttonNameText.text = actionElementMap.elementIdentifierName;
// 						buttonMapper.controllerType = controllerMap.controllerType;
// 						buttonMapper.Save ();
// 					}
// 				}
// 			}
// 		}
// 	}
// }