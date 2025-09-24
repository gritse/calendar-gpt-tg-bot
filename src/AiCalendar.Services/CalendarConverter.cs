using AiCalendarTelegramBot;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace AiCalendar.Services;

public static class CalendarConverter
{
    private static readonly DateTimeOffset NullDate = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static string Serialize(CalendarResponse calendarResponse)
    {
        var calendar = new Calendar();

        foreach (var responseEvent in calendarResponse.Events)
        {
            var duration = responseEvent.Duration;

            var e = new CalendarEvent
            {
                Start = new CalDateTime(responseEvent.StartDateTime.DateTime),
                Duration = duration,
                IsAllDay = responseEvent.IsAllDayEvent,
                Summary = responseEvent.Name,
                Description = responseEvent.Description,
                Location = responseEvent.Location,
                Alarms =
                {
                    new Alarm()
                    {
                        Trigger = new Trigger(TimeSpan.FromMinutes(-60)),
                        Action = "DISPLAY",
                        Description = "Alarm"
                    }
                }
            };

            if (responseEvent.EndDateTime != NullDate)
            {
                e.End = new CalDateTime(responseEvent.EndDateTime.DateTime);
            }

            calendar.Events.Add(e);
        }

        var serializer = new CalendarSerializer();
        var serializedCalendar = serializer.SerializeToString(calendar);

        return serializedCalendar!;
    }
}