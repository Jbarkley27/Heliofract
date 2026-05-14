using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PixelNarval.HPBars
{
    [CustomPropertyDrawer(typeof(TransitionUpdateManagerConfig))]
    public class TransitionUpdateManagerConfigDrawer : DrawerCommon
    {
        SerializedProperty advancedConfig;
        SerializedProperty fillType;
        SerializedProperty timeFllType;
        SerializedProperty speedType;
        SerializedProperty duration;
        SerializedProperty proportional;
        SerializedProperty speedCurve;
        SerializedProperty fillCurve;
        SerializedProperty absoluteSpeed;
        SerializedProperty useDeltaTime;
        SerializedProperty percentageSpeed;

        private void Init(SerializedProperty property)
        {
            advancedConfig = property.FindPropertyRelative("advancedConfig");
            fillType = property.FindPropertyRelative("fillType");
            speedType = property.FindPropertyRelative("speedType");
            timeFllType = property.FindPropertyRelative("timeFllType");
            duration = property.FindPropertyRelative("duration");
            proportional = property.FindPropertyRelative("proportional");
            speedCurve = property.FindPropertyRelative("speedCurve");
            fillCurve = property.FindPropertyRelative("fillCurve");
            absoluteSpeed = property.FindPropertyRelative("absoluteSpeed");
            useDeltaTime = property.FindPropertyRelative("useDeltaTime");
            percentageSpeed = property.FindPropertyRelative("percentageSpeed");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            int lines = 0;
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(GetRectAtLine(position, lines++), proportional);

            advancedConfig.boolValue = EditorGUI.ToggleLeft(GetRectAtLine(position, lines++), advancedConfig.displayName, advancedConfig.boolValue);
            EditorGUI.indentLevel++;
            if (!advancedConfig.boolValue)
            {
                EditorGUI.PropertyField(GetRectAtLine(position, lines++), duration);
            }
            else
            {
                EditorGUI.PropertyField(GetRectAtLine(position, lines++), fillType);

                TransitionUpdateManagerConfig.fillTypeEnum fillTypeEnum = (TransitionUpdateManagerConfig.fillTypeEnum)fillType.enumValueIndex;
                switch (fillTypeEnum)
                {
                    case TransitionUpdateManagerConfig.fillTypeEnum.perTime:

                        TransitionUpdateManagerConfig.timeFillTypeEnum timeFllTypeEnum = (TransitionUpdateManagerConfig.timeFillTypeEnum)System.Enum.GetValues(typeof(TransitionUpdateManagerConfig.timeFillTypeEnum)).GetValue(timeFllType.enumValueIndex);

                        EditorGUI.PropertyField(GetRectAtLine(position, lines++), duration);
                        duration.floatValue = Mathf.Max(0, duration.floatValue);

                        EditorGUI.PropertyField(GetRectAtLine(position, lines++), timeFllType);
                        switch (timeFllTypeEnum)
                        {
                            case TransitionUpdateManagerConfig.timeFillTypeEnum.speedOverTime:
                                EditorGUI.CurveField(GetRectAtLine(position, lines++, 5), speedCurve, Color.blue, new Rect(0, 0, 1, 1));
                                break;
                            case TransitionUpdateManagerConfig.timeFillTypeEnum.fillOverTime:
                                EditorGUI.CurveField(GetRectAtLine(position, lines++, 5), fillCurve, Color.green, new Rect(0, 0, 1, 1));
                                break;
                            default:
                                break;
                        }
                        lines += 4;
                        break;

                    case TransitionUpdateManagerConfig.fillTypeEnum.perFrame:
                        EditorGUI.PropertyField(GetRectAtLine(position, lines++), speedType);
                        TransitionUpdateManagerConfig.speedFillTypeEnum speedFllTypeEnum = (TransitionUpdateManagerConfig.speedFillTypeEnum)System.Enum.GetValues(typeof(TransitionUpdateManagerConfig.speedFillTypeEnum)).GetValue(speedType.enumValueIndex);

                        switch (speedFllTypeEnum)
                        {
                            case TransitionUpdateManagerConfig.speedFillTypeEnum.absolute:
                                EditorGUI.PropertyField(GetRectAtLine(position, lines++, 1), absoluteSpeed);
                                break;
                            case TransitionUpdateManagerConfig.speedFillTypeEnum.percentage:
                                EditorGUI.PropertyField(GetRectAtLine(position, lines++, 1), percentageSpeed);
                                percentageSpeed.floatValue = Mathf.Max(percentageSpeed.floatValue, 0.005f);
                                break;
                            default:
                                break;
                        }
                        EditorGUI.PropertyField(GetRectAtLine(position, lines++, 1), useDeltaTime);

                        break;
                    default:
                        break;
                }
            }
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
            RefreshOnHeightChange(lines, property);
        }
    }
}