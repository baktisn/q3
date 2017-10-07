using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApplication3.Data;

using Microsoft.Extensions.DependencyInjection;


/// <summary>
/// testing github greg
/// </summary>
/// 


/// 
/// test 2 greg
/// 
namespace WebApplication3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);


            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<MunicContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
         WebHost.CreateDefaultBuilder(args)
             .UseStartup<Startup>()
             .Build();
    }

     
    }
