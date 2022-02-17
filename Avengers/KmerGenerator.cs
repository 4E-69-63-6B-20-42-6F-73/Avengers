namespace Avengers
{
    public class KmerGenerator
    {
        public IEnumerable<string> Generate(string read, int k)
        {
            for (int i = 0; i <= read.Length - k; i++)
            {
                yield return read.Substring(i, k);
            }

        }
    }
}