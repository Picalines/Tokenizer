using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PicTokenizer
{
    public sealed class Tokenizer<T>
    {
        private class TokenDefinition
        {
            public readonly T type;
            private readonly Regex regex;

            public TokenDefinition(T type, string regex)
            {
                this.type = type;
                this.regex = new Regex(regex, RegexOptions.Compiled);
            }

            public MatchCollection GetMatches(string input) => regex.Matches(input);
        }

        private readonly List<TokenDefinition> tokenDefinitions = new List<TokenDefinition>();

        public Tokenizer<T> WithToken(T type, params string[] regexes)
        {
            foreach (string regex in regexes)
            {
                tokenDefinitions.Add(new TokenDefinition(type, regex));
            }
            return this;
        }

        public IToken<T>[] Tokenize(string input)
        {
            bool[] occupied = new bool[input.Length];
            List<IToken<T>> tokens = new List<IToken<T>>();

            foreach (TokenDefinition definition in tokenDefinitions)
            {
                foreach (IToken<T> token in TokenizeInternal(input, occupied, definition))
                {
                    tokens.Add(token);
                }
            }

            return tokens.OrderBy(t => t.position).ToArray();
        }

        private static IEnumerable<IToken<T>> TokenizeInternal(string input, bool[] occupied, TokenDefinition tokenDefinition)
        {
            foreach (Match match in tokenDefinition.GetMatches(input))
            {
                if (!match.Success)
                    continue;

                var indexRange = Enumerable.Range(match.Index, match.Length).ToList();
                if (indexRange.Any(idx => occupied[idx]))
                    continue;

                indexRange.ForEach(idx => occupied[idx] = true);

                yield return new Token<T>(tokenDefinition.type, match.Value, match.Index);
            }
        }
    }
}
