namespace PicTokenizer
{
    public interface IToken<T>
    {
        T type { get; }
        string value { get; }
        int position { get; }
    }

    public class Token<T> : IToken<T>
    {
        public T type { get; }
        public string value { get; }
        public int position { get; }

        public Token(T type, string value, int position)
        {
            this.type = type;
            this.value = value;
            this.position = position;
        }
    }

    public class Token : IToken<string>
    {
        public string type { get; }
        public string value { get; }
        public int position { get; }

        public Token(string type, string value, int position)
        {
            this.type = type;
            this.value = value;
            this.position = position;
        }
    }
}
