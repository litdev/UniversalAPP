using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniversalAPP.MongoDB.Models
{
    public class Demo : BaseModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
