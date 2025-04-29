using Meetings.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Application.Extensions;

namespace MeetingManagment.Utilities
{
    public static class WorkWithFiles
    {
        public static void ExportMeetingsToTextFile(IEnumerable<Meeting> meetingsOnDate, string fileName)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            string solutionPath = Path.GetFullPath(Path.Combine(directoryPath, @"..\..\"));

            string folderPath = Path.Combine(solutionPath, "Встречи");
            Directory.CreateDirectory(folderPath);

            if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                fileName += ".txt";

            string filePath = Path.Combine(folderPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var meeting in meetingsOnDate)
                {
                    writer.WriteLine(
                        $"{(meeting.Description is null ? "Встреча" : meeting.Description)} начинается в  {meeting.StartTime.FormatForDisplay()} заканчивается в {meeting.EndTime.FormatForDisplay()}");
                }
            }
        }
    }
}