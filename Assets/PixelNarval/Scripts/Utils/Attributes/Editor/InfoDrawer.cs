using UnityEngine;
using UnityEditor;

namespace PixelNarval.HPBars
{
    [CustomPropertyDrawer(typeof(InfoAttribute))]
    public class InfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InfoAttribute infoAttribute = attribute as InfoAttribute;
            float height = EditorStyles.helpBox.CalcHeight(new GUIContent(infoAttribute.message), EditorGUIUtility.currentViewWidth - 100);
            //return base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing + rect.height;
            //return EditorGUIUtility.standardVerticalSpacing + rect.height;
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InfoAttribute infoAttribute = attribute as InfoAttribute;
            int numLines = infoAttribute.message.Split('\n').Length;
            EditorGUI.BeginProperty(position, label, property);

            float height = EditorStyles.helpBox.CalcHeight(new GUIContent(infoAttribute.message), EditorGUIUtility.currentViewWidth - 100);
            Rect rect = position;
            rect.position = position.position + new Vector2(0, EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);
            rect.height = height;

            EditorGUI.PropertyField(position, property, label, true);

            //EditorGUI.DrawRect(rect, Color.green);

            EditorGUI.HelpBox(rect, infoAttribute.message, MessageType.Info);


            EditorGUI.EndProperty();
        }
    }
}