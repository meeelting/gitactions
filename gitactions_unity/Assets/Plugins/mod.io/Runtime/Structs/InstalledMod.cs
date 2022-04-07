using System.Collections.Generic;

namespace ModIO
{
    public struct InstalledMod
    {
        public List<string> subscribedUsers;
        public bool updatePending;
        public string directory;
        public ModProfile modProfile;
    }
}
