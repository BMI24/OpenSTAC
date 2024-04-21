using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    internal class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(Vector3))
                property.ShouldSerialize = instance => false;

            return property;
        }
    }
    internal class Serializer
    {
        static JsonSerializerSettings Settings = new()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = ShouldSerializeContractResolver.Instance
        };
        public static string Serialize<T>(T calculatorViewModel)
        {
            return JsonConvert.SerializeObject(calculatorViewModel, Formatting.Indented, Settings);
        }

        public static T? Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static T Clone<T>(T obj)
        {
            var str = Serialize(obj);
            return Deserialize<T>(str)!;
        }
    }
}
