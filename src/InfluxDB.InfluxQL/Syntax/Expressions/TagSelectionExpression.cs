namespace InfluxDB.InfluxQL.Syntax.Expressions
{
    public class TagSelectionExpression : ColumnSelectionExpression
    {
        public TagSelectionExpression(string name, string alias) : base(alias)
        {
            TagName = name;
        }

        public string TagName { get; }

        public override string ToString()
        {
            var identifier = TagName;

            if (identifier.Contains(" "))
            {
                identifier = $"\"{identifier}\"";
            }

            if (TagName != Alias)
            {
                return $"{identifier} AS {Alias}";
            }

            return identifier;
        }
    }
}