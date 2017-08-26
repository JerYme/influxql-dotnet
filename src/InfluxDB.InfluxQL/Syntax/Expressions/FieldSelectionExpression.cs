namespace InfluxDB.InfluxQL.Syntax.Expressions
{
    public class FieldSelectionExpression : ColumnSelectionExpression
    {
        public FieldSelectionExpression(string name, string alias) : base(alias)
        {
            FieldName = name;
        }

        public FieldSelectionExpression(FieldSelectionExpression selection, FieldAggregationExpression aggregation) : base((string) selection.Alias)
        {
            FieldName = selection.FieldName;
            Aggregation = aggregation;
        }

        public string FieldName { get; }

        public FieldAggregationExpression Aggregation { get; }

        public override string ToString()
        {
            var identifier = FieldName;

            if (identifier.Contains(" "))
            {
                identifier = $"\"{identifier}\"";
            }

            if (Aggregation != null)
            {
                identifier = $"{Aggregation.Name}({identifier})";
            }

            if (FieldName != Alias)
            {
                return $"{identifier} AS {Alias}";
            }

            return identifier;
        }
    }
}