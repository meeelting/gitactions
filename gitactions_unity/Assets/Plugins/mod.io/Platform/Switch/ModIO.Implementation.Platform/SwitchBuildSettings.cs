#if UNITY_SWITCH || (MODIO_COMPILE_ALL && UNITY_EDITOR)

using System.Collections.Generic;

namespace ModIO.Implementation.Platform
{
    /// <summary>Indicies for the BuildSettings.extData in use on Switch.</summary>
    public static class SwitchBuildSettings
    {
        // - Contstants -
        public const int EXTDATAINDEX_USERDATAMOUNT = 0;
        public const int EXTDATAINDEX_PERSISTENTDATAMOUNT = 1;
        public const int EXTDATAINDEX_TEMPDATAMOUNT = 2;

        // - User Data Mount Point -
        public static string GetSwitchUserDataMountPoint(this ref BuildSettings settings)
        {
            if(settings.extData != null
               && settings.extData.Count > SwitchBuildSettings.EXTDATAINDEX_USERDATAMOUNT)
            {
                return settings.extData[SwitchBuildSettings.EXTDATAINDEX_USERDATAMOUNT];
            }

            return null;
        }

        public static void SetSwitchUserDataMountPoint(this ref BuildSettings settings,
                                                       string mountPoint)
        {
            if(settings.extData == null)
            {
                settings.extData = new List<string>();
            }

            while(settings.extData.Count < SwitchBuildSettings.EXTDATAINDEX_USERDATAMOUNT)
            {
                settings.extData.Add(null);
            }

            settings.extData[SwitchBuildSettings.EXTDATAINDEX_USERDATAMOUNT] = mountPoint;
        }

        // - Persistent Data Mount Point -
        public static string GetSwitchPersistentDataMountPoint(this ref BuildSettings settings)
        {
            if(settings.extData != null
               && settings.extData.Count > SwitchBuildSettings.EXTDATAINDEX_PERSISTENTDATAMOUNT)
            {
                return settings.extData[SwitchBuildSettings.EXTDATAINDEX_PERSISTENTDATAMOUNT];
            }

            return null;
        }

        public static void SetSwitchPersistentDataMountPoint(this ref BuildSettings settings,
                                                             string mountPoint)
        {
            if(settings.extData == null)
            {
                settings.extData = new List<string>();
            }

            while(settings.extData.Count < SwitchBuildSettings.EXTDATAINDEX_PERSISTENTDATAMOUNT)
            {
                settings.extData.Add(null);
            }

            settings.extData[SwitchBuildSettings.EXTDATAINDEX_PERSISTENTDATAMOUNT] = mountPoint;
        }

        // - Temporary Data Mount Point -
        public static string GetSwitchTempDataMountPoint(this ref BuildSettings settings)
        {
            if(settings.extData != null
               && settings.extData.Count > SwitchBuildSettings.EXTDATAINDEX_TEMPDATAMOUNT)
            {
                return settings.extData[SwitchBuildSettings.EXTDATAINDEX_TEMPDATAMOUNT];
            }

            return null;
        }

        public static void SetSwitchTempDataMountPoint(this ref BuildSettings settings,
                                                       string mountPoint)
        {
            if(settings.extData == null)
            {
                settings.extData = new List<string>();
            }

            while(settings.extData.Count < SwitchBuildSettings.EXTDATAINDEX_TEMPDATAMOUNT)
            {
                settings.extData.Add(null);
            }

            settings.extData[SwitchBuildSettings.EXTDATAINDEX_TEMPDATAMOUNT] = mountPoint;
        }
    }
}

#endif
