using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace BackendJPMAnalysis.JsonConverters
{
    public class SnakeCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return ToSnakeCase(propertyName);
        }

        /// <summary>
        /// The ToSnakeCase function converts a given string to snake_case format.
        /// </summary>
        /// <param name="str">The `ToSnakeCase` method you provided converts a given string to snake
        /// case. Snake case is a naming convention where spaces are replaced by underscores and each
        /// word is in lowercase.</param>
        /// <returns>
        /// The method `ToSnakeCase` takes a string as input and converts it to snake case. It returns
        /// the input string converted to snake case.
        /// </returns>
        private string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            var snakeCase = char.ToLower(str[0]).ToString();

            for (var i = 1; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsUpper(c))
                {
                    snakeCase += "_";
                    snakeCase += char.ToLower(c);
                }
                else
                {
                    snakeCase += c;
                }
            }

            return snakeCase;
        }
    }


    public class SnakeCasePropertyNamesContractResolverJsonConverter : JsonConverter
    {
        private readonly SnakeCasePropertyNamesContractResolver _resolver;

        public SnakeCasePropertyNamesContractResolverJsonConverter()
        {
            _resolver = new SnakeCasePropertyNamesContractResolver();
        }

        /// <summary>
        /// The function writes JSON properties for a given object using a JSON writer and serializer in
        /// C#.
        /// </summary>
        /// <param name="JsonWriter">JsonWriter is a class in Json.NET that provides a way to write JSON
        /// data to a stream. It is used to write JSON data in a forward-only manner.</param>
        /// <param name="value">The `value` parameter in the `WriteJson` method represents the object
        /// that needs to be serialized into JSON format. This object will be converted into a JSON
        /// object by iterating over its properties and writing them to the JSON writer.</param>
        /// <param name="JsonSerializer">The JsonSerializer parameter in the WriteJson method is used to
        /// serialize the property values of the object being written to JSON format. It provides
        /// methods to serialize objects into JSON and write the JSON representation to a JsonWriter. In
        /// the provided code snippet, the serializer.Serialize method is used to serialize the property
        /// values before</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contract = _resolver.ResolveContract(value.GetType()) as JsonObjectContract;
            writer.WriteStartObject();
            foreach (var property in contract.Properties)
            {
                var propertyValue = property.ValueProvider.GetValue(value);
                writer.WritePropertyName(property.PropertyName);
                serializer.Serialize(writer, propertyValue);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// The function reads JSON data and sets properties of an object using snake case property
        /// names.
        /// </summary>
        /// <param name="JsonReader">The `JsonReader` parameter in the `ReadJson` method is used to read
        /// JSON data from a `JsonReader` stream. It provides methods to read various JSON tokens like
        /// properties, arrays, and values from the JSON input. You can use the `JsonReader` to navigate
        /// through the JSON</param>
        /// <param name="Type">The `Type` parameter in the `ReadJson` method represents the type of the
        /// object being deserialized. It specifies the type to which the JSON should be
        /// deserialized.</param>
        /// <param name="existingValue">The `existingValue` parameter in the `ReadJson` method
        /// represents the existing value of the object being deserialized. It is the object that is
        /// being populated with the deserialized values from the JSON. If an existing object is passed
        /// to the deserializer, it will attempt to populate this object with</param>
        /// <param name="JsonSerializer">The JsonSerializer parameter in the ReadJson method is used to
        /// deserialize JSON data into .NET objects. It provides methods for reading JSON data from a
        /// JsonReader and converting it into the specified object type. The JsonSerializer parameter
        /// also allows you to customize the deserialization process by providing settings and
        /// configurations.</param>
        /// <returns>
        /// The method is returning the deserialized object after setting the property values in snake
        /// case format.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = serializer.Deserialize(reader, objectType);
            if (obj == null) return null;

            var contract = _resolver.ResolveContract(obj.GetType()) as JsonObjectContract;
            if (contract != null)
            {
                foreach (var property in contract.Properties)
                {
                    var snakeCasePropertyName = _resolver.GetResolvedPropertyName(property.PropertyName);
                    var propertyValue = reader.Value;
                    property.ValueProvider.SetValue(obj, propertyValue);
                }
            }

            return obj;
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}