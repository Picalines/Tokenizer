using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace PicTokenizer
{
    public class Token : ICloneable
    {
        public string Type  { get; }
        public string Value { get; }
        public int Position { get; }

        public Token(string type, string value, int position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        public override string ToString()
        {
            return Value;
        }

        public object Clone()
        {
            return new Token(Type, Value, Position);
        }

        public static implicit operator bool(Token token)
        {
            return token != null;
        }
    }

    public class TokenDefinition : ICloneable
    {
        public readonly string Type;
        public readonly Regex Regex;

        public TokenDefinition(string type, string regex)
        {
            Type = type;
            Regex = new Regex(regex, RegexOptions.Compiled);
        }

        public TokenDefinition(KeyValuePair<string, string> def) : this(def.Key, def.Value) { }

        public override string ToString()
        {
            return Type;
        }

        public object Clone()
        {
            return new TokenDefinition(Type, Regex.ToString());
        }

        public static implicit operator TokenDefinition((string Type, string Regex) tokenDefinition)
        {
            return new TokenDefinition(tokenDefinition.Type, tokenDefinition.Regex);
        }
    }
}
