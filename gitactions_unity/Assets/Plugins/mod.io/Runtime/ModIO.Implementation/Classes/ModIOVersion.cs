namespace ModIO.Implementation
{
    /// <summary>Describes the mod.io UnityPlugin version.</summary>
    internal struct ModIOVersion : System.IComparable<ModIOVersion>
    {
        // ---------[ Singleton ]---------
        /// <summary>Singleton instance for current version.</summary>
        public static readonly ModIOVersion Current = new ModIOVersion(3, 0, "-dev");

        // ---------[ Fields ]---------
        /// <summary>Major version number.</summary>
        public int version;

        /// <summary>Version build number.</summary>
        public int build;

        /// <summary>Suffix for the current version.</summary>
        public string suffix;

        // ---------[ Initialization ]---------
        /// <summary>Constructs an object with the given version values.</summary>
        public ModIOVersion(int version = 0, int build = 0, string suffix = null)
        {
            this.version = version;
            this.build = build;

            if(suffix == null)
            {
                suffix = string.Empty;
            }
            this.suffix = suffix;
        }

        // ---------[ IComparable Interface ]---------
        /// <summary>Compares the current instance with another ModIOVersion.</summary>
        public int CompareTo(ModIOVersion other)
        {
            int result = this.version.CompareTo(other.version);

            if(result == 0)
            {
                result = this.build.CompareTo(other.build);
            }

            return result;
        }

#region Operator Overloads

        // clang-format off
        public static bool operator > (ModIOVersion a, ModIOVersion b)
        {
            return a.CompareTo(b) == 1;
        }

        public static bool operator < (ModIOVersion a, ModIOVersion b)
        {
            return a.CompareTo(b) == -1;
        }

        public static bool operator >= (ModIOVersion a, ModIOVersion b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <= (ModIOVersion a, ModIOVersion b)
        {
            return a.CompareTo(b) <= 0;
        }
        // clang-format on

#endregion // Operator Overloads

#region Utility

        /// <summary>Creates the request header representation of the version.</summary>
        public string ToHeaderString()
        {
            return $"modioUnity-{this.version.ToString()}.{this.build.ToString()}{this.suffix}";
        }

#endregion // Utility
    }
}
