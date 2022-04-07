#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using ModIO.Implementation;
using ModIO.Implementation.Platform;

namespace ModIO.EditorCode
{
    /// <summary>summary</summary>
    public static class EditorMenu
    {
        static EditorMenu()
        {
            new MenuItem("Tools/mod.io/Edit Settings", false, 0);
        }

        [MenuItem("Tools/mod.io/Edit Settings", false, 0)]
        public static void EditSettingsAsset()
        {
            var settingsAsset = Resources.Load<SettingsAsset>(SettingsAsset.FilePath);

            if(settingsAsset == null)
            {
                // create asset
                settingsAsset = ScriptableObject.CreateInstance<SettingsAsset>();

                UnityEditor.AssetDatabase.CreateAsset(
                    settingsAsset, $@"Assets/Resources/{SettingsAsset.FilePath}.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }

            UnityEditor.EditorGUIUtility.PingObject(settingsAsset);
            UnityEditor.Selection.activeObject = settingsAsset;
        }

        [MenuItem("Tools/mod.io/Debug/Clear Data", false, 0)]
        public static void ClearStoredData()
        {
            SystemIOWrapper.DeleteDirectory(EditorDataService.GlobalRootDirectory);
        }
    }
}
#endif
