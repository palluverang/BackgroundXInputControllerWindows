using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

namespace BackgroundXInputControllerWindows
{
    public class BackgroundXInputControllerWindows : MonoBehaviour
    {
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        private struct State : IInputStateTypeInfo
        {
            public FourCC format => new FourCC('X', 'I', 'N', 'P');

            [FieldOffset(0)]
            public ushort buttons;

            [FieldOffset(2)]
            public byte leftTrigger;

            [FieldOffset(3)]
            public byte rightTrigger;

            [FieldOffset(4)]
            public short leftStickX;

            [FieldOffset(6)]
            public short leftStickY;

            [FieldOffset(8)]
            public short rightStickX;

            [FieldOffset(10)]
            public short rightStickY;
        }

        [SerializeField] private int _deviceIndex = -1;
        [SerializeField] private InputDeviceDescription _deviceDescription = default;
        [SerializeField] private InputDevice _device = null;
        [SerializeField] private int _xinputIndex = 0;

        private void OnEnable()
        {
            InputSystem.onDeviceChange += onDeviceChange;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= onDeviceChange;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (UnityEditorInternal.InternalEditorUtility.isApplicationActive)
                return;

            if (_device is not XInputControllerWindows gamepad)
                return;

            if (!XInputGamepadInternal.GetState(_xinputIndex))
                return;

            InputSystem.QueueStateEvent(gamepad, new State
            {
                buttons = (ushort)XInputGamepadInternal.GetButtons(_xinputIndex),
                leftTrigger = XInputGamepadInternal.GetLeftTrigger(_xinputIndex),
                rightTrigger = XInputGamepadInternal.GetRightTrigger(_xinputIndex),
                leftStickX = XInputGamepadInternal.GetThumbLX(_xinputIndex),
                leftStickY = XInputGamepadInternal.GetThumbLY(_xinputIndex),
                rightStickX = XInputGamepadInternal.GetThumbRX(_xinputIndex),
                rightStickY = XInputGamepadInternal.GetThumbRY(_xinputIndex),
            });
        }
#endif

        private void onDeviceChange(InputDevice device, InputDeviceChange inputDeviceChange)
        {
            UpdateDevice(_deviceDescription);
        }

        public void UpdateDevice(InputDeviceDescription description)
        {
            for (_deviceIndex = 0; _deviceIndex < InputSystem.devices.Count; _deviceIndex++)
            {
                var device = InputSystem.devices[_deviceIndex];
                if (device.description == description)
                {
                    _device = device;
                    _deviceDescription = description;
                    break;
                }
            }

            if (InputSystem.devices.Count == _deviceIndex)
            {
                _deviceIndex = -1;
                _device = null;
            }
        }
    }

    internal static class XInputGamepadInternal
    {
        private const string dll = "XInputGamepad.dll";

        [DllImport(dll)]
        public static extern bool GetState(int index);

        [DllImport(dll)]
        public static extern ulong GetPacketNumber(int index);

        [DllImport(dll)]
        public static extern ulong GetButtons(int index);

        [DllImport(dll)]
        public static extern byte GetLeftTrigger(int index);

        [DllImport(dll)]
        public static extern byte GetRightTrigger(int index);

        [DllImport(dll)]
        public static extern short GetThumbLX(int index);

        [DllImport(dll)]
        public static extern short GetThumbLY(int index);

        [DllImport(dll)]
        public static extern short GetThumbRX(int index);

        [DllImport(dll)]
        public static extern short GetThumbRY(int index);

        [DllImport(dll)]
        public static extern bool GetCapabilities(int index);

        [DllImport(dll)]
        public static extern byte GetType(int index);

        [DllImport(dll)]
        public static extern byte GetSubType(int index);

        [DllImport(dll)]
        public static extern bool GetKeystroke(int index);

        [DllImport(dll)]
        public static extern ushort GetVirtualKey(int index);

        [DllImport(dll)]
        public static extern ushort GetUnicode(int index);

        [DllImport(dll)]
        public static extern ushort GetFlags(int index);

        [DllImport(dll)]
        public static extern byte GetUserIndex(int index);

        [DllImport(dll)]
        public static extern byte GetHidCode(int index);
    }
}
