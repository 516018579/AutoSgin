using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSgin.Services;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Panda.DynamicWebApi;
using Swashbuckle.AspNetCore.Swagger;

namespace AutoSgin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // TODO:一定要返回true！
                c.DocInclusionPredicate((docName, description) => true);

            });

            services.AddDynamicWebApi();

            services.AddScheduler();

            var mutex = new InMemoryMutex();
            mutex.Using(new SystemTime());
            services.AddSingleton<IMutex>(mutex);

            services.AddSingleton(typeof(ISginService), typeof(SginService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

           

            app.ApplicationServices.UseScheduler(scheduler =>
            {
                scheduler.ScheduleAsync(() =>
                {
                    var sginService = app.ApplicationServices.GetService<ISginService>();
                    return sginService.SginAll();
                })
                .DailyAtHour(8);
            });
        }
    }
}
