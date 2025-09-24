namespace AiCalendar.Services;

using Newtonsoft.Json;
using System.Xml;

public class Iso8601TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
    {
        writer.WriteValue(XmlConvert.ToString(value));
    }

    public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is string str)
        {
            return XmlConvert.ToTimeSpan(str);
        }
        throw new JsonSerializationException($"Unable to convert {reader.Value} to TimeSpan");
    }
}

public class DateTimeOffsetSpanConverter : JsonConverter<DateTimeOffset?>
{
    public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override DateTimeOffset? ReadJson(JsonReader reader, Type objectType, DateTimeOffset? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is string str)
        {
        }

        throw new JsonSerializationException($"Unable to convert {reader.Value} to TimeSpan");
    }
}