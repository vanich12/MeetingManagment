namespace Meetings.Core.Models
{
    public class Meeting
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public DateTime? Reminder { get; set; }

        public string? Description { get; set; }
    }
}
