namespace AiCalendar.Services;

public static class Constants
{
    public const string GptModelVersion = "gpt-5";
    public const string Instructions =
        """
        You are a calendar event parser. Parse the provided unstructured text into structured calendar events.

        Current date context will be provided for calculating relative dates and nearest day occurrences.

        Extract the following information for each event:
        - Name: Short, descriptive title of the event
        - StartDateTime: Date and time of the event. If no time specified, then assume it's 00:00:00
        - EndDateTime: ONLY if event spans across multiple days. Otherwise use null date 2000-01-01T00:00:00Z. If EndDateTime is specified and not null date, then Duration should be PT0S
        - IsAllDayEvent: Set to true if no specific time is mentioned, otherwise false
        - Duration: Use ISO 8601 duration format (PT2H, PT30M). If explicitly mentioned (e.g., "2 hours", "30 minutes"), use that value. If not specified, set to PT1H
        - Location: Event location
        - Description: Event description.

        Rules:
        1. If no year is mentioned, assume current year
        2. If no month is mentioned, assume current month
        3. If no time is specified, treat as all-day event
        4. Parse relative dates (today, tomorrow, next week, etc.) based on current context
        5. If only a day of the week is mentioned (Monday, Tuesday, etc.), assume the nearest upcoming occurrence of that day
        6. Extract multiple events if the text describes several activities
        7. Be conservative with duration - only set if explicitly stated
        8. Location should be specific addresses or venue names, not general descriptions
        9. Events may be in other languages (Russian, Polish, etc.) - preserve the original language in event names and locations

        Return valid JSON matching the CalendarResponse schema with Events array.
        
        {0}
        """;
}