using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CodeChallenge.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                   .ReadFrom.Configuration(configuration)
                   .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CodeChallengeConnection")));
            services.AddRepositoriesServices();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddSerilog();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
    internal static class ServicesExtensions {
        internal static void AddRepositoriesServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var assemblyDefinedType in assembly.DefinedTypes)
            {
                if (assemblyDefinedType.IsClass && assemblyDefinedType.FullName.EndsWith("Repository") && !assemblyDefinedType.FullName.EndsWith("GenericRepository"))
                {
                    var implementType = assemblyDefinedType.AsType();
                    var interfaceType = implementType.GetInterface("I" + implementType.Name);
                    if ((interfaceType != null))
                    {
                        services.AddScoped(interfaceType, implementType);
                    }

                }
            }
        }
    }

}
