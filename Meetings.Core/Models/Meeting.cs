namespace Meetings.Core.Models
{
    /// <summary>
    /// Встреча
    /// </summary>
    public class Meeting
    {
        /// <summary>
        /// Персональный идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Время начала
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Время конца
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// Время напоминания
        /// </summary>
        public DateTime? Reminder { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
    }
}
