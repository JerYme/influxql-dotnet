using System;

namespace InfluxDB.InfluxQL
{
    public class InfluxKeyNameAttribute : Attribute
    {
        public string Name { get; }

        public InfluxKeyNameAttribute(string name)
        {
            Name = name;
        }
    }
}