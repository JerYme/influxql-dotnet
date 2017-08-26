namespace InfluxDB.InfluxQL.Syntax.Expressions
{
    public class FieldAggregationExpression
    {
        public FieldAggregationExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}