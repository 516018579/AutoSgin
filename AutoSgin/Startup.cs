using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSgin.DB;
using AutoSgin.Filters;
using AutoSgin.Services;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Panda.DynamicWebApi;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace AutoSgin
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IConfiguration configuration)
        {
            _appConfiguration = (IConfigurationRoot)configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite()
                .AddDbContext<SginDbContext>(options =>
                    {
                        options.UseSqlite(_appConfiguration.GetConnectionString("Default"));
                    });

            services.AddControllers(config => { config.Filters.Add<ApiExceptionFilter>(); });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // TODO:一定要返回true！
                c.DocInclusionPredicate((docName, description) => true);

            });

            services.AddDynamicWebApi();

            services.AddScheduler();

            services.AddSingleton<IMutex>(new InMemoryMutex());

            services.AddTransient(typeof(ISginService), typeof(SginService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

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
                .DailyAtHour(1);
            });
        }
    }
}
