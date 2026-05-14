using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PixelNarval.HPBars
{
    [CustomEditor(typeof(TransitionData)), CanEditMultipleObjects]
    public partial class TransitionDataEditor : Editor
    {
        protected static Dictionary<string, bool> openDic = new Dictionary<string, bool>();

        private string documentationFileName = "Pixelnarval_TransitionComponets_Asset_Guide";
        private string documentationRoute = "";
        private static string webpage = "";

        SerializedProperty eventValueCommon;
        SerializedProperty currentValue;
        SerializedProperty targetValue;
        SerializedProperty lastValue;
        SerializedProperty timer;
        SerializedProperty percentage;

        TransitionData transitionStatus;
        protected GUIContent docContent;

        public GUIContent status = new GUIContent("Transition Data Debug");
        public static GUIContent titleContent;
        public static GUIContent web;
        public override bool HasPreviewGUI() => true;
        public override GUIContent GetPreviewTitle() => status;

        public static GUIStyle titleStyle; 


        private void OnEnable()
        {
            eventValueCommon = serializedObject.FindProperty("eventValueCommon");
            currentValue = serializedObject.FindProperty("currentValue");
            targetValue = serializedObject.FindProperty("targetValue");
            lastValue = serializedObject.FindProperty("lastValue");
            timer = serializedObject.FindProperty("timer");
            percentage = serializedObject.FindProperty("percentage");

            docContent = EditorGUIUtility.TrIconContent("TextAsset Icon", "General Documentation");
            titleContent = EditorGUIUtility.TrTextContent("Transition Data");
            web = EditorGUIUtility.TrIconContent("ToolHandleGlobal", "Webpage");

            documentationRoute =  AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(documentationFileName)[0]);
            documentationRoute = Path.Combine(Application.dataPath.Replace('/', Path.DirectorySeparatorChar), documentationRoute.Replace("Assets/", "").Replace('/', Path.DirectorySeparatorChar)); 
            transitionStatus = target as TransitionData;
            

        }

        public override void OnInspectorGUI()
        {
            DrawCustomHeader();

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(eventValueCommon);
            eventValueCommon.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                transitionStatus.CommonDataChangedEvent?.Invoke(transitionStatus);
            }

            EditorGUILayout.PropertyField(lastValue);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(currentValue);
            currentValue.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                transitionStatus.targetValue.FloatValue = transitionStatus.currentValue.FloatValue;
            }
            EditorGUILayout.PropertyField(targetValue);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(timer);
            Rect r = EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(r, percentage.floatValue, "Transition Percentage: " + (int)(percentage.floatValue * 100));
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCustomHeader()
        {
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = (int)(20),
            };

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(docContent, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f), GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.5f)))
            {
                OpenDocumentation();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label(titleContent, titleStyle);
            GUILayout.FlexibleSpace();

            //if (GUILayout.Button(web, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f), GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.5f)))
            //{
            //    OpenWebPage();
            //}
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            EditorGUI.indentLevel++;
            Rect eventValueCommonRect = new Rect(r.x, r.y, r.width, EditorGUIUtility.singleLineHeight);
            eventValueCommonRect.height = EditorGUI.GetPropertyHeight(eventValueCommon);


            eventValueCommon.isExpanded = true;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(eventValueCommonRect, eventValueCommon, true);

            if (EditorGUI.EndChangeCheck())
            {
                eventValueCommon.serializedObject.ApplyModifiedProperties();
                transitionStatus.CommonDataChangedEvent?.Invoke(transitionStatus);
            }
            eventValueCommon.isExpanded = true;

            Rect lastValueRect = eventValueCommonRect;
            lastValueRect.height = EditorGUI.GetPropertyHeight(lastValue);
            lastValueRect.position += eventValueCommonRect.height * Vector2.up;
            Rect currentValueRect = lastValueRect;
            currentValueRect.height = EditorGUI.GetPropertyHeight(currentValue);
            currentValueRect.position += lastValueRect.height * Vector2.up;
            Rect targetValueRect = currentValueRect;
            targetValueRect.height = EditorGUI.GetPropertyHeight(targetValue);
            targetValueRect.position += currentValueRect.height * Vector2.up;

            EditorGUI.PropertyField(lastValueRect, lastValue, true);

            EditorGUI.PropertyField(currentValueRect, currentValue, true);

            EditorGUI.PropertyField(targetValueRect, targetValue, true);

            Rect debugLabelRect = targetValueRect;
            debugLabelRect.height = EditorGUIUtility.singleLineHeight;
            debugLabelRect.position += targetValueRect.height * Vector2.up;

            EditorGUI.LabelField(debugLabelRect, "Transition Debug");

            GUI.enabled = false;
            Rect timerRect = debugLabelRect;
            timerRect.height = EditorGUIUtility.singleLineHeight;
            timerRect.position += debugLabelRect.height * Vector2.up;
            EditorGUI.PropertyField(timerRect, timer);

            Rect percentageRect = timerRect;
            percentageRect.height = EditorGUIUtility.singleLineHeight;
            percentageRect.position += timerRect.height * Vector2.up;
            EditorGUI.ProgressBar(percentageRect, percentage.floatValue, "" + (int)(percentage.floatValue * 100));
            EditorGUI.LabelField(percentageRect, "Transition Percentage:");
            GUI.enabled = true;

            EditorGUI.indentLevel--;
        }

        public override void DrawPreview(Rect previewArea)
        {
            OnInteractivePreviewGUI(previewArea, "PreBackground");
        }

        protected virtual void OpenDocumentation()
        {
            System.Diagnostics.Process.Start(documentationRoute);
        }

        protected virtual void OpenWebPage()
        {
            Application.OpenURL(webpage);
        }
    }
}