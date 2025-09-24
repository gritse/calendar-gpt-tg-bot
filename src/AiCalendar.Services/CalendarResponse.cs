using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AiCalendar.Services;
using Newtonsoft.Json;
using NJsonSchema.Annotations;
using TimeOnlyConverter = AiCalendar.Services.TimeOnlyConverter;

namespace AiCalendarTelegramBot;

public class CalendarResponse
{
    [Required]
    public Event[] Events { get; init; }
}

public class Event
{
    [Required]
    public required string Name { get; init; }

    [Required, DataType(DataType.DateTime)]
    public DateTimeOffset StartDateTime { get; init; }

    [Required, DataType(DataType.DateTime)]
    public DateTimeOffset EndDateTime { get; init; }

    [Required]
    public bool IsAllDayEvent { get; init; }

    [JsonConverter(typeof(Iso8601TimeSpanConverter))]
    [Required, DataType(DataType.Duration)]
    public required TimeSpan Duration { get; init; }

    [Required(AllowEmptyStrings = true)]
    public required string Location { get; init; }

    [Required(AllowEmptyStrings = true)]
    public required string Description { get; init; }
}