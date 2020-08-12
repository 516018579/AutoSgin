using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoSgin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.UTF8;
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Console.WriteLine(Encoding.GetEncoding("GB2312"));
            //Console.WriteLine("Æô¶¯");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
