using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace PicTokenizer
{
    public class Tokenizer
    {
        public readonly TokenDefinition[] TokenDefinitions;

        public Tokenizer(params TokenDefinition[] tokenDefinitions)
        {
            TokenDefinitions = tokenDefinitions;
        }

        public Token[] Tokenize(string input)
        {
            bool[] occupied = new bool[input.Length];
            List<Token> tokens = new List<Token>();

            foreach (TokenDefinition definition in TokenDefinitions)
            {
                foreach (Token token in TokenizeInternal(input, occupied, definition))
                {
                    tokens.Add(token);
                }
            }

            return tokens.OrderBy(t => t.Position).ToArray();
        }

        private static IEnumerable<Token> TokenizeInternal(string input, bool[] occupied, TokenDefinition tokenDefinition)
        {
            foreach (Match match in tokenDefinition.Regex.Matches(input))
            {
                if (!match.Success)
                    continue;

                var indexRange = Enumerable.Range(match.Index, match.Length).ToList();
                if (indexRange.Any(idx => occupied[idx]))
                    continue;

                indexRange.ForEach(idx => occupied[idx] = true);

                yield return new Token(tokenDefinition.Type, match.Value, match.Index);
            }
        }
    }
}
