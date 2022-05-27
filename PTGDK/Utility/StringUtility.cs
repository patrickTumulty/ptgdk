using System;
using System.Linq;

namespace PTGDK.Utility
{
    public static class StringUtility
    {
        public static bool EqualsCaseInsensitive(string s1, string s2)
        {
            return s1.ToLower().Equals(s2.ToLower());
        }
    }
}