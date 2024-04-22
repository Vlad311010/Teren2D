
public static class Extensions
{
    public static float UnitInterval(this System.Random rng, int precision = 100000)
    {
        return (rng.Next(0, precision + 1)) / ((float)precision);
    }
}
