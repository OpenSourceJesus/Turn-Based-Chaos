using UnityEngine;
using Extensions;
using UnityEngine.InputSystem;

namespace GridGame
{
	public class InputManager : MonoBehaviour
	{
		public InputSettings settings;
		public static InputSettings Settings
		{
			get
			{
				return GameManager.GetSingleton<InputManager>().settings;
			}
		}
		public static bool UsingGamepad
		{
			get
			{
				return Gamepad.current != null;
			}
		}
		public static bool UsingMouse
		{
			get
			{
				return Mouse.current != null;
			}
		}
		public static bool UsingKeyboard
		{
			get
			{
				return Keyboard.current != null;
			}
		}
		public static bool UsingTouchscreen
		{
			get
			{
				return Touchscreen.current != null;
			}
		}
		public static int MoveInput
		{
			get
			{
				int output = Input.touchCount;
#if UNITY_EDITOR
				if (Mouse.current.leftButton.isPressed)
					output ++;
				if (Mouse.current.rightButton.isPressed)
					output ++;
#endif
				return output;
			}
		}
		public int _MoveInput
		{
			get
			{
				return MoveInput;
			}
		}
		public static bool InteractInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.aButton.isPressed;
				else
					return Keyboard.current.eKey.isPressed;
			}
		}
		public bool _InteractInput
		{
			get
			{
				return InteractInput;
			}
		}
		public static float ZoomInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.rightStick.y.ReadValue();
				else
					return Mouse.current.scroll.y.ReadValue();
			}
		}
		public float _ZoomInput
		{
			get
			{
				return ZoomInput;
			}
		}
		public static bool SubmitInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.aButton.isPressed;
				else
					return Keyboard.current.enterKey.isPressed;// || Mouse.current.leftButton.isPressed;
			}
		}
		public bool _SubmitInput
		{
			get
			{
				return SubmitInput;
			}
		}
		public static int SwitchMenuSectionInput
		{
			get
			{
				if (UsingGamepad)
				{
					int output = 0;
					if (Gamepad.current.rightShoulder.isPressed)
						output ++;
					if (Gamepad.current.leftShoulder.isPressed)
						output --;
					return output;
				}
				else
					return 0;
			}
		}
		public int _SwitchMenuSectionInput
		{
			get
			{
				return SwitchMenuSectionInput;
			}
		}
		public static Vector2 UIMovementInput
		{
			get
			{
				if (UsingGamepad)
					return Vector2.ClampMagnitude(Gamepad.current.leftStick.ToVec2(), 1);
				else
				{
					int x = 0;
					if (Keyboard.current.dKey.isPressed)
						x ++;
					if (Keyboard.current.aKey.isPressed)
						x --;
					int y = 0;
					if (Keyboard.current.wKey.isPressed)
						y ++;
					if (Keyboard.current.sKey.isPressed)
						y --;
					return Vector2.ClampMagnitude(new Vector2(x, y), 1);
				}
			}
		}
		public Vector2 _UIMovementInput
		{
			get
			{
				return UIMovementInput;
			}
		}
		public static bool PauseInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.startButton.isPressed || Gamepad.current.selectButton.isPressed;
				else
					return Keyboard.current.escapeKey.isPressed;
			}
		}
		public bool _PauseInput
		{
			get
			{
				return PauseInput;
			}
		}
		public static bool LeftClickInput
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.leftButton.isPressed;
				else
					return false;
			}
		}
		public bool _LeftClickInput
		{
			get
			{
				return LeftClickInput;
			}
		}
		public static bool RightClickInput
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.rightButton.isPressed;
				else
					return false;
			}
		}
		public bool _RightClickInput
		{
			get
			{
				return RightClickInput;
			}
		}
		public static Vector2 MousePosition
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.position.ToVec2();
				else
					return VectorExtensions.NULL;
			}
		}
		public Vector2 _MousePosition
		{
			get
			{
				return MousePosition;
			}
		}
		public static bool ClearDataInput
		{
			get
			{
				if (UsingKeyboard)
					return Keyboard.current.leftCtrlKey.isPressed && Keyboard.current.leftShiftKey.isPressed && Keyboard.current.cKey.isPressed && Keyboard.current.dKey.isPressed;
				else
					return false;
			}
		}
		public bool _ClearDataInput
		{
			get
			{
				return ClearDataInput;
			}
		}
	}

	// [Serializable]
	// public class InputButton
	// {
	// 	public string[] buttonNames;
	// 	public KeyCode[] keyCodes;

	// 	public virtual bool GetDown ()
	// 	{
	// 		bool output = false;
	// 		foreach (KeyCode keyCode in keyCodes)
	// 			output |= Input.GetKeyDown(keyCode);
	// 		foreach (string buttonName in buttonNames)
	// 			output |= InputManager.inputter.GetButtonDown(buttonName);
	// 		return output;
	// 	}

	// 	public virtual bool Get ()
	// 	{
	// 		bool output = false;
	// 		foreach (KeyCode keyCode in keyCodes)
	// 			output |= Input.GetKey(keyCode);
	// 		foreach (string buttonName in buttonNames)
	// 			output |= InputManager.inputter.GetButton(buttonName);
	// 		return output;
	// 	}

	// 	public virtual bool GetUp ()
	// 	{
	// 		bool output = false;
	// 		foreach (KeyCode keyCode in keyCodes)
	// 			output |= Input.GetKeyUp(keyCode);
	// 		foreach (string buttonName in buttonNames)
	// 			output |= InputManager.inputter.GetButtonUp(buttonName);
	// 		return output;
	// 	}
	// }

	// [Serializable]
	// public class InputAxis
	// {
	// 	public InputButton positiveButton;
	// 	public InputButton negativeButton;

	// 	public virtual int Get ()
	// 	{
	// 		int output = 0;
	// 		if (positiveButton.Get())
	// 			output ++;
	// 		if (negativeButton.Get())
	// 			output --;
	// 		return output;
	// 	}
	// }

	// public enum InputDevice
	// {
	// 	Keyboard,
	// 	Gamepad
	// }
}