namespace InfluxDB.InfluxQL.Syntax.Expressions
{
    public abstract class ColumnSelectionExpression
    {
        protected ColumnSelectionExpression(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; }
    }
}