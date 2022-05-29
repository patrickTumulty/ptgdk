
using System;

namespace PTGDK.Utility
{
    public static class NumberUtils
    {
        /// <summary>
        /// Compare two doubles to check for equality within a certain amount of precision.
        /// </summary>
        /// <param name="a">double a</param>
        /// <param name="b">double b</param>
        /// <param name="precision">number of digits to compare</param>
        /// <returns>true if a and b are equal</returns>
        public static bool DoubleEquals(double a, double b, int precision = 5)
        {
            int multiplier = (int) Math.Pow(10, precision);
            return (int) (a * multiplier) == (int) (b * multiplier);
        }
    }
}