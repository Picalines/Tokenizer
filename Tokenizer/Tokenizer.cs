using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PicTokenizer
{
    public class Tokenizer<T> : ICloneable where T : IComparable<T>
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

        public Tokenizer(bool inverseAdd = false)
        {
            tokenDefinitions = new List<TokenDefinition>();
            InverseAdd = inverseAdd;
        }

        private Tokenizer(IEnumerable<TokenDefinition> tokenDefinitions, bool inverseAdd)
        {
            this.tokenDefinitions = new List<TokenDefinition>(tokenDefinitions);
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

        public Tokenizer<T> WithoutToken(T type)
        {
            foreach (TokenDefinition td in tokenDefinitions)
            {
                if (td.type.CompareTo(type) == 0)
                {
                    tokenDefinitions.Remove(td);
                    return this;
                }
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

        public object Clone()
        {
            return new Tokenizer<T>(tokenDefinitions, InverseAdd);
        }
    }
}
