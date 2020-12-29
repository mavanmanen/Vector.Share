namespace Vector.Share.Providers.Random
{
    public class RandomProvider : IRandomProvider
    {
        public IRandom GetRandom() => new Random();
    }
}