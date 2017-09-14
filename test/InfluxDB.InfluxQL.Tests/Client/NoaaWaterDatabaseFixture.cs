using System;
using InfluxDB.InfluxQL.Client;

namespace InfluxDB.InfluxQL.Tests.Client
{
    public class NoaaWaterDatabaseFixture
    {
        public InfluxQLClient Client => new InfluxQLClient(new Uri("http://localhost:8086"), "NOAA_water_database");
    }
}