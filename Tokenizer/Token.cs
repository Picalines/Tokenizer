using System.Text.RegularExpressions;

namespace PicTokenizer
{
    public class Token
    {
        public readonly string Type;
        public readonly string Value;
        public readonly int Position;

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
    }

    public class TokenDefinition
    {
        public string Type { get; }
        public Regex Regex { get; }

        public TokenDefinition(string type, string regex, RegexOptions options = RegexOptions.Compiled)
        {
            Type = type;
            Regex = new Regex(regex, options);
        }

        public static implicit operator TokenDefinition((string type, string regex) def)
        {
            return new TokenDefinition(def.type, def.regex);
        }

        public static implicit operator TokenDefinition((string type, string regex, RegexOptions options) def)
        {
            return new TokenDefinition(def.type, def.regex, def.options);
        }
    }
}
