using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PicTokenizer
{
    public interface IReadOnlyTokenizer : ICloneable
    {
        IEnumerable<Token> Tokenize(string input);
    }

    public class Tokenizer : IReadOnlyTokenizer
    {
        protected readonly List<TokenDefinition> tokenDefinitions;

        public Tokenizer()
        {
            tokenDefinitions = new List<TokenDefinition>();
        }

        public Tokenizer(Dictionary<string, string> tokenDefinitions, bool inverseAdd = false) : this()
        {
            foreach (var def in tokenDefinitions)
            {
                WithToken(def, inverseAdd);
            }
        }

        private Tokenizer(IEnumerable<TokenDefinition> tokenDefinitions)
        {
            this.tokenDefinitions = new List<TokenDefinition>(tokenDefinitions);
        }

        public Tokenizer WithToken(string type, string regex, bool inverseAdd = false)
        {
            if (inverseAdd)
                tokenDefinitions.Insert(0, new TokenDefinition(type, regex));
            else
                tokenDefinitions.Add(new TokenDefinition(type, regex));
            return this;
        }

        public Tokenizer WithToken(KeyValuePair<string, string> tokenDefinition, bool inverseAdd = false)
        {
            return WithToken(tokenDefinition.Key, tokenDefinition.Value, inverseAdd);
        }

        public Tokenizer WithToken(TokenDefinition tokenDefinition, bool inverseAdd = false)
        {
            return WithToken(tokenDefinition.Type, tokenDefinition.Regex.ToString(), inverseAdd);
        }

        public Tokenizer WithoutToken(string type)
        {
            foreach (TokenDefinition td in tokenDefinitions)
            {
                if (td.Type == type)
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

        public object Clone()
        {
            return new Tokenizer(tokenDefinitions);
        }
    }
}
