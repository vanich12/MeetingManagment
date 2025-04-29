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

namespace MeetingManagment
{
    public class MeetingHandler(IMeetingService meetingService)
    {
        public async Task RunAsync()
        {
            Console.WriteLine("Приложение запущено");
            bool keepRunning = true;

            while (keepRunning)
            {
                DisplayMenu();
                Console.Write("Введите ваш выбор: ");
                string? choice = Console.ReadLine();
                Console.WriteLine(); // Пустая строка для разделения

                // Обрабатываем выбор асинхронно и решаем, продолжать ли цикл
                keepRunning = await HandleUserChoiceAsync(choice);

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
                    await ShowMeetingByIdAsync();
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
            DateTime startTime = GetDateTimeValue("Введите время начала встречи:");
            DateTime endTime = GetDateTimeValue("Введите примерное время окончания встречи:");
            Meeting meeting = new Meeting() { StartTime = startTime, EndTime = endTime };
            Console.WriteLine("Установить уведомление о встрече?");
            Console.WriteLine("1. Да");
            Console.WriteLine("2. Нет");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SetReminderTime(meeting, "Введите время для напоминания");
                    break;
                case "2":
                    break;
                default:
                    Console.WriteLine("Выберите вариант 1 либо 2");
                    return;
            }
        }

        private async void SetReminderTime(Meeting? meet, string prompt)
        {
            try
            {
                var reminderTime = GetDateTimeValue(prompt).ToUtcSafe();
                meet.Reminder = reminderTime;
                await meetingService.CreateAsync(meet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task UpdateMeetingAsync()
        {
            await ListAllMeetingsAsync();
            Console.Write("Введите номер встречи которую хотите отредактировать: ");
            // огранничить чтоб не вышло за пределы массива
            int index = int.Parse(Console.ReadLine());
            Meeting currentMeeting;
            try
            {
                currentMeeting = await meetingService.GetByIdAsync(index);
            }
            catch (Exception)
            {
                Console.WriteLine("Не найдено встречи под таким номером");
                throw;
            }

            bool isOpenForm = true;

            Console.WriteLine("Выберите, что вы хотите изменить:");
            Console.WriteLine("1. Время начала встречи");
            Console.WriteLine("2. Время окончания встречи");
            Console.WriteLine("3. Время напоминания");
            Console.WriteLine("4. Содержание встречи");
            Console.WriteLine("5. Сохранить");
            Console.WriteLine("6. Отменить");

            while (isOpenForm)
            {
                Console.WriteLine("Выберите, что вы хотите изменить:");
                string choice = Console.ReadLine();
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
                        var description = Console.ReadLine();
                        currentMeeting.Description = description;
                        break;
                    case "5":
                        await meetingService.UpdateAsync(index, currentMeeting);
                        isOpenForm = false;
                        break;
                    case "6":
                        isOpenForm = false;
                        break;
                }
            }
        }

        private async Task DeleteMeetingAsync()
        {
            await ListAllMeetingsAsync();
            Console.Write("Введите номер встречи которую хотите отредактировать: ");
            int index = int.Parse(Console.ReadLine());
            var removeItem = await meetingService.GetByIdAsync(index);
            if (removeItem is null)
            {
                Console.WriteLine("Такой встречи нет в базе");
                return;
            }

            await meetingService.RemoveAsync(removeItem);
        }

        private async Task ListAllMeetingsAsync()
        {
            //DateTime date = GetDateTimeValue("На какую дату вы хотите посмотреть встречи?:");
            var meetings = await meetingService.GetAllAsync();
            foreach (var meeting in meetings)
            {
                Console.WriteLine(
                    $"{meeting.Id}: Встреча с {meeting.StartTime.FormatForDisplay()} по {meeting.EndTime.FormatForDisplay()}");
            }

            Console.WriteLine();
        }

        private async Task ShowMeetingByIdAsync()
        {
        }

        private async Task ExportMeetingsAsync()
        {
            Console.WriteLine("--- Экспорт встреч за указанную дату ---");

            // 1. Получаем целевую ЛОКАЛЬНУЮ дату от пользователя
            DateTime targetLocalDate;
            Console.Write($"Введите дату для экспорта в формате - гггг-ММ-дд): "); // Подсказка формата даты
            string? dateInput = Console.ReadLine();

            if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out targetLocalDate))
            {
                Console.WriteLine("Неверный формат даты. Пожалуйста, используйте формат 'гггг-ММ-дд'.");
                return;
            }

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
                Console.WriteLine("В выбранную дату не существует встреч");
                throw;
            }

            Console.WriteLine("Введите название файла");
            string fileName = Console.ReadLine();
            WorkWithFiles.ExportMeetingsToTextFile(filteredMeetings, fileName);
            Console.WriteLine("Расписание экспортировано.");
        }

        private DateTime GetDateTimeValue(string prompt, bool fileExport = false)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime newDateTime))
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