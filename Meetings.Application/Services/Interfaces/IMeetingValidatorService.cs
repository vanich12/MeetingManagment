using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meetings.Core.Models;

namespace Meetings.Application.Services.Interfaces
{
    public interface IMeetingValidatorService
    {
        /// <summary>
        /// Валидация
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        Task ValidASync(Meeting meeting, bool isUpdate = false);
    }
}