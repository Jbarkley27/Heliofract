using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace PixelNarval.HPBars
{
    public static class IconsUtils
    {
        public static void LoadIcon(string classScriptString, string iconName, bool isTexture = false)
        {
            if (UnityEditor.EditorPrefs.HasKey(classScriptString) && UnityEditor.EditorPrefs.GetString(classScriptString) == iconName)
            {
                return;
            }

            if (string.IsNullOrEmpty(iconName))
            {
                Debug.LogWarning("IconName is not set.");
                return;
            }

            UnityEditor.MonoImporter monoImporter = null;

            string[] guids = UnityEditor.AssetDatabase.FindAssets(classScriptString + " t:Script");
            if (guids.Length == 0)
            {
                Debug.LogWarning("Can't find script. Was it renamed or moved?");
                return;
            }
            else if (guids.Length > 1)
            {
                foreach (string guid in guids)
                {

                    string scriptPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    UnityEditor.MonoImporter monoImporter2 = UnityEditor.MonoImporter.GetAtPath(scriptPath) as UnityEditor.MonoImporter;

                    if (monoImporter2 != null)
                    {
                        UnityEditor.MonoScript monoScript = monoImporter2.GetScript();
                        var a = monoScript.GetClass();
                        if (monoScript != null && a != null && a.Name == classScriptString)
                        {
                            monoImporter = monoImporter2;
                            break;
                        }
                    }
                }
                if (monoImporter == null)
                {

                    Debug.LogWarning("MonoImporter not found");
                    return;
                }
            }
            else
            {
                string scriptPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                monoImporter = UnityEditor.AssetImporter.GetAtPath(scriptPath) as UnityEditor.MonoImporter;
                if (monoImporter == null)
                {
                    Debug.LogWarning("MonoImporter not found for script: " + scriptPath);
                    return;
                }
            }
            Texture2D icon;
            if (!isTexture)
            {
                var iconContent = UnityEditor.EditorGUIUtility.IconContent(iconName);
                if (iconContent == null)
                {
                    Debug.LogWarning("Icon not found: " + iconName);
                    return;
                }

                icon = (Texture2D)iconContent.image;
            }
            else
            {
                string[] textureGuids = UnityEditor.AssetDatabase.FindAssets(iconName + " t:Texture2D");
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Can't Texture. Was it renamed or moved?");
                    return;
                }
                else if (textureGuids.Length > 1)
                {
                    // Need to add this check
                    return;
                }
                else
                {
                    icon = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath(textureGuids[0]));
                }
            }


            Texture2D actualIcon = monoImporter.GetIcon();
            if (actualIcon != null && actualIcon.name == icon.name) // No need to change
            {
                return;
            }

            monoImporter.SetIcon(icon);
            UnityEditor.EditorPrefs.SetString(classScriptString, iconName);
            monoImporter.SaveAndReimport();
        }
    }
}
#endif