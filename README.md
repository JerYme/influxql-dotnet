A type safe way to query data from influxdb.


Can build up [InfluxQL](https://docs.influxdata.com/influxdb/v1.3/query_language/spec/) statements using fluent syntax.
```
var query = InfluxQuery.From(h2o_feet)
	.Select(fields => new { fields.water_level })
	.Where("location = 'santa_monica'");

query.SelectStatement.Text.ShouldBe("SELECT water_level FROM h2o_feet WHERE location = 'santa_monica'");
```

```
var query = InfluxQuery.From(h2o_feet)
	.Select(fields => new { mean = MEAN(fields.water_level) })
	.GroupBy(tags => new { tags.location });

query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet GROUP BY location");
```
