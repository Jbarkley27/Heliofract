using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace PixelNarval.HPBars
{
    public abstract class DrawerCommon : PropertyDrawer
    {
        protected static Dictionary<string, int> linesDic = new Dictionary<string, int>();
        private static Dictionary<string, int> linesBackupDic = new Dictionary<string, int>();

        public DrawerCommon()
        {
            if (linesDic == null)
            {
                linesDic = new Dictionary<string, int>();
            }
            if (linesBackupDic == null)
            {
                linesBackupDic = new Dictionary<string, int>();
            }
        }

        public Rect GetRectAtLine(Rect position, int lineNumber, int height = 1, bool draw = false)
        {
            Rect rect = new Rect(
                position.min.x,
                position.min.y + (lineNumber * EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.standardVerticalSpacing * (lineNumber - 1)),
                position.size.x,
                (EditorGUIUtility.singleLineHeight * height) + EditorGUIUtility.standardVerticalSpacing
                );
            if (draw)
            {
                Color color = Random.ColorHSV();
                color.a = 0.5f;
                EditorGUI.DrawRect(rect, color);
            }
            return rect;
        }


        public Rect DrawRect(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
            return rect;
        }

        protected int DrawTabs(Rect position, int height, string[] labels, int currentSelected, Color color)
        {
            int identLevelBackup = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            float squareHeight = 5 * EditorGUIUtility.singleLineHeight;
            position.height = height * EditorGUIUtility.singleLineHeight;
            //DrawRect(mainRect, Color.green);

            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;

            Color colorBackup = GUI.backgroundColor;
            for (int i = 0; i < labels.Length; i++)
            {
                Rect buttonRect = position;
                buttonRect.width /= labels.Length;
                buttonRect.x += (buttonRect.width * i);
                GUI.backgroundColor = i == currentSelected ? color : Color.white;
                //DrawRect(buttonRect, Color.green);

                EditorGUI.LabelField(buttonRect, labels[i], style);
                if (GUI.Button(buttonRect, new GUIContent(labels[i])))
                {
                    EditorGUI.indentLevel = identLevelBackup;
                    return i;
                }

                //DrawRect(imageRect, Color.yellow);

                //imageProperties[i].objectReferenceValue = EditorGUI.ObjectField(imageRect, GUIContent.none, (Sprite)imageProperties[i].objectReferenceValue, typeof(Sprite), false);
            }
            GUI.backgroundColor = colorBackup;
            EditorGUI.indentLevel = identLevelBackup;
            return currentSelected;

        }

        protected int DrawButtonRow(Rect position, string[] labels)
        {
            int identLevelBackup = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                Rect buttonRect = position;
                buttonRect.width /= labels.Length;
                buttonRect.x += (buttonRect.width * i);

                EditorGUI.LabelField(buttonRect, labels[i]);
                if (GUI.Button(buttonRect, new GUIContent(labels[i])))
                {
                    EditorGUI.indentLevel = identLevelBackup;
                    return i;
                }
            }
            EditorGUI.indentLevel = identLevelBackup;
            return -1;
        }

        public void DrawUnfilledRect(Rect rect, Color color, int thickness = 2)
        {
            EditorGUI.DrawRect(rect, color);

            Rect fillRect = new Rect(rect.position.x + thickness, rect.position.y + thickness, rect.width - (2 * thickness), rect.height - (2 * thickness));
            Color defaultInspectorBackgroundColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.76f, 0.76f, 0.76f);
            EditorGUI.DrawRect(fillRect, defaultInspectorBackgroundColor);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            linesDic.TryGetValue(PropertyIdentifier(property), out int totalLines);
            //lines += (int)((EditorGUI.GetPropertyHeight(texts) + EditorGUIUtility.standardVerticalSpacing) / (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
            return (EditorGUIUtility.singleLineHeight * totalLines) + (EditorGUIUtility.standardVerticalSpacing * (totalLines - 1));
        }

        protected bool RefreshOnHeightChange(int lines, SerializedProperty property)
        {
            linesBackupDic.TryGetValue(PropertyIdentifier(property), out int linesBackup);
            if (lines != linesBackup)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            linesDic[PropertyIdentifier(property)] = lines;
            linesBackupDic[PropertyIdentifier(property)] = lines;
            return lines != linesBackup;
        }

        protected string PropertyIdentifier(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetInstanceID() + property.propertyPath;
        }

        public static object GetValue(UnityEditor.SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            FieldInfo field = null;
            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                //field = type.GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                field = type.GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                obj = field.GetValue(obj);
            }
            return obj;
        }

        public static object GetValue(object obj, string propertyPath)
        {
            FieldInfo field = null;
            foreach (var path in propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                obj = field.GetValue(obj);
            }
            return obj;
        }
    }
}