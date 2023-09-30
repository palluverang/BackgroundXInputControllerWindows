using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BackgroundXInputControllerWindows.Editor
{
    [CustomEditor(typeof(BackgroundXInputControllerWindows))]
    public class BackgroundXInputControllerWindowsEditor : UnityEditor.Editor
    {
        private SerializedProperty sp_deviceIndex;
        private SerializedProperty sp_xinputIndex;

        private void OnEnable()
        {
            sp_deviceIndex = serializedObject.FindProperty("_deviceIndex");
            sp_xinputIndex = serializedObject.FindProperty("_xinputIndex");
        }

        public override void OnInspectorGUI()
        {
            var bxicw = target as BackgroundXInputControllerWindows;
            if (bxicw == null)
                return;

            serializedObject.Update();

            if (sp_deviceIndex.intValue < 0)
            {
                var deviceNames = InputSystem
                    .devices
                    .Select(device => device.name.Replace('/', ' '))
                    .Prepend("default")
                    .ToArray();

                EditorGUI.BeginChangeCheck();
                var index = EditorGUILayout.Popup("Device", 0, deviceNames);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(bxicw, "Change Device");
                    EditorUtility.SetDirty(bxicw);
                }

                bxicw.UpdateDevice(index switch
                {
                    0 => default,
                    _ => InputSystem.devices[index - 1].description
                });
            }
            else
            {
                var deviceNames = InputSystem
                    .devices
                    .Select(device => device.name.Replace('/', ' '))
                    .ToArray();

                EditorGUI.BeginChangeCheck();
                var index = EditorGUILayout.Popup("Device", sp_deviceIndex.intValue, deviceNames);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(bxicw, "Change Device");
                    EditorUtility.SetDirty(bxicw);
                }

                bxicw.UpdateDevice(InputSystem.devices[index].description);
            }

            sp_xinputIndex.intValue = Mathf.Clamp(EditorGUILayout.IntField("XInput Index", sp_xinputIndex.intValue), 0, 3);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
