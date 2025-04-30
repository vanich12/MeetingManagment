namespace Meetings.Application.Extensions
{
    public static class DateTimeExtensions
    {
        private const string DefaultDisplayFormat = "yyyy-MM-dd HH:mm"; 
        private static readonly TimeZoneInfo _targetDisplayTimeZone;

        /// <summary>
        /// Настройка часового пояса
        /// </summary>
        static DateTimeExtensions()
        {
            TimeSpan targetOffset = TimeSpan.FromHours(4);
            string timeZoneId = "Custom_UTC+4"; 

            _targetDisplayTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                id: timeZoneId,
                baseUtcOffset: targetOffset,
                displayName: $"UTC{targetOffset.Hours:+00;-00}:00", 
                standardDisplayName: timeZoneId 
            );
        }


        /// <summary>
        /// Безопасно конвертирует DateTime в UTC.
        /// Если Kind == Utc, возвращает без изменений.
        /// Если Kind == Local, конвертирует в UTC.
        /// Если Kind == Unspecified, предполагает, что это локальное время, и конвертирует в UTC.
        /// </summary>
        /// <param name="dt">Исходный DateTime.</param>
        /// <returns>DateTime с Kind=Utc.</returns>
        public static DateTime ToUtcSafe(this DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc)
            {
                return dt;
            }
            if (dt.Kind == DateTimeKind.Local)
            {
                return dt.ToUniversalTime();
            }
            return DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime();
        }


        /// <summary>
        /// Конвертирует UTC DateTime в целевую временную зону приложения (заданную в _targetDisplayTimeZone)
        /// и форматирует её в виде строки.
        /// </summary>
        /// <param name="utcDateTime">DateTime для форматирования (предполагается, что Kind=Utc).</param>
        /// <param name="format">Необязательная строка формата. Если null, используется DefaultDisplayFormat.</param>
        /// <returns>Отформатированная строка времени в целевой зоне или UTC с пометкой при ошибке.</returns>
        public static string FormatForDisplay(this DateTime utcDateTime, string? format = null)
        {

            if (utcDateTime.Kind == DateTimeKind.Local)
                utcDateTime = utcDateTime.ToUniversalTime();
     

            try
            {
                DateTime timeInTargetZone = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _targetDisplayTimeZone);
                return timeInTargetZone.ToString(format ?? DefaultDisplayFormat);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка форматирования времени {utcDateTime} для отображения: {ex.Message}");
                return utcDateTime.ToString(format ?? DefaultDisplayFormat) + $" ({TimeZoneInfo.Utc.StandardName})";
            }
        }

        /// <summary>
        /// Перегрузка для форматирования nullable DateTime.
        /// </summary>
        /// <param name="utcDateTime">Nullable DateTime для форматирования.</param>
        /// <param name="format">Необязательная строка формата.</param>
        /// <param name="valueIfNull">Строка, возвращаемая, если DateTime равен null.</param>
        /// <returns>Отформатированная строка или valueIfNull.</returns>
        public static string FormatForDisplay(this DateTime? utcDateTime, string? format = null, string valueIfNull = "N/A")
        {
            return utcDateTime.HasValue ? utcDateTime.Value.FormatForDisplay(format) : valueIfNull;
        }
    }

}
