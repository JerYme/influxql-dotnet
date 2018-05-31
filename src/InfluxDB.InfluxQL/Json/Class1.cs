using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InfluxDB.InfluxQL.Json
{

    public interface IJsonSerializer
    {

    }

    class JsonSerializerImpl : IJsonSerializer
    {
        private readonly JsonSerializer serialiser = JsonSerializer.CreateDefault();
    }

    public interface IJsonSerializerFactory
    {
        IJsonSerializer Create();
    }
}
