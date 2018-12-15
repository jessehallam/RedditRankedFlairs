using System;
using System.Linq;

namespace RedditFlairs.Core.Utility
{
    public class ValidationUtility
    {
        private static readonly char[] CodeCharacters =
            Enumerable.Range('A', 1 + 'Z' - 'A')
                .Concat(Enumerable.Range('0', 1 + '9' - '0'))
                .Select(num => (char) num)
                .ToArray();

        private const int CodeLength = 5;

        [ThreadStatic] private static Random randomInstance;

        private static Random Random => randomInstance ?? (randomInstance = new Random());

        public static string CreateValidationCode()
        {
            var chars = new char[CodeLength];

            for (var i = 0; i < CodeLength; i++)
            {
                chars[i] = CodeCharacters[Random.Next(0, CodeCharacters.Length)];
            }

            return new string(chars).ToUpperInvariant();
        }
    }
}