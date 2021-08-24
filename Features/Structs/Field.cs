namespace BanLogger.Features.Structs
{
    public struct Field
    {
        // idea by @SrLicht
        private readonly string _name;
        private readonly string _value;
        private readonly bool _inLine;

        public Field(string name, string value, bool inLine = true)
        {
            _name = name;
            _value = value;
            _inLine = inLine;
        }

        public override string ToString()
        {
            return _inLine ? $"{_name}\n```{_value}```" : $"\n{_name}\n```{_value}```";
        }
    }
}