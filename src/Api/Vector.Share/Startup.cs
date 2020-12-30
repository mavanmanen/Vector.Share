using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Vector.Share.Configuration;
using Vector.Share.Data;
using Vector.Share.Providers.Random;
using Vector.Share.Providers.Timestamp;
using Vector.Share.Repositories;
using Vector.Share.Services;

namespace Vector.Share
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServerConfiguration>(_configuration.GetSection("Server"));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddLogging(options =>
            {
                options.AddConsole();
                options.AddDebug();
            });

            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vector.Share",
                    Version = "v1"
                });
            });

            services.AddDbContext<DatabaseContext>();
            services.AddScoped<IScheduledDeletionRepository, ScheduledDeletionRepository>();
            services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();
            
            services.AddSingleton<IRandomProvider, RandomProvider>();
            services.AddSingleton<ITimestampProvider, TimestampProvider>();

            services.AddSingleton<IIdentifierService, IdentifierService>()
                    .Configure<IdentifierServiceConfiguration>(_configuration.GetSection(nameof(IdentifierService)));

            services.AddScoped<IFileService, FileService>()
                    .Configure<FileServiceConfiguration>(_configuration.GetSection(nameof(FileService)));

            services.AddScoped<ISchedulerService, SchedulerService>();

            services.AddHostedService<DeletionService>()
                    .Configure<DeletionServiceConfiguration>(_configuration.GetSection(nameof(DeletionService)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vector.Share");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
