using UnityEngine;
using UnityEditor;

namespace PixelNarval.HPBars
{
    [CustomPropertyDrawer(typeof(TransitionEventValueCommonData))]
    public class TransitionEventValueCommonDataDrawer : DrawerCommon
    {
        SerializedProperty maxValue;
        SerializedProperty roundingType;
        SerializedProperty snapToInt;

        Color yellow = Color.yellow;
        Color white = Color.white;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            maxValue = property.FindPropertyRelative("maxValue");
            roundingType = property.FindPropertyRelative("roundingType");
            snapToInt = property.FindPropertyRelative("snapToInt");

            
            int lines = 0;
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(GetRectAtLine(position, lines++), maxValue);

            if (maxValue.intValue > 0)
            {
                EditorGUI.PropertyField(GetRectAtLine(position, lines++), roundingType);
                if (snapToInt.boolValue)
                {
                    GUI.contentColor = yellow;
                }
                //EditorGUI.PropertyField(GetRectAtLine(position, lines++), snapToInt);
                snapToInt.boolValue = EditorGUI.Toggle(GetRectAtLine(position, lines++), "Snap to Int", snapToInt.boolValue, EditorStyles.toggle);
                GUI.contentColor = white;
            }
            else
            {
                lines += 2;
            }            

            RefreshOnHeightChange(lines, property);
            EditorGUI.EndProperty();
        }
    }
}