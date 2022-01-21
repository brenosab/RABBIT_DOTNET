using ADM_RT_CORE_LIB.Messaging;
using ADM_RT_CORE_LIB.Messaging.Interfaces;
using ADM_RT_UTILIDADES.Worker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ADM_RT_UTILIDADES
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
            services.AddOptions();

            services.AddSingleton<IMessageBus>(
                new MessageBus(
                    Environment.GetEnvironmentVariable("APPLICATION"),
                    Environment.GetEnvironmentVariable("APP_RABBIT_HOST"),
                    int.Parse(Environment.GetEnvironmentVariable("APP_RABBIT_PORT")),
                    Environment.GetEnvironmentVariable("APP_RABBIT_USER"),
                    Environment.GetEnvironmentVariable("APP_RABBIT_PASSWORD"))
                );

            services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "ApiSensores",
                    Description = "Api para consumir filas do RABBITMQ"
                });
            });

            services.AddControllers();
            services.AddTransient<IHostedService, SensorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiSensores v1"));
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
