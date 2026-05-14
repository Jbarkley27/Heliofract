using System.Collections;
using UnityEngine;
using UnityEditor;

namespace PixelNarval.HPBars
{

    [CustomPropertyDrawer(typeof(TransitionEventValue))]
    public class TransitionEventValueDrawer : DrawerCommon
    {
        public GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
        public Texture texture;
        Color yellow = Color.yellow;
        Color white = Color.white;
        Color paleYellow = new Color(0.5f, 0.5f, 0);

        bool debug = false;

        SerializedProperty floatValueProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            floatValueProperty = property.FindPropertyRelative("floatValue");

            TransitionEventValue eventValue = GetValue(property) as TransitionEventValue;
            int lines = 0;
            int identationBackup = EditorGUI.indentLevel;

            Rect sliderRect = GetRectAtLine(position, lines++);
            Rect intRect = sliderRect;
            intRect.xMin = intRect.xMax - 50;
            Rect progressbarRect = sliderRect;
            int sliderBoxWidth = 56;
            progressbarRect.xMax -= 104;

            EditorGUI.indentLevel = 0;
            float floatValue = 0;
            EditorGUI.BeginProperty(position, label, property);


            /////// -------------------- Slider
            sliderRect.xMax = sliderRect.xMax - 50;
            EditorGUI.BeginChangeCheck();
            floatValueProperty.serializedObject.UpdateIfRequiredOrScript();
            EditorGUI.Slider(sliderRect, floatValueProperty, 0, 1, GUIContent.none); // Float Slider
            floatValueProperty.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                floatValue = eventValue.FloatValue;
                foreach (var targetTransitionEventValue in floatValueProperty.serializedObject.targetObjects)
                {
                    TransitionEventValue currentTarget = (GetValue(targetTransitionEventValue, property.propertyPath) as TransitionEventValue);
                    if (eventValue.CommonData.maxValue > 0 && eventValue.CommonData.snapToInt)
                    {
                        currentTarget.AdjustedFloatValue = floatValue;
                    }
                    else
                    {
                        currentTarget.FloatValue = floatValue;
                    }

                }
            }
            EditorGUI.ProgressBar(progressbarRect, eventValue.FloatValue, $"{property.displayName}");


            ///// ----------- Snap points
            if (eventValue.CommonData.maxValue > 0 && eventValue.CommonData.snapToInt)
            {
                Rect adjustedRect = sliderRect;
                adjustedRect.xMax = sliderRect.xMax - sliderBoxWidth;
                adjustedRect.xMin = adjustedRect.xMin + (eventValue.AdjustedFloatValue * adjustedRect.width);
                adjustedRect.width = 2;
                EditorGUI.DrawRect(adjustedRect, yellow);

                Rect prevRect = adjustedRect;
                if (eventValue.IntValue > 0)
                {
                    prevRect.position -= new Vector2((sliderRect.width - sliderBoxWidth) / eventValue.CommonData.maxValue, 0);
                    EditorGUI.DrawRect(prevRect, paleYellow);
                }

                Rect nextRect = adjustedRect;
                if (eventValue.IntValue < eventValue.CommonData.maxValue)
                {
                    nextRect.position += new Vector2((sliderRect.width - sliderBoxWidth) / eventValue.CommonData.maxValue, 0);
                    EditorGUI.DrawRect(nextRect, paleYellow);
                }
                
            }

            /////// -------------------- Int
            if (eventValue.CommonData.maxValue > 0)
            {
                EditorGUI.indentLevel = 0;
                EditorGUI.BeginChangeCheck();
                GUI.contentColor = yellow;
                int intValue = EditorGUI.IntField(intRect, eventValue.IntValue); // IntField
                GUI.contentColor = white;
                if (EditorGUI.EndChangeCheck() && intValue != eventValue.IntValue)
                {
                    eventValue.IntValue = intValue;
                }
                EditorGUI.indentLevel = identationBackup;
                sliderRect.xMax -= 50;
            }

            if (debug)
            {
                Rect debugEventsLine = GetRectAtLine(position, lines++);
                foreach (var item in eventValue.ValueChangedEvent.dic)
                {
                    EditorGUI.LabelField(GetRectAtLine(position, lines++), item.Key + ": " + item.Value.GetInvocationList().Length);

                }

            }

            EditorGUI.indentLevel = identationBackup;

            RefreshOnHeightChange(lines, property);
            EditorGUI.EndProperty();
        }
    }
}