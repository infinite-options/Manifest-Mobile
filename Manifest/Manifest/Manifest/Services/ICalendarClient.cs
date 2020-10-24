using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Manifest.Services
{
    public interface ICalendarClient
    {
        Task<List<Event>> GetEventsList(DateTimeOffset dateTimeOffset);
    }
}
