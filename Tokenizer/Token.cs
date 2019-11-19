using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace PicTokenizer
{
    public interface IReadOnlyToken : ICloneable
    {
        string Type { get; }
        string Value { get; }
        int Position { get; }
    }

    public class Token : IReadOnlyToken
    {
        public string Type  { get; }
        public string Value { get; set; }
        public int Position { get; set; }

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

        public override bool Equals(object obj)
        {
            if (obj is Token b)
            {
                return this == b;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Token a, Token b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null || b is null)
            {
                return false;
            }
            return a.Type == b.Type && a.Value == b.Value && a.Position == b.Position;
        }

        public static bool operator !=(Token a, Token b)
        {
            return !(a == b);
        }
    }

    public class TokenDefinition : ICloneable
    {
        public string Type { get; }
        public Regex Regex { get; }

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

        public override bool Equals(object obj)
        {
            if (obj is TokenDefinition b)
            {
                return this == b;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TokenDefinition a, TokenDefinition b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if (a == null || b == null)
            {
                return false;
            }
            return a.Type == b.Type && a.Regex == b.Regex;
        }

        public static bool operator !=(TokenDefinition a, TokenDefinition b)
        {
            return !(a == b);
        }
    }
}
