using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MeetingManagment.Utilities;
using Meetings.Application.Extensions;
using Meetings.Application.Services.Interfaces;
using Meetings.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;
using Index = Microsoft.EntityFrameworkCore.Metadata.Internal.Index;

namespace MeetingManagment
{
    public class MeetingHandler(IMeetingService meetingService, ILogger logger)
    {
        public async Task RunAsync()
        {
            Console.WriteLine("Приложение запущено");
            bool keepRunning = true;

            // по хорошему нужно воспользоваться планировщиком, но SqlLite будет работать с ним плохоЮ поэтому сделал так
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await meetingService.CheckReminders();
                    await Task.Delay(20000); // Ждем 20 секунд перед каждой проверкой уведомлений
                }
            });


            while (keepRunning)
            {
                DisplayMenu();
                Console.Write("Введите ваш выбор: ");
                string? choice = Console.ReadLine().Trim();
                Console.WriteLine(); // Пустая строка для разделения
                try
                {
                    keepRunning = await HandleUserChoiceAsync(choice);
                }
                catch (Exception e)
                {
                    logger.Warning($"Некорректный ввод {e.Message}");
                }

                if (keepRunning)
                {
                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine("Выход из приложения...");
        }

        public void DisplayMenu()
        {
            Console.WriteLine("================ Меню ================");
            Console.WriteLine("1. Добавить встречу");
            Console.WriteLine("2. Обновить встречу (по ID)");
            Console.WriteLine("3. Удалить встречу (по ID)");
            Console.WriteLine("4. Показать все встречи");
            Console.WriteLine("5. Показать встречи по на конкретные дни");
            Console.WriteLine("6. Экспортировать расписание в meetings.txt"); // Уточнил имя файла
            Console.WriteLine("7. Выход");
            Console.WriteLine("====================================");
        }

        private async Task<bool> HandleUserChoiceAsync(string? choice)
        {
            switch (choice)
            {
                case "1":
                    await AddMeetingAsync();
                    break;
                case "2":
                    await UpdateMeetingAsync();
                    break;
                case "3":
                    await DeleteMeetingAsync();
                    break;
                case "4":
                    await ListAllMeetingsAsync();
                    break;
                case "5":
                    await ShowMeetingByDateAsync();
                    break;
                case "6":
                    await ExportMeetingsAsync();
                    break;
                case "7":
                    return false;
                default:
                    Console.WriteLine("Неверный ввод. Пожалуйста, выберите опцию из меню.");
                    break;
            }

            return true;
        }

        private async Task AddMeetingAsync()
        {
            try
            {
                DateTime startTime = GetDateTimeValue("Введите время начала встречи:");
                DateTime endTime = GetDateTimeValue("Введите примерное время окончания встречи:");
                Console.WriteLine("Введите описание встречи");
                string description = Console.ReadLine().Trim();
                Meeting meeting = new Meeting() { StartTime = startTime, EndTime = endTime, Description = description };
                Console.WriteLine("Установить уведомление о встрече?");
                Console.WriteLine("1. Да");
                Console.WriteLine("2. Нет");
                var choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1":
                        await SetReminderTime(meeting, "Введите время для напоминания");
                        break;
                    case "2":
                        break;
                    default:
                        Console.WriteLine("Выберите вариант 1 либо 2");
                        return;
                }

                await meetingService.CreateAsync(meeting);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Неверный формат ввода");
                logger.Warning($"Ошибка : {ex.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Окончание встречи неможет быть раньше начала");
                logger.Warning($"Ошибка : {e.Message}");
            }
        }

        private async Task SetReminderTime(Meeting? meet, string prompt)
        {
            try
            {
                var reminderTime = GetDateTimeValue(prompt).ToUtcSafe();
                meet.Reminder = reminderTime;
                await meetingService.CreateAsync(meet);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Неверный формат ввода");
                logger.Warning($"Ошибка : {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                logger.Error($"Передан не верный аргумент: {ex.Message}.");
            }
            catch (Exception e)
            {
                logger.Error($"Не удалось создать встерчу с ошибкой: {e.Message}.");
            }
        }

        private async Task UpdateMeetingAsync()
        {
            var meetings = await ListAllMeetingsAsync();
            if (meetings.Count() == 0)
                return;

            bool isCorrectIndex = true;
            Meeting currentMeeting = null;
            int meetingId = -1;

            while (currentMeeting == null)
            {
                Console.Write("Введите ID встречи, которую хотите отредактировать: ");
                string? inputId = Console.ReadLine().Trim();

                if (int.TryParse(inputId, out meetingId))
                {
                    try
                    {
                        currentMeeting = await meetingService.GetByIdAsync(meetingId);
                        if (currentMeeting == null)
                        {
                            Console.WriteLine($"Встреча с ID {meetingId} не найдена.");
                            logger.Warning("Meeting with ID {MeetingId} not found (returned null).", meetingId);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ошибка при поиске встречи с ID {meetingId}.");
                        logger.Error(e, "Ошибка в при выборке по идентификатору {MeetingId}: {ErrorMessage}", meetingId,
                            e.Message);
                        currentMeeting = null;
                    }
                }
                else
                {
                    Console.WriteLine("Некорректный ввод ID. Пожалуйста, введите целое число.");
                }
            }

            bool isOpenForm = true;

            Console.WriteLine("Выберите, что вы хотите изменить:");
            Console.WriteLine("1. Время начала встречи");
            Console.WriteLine("2. Время окончания встречи");
            Console.WriteLine("3. Время напоминания");
            Console.WriteLine("4. Содержание встречи");
            Console.WriteLine("5. Сохранить");
            Console.WriteLine("6. Отменить");
            try
            {
                while (isOpenForm)
                {
                    Console.WriteLine("Выберите, что вы хотите изменить:");
                    string choice = Console.ReadLine().Trim();
                    switch (choice)
                    {
                        case "1":
                            var newTime = GetDateTimeValue("Введите новое время НАЧАЛА встречи:");
                            currentMeeting.StartTime = newTime;
                            break;
                        case "2":
                            var endTime = GetDateTimeValue("Введите новое время ОКОНЧАНИЯ встречи:");
                            currentMeeting.EndTime = endTime;
                            break;
                        case "3":
                            var remindTime = GetDateTimeValue("Введите новое время для напоминания:");
                            currentMeeting.Reminder = remindTime;
                            break;
                        case "4":
                            Console.WriteLine("Введите новое содежрание встречи");
                            var description = Console.ReadLine().Trim();
                            currentMeeting.Description = description;
                            break;
                        case "5":
                            await meetingService.UpdateAsync(meetingId, currentMeeting);
                            isOpenForm = false;
                            break;
                        case "6":
                            isOpenForm = false;
                            break;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Неверный формат ввода");
                logger.Warning($"Ошибка : {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                logger.Error($"Передан не верный аргумент: {ex.Message}.");
            }
            catch (Exception e)
            {
                logger.Warning("Встреча с ID {MeetingId} не найдена (returned null).", meetingId);
            }
        }

        private async Task DeleteMeetingAsync()
        {
            try
            {
                var meetings = await ListAllMeetingsAsync();

                if (meetings.Count() == 0)
                    return;

                Console.Write("Введите номер встречи которую хотите удалить: ");
                int index = int.Parse(Console.ReadLine().Trim());
                var removeItem = await meetingService.GetByIdAsync(index);

                await meetingService.RemoveAsync(removeItem);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine("Встречи с таким номером не найдено");
                logger.Warning($"Ошибка : {e.Message}");
            }
            catch (Exception e)
            {
                logger.Warning($"Ошибка : {e.Message}");
            }
        }

        private async Task<IEnumerable<Meeting>?> ListAllMeetingsAsync()
        {
            //DateTime date = GetDateTimeValue("На какую дату вы хотите посмотреть встречи?:");
            var meetings = await meetingService.GetAllAsync();
            foreach (var meeting in meetings)
            {
                Console.WriteLine(
                    $"{meeting.Id}: Встреча с {meeting.StartTime.FormatForDisplay()} по {meeting.EndTime.FormatForDisplay()} с описанием: {meeting.Description}");
            }

            if (meetings.Count() == 0)
                Console.WriteLine("Встреч не найдено");
            Console.WriteLine();

            return meetings;
        }

        private async Task ShowMeetingByDateAsync()
        {
            DateTime targetLocalDate;
            Console.Write($"Введите дату для экспорта в формате - гггг-ММ-дд: ");
            string? dateInput = Console.ReadLine().Trim();
            if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out targetLocalDate))
            {
                Console.WriteLine("Неверный формат даты. Пожалуйста, используйте формат 'гггг-ММ-дд'.");
                return;
            }

            var filteredMeetings = await GetMeetingsByDateOrDefault(targetLocalDate);

            foreach (var meeting in filteredMeetings)
            {
                Console.WriteLine(
                    $"{meeting.Id}: Встреча с {meeting.StartTime.FormatForDisplay()} по {meeting.EndTime.FormatForDisplay()}");
            }
        }

        private async Task<IEnumerable<Meeting>?> GetMeetingsByDateOrDefault(DateTime targetLocalDate)
        {
            DateTime localStartOfDay = targetLocalDate.Date;
            DateTime localEndOfDay = localStartOfDay.AddDays(1);
            DateTime utcStartOfDay = localStartOfDay.ToUtcSafe();
            DateTime utcEndOfDay = localEndOfDay.ToUtcSafe();

            Expression<Func<Meeting, bool>> expression = meeting =>
                meeting.StartTime >= utcStartOfDay && meeting.StartTime < utcEndOfDay;

            IEnumerable<Meeting> filteredMeetings;
            try
            {
                filteredMeetings = await meetingService.GetAsync(expression);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logger.Warning(" Встреч на данную дату не найдено.");
                return null;
            }

            return filteredMeetings;
        }

        private async Task ExportMeetingsAsync()
        {
            Console.WriteLine("--- Экспорт встреч за указанную дату ---");

            // 1. Получаем целевую ЛОКАЛЬНУЮ дату от пользователя
            DateTime targetLocalDate;
            Console.Write($"Введите дату для экспорта в формате - гггг-ММ-дд: ");
            string? dateInput = Console.ReadLine().Trim();

            if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out targetLocalDate))
            {
                Console.WriteLine("Неверный формат даты. Пожалуйста, используйте формат 'гггг-ММ-дд'.");
                return;
            }

            var filteredMeetings = await GetMeetingsByDateOrDefault(targetLocalDate);
            if (filteredMeetings.Count() == 0)
            {
                Console.WriteLine("Встречь не найдено");
                return;
            }

            Console.WriteLine("Введите название файла");
            string fileName = Console.ReadLine().Trim();
            WorkWithFiles.ExportMeetingsToTextFile(filteredMeetings, fileName);
            Console.WriteLine("Расписание экспортировано.");
        }

        private DateTime GetDateTimeValue(string prompt, bool fileExport = false)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine().Trim(), out DateTime newDateTime))
            {
                return fileExport ? newDateTime : newDateTime.ToUtcSafe();
            }
            else
            {
                throw new InvalidOperationException("Неверный формат даты и времени.");
            }
        }
    }
}