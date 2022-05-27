namespace PTGDK.Utility
{
    public static class ConversionUtility
    {
        public static double InchesToMeters(double inches)
        {
            return inches * 0.0254f;
        }

        public static double MetersToInches(double meters)
        {
            return meters * 39.97f;
        }
    }
}