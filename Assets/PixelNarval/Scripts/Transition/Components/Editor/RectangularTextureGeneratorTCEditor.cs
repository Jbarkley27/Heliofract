using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PixelNarval.HPBars
{
    [CustomEditor(typeof(RectangularTextureGeneratorTC), editorForChildClasses: true)]
    public class RectangularTextureGeneratorTCEditor : Editor
    {
        SerializedProperty targetImages;
        SerializedProperty resolution;
        SerializedProperty colorGradient;
        SerializedProperty colorGradientType;

        SerializedProperty filterMode;

        SerializedProperty useMaxValue;
        SerializedProperty segmentNumber;
        SerializedProperty segmentsSeparation;
        SerializedProperty horizontalSections;

        SerializedProperty texture;

        private void OnEnable()
        {
            targetImages = serializedObject.FindProperty("targetImages");
            resolution = serializedObject.FindProperty("resolution");
            colorGradient = serializedObject.FindProperty("colorGradient");
            colorGradientType = serializedObject.FindProperty("colorGradientType");
            filterMode = serializedObject.FindProperty("filterMode");
            useMaxValue = serializedObject.FindProperty("useMaxValue");
            segmentNumber = serializedObject.FindProperty("segmentNumber");
            segmentsSeparation = serializedObject.FindProperty("segmentsSeparation");
            horizontalSections = serializedObject.FindProperty("horizontalSections");
            texture = serializedObject.FindProperty("texture");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Texture Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetImages);
            EditorGUILayout.PropertyField(resolution);
            EditorGUILayout.PropertyField(colorGradient);
            EditorGUILayout.PropertyField(colorGradientType);
            EditorGUILayout.PropertyField(filterMode);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Segments Configuration", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useMaxValue);
            if (useMaxValue.boolValue)
            {
                GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(segmentNumber);
            if (useMaxValue.boolValue)
            {
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            if (segmentNumber.intValue > 1)
            {
                EditorGUILayout.PropertyField(horizontalSections);
                EditorGUILayout.PropertyField(segmentsSeparation);
            }

            //base.OnInspectorGUI();

            if (texture.objectReferenceValue != null)
            {
                Vector2Int imageResolution = resolution.vector2IntValue;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label((Texture2D) texture.objectReferenceValue, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.8f), GUILayout.MaxHeight(Mathf.Max(imageResolution.y, 50)));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}