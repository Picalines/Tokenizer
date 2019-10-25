using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PicTokenizer
{
    public class Tokenizer<T>
    {
        protected class TokenDefinition
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

        protected bool InverseAdd { get; private set; }
        protected readonly List<TokenDefinition> tokenDefinitions;

        public Tokenizer()
        {
            tokenDefinitions = new List<TokenDefinition>();
            InverseAdd = false;
        }

        public Tokenizer(bool inverseAdd) : this()
        {
            InverseAdd = inverseAdd;
        }

        public Tokenizer<T> WithToken(T type, params string[] regexes)
        {
            if (InverseAdd)
                foreach (string regex in regexes) tokenDefinitions.Insert(0, new TokenDefinition(type, regex));
            else
                foreach (string regex in regexes) tokenDefinitions.Add(new TokenDefinition(type, regex));
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

        protected static IEnumerable<IToken<T>> TokenizeInternal(string input, bool[] occupied, TokenDefinition tokenDefinition)
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
