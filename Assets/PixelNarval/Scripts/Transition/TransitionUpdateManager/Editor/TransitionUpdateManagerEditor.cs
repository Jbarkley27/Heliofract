using UnityEngine;
using UnityEditor;

namespace PixelNarval.HPBars
{
    public partial class TransitionDataEditor
    {
        [CustomEditor(typeof(TransitionUpdateManager)), CanEditMultipleObjects]
        public class TransitionUpdateManagerEditor : Editor
        {
            SerializedProperty stoppedProperty;
            SerializedProperty simulateOnEditorProperty;
            SerializedProperty childTransitionsProperty;
            SerializedProperty transitionConfigProperty;

            private void OnEnable()
            {
                
            }

            public override void OnInspectorGUI()
            {
                serializedObject.UpdateIfRequiredOrScript();
                EditorGUI.indentLevel++;

                DrawPropertiesExcluding(serializedObject, "m_Script");
                
                EditorGUI.indentLevel--;
                serializedObject.ApplyModifiedProperties();

            }       
        }
    }
}