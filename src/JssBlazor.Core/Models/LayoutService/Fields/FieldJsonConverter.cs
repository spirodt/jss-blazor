using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JssBlazor.Core.Models.LayoutService.Fields
{
    public class FieldJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jToken = JToken.Load(reader);
            if (jToken.Type == JTokenType.Array)
            {
                var converter = new FieldValueJsonConverter();
                var multilistField = new MutlilistField
                {
                    Value = (MultilistFieldValue)converter.ReadJson(jToken.CreateReader(), objectType, existingValue, serializer)
                };
                return multilistField;
            }

            if (jToken.Type == JTokenType.Object)
            {
                if (jToken.SelectToken("value.href") != null)
                {
                    return jToken.ToObject<LinkField>();
                }
                if (jToken.SelectToken("value.src") != null)
                {
                    if (jToken.SelectToken("value.displayName") != null)
                    {
                        return jToken.ToObject<FileField>();
                    }
                    return jToken.ToObject<ImageField>();
                }
            }
            return jToken.ToObject<Field>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is MutlilistField multilistField)
            {
                serializer.Serialize(writer, multilistField.Value.RawValue);
                return;
            }
            serializer.Serialize(writer, value);
        }
    }
}
