using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace PixelNarval.HPBars
{
    [CustomEditor(typeof(CircularTextureGeneratorTC), editorForChildClasses: true)]
    public class CircularTextureGeneratorTCEditor : Editor
    {
        SerializedProperty targetImages;
        SerializedProperty resolution;
        SerializedProperty colorGradient;
        SerializedProperty colorGradientType;
        SerializedProperty border;
        SerializedProperty center;
        SerializedProperty externalRadius;
        SerializedProperty internalRadius;


        SerializedProperty filterMode;

        SerializedProperty useMaxValue;
        SerializedProperty segmentNumber;
        SerializedProperty segmentsSeparation;
        SerializedProperty originSegmentsSeparation;
        SerializedProperty angleOffset;

        SerializedProperty texture;

        private void OnEnable()
        {
            targetImages = serializedObject.FindProperty("targetImages");
            resolution = serializedObject.FindProperty("resolution");
            colorGradient = serializedObject.FindProperty("colorGradient");
            colorGradientType = serializedObject.FindProperty("colorGradientType");
            border = serializedObject.FindProperty("border");
            center = serializedObject.FindProperty("center");
            externalRadius = serializedObject.FindProperty("externalRadius");
            internalRadius = serializedObject.FindProperty("internalRadius");

            filterMode = serializedObject.FindProperty("filterMode");

            useMaxValue = serializedObject.FindProperty("useMaxValue");
            segmentNumber = serializedObject.FindProperty("segmentNumber");
            segmentsSeparation = serializedObject.FindProperty("segmentsSeparation");
            originSegmentsSeparation = serializedObject.FindProperty("originSegmentsSeparation");
            angleOffset = serializedObject.FindProperty("angleOffset");

            texture = serializedObject.FindProperty("texture");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Texture Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetImages);
            EditorGUILayout.PropertyField(resolution);
            EditorGUILayout.PropertyField(center);
            EditorGUILayout.PropertyField(colorGradient);
            EditorGUILayout.PropertyField(colorGradientType);
            EditorGUILayout.PropertyField(border);
            EditorGUILayout.PropertyField(angleOffset);

            float internalRadiusValue = internalRadius.floatValue;
            float externalRadiusValue = externalRadius.floatValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Radius");
            internalRadiusValue = EditorGUILayout.FloatField(internalRadiusValue);
            externalRadiusValue = EditorGUILayout.FloatField(externalRadiusValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref internalRadiusValue, ref externalRadiusValue, 0, 2);
            internalRadius.floatValue = internalRadiusValue;
            externalRadius.floatValue = externalRadiusValue;

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
                EditorGUILayout.PropertyField(originSegmentsSeparation, new GUIContent("Origin Separation"));
                EditorGUILayout.PropertyField(segmentsSeparation, new GUIContent("Border Separation"));
            }

            //base.OnInspectorGUI();

            if (texture.objectReferenceValue != null)
            {
                Vector2Int imageResolution = resolution.vector2IntValue;

                GUILayout.Label("Generated Texture:");
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Rect textureRect = GUILayoutUtility.GetRect(200, 200);
                GUI.DrawTexture(textureRect, (Texture2D) texture.objectReferenceValue, ScaleMode.ScaleToFit);
                //GUILayout.Label((Texture2D)texture.objectReferenceValue, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.8f), GUILayout.MaxHeight(Mathf.Max(imageResolution.y, 50)));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            

            serializedObject.ApplyModifiedProperties();

        }
    }
}