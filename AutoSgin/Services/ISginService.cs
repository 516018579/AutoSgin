using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSgin.Services
{
    public interface ISginService : IApplicationService
    {
        Task SginAll();
        Task Sgin(string sginUrl, string cookieValue);
    }
}
