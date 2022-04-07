using UnityEngine;

namespace ModIO.Implementation
{
    /// <summary>Switch extension to the SettingsAsset.</summary>
    internal partial class SettingsAsset : ScriptableObject
    {
        /// <summary>Configuration for Switch.</summary>
        public BuildSettings switchConfiguration;

#if UNITY_SWITCH && !UNITY_EDITOR

        /// <summary>Gets the configuration for Switch.</summary>
        public BuildSettings GetBuildSettings()
        {
            return this.switchConfiguration;
        }

#endif // UNITY_SWITCH && !UNITY_EDITOR
    }
}
