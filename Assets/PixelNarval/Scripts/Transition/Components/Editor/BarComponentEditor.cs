using UnityEditor;
using UnityEngine;

namespace PixelNarval.HPBars
{
    [CustomEditor(typeof(TransitionComponent), editorForChildClasses:true), CanEditMultipleObjects]
    public class BarComponentEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "order", "m_Script" };
        SerializedProperty order;
        TransitionComponent transitionComponent;

        private void OnEnable()
        {
            order = serializedObject.FindProperty("order");
            transitionComponent = (target as TransitionComponent);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.PropertyField(order);
            int newValue = EditorGUILayout.IntField("Execution Order", order.intValue);
            order.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                transitionComponent.ChangeOrder(newValue);
            }

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);


            serializedObject.ApplyModifiedProperties();

        }
    }
}