using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule.Interfaces;

namespace AutoSgin
{
    public class SystemTime : IUtcTime
    {
        public DateTime Now => DateTime.Now;
    }
}
