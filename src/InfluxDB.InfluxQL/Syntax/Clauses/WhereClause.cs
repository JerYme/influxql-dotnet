namespace InfluxDB.InfluxQL.Syntax.Clauses
{
    public class WhereClause
    {
        private readonly string predicate;

        public WhereClause(string predicate)
        {
            this.predicate = predicate;
        }

        public override string ToString()
        {
            return $"WHERE {predicate}";
        }
    }
}