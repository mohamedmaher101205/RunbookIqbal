using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Runbook.API
{
    /// <summary>
    /// This class is to run and start the runbook application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// this method is to run and start the runbook application
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// create host for application
        /// </summary>
        /// <param name="args"></param>
        /// <returns>None</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
