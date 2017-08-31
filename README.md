# InfluxQL.net [![Build status](https://ci.appveyor.com/api/projects/status/krvvi1yjk6ci6aje/branch/master?svg=true)](https://ci.appveyor.com/project/gambrose/influxql-dotnet/branch/master) [![NuGet Version](http://img.shields.io/nuget/v/InfluxQL.net.svg?style=flat)](https://www.nuget.org/packages/InfluxQL.net/)

A C# client for querying data from [InfluxDB](https://www.influxdata.com/) in a type safe way.

By building up [InfluxQL](https://docs.influxdata.com/influxdb/v1.3/query_language/spec/) statements using a fluent syntax not only is valid, properly escaped, InfluxQL produced the types are tracked through and SELECT or GROUP BY projections.

## Building InfluxQL statements using fluent syntax.
```csharp
var query = InfluxQuery.From(h2o_feet).Select(fields => fields);

// Note escaping and alias for field "level description"
query.Statement.Text.ShouldBe("SELECT \"level description\" AS level_description, water_level FROM h2o_feet");
```

We can do `SELECT` projection to just return the water_level.

```csharp
var waterLevelQuery = InfluxQuery.From(h2o_feet).Select(fields => new { fields.water_level });

waterLevelQuery.Statement.Text.ShouldBe("SELECT water_level FROM h2o_feet");
```

Retrive the results from Influx.

```csharp
var client = new InfluxQLClient(config.Endpoint, "NOAA_water_database");

var results = await client.Query(waterLevelQuery.Statement);

foreach (var (time, values) in results)
{
    Console.WriteLine($"{time} {values.water_level}");
}
```
Note the strong typing for returning a single series of points based on query statement passed in to `Query` method.

 The type retuned in the results from issuing this query is an instance of the anonymous type we define in the select statement meaning tring to reference level_description in the results will cause an complie error.


Doing a `GROUP BY` will retun multiple series:

```csharp
using static InfluxDB.InfluxQL.Aggregations;
...

var query = InfluxQuery.From(h2o_feet)
	.Select(fields => new { mean = MEAN(fields.water_level) })
	.GroupBy(tags => new { tags.location });

query.Statement.Text.ShouldBe("SELECT MEAN(water_level) AS mean FROM h2o_feet GROUP BY location");

var results = await client.Query(query.Statement);

foreach (var series in results)
{
    foreach (var point in series.Points)
    {
        Console.WriteLine($"{series.Tags.location}, {point.values.mean}, {point.time});
    }
}
```
We are importing `InfluxDB.InfluxQL.Aggregations` static class so we can use the `MEAN` function without prefixing. We could have written `Aggregations.MEAN((fields.water_level)`.