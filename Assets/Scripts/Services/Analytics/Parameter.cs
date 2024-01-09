namespace Services.Analytics
{
    public struct Parameter
    {
        public readonly string Name;
        public readonly string Value;
        public readonly int IntValue;
        
        public Parameter(string name, int value)
        {
            Name = name;
            IntValue = value;
            Value = null;
        }

        public Parameter(string name, string value)
        {
            Name = name;
            Value = value;
            IntValue = 0;
        }
    }
}