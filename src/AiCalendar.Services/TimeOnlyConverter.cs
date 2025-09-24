using Newtonsoft.Json;

namespace AiCalendar.Services;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("HH:mm:ss"));
    }

    public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is string str)
        {
            return TimeOnly.Parse(str);
        }



        throw new JsonSerializationException($"Unable to convert {reader.Value} to TimeOnly");
    }
}