namespace PicTokenizer
{
    public class Token
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

        public static implicit operator bool(Token token)
        {
            return token != null;
        }
    }
}
