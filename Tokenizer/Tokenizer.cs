using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PicTokenizer
{
    public class Tokenizer : ICloneable
    {
        protected class TokenDefinition
        {
            public readonly string type;
            private readonly Regex regex;

            public TokenDefinition(string type, string regex)
            {
                this.type = type;
                this.regex = new Regex(regex, RegexOptions.Compiled);
            }

            public TokenDefinition(KeyValuePair<string, string> def) : this(def.Key, def.Value) { }

            public MatchCollection GetMatches(string input) => regex.Matches(input);
        }

        protected bool InverseAdd { get; private set; }
        protected readonly List<TokenDefinition> tokenDefinitions;

        public Tokenizer(bool inverseAdd = false)
        {
            InverseAdd = inverseAdd;
            tokenDefinitions = new List<TokenDefinition>();
        }

        public Tokenizer(Dictionary<string, string> tokenDefinitions, bool inverseAdd = false) : this(inverseAdd)
        {
            foreach (var def in tokenDefinitions)
            {
                WithToken(def);
            }
        }

        private Tokenizer(IEnumerable<TokenDefinition> tokenDefinitions, bool inverseAdd)
        {
            InverseAdd = inverseAdd;
            this.tokenDefinitions = new List<TokenDefinition>(tokenDefinitions);
        }

        public Tokenizer WithToken(string type, string regex)
        {
            if (InverseAdd)
                tokenDefinitions.Insert(0, new TokenDefinition(type, regex));
            else
                tokenDefinitions.Add(new TokenDefinition(type, regex));
            return this;
        }

        public Tokenizer WithToken(KeyValuePair<string, string> tokenDefinition)
        {
            if (InverseAdd)
                tokenDefinitions.Insert(0, new TokenDefinition(tokenDefinition));
            else
                tokenDefinitions.Add(new TokenDefinition(tokenDefinition));
            return this;
        }

        public Tokenizer WithoutToken(string type)
        {
            foreach (TokenDefinition td in tokenDefinitions)
            {
                if (td.type == type)
                {
                    tokenDefinitions.Remove(td);
                    return this;
                }
            }
            return this;
        }

        public IEnumerable<Token> Tokenize(string input)
        {
            bool[] occupied = new bool[input.Length];
            List<Token> tokens = new List<Token>();

            foreach (TokenDefinition definition in tokenDefinitions)
            {
                foreach (Token token in TokenizeInternal(input, occupied, definition))
                {
                    tokens.Add(token);
                }
            }

            return tokens.OrderBy(t => t.Position).ToArray();
        }

        protected static IEnumerable<Token> TokenizeInternal(string input, bool[] occupied, TokenDefinition tokenDefinition)
        {
            foreach (Match match in tokenDefinition.GetMatches(input))
            {
                if (!match.Success)
                    continue;

                var indexRange = Enumerable.Range(match.Index, match.Length).ToList();
                if (indexRange.Any(idx => occupied[idx]))
                    continue;

                indexRange.ForEach(idx => occupied[idx] = true);

                yield return new Token(tokenDefinition.type, match.Value, match.Index);
            }
        }

        public object Clone()
        {
            return new Tokenizer(tokenDefinitions, InverseAdd);
        }
    }
}
